using UnityEngine;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class TimeScaler : IGameSettingData, IGamePauseController
    {
        private const float PauseScale = 0f;
        private const float MinScale = 1;
        private const float MaxScale = 11;
        private const float SpeedUpScale = 100f;

        public float Value { get; private set; }

        public string Name { get { return "Time"; } }

        public void Stop()
        {
            Time.timeScale = PauseScale;
        }

        public void SetRegular()
        {
            Change(Value);
        }

        public void Change(float value)
        {
            Value = Mathf.Clamp01(value);
            Time.timeScale = Mathf.Lerp(MinScale, MaxScale, Value);
        }

        public void SpeedUp()
        {
            Time.timeScale = SpeedUpScale;
        }
    }
}