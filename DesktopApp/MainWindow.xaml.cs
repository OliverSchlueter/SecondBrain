using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopApp.notes;
using DesktopApp.utils;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Category<Note> _rootCategory = new("");
        public MainWindow()
        {
            InitializeComponent();

            Title = Title + " " + App.Version;

            var contactsCategory = _rootCategory.AddSubCategory("Contacts");
            contactsCategory.Values.Add(new ContactNote(DateTime.Now, "+49 112233", "Oliver", "Schlüter"));
            contactsCategory.Values.Add(new ContactNote(DateTime.Now, "+49 445566", "Max", "Mustermann"));
            
            var quickNotesCategory = _rootCategory.AddSubCategory("Quick Notes");
            quickNotesCategory.Values.Add(new PlaintextNote("today", DateTime.Now, "today.txt"));


            
            File.WriteAllText("temp.json", JsonUtils.Serialize(_rootCategory));
            //var cat = Category<Note>.DeserializeNoteCategory(JsonUtils.Deserialize(File.ReadAllText("temp.json")));
            //_rootCategory = cat;
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
                    case Key.Enter:
                        MessageBox.Show(
                            "Searching with: '" + TextBoxSearch.Text + "'", 
                            "Search", 
                            MessageBoxButton.OKCancel, 
                            MessageBoxImage.Information, 
                            MessageBoxResult.OK);
                        break;
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
                        var suggestions = _rootCategory.SearchItems(item =>
                        {
                            if (item.Name.Trim().ToLower().Contains(query))
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
    }
}