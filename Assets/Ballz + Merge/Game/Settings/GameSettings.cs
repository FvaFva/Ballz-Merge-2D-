using System;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class GameSettings : IDisposable
    {
        private EscapeMenu _settingsMenu;
        private GameSettingsStorage _db;
        private readonly TimeScaler _timeScaler;

        public GameSettings(EscapeMenu settingsMenu, GameSettingsStorage db, TimeScaler timeScaler)
        {
            AudioSettings = new GameSettingsDataProxyAudio();
            _timeScaler = timeScaler;
            _settingsMenu = settingsMenu;
            _settingsMenu.SettingsChanged += OnSettingsChanged;
            _db = db;
            ReadData();
        }

        public readonly GameSettingsDataProxyAudio AudioSettings;

        public void Dispose()
        {
            _settingsMenu.SettingsChanged -= OnSettingsChanged;
        }

        private void ReadData()
        {
            AudioSettings.Change(_db.Get(AudioSettings));
            _timeScaler.Change(_db.Get(_timeScaler));
            _settingsMenu.UpdateFromData(AudioSettings.Value, _timeScaler.Value);
        }

        private void OnSettingsChanged(float audio, float timescale)
        {
            if(audio.Equals(AudioSettings.Value) == false)
            {
                AudioSettings.Change(audio);
                _db.Set(AudioSettings);
            }

            if (timescale.Equals(_timeScaler.Value) == false)
            {
                _timeScaler.Change(timescale);
                _db.Set(_timeScaler);
            }
        }
    }
}