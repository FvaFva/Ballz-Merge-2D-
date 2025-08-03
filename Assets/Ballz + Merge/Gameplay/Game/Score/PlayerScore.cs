using System;
using BallzMerge.Data;
using UnityEngine;

public class PlayerScore : CyclicBehavior, ILevelStarter, ISaveDependedObject, IWaveUpdater, IHistorical
{
    private const string ScoreProp = "PlayerScore";

    private int _score;

    public event Action<int> ScoreChanged;

    public void StartLevel(bool isAfterLoad = false)
    {
        if (isAfterLoad == false)
            _score = 0;

        ScoreChanged?.Invoke(_score);
    }

    public void Load(SaveDataContainer save) => _score = Mathf.RoundToInt(save.Get(ScoreProp));

    public void Save(SaveDataContainer save) => save.Set(ScoreProp, _score);

    public void UpdateWave()
    {
        _score++;
        ScoreChanged?.Invoke(_score);
    }

    public GameHistoryData Write(GameHistoryData data)
    {
        data.Score = _score;
        return data;
    }
}