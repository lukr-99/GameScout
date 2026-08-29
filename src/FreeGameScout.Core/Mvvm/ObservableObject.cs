using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FreeGameScout.Core.Mvvm;

/// <summary>Base class for all view-model objects; implements <see cref="INotifyPropertyChanged"/>.</summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raises <see cref="PropertyChanged"/> for the calling property.</summary>
    /// <param name="propertyName">Name of the changed property (auto-filled by the compiler).</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Sets <paramref name="field"/> to <paramref name="value"/> and raises
    /// <see cref="PropertyChanged"/> if the value actually changed.
    /// </summary>
    /// <typeparam name="T">Field type.</typeparam>
    /// <param name="field">Backing field reference.</param>
    /// <param name="value">New value.</param>
    /// <param name="propertyName">Property name (auto-filled).</param>
    /// <returns><see langword="true"/> when the value changed; otherwise <see langword="false"/>.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
