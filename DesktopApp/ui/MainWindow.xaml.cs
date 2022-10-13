using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopApp.notes;

using DesktopApp.utils;

namespace DesktopApp.ui
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

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _rootCategory.Save($"{App.DataFolderPath}data");
        }
    }
}