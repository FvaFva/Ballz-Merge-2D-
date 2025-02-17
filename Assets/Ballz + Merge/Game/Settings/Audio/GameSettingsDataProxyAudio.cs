using UnityEngine;
using UnityEngine.Audio;

namespace BallzMerge.Root.Settings
{
    using Data;
    using System;
    using UnityEngine.InputSystem;

    public class GameSettingsDataProxyAudio : IGameSettingData
    {
        private const float MinValue = 0f;
        private const float MaxValue = 1f;
        private const float MixerMultiplayer = 20f;
        private const float MuteValueDB = -80f;
        private const int AdditionalZero = 2;
        private const int PointsAfterDot = 2;
        private const string Suffix = "%";
        private const float Shift = 0f;

        private float _lastValue;
        private readonly int _labelMultiplier = (int)Math.Pow(10, AdditionalZero);
        private readonly AudioMixer _mixer;

        public GameSettingsDataProxyAudio(AudioMixer mixer, string name)
        {
            _mixer = mixer;
            Name = name;
        }

        public float Value { get; private set; }
        public string Name { get; private set; }
        public string Label { get; private set; }
        public int? CountOfPresets { get; private set; }

        public void Change(float value)
        {
            Value = Mathf.Clamp(value, MinValue, MaxValue);
            Label = (_labelMultiplier * value + Shift).ToString($"F{PointsAfterDot}") + Suffix;

            if (Mathf.Approximately(Value, 0f))
                _mixer.SetFloat(Name, MuteValueDB);
            else
                _mixer.SetFloat(Name, Mathf.Log10(Value) * MixerMultiplayer);
        }

        public void Disable()
        {
            _lastValue = Value;
            Change(0);
        }

        public void Enable()
        {
            Change(_lastValue);
        }
    }
}