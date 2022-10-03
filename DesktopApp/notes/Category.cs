using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopApp.notes
{
    public class Category<T>
    {
        public string Name { get; private set; }
        public List<T> Values { get; set; }
        public List<Category<T>> SubCategories { get; set; }

        public Category(string name)
        {
            Name = name;
            Values = new List<T>();
            SubCategories = new List<Category<T>>();
        }

        public Category<T> AddSubCategory(string name)
        {
            var category = new Category<T>(name);
            
            SubCategories.Add(category);

            return category;
        }

        public List<T> SearchItems(Func<T, bool> condition)
        {
            var results = new List<T>();
            
            foreach (var value in Values)
            {
                if (condition(value))
                {
                    results.Add(value);
                }
            }
            
            foreach (var subCategory in SubCategories)
            {
                var subItem = subCategory.SearchItems(condition);
                results.AddRange(subItem);
            }

            return results;
        }

        public TreeView ToTreeView()
        {
            var treeView = new TreeView();
            ToTreeView(treeView);
            return treeView;
        }
        public void ToTreeView(TreeView root, TreeViewItem currentItem = null)
        {
            root.MouseDoubleClick += (sender, args) =>
            {
                if (root.SelectedItem is Note note)
                {
                    note.OnClick();
                }
                
                args.Handled = true;
            };
            
            if (Name.Equals(string.Empty))
            {
                foreach (var value in Values)
                {
                    root.Items.Add(value);
                }

                foreach (var subCategory in SubCategories)
                {
                    var sub = new TreeViewItem
                    {
                        Header = subCategory,
                        Margin = new Thickness(0, 3, 0, 3)
                    };

                    sub.Resources.Add(SystemColors.HighlightBrushKey, new SolidColorBrush(Color.FromRgb(145, 242, 172)));
                    sub.Resources.Add(SystemColors.HighlightTextBrushKey, new SolidColorBrush(Color.FromRgb(21, 133, 51)));
                    
                    root.Items.Add(sub);
                    subCategory.ToTreeView(root, sub);
                }
                return;
            }

            if (currentItem == null)
            {
                currentItem = new TreeViewItem
                {
                    Header = this
                };
                
                root.Items.Add(currentItem);
            }

            foreach (var value in Values)
            {
                currentItem.Items.Add(value);
            }

            foreach (var subCategory in SubCategories)
            {
                var sub = new TreeViewItem
                {
                    Header = subCategory
                };
                currentItem.Items.Add(sub);
                subCategory.ToTreeView(root, sub);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}