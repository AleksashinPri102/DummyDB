using System;
using System.Windows.Input;

namespace DummyDB.Desktop.ViewModels;

public class RelayCommand : ICommand
{
    private readonly Action<object?> _action;
    
    public RelayCommand(Action<object?> action) => _action = action;
    
    public bool CanExecute(object? parameter) => true;
    
    public void Execute(object? parameter) => _action(parameter);

    public event EventHandler? CanExecuteChanged;
}