using GameScout.Core.Abstractions;
using GameScout.Core.Aggregation;
using GameScout.Core.Games;
using GameScout.Core.Tests.TestSupport;

namespace GameScout.Core.Tests.Aggregation;

public sealed class DealAggregatorTests
{
    private static readonly FakeTimeProvider Clock =
        new(new DateTimeOffset(2026, 8, 29, 12, 0, 0, TimeSpan.Zero));

    private static GameDeal Deal(string title, GameStore store, int percent) =>
        new(title, store, "$1.00", "$10.00", percent);

    [Fact]
    public async Task ScanAsync_DeduplicatesByTitleKeepingDeepestDiscount()
    {
        var source = StubDealSource.Returning(
            "CheapShark",
            Deal("Suicide Squad", GameStore.Steam, 80),
            Deal("Suicide Squad", GameStore.IndieGala, 96));

        var aggregator = new DealAggregator([source], Clock);
        DealReport report = await aggregator.ScanAsync();

        GameDeal only = Assert.Single(report.Deals);
        Assert.Equal(96, only.DiscountPercent);
        Assert.Equal(GameStore.IndieGala, only.Store);
    }

    [Fact]
    public async Task ScanAsync_OrdersByDiscountDescending()
    {
        var source = StubDealSource.Returning(
            "CheapShark",
            Deal("A", GameStore.Steam, 40),
            Deal("B", GameStore.Gog, 90),
            Deal("C", GameStore.Humble, 65));

        var aggregator = new DealAggregator([source], Clock);
        DealReport report = await aggregator.ScanAsync();

        Assert.Equal([90, 65, 40], report.Deals.Select(d => d.DiscountPercent));
    }

    [Fact]
    public async Task ScanAsync_FailingSource_RecordsError()
    {
        var aggregator = new DealAggregator([StubDealSource.Failing("CheapShark")], Clock);
        DealReport report = await aggregator.ScanAsync();

        Assert.Empty(report.Deals);
        Assert.Single(report.Errors);
    }
}
