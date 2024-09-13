using System;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class GameSettings : IDisposable
    {
        private SettingsMenuView _settingsMenu;
        private GameSettingsStorage _db;

        public GameSettings(SettingsMenuView settingsMenu, GameSettingsStorage db)
        {
            AudioSettings = new GameSettingsDataProxyAudio();
            TimeScaler = new TimeScaler();
            _settingsMenu = settingsMenu;
            _settingsMenu.SettingsChanged += OnSettingsChanged;
            _db = db;
            ReadData();
        }

        public readonly GameSettingsDataProxyAudio AudioSettings;
        public readonly TimeScaler TimeScaler;

        public void Dispose()
        {
            _settingsMenu.SettingsChanged -= OnSettingsChanged;
        }

        private void ReadData()
        {
            AudioSettings.Change(_db.Get(AudioSettings));
            TimeScaler.Change(_db.Get(TimeScaler));
            _settingsMenu.UpdateFromData(AudioSettings.Value, TimeScaler.Value);
        }

        private void OnSettingsChanged(float audio, float timescale)
        {
            if(audio.Equals(AudioSettings.Value) == false)
            {
                AudioSettings.Change(audio);
                _db.Set(AudioSettings);
            }

            if (timescale.Equals(TimeScaler.Value) == false)
            {
                TimeScaler.Change(timescale);
                _db.Set(TimeScaler);
            }
        }
    }
}