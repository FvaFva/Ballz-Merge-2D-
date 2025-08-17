using System;
using BallzMerge.Data;
using UnityEngine;

public class PlayerScore : CyclicBehavior, ILevelStarter, ISaveDependedObject, IWaveUpdater, IHistorical, IDependentSettings
{
    private const string ScoreProp = "PlayerScore";

    private int _wave;
    private int _totalWaves;

    public event Action<int, int> ScoreChanged;

    public void StartLevel(bool isAfterLoad = false)
    {
        if (isAfterLoad == false)
            _wave = 1;

        ScoreChanged?.Invoke(_wave, _totalWaves);
    }

    public void Load(SaveDataContainer save) => _wave = Mathf.RoundToInt(save.Get(ScoreProp));

    public void Save(SaveDataContainer save) => save.Set(ScoreProp, _wave);

    public void UpdateWave()
    {
        if (_wave == _totalWaves)
            return;

        ScoreChanged?.Invoke(++_wave, _totalWaves);
    }

    public GameHistoryData Write(GameHistoryData data)
    {
        data.Score = _wave;
        return data;
    }

    public void ApplySettings(LevelSettings settings)
    {
        _totalWaves = settings.BlocksSettings.SpawnProperties.Count;
    }
}