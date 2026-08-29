using System.Text.Json;
using GameScout.Core.Abstractions;
using GameScout.Core.Games;

namespace GameScout.Core.Sources.Epic;

/// <summary>
/// Reads the Epic Games Store weekly free-games promotion feed and normalizes it into
/// <see cref="FreeGame"/> offers. Epic marks a free offer with a promotional
/// <c>discountPercentage</c> of <c>0</c> (i.e. zero percent of the price remains).
/// </summary>
public sealed class EpicFreeGamesSource : IGiveawaySource
{
    /// <summary>Default locale used for the feed query and store links.</summary>
    public const string DefaultLocale = "en-US";

    /// <summary>Default country used to scope the promotions query.</summary>
    public const string DefaultCountry = "US";

    private const string PromotionsBaseUrl =
        "https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions";

    /// <summary>The public, unauthenticated promotions endpoint for the default locale/country.</summary>
    public static string DefaultEndpoint => BuildEndpoint(DefaultLocale, DefaultCountry);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpTextClient _http;
    private readonly string _endpoint;
    private readonly string _locale;

    /// <summary>Initializes a new <see cref="EpicFreeGamesSource"/>.</summary>
    /// <param name="http">Transport used to fetch the feed.</param>
    /// <param name="locale">Locale for the query and store links (e.g. "en-US", "de-DE").</param>
    /// <param name="country">Country code scoping the promotions query (e.g. "US", "DE").</param>
    /// <param name="endpointOverride">Full feed URL override; when set, <paramref name="country"/> is ignored.</param>
    public EpicFreeGamesSource(
        IHttpTextClient http,
        string locale = DefaultLocale,
        string country = DefaultCountry,
        string? endpointOverride = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _locale = string.IsNullOrWhiteSpace(locale) ? DefaultLocale : locale;
        _endpoint = endpointOverride ?? BuildEndpoint(_locale, country);
    }

    /// <inheritdoc/>
    public string Name => "Epic Games Store";

    /// <summary>Builds the promotions endpoint URL for a given locale and country.</summary>
    /// <param name="locale">Locale code (e.g. "en-US").</param>
    /// <param name="country">Country code (e.g. "US").</param>
    /// <returns>The fully-qualified query URL.</returns>
    public static string BuildEndpoint(string locale, string country)
    {
        string safeLocale = string.IsNullOrWhiteSpace(locale) ? DefaultLocale : locale;
        string safeCountry = string.IsNullOrWhiteSpace(country) ? DefaultCountry : country;
        return $"{PromotionsBaseUrl}?locale={Uri.EscapeDataString(safeLocale)}" +
               $"&country={Uri.EscapeDataString(safeCountry)}" +
               $"&allowCountries={Uri.EscapeDataString(safeCountry)}";
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FreeGame>> GetFreeGamesAsync(CancellationToken cancellationToken = default)
    {
        string json = await _http.GetStringAsync(_endpoint, cancellationToken).ConfigureAwait(false);
        return Parse(json, _locale);
    }

    /// <summary>
    /// Parses a raw Epic promotions payload into free/upcoming offers. Exposed for testing so the
    /// mapping can be verified against a fixed sample without any network access.
    /// </summary>
    /// <param name="json">The raw JSON body from the promotions endpoint.</param>
    /// <param name="locale">Locale used to build store links; defaults to <see cref="DefaultLocale"/>.</param>
    /// <returns>The free and upcoming-free offers found in the payload.</returns>
    public static IReadOnlyList<FreeGame> Parse(string json, string locale = DefaultLocale)
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

            if (TryMap(element, element.Promotions?.PromotionalOffers, GiveawayKind.CurrentlyFree, locale, out FreeGame? current))
                games.Add(current!);
            else if (TryMap(element, element.Promotions?.UpcomingPromotionalOffers, GiveawayKind.Upcoming, locale, out FreeGame? upcoming))
                games.Add(upcoming!);
        }

        return games;
    }

    private static bool TryMap(
        EpicElementDto element,
        IReadOnlyList<EpicOfferGroupDto>? groups,
        GiveawayKind kind,
        string locale,
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
                    Url: BuildUrl(element, locale),
                    NormalPrice: element.Price?.TotalPrice?.FmtPrice?.OriginalPrice,
                    StartsUtc: offer.StartDate,
                    EndsUtc: offer.EndDate);
                return true;
            }
        }

        return false;
    }

    private static string? BuildUrl(EpicElementDto element, string locale)
    {
        string? slug = FirstUsableSlug(element);
        return slug is null ? null : $"https://store.epicgames.com/{locale}/p/{slug}";
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
