using BallzMerge.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Zenject;

public class PlayerScore : CyclicBehavior, IInitializable, ILevelSaver, ILevelLoader, IWaveUpdater, ILevelFinisher
{
    private const string Score = "PlayerScore";

    [Inject] private DataBaseSource _data;
    [Inject] private BallWaveVolume _waveVolume;

    private int _bestScore;

    private int _score;
    public event Action<int> ScoreChanged;

    public void Init()
    {
        _bestScore = _data.History.GetBestScore();
    }

    public void StartLevel()
    {
        _score = 0;
        ScoreChanged?.Invoke(_score);
    }

    public void Load(IDictionary<string, object> data)
    {
        _score = JsonConvert.DeserializeObject<int>(data[Score].ToString());
        ScoreChanged?.Invoke(_score);
    }

    public IDictionary<string, object> GetSavingData()
    {
        return new Dictionary<string, object>
        {
            { Score, _score }
        };
    }

    public void FinishLevel()
    {
        if (_score == 0)
            return;

        _data.History.SaveResult(_score, _waveVolume.Bag);

        if (_score > _bestScore)
            _bestScore = _score;

        ScoreChanged?.Invoke(_score);
    }

    public void UpdateWave()
    {
        _score++;
        ScoreChanged?.Invoke(_score);
    }
}