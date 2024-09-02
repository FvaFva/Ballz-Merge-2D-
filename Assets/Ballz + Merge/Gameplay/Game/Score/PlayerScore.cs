using System;
using UnityEngine;
using Zenject;

public class PlayerScore : CyclicBehavior, ILevelStarter, ILevelFinisher, IInitializable, IWaveUpdater
{
    private const string ScoreName = "Score";

    [Inject] private UserQuestioner _userQuestioner;

    public int Score { get; private set; }
    public int BestScore { get; private set; }
    public event Action ScoreChanged;

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

    public void UpdateWave()
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