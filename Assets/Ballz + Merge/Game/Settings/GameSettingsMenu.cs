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

        public void Add(IGameSettingData settingData, PanelToggleType panelType)
        {
            SliderValueView newSlider = Instantiate(_prefab, _settingsPanelController.GetContent(panelType));
            newSlider.SetProperty(countOfPresets: settingData.CountOfPresets, key: settingData.Name, header: settingData.Name);
            _sliders.Add(settingData, newSlider);
        }

        public void UpdateStartValue(IGameSettingData settingData) => _sliders[settingData].SetStartValues(settingData.Value, settingData.Label);

        public void UpdateLabel(IGameSettingData settingData) => _sliders[settingData].SetLabel(settingData.Label);

        private void OnSliderChanged(string key, float value) => ValueChanged?.Invoke(key, value);
    }
}
