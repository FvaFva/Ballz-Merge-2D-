using UnityEngine;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class TimeScaler : IGameSettingData, IGamePauseController
    {
        private const float PauseScale = 0f;
        private const float MinScale = 1;
        private const float MaxScale = 10;
        private const float SpeedUpScale = 100f;

        public float Value { get; private set; }

        public string Name { get { return "Time"; } }

        public void Stop()
        {
            Time.timeScale = PauseScale;
        }

        public void SetRegular()
        {
            Time.timeScale = Value;
        }

        public void Change(float value)
        {
            value = Mathf.Lerp(MinScale, MaxScale, value);
            Value = value;
            Time.timeScale = value;
        }

        public void SpeedUp()
        {
            Time.timeScale = SpeedUpScale;
        }
    }
}