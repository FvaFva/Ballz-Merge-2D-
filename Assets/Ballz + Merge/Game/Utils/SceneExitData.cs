public struct SceneExitData
{
    public bool IsGameQuit;
    public string TargetScene;

    public SceneExitData(bool isExit)
    {
        IsGameQuit = isExit;
        TargetScene = null;
    }

    public SceneExitData(string targetScene)
    {
        TargetScene = targetScene;
        IsGameQuit = false;
    }
}