using GameScout.Core.Updating;

namespace GameScout.Core.Tests.TestSupport;

/// <summary>A deterministic <see cref="IReleaseSource"/> for update-checker tests.</summary>
internal sealed class StubReleaseSource : IReleaseSource
{
    private readonly ReleaseInfo? _release;

    public StubReleaseSource(ReleaseInfo? release) => _release = release;

    public Task<ReleaseInfo?> GetLatestAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_release);
}
