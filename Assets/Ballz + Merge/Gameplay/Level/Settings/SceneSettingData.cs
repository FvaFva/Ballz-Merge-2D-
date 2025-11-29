using BallzMerge.Data;
using System;
using System.Collections.Generic;

public class SceneSettingData : IGameSettingData
{
    private const string Enabled = "On";
    private const string Disabled = "Off";

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    private readonly Dictionary<float, string> EffectsStates = new Dictionary<float, string>()
    {
        { 1, Enabled },
        { 0, Disabled }
    };

    public event Action<bool> StateChanged;
    public event Action Changed;
    public event Action<bool> ValueChanged;

    public SceneSettingData(string name)
    {
        Name = name;
        CountOfPresets = 1;
    }

    public void Load(float value)
    {
        Value = value;
        Label = EffectsStates[Value];
        ValueChanged?.Invoke(Convert.ToBoolean(Value));
    }

    public void Change(float value)
    {
        Value = value;
        Label = EffectsStates[Value];
        ValueChanged?.Invoke(Convert.ToBoolean(Value));
        Changed?.Invoke();
    }
}
