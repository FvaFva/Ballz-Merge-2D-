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
        private const int AdditionalZero = 1;
        private const int PointsAfterDot = 0;
        private const string Suffix = "X";
        private const float Shift = 1f;

        public float Value { get; private set; }
        public string Name { get { return "Time"; } }
        public string Label { get; private set; }
        public int? CountOfPresets { get; private set; }

        private readonly int _labelMultiplier = (int)Math.Pow(10, AdditionalZero);

        public void Stop()
        {
            Time.timeScale = PauseScale;
        }

        public void SetRegular()
        {
            Change(Value);
        }

        public void Get(float value)
        {
            Value = Mathf.Clamp01(value);
            Change(Value);
        }

        public void Change(float value)
        {
            Value = value;
            Label = (_labelMultiplier * value + Shift).ToString($"F{PointsAfterDot}") + Suffix;
            Time.timeScale = Mathf.Lerp(MinScale, MaxScale, Value);
        }

        public void SpeedUp()
        {
            Time.timeScale = SpeedUpScale;
        }
    }
}