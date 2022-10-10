using System;
using System.Windows.Input;
using DesktopApp.ui;

namespace DesktopApp.notes.commands;

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
        newCategoryWindow.Show();
    }

    public event EventHandler CanExecuteChanged;
}