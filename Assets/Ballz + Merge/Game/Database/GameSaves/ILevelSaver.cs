using System;

public interface ILevelSaver
{
    public event Action<string, float> Saved;
    public event Action<ILevelSaver, string> Requested;

    public void Save();
    public void Request();
    public void Load(string key, float value);
    public void Restore();
}