public class GameSaves
{
    private GameSavesStorage _storage;

    public GameSaves(GameSavesStorage storage)
    {
        _storage = storage;
    }

    public void SaveGame(SaveDataContainer saver)
    {
        _storage.Save(saver);
    }
}