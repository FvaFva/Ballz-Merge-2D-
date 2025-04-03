using BallzMerge.Data;
using UnityEngine;

public class QualityPreset : IGameSettingData
{
    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    public QualityPreset(string name)
    {
        Name = name;
        CountOfPresets = QualitySettings.count - 1;
    }

    public void Change(float value)
    {
        Value = value;
        Label = QualitySettings.names[Mathf.RoundToInt(Value)];
        QualitySettings.SetQualityLevel(Mathf.RoundToInt(Value), true);
    }
}