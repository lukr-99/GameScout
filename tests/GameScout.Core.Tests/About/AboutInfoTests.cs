using GameScout.Core.About;

namespace GameScout.Core.Tests.About;

public sealed class AboutInfoTests
{
    [Fact]
    public void AuthorHeadline_Always_UsesAuthorName()
        => Assert.Equal("Made by Lukáš Krejčí", AboutInfo.AuthorHeadline);

    [Fact]
    public void Links_Always_ListInstagramGitHubAndIssues()
    {
        string[] urls = [.. AboutInfo.Links.Select(l => l.Url)];

        Assert.Equal(
            [
                "https://www.instagram.com/lukas9krejci/",
                "https://github.com/lukr-99",
                "https://github.com/lukr-99/GameScout/issues",
            ],
            urls);
    }

    [Fact]
    public void Links_Always_AreAbsoluteHttpsWithLabelAndHandle()
    {
        foreach (AboutLink link in AboutInfo.Links)
        {
            Assert.True(Uri.TryCreate(link.Url, UriKind.Absolute, out Uri? uri));
            Assert.Equal(Uri.UriSchemeHttps, uri.Scheme);
            Assert.False(string.IsNullOrWhiteSpace(link.Label));
            Assert.False(string.IsNullOrWhiteSpace(link.Handle));
        }
    }

    [Fact]
    public void Credits_Always_HaveNoBlankFieldsAndNoDuplicates()
    {
        Assert.NotEmpty(AboutInfo.Credits);
        foreach (CreditEntry c in AboutInfo.Credits)
        {
            Assert.False(string.IsNullOrWhiteSpace(c.Name));
            Assert.False(string.IsNullOrWhiteSpace(c.Role));
            Assert.False(string.IsNullOrWhiteSpace(c.License));
        }

        Assert.Equal(AboutInfo.Credits.Count, AboutInfo.Credits.Select(c => c.Name).Distinct().Count());
    }

    [Fact]
    public void Texts_Always_ContainNoEmDash()
    {
        string all = string.Join(' ',
            AboutInfo.Summary, AboutInfo.FeedbackInvitation, AboutInfo.LicenseNote,
            string.Join(' ', AboutInfo.Credits.Select(c => c.Name + c.Role + c.License)));

        Assert.DoesNotContain('\u2014', all);
    }
}
