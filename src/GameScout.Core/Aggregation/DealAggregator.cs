using GameScout.Core.Abstractions;
using GameScout.Core.Games;

namespace GameScout.Core.Aggregation;

/// <summary>
/// Fans out to every configured <see cref="IDealSource"/>, merges the results into a single
/// de-duplicated list ordered by discount depth, and caps it for display. A failing source degrades
/// to an error message rather than failing the whole scan.
/// </summary>
public sealed class DealAggregator
{
    private const int MaxDeals = 60;

    private readonly IReadOnlyList<IDealSource> _sources;
    private readonly TimeProvider _time;

    /// <summary>Initializes a new <see cref="DealAggregator"/>.</summary>
    /// <param name="sources">The deal sources to query.</param>
    /// <param name="time">Clock used for the report timestamp; defaults to the system clock.</param>
    public DealAggregator(IEnumerable<IDealSource> sources, TimeProvider? time = null)
    {
        ArgumentNullException.ThrowIfNull(sources);
        _sources = [.. sources];
        _time = time ?? TimeProvider.System;
    }

    /// <summary>Queries all sources concurrently and merges their deals.</summary>
    /// <param name="cancellationToken">Token used to cancel the scan.</param>
    /// <returns>The merged report, including any per-source errors.</returns>
    public async Task<DealReport> ScanAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<GameDeal>[] results = await Task.WhenAll(
            _sources.Select(source => SafeFetchAsync(source, cancellationToken))).ConfigureAwait(false);

        List<string> errors = [];
        List<GameDeal> merged = [];
        for (int i = 0; i < _sources.Count; i++)
        {
            if (results[i] is null)
                errors.Add($"{_sources[i].Name}: could not be reached.");
            else
                merged.AddRange(results[i]);
        }

        return new DealReport(Normalize(merged), errors, _time.GetUtcNow());
    }

    private static async Task<IReadOnlyList<GameDeal>> SafeFetchAsync(
        IDealSource source, CancellationToken cancellationToken)
    {
        try
        {
            return await source.GetDealsAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return null!;
        }
    }

    private static IReadOnlyList<GameDeal> Normalize(IEnumerable<GameDeal> deals)
    {
        // Sources return one row per storefront; collapse to one entry per title, keeping the
        // deepest discount so the same game doesn't flood the list.
        HashSet<string> seen = [];
        List<GameDeal> deduped = [];
        foreach (GameDeal deal in deals.OrderByDescending(d => d.DiscountPercent))
        {
            if (seen.Add(deal.Title.Trim().ToLowerInvariant()))
                deduped.Add(deal);
        }

        return [.. deduped.Take(MaxDeals)];
    }
}
