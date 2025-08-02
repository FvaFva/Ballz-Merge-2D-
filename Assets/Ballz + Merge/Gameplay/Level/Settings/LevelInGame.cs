using BallzMerge.Data;
using Zenject;

public class LevelInGame : CyclicBehavior, ISaveDependedObject, IInitializable, IHistorical
{
    private const string SaveKey = "LevelSetting";

    [Inject] private LevelSettingsMap _map;
    [Inject] private LevelSettingsContainer container;

    public LevelSettings Current { get; private set; }

    public void Load(SaveDataContainer save) => Current = _map.GetSetting(save.GetInt(SaveKey));

    public void Save(SaveDataContainer save) => save.Set(SaveKey, Current.ID);

    public void Init() => Current = container.Get();

    public GameHistoryData Write(GameHistoryData data)
    {
        data.Level = Current.ID;
        return data;
    }
}
