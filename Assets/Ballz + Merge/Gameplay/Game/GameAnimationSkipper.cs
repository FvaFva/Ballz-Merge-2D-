using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Root;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameAnimationSkipper : MonoBehaviour
{
    [SerializeField] private Button _skipButton;

    [Inject] private BallzMerge.Root.Settings.GameSettingsDataProxyAudio _audio;
    [Inject] private IGamePauseController _scaler;
    [Inject] private UIRootView _rootView;
    [Inject] private Ball _ball;

    private void OnEnable()
    {
        _ball.EnterGame += OnBallEnterGame;
    }

    private void OnDisable()
    {
        _ball.EnterGame -= OnBallEnterGame;
    }

    public void OnBallLeftGame()
    {
        _ball.LeftGame -= OnBallLeftGame;
        _rootView.LoadScreen.Hide();
        _scaler.SetRegular();
        _audio.Enable();
        DeactivateButton();
    }

    private void OnSkipRequired()
    {
        _rootView.LoadScreen.Show();
        _scaler.SpeedUp();
        _audio.Disable();
        DeactivateButton();
    }

    private void OnBallEnterGame()
    {
        _ball.LeftGame += OnBallLeftGame;
        ActivateButton();
    }

    private void ActivateButton()
    {
        _skipButton.onClick.AddListener(OnSkipRequired);
        _skipButton.gameObject.SetActive(true);
    }

    private void DeactivateButton()
    {
        _skipButton.onClick.RemoveListener(OnSkipRequired);
        _skipButton.gameObject.SetActive(false);
    }
}
