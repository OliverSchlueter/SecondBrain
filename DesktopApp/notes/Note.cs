using System;

namespace DesktopApp.notes
{
    public abstract class Note
    {
        public string Content { get; protected set; }
        public DateTime TimeCreated { get; protected set; }

        protected Note(string content, DateTime timeCreated)
        {
            Content = content;
            TimeCreated = timeCreated;
        }

        public abstract string ToJson();
    }
}