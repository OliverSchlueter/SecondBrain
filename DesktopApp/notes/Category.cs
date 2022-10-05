using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DesktopApp.utils;
using Newtonsoft.Json.Linq;

namespace DesktopApp.notes
{
    public class Category<T>
    {
        public string CategoryType { get => typeof(T).Name; }
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

        public void Save(string path)
        {
            File.WriteAllText(path, JsonUtils.Serialize(this));
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