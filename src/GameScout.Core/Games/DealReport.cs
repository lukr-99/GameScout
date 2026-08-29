using System.Collections.ObjectModel;

namespace GameScout.Core.Games;

/// <summary>The aggregated result of a deals scan: the deals plus any per-source errors.</summary>
public sealed class DealReport
{
    /// <summary>Initializes a new <see cref="DealReport"/>.</summary>
    /// <param name="deals">The normalized, de-duplicated deals, already ordered for display.</param>
    /// <param name="errors">Human-readable messages for sources that failed.</param>
    /// <param name="generatedUtc">When the scan completed (UTC).</param>
    public DealReport(
        IReadOnlyList<GameDeal> deals,
        IReadOnlyList<string> errors,
        DateTimeOffset generatedUtc)
    {
        Deals = new ReadOnlyCollection<GameDeal>([.. deals]);
        Errors = new ReadOnlyCollection<string>([.. errors]);
        GeneratedUtc = generatedUtc;
    }

    /// <summary>The deals found, ordered by discount (deepest first).</summary>
    public IReadOnlyList<GameDeal> Deals { get; }

    /// <summary>Messages describing any sources that failed during the scan.</summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>When the scan completed (UTC).</summary>
    public DateTimeOffset GeneratedUtc { get; }
}
