using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopApp.ui;

namespace DesktopApp.notes.commands;

public class DeleteCategoryCommand : ICommand
{
    private readonly Category<Note> _category;

    public DeleteCategoryCommand(Category<Note> category)
    {
        _category = category;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        var confirm = MessageBox.Show(
            $"Do you want to permanently delete the '{_category.Name}' Category?",
            "Delete Category",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (confirm == MessageBoxResult.Yes)
        {
            if (_category.TreeViewItem.Parent is TreeViewItem parent)
            {
                parent.Items.Remove(_category.TreeViewItem);
            }
            else if (_category.TreeViewItem.Parent is TreeView)
            {
                MessageBox.Show(
                    "You can not delete this category.",
                    "Delete Category",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            MainWindow.Instance.RootCategory.RemoveSubCategory(_category);
        }
    }

    public event EventHandler CanExecuteChanged;
    
}