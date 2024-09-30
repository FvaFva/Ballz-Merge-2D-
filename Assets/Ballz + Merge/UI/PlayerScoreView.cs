using UnityEngine;

public class PlayerScoreView : MonoBehaviour
{
    [SerializeField] private ValueView _score;
    [SerializeField] private PlayerScore _playerScore;

    private void Awake()
    {
        UpdateScore();
    }

    private void OnEnable()
    {
        _playerScore.ScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        _playerScore.ScoreChanged += UpdateScore;
    }

    private void UpdateScore()
    {
        _score.Show(_playerScore.Score);
    }
}
