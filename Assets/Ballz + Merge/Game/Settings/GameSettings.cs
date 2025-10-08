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
        private Dictionary<string, IGameSettingData> _settings;
        private SceneSetting _sceneSetting;

        private List<IDependentSceneSettings> _sceneElements = new List<IDependentSceneSettings>();

        public GameSettings(GameSettingsMenu settingsMenu, OwnerPrimaryComponents primary, InfoPanelShowcase infoPanelShowcase, GlobalEffects globalEffects)
        {
            var mixer = primary.Hub.Get<AudioMixer>();
            SoundVolumeGlobal = new GameSettingsDataProxyAudio(mixer, "Global");
            SoundVolumeEffects = new GameSettingsDataProxyAudio(mixer, "Effects");
            SoundVolumeMusic = new GameSettingsDataProxyAudio(mixer, "Music");
            DisplayQualityPreset = new QualityPreset("Quality");
            _environmentPresets = new List<EnvironmentPreset>();
            _sceneSetting = new SceneSetting();

            foreach (string name in globalEffects.AllEffects)
                _environmentPresets.Add(new EnvironmentPreset(name, globalEffects));

            _timeScaler = primary.TimeScaler;
            _infoPanelShowcase = infoPanelShowcase;
            _settingsMenu = settingsMenu;
            _settingsMenu.ValueChanged += OnSettingsChanged;
            _sceneSetting.Changed += OnGlobalSettingChanged;
            _settingsMenu.PanelSwitch.PanelSwitched += ResetData;
            _infoPanelShowcase.CloseTriggered += ReadData;
            _db = primary.Data.Settings;

            PlatformRunner.RunOnDesktopPlatform(
            desktopAction: () =>
            {
                DisplayResolution = new DisplayResolution("Resolution");
                DisplayMode = new DisplayMode("Display");
            });

            PlatformRunner.RunOnMobilePlatform(
            mobileAction: () =>
            {
                DisplayOrientation = new DisplayOrientation("Orientation");
            });

            GenerateMenu();
            CashSettings();

            PlatformRunner.RunOnDesktopPlatform(
            desktopAction: () =>
            {
                Button applyButton = _settingsMenu.GetApplyButton(GameSettingType.GameScreenResolutionSetting);
                DisplayApplier = new DisplayApplier(applyButton);
                DisplayApplier.Applied += OnSettingsApplyChanges;
                DisplayResolution.SetDisplayApplier(DisplayApplier);
                DisplayMode.SetDisplayApplier(DisplayApplier);
            });

            PlatformRunner.RunOnMobilePlatform(
            mobileAction: () =>
            {
                Button applyButton = _settingsMenu.GetApplyButton(GameSettingType.GameApplierSetting);
                DisplayOrientation.SetApplyButton(applyButton);
                DisplayOrientation.Applied += OnSettingsApplyChanges;
            });
        }

        public readonly GameSettingsDataProxyAudio SoundVolumeGlobal;
        public readonly GameSettingsDataProxyAudio SoundVolumeEffects;
        public readonly GameSettingsDataProxyAudio SoundVolumeMusic;
        public readonly QualityPreset DisplayQualityPreset;
        public DisplayResolution DisplayResolution { get; private set; }
        public DisplayMode DisplayMode { get; private set; }
        public DisplayApplier DisplayApplier { get; private set; }
        public DisplayOrientation DisplayOrientation { get; private set; }

        public void Dispose()
        {
            _settingsMenu.ValueChanged -= OnSettingsChanged;
            _settingsMenu.PanelSwitch.PanelSwitched -= ResetData;
            _infoPanelShowcase.CloseTriggered -= ReadData;
            DisplayOrientation.Applied -= OnSettingsApplyChanges;
            DisplayApplier.Applied -= OnSettingsApplyChanges;
            _sceneSetting.Changed -= OnGlobalSettingChanged;
        }

        public void ReadData()
        {
            foreach (var setting in _settings.Values)
                ResetData(setting);
        }

        public void CheckInSceneElement(IDependentSceneSettings element)
        {
            if (_sceneElements.Contains(element))
                return;

            _sceneElements.Add(element);
            element.ApplySetting(_sceneSetting);
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
                { DisplayQualityPreset.Name, DisplayQualityPreset }
            };

            foreach (EnvironmentPreset preset in _environmentPresets)
                _settings.Add(preset.Name, preset);

            foreach (IGameSettingData preset in _sceneSetting.GameSettings)
                _settings.Add(preset.Name, preset);

            PlatformRunner.RunOnDesktopPlatform(
            desktopAction: () =>
            {
                _settings.Add(DisplayResolution.Name, DisplayResolution);
                _settings.Add(DisplayMode.Name, DisplayMode);
            });

            PlatformRunner.RunOnMobilePlatform(
            mobileAction: () =>
            {
                _settings.Add(DisplayOrientation.Name, DisplayOrientation);
            });
        }

        private void ResetData()
        {
            foreach (var setting in _settings.Values)
            {
                if (setting == DisplayResolution || setting == DisplayMode || setting == DisplayOrientation)
                    ResetData(setting);
            }
        }

        private void ResetData(IGameSettingData settingData)
        {
            settingData.Get(_db.Get(settingData));
            _settingsMenu.UpdateStartValue(settingData);
        }

        private void GenerateMenu()
        {
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeGlobal, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeEffects, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeMusic, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, _timeScaler, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, DisplayQualityPreset, PanelToggleType.DisplayToggle);

            foreach (IGameSettingData preset in _sceneSetting.GameSettings)
                _settingsMenu.AddInstantiate(GameSettingType.GameSetting, preset, PanelToggleType.DisplayToggle, PanelSubToggleType.Second);
            
            foreach (IGameSettingData preset in _environmentPresets)
                _settingsMenu.AddInstantiate(GameSettingType.GameSetting, preset, PanelToggleType.DisplayToggle, PanelSubToggleType.Second);


            PlatformRunner.RunOnDesktopPlatform(
            desktopAction: () =>
            {
                _settingsMenu.AddInstantiate(GameSettingType.GameScreenResolutionSetting, DisplayResolution, PanelToggleType.DisplayToggle);
                _settingsMenu.AddExist(GameSettingType.GameScreenResolutionSetting, DisplayMode);
            });

            PlatformRunner.RunOnMobilePlatform(
            mobileAction: () =>
            {
                _settingsMenu.AddInstantiate(GameSettingType.GameApplierSetting, DisplayOrientation, PanelToggleType.DisplayToggle);
            });
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

        private void OnGlobalSettingChanged()
        {
            foreach (var element in _sceneElements)
                element.ApplySetting(_sceneSetting);
        }
    }
}