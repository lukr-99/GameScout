using System.Text.Json.Serialization;

namespace GameScout.Core.Sources.Epic;

// DTOs mirroring the subset of the Epic "freeGamesPromotions" payload we consume.
// Only the fields the parser reads are declared; everything else is ignored.

internal sealed class EpicPromotionsDto
{
    [JsonPropertyName("data")]
    public EpicDataDto? Data { get; init; }
}

internal sealed class EpicDataDto
{
    [JsonPropertyName("Catalog")]
    public EpicCatalogDto? Catalog { get; init; }
}

internal sealed class EpicCatalogDto
{
    [JsonPropertyName("searchStore")]
    public EpicSearchStoreDto? SearchStore { get; init; }
}

internal sealed class EpicSearchStoreDto
{
    [JsonPropertyName("elements")]
    public IReadOnlyList<EpicElementDto>? Elements { get; init; }
}

internal sealed class EpicElementDto
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("productSlug")]
    public string? ProductSlug { get; init; }

    [JsonPropertyName("urlSlug")]
    public string? UrlSlug { get; init; }

    [JsonPropertyName("offerMappings")]
    public IReadOnlyList<EpicSlugMappingDto>? OfferMappings { get; init; }

    [JsonPropertyName("catalogNs")]
    public EpicCatalogNsDto? CatalogNs { get; init; }

    [JsonPropertyName("price")]
    public EpicPriceDto? Price { get; init; }

    [JsonPropertyName("promotions")]
    public EpicPromotionsBlockDto? Promotions { get; init; }
}

internal sealed class EpicCatalogNsDto
{
    [JsonPropertyName("mappings")]
    public IReadOnlyList<EpicSlugMappingDto>? Mappings { get; init; }
}

internal sealed class EpicSlugMappingDto
{
    [JsonPropertyName("pageSlug")]
    public string? PageSlug { get; init; }
}

internal sealed class EpicPriceDto
{
    [JsonPropertyName("totalPrice")]
    public EpicTotalPriceDto? TotalPrice { get; init; }
}

internal sealed class EpicTotalPriceDto
{
    [JsonPropertyName("originalPrice")]
    public long OriginalPrice { get; init; }

    [JsonPropertyName("discountPrice")]
    public long DiscountPrice { get; init; }

    [JsonPropertyName("fmtPrice")]
    public EpicFmtPriceDto? FmtPrice { get; init; }
}

internal sealed class EpicFmtPriceDto
{
    [JsonPropertyName("originalPrice")]
    public string? OriginalPrice { get; init; }
}

internal sealed class EpicPromotionsBlockDto
{
    [JsonPropertyName("promotionalOffers")]
    public IReadOnlyList<EpicOfferGroupDto>? PromotionalOffers { get; init; }

    [JsonPropertyName("upcomingPromotionalOffers")]
    public IReadOnlyList<EpicOfferGroupDto>? UpcomingPromotionalOffers { get; init; }
}

internal sealed class EpicOfferGroupDto
{
    [JsonPropertyName("promotionalOffers")]
    public IReadOnlyList<EpicOfferDto>? PromotionalOffers { get; init; }
}

internal sealed class EpicOfferDto
{
    [JsonPropertyName("startDate")]
    public DateTimeOffset? StartDate { get; init; }

    [JsonPropertyName("endDate")]
    public DateTimeOffset? EndDate { get; init; }

    [JsonPropertyName("discountSetting")]
    public EpicDiscountSettingDto? DiscountSetting { get; init; }
}

internal sealed class EpicDiscountSettingDto
{
    [JsonPropertyName("discountPercentage")]
    public int? DiscountPercentage { get; init; }
}
