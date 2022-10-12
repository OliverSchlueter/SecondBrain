using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DesktopApp.notes.commands;
using DesktopApp.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DesktopApp.notes
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
                _treeViewItem.Foreground = new SolidColorBrush(Colors.Green);
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
                        
                        var confirm = MessageBox.Show(
                            $"Do you want to permanently delete the '{note.ToString()}' Note?",
                            "Delete Note",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning
                        );

                        if (confirm == MessageBoxResult.Yes)
                        {
                            _treeViewItem.Items.Remove(source);
                            Values.Remove(note);
                        }
                        
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
                                    new MenuItem { Header = "Plaintext", Command = new AddNoteCommand(category, NoteType.Plaintext), IsEnabled = false },
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

                    sub.Resources.Add(SystemColors.HighlightBrushKey, new SolidColorBrush(Color.FromRgb(145, 242, 172)));
                    sub.Resources.Add(SystemColors.HighlightTextBrushKey, new SolidColorBrush(Color.FromRgb(21, 133, 51)));
                    
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

        public static Category<Note> DeserializeNoteCategory(JObject json)
        {
            if (!CheckForRequiredKeys(new[] { "CategoryType", "Name", "Values", "SubCategories" }, json))
            {
                throw new Exception("Missing at least one key");
            }

            var categoryType = json["CategoryType"].ToString();

            if (categoryType != "Note")
            {
                throw new Exception("Trying to deserialize category with invalid type. Expected: 'Note'");
            }
            
            var category = new Category<Note>(json["Name"].ToString());

            foreach (JObject prop in json["Values"])
            {
                category.Values.Add(DeserializeNote(prop));
            }
            
            foreach (JObject prop in json["SubCategories"])
            {
                category.SubCategories.Add(DeserializeNoteCategory(prop));
            }
                
            return category;
        }

        private static Note DeserializeNote(JObject json)
        {
            if (!CheckForRequiredKeys(new[] { "NoteType", "Name", "Tags", "TimeCreated" }, json))
            {
                throw new Exception("Missing at least one key");
            }
            
            var parsedType = NoteType.TryParse(json["NoteType"].ToString(), out NoteType noteType);

            if (!parsedType)
            {
                throw new Exception("Could not find note type");
            }
            
            Note note = null;
            
            switch (noteType)
            {
                case NoteType.Plaintext:
                    if (!CheckForRequiredKeys(new[] { "PathToContent" }, json))
                    {
                        throw new Exception("Missing at least one key");
                    }

                    note = new PlaintextNote(
                        json["Name"].ToString(), 
                        ((JArray) json["Tags"]).ToObject<List<string>>(),
                        DateTime.Parse(json["TimeCreated"].ToString()), 
                        json["PathToContent"].ToString());
                    break;
                case NoteType.Contact:
                    note = new ContactNote(
                        DateTime.Parse(json["TimeCreated"].ToString()),
                        ((JArray) json["Tags"]).ToObject<List<string>>(),
                        json["Number"].ToString(),
                        json["FirstName"].ToString(),
                        json["LastName"].ToString());
                    break;
            }

            if (note == null)
            {
                throw new Exception("Invalid note type");
            }

            return note;
        }

        private static bool CheckForRequiredKeys(string[] requiredKeys, JObject json)
        {
            foreach (var requiredKey in requiredKeys)
            {
                if (!json.ContainsKey(requiredKey))
                {
                    return false;
                }
            }

            return true;
        }
    }
}