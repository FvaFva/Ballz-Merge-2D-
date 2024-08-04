using UnityEngine;

public class PlayerScoreView : MonoBehaviour
{
    [SerializeField] private InfoPanel _score;
    [SerializeField] private InfoPanel _best;
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
        _best.Show(_playerScore.BestScore);
    }
}
