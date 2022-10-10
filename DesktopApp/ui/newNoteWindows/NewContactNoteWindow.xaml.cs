using System;
using System.Windows;
using System.Windows.Controls;
using DesktopApp.notes;

namespace DesktopApp.ui.newNoteWindows;

public partial class NewContactNoteWindow : Window
{
    private readonly Category<Note> _category;
    private readonly NoteType _noteType;
    
    public NewContactNoteWindow(Category<Note> category, NoteType noteType)
    {
        _category = category;
        _noteType = noteType;
        
        InitializeComponent();
    }

    private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
    {
        var firstName = TextBoxFirstName.Text;
        var lastName = TextBoxLastName.Text;
        var telNumber = TextBoxNumber.Text;

        if (firstName.Length == 0)
        {
            MessageBox.Show(
                "Please fill out the 'First Name' field", 
                "Create new contact note",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }
        
        if (telNumber.Length == 0)
        {
            MessageBox.Show(
                "Please fill out the 'Tel. Number' field", 
                "Create new contact note",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        var contactNote = new ContactNote(DateTime.Now, telNumber, firstName, lastName);
        contactNote.AddDefaultTags();
        
        _category.Values.Add(contactNote);
        
        _category.TreeViewItem.Items.Add(new TreeViewItem
        {
            Header = contactNote
        });
        
        MessageBox.Show(
            $"Successfully created new contact note.", 
            "Created new sub-category",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        
        Close();
    }
}