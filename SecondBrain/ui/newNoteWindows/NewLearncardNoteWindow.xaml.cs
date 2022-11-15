using System;
using System.Windows;
using System.Windows.Controls;
using SecondBrain.notes;

namespace SecondBrain.ui.newNoteWindows;

public partial class NewLearncardNoteWindow : Window
{
    private readonly Category<Note> _category;

    public NewLearncardNoteWindow(Category<Note> category)
    {
        _category = category;

        InitializeComponent();
    }

    private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
    {
        var topic = TextBoxTopic.Text;
        var question = TextBoxQuestion.Text;
        var answer = TextBoxAnswer.Text;
        
        if (topic.Length == 0)
        {
            MessageBox.Show(
                "Please fill out the 'topic' field", 
                "Create new learncard note",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }
        
        if (question.Length == 0)
        {
            MessageBox.Show(
                "Please fill out the 'question' field", 
                "Create new learncard note",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }
        
        if (answer.Length == 0)
        {
            MessageBox.Show(
                "Please fill out the 'answer' field", 
                "Create new learncard note",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        var learncardNote = new LearncardNote(DateTime.Now, topic, question, answer);
        learncardNote.AddDefaultTags();
        
        _category.Values.Add(learncardNote);
        
        _category.TreeViewItem.Items.Add(new TreeViewItem
        {
            Header = learncardNote
        });
        
        MessageBox.Show(
            $"Successfully created new learncard note.", 
            "Created new note",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        
        Close();
    }
}