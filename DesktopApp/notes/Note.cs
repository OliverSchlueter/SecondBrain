using System;
using System.Collections.Generic;

namespace DesktopApp.notes
{
    public abstract class Note
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public DateTime TimeCreated { get; protected set; }

        protected Note(string name, List<string> tags, DateTime timeCreated)
        {
            Name = name;
            Tags = tags;
            Tags.Add(Name);
            TimeCreated = timeCreated;
        }

        protected Note(string name, DateTime timeCreated)
        {
            Name = name;
            TimeCreated = timeCreated;
            Tags = new List<string> { Name };
        }

        public abstract string ToJson();
    }
}