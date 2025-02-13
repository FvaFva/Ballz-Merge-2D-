using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Data
{
    public class GameSettingsMenu : MonoBehaviour
    {
        [SerializeField] private SliderValueView _prefab;
        [SerializeField] private PanelSwitch _settingsPanelController;

        private Dictionary<IGameSettingData, SliderValueView> _sliders = new Dictionary<IGameSettingData, SliderValueView>();
        public event Action<string, float> ValueChanged;

        private void OnEnable()
        {
            foreach (var slider in _sliders.Values)
                slider.ValueChanged += OnSliderChanged;
        }

        private void OnDisable()
        {
            foreach (var slider in _sliders.Values)
                slider.ValueChanged -= OnSliderChanged;
        }

        public void Add(IGameSettingData settingData, PanelToggleType panelType, string suffix = "", int additionalZero = int.MinValue, int pointsAfterDot = int.MinValue, int shift = int.MinValue)
        {
            SliderValueView newSlider = Instantiate(_prefab, _settingsPanelController.GetContent(panelType));
            newSlider.SetProperty(key: settingData.Name, header: settingData.Name, suffix: suffix, additionalZero: additionalZero, pointsAfterDot: pointsAfterDot, shift: shift);
            _sliders.Add(settingData, newSlider);
        }

        public void UpdateValue(IGameSettingData settingData) => _sliders[settingData].SetValue(settingData.Value, settingData.Label);

        private void OnSliderChanged(string key, float value) => ValueChanged?.Invoke(key, value);
    }
}
