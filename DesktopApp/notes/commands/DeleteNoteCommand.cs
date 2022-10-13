using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopApp.notes.commands;

public class DeleteNoteCommand : ICommand
{
    private readonly Category<Note> _category;
    private readonly Note _note;
    private readonly TreeViewItem _noteSource;

    public DeleteNoteCommand(Category<Note> category, Note note, TreeViewItem noteSource)
    {
        _category = category;
        _note = note;
        _noteSource = noteSource;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        if (!_category.Values.Contains(_note))
        {
            return;
        }
                        
        var confirm = MessageBox.Show(
            $"Do you want to permanently delete the '{_note.ToString()}' Note?",
            "Delete Note",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (confirm == MessageBoxResult.Yes)
        {
            _note.Remove();
            _category.Values.Remove(_note);
            _category.TreeViewItem.Items.Remove(_noteSource);
        }
    }

    public event EventHandler CanExecuteChanged;
}