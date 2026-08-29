using System.Collections.ObjectModel;
using GameScout.App.Services;
using GameScout.Core.Aggregation;
using GameScout.Core.Games;
using GameScout.Core.Mvvm;

namespace GameScout.App.ViewModels;

/// <summary>The "On sale" tab: popular, normally-paid games currently discounted.</summary>
public sealed class DealsViewModel : ScannerViewModel
{
    private readonly DealAggregator _aggregator;

    /// <summary>Initializes a new <see cref="DealsViewModel"/>.</summary>
    public DealsViewModel(DealAggregator aggregator)
    {
        _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
        OpenCommand = new RelayCommand(p => UrlOpener.Open((p as GameDeal)?.Url));
    }

    /// <summary>Raised on the UI thread after each completed scan (for logging).</summary>
    public event Action<DealReport>? ScanCompleted;

    /// <summary>The discounted games, deepest discount first.</summary>
    public ObservableCollection<GameDeal> Deals { get; } = [];

    /// <summary>Opens a deal's page in the browser.</summary>
    public RelayCommand OpenCommand { get; }

    /// <inheritdoc/>
    protected override string BusyText => "Finding the best deals…";

    /// <inheritdoc/>
    protected override async Task<ScanOutcome> ScanAsync(CancellationToken cancellationToken)
    {
        DealReport report = await _aggregator.ScanAsync(cancellationToken).ConfigureAwait(true);

        Deals.Clear();
        foreach (GameDeal deal in report.Deals)
            Deals.Add(deal);
        ScanCompleted?.Invoke(report);

        string status = Deals.Count == 0 ? "No deals found." : $"{Deals.Count} deals on sale.";
        return new ScanOutcome(status, report.Errors);
    }
}
