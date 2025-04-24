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
        Dictionary<string, float> data = new Dictionary<string, float>();

        foreach (ILevelSaver saver in savers) 
            foreach (KeyValuePair<string, float> save in saver.GetSavingData())
                data.Add(save.Key, save.Value);

        _db.Save(data);
    }

    public IDictionary<string, float> Load()
    {
        return _db.Get();
    }
}