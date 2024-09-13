using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Root.Settings
{
    public class SettingsMenuView: MonoBehaviour
    {
        [SerializeField] private Slider _audioSettings;
        [SerializeField] private Slider _timeSettings;
        [SerializeField] private Button _saveSettings;
        [SerializeField] private Button _leftToMainMenu;
        [SerializeField] private Button _leftGame;
        [SerializeField] private RectTransform _menu;
        [SerializeField] private List<Button> _activityChangers;

        public event Action<SceneExitData> QuitRequired;
        public event Action<float, float> SettingsChanged;

        private void Awake()
        {
            _menu.gameObject.SetActive(false);
        }

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

        public void ChangeActivity()
        {
            bool newState = !_menu.gameObject.activeSelf;
            _menu.gameObject.SetActive(newState);
        }

        private void LeftToMainMenu() => RequireQuite(new SceneExitData(ScenesNames.MAINMENU));

        private void LeftGame() => RequireQuite(new SceneExitData(true));

        private void SaveSettings() => SettingsChanged?.Invoke(_audioSettings.value, _timeSettings.value);

        private void ChangeButtonSubscribe(bool isActive)
        {
            _saveSettings.ChangeListeningState(SaveSettings, isActive);
            _leftToMainMenu.ChangeListeningState(LeftToMainMenu, isActive);
            _leftGame.ChangeListeningState(LeftGame, isActive);

            foreach (var activity in _activityChangers)
                activity.ChangeListeningState(ChangeActivity, isActive);
        }

        private void RequireQuite(SceneExitData exitData)
        {
            QuitRequired?.Invoke(exitData);
            ChangeActivity();
        }
    }
}
