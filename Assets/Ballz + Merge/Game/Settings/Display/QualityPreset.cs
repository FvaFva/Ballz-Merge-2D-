using BallzMerge.Data;
using System;
using UnityEngine;

public class QualityPreset : IGameSettingData
{
    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    public event Action<bool> StateChanged;
    public event Action Changed;

    public QualityPreset(string name)
    {
        Name = name;
        CountOfPresets = QualitySettings.count - 1;
        Load((float)CountOfPresets);
    }

    public void Load(float value)
    {
        Value = CountOfPresets < value ? (float)CountOfPresets : value;
        Change(Value);
    }

    public void Change(float value)
    {
        Value = value;
        Label = QualitySettings.names[Mathf.RoundToInt(Value)];
        QualitySettings.SetQualityLevel(Mathf.RoundToInt(Value), true);
    }
}