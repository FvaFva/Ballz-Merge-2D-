using System;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Root.Settings
{
    public class EscapeMenu: MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _leftToMainMenu;
        [SerializeField] private Button _leftGame;

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
            _leftToMainMenu.gameObject.SetActive(isActiveMainMenu);
            _continueButton.gameObject.SetActive(isActiveMainMenu);
            _restartButton.gameObject.SetActive(isActiveMainMenu);
            _leftGame.gameObject.SetActive(isActiveQuite);
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
            _leftToMainMenu.ChangeListeningState(LeftToMainMenu, isActive);
            _leftGame.ChangeListeningState(LeftGame, isActive);
        }

        private void RequireQuite(SceneExitData exitData)
        {
            QuitRequired?.Invoke(exitData);
        }
    }
}
