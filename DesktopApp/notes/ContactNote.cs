using System;
using System.Windows;

namespace DesktopApp.notes
{
    public class ContactNote : Note
    {
        public string Number { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        

        public ContactNote(DateTime timeCreated, string number, string firstName, string lastName) : base(firstName + " " + lastName, timeCreated)
        {
            Number = number;
            FirstName = firstName;
            LastName = lastName;
            
            Tags.Add(Number);
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
            return $"[C] {FirstName} {LastName} {Number}";
        }
    }
}