using System;

namespace SecondBrain.notes;

public class AlreadyLoadedException : Exception
{
    private Note _note;
    
    public AlreadyLoadedException(Note note) : base($"Note '{note.Name}' is already loaded")
    {
        _note = note;
    }
}