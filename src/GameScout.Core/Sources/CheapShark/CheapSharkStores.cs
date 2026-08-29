using GameScout.Core.Games;

namespace GameScout.Core.Sources.CheapShark;

/// <summary>Maps CheapShark numeric store IDs to <see cref="GameStore"/> values.</summary>
internal static class CheapSharkStores
{
    public static GameStore Map(string? storeId) => storeId switch
    {
        "1" => GameStore.Steam,
        "3" => GameStore.GreenManGaming,
        "7" => GameStore.Gog,
        "8" => GameStore.Origin,
        "11" => GameStore.Humble,
        "13" => GameStore.Ubisoft,
        "15" => GameStore.Fanatical,
        "25" => GameStore.Epic,
        "30" => GameStore.IndieGala,
        _ => GameStore.Other,
    };
}
