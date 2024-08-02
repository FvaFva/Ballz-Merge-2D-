using TMPro;
using UnityEngine;

public class PlayerScoreView : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _best;
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
        _score.text = _playerScore.Score.ToString();
        _best.text = _playerScore.BestScore.ToString();
    }
}
