using System.Drawing;
using System.IO;
using Application = System.Windows.Application;

namespace GameScout.App.Services;

/// <summary>Loads the embedded application icon for use by the tray <c>NotifyIcon</c>.</summary>
public static class AppIcon
{
    private static readonly Uri IconUri = new("pack://application:,,,/Assets/gamescout.ico");

    /// <summary>Returns the app icon, or the system default if the resource can't be loaded.</summary>
    public static Icon LoadTrayIcon()
    {
        try
        {
            System.Windows.Resources.StreamResourceInfo? info = Application.GetResourceStream(IconUri);
            if (info is not null)
            {
                using Stream stream = info.Stream;
                return new Icon(stream);
            }
        }
        catch (Exception)
        {
            // Fall through to the system icon below.
        }

        return SystemIcons.Application;
    }
}
