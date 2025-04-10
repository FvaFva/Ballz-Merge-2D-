using UnityEngine;

namespace BallzMerge.Root.Settings
{
    using Data;
    using DG.Tweening;
    using System;

    public class TimeScaler : IGameSettingData, IGameTimeOwner
    {
        private const float PauseScale = 0f;
        private const float SlowMoScale = 0.05f;
        private const float MinScale = 1;
        private const float MaxScale = 11;
        private const float SpeedUpScale = 100f;
        private const int AdditionalZero = 1;
        private const int PointsAfterDot = 0;
        private const string Suffix = "X";
        private const float Shift = 1f;

        private Tween _currentSlowMoTween;

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
            KillAvailableSlowMo();
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

        public void PlaySlowMo(float time)
        {
            KillAvailableSlowMo();

            Time.timeScale = SlowMoScale;

            _currentSlowMoTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, MinScale, time)
                .SetEase(Ease.InSine)
                .SetUpdate(true)
                .OnComplete(SetRegular);
        }

        private void KillAvailableSlowMo()
        {
            if (_currentSlowMoTween != null && _currentSlowMoTween.IsActive() && _currentSlowMoTween.IsPlaying())
            {
                _currentSlowMoTween.Kill(false);
                _currentSlowMoTween = null;
            }
        }
    }
}