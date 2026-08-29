using GameScout.Core.Abstractions;
using GameScout.Core.Games;

namespace GameScout.Core.Aggregation;

/// <summary>
/// Fans out to every configured <see cref="IGiveawaySource"/>, then merges the results into a
/// single de-duplicated, display-ordered <see cref="FreeGameReport"/>. A failing source degrades
/// to an error message rather than failing the whole scan.
/// </summary>
public sealed class GiveawayAggregator
{
    private readonly IReadOnlyList<IGiveawaySource> _sources;
    private readonly TimeProvider _time;
    private readonly decimal _minimumWorth;

    /// <summary>Initializes a new <see cref="GiveawayAggregator"/>.</summary>
    /// <param name="sources">The sources to query.</param>
    /// <param name="time">Clock used for relevance filtering; defaults to the system clock.</param>
    /// <param name="minimumWorth">
    /// Drops giveaways whose known normal price is below this value, to filter out trivial freebies.
    /// Offers with an unknown price are always kept. Defaults to 0 (no filtering).
    /// </param>
    public GiveawayAggregator(
        IEnumerable<IGiveawaySource> sources,
        TimeProvider? time = null,
        decimal minimumWorth = 0m)
    {
        ArgumentNullException.ThrowIfNull(sources);
        _sources = [.. sources];
        _time = time ?? TimeProvider.System;
        _minimumWorth = minimumWorth;
    }

    /// <summary>Queries all sources concurrently and merges their offers.</summary>
    /// <param name="cancellationToken">Token used to cancel the scan.</param>
    /// <returns>The merged report, including any per-source errors.</returns>
    public async Task<FreeGameReport> ScanAsync(CancellationToken cancellationToken = default)
    {
        DateTimeOffset now = _time.GetUtcNow();

        IReadOnlyList<FreeGame>[] results = await Task.WhenAll(
            _sources.Select(source => SafeFetchAsync(source, cancellationToken))).ConfigureAwait(false);

        List<string> errors = [];
        List<FreeGame> merged = [];
        for (int i = 0; i < _sources.Count; i++)
        {
            if (results[i] is null)
                errors.Add($"{_sources[i].Name}: could not be reached.");
            else
                merged.AddRange(results[i]);
        }

        IReadOnlyList<FreeGame> ordered = Normalize(merged, now);
        return new FreeGameReport(ordered, errors, now);
    }

    // Keep offers whose worth is unknown or at/above the configured minimum.
    private bool MeetsWorth(FreeGame game)
        => _minimumWorth <= 0m || game.WorthAmount is null || game.WorthAmount.Value >= _minimumWorth;

    private static async Task<IReadOnlyList<FreeGame>> SafeFetchAsync(
        IGiveawaySource source, CancellationToken cancellationToken)
    {
        try
        {
            return await source.GetFreeGamesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // Signal failure to the caller via a null sentinel so it can record a per-source error.
            return null!;
        }
    }

    private IReadOnlyList<FreeGame> Normalize(IEnumerable<FreeGame> games, DateTimeOffset now)
    {
        HashSet<(string, GameStore, GiveawayKind)> seen = [];
        List<FreeGame> deduped = [];
        foreach (FreeGame game in games)
        {
            if (!game.IsRelevantAt(now) || !MeetsWorth(game))
                continue;

            (string, GameStore, GiveawayKind) key =
                (game.Title.Trim().ToLowerInvariant(), game.Store, game.Kind);
            if (seen.Add(key))
                deduped.Add(game);
        }

        return
        [
            .. deduped
                .OrderBy(g => g.Kind)          // CurrentlyFree (0) before Upcoming (1)
                .ThenBy(g => g.Store)          // Epic, Steam, GOG, ...
                .ThenBy(g => g.Title, StringComparer.OrdinalIgnoreCase),
        ];
    }
}
