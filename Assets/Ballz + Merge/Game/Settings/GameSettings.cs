using System;

namespace BallzMerge.Root.Settings
{
    using Data;
    using System.Collections.Generic;
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
            ReadData();
        }

        public readonly GameSettingsDataProxyAudio SoundVolumeGlobal;
        public readonly GameSettingsDataProxyAudio SoundVolumeEffects;
        public readonly GameSettingsDataProxyAudio SoundVolumeMusic;

        public void Dispose()
        {
            _settingsMenu.ValueChanged -= OnSettingsChanged;
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

        private void ReadData()
        {
            foreach (var setting in _settings.Values)
            {
                setting.Change(_db.Get(setting));
                _settingsMenu.Add(setting);
            }
        }

        private void OnSettingsChanged(string key, float value)
        {
            IGameSettingData changed = _settings[key];
            changed.Change(value);
            _db.Set(changed);
        }
    }
}