using System.Text.Json.Serialization;

namespace GameScout.Core.Sources.GamerPower;

/// <summary>One entry from the GamerPower giveaways feed (subset of fields consumed).</summary>
internal sealed class GamerPowerGiveawayDto
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("worth")]
    public string? Worth { get; init; }

    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; init; }

    [JsonPropertyName("image")]
    public string? Image { get; init; }

    [JsonPropertyName("open_giveaway_url")]
    public string? OpenGiveawayUrl { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("platforms")]
    public string? Platforms { get; init; }

    [JsonPropertyName("end_date")]
    public string? EndDate { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
