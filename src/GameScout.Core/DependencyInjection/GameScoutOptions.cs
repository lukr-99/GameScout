using GameScout.Core.Sources.Epic;

namespace GameScout.Core.DependencyInjection;

/// <summary>Configuration for the Core services (storefront locale/country).</summary>
public sealed class GameScoutOptions
{
    /// <summary>Locale used for Epic queries and store links (e.g. "en-US").</summary>
    public string Locale { get; set; } = EpicFreeGamesSource.DefaultLocale;

    /// <summary>Country code scoping the Epic promotions query (e.g. "US").</summary>
    public string Country { get; set; } = EpicFreeGamesSource.DefaultCountry;
}
