using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GameScout.Core.Games;

namespace GameScout.App.Converters;

/// <summary>Maps a <see cref="GameStore"/> to its short display label.</summary>
public sealed class StoreLabelConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is GameStore store ? store.DisplayName().ToUpperInvariant() : "STORE";

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Loads a remote image URL into a <see cref="BitmapImage"/> (async, cached); null when absent.</summary>
public sealed class ImageUrlToSourceConverter : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrWhiteSpace(url) ||
            !Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            return null;

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = uri;
            bitmap.EndInit();
            return bitmap;
        }
        catch (Exception)
        {
            return null; // Broken/unreachable image URLs just render as an empty tile.
        }
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Renders a human-readable timing line for an offer: how long a current freebie lasts, or when
/// an upcoming freebie begins.
/// </summary>
public sealed class OfferTimingConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not FreeGame game)
            return string.Empty;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (game.Kind == GiveawayKind.Upcoming)
        {
            return game.StartsUtc is { } starts
                ? $"Free from {starts.ToLocalTime():MMM d}"
                : "Coming soon";
        }

        if (game.EndsUtc is not { } ends)
            return "Free now";

        TimeSpan remaining = ends - now;
        if (remaining <= TimeSpan.Zero)
            return "Ending now";
        if (remaining.TotalDays >= 1)
        {
            int days = (int)Math.Round(remaining.TotalDays);
            return $"Ends in {days} day{(days == 1 ? string.Empty : "s")}";
        }

        int hours = Math.Max(1, (int)Math.Round(remaining.TotalHours));
        return $"Ends in {hours} hour{(hours == 1 ? string.Empty : "s")}";
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Collapses an element when the bound string is null/empty; visible otherwise.</summary>
public sealed class StringToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Collapses an element when the bound count is zero; visible otherwise.</summary>
public sealed class CountToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int count && count > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Maps a boolean to <see cref="Visibility"/> (true → Visible), invertible via parameter "invert".</summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool flag = value is true;
        if (string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase))
            flag = !flag;
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
