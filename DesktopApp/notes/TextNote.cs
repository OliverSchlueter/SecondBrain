using System;

namespace DesktopApp.notes
{
    public class TextNote : Note
    {
        public TextNote(string content, DateTime timeCreated) : base(content, timeCreated)
        {
        }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}