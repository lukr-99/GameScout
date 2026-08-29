using System.Diagnostics;
using Microsoft.Win32;

namespace GameScout.App.Services;

/// <summary>
/// Registers (or removes) GameScout in the per-user Windows "Run" key so it launches at sign-in.
/// Uses HKCU only, so no elevation is required. The startup launch adds a <c>--tray</c> argument so
/// the app comes up quietly in the notification area instead of stealing focus every boot.
/// </summary>
public sealed class StartupRegistration
{
    /// <summary>The argument passed when Windows launches the app at sign-in.</summary>
    public const string StartupArgument = "--tray";

    private readonly string _runKeyPath;
    private readonly string _valueName;

    /// <summary>Initializes a new <see cref="StartupRegistration"/> targeting the per-user Run key.</summary>
    /// <param name="valueName">The registry value name to use; defaults to "GameScout".</param>
    public StartupRegistration(string valueName = "GameScout")
    {
        _runKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        _valueName = valueName;
    }

    /// <summary>Whether the app is currently registered to run at sign-in.</summary>
    public bool IsEnabled()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(_runKeyPath, writable: false);
        return key?.GetValue(_valueName) is string;
    }

    /// <summary>Enables or disables launch at sign-in.</summary>
    /// <param name="enabled">
    /// <see langword="true"/> to register; <see langword="false"/> to remove the entry.
    /// </param>
    public void Set(bool enabled)
    {
        using RegistryKey key = Registry.CurrentUser.CreateSubKey(_runKeyPath, writable: true);
        if (enabled)
            key.SetValue(_valueName, $"\"{ExecutablePath()}\" {StartupArgument}");
        else if (key.GetValue(_valueName) is not null)
            key.DeleteValue(_valueName, throwOnMissingValue: false);
    }

    private static string ExecutablePath()
    {
        // Prefer the real host .exe; ProcessPath is the launched executable on a published app.
        string? processPath = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(processPath))
            return processPath;

        return Process.GetCurrentProcess().MainModule?.FileName
            ?? Environment.GetCommandLineArgs()[0];
    }
}
