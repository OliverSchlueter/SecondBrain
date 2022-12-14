using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace SecondBrain.notes
{
    public class PlaintextNote : Note, ILoadable
    {
        [JsonIgnore]
        public string Content { get; set; }
        
        public string PathToContent { get; }
        
        public PlaintextNote(string name, DateTime timeCreated, string content, string pathToContent) : base(NoteType.Plaintext, name, timeCreated)
        {
            Content = content;
            IsLoaded = true;
            PathToContent = pathToContent;
        }
        
        public PlaintextNote(string name, List<string> tags, DateTime timeCreated, string pathToContent) : base(NoteType.Plaintext, name, tags, timeCreated)
        {
            PathToContent = pathToContent;
        }
        
        public PlaintextNote(string name, DateTime timeCreated, string pathToContent) : base(NoteType.Plaintext, name, timeCreated)
        {
            PathToContent = pathToContent;
        }

        public override void OnClick()
        {
            if (!IsLoaded)
            {
                Load();
            }
            
            MessageBox.Show($"Clicked {Name}");
            MessageBox.Show(Content, $"Content of note: {Name}");
        }

        public override string ToString()
        {
            return $"{Name}.txt";
        }

        public override void Remove()
        {
            base.Remove();
            if (File.Exists(PathToContent))
            {
                File.Delete(PathToContent);
            }
        }

        public void Load()
        {
            if (IsLoaded)
            {
                throw new AlreadyLoadedException(this);
            }

            if (File.Exists(PathToContent))
            {
                Content = File.ReadAllText(PathToContent);
                IsLoaded = true;   
            }
            else
            {
                Content = "Could not load contents";
            }
        }

        public void Unload()
        {
            if (!IsLoaded)
            {
                throw new AlreadyUnloadedException(this);
            }

            Content = string.Empty;
            
            IsLoaded = false;
        }

        public void Reload()
        {
            Unload();
            Load();
        }
    }
}