using FreeGameScout.Core.Games;

namespace FreeGameScout.Core.Abstractions;

/// <summary>
/// A source of free-game offers (one storefront or one aggregator feed). Implementations
/// must not throw for expected "nothing found" cases; they return an empty list instead.
/// </summary>
public interface IGiveawaySource
{
    /// <summary>A short, stable name used in error messages and logs (e.g. "Epic Games Store").</summary>
    string Name { get; }

    /// <summary>Fetches the current set of free/upcoming offers this source knows about.</summary>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The offers found; empty when there are none.</returns>
    Task<IReadOnlyList<FreeGame>> GetFreeGamesAsync(CancellationToken cancellationToken = default);
}
