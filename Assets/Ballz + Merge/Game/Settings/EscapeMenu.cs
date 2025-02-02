using System;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Root.Settings
{
    public class EscapeMenu: MonoBehaviour
    {
        [SerializeField] private Slider _audioSettings;
        [SerializeField] private Slider _timeSettings;
        [SerializeField] private Button _continueButton;
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
            _leftGame.gameObject.SetActive(isActiveQuite);
        }

        public void UpdateFromData(float audio, float time)
        {
            _audioSettings.value = audio;
            _timeSettings.value = time;
        }

        private void LeftToMainMenu() => RequireQuite(new SceneExitData(ScenesNames.MAINMENU));

        private void OpenSettings() => SettingsRequired?.Invoke();

        private void LeftGame() => RequireQuite(new SceneExitData(true));

        private void CloseMenu() => CloseRequired?.Invoke();

        private void ChangeButtonSubscribe(bool isActive)
        {
            _continueButton.ChangeListeningState(CloseMenu, isActive);
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
