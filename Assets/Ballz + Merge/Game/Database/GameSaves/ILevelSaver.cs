using System.Collections.Generic;

public interface ILevelSaver
{
    public IDictionary<string, object> GetSavingData();
}