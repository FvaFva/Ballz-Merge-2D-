using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Data
{
    public class GameSettingsMenu : MonoBehaviour
    {
        [SerializeField] private PanelSwitch _settingsPanelController;
        [SerializeField] private List<GameSettingProperty> _settingsPrefabs;

        private Dictionary<IGameSettingData, SliderValueView> _sliders = new Dictionary<IGameSettingData, SliderValueView>();
        private Dictionary<GameSettingType, SliderViewProperty> _settingsTypes = new Dictionary<GameSettingType, SliderViewProperty>();
        public event Action<string, float> ValueChanged;

        private void Awake()
        {
            for (int i = 0; i < _settingsPrefabs.Count; i++)
                _settingsTypes.Add(_settingsPrefabs[i].SettingType, _settingsPrefabs[i].SliderViewProperty);
        }

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

        public void AddInstantiate(GameSettingType settingType, IGameSettingData settingData, PanelToggleType panelType)
        {
            SliderValueView newSlider = Instantiate(_settingsTypes[settingType].SliderView, _settingsPanelController.GetContent(panelType)).Init();
            newSlider.RectTransform.sizeDelta = new Vector2(newSlider.RectTransform.sizeDelta.x, _settingsTypes[settingType].Height);
            newSlider.SetProperty(countOfPresets: settingData.CountOfPresets, key: settingData.Name, header: settingData.Name);
            _sliders.Add(settingData, newSlider);
        }

        public void AddExist(SliderValueView slider, IGameSettingData settingData, PanelToggleType panelType)
        {
            slider.SetProperty(countOfPresets: settingData.CountOfPresets, key: settingData.Name, header: settingData.Name);
            _sliders.Add(settingData, slider);
        }

        public void UpdateStartValue(IGameSettingData settingData) => _sliders[settingData].SetStartValues(settingData.Value, settingData.Label);

        public void UpdateLabel(IGameSettingData settingData) => _sliders[settingData].SetLabel(settingData.Label);

        private void OnSliderChanged(string key, float value) => ValueChanged?.Invoke(key, value);
    }
}
