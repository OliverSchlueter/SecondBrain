using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace DesktopApp.notes
{
    public class PlaintextNote : Note, ILoadable
    {
        [JsonIgnore]
        public string Content { get; set; }
        
        public string PathToContent { get; set; }
        
        public PlaintextNote(string name, List<string> tags, DateTime timeCreated, string pathToContent) : base(NoteType.Plaintext, name, tags, timeCreated)
        {
            PathToContent = pathToContent;
        }
        
        public PlaintextNote(string name, DateTime timeCreated, string pathToContent) : base(NoteType.Plaintext, name, timeCreated)
        {
            PathToContent = pathToContent;
        }
        
        public override string ToJson()
        {
            throw new NotImplementedException();
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
            return $"[PT] {Name}";
        }

        public void Load()
        {
            if (IsLoaded)
            {
                throw new AlreadyLoadedException(this);
            }

            Content = File.ReadAllText(PathToContent);
            
            IsLoaded = true;
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