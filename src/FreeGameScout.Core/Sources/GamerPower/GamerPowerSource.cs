using System.Globalization;
using System.Text.Json;
using FreeGameScout.Core.Abstractions;
using FreeGameScout.Core.Games;

namespace FreeGameScout.Core.Sources.GamerPower;

/// <summary>
/// Reads the public GamerPower giveaways feed to surface "normally paid, currently free"
/// keys on Steam. Free-to-play titles (no real list price) are filtered out so the report
/// only shows genuine give-away-a-paid-game offers.
/// </summary>
public sealed class GamerPowerSource : IGiveawaySource
{
    /// <summary>Steam full-game giveaways, most recent first.</summary>
    public const string SteamGamesEndpoint =
        "https://www.gamerpower.com/api/giveaways?platform=steam&type=game";

    // GamerPower stamps dates as "yyyy-MM-dd HH:mm:ss" in UTC.
    private const string EndDateFormat = "yyyy-MM-dd HH:mm:ss";

    private readonly IHttpTextClient _http;
    private readonly string _endpoint;

    /// <summary>Initializes a new <see cref="GamerPowerSource"/>.</summary>
    /// <param name="http">Transport used to fetch the feed.</param>
    /// <param name="endpoint">Feed URL; defaults to <see cref="SteamGamesEndpoint"/>.</param>
    public GamerPowerSource(IHttpTextClient http, string? endpoint = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _endpoint = endpoint ?? SteamGamesEndpoint;
    }

    /// <inheritdoc/>
    public string Name => "GamerPower (Steam)";

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
    /// <returns>The offers with a real list price; free-to-play entries are dropped.</returns>
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

            games.Add(new FreeGame(
                Title: CleanTitle(entry.Title),
                Store: DetectStore(entry.Platforms),
                Kind: GiveawayKind.CurrentlyFree,
                Url: entry.OpenGiveawayUrl,
                NormalPrice: entry.Worth,
                StartsUtc: null,
                EndsUtc: ParseEndDate(entry.EndDate)));
        }

        return games;
    }

    private static bool HasRealPrice(string? worth)
    {
        if (string.IsNullOrWhiteSpace(worth) ||
            worth.Equals("N/A", StringComparison.OrdinalIgnoreCase))
            return false;

        // Strip currency/formatting and check the amount is above zero.
        string digits = new(worth.Where(c => char.IsDigit(c) || c == '.').ToArray());
        return decimal.TryParse(digits, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount)
            && amount > 0m;
    }

    private static GameStore DetectStore(string? platforms)
    {
        if (string.IsNullOrWhiteSpace(platforms))
            return GameStore.Unknown;
        if (platforms.Contains("Steam", StringComparison.OrdinalIgnoreCase))
            return GameStore.Steam;
        if (platforms.Contains("Epic", StringComparison.OrdinalIgnoreCase))
            return GameStore.Epic;
        if (platforms.Contains("GOG", StringComparison.OrdinalIgnoreCase))
            return GameStore.Gog;
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
        string cleaned = title
            .Replace("(Steam)", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Key Giveaway", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Giveaway", string.Empty, StringComparison.OrdinalIgnoreCase);
        return string.Join(' ', cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
}
