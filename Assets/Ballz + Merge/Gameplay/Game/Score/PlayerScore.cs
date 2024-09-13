using BallzMerge.Data;
using System;
using Zenject;

public class PlayerScore : CyclicBehavior, ILevelStarter, ILevelFinisher, IInitializable, IWaveUpdater
{
    [Inject] private DataBaseSource _data;
    [Inject] private BallWaveVolume _waveVolume;

    public int Score { get; private set; }
    public int BestScore { get; private set; }
    public event Action ScoreChanged;

    public void Init()
    {
        BestScore = _data.History.GetBestScore();
        ScoreChanged?.Invoke();
    }

    public void StartLevel()
    {
        Score = 0;
    }

    public void FinishLevel()
    {
        if (Score == 0)
            return;

        _data.History.SaveResult(Score, _waveVolume.GlobalVolumes);

        if (Score > BestScore)
            BestScore = Score;

        ScoreChanged?.Invoke();
    }

    public void UpdateWave()
    {
        Score++;
        ScoreChanged?.Invoke();
    }
}