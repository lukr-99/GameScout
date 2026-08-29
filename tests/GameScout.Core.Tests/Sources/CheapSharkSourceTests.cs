using GameScout.Core.Games;
using GameScout.Core.Sources.CheapShark;
using GameScout.Core.Tests.TestSupport;

namespace GameScout.Core.Tests.Sources;

public sealed class CheapSharkSourceTests
{
    [Fact]
    public void Parse_Sample_MapsAllRows()
    {
        IReadOnlyList<GameDeal> deals = CheapSharkSource.Parse(Samples.CheapSharkDeals());

        Assert.Equal(3, deals.Count);
    }

    [Fact]
    public void Parse_Sample_FormatsPricesAndDiscount()
    {
        IReadOnlyList<GameDeal> deals = CheapSharkSource.Parse(Samples.CheapSharkDeals());

        GameDeal gog = deals.Single(d => d.Title == "Cool Indie RPG");
        Assert.Equal(GameStore.Gog, gog.Store);
        Assert.Equal("$4.99", gog.SalePrice);
        Assert.Equal("$19.99", gog.NormalPrice);
        Assert.Equal(75, gog.DiscountPercent);
        Assert.Equal(4.99m, gog.SaleAmount);
    }

    [Fact]
    public void Parse_Sample_PrefersSteamHeaderImageWhenAppIdPresent()
    {
        IReadOnlyList<GameDeal> deals = CheapSharkSource.Parse(Samples.CheapSharkDeals());

        GameDeal steam = deals.First(d => d.Title.StartsWith("Suicide", StringComparison.Ordinal));
        Assert.Contains("steam/apps/315210/header.jpg", steam.ImageUrl!, StringComparison.Ordinal);
    }

    [Fact]
    public void Parse_Sample_UsesThumbWhenNoSteamAppId()
    {
        IReadOnlyList<GameDeal> deals = CheapSharkSource.Parse(Samples.CheapSharkDeals());

        GameDeal gog = deals.Single(d => d.Title == "Cool Indie RPG");
        Assert.Equal("https://img/thumb.jpg", gog.ImageUrl);
    }

    [Fact]
    public void Parse_Sample_BuildsRedirectUrl()
    {
        IReadOnlyList<GameDeal> deals = CheapSharkSource.Parse(Samples.CheapSharkDeals());

        GameDeal gog = deals.Single(d => d.Title == "Cool Indie RPG");
        Assert.Equal("https://www.cheapshark.com/redirect?dealID=ABC123", gog.Url);
    }

    [Fact]
    public void Parse_EmptyArray_ReturnsEmpty()
        => Assert.Empty(CheapSharkSource.Parse("[]"));
}
