using System.Globalization;
using System.IO;
using System.Text;
using GameScout.Core.Games;

namespace GameScout.App.Services;

/// <summary>
/// Appends a short, human-readable record of each scan to a log file under the user's local app
/// data. This exists mainly so a first run on a fresh machine leaves a diagnostic trail even when
/// nobody is watching the UI. All writes are best-effort and never throw.
/// </summary>
public sealed class ScanLog
{
    private readonly string _path;
    private readonly object _gate = new();

    /// <summary>Initializes a log at the default location, creating the folder if needed.</summary>
    public ScanLog()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GameScout");
        _path = Path.Combine(dir, "scout.log");

        try
        {
            Directory.CreateDirectory(dir);
        }
        catch (IOException)
        {
            // Logging is optional; ignore folder creation failures.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    /// <summary>The full path of the log file (shown to the user for troubleshooting).</summary>
    public string FilePath => _path;

    /// <summary>Writes a free-form informational line.</summary>
    public void Info(string message) => Append(message);

    /// <summary>Writes a one-line summary of a completed giveaway scan plus any per-source errors.</summary>
    public void RecordFree(FreeGameReport report)
    {
        var builder = new StringBuilder();
        int free = report.CurrentlyFree.Count();
        int upcoming = report.Upcoming.Count();
        builder.Append(CultureInfo.InvariantCulture, $"free scan: {free} free, {upcoming} upcoming");

        if (free > 0)
            builder.Append(" | free: ").Append(string.Join(", ", report.CurrentlyFree.Select(g => $"{g.Title} ({g.Store})")));
        if (report.Errors.Count > 0)
            builder.Append(" | errors: ").Append(string.Join("; ", report.Errors));

        Append(builder.ToString());
    }

    /// <summary>Writes a one-line summary of a completed deals scan plus any per-source errors.</summary>
    public void RecordDeals(DealReport report)
    {
        var builder = new StringBuilder();
        builder.Append(CultureInfo.InvariantCulture, $"deals scan: {report.Deals.Count} on sale");
        if (report.Errors.Count > 0)
            builder.Append(" | errors: ").Append(string.Join("; ", report.Errors));

        Append(builder.ToString());
    }

    private void Append(string message)
    {
        string line = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}";
        try
        {
            lock (_gate)
                File.AppendAllText(_path, line);
        }
        catch (IOException)
        {
            // Best-effort logging; swallow write failures.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
