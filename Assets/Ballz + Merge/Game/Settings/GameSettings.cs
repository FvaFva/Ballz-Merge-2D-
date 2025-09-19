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
        private Dictionary<string, IGameSettingData> _settings;

        public GameSettings(GameSettingsMenu settingsMenu, OwnerPrimaryComponents primary, InfoPanelShowcase infoPanelShowcase)
        {
            var mixer = primary.Hub.Get<AudioMixer>();
            SoundVolumeGlobal = new GameSettingsDataProxyAudio(mixer, "Global");
            SoundVolumeEffects = new GameSettingsDataProxyAudio(mixer, "Effects");
            SoundVolumeMusic = new GameSettingsDataProxyAudio(mixer, "Music");
            DisplayQualityPreset = new QualityPreset("Quality");

            _timeScaler = primary.TimeScaler;
            _infoPanelShowcase = infoPanelShowcase;
            _settingsMenu = settingsMenu;
            _settingsMenu.ValueChanged += OnSettingsChanged;
            _settingsMenu.PanelSwitch.PanelSwitched += ReadData;
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
            _settingsMenu.PanelSwitch.PanelSwitched -= ReadData;
            _infoPanelShowcase.CloseTriggered -= ReadData;
            DisplayOrientation.Applied -= OnSettingsApplyChanges;
            DisplayApplier.Applied -= OnSettingsApplyChanges;
        }

        public void ReadData()
        {
            foreach (var setting in _settings.Values)
            {
                setting.Get(_db.Get(setting));
                _settingsMenu.UpdateStartValue(setting);
            }
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

        private void GenerateMenu()
        {
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeGlobal, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeEffects, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeMusic, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, _timeScaler, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, DisplayQualityPreset, PanelToggleType.DisplayToggle);

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
    }
}