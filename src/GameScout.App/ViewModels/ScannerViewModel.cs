using GameScout.Core.Mvvm;

namespace GameScout.App.ViewModels;

/// <summary>
/// Base for a tab that scans a set of sources: owns the busy/error/status/last-updated state and the
/// refresh command, and defers the actual fetch to <see cref="ScanAsync"/> (template method).
/// </summary>
public abstract class ScannerViewModel : ObservableObject
{
    private bool _isBusy;
    private string _statusText = "Ready.";
    private string? _errorText;
    private string _lastUpdatedText = string.Empty;

    /// <summary>Initializes the base and wires the refresh command.</summary>
    protected ScannerViewModel()
        => RefreshCommand = new RelayCommand(_ => _ = RefreshAsync(), _ => !IsBusy);

    /// <summary>Re-runs the scan.</summary>
    public RelayCommand RefreshCommand { get; }

    /// <summary>Whether a scan is in progress.</summary>
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsIdle));
                RefreshCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>Convenience inverse of <see cref="IsBusy"/>.</summary>
    public bool IsIdle => !IsBusy;

    /// <summary>Short status line shown under the header.</summary>
    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    /// <summary>Per-source error summary, or null when everything succeeded.</summary>
    public string? ErrorText
    {
        get => _errorText;
        private set
        {
            if (SetProperty(ref _errorText, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    /// <summary>Whether <see cref="ErrorText"/> has content.</summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorText);

    /// <summary>"Updated ..." caption, empty until the first scan finishes.</summary>
    public string LastUpdatedText
    {
        get => _lastUpdatedText;
        private set => SetProperty(ref _lastUpdatedText, value);
    }

    /// <summary>The status shown while a scan runs.</summary>
    protected abstract string BusyText { get; }

    /// <summary>Performs the fetch and populates the bound collections.</summary>
    /// <param name="cancellationToken">Token used to cancel the scan.</param>
    /// <returns>The status line and any per-source errors.</returns>
    protected abstract Task<ScanOutcome> ScanAsync(CancellationToken cancellationToken);

    /// <summary>Runs a scan, updating busy/status/error/last-updated state around it.</summary>
    public async Task RefreshAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        StatusText = BusyText;
        ErrorText = null;

        try
        {
            ScanOutcome outcome = await ScanAsync(CancellationToken.None).ConfigureAwait(true);
            StatusText = outcome.StatusText;
            ErrorText = outcome.Errors.Count > 0 ? string.Join(Environment.NewLine, outcome.Errors) : null;
            LastUpdatedText = $"Updated {DateTimeOffset.Now:t}";
        }
        catch (Exception ex)
        {
            StatusText = "Scan failed.";
            ErrorText = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
