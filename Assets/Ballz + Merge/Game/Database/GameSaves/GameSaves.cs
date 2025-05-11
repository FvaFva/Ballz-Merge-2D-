using BallzMerge.Data;
using System.Collections.Generic;

public class GameSaves
{
    private GameSavesStorage _db;

    public GameSaves(DataBaseSource db)
    {
        _db = db.Saves;
    }

    public void SaveGame(IEnumerable<ILevelSaver> savers)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        foreach (ILevelSaver saver in savers)
        {
            IDictionary<string, object> savingData = saver.GetSavingData();

            if (savingData != null)
            {
                foreach (KeyValuePair<string, object> save in savingData)
                {
                    data.Add(save.Key, save.Value);
                }
            }
        }

        _db.Save(data);
    }

    public IDictionary<string, object> Load()
    {
        return _db.Get();
    }
}