using System.Text.Json.Serialization;

namespace GameScout.Core.Sources.CheapShark;

/// <summary>One entry from the CheapShark deals feed (subset of fields consumed).</summary>
internal sealed class CheapSharkDealDto
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("dealID")]
    public string? DealId { get; init; }

    [JsonPropertyName("storeID")]
    public string? StoreId { get; init; }

    [JsonPropertyName("salePrice")]
    public string? SalePrice { get; init; }

    [JsonPropertyName("normalPrice")]
    public string? NormalPrice { get; init; }

    [JsonPropertyName("savings")]
    public string? Savings { get; init; }

    [JsonPropertyName("steamAppID")]
    public string? SteamAppId { get; init; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; init; }
}
