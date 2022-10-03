using System;

namespace DesktopApp.notes;

public class AlreadyUnloadedException : Exception
{
    public Note Note { get; private set; }

    public AlreadyUnloadedException(Note note) : base($"Note '{note.Name}' is already unloaded")
    {
        Note = note;
    }
}