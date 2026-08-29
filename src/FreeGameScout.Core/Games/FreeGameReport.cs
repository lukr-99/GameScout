using System.Collections.ObjectModel;

namespace FreeGameScout.Core.Games;

/// <summary>
/// The aggregated result of a single scan: the games found plus any per-source
/// errors so the UI can show a partial result instead of failing outright.
/// </summary>
public sealed class FreeGameReport
{
    /// <summary>Initializes a new <see cref="FreeGameReport"/>.</summary>
    /// <param name="games">The normalized, de-duplicated games, already ordered for display.</param>
    /// <param name="errors">Human-readable messages for sources that failed.</param>
    /// <param name="generatedUtc">When the scan completed (UTC).</param>
    public FreeGameReport(
        IReadOnlyList<FreeGame> games,
        IReadOnlyList<string> errors,
        DateTimeOffset generatedUtc)
    {
        Games = new ReadOnlyCollection<FreeGame>([.. games]);
        Errors = new ReadOnlyCollection<string>([.. errors]);
        GeneratedUtc = generatedUtc;
    }

    /// <summary>The games found, ordered currently-free first then by store and title.</summary>
    public IReadOnlyList<FreeGame> Games { get; }

    /// <summary>Messages describing any sources that failed during the scan.</summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>When the scan completed (UTC).</summary>
    public DateTimeOffset GeneratedUtc { get; }

    /// <summary>Games that can be claimed right now.</summary>
    public IEnumerable<FreeGame> CurrentlyFree => Games.Where(g => g.Kind == GiveawayKind.CurrentlyFree);

    /// <summary>Games announced to become free later.</summary>
    public IEnumerable<FreeGame> Upcoming => Games.Where(g => g.Kind == GiveawayKind.Upcoming);
}
