using System.Windows;
using Application = System.Windows.Application;

namespace GameScout.App.Services;

/// <summary>The available visual themes.</summary>
public enum AppTheme
{
    /// <summary>Light surfaces, dark text.</summary>
    Light,

    /// <summary>Dark surfaces, light text.</summary>
    Dark,
}

/// <summary>
/// Swaps the application's semantic theme dictionary at runtime. The theme dictionary is always
/// kept at index 0 of the app's merged dictionaries so shared primitives (index 1+) survive swaps.
/// </summary>
public sealed class ThemeManager
{
    private const string LightSource = "Themes/Theme.Light.xaml";
    private const string DarkSource = "Themes/Theme.Dark.xaml";

    private readonly Application _application;

    /// <summary>Initializes a new <see cref="ThemeManager"/> over <paramref name="application"/>.</summary>
    public ThemeManager(Application application)
        => _application = application ?? throw new ArgumentNullException(nameof(application));

    /// <summary>The currently applied theme.</summary>
    public AppTheme Current { get; private set; } = AppTheme.Light;

    /// <summary>Raised after a theme has been applied, so chrome (window borders) can re-sync.</summary>
    public event EventHandler? ThemeChanged;

    /// <summary>Applies <paramref name="theme"/>, replacing the theme dictionary in place.</summary>
    public void Apply(AppTheme theme)
    {
        string source = theme == AppTheme.Dark ? DarkSource : LightSource;
        var dictionary = new ResourceDictionary
        {
            Source = new Uri(source, UriKind.Relative),
        };

        _application.Resources.MergedDictionaries[0] = dictionary;
        Current = theme;
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Flips between light and dark, returning the newly applied theme.</summary>
    public AppTheme Toggle()
    {
        Apply(Current == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);
        return Current;
    }
}
