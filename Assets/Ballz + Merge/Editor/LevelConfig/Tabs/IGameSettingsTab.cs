namespace BallzMerge.Editor
{
    public interface IGameSettingsTab
    {
        public string Title { get; }
        public void OnGUI();
        public IGameSettingsTab LoadData(AssetData<LevelSettings> settings);
    }
}