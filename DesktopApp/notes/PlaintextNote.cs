using System;
using System.Collections.Generic;
using System.Windows;

namespace DesktopApp.notes
{
    public class PlaintextNote : Note
    {
        public string Content { get; set; }
        
        public PlaintextNote(string name, List<string> tags, DateTime timeCreated, string content) : base(name, tags, timeCreated)
        {
            Content = content;
        }

        public PlaintextNote(string name, DateTime timeCreated, string content) : base(name, timeCreated)
        {
            Content = content;
        }
        
        public override string ToJson()
        {
            throw new NotImplementedException();
        }

        public override void OnClick()
        {
            MessageBox.Show($"Clicked {Name}");
        }

        public override string ToString()
        {
            return $"[PT] {Name}";
        }
    }
}