using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Root;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameAnimationSkipper : MonoBehaviour
{
    [SerializeField] private Button _skipButton;

    [Inject] private BallzMerge.Root.Settings.GameSettingsDataProxyAudio _audio;
    [Inject] private IGameTimeOwner _scaler;
    [Inject] private UIRootView _rootView;
    [Inject] private Ball _ball;

    private bool _inSkip;

    private void OnEnable()
    {
        _ball.EnterGame += OnBallEnterGame;
    }

    private void OnDisable()
    {
        _ball.EnterGame -= OnBallEnterGame;
    }

    private void OnBallLeftGame()
    {
        _ball.LeftGame -= OnBallLeftGame;
        DeactivateButton();

        if (_inSkip)
        {
            _rootView.LoadScreen.Hide();
            _scaler.SetRegular();
            _audio.Enable();
            _inSkip = false;
        }
    }

    private void OnSkipRequired()
    {
        _inSkip = true;
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
        _skipButton.AddListener(OnSkipRequired);
        _skipButton.gameObject.SetActive(true);
    }

    private void DeactivateButton()
    {
        _skipButton.RemoveListener(OnSkipRequired);
        _skipButton.gameObject.SetActive(false);
    }
}
