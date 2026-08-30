using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace GameScout.Core.Settings;

/// <summary>Persisted user preferences that configure giveaway sources and filtering.</summary>
public sealed record GameScoutSettings
{
    /// <summary>Canonical value for the light theme.</summary>
    public const string ThemeLight = "Light";

    /// <summary>Canonical value for the dark theme.</summary>
    public const string ThemeDark = "Dark";

    /// <summary>Default settings used when no valid settings file exists.</summary>
    public static GameScoutSettings Default { get; } = new("en-US", "US", 2.99m);

    /// <summary>Initializes a settings value.</summary>
    /// <param name="locale">Storefront locale.</param>
    /// <param name="country">Two-letter country code.</param>
    /// <param name="minimumWorth">Minimum normal price for known-price giveaways.</param>
    public GameScoutSettings(string locale, string country, decimal minimumWorth)
    {
        Locale = locale;
        Country = country;
        MinimumWorth = minimumWorth;
    }

    /// <summary>Locale used for Epic queries and store links.</summary>
    public string Locale { get; init; }

    /// <summary>Two-letter country code used for Epic promotions.</summary>
    public string Country { get; init; }

    /// <summary>Minimum normal price for known-price giveaways.</summary>
    public decimal MinimumWorth { get; init; }

    /// <summary>
    /// Selected UI theme; one of <see cref="ThemeLight"/> or <see cref="ThemeDark"/>. Kept as a
    /// string so the Core settings model stays independent of the WPF theme enum. Not part of the
    /// positional constructor: it round-trips via <see cref="NormalizeTheme"/> in the store.
    /// </summary>
    public string Theme { get; init; } = ThemeLight;

    /// <summary>Normalizes an arbitrary theme string to a known value, defaulting to light.</summary>
    /// <param name="theme">Candidate theme name (case-insensitive).</param>
    /// <returns><see cref="ThemeDark"/> when the value names dark; otherwise <see cref="ThemeLight"/>.</returns>
    public static string NormalizeTheme(string? theme)
        => string.Equals(theme?.Trim(), ThemeDark, StringComparison.OrdinalIgnoreCase)
            ? ThemeDark
            : ThemeLight;

    /// <summary>Validates and normalizes user-entered settings.</summary>
    /// <param name="locale">Candidate locale.</param>
    /// <param name="country">Candidate country code.</param>
    /// <param name="minimumWorth">Candidate minimum worth.</param>
    /// <param name="settings">Normalized settings when validation succeeds.</param>
    /// <param name="error">User-facing validation error when validation fails.</param>
    /// <returns><see langword="true"/> when all values are valid.</returns>
    public static bool TryCreate(
        string? locale,
        string? country,
        decimal minimumWorth,
        [NotNullWhen(true)] out GameScoutSettings? settings,
        [NotNullWhen(false)] out string? error)
    {
        settings = null;

        string trimmedLocale = locale?.Trim() ?? string.Empty;
        string[] localeParts = trimmedLocale.Split('-');
        bool validLocaleShape = localeParts.Length is 1 or 2 &&
            localeParts[0].Length is 2 or 3 &&
            localeParts[0].All(IsAsciiLetter) &&
            (localeParts.Length == 1 || localeParts[1].Length == 2 && localeParts[1].All(IsAsciiLetter));
        if (!validLocaleShape)
        {
            error = "Enter a valid locale such as en-US or cs-CZ.";
            return false;
        }

        CultureInfo culture;
        try
        {
            culture = CultureInfo.GetCultureInfo(trimmedLocale);
        }
        catch (CultureNotFoundException)
        {
            error = "Enter a valid locale such as en-US or cs-CZ.";
            return false;
        }

        if (string.IsNullOrEmpty(culture.Name))
        {
            error = "Enter a valid locale such as en-US or cs-CZ.";
            return false;
        }

        string trimmedCountry = country?.Trim() ?? string.Empty;
        if (trimmedCountry.Length != 2 || !trimmedCountry.All(IsAsciiLetter))
        {
            error = "Country must be a two-letter code such as US or CZ.";
            return false;
        }

        if (minimumWorth < 0)
        {
            error = "Minimum worth cannot be negative.";
            return false;
        }

        settings = new GameScoutSettings(
            culture.Name,
            trimmedCountry.ToUpperInvariant(),
            minimumWorth);
        error = null;
        return true;
    }

    private static bool IsAsciiLetter(char value)
        => value is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
}
