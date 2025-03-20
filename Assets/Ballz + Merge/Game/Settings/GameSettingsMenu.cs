﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Data
{
    public class GameSettingsMenu : MonoBehaviour
    {
        [SerializeField] private PanelSwitch _settingsPanelController;
        [SerializeField] private List<GameSettingProperty> _settingsPrefabs;

        private Dictionary<IGameSettingData, SliderValueView> _sliders = new Dictionary<IGameSettingData, SliderValueView>();
        private Dictionary<GameSettingType, GameSettingProperty> _settingsTypes = new Dictionary<GameSettingType, GameSettingProperty>();

        public event Action<string, float> ValueChanged;
        public event Action Initialized;

        private void Awake()
        {
            for (int i = 0; i < _settingsPrefabs.Count; i++)
                _settingsTypes.Add(_settingsPrefabs[i].SettingType, _settingsPrefabs[i]);
        }

        private void OnEnable()
        {
            foreach (var sliderProperty in _sliders.Values)
            {
                sliderProperty.ValueChanged += OnSliderChanged;
                sliderProperty.Initialized += OnSliderInitialized;
            }
        }

        private void OnDisable()
        {
            foreach (var sliderProperty in _sliders.Values)
            {
                sliderProperty.ValueChanged -= OnSliderChanged;
                sliderProperty.Initialized -= OnSliderInitialized;
            }
        }

        public void AddInstantiate(GameSettingType settingType, IGameSettingData settingData, PanelToggleType panelType)
        {
            GameSettingProperty settingProperty = _settingsTypes[settingType];
            SliderValueView newSlider = Instantiate(settingProperty.SliderValueView, _settingsPanelController.GetContent(panelType));
            newSlider.Init(settingData);
            newSlider.SetProperty(settingData, countOfPresets: settingData.CountOfPresets, key: settingData.Name, header: settingData.Name);
            newSlider.RectTransform.sizeDelta = new Vector2(newSlider.RectTransform.sizeDelta.x, _settingsTypes[settingType].Height);
            _settingsTypes[settingType].SetSliderView(newSlider);
            _sliders[settingData] = newSlider;
        }

        public void AddExist(GameSettingType settingType, IGameSettingData settingData)
        {
            GameSettingProperty settingsProperty = _settingsTypes[settingType];
            SliderValueView existSlider = settingsProperty.SliderValueView;
            existSlider.Init(settingData);
            existSlider.SetProperty(settingData, countOfPresets: settingData.CountOfPresets, key: settingData.Name, header: settingData.Name);
            _settingsTypes[settingType].SetSliderView(existSlider);
            _sliders[settingData] = existSlider;
        }

        public Button GetApplyButton(GameSettingType settingType)
        {
            return _settingsTypes[settingType].SliderValueView.ApplyButton;
        }

        public void UpdateStartValue(IGameSettingData settingData) => _sliders[settingData].SetStartValues(settingData);

        public void UpdateLabel(IGameSettingData settingData) => _sliders[settingData].SetLabel(settingData);

        private void OnSliderChanged(string key, float value) => ValueChanged?.Invoke(key, value);

        private void OnSliderInitialized() => Initialized?.Invoke();
    }
}