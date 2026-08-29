using UserControl = System.Windows.Controls.UserControl;

namespace GameScout.App.Views;

/// <summary>The "On sale" tab view. View wiring only; state lives in the view-model.</summary>
public partial class DealsView : UserControl
{
    /// <summary>Initializes the view.</summary>
    public DealsView() => InitializeComponent();
}
