using System;
using UnityEngine;

namespace BallzMerge.Root.Settings
{
    public class EscapeMenu : MonoBehaviour
    {
        [SerializeField] private ButtonProperty _continueButton;
        [SerializeField] private ButtonProperty _restartButton;
        [SerializeField] private ButtonProperty _settingsButton;
        [SerializeField] private ButtonProperty _leftToMainMenuButton;
        [SerializeField] private ButtonProperty _quitButton;

        public event Action<SceneExitData> QuitRequired;
        public event Action SettingsRequired;
        public event Action CloseRequired;

        private void OnEnable()
        {
            ChangeButtonSubscribe(true);
        }

        private void OnDisable()
        {
            ChangeButtonSubscribe(false);
        }

        public void UpdateButtonView(bool isActiveQuite, bool isActiveMainMenu)
        {
            _leftToMainMenuButton.ChangeState(isActiveMainMenu);
            _restartButton.ChangeState(isActiveMainMenu);
            _quitButton.ChangeState(isActiveQuite);
        }

        private void LeftToMainMenu() => RequireQuite(new SceneExitData(ScenesNames.MAIN_MENU));

        private void OpenSettings() => SettingsRequired?.Invoke();

        private void LeftGame() => RequireQuite(new SceneExitData(true));

        private void Restart() => RequireQuite(new SceneExitData(ScenesNames.GAMEPLAY));

        private void CloseMenu() => CloseRequired?.Invoke();

        private void ChangeButtonSubscribe(bool isActive)
        {
            _continueButton.ChangeListeningState(CloseMenu, isActive);
            _restartButton.ChangeListeningState(Restart, isActive);
            _settingsButton.ChangeListeningState(OpenSettings, isActive);
            _leftToMainMenuButton.ChangeListeningState(LeftToMainMenu, isActive);
            _quitButton.ChangeListeningState(LeftGame, isActive);
        }

        private void RequireQuite(SceneExitData exitData)
        {
            QuitRequired?.Invoke(exitData);
        }
    }
}
