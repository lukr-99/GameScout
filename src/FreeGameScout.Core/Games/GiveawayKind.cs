namespace FreeGameScout.Core.Games;

/// <summary>Whether a free offer is claimable now or scheduled to become free later.</summary>
public enum GiveawayKind
{
    /// <summary>The game is free to claim right now.</summary>
    CurrentlyFree = 0,

    /// <summary>The game is announced to become free in the future.</summary>
    Upcoming,
}
