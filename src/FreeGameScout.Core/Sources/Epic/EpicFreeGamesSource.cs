using System.Text.Json;
using FreeGameScout.Core.Abstractions;
using FreeGameScout.Core.Games;

namespace FreeGameScout.Core.Sources.Epic;

/// <summary>
/// Reads the Epic Games Store weekly free-games promotion feed and normalizes it into
/// <see cref="FreeGame"/> offers. Epic marks a free offer with a promotional
/// <c>discountPercentage</c> of <c>0</c> (i.e. zero percent of the price remains).
/// </summary>
public sealed class EpicFreeGamesSource : IGiveawaySource
{
    /// <summary>The public, unauthenticated promotions endpoint.</summary>
    public const string DefaultEndpoint =
        "https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions" +
        "?locale=en-US&country=US&allowCountries=US";

    private const string StoreBaseUrl = "https://store.epicgames.com/en-US/p/";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpTextClient _http;
    private readonly string _endpoint;

    /// <summary>Initializes a new <see cref="EpicFreeGamesSource"/>.</summary>
    /// <param name="http">Transport used to fetch the feed.</param>
    /// <param name="endpoint">Feed URL; defaults to <see cref="DefaultEndpoint"/>.</param>
    public EpicFreeGamesSource(IHttpTextClient http, string? endpoint = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _endpoint = endpoint ?? DefaultEndpoint;
    }

    /// <inheritdoc/>
    public string Name => "Epic Games Store";

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FreeGame>> GetFreeGamesAsync(CancellationToken cancellationToken = default)
    {
        string json = await _http.GetStringAsync(_endpoint, cancellationToken).ConfigureAwait(false);
        return Parse(json);
    }

    /// <summary>
    /// Parses a raw Epic promotions payload into free/upcoming offers. Exposed for testing so the
    /// mapping can be verified against a fixed sample without any network access.
    /// </summary>
    /// <param name="json">The raw JSON body from the promotions endpoint.</param>
    /// <returns>The free and upcoming-free offers found in the payload.</returns>
    public static IReadOnlyList<FreeGame> Parse(string json)
    {
        EpicPromotionsDto? dto = JsonSerializer.Deserialize<EpicPromotionsDto>(json, JsonOptions);
        IReadOnlyList<EpicElementDto>? elements = dto?.Data?.Catalog?.SearchStore?.Elements;
        if (elements is null)
            return [];

        List<FreeGame> games = [];
        foreach (EpicElementDto element in elements)
        {
            // Skip perpetually free-to-play entries: a real giveaway has a non-zero list price.
            if ((element.Price?.TotalPrice?.OriginalPrice ?? 0) <= 0)
                continue;

            if (TryMap(element, element.Promotions?.PromotionalOffers, GiveawayKind.CurrentlyFree, out FreeGame? current))
                games.Add(current!);
            else if (TryMap(element, element.Promotions?.UpcomingPromotionalOffers, GiveawayKind.Upcoming, out FreeGame? upcoming))
                games.Add(upcoming!);
        }

        return games;
    }

    private static bool TryMap(
        EpicElementDto element,
        IReadOnlyList<EpicOfferGroupDto>? groups,
        GiveawayKind kind,
        out FreeGame? game)
    {
        game = null;
        if (groups is null)
            return false;

        foreach (EpicOfferGroupDto group in groups)
        {
            if (group.PromotionalOffers is null)
                continue;

            foreach (EpicOfferDto offer in group.PromotionalOffers)
            {
                if (offer.DiscountSetting?.DiscountPercentage != 0)
                    continue;

                game = new FreeGame(
                    Title: element.Title ?? "(untitled)",
                    Store: GameStore.Epic,
                    Kind: kind,
                    Url: BuildUrl(element),
                    NormalPrice: element.Price?.TotalPrice?.FmtPrice?.OriginalPrice,
                    StartsUtc: offer.StartDate,
                    EndsUtc: offer.EndDate);
                return true;
            }
        }

        return false;
    }

    private static string? BuildUrl(EpicElementDto element)
    {
        string? slug = FirstUsableSlug(element);
        return slug is null ? null : StoreBaseUrl + slug;
    }

    private static string? FirstUsableSlug(EpicElementDto element)
    {
        string? product = Normalize(element.ProductSlug);
        if (product is not null)
            return product;

        string? offerMapping = Normalize(FirstSlug(element.OfferMappings));
        if (offerMapping is not null)
            return offerMapping;

        string? catalogMapping = Normalize(FirstSlug(element.CatalogNs?.Mappings));
        if (catalogMapping is not null)
            return catalogMapping;

        return Normalize(element.UrlSlug);
    }

    private static string? FirstSlug(IReadOnlyList<EpicSlugMappingDto>? mappings)
        => mappings is { Count: > 0 } ? mappings[0].PageSlug : null;

    private static string? Normalize(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        // Epic sometimes suffixes product slugs with "/home"; the store page lives at the bare slug.
        const string homeSuffix = "/home";
        return slug.EndsWith(homeSuffix, StringComparison.OrdinalIgnoreCase)
            ? slug[..^homeSuffix.Length]
            : slug;
    }
}
