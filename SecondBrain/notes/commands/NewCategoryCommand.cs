using System;
using System.Windows.Input;
using SecondBrain.ui;

namespace SecondBrain.notes.commands;

public class NewCategoryCommand : ICommand
{
    private readonly Category<Note> _category;

    public NewCategoryCommand(Category<Note> category)
    {
        _category = category;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        var newCategoryWindow = new NewCategoryWindow(_category);
        newCategoryWindow.ShowDialog();
    }

    public event EventHandler CanExecuteChanged;
}