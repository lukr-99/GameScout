using GameScout.Core.Games;
using GameScout.Core.Sources.Epic;
using GameScout.Core.Tests.TestSupport;

namespace GameScout.Core.Tests.Sources;

public sealed class EpicFreeGamesSourceTests
{
    [Fact]
    public void Parse_Sample_ReturnsTwoCurrentAndOneUpcoming()
    {
        IReadOnlyList<FreeGame> games = EpicFreeGamesSource.Parse(Samples.EpicPromotions());

        Assert.Equal(2, games.Count(g => g.Kind == GiveawayKind.CurrentlyFree));
        Assert.Equal(1, games.Count(g => g.Kind == GiveawayKind.Upcoming));
    }

    [Fact]
    public void Parse_Sample_ExcludesDiscountedButNotFreeOffers()
    {
        IReadOnlyList<FreeGame> games = EpicFreeGamesSource.Parse(Samples.EpicPromotions());

        // Ghostrunner 2 is only 20% off (an upcoming discount, not free) and must be dropped.
        Assert.DoesNotContain(games, g => g.Title.Contains("Ghostrunner", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Parse_Sample_MapsTitlePriceAndStoreUrl()
    {
        IReadOnlyList<FreeGame> games = EpicFreeGamesSource.Parse(Samples.EpicPromotions());

        FreeGame breathedge = games.Single(g => g.Title == "Breathedge");
        Assert.Equal(GameStore.Epic, breathedge.Store);
        Assert.Equal(GiveawayKind.CurrentlyFree, breathedge.Kind);
        Assert.Equal("$24.99", breathedge.NormalPrice);
        Assert.Equal("https://store.epicgames.com/en-US/p/breathedge", breathedge.Url);
        Assert.NotNull(breathedge.EndsUtc);
    }

    [Fact]
    public void Parse_Sample_MapsUpcomingFreeGame()
    {
        IReadOnlyList<FreeGame> games = EpicFreeGamesSource.Parse(Samples.EpicPromotions());

        FreeGame upcoming = games.Single(g => g.Kind == GiveawayKind.Upcoming);
        Assert.Equal("Alone With You", upcoming.Title);
        Assert.NotNull(upcoming.StartsUtc);
    }

    [Fact]
    public void Parse_EmptyPayload_ReturnsEmpty()
    {
        IReadOnlyList<FreeGame> games = EpicFreeGamesSource.Parse("{\"data\":{\"Catalog\":{\"searchStore\":{\"elements\":[]}}}}");

        Assert.Empty(games);
    }

    [Fact]
    public void BuildEndpoint_UsesGivenLocaleAndCountry()
    {
        string endpoint = EpicFreeGamesSource.BuildEndpoint("de-DE", "DE");

        Assert.Contains("locale=de-DE", endpoint, StringComparison.Ordinal);
        Assert.Contains("country=DE", endpoint, StringComparison.Ordinal);
        Assert.Contains("allowCountries=DE", endpoint, StringComparison.Ordinal);
    }

    [Fact]
    public void Parse_PicksWideKeyImage()
    {
        const string json = """
        {"data":{"Catalog":{"searchStore":{"elements":[{
          "title":"Imaged Game",
          "productSlug":"imaged-game",
          "price":{"totalPrice":{"originalPrice":1999,"discountPrice":0,"fmtPrice":{"originalPrice":"$19.99"}}},
          "keyImages":[
            {"type":"Thumbnail","url":"https://img/thumb.jpg"},
            {"type":"OfferImageWide","url":"https://img/wide.jpg"}
          ],
          "promotions":{"promotionalOffers":[{"promotionalOffers":[
            {"startDate":"2026-08-27T15:00:00.000Z","endDate":"2026-09-03T15:00:00.000Z","discountSetting":{"discountPercentage":0}}
          ]}]}
        }]}}}}
        """;

        FreeGame game = Assert.Single(EpicFreeGamesSource.Parse(json));
        Assert.Equal("https://img/wide.jpg", game.ImageUrl);
    }

    [Fact]
    public void Parse_WithLocale_BuildsLocalizedStoreUrl()
    {
        IReadOnlyList<FreeGame> games = EpicFreeGamesSource.Parse(Samples.EpicPromotions(), "de-DE");

        FreeGame breathedge = games.Single(g => g.Title == "Breathedge");
        Assert.Equal("https://store.epicgames.com/de-DE/p/breathedge", breathedge.Url);
    }
}
