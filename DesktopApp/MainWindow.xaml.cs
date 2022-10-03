using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        
        private readonly List<string> _autocompleteSuggestions = new List<string> {"Pdf File","AVI File","JPEG file","MP3 sound","MP4 Video"};

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
                            "Seach", 
                            MessageBoxButton.OKCancel, 
                            MessageBoxImage.Information, 
                            MessageBoxResult.OK);
                        break;
                    case Key.Up:
                        if (ListBoxAutoCompleteResults.SelectedIndex > 0)
                        {
                            ListBoxAutoCompleteResults.SelectedIndex--;   
                        }
                        break;
                    case Key.Down:
                        if (ListBoxAutoCompleteResults.SelectedIndex < ListBoxAutoCompleteResults.Items.Count)
                        {
                            ListBoxAutoCompleteResults.SelectedIndex++;
                        }

                        break;
                    default:
                        if (TextBoxSearch.Text.Trim() != "")
                        {
                            PopupAutoComplete.IsOpen = true;
                            PopupAutoComplete.Visibility = Visibility.Visible;
                            ListBoxAutoCompleteResults.ItemsSource = _autocompleteSuggestions.Where(td => td.Trim().ToLower().Contains(TextBoxSearch.Text.Trim().ToLower()));
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