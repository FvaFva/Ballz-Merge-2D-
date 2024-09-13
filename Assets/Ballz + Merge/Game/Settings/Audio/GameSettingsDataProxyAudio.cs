using System;
using UnityEngine;

namespace BallzMerge.Root.Settings
{
    using Data;

    public class GameSettingsDataProxyAudio: IGameSettingData
    {
        private const float MinValue = 0f;
        private const float MaxValue = 1f;

        public float Value { get; private set; }

        public string Name { get { return "Audio"; } }

        public event Action Changed;

        public void Change(float value)
        {
            Value = Mathf.Clamp(value, MinValue, MaxValue);
            Changed?.Invoke();
        }
    }
}