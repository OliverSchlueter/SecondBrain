using System;
using System.Collections.Generic;
using System.Windows;

namespace DesktopApp.notes
{
    public class ContactNote : Note
    {
        public string Number { get; }
        
        public string FirstName { get; }
        
        public string LastName { get; }

        public ContactNote(DateTime timeCreated, List<string> tags, string number, string firstName, string lastName) : base(NoteType.Contact, firstName + " " + lastName, tags, timeCreated)
        {
            Number = number;
            FirstName = firstName;
            LastName = lastName;
        }

        public ContactNote(DateTime timeCreated, string number, string firstName, string lastName) : base(NoteType.Contact, firstName + " " + lastName, timeCreated)
        {
            Number = number;
            FirstName = firstName;
            LastName = lastName;
        }

        public override void AddDefaultTags()
        {
            base.AddDefaultTags();
            Tags.Add(Number);
        }

        public override void OnClick()
        {
            MessageBox.Show($"Clicked {Name}");
        }

        public override string ToString()
        {
            return $"[C] {FirstName} {LastName} {Number}";
        }
    }
}