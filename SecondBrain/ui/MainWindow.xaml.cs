using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SecondBrain.notes;

using SecondBrain.utils;

namespace SecondBrain.ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }
        
        private readonly Category<Note> _rootCategory = new("");
        
        public Category<Note> RootCategory
        {
            get => _rootCategory;
        }

        public MainWindow()
        {
            if (Instance == null)
                Instance = this;

            InitializeComponent();

            Title = Title + " " + App.Version;

            var contactsCategory = _rootCategory.AddSubCategory("Contacts");
            contactsCategory.Values.Add(new ContactNote(DateTime.Now, "+49 112233", "Oliver", "Schlüter"));
            contactsCategory.Values.Add(new ContactNote(DateTime.Now, "+49 445566", "Max", "Mustermann"));
            
            var quickNotesCategory = _rootCategory.AddSubCategory("Quick Notes");
            quickNotesCategory.Values.Add(new PlaintextNote("today", DateTime.Now, "today.txt"));

            var learnCategory = _rootCategory.AddSubCategory("Learn");
            var sen = learnCategory.AddSubCategory("Software Engineering");
            sen.Values.Add(new LearncardNote(DateTime.Now, "Bits", "What is a bit", "0 or 1"));

            if (File.Exists($"{App.DataFolderPath}data.json"))
            {
                try
                {
                    var cat = DeserializingHelper.DeserializeCategory(JsonUtils.Deserialize(File.ReadAllText($"{App.DataFolderPath}data.json")));
                    _rootCategory = cat;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not load root category data correctly.");
                }
            }


            _rootCategory.ToTreeView(TreeViewOverview);
        }
        
        private void ListBoxAutoCompleteResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListBoxAutoCompleteResults.SelectedItem != null)
                {
                    TextBoxSearch.Text = ListBoxAutoCompleteResults.SelectedValue.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key) 
            {
                case Key.Up:
                    if (ListBoxAutoCompleteResults.SelectedIndex > 0)
                    {
                        ListBoxAutoCompleteResults.SelectedIndex--;   
                    }
                    else
                    {
                        ListBoxAutoCompleteResults.SelectedIndex = ListBoxAutoCompleteResults.Items.Count-1;
                    }
                    break;
                case Key.Down:
                    if (ListBoxAutoCompleteResults.SelectedIndex < ListBoxAutoCompleteResults.Items.Count-1)
                    {
                        ListBoxAutoCompleteResults.SelectedIndex++;
                    }
                    else
                    {
                        ListBoxAutoCompleteResults.SelectedIndex = 0;
                    }

                    break;
                default:
                    var query = TextBoxSearch.Text.Trim().ToLower();
                    var suggestions = SearchNotes(query);

                    if (TextBoxSearch.Text.Trim() != "" && suggestions.Count > 0)
                    {
                        PopupAutoComplete.IsOpen = true;
                        PopupAutoComplete.Visibility = Visibility.Visible;
                        ListBoxAutoCompleteResults.ItemsSource = suggestions;
                    }
                    else
                    {
                        PopupAutoComplete.IsOpen = false;
                        PopupAutoComplete.Visibility = Visibility.Collapsed;
                        ListBoxAutoCompleteResults.ItemsSource = null;
                    }
                    
                    StackPanelSearchResults.Children.Clear();
                    

                    foreach (var suggestion in suggestions)
                    {
                        var item = new Label
                        {
                            Content = suggestion.Name,
                            Visibility = Visibility.Visible,
                            Cursor = Cursors.Hand,
                        };
                        
                        item.MouseLeftButtonUp += (o, args) =>
                        {
                            suggestion.OnClick();
                        };

                        StackPanelSearchResults.Children.Add(item);
                    }
                    break;
                }
            }

        private void TextBoxSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListBoxAutoCompleteResults.SelectedItem != null)
                {
                    TextBoxSearch.Text = ListBoxAutoCompleteResults.SelectedValue.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _rootCategory.Save($"{App.DataFolderPath}data");
        }

        private List<Note> SearchNotes(string query)
        {
            return _rootCategory.SearchItems(item => 
            {
                if (item.Name.Trim().ToLower().Contains(query))
                {
                    return true;
                }

                if (item.ToString().Trim().ToLower().Contains(query))
                {
                    return true;
                }

                foreach (var tag in item.Tags)
                {
                    if (tag.Trim().ToLower().Contains(query))
                    {
                        return true;
                    }
                }
                        
                return false;
            });
        }
    }
}