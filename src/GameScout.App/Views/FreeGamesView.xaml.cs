using UserControl = System.Windows.Controls.UserControl;

namespace GameScout.App.Views;

/// <summary>The "Free now" tab view. View wiring only; state lives in the view-model.</summary>
public partial class FreeGamesView : UserControl
{
    /// <summary>Initializes the view.</summary>
    public FreeGamesView() => InitializeComponent();
}
