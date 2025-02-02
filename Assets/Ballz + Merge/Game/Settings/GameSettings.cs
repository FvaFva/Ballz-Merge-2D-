using System;

namespace BallzMerge.Root.Settings
{
    using Data;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;

    public class GameSettings : IDisposable
    {
        private readonly GameSettingsMenu _settingsMenu;
        private readonly GameSettingsStorage _db;
        private Dictionary<string, IGameSettingData> _settings;
        private readonly TimeScaler _timeScaler;

        public GameSettings(GameSettingsMenu settingsMenu, GameSettingsStorage db, AudioMixer mixer, TimeScaler timeScaler)
        {
            SoundVolumeGlobal = new GameSettingsDataProxyAudio(mixer, "SoundVolumeGlobal");
            SoundVolumeEffects = new GameSettingsDataProxyAudio(mixer, "SoundVolumeEffects");
            SoundVolumeMusic = new GameSettingsDataProxyAudio(mixer, "SoundVolumeMusic");
            _timeScaler = timeScaler;
            _settingsMenu = settingsMenu;
            _settingsMenu.ValueChanged += OnSettingsChanged;
            _db = db;
            CashSettings();
            GenerateMenu();
        }

        public readonly GameSettingsDataProxyAudio SoundVolumeGlobal;
        public readonly GameSettingsDataProxyAudio SoundVolumeEffects;
        public readonly GameSettingsDataProxyAudio SoundVolumeMusic;

        public void Dispose()
        {
            _settingsMenu.ValueChanged -= OnSettingsChanged;
        }

        public void ReadData()
        {
            foreach (var setting in _settings.Values)
            {
                setting.Change(_db.Get(setting));
                _settingsMenu.UpdateValue(setting);
            }
        }

        private void CashSettings()
        {
            _settings = new Dictionary<string, IGameSettingData>
            {
                { SoundVolumeGlobal.Name, SoundVolumeGlobal },
                { SoundVolumeEffects.Name, SoundVolumeEffects },
                { SoundVolumeMusic.Name, SoundVolumeMusic },
                { _timeScaler.Name, _timeScaler }
            };
        }

        private void GenerateMenu()
        {
            _settingsMenu.Add(SoundVolumeGlobal, additionalZero: 2, pointsAfterDot: 2);
            _settingsMenu.Add(SoundVolumeEffects, additionalZero: 2, pointsAfterDot: 2);
            _settingsMenu.Add(SoundVolumeMusic, additionalZero: 2, pointsAfterDot: 2);
            _settingsMenu.Add(_timeScaler, "X", 1, 0, 1);
        }

        private void OnSettingsChanged(string key, float value)
        {
            IGameSettingData changed = _settings[key];

            if (Mathf.Approximately(changed.Value, value))
                return;

            changed.Change(value);
            _db.Set(changed);
        }
    }
}