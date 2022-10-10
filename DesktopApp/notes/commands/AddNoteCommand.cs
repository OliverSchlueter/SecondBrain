using System;
using System.Windows.Input;

namespace DesktopApp.notes.commands;

public class AddNoteCommand : ICommand
{
    private readonly Category<Note> _category;
    private readonly Type _noteType;

    public AddNoteCommand(Category<Note> category, Type noteType)
    {
        _category = category;
        _noteType = noteType;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        Console.WriteLine("[AddNoteCommand::Execute] Not implemented");
    }

    public event EventHandler CanExecuteChanged;
}