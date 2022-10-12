using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DesktopApp.notes
{
    public abstract class Note
    {
        public NoteType NoteType { get; }
        
        public string Name { get; set; }
        
        public List<string> Tags { get; set; }
        
        public DateTime TimeCreated { get; protected set; }
        
        [JsonIgnore]
        public bool IsLoaded { get; protected set; }

        protected Note(NoteType noteType, string name, List<string> tags, DateTime timeCreated)
        {
            NoteType = noteType;
            Name = name;
            Tags = tags;
            TimeCreated = timeCreated;
            IsLoaded = false;
        }

        protected Note(NoteType noteType, string name, DateTime timeCreated)
        {
            NoteType = noteType;
            Name = name;
            TimeCreated = timeCreated;
            IsLoaded = false;
            Tags = new List<string>();
        }

        public abstract void OnClick();

        public virtual void AddDefaultTags()
        {
            Tags.Add(Name);
        }
    }
}