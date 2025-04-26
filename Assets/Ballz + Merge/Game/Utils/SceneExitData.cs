using System.Collections.Generic;

public struct SceneExitData
{
    public bool IsGameQuit;
    public string TargetScene;
    public List<ILevelSaver> Savers;
    public bool IsLoad;

    public SceneExitData(bool isExit)
    {
        IsGameQuit = isExit;
        TargetScene = null;
        Savers = null;
        IsLoad = false;
    }

    public SceneExitData(string targetScene, bool isLoad = false)
    {
        TargetScene = targetScene;
        IsGameQuit = false;
        IsLoad = isLoad;
        Savers = null;
    }

    public void ConnectSavers(IEnumerable<ILevelSaver> savers)
    {
        Savers = new List<ILevelSaver>(savers);
    }
}