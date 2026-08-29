using System.Globalization;
using System.Text.Json;
using GameScout.Core.Abstractions;
using GameScout.Core.Games;

namespace GameScout.Core.Sources.GamerPower;

/// <summary>
/// Reads the public GamerPower giveaways feed to surface "normally paid, currently free" games
/// across many storefronts (Steam, GOG, Prime Gaming, itch.io, ...). Free-to-play titles (no real
/// list price) are dropped, and Epic entries are skipped because the dedicated Epic source is
/// authoritative for them.
/// </summary>
public sealed class GamerPowerSource : IGiveawaySource
{
    /// <summary>All full-game giveaways across platforms, most recent first.</summary>
    public const string AllGamesEndpoint =
        "https://www.gamerpower.com/api/giveaways?type=game";

    // GamerPower stamps dates as "yyyy-MM-dd HH:mm:ss" in UTC.
    private const string EndDateFormat = "yyyy-MM-dd HH:mm:ss";

    private readonly IHttpTextClient _http;
    private readonly string _endpoint;

    /// <summary>Initializes a new <see cref="GamerPowerSource"/>.</summary>
    /// <param name="http">Transport used to fetch the feed.</param>
    /// <param name="endpoint">Feed URL; defaults to <see cref="AllGamesEndpoint"/>.</param>
    public GamerPowerSource(IHttpTextClient http, string? endpoint = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _endpoint = endpoint ?? AllGamesEndpoint;
    }

    /// <inheritdoc/>
    public string Name => "GamerPower";

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FreeGame>> GetFreeGamesAsync(CancellationToken cancellationToken = default)
    {
        string json = await _http.GetStringAsync(_endpoint, cancellationToken).ConfigureAwait(false);
        return Parse(json);
    }

    /// <summary>
    /// Parses a raw GamerPower giveaways array into paid-game-now-free offers. Exposed for testing.
    /// </summary>
    /// <param name="json">The raw JSON array from the giveaways endpoint.</param>
    /// <returns>Offers with a real list price; free-to-play and Epic entries are dropped.</returns>
    public static IReadOnlyList<FreeGame> Parse(string json)
    {
        List<GamerPowerGiveawayDto>? entries =
            JsonSerializer.Deserialize<List<GamerPowerGiveawayDto>>(json);
        if (entries is null)
            return [];

        List<FreeGame> games = [];
        foreach (GamerPowerGiveawayDto entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Title) || !HasRealPrice(entry.Worth))
                continue;

            GameStore store = DetectStore(entry.Platforms);
            if (store == GameStore.Epic)
                continue; // The dedicated Epic source is authoritative; avoid duplicates.

            games.Add(new FreeGame(
                Title: CleanTitle(entry.Title),
                Store: store,
                Kind: GiveawayKind.CurrentlyFree,
                Url: entry.OpenGiveawayUrl,
                NormalPrice: entry.Worth,
                StartsUtc: null,
                EndsUtc: ParseEndDate(entry.EndDate),
                ImageUrl: entry.Thumbnail ?? entry.Image));
        }

        return games;
    }

    private static bool HasRealPrice(string? worth)
    {
        if (string.IsNullOrWhiteSpace(worth) ||
            worth.Equals("N/A", StringComparison.OrdinalIgnoreCase))
            return false;

        string digits = new(worth.Where(c => char.IsDigit(c) || c == '.').ToArray());
        return decimal.TryParse(digits, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount)
            && amount > 0m;
    }

    private static GameStore DetectStore(string? platforms)
    {
        if (string.IsNullOrWhiteSpace(platforms))
            return GameStore.Unknown;

        bool Has(string token) => platforms.Contains(token, StringComparison.OrdinalIgnoreCase);

        if (Has("Steam")) return GameStore.Steam;
        if (Has("Epic")) return GameStore.Epic;
        if (Has("GOG")) return GameStore.Gog;
        if (Has("Prime")) return GameStore.PrimeGaming;
        if (Has("itch")) return GameStore.Itch;
        if (Has("Ubisoft") || Has("Uplay")) return GameStore.Ubisoft;
        if (Has("Origin") || Has("EA ")) return GameStore.Origin;
        if (Has("Fanatical")) return GameStore.Fanatical;
        if (Has("IndieGala")) return GameStore.IndieGala;
        if (Has("Humble")) return GameStore.Humble;
        if (Has("Xbox") || Has("Microsoft")) return GameStore.Microsoft;
        return GameStore.Other;
    }

    private static DateTimeOffset? ParseEndDate(string? endDate)
    {
        if (string.IsNullOrWhiteSpace(endDate) ||
            endDate.Equals("N/A", StringComparison.OrdinalIgnoreCase))
            return null;

        return DateTimeOffset.TryParseExact(
            endDate, EndDateFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out DateTimeOffset parsed)
            ? parsed
            : null;
    }

    private static string CleanTitle(string title)
    {
        // Drop trailing "Giveaway"/"Key Giveaway" noise and any "(Store)" parenthetical.
        string cleaned = title
            .Replace("Key Giveaway", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Giveaway", string.Empty, StringComparison.OrdinalIgnoreCase);

        int paren = cleaned.IndexOf('(', StringComparison.Ordinal);
        if (paren > 0)
            cleaned = cleaned[..paren];

        return string.Join(' ', cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
}
