using System;

namespace BallzMerge.Data
{
    public interface IGameSettingData
    {
        public string Name { get; }
        public float Value { get; }
        public string Label { get; }
        public int? CountOfPresets { get; }

        public void Change(float value);
        public void Get(float value);
    }
}
