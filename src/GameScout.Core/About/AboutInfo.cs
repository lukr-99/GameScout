namespace GameScout.Core.About;

/// <summary>
/// Single source for the author credit, contact links and credits shown on the About page.
/// The app name and version come from the build (see the App's AppInfo), not from here.
/// </summary>
public static class AboutInfo
{
    /// <summary>Author's display name.</summary>
    public const string AuthorName = "Lukáš Krejčí";

    /// <summary>Headline shown on the author card.</summary>
    public static string AuthorHeadline { get; } = $"Made by {AuthorName}";

    /// <summary>One or two plain lines on what GameScout is.</summary>
    public const string Summary =
        "GameScout lives in your tray and shows which PC games are free to keep right now, "
        + "and which popular games are on sale.";

    /// <summary>Invitation shown under the author headline.</summary>
    public const string FeedbackInvitation =
        "Feedback is welcome: what works, what does not, what is missing.";

    /// <summary>Copyright line for the app.</summary>
    public const string Copyright = "Copyright © Lukáš Krejčí";

    /// <summary>Note about the licence of the GameScout source and binaries.</summary>
    public const string LicenseNote =
        "GameScout is published on GitHub. No open-source licence file is included yet, "
        + "so all rights stay with the author.";

    /// <summary>Address of the author's Instagram profile.</summary>
    public const string InstagramUrl = "https://www.instagram.com/lukas9krejci/";

    /// <summary>Address of the author's GitHub profile.</summary>
    public const string GitHubUrl = "https://github.com/lukr-99";

    /// <summary>Address of this repository's issue tracker.</summary>
    public const string IssuesUrl = "https://github.com/lukr-99/GameScout/issues";

    /// <summary>Contact and feedback links, in display order.</summary>
    public static IReadOnlyList<AboutLink> Links { get; } =
    [
        new AboutLink("Instagram", "@lukas9krejci", InstagramUrl),
        new AboutLink("GitHub", "lukr-99", GitHubUrl),
        new AboutLink("Issues", "lukr-99/GameScout", IssuesUrl),
    ];

    /// <summary>Libraries, data providers and assets GameScout ships with or relies on.</summary>
    public static IReadOnlyList<CreditEntry> Credits { get; } =
    [
        new CreditEntry(".NET and WPF", "Runtime and user interface", "MIT, Microsoft"),
        new CreditEntry("Microsoft.Extensions.DependencyInjection", "Wires the app together", "MIT, Microsoft"),
        new CreditEntry("Epic Games Store", "Free game promotions feed", "Public feed, content belongs to Epic Games"),
        new CreditEntry("GamerPower", "Free giveaways from other stores", "Public API, content belongs to GamerPower and the stores"),
        new CreditEntry("CheapShark", "Game sale prices and discounts", "Public API, content belongs to CheapShark and the stores"),
        new CreditEntry("Segoe UI", "Interface font, used from Windows", "Not bundled, part of Windows"),
        new CreditEntry("App icon", "gamescout.ico and gamescout.png", "Part of this repository"),
    ];
}
