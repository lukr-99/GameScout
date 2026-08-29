using GameScout.Core.Abstractions;
using GameScout.Core.Games;

namespace GameScout.Core.Tests.TestSupport;

/// <summary>A deterministic <see cref="IGiveawaySource"/> for aggregator tests.</summary>
internal sealed class StubGiveawaySource : IGiveawaySource
{
    private readonly IReadOnlyList<FreeGame> _games;
    private readonly Exception? _throw;

    private StubGiveawaySource(string name, IReadOnlyList<FreeGame> games, Exception? toThrow)
    {
        Name = name;
        _games = games;
        _throw = toThrow;
    }

    public string Name { get; }

    public static StubGiveawaySource Returning(string name, params FreeGame[] games)
        => new(name, games, null);

    public static StubGiveawaySource Failing(string name)
        => new(name, [], new InvalidOperationException("boom"));

    public Task<IReadOnlyList<FreeGame>> GetFreeGamesAsync(CancellationToken cancellationToken = default)
        => _throw is not null ? Task.FromException<IReadOnlyList<FreeGame>>(_throw) : Task.FromResult(_games);
}
