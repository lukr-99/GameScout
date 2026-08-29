namespace GameScout.Core.Games;

/// <summary>The storefront a free game offer originates from.</summary>
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

    /// <summary>Any other storefront reported by an aggregated source.</summary>
    Other,
}
