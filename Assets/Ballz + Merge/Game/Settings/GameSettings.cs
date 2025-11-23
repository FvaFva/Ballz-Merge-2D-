using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class GameSettings : IDisposable
    {
        private readonly GameSettingsMenu _settingsMenu;
        private readonly GameSettingsStorage _db;
        private readonly TimeScaler _timeScaler;
        private readonly InfoPanelShowcase _infoPanelShowcase;
        private readonly List<EnvironmentPreset> _environmentPresets;
        private readonly GameColors _gameColors;
        private UIReorganizer _uiReorganizer;
        private Dictionary<string, IGameSettingData> _settings;

        private List<IDependentSceneSettings> _sceneElements = new List<IDependentSceneSettings>();

        public GameSettings(GameSettingsMenu settingsMenu, OwnerPrimaryComponents primary, InfoPanelShowcase infoPanelShowcase, UserQuestioner questioner, GlobalEffects globalEffects)
        {
            var mixer = primary.Hub.Get<AudioMixer>();
            SoundVolumeGlobal = new GameSettingsDataProxyAudio(mixer, "Global");
            SoundVolumeEffects = new GameSettingsDataProxyAudio(mixer, "Effects");
            SoundVolumeMusic = new GameSettingsDataProxyAudio(mixer, "Music");
            DisplayQualityPreset = new QualityPreset("Quality");
            _environmentPresets = new List<EnvironmentPreset>();
            _gameColors = new GameColors("Theme");
            SceneSetting = new SceneSetting(_gameColors);

            foreach (string name in globalEffects.AllEffects)
                _environmentPresets.Add(new EnvironmentPreset(name, globalEffects));

            _timeScaler = primary.TimeScaler;
            _infoPanelShowcase = infoPanelShowcase;
            _settingsMenu = settingsMenu;
            _settingsMenu.ValueChanged += OnSettingsChanged;
            SceneSetting.Changed += OnGlobalSettingChanged;
            _settingsMenu.PanelSwitch.PanelSwitched += OnPanelSwitched;
            _infoPanelShowcase.CloseTriggered += OnPanelSwitched;
            _db = primary.Data.Settings;

            void GenerateResolution()
            {
                DisplayResolution = new DisplayResolution("Resolution");
                DisplayMode = new DisplayMode("Display");
            }

            void GenerateOrientation()
            {
                DisplayOrientation = new DisplayOrientation("Orientation");
            }

            PlatformRunner.Run(GenerateResolution, () => { GenerateResolution(); GenerateOrientation(); }, androidAction: GenerateOrientation, iosAction: GenerateOrientation);

            GenerateMenu();
            CashSettings();

            void DesktopSetup()
            {
                Button applyButton = _settingsMenu.GetApplyButton(GameSettingType.GameScreenResolutionSetting);
                DisplayApplier = new DisplayApplier(applyButton, questioner);
                DisplayApplier.Applied += OnSettingsApplyChanges;
                DisplayApplier.Discarded += LoadSetting;
                DisplayResolution.SetDisplayApplier(DisplayApplier);
                DisplayMode.SetDisplayApplier(DisplayApplier);
            }

            void MobileSetup()
            {
                Button applyButton = _settingsMenu.GetApplyButton(GameSettingType.GameApplierSetting);
                DisplayOrientation.SetApplyButton(applyButton);
                DisplayOrientation.Applied += OnSettingsApplyChanges;
            }

            PlatformRunner.Run(DesktopSetup, () => { DesktopSetup(); MobileSetup(); }, androidAction: MobileSetup, iosAction: MobileSetup);
        }

        public readonly GameSettingsDataProxyAudio SoundVolumeGlobal;
        public readonly GameSettingsDataProxyAudio SoundVolumeEffects;
        public readonly GameSettingsDataProxyAudio SoundVolumeMusic;
        public readonly QualityPreset DisplayQualityPreset;
        public readonly SceneSetting SceneSetting;
        public DisplayResolution DisplayResolution { get; private set; }
        public DisplayMode DisplayMode { get; private set; }
        public DisplayApplier DisplayApplier { get; private set; }
        public DisplayOrientation DisplayOrientation { get; private set; }

        public void Dispose()
        {
            _settingsMenu.ValueChanged -= OnSettingsChanged;
            _settingsMenu.PanelSwitch.PanelSwitched -= OnPanelSwitched;
            _infoPanelShowcase.CloseTriggered -= OnPanelSwitched;
            DisplayOrientation.Applied -= OnSettingsApplyChanges;
            DisplayApplier.Applied -= OnSettingsApplyChanges;
            DisplayApplier.Discarded -= LoadSetting;
            SceneSetting.Changed -= OnGlobalSettingChanged;
        }

        public void LoadData()
        {
            foreach (var setting in _settings.Values)
                LoadSetting(setting);
        }

        public void CheckInSceneElement(IDependentSceneSettings element)
        {
            if (_sceneElements.Contains(element))
                return;

            _sceneElements.Add(element);
            element.ApplySetting(SceneSetting);

            if (element is UIReorganizer)
                _uiReorganizer = element as UIReorganizer;
        }

        public void ConnectSliders()
        {
            foreach (SliderValueView slider in _settingsMenu.Sliders)
            {
                foreach (DependentColorUI sliderProperty in slider.SlidersProperty)
                    _uiReorganizer.ConnectUI(sliderProperty);

                if (slider.AnimatedButton != null)
                    _uiReorganizer.ConnectUI(slider.AnimatedButton);

                _uiReorganizer.ConnectUI(slider.DependentColorUI);
            }
        }

        public void OnGlobalSettingChanged()
        {
            foreach (var element in _sceneElements)
                element.ApplySetting(SceneSetting);
        }

        public void CheckOutScene()
        {
            _sceneElements.Clear();
        }

        private void CashSettings()
        {
            _settings = new Dictionary<string, IGameSettingData>
            {
                { SoundVolumeGlobal.Name, SoundVolumeGlobal },
                { SoundVolumeEffects.Name, SoundVolumeEffects },
                { SoundVolumeMusic.Name, SoundVolumeMusic },
                { _timeScaler.Name, _timeScaler },
                { _gameColors.Name, _gameColors },
                { DisplayQualityPreset.Name, DisplayQualityPreset }
            };

            foreach (EnvironmentPreset preset in _environmentPresets)
                _settings.Add(preset.Name, preset);

            foreach (SceneSettingData preset in SceneSetting.GameSettings)
                _settings.Add(preset.Name, preset);

            void AddResolution()
            {
                _settings.Add(DisplayResolution.Name, DisplayResolution);
                _settings.Add(DisplayMode.Name, DisplayMode);
            }

            void AddOrientation()
            {
                _settings.Add(DisplayOrientation.Name, DisplayOrientation);
            }

            PlatformRunner.Run(AddResolution, () => { AddResolution(); AddOrientation(); }, androidAction: AddOrientation, iosAction: AddOrientation);
        }

        private void LoadSetting(IGameSettingData settingData)
        {
            settingData.Load(_db.Get(settingData));
            _settingsMenu.UpdateStartValue(settingData);
        }

        private void OnPanelSwitched()
        {
            LoadSetting(DisplayResolution);
            LoadSetting(DisplayOrientation);
            LoadSetting(DisplayMode);
        }

        private void GenerateMenu()
        {
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeGlobal, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeEffects, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeMusic, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, _timeScaler, PanelToggleType.GeneralToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameColorSetting, _gameColors, PanelToggleType.GeneralToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, DisplayQualityPreset, PanelToggleType.DisplayToggle);

            foreach (IGameSettingData preset in SceneSetting.GameSettings)
                _settingsMenu.AddInstantiate(GameSettingType.GameSetting, preset, PanelToggleType.DisplayToggle, PanelSubToggleType.Second);

            foreach (IGameSettingData preset in _environmentPresets)
                _settingsMenu.AddInstantiate(GameSettingType.GameSetting, preset, PanelToggleType.DisplayToggle, PanelSubToggleType.Second);

            void CreateResolutionSetting()
            {
                _settingsMenu.AddInstantiate(GameSettingType.GameScreenResolutionSetting, DisplayResolution, PanelToggleType.DisplayToggle);
                _settingsMenu.AddExist(GameSettingType.GameScreenResolutionSetting, DisplayMode);
            }

            void CreateOrientationSetting()
            {
                _settingsMenu.AddInstantiate(GameSettingType.GameApplierSetting, DisplayOrientation, PanelToggleType.DisplayToggle);
            }

            PlatformRunner.Run(CreateResolutionSetting, () => { CreateResolutionSetting(); CreateOrientationSetting(); }, androidAction: CreateOrientationSetting, iosAction: CreateOrientationSetting);
        }

        private void OnSettingsChanged(string key, float value)
        {
            IGameSettingData changed = _settings[key];

            if (Mathf.Approximately(changed.Value, value))
                return;

            changed.Change(value);
            _settingsMenu.UpdateLabel(changed);

            if (changed == DisplayMode || changed == DisplayResolution || changed == DisplayOrientation)
                return;

            OnSettingsApplyChanges(changed);
        }

        private void OnSettingsApplyChanges(IGameSettingData settingData)
        {
            _db.Set(settingData);
        }
    }
}