using System.Diagnostics;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace GameScout.App.Services;

/// <summary>Opens external links in the user's default browser, with a friendly failure message.</summary>
public static class UrlOpener
{
    /// <summary>Opens <paramref name="url"/> in the default browser; ignores null/empty input.</summary>
    public static void Open(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Couldn't open the link:\n{ex.Message}", "GameScout",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
