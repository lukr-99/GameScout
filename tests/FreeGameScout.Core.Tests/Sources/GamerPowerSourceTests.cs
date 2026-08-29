using FreeGameScout.Core.Games;
using FreeGameScout.Core.Sources.GamerPower;
using FreeGameScout.Core.Tests.TestSupport;

namespace FreeGameScout.Core.Tests.Sources;

public sealed class GamerPowerSourceTests
{
    [Fact]
    public void Parse_Sample_KeepsOnlyOffersWithARealPrice()
    {
        IReadOnlyList<FreeGame> games = GamerPowerSource.Parse(Samples.GamerPowerSteam());

        // The "N/A" worth entry is free-to-play and must be filtered out.
        FreeGame only = Assert.Single(games);
        Assert.Equal("$9.99", only.NormalPrice);
    }

    [Fact]
    public void Parse_Sample_CleansTitleAndMapsSteamStore()
    {
        IReadOnlyList<FreeGame> games = GamerPowerSource.Parse(Samples.GamerPowerSteam());

        FreeGame game = games.Single();
        Assert.Equal("Dwarven Realms", game.Title);
        Assert.Equal(GameStore.Steam, game.Store);
        Assert.Equal(GiveawayKind.CurrentlyFree, game.Kind);
    }

    [Fact]
    public void Parse_EmptyArray_ReturnsEmpty()
    {
        IReadOnlyList<FreeGame> games = GamerPowerSource.Parse("[]");

        Assert.Empty(games);
    }
}
