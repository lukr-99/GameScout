using System.Globalization;
using System.Text.Json;
using GameScout.Core.Abstractions;
using GameScout.Core.Games;

namespace GameScout.Core.Sources.CheapShark;

/// <summary>
/// Reads the public CheapShark deals feed for popular, currently-discounted (but not free) games
/// across many storefronts, with cover images and normal/sale prices.
/// </summary>
public sealed class CheapSharkSource : IDealSource
{
    /// <summary>Top deals ordered by CheapShark's "Deal Rating" (popularity/value blend).</summary>
    public const string DefaultEndpoint =
        "https://www.cheapshark.com/api/1.0/deals?sortBy=Deal%20Rating&pageSize=60&onSale=1";

    private const string RedirectBaseUrl = "https://www.cheapshark.com/redirect?dealID=";
    private const string SteamCdnBase = "https://cdn.cloudflare.steamstatic.com/steam/apps/";

    private readonly IHttpTextClient _http;
    private readonly string _endpoint;

    /// <summary>Initializes a new <see cref="CheapSharkSource"/>.</summary>
    /// <param name="http">Transport used to fetch the feed.</param>
    /// <param name="endpoint">Feed URL; defaults to <see cref="DefaultEndpoint"/>.</param>
    public CheapSharkSource(IHttpTextClient http, string? endpoint = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _endpoint = endpoint ?? DefaultEndpoint;
    }

    /// <inheritdoc/>
    public string Name => "CheapShark";

    /// <inheritdoc/>
    public async Task<IReadOnlyList<GameDeal>> GetDealsAsync(CancellationToken cancellationToken = default)
    {
        string json = await _http.GetStringAsync(_endpoint, cancellationToken).ConfigureAwait(false);
        return Parse(json);
    }

    /// <summary>Parses a raw CheapShark deals array into <see cref="GameDeal"/> values. Exposed for testing.</summary>
    /// <param name="json">The raw JSON array from the deals endpoint.</param>
    /// <returns>The mapped deals; entries missing a title are skipped.</returns>
    public static IReadOnlyList<GameDeal> Parse(string json)
    {
        List<CheapSharkDealDto>? entries = JsonSerializer.Deserialize<List<CheapSharkDealDto>>(json);
        if (entries is null)
            return [];

        List<GameDeal> deals = [];
        foreach (CheapSharkDealDto entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Title))
                continue;

            deals.Add(new GameDeal(
                Title: entry.Title,
                Store: CheapSharkStores.Map(entry.StoreId),
                SalePrice: FormatPrice(entry.SalePrice),
                NormalPrice: FormatPrice(entry.NormalPrice),
                DiscountPercent: ParsePercent(entry.Savings),
                Url: entry.DealId is { Length: > 0 } id ? RedirectBaseUrl + id : null,
                ImageUrl: ImageFor(entry),
                SaleAmount: ParseAmount(entry.SalePrice)));
        }

        return deals;
    }

    private static string FormatPrice(string? raw)
        => ParseAmount(raw) is { } amount ? amount.ToString("C2", CultureInfo.GetCultureInfo("en-US")) : (raw ?? "—");

    private static decimal? ParseAmount(string? raw)
        => decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) ? value : null;

    private static int ParsePercent(string? savings)
        => decimal.TryParse(savings, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value)
            ? (int)Math.Round(value)
            : 0;

    private static string? ImageFor(CheapSharkDealDto entry)
    {
        if (!string.IsNullOrWhiteSpace(entry.SteamAppId) && entry.SteamAppId != "0")
            return $"{SteamCdnBase}{entry.SteamAppId}/header.jpg";
        return string.IsNullOrWhiteSpace(entry.Thumb) ? null : entry.Thumb;
    }
}
