using System;

namespace DesktopApp.notes;

public class AlreadyUnloadedException : Exception
{
    private Note _note;

    public AlreadyUnloadedException(Note note) : base($"Note '{note.Name}' is already unloaded")
    {
        _note = note;
    }
}