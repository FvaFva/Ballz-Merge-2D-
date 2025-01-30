using UnityEngine;
using UnityEngine.Audio;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class GameSettingsDataProxyAudio : IGameSettingData
    {
        private const float MinValue = 0f;
        private const float MaxValue = 1f;

        private float _lastValue;
        private readonly AudioMixer _mixer;

        public GameSettingsDataProxyAudio(AudioMixer mixer, string name)
        {
            _mixer = mixer;
            Name = name;
        }

        public float Value { get; private set; }

        public string Name { get; private set; }

        public void Change(float value)
        {
            Value = Mathf.Clamp(value, MinValue, MaxValue);
            _mixer.SetFloat(Name, Value);
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