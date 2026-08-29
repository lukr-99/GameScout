using GameScout.Core.Abstractions;
using GameScout.Core.Games;

namespace GameScout.Core.Tests.TestSupport;

/// <summary>A deterministic <see cref="IDealSource"/> for aggregator tests.</summary>
internal sealed class StubDealSource : IDealSource
{
    private readonly IReadOnlyList<GameDeal> _deals;
    private readonly Exception? _throw;

    private StubDealSource(string name, IReadOnlyList<GameDeal> deals, Exception? toThrow)
    {
        Name = name;
        _deals = deals;
        _throw = toThrow;
    }

    public string Name { get; }

    public static StubDealSource Returning(string name, params GameDeal[] deals)
        => new(name, deals, null);

    public static StubDealSource Failing(string name)
        => new(name, [], new InvalidOperationException("boom"));

    public Task<IReadOnlyList<GameDeal>> GetDealsAsync(CancellationToken cancellationToken = default)
        => _throw is not null ? Task.FromException<IReadOnlyList<GameDeal>>(_throw) : Task.FromResult(_deals);
}
