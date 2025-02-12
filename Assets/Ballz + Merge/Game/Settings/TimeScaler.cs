using UnityEngine;

namespace BallzMerge.Root.Settings
{
    using Data;
    using System;

    public class TimeScaler : IGameSettingData, IGamePauseController
    {
        private const float PauseScale = 0f;
        private const float MinScale = 1;
        private const float MaxScale = 11;
        private const float SpeedUpScale = 100f;

        public float Value { get; private set; }
        public string Name { get { return "Time"; } }
        public string Label { get; private set; }

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
            Value = Mathf.Clamp01(value);
            Time.timeScale = Mathf.Lerp(MinScale, MaxScale, Value);
        }

        public void SpeedUp()
        {
            Time.timeScale = SpeedUpScale;
        }
    }
}