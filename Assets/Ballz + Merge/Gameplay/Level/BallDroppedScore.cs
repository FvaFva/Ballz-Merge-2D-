using BallzMerge.Data;
using BallzMerge.Gameplay.BallSpace;
using System;
using UnityEngine;
using Zenject;

public class BallDroppedScore : CyclicBehavior, ILevelStarter, IHistorical, ISaveDependedObject, IValueViewScore
{
    [Inject] private Ball _ball;

    private const string BallScore = "BallScore";

    private int _ballScore;

    public event Action<IValueViewScore, int, int> ScoreChanged;

    private void OnEnable()
    {
        _ball.LeftGame += OnBallLeftGame;
    }

    private void OnDisable()
    {
        _ball.LeftGame -= OnBallLeftGame;
    }

    public void StartLevel(bool isAfterLoad = false)
    {
        if (isAfterLoad == false)
            _ballScore = 0;

        ScoreChanged?.Invoke(this, _ballScore, 0);
    }

    public GameHistoryData Write(GameHistoryData data)
    {
        data.Score = _ballScore;
        return data;
    }

    public void Load(SaveDataContainer save)
    {
        _ballScore = Mathf.RoundToInt(save.Get(BallScore));
        ScoreChanged?.Invoke(this, _ballScore, 0);
    }

    public void Save(SaveDataContainer save)
    {
        save.Set(BallScore, _ballScore);
    }

    private void OnBallLeftGame()
    {
        _ballScore++;
        ScoreChanged?.Invoke(this, _ballScore, 0);
    }
}
