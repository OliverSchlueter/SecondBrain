using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DesktopApp.notes;

namespace DesktopApp.ui.newNoteWindows;

public partial class NewPlaintextNoteWindow : Window
{
    private readonly Category<Note> _category;

    public NewPlaintextNoteWindow(Category<Note> category)
    {
        _category = category;

        InitializeComponent();
    }

    private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
    {
        var noteName = TextBoxNoteName.Text;

        if (noteName.Length == 0)
        {
            MessageBox.Show(
                "Please fill out the 'Note Name' field", 
                "Create new contact note",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }
        
        var content = TextBoxContent.Text;
        
        var contentFilePath = $"{App.DataFolderPath}plaintext-notes/{noteName}.txt";
        if (!Directory.Exists($"{App.DataFolderPath}plaintext-notes/"))
        {
            Directory.CreateDirectory($"{App.DataFolderPath}plaintext-notes/");
        }

        var fs = File.Create(contentFilePath);
        var writeTask = fs.WriteAsync(Encoding.UTF8.GetBytes(content), 0, content.Length);
        writeTask.ContinueWith(task => { fs.Close(); });

        var plaintextNote = new PlaintextNote(noteName, DateTime.Now, content, contentFilePath);
        plaintextNote.AddDefaultTags();
        
        _category.Values.Add(plaintextNote);
        
        _category.TreeViewItem.Items.Add(new TreeViewItem
        {
            Header = plaintextNote
        });
        
        MessageBox.Show(
            $"Successfully created new plaintext note.", 
            "Created new note",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        
        Close();
    }
}