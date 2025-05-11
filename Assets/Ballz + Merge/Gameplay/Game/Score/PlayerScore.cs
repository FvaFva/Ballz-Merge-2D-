using BallzMerge.Data;
using System;
using System.Collections.Generic;
using Zenject;

public class PlayerScore : CyclicBehavior, ILevelLoader, ILevelFinisher, IInitializable, IWaveUpdater
{
    [Inject] private DataBaseSource _data;
    [Inject] private BallWaveVolume _waveVolume;

    private int _bestScore;

    private int _score;
    public event Action<int> ScoreChanged;

    public void Init()
    {
        _bestScore = _data.History.GetBestScore();
        ScoreChanged?.Invoke(_score);
    }

    public void StartLevel()
    {
        _score = 0;
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