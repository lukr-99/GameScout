namespace FreeGameScout.Core.Tests.TestSupport;

/// <summary>A fixed-clock <see cref="TimeProvider"/> so aggregator tests are deterministic.</summary>
internal sealed class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;

    public FakeTimeProvider(DateTimeOffset now) => _now = now;

    public override DateTimeOffset GetUtcNow() => _now;
}
