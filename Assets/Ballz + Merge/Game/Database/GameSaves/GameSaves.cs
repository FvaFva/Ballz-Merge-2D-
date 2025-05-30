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
        foreach (ILevelSaver saver in savers)
            saver.GetSavingData();
    }
}