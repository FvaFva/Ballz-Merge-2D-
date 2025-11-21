using BallzMerge.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPreset : IGameSettingData
{
    private const string Enabled = "On";
    private const string Disabled = "Off";

    private readonly GlobalEffects _globalEffects;
    private readonly Dictionary<int, string> EffectsStates = new Dictionary<int, string>()
    {
        { 1, Enabled },
        { 0, Disabled }
    };

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    public event Action<bool> StateChanged;
    public event Action Changed;

    public EnvironmentPreset(string name, GlobalEffects globalEffects)
    {
        Name = name;
        _globalEffects = globalEffects;
        CountOfPresets = 1;
    }

    public void Load(float value)
    {
        Value = CountOfPresets < value ? (float)CountOfPresets : value;
        Change(Value);
    }

    public void Change(float value)
    {
        Value = value;
        Label = EffectsStates[Mathf.RoundToInt(Value)];
        bool state = Value == 1;
        _globalEffects.ChangeState(state, Name);
    }
}