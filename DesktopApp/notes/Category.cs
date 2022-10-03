using System;
using System.Collections.Generic;
using System.Windows.Controls;

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
        
        public void ToTreeView(TreeView root, TreeViewItem currentItem = null)
        {
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
                        Header = subCategory.Name
                    };
                    root.Items.Add(sub);
                    subCategory.ToTreeView(root, sub);
                }
                return;
            }
            
            if (currentItem == null)
            {
                currentItem = new TreeViewItem
                {
                    Header = Name
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
                    Header = subCategory.Name
                };
                currentItem.Items.Add(sub);
                subCategory.ToTreeView(root, sub);
            }
        }

    }
}