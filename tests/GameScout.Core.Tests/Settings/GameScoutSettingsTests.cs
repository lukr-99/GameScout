using GameScout.Core.Settings;

namespace GameScout.Core.Tests.Settings;

public sealed class GameScoutSettingsTests
{
    [Fact]
    public void TryCreate_ValidValues_NormalizesLocaleAndCountry()
    {
        bool valid = GameScoutSettings.TryCreate(
            " cs-cz ",
            " cz ",
            4.50m,
            out GameScoutSettings? settings,
            out string? error);

        Assert.True(valid);
        Assert.Null(error);
        Assert.NotNull(settings);
        Assert.Equal("cs-CZ", settings.Locale);
        Assert.Equal("CZ", settings.Country);
        Assert.Equal(4.50m, settings.MinimumWorth);
    }

    [Theory]
    [InlineData("not-a-locale", "US", 2.99, "locale")]
    [InlineData("en-US", "USA", 2.99, "Country")]
    [InlineData("en-US", "US", -0.01, "negative")]
    public void TryCreate_InvalidValue_ReturnsError(
        string locale,
        string country,
        decimal minimumWorth,
        string errorFragment)
    {
        bool valid = GameScoutSettings.TryCreate(
            locale,
            country,
            minimumWorth,
            out GameScoutSettings? settings,
            out string? error);

        Assert.False(valid);
        Assert.Null(settings);
        Assert.Contains(errorFragment, error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Save_ThenLoad_RoundTripsNormalizedSettings()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"GameScout-settings-{Guid.NewGuid():N}.json");
        try
        {
            var store = new JsonGameScoutSettingsStore(filePath);
            store.Save(new GameScoutSettings("de-de", "de", 6.75m));

            GameScoutSettings loaded = store.Load();

            Assert.Equal("de-DE", loaded.Locale);
            Assert.Equal("DE", loaded.Country);
            Assert.Equal(6.75m, loaded.MinimumWorth);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void Load_MalformedJson_ReturnsDefaults()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"GameScout-settings-{Guid.NewGuid():N}.json");
        try
        {
            File.WriteAllText(filePath, "{not json}");
            var store = new JsonGameScoutSettingsStore(filePath);

            Assert.Equal(GameScoutSettings.Default, store.Load());
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}
