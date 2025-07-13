namespace BallzMerge.Editor
{
    public interface IGameDataTab
    {
        public string Title { get; }
        public void OnGUI();
        public IGameDataTab LoadData();
    }
}