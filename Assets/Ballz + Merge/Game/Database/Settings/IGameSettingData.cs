using System;

namespace BallzMerge.Data
{
    public interface IGameSettingData
    {
        public string Name { get; }
        public float Value { get; }
        public string Label { get; }
        public int? CountOfPresets { get; }

        public void Load(float value);
        public void Change(float value);

        public event Action<bool> StateChanged;
        public event Action Changed;
    }
}
