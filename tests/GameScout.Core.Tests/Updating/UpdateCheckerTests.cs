using GameScout.Core.Updating;
using GameScout.Core.Tests.TestSupport;

namespace GameScout.Core.Tests.Updating;

public sealed class UpdateCheckerTests
{
    [Theory]
    [InlineData("v1.2.3", 1, 2, 3)]
    [InlineData("1.4", 1, 4, -1)]
    [InlineData("v2.0.0-beta1", 2, 0, 0)]
    public void ReleaseVersion_TryParse_ParsesTags(string tag, int major, int minor, int build)
    {
        Assert.True(ReleaseVersion.TryParse(tag, out Version version));
        Assert.Equal(major, version.Major);
        Assert.Equal(minor, version.Minor);
        Assert.Equal(build, version.Build);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-version")]
    public void ReleaseVersion_TryParse_RejectsGarbage(string tag)
        => Assert.False(ReleaseVersion.TryParse(tag, out _));

    [Fact]
    public void Parse_Sample_PicksInstallerAssetAndVersion()
    {
        ReleaseInfo? info = GitHubReleaseSource.Parse(Samples.GitHubRelease());

        Assert.NotNull(info);
        Assert.Equal(new Version(0, 3, 0), info!.Version);
        Assert.EndsWith("GameScoutSetup-0.3.0.exe", info.DownloadUrl!, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CheckForUpdateAsync_ReturnsRelease_WhenNewer()
    {
        var checker = new UpdateChecker(new StubReleaseSource(GitHubReleaseSource.Parse(Samples.GitHubRelease())));

        ReleaseInfo? update = await checker.CheckForUpdateAsync(new Version(0, 1, 0));

        Assert.NotNull(update);
        Assert.Equal("v0.3.0", update!.TagName);
    }

    [Fact]
    public async Task CheckForUpdateAsync_ReturnsNull_WhenCurrentIsUpToDate()
    {
        var checker = new UpdateChecker(new StubReleaseSource(GitHubReleaseSource.Parse(Samples.GitHubRelease())));

        ReleaseInfo? update = await checker.CheckForUpdateAsync(new Version(0, 3, 0));

        Assert.Null(update);
    }

    [Fact]
    public async Task CheckForUpdateAsync_ReturnsNull_WhenNoRelease()
    {
        var checker = new UpdateChecker(new StubReleaseSource(null));

        Assert.Null(await checker.CheckForUpdateAsync(new Version(0, 1, 0)));
    }
}
