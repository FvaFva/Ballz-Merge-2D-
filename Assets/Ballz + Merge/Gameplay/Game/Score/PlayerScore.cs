using BallzMerge.Gameplay.BlockSpace;
using System;
using UnityEngine;
using Zenject;

public class PlayerScore : CyclicBehaviour, ILevelStarter, ILevelFinisher, IInitializable
{
    private const string ScoreName = "Score";

    [Inject] private BlocksBus _blocks;
    [Inject] private UserQuestioner _userQuestioner;

    public int Score { get; private set; }
    public int BestScore { get; private set; }
    public event Action ScoreChanged;

    private void OnEnable()
    {
        _blocks.WaveLoaded += OnWaveLoaded;
    }

    private void OnDisable()
    {
        _blocks.WaveLoaded -= OnWaveLoaded;
    }

    public void Init()
    {
        BestScore = PlayerPrefs.GetInt(ScoreName, 0);
        ScoreChanged?.Invoke();
    }

    public void StartLevel()
    {
        Score = 0;
    }

    public void FinishLevel()
    {
        if (Score > BestScore)
        {
            _userQuestioner.Answer += OnUserAnswer;
            _userQuestioner.Show(new UserQuestion(ScoreName, $"{Score} - the best score! Do you want to save?"));
        }
    }

    private void OnWaveLoaded()
    {
        Score++;
        ScoreChanged?.Invoke();
    }

    private void OnUserAnswer(UserQuestion answer)
    {
        if(answer.Name == ScoreName)
        {
            _userQuestioner.Answer -= OnUserAnswer;

            if(answer.IsPositiveAnswer)
            {
                BestScore = Score;
                PlayerPrefs.SetInt(ScoreName, BestScore);
                ScoreChanged?.Invoke();
            }
        }
    }
}