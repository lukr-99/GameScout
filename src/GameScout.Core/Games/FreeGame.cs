namespace GameScout.Core.Games;

/// <summary>
/// A single "normally paid, currently (or soon) free" game offer, normalized across storefronts.
/// </summary>
/// <param name="Title">Display title of the game.</param>
/// <param name="Store">Storefront the offer belongs to.</param>
/// <param name="Kind">Whether the offer is claimable now or upcoming.</param>
/// <param name="Url">Direct link to claim/view the offer, if known.</param>
/// <param name="NormalPrice">The formatted normal price (e.g. "$24.99"), if known.</param>
/// <param name="StartsUtc">When the free window opens (UTC), if known.</param>
/// <param name="EndsUtc">When the free window closes (UTC), if known.</param>
public sealed record FreeGame(
    string Title,
    GameStore Store,
    GiveawayKind Kind,
    string? Url = null,
    string? NormalPrice = null,
    DateTimeOffset? StartsUtc = null,
    DateTimeOffset? EndsUtc = null)
{
    /// <summary>
    /// Whether this offer is still worth showing at <paramref name="nowUtc"/>: currently-free
    /// offers must not have ended, and upcoming offers must not already be in the past.
    /// </summary>
    public bool IsRelevantAt(DateTimeOffset nowUtc) => Kind switch
    {
        GiveawayKind.CurrentlyFree => EndsUtc is null || EndsUtc.Value > nowUtc,
        GiveawayKind.Upcoming => StartsUtc is null || StartsUtc.Value > nowUtc,
        _ => true,
    };
}
