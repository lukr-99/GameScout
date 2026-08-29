using GameScout.Core.Games;
using GameScout.Core.Sources.GamerPower;
using GameScout.Core.Tests.TestSupport;

namespace GameScout.Core.Tests.Sources;

public sealed class GamerPowerAllPlatformsTests
{
    [Fact]
    public void Parse_SkipsEpicAndFreeToPlay()
    {
        IReadOnlyList<FreeGame> games = GamerPowerSource.Parse(Samples.GamerPowerAll());

        // Steam + GOG kept; Epic (covered by the Epic source) and the N/A-worth F2P dropped.
        Assert.Equal(2, games.Count);
        Assert.DoesNotContain(games, g => g.Store == GameStore.Epic);
    }

    [Fact]
    public void Parse_MapsGogStoreAndImage()
    {
        IReadOnlyList<FreeGame> games = GamerPowerSource.Parse(Samples.GamerPowerAll());

        FreeGame gog = games.Single(g => g.Store == GameStore.Gog);
        Assert.Equal("GOG Classic Adventure", gog.Title);
        Assert.Equal("https://www.gamerpower.com/offers/1/gog.jpg", gog.ImageUrl);
    }

    [Fact]
    public void Parse_UsesThumbnailBeforeFullImage()
    {
        IReadOnlyList<FreeGame> games = GamerPowerSource.Parse(Samples.GamerPowerAll());

        FreeGame steam = games.Single(g => g.Store == GameStore.Steam);
        Assert.Equal("https://www.gamerpower.com/offers/1/dwarven.jpg", steam.ImageUrl);
    }
}
