using System.Collections.Generic;

public class GameSaves
{
    public void SaveGame(IEnumerable<ILevelSaver> savers)
    {
        foreach (ILevelSaver saver in savers)
            saver.GetSavingData();
    }
}