using System;
using UnityEngine;

public class WaveScore : CyclicBehavior, ILevelStarter, ISaveDependedObject, IWaveUpdater, IDependentLevelSetting, IValueViewScore
{
    private const string ScoreProp = "WaveScore";

    private int _wave;
    private int _totalWaves;

    public event Action<IValueViewScore, int, int> ScoreChanged;

    public void StartLevel(bool isAfterLoad = false)
    {
        if (isAfterLoad == false)
            _wave = 1;

        ScoreChanged?.Invoke(this, _wave, _totalWaves);
    }

    public void Load(SaveDataContainer save)
    {
        _wave = Mathf.RoundToInt(save.Get(ScoreProp));
        ScoreChanged?.Invoke(this, _wave, _totalWaves);
    }

    public void Save(SaveDataContainer save) => save.Set(ScoreProp, _wave);

    public void UpdateWave()
    {
        if (_wave == _totalWaves)
            return;

        ScoreChanged?.Invoke(this, ++_wave, _totalWaves);
    }

    public void ApplySettings(LevelSettings settings)
    {
        _totalWaves = settings.BlocksSettings.SpawnProperties.Count;
    }
}