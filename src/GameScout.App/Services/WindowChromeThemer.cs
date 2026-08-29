using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace GameScout.App.Services;

/// <summary>
/// Tints the native Win11 title bar and window border to match the current theme, using DWM window
/// attributes. Reads the live theme colors from application resources, so a theme swap just calls
/// <see cref="Apply"/> again. No-ops gracefully on OS versions that don't support the attributes.
/// </summary>
public sealed class WindowChromeThemer
{
    private const int DwmwaUseImmersiveDarkMode = 20;
    private const int DwmwaBorderColor = 34;
    private const int DwmwaCaptionColor = 35;
    private const int DwmwaTextColor = 36;

    private readonly Window _window;

    /// <summary>Initializes a themer for <paramref name="window"/>.</summary>
    public WindowChromeThemer(Window window)
        => _window = window ?? throw new ArgumentNullException(nameof(window));

    /// <summary>Applies the current theme colors to the window chrome. Safe to call repeatedly.</summary>
    /// <param name="isDark">Whether the dark theme is active (drives the immersive-dark flag).</param>
    public void Apply(bool isDark)
    {
        IntPtr hwnd = new WindowInteropHelper(_window).Handle;
        if (hwnd == IntPtr.Zero)
            return; // Window not yet sourced; caller retries after SourceInitialized.

        int darkFlag = isDark ? 1 : 0;
        _ = DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkMode, ref darkFlag, sizeof(int));

        SetColor(hwnd, DwmwaCaptionColor, "Surface.Window");
        SetColor(hwnd, DwmwaBorderColor, "Accent");
        SetColor(hwnd, DwmwaTextColor, "Text.Primary");
    }

    private void SetColor(IntPtr hwnd, int attribute, string resourceKey)
    {
        if (_window.TryFindResource(resourceKey) is not SolidColorBrush brush)
            return;

        int colorRef = ToColorRef(brush.Color);
        _ = DwmSetWindowAttribute(hwnd, attribute, ref colorRef, sizeof(int));
    }

    // DWM expects a COLORREF: 0x00BBGGRR.
    private static int ToColorRef(Color c) => c.R | (c.G << 8) | (c.B << 16);

    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
