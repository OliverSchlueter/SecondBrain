using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DesktopApp.notes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        Category<string> rootCategory = new Category<string>("");
        public MainWindow()
        {
            InitializeComponent();

            
            var sub1 = rootCategory.AddSubCategory("Contacts");
            sub1.Values.Add("Oliver Schlüter");
            sub1.Values.Add("Max Mustermann");
            var sub2 = rootCategory.AddSubCategory("Notes");
            sub2.Values.Add("Note1");
            sub2.Values.Add("Note2");
            rootCategory.ToTreeView(TreeViewOverview);
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
                        //var suggestions = _autocompleteSuggestions.Where(td => td.Trim().ToLower().Contains(TextBoxSearch.Text.Trim().ToLower())).ToList();
                        var suggestions = rootCategory.SearchItems(i =>
                            i.Trim().ToLower().Contains(TextBoxSearch.Text.Trim().ToLower()));
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