namespace GameScout.Core.Games;

/// <summary>The storefront a free game or deal originates from.</summary>
public enum GameStore
{
    /// <summary>Storefront could not be determined.</summary>
    Unknown = 0,

    /// <summary>Epic Games Store.</summary>
    Epic,

    /// <summary>Steam.</summary>
    Steam,

    /// <summary>GOG.com.</summary>
    Gog,

    /// <summary>Humble Store.</summary>
    Humble,

    /// <summary>Fanatical.</summary>
    Fanatical,

    /// <summary>Green Man Gaming.</summary>
    GreenManGaming,

    /// <summary>Ubisoft Store / Uplay.</summary>
    Ubisoft,

    /// <summary>EA / Origin.</summary>
    Origin,

    /// <summary>IndieGala.</summary>
    IndieGala,

    /// <summary>itch.io.</summary>
    Itch,

    /// <summary>Amazon Prime Gaming.</summary>
    PrimeGaming,

    /// <summary>Microsoft / Xbox store.</summary>
    Microsoft,

    /// <summary>Any other storefront reported by a source.</summary>
    Other,
}
