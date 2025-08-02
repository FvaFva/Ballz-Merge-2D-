using System.Collections.Generic;
using BallzMerge.Data;

public struct SceneExitData
{
    public bool IsGameQuit;
    public string TargetScene;
    public SaveDataContainer Save { get; private set; }
    public GameHistoryData History;
    public bool IsLoad;

    public SceneExitData(bool isExit)
    {
        IsGameQuit = isExit;
        TargetScene = null;
        Save = null;
        IsLoad = false;
        History = default;
    }

    public SceneExitData(string targetScene, bool isLoad = false)
    {
        TargetScene = targetScene;
        IsGameQuit = false;
        IsLoad = isLoad;
        Save = null;
        History = default;
    }

    public void Put(SaveDataContainer saver)
    {
        Save = saver;
    }

    public void Put(GameHistoryData history) => History = history;
}