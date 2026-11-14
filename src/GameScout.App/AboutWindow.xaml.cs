using System.Windows;
using System.Windows.Media.Animation;
using GameScout.App.Services;
using GameScout.Core.About;

namespace GameScout.App;

/// <summary>Modal About page: app facts, author credit, feedback links and credits.</summary>
public partial class AboutWindow : Window
{
    private readonly ThemeManager _theme;
    private WindowChromeThemer? _themer;

    /// <summary>Initializes the window and fills it from <see cref="AboutInfo"/> and <see cref="AppInfo"/>.</summary>
    public AboutWindow(ThemeManager theme)
    {
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        InitializeComponent();

        VersionText.Text = $"Version {AppInfo.Version}";
        SummaryText.Text = AboutInfo.Summary;
        AuthorHeadline.Text = AboutInfo.AuthorHeadline;
        FeedbackText.Text = AboutInfo.FeedbackInvitation;
        LinkList.ItemsSource = AboutInfo.Links;
        CreditList.ItemsSource = AboutInfo.Credits;
        CopyrightText.Text = AboutInfo.Copyright;
        LicenseNoteText.Text = AboutInfo.LicenseNote;

        _theme.ThemeChanged += OnThemeChanged;
        Closed += (_, _) => _theme.ThemeChanged -= OnThemeChanged;
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _themer = new WindowChromeThemer(this);
        ApplyChrome();
    }

    private void OnThemeChanged(object? sender, EventArgs e) => ApplyChrome();

    private void ApplyChrome() => _themer?.Apply(_theme.Current == AppTheme.Dark);

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // One gentle fade-in; skipped when Windows has animations turned off (reduced motion).
        if (!SystemParameters.ClientAreaAnimation)
            return;

        ScrollArea.Opacity = 0;
        ScrollArea.BeginAnimation(OpacityProperty,
            new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220)));
    }

    private void OnLinkClick(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button { Tag: string url })
            UrlOpener.Open(url);
    }
}
