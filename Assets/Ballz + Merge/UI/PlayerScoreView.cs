using UnityEngine;

public class PlayerScoreView : MonoBehaviour
{
    [SerializeField] private ValueView _score;
    [SerializeField] private PlayerScore _playerScore;

    private void OnEnable()
    {
        _playerScore.ScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        if (_playerScore != null)
            _playerScore.ScoreChanged -= UpdateScore;
    }

    private void UpdateScore(int current, int total)
    {
        _score.Show(current, total);
    }
}
