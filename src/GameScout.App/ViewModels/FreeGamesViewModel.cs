using System.Collections.ObjectModel;
using GameScout.App.Services;
using GameScout.Core.Aggregation;
using GameScout.Core.Games;
using GameScout.Core.Mvvm;

namespace GameScout.App.ViewModels;

/// <summary>The "Free now" tab: currently-free giveaways plus upcoming free games.</summary>
public sealed class FreeGamesViewModel : ScannerViewModel
{
    private readonly GiveawayAggregator _aggregator;

    /// <summary>Initializes a new <see cref="FreeGamesViewModel"/>.</summary>
    public FreeGamesViewModel(GiveawayAggregator aggregator)
    {
        _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
        OpenCommand = new RelayCommand(p => UrlOpener.Open((p as FreeGame)?.Url));
    }

    /// <summary>Raised on the UI thread after each completed scan (for the tray balloon/log).</summary>
    public event Action<FreeGameReport>? ScanCompleted;

    /// <summary>Games claimable right now.</summary>
    public ObservableCollection<FreeGame> CurrentlyFree { get; } = [];

    /// <summary>Games announced to become free soon.</summary>
    public ObservableCollection<FreeGame> Upcoming { get; } = [];

    /// <summary>Opens a game's claim page in the browser.</summary>
    public RelayCommand OpenCommand { get; }

    /// <inheritdoc/>
    protected override string BusyText => "Scanning Epic, Steam, GOG & more…";

    /// <inheritdoc/>
    protected override async Task<ScanOutcome> ScanAsync(CancellationToken cancellationToken)
    {
        FreeGameReport report = await _aggregator.ScanAsync(cancellationToken).ConfigureAwait(true);

        Replace(CurrentlyFree, report.CurrentlyFree);
        Replace(Upcoming, report.Upcoming);
        ScanCompleted?.Invoke(report);

        int free = CurrentlyFree.Count;
        string status = free switch
        {
            0 => "No free games right now.",
            1 => "1 free game to grab.",
            _ => $"{free} free games to grab.",
        };
        if (Upcoming.Count > 0)
            status += $" {Upcoming.Count} coming soon.";

        return new ScanOutcome(status, report.Errors);
    }

    private static void Replace(ObservableCollection<FreeGame> target, IEnumerable<FreeGame> items)
    {
        target.Clear();
        foreach (FreeGame item in items)
            target.Add(item);
    }
}
