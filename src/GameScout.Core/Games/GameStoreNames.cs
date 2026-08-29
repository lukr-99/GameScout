namespace GameScout.Core.Games;

/// <summary>Human-friendly display names for <see cref="GameStore"/> values.</summary>
public static class GameStoreNames
{
    /// <summary>Returns a short display label for a storefront (e.g. "Epic", "GOG").</summary>
    public static string DisplayName(this GameStore store) => store switch
    {
        GameStore.Epic => "Epic",
        GameStore.Steam => "Steam",
        GameStore.Gog => "GOG",
        GameStore.Humble => "Humble",
        GameStore.Fanatical => "Fanatical",
        GameStore.GreenManGaming => "GMG",
        GameStore.Ubisoft => "Ubisoft",
        GameStore.Origin => "EA",
        GameStore.IndieGala => "IndieGala",
        GameStore.Itch => "itch.io",
        GameStore.PrimeGaming => "Prime",
        GameStore.Microsoft => "Xbox",
        _ => "Store",
    };
}
