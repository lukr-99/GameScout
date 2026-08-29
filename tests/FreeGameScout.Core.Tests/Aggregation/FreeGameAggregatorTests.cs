using FreeGameScout.Core.Aggregation;
using FreeGameScout.Core.Games;
using FreeGameScout.Core.Tests.TestSupport;

namespace FreeGameScout.Core.Tests.Aggregation;

public sealed class FreeGameAggregatorTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 28, 12, 0, 0, TimeSpan.Zero);
    private static readonly FakeTimeProvider Clock = new(Now);

    [Fact]
    public async Task ScanAsync_MergesGamesFromAllSources()
    {
        var epic = StubGiveawaySource.Returning(
            "Epic", new FreeGame("A", GameStore.Epic, GiveawayKind.CurrentlyFree));
        var steam = StubGiveawaySource.Returning(
            "Steam", new FreeGame("B", GameStore.Steam, GiveawayKind.CurrentlyFree));

        var aggregator = new FreeGameAggregator([epic, steam], Clock);
        FreeGameReport report = await aggregator.ScanAsync();

        Assert.Equal(2, report.Games.Count);
        Assert.Empty(report.Errors);
    }

    [Fact]
    public async Task ScanAsync_FailingSource_RecordsErrorButKeepsOthers()
    {
        var ok = StubGiveawaySource.Returning(
            "Epic", new FreeGame("A", GameStore.Epic, GiveawayKind.CurrentlyFree));
        var broken = StubGiveawaySource.Failing("Steam");

        var aggregator = new FreeGameAggregator([ok, broken], Clock);
        FreeGameReport report = await aggregator.ScanAsync();

        Assert.Single(report.Games);
        Assert.Single(report.Errors);
        Assert.Contains("Steam", report.Errors[0], StringComparison.Ordinal);
    }

    [Fact]
    public async Task ScanAsync_DeduplicatesSameTitleStoreAndKind()
    {
        var one = StubGiveawaySource.Returning(
            "Epic", new FreeGame("Breathedge", GameStore.Epic, GiveawayKind.CurrentlyFree));
        var two = StubGiveawaySource.Returning(
            "Aggregator", new FreeGame("breathedge", GameStore.Epic, GiveawayKind.CurrentlyFree));

        var aggregator = new FreeGameAggregator([one, two], Clock);
        FreeGameReport report = await aggregator.ScanAsync();

        Assert.Single(report.Games);
    }

    [Fact]
    public async Task ScanAsync_OrdersCurrentlyFreeBeforeUpcoming()
    {
        var source = StubGiveawaySource.Returning(
            "Mixed",
            new FreeGame("Zeta", GameStore.Epic, GiveawayKind.Upcoming, StartsUtc: Now.AddDays(3)),
            new FreeGame("Alpha", GameStore.Steam, GiveawayKind.CurrentlyFree));

        var aggregator = new FreeGameAggregator([source], Clock);
        FreeGameReport report = await aggregator.ScanAsync();

        Assert.Equal(GiveawayKind.CurrentlyFree, report.Games[0].Kind);
        Assert.Equal(GiveawayKind.Upcoming, report.Games[1].Kind);
    }

    [Fact]
    public async Task ScanAsync_DropsExpiredCurrentOffers()
    {
        var source = StubGiveawaySource.Returning(
            "Epic",
            new FreeGame("Expired", GameStore.Epic, GiveawayKind.CurrentlyFree, EndsUtc: Now.AddDays(-1)),
            new FreeGame("Live", GameStore.Epic, GiveawayKind.CurrentlyFree, EndsUtc: Now.AddDays(1)));

        var aggregator = new FreeGameAggregator([source], Clock);
        FreeGameReport report = await aggregator.ScanAsync();

        FreeGame game = Assert.Single(report.Games);
        Assert.Equal("Live", game.Title);
    }
}
