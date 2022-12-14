using System.Windows;
using System.Windows.Controls;
using SecondBrain.notes;

namespace SecondBrain.ui;

public partial class NewCategoryWindow : Window
{
    private readonly Category<Note> _rootCategory;
    
    public NewCategoryWindow(Category<Note> rootCategory)
    {
        _rootCategory = rootCategory;
        InitializeComponent();
        LabelHeadline.Content = LabelHeadline.Content.ToString() + " " + _rootCategory.Name;
    }

    private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
    {
        var name = TextBoxName.Text;

        if (name.Length == 0) return;

        var subCategory = _rootCategory.AddSubCategory(name);
        
        var newTreeView = new TreeViewItem
        {
            Header = subCategory,
            Margin = new Thickness(0, 3, 0, 3)
        };
        
        subCategory.TreeViewItem = newTreeView;
        
        _rootCategory.TreeViewItem.Items.Add(newTreeView);

        MessageBox.Show(
            $"Successfully created new sub-category: '{name}'.", 
            "Created new sub-category",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        
        Close();
    }
}