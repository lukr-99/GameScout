using GameScout.Core.Games;

namespace GameScout.Core.Abstractions;

/// <summary>
/// A source of discounted (but not free) game deals. Implementations must not throw for expected
/// "nothing found" cases; they return an empty list instead.
/// </summary>
public interface IDealSource
{
    /// <summary>A short, stable name used in error messages and logs (e.g. "CheapShark").</summary>
    string Name { get; }

    /// <summary>Fetches the current set of deals this source knows about.</summary>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The deals found; empty when there are none.</returns>
    Task<IReadOnlyList<GameDeal>> GetDealsAsync(CancellationToken cancellationToken = default);
}
