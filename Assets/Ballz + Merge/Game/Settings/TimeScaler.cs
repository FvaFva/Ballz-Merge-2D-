using UnityEngine;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class TimeScaler : IGameSettingData, IGamePauseController
    {
        private const float PauseScale = 0f;
        private const float MinScale = 1;
        private const float MaxScale = 10;

        public float Value { get; private set; }

        public string Name { get { return "Time"; } }

        public void Pause()
        {
            Time.timeScale = PauseScale;
        }

        public void Play()
        {
            Time.timeScale = Value;
        }

        public void Change(float value)
        {
            value = Mathf.Clamp(value, MinScale, MaxScale);
            Value = value;
            Time.timeScale = value;
        }
    }
}