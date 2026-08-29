using System.ComponentModel;
using System.Windows;

namespace FreeGameScout.App;

/// <summary>
/// The rundown window. View wiring only: the close box hides to the tray (the app keeps running so
/// it can re-scan on the next launch), and full exit happens from the tray menu.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>Initializes the window.</summary>
    public MainWindow() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnClosing(CancelEventArgs e)
    {
        // Unless the app is really exiting, keep it alive in the tray instead of closing.
        if (System.Windows.Application.Current is App { IsExiting: false })
        {
            e.Cancel = true;
            Hide();
        }

        base.OnClosing(e);
    }

    private void OnHideClick(object sender, RoutedEventArgs e)
    {
        if (System.Windows.Application.Current is App app)
            app.HideToTray();
    }
}
