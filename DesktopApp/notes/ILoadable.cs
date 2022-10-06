namespace DesktopApp.notes;

public interface ILoadable
{
    void Load();
    
    void Unload();
    
    void Reload();
}