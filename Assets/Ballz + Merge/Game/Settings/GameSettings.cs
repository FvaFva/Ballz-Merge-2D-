using System;

namespace BallzMerge.Root.Settings
{
    using Data;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.UI;

    public class GameSettings : IDisposable
    {
        private readonly GameSettingsMenu _settingsMenu;
        private readonly GameSettingsStorage _db;
        private Dictionary<string, IGameSettingData> _settings;
        private readonly TimeScaler _timeScaler;

        public GameSettings(GameSettingsMenu settingsMenu, GameSettingsStorage db, AudioMixer mixer, TimeScaler timeScaler)
        {
            SoundVolumeGlobal = new GameSettingsDataProxyAudio(mixer, "Global");
            SoundVolumeEffects = new GameSettingsDataProxyAudio(mixer, "Effects");
            SoundVolumeMusic = new GameSettingsDataProxyAudio(mixer, "Music");
            DisplayQualityPreset = new QualityPreset("Quality");
            DisplayResolution = new DisplayResolution("Resolution");
            DisplayMode = new DisplayMode("Display");
            _timeScaler = timeScaler;
            _settingsMenu = settingsMenu;
            _settingsMenu.ValueChanged += OnSettingsChanged;
            _db = db;
            CashSettings();
            GenerateMenu();
            Button applyButton = _settingsMenu.GetApplyButton(GameSettingType.GameScreenResolutionSetting);
            DisplayApplier = new DisplayApplier(applyButton);
            DisplayResolution.SetDisplayApplier(DisplayApplier);
            DisplayMode.SetDisplayApplier(DisplayApplier);
        }

        public readonly GameSettingsDataProxyAudio SoundVolumeGlobal;
        public readonly GameSettingsDataProxyAudio SoundVolumeEffects;
        public readonly GameSettingsDataProxyAudio SoundVolumeMusic;
        public readonly QualityPreset DisplayQualityPreset;
        public readonly DisplayResolution DisplayResolution;
        public readonly DisplayMode DisplayMode;
        public readonly DisplayApplier DisplayApplier;

        public void Dispose()
        {
            _settingsMenu.ValueChanged -= OnSettingsChanged;
        }

        public void ReadData()
        {
            foreach (var setting in _settings.Values)
            {
                setting.Change(_db.Get(setting));
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
                { DisplayQualityPreset.Name, DisplayQualityPreset },
                { DisplayResolution.Name, DisplayResolution },
                { DisplayMode.Name, DisplayMode },
            };
        }

        private void GenerateMenu()
        {
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeGlobal, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeEffects, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, SoundVolumeMusic, PanelToggleType.AudioToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, DisplayQualityPreset, PanelToggleType.DisplayToggle);
            _settingsMenu.AddInstantiate(GameSettingType.GameScreenResolutionSetting, DisplayResolution, PanelToggleType.DisplayToggle);
            _settingsMenu.AddExist(GameSettingType.GameScreenResolutionSetting, DisplayMode);
            _settingsMenu.AddInstantiate(GameSettingType.GameSetting, _timeScaler, PanelToggleType.AudioToggle);
        }

        private void OnSettingsChanged(string key, float value)
        {
            IGameSettingData changed = _settings[key];

            if (Mathf.Approximately(changed.Value, value))
                return;

            changed.Change(value);
            _settingsMenu.UpdateLabel(changed);
            _db.Set(changed);
        }
    }
}