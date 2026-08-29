using System.Windows.Input;

namespace FreeGameScout.Core.Mvvm;

/// <summary>
/// A lightweight <see cref="ICommand"/> implementation that delegates
/// <see cref="Execute"/> and <see cref="CanExecute"/> to caller-supplied delegates.
/// </summary>
public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>Initializes a new instance of <see cref="RelayCommand"/>.</summary>
    /// <param name="execute">The action to invoke on <see cref="Execute"/>.</param>
    /// <param name="canExecute">Optional predicate for <see cref="CanExecute"/>.</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <inheritdoc/>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    /// <inheritdoc/>
    public void Execute(object? parameter) => _execute(parameter);

    /// <summary>Raises <see cref="CanExecuteChanged"/> to notify bound controls.</summary>
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
