using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Data
{
    public class GameSettingsMenu : MonoBehaviour
    {
        [SerializeField] private SliderValueView _prefab;
        [SerializeField] private VerticalLayoutGroup _settingsContainer;

        private List<SliderValueView> _sliders = new List<SliderValueView>();
        public event Action<string, float> ValueChanged;

        private void OnEnable()
        {
            foreach (var slider in _sliders)
                slider.ValueChanged += OnSliderChanged;
        }

        private void OnDisable()
        {
            foreach (var slider in _sliders)
                slider.ValueChanged -= OnSliderChanged;
        }

        public void Add(IGameSettingData settingData)
        {
            SliderValueView newSlider = Instantiate(_prefab, _settingsContainer.transform);
            newSlider.SetValue(settingData.Value);
            newSlider.SetProperty(key: settingData.Name, label: settingData.Name);
            _sliders.Add(newSlider);
        }

        private void OnSliderChanged(string key, float value) => ValueChanged?.Invoke(key, value);
    }
}
