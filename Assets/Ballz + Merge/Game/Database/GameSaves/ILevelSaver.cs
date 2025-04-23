using System.Collections.Generic;

public interface ILevelSaver
{
    public IDictionary<string, float> GetSavingData();
    public void Load(IDictionary<string, float> data);
}