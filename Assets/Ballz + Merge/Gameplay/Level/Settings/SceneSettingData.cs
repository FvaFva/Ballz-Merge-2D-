using BallzMerge.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettingData : IGameSettingData
{
    private const string Enabled = "On";
    private const string Disabled = "Off";

    public bool IsDynamicBoards;

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    private readonly Dictionary<int, string> EffectsStates = new Dictionary<int, string>()
    {
        { 1, Enabled },
        { 0, Disabled }
    };

    public event Action<bool> StateChanged;

    public SceneSettingData(string name)
    {
        Name = name;
        CountOfPresets = 1;
    }

    public void Get(float value)
    {
        Value = CountOfPresets < value ? (float)CountOfPresets : value;
        Change(Value);
    }

    public void Change(float value)
    {
        Value = value;
        Label = EffectsStates[Mathf.RoundToInt(Value)];
        StateChanged?.Invoke(true);
    }
}
