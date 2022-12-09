using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SecondBrain.notes.commands;
using SecondBrain.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SecondBrain.notes
{
    public class Category<T>
    {
        public string CategoryType { get => typeof(T).Name; }
        
        public string Name { get; private set; }
        
        public List<T> Values { get; set; }
        
        public List<Category<T>> SubCategories { get; set; }
        
        [JsonIgnore]
        private TreeViewItem _treeViewItem;
        
        [JsonIgnore]
        public TreeViewItem TreeViewItem
        {
            get => _treeViewItem;

            set
            {
                _treeViewItem = value;
                _treeViewItem.KeyUp += (sender, args) =>
                {
                    if (args.Key != Key.Delete)
                    {
                        return;
                    }

                    var source = (TreeViewItem) args.OriginalSource;

                    if (source.Header is Category<Note> category)
                    {
                        var deleteCategoryCommand = new DeleteCategoryCommand(category);
                        deleteCategoryCommand.Execute(null);
                        
                        args.Handled = true;
                    }
                    else if (source.Header is T note)
                    {
                        if (!Values.Contains(note))
                        {
                            return;
                        }
                        
                        var parent = (TreeViewItem) ((TreeViewItem)args.Source).Parent;
                        Category<Note> categoryOfNote = null;

                        if (parent.Header is Category<Note> c)
                        {
                            categoryOfNote = c;
                        }

                        var deleteNoteCommand = new DeleteNoteCommand(categoryOfNote, (Note)source.Header, value);
                        deleteNoteCommand.Execute(null);
                        
                        args.Handled = true;
                    }
                };

            }
        }

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

        public bool RemoveSubCategory(Category<T> subCategory)
        {
            if (SubCategories.Contains(subCategory))
            {
                SubCategories.Remove(subCategory);
                return true;
            }

            foreach (var sc in SubCategories)
            {
                if (sc.RemoveSubCategory(subCategory))
                {
                    return true;
                }
            }

            return false;
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
                if (((TreeViewItem) root.SelectedItem).Header is Note note)
                {
                    note.OnClick();
                }

                args.Handled = true;
            };

            root.MouseRightButtonUp += (sender, args) =>
            {
                if (args.Source is TreeViewItem treeViewItem)
                {
                    treeViewItem.Focus();
                }
                else
                {
                    return;
                }
                
                if (((TreeViewItem) args.Source).Header is Category<Note> category)
                {
                    var contextMenu = new ContextMenu{
                        Placement = PlacementMode.Mouse,
                        IsOpen = true,
                        Visibility = Visibility.Visible,
                        StaysOpen = false,
                        Items =
                        {
                            new MenuItem
                            {
                                Header = "Add Note",
                                Items =
                                {
                                    new MenuItem { Header = "Contact", Command = new AddNoteCommand(category, NoteType.Contact) },
                                    new MenuItem { Header = "Plaintext", Command = new AddNoteCommand(category, NoteType.Plaintext) },
                                    new MenuItem { Header = "Learncard", Command = new AddNoteCommand(category, NoteType.Learncard) },
                                }
                            },
                            new MenuItem { Header = "Add sub-category", Command = new NewCategoryCommand(category) },
                            new MenuItem { Header = "Delete", Command = new DeleteCategoryCommand(category) },
                            new MenuItem { Header = "Rename", IsEnabled = false },
                        }
                    };

                    ((TreeViewItem)args.Source).ContextMenu = contextMenu;
                }
                else if (((TreeViewItem)args.Source).Header is Note note)
                {
                    var parent = (TreeViewItem) ((TreeViewItem)args.Source).Parent;
                    Category<Note> categoryOfNote = null;

                    if (parent.Header is Category<Note> c)
                    {
                        categoryOfNote = c;
                    }
                    
                    var contextMenu = new ContextMenu{
                        Placement = PlacementMode.Mouse,
                        IsOpen = true,
                        Visibility = Visibility.Visible,
                        StaysOpen = false,
                        Items =
                        {
                            new MenuItem { Header = "Delete", Command = new DeleteNoteCommand(categoryOfNote, note, (TreeViewItem)args.Source) },
                            new MenuItem { Header = "Rename", IsEnabled = false },
                        }
                    };

                    ((TreeViewItem)args.Source).ContextMenu = contextMenu;
                }
                
                args.Handled = true;
            };
            
            if (Name.Equals(string.Empty))
            {
                foreach (var value in Values)
                {
                    var item = new TreeViewItem
                    {
                        Header = value
                    };
                    
                    root.Items.Add(item);
                }

                foreach (var subCategory in SubCategories)
                {
                    var sub = new TreeViewItem
                    {
                        Header = subCategory,
                        Margin = new Thickness(0, 3, 0, 3)
                    };

                    sub.Resources.Add(SystemColors.HighlightBrushKey, new SolidColorBrush(Colors.Transparent));
                    sub.Resources.Add(SystemColors.HighlightTextBrushKey, new SolidColorBrush(Color.FromRgb(171, 25, 14)));
                    
                    root.Items.Add(sub);
                    subCategory.TreeViewItem = sub;
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
                var item = new TreeViewItem
                {
                    Header = value
                };
                
                currentItem.Items.Add(item);
            }

            foreach (var subCategory in SubCategories)
            {
                var sub = new TreeViewItem
                {
                    Header = subCategory
                };

                currentItem.Items.Add(sub);
                subCategory.TreeViewItem = sub;
                subCategory.ToTreeView(root, sub);
            }
        }

        public void Save(string path)
        {
            File.WriteAllText(path + ".json", JsonUtils.Serialize(this));
            File.WriteAllText(path + "-backup.json", JsonUtils.Serialize(this));
        }

        public override string ToString()
        {
            return Name;
        }
        
    }
}