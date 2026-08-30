using System.Reflection;

namespace GameScout.App.Services;

/// <summary>
/// Exposes the running build's product name and version for window titles, the tray tooltip, and
/// the diagnostic log. Debug builds report a pre-release version (e.g. "0.2.0-dev") so a test
/// instance is easy to tell apart from an installed release.
/// </summary>
public static class AppInfo
{
    /// <summary>Base product name.</summary>
    public const string ProductName = "GameScout";

    /// <summary>Informational version such as "0.2.0" or "0.2.0-dev" (git build metadata stripped).</summary>
    public static string Version { get; } = ReadInformationalVersion();

    /// <summary>Whether this is a pre-release build (its version carries a suffix such as -dev).</summary>
    public static bool IsPreRelease { get; } = Version.Contains('-', StringComparison.Ordinal);

    /// <summary>Product name with version, e.g. "GameScout 0.2.0" or "GameScout 0.2.0-dev".</summary>
    public static string TitleWithVersion { get; } = $"{ProductName} {Version}";

    private static string ReadInformationalVersion()
    {
        string? info = typeof(AppInfo).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (string.IsNullOrWhiteSpace(info))
            return typeof(AppInfo).Assembly.GetName().Version?.ToString(3) ?? "0.0.0";

        // The SDK appends "+<git sha>" build metadata; drop it for a clean display string.
        int plus = info.IndexOf('+', StringComparison.Ordinal);
        return plus >= 0 ? info[..plus] : info;
    }
}
