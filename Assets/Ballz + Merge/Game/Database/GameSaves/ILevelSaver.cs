using System;

public interface ILevelSaver
{
    public event Action<string, float> Saved;
    public event Action<string> Requested;

    public void Save();
    public void RequestLoad();
    public void Load(string key, float value);
    public void Restore();
}