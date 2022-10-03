using System;

namespace DesktopApp.notes;

public class AlreadyLoadedException : Exception
{
    public Note Note { get; private set; }
    
    public AlreadyLoadedException(Note note) : base($"Note '{note.Name}' is already loaded")
    {
        Note = note;
    }
}