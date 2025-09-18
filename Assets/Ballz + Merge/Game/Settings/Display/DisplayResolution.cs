using BallzMerge.Data;
using System;
using System.Linq;
using UnityEngine;

public class DisplayResolution : IGameSettingData
{
    private Resolution[] _resolutions;
    private DisplayApplier _displayApplier;
    private Resolution _resolution;

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    public event Action<bool> StateChanged;

    public DisplayResolution(string name)
    {
        Name = name;
        _resolutions = Screen.resolutions
            .GroupBy(resolution => (resolution.width, resolution.height))
            .Select((group) => new Resolution { width = group.Key.width, height = group.Key.height, refreshRateRatio = Screen.currentResolution.refreshRateRatio })
            .Distinct()
            .ToArray();

        CountOfPresets = _resolutions.Length - 1;

        Value = _resolutions.ToList().FindIndex(res => res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height);
        Debug.Log($"Value is: {Value}");

        if (Value == -1)
            Value = _resolutions.Length - 1;

        _resolution = _resolutions[Mathf.RoundToInt(Value)];
    }

    public void SetDisplayApplier(DisplayApplier displayApplier)
    {
        _displayApplier = displayApplier;
    }

    public void ChangeState(bool state)
    {
        StateChanged?.Invoke(state);
    }

    public void Get(float value)
    {
        Value = CountOfPresets < value ? (float)CountOfPresets : value;
        Change(Value);
        _displayApplier.SetLoadResolution();
    }

    public void Change(float value)
    {
        Value = value;
        Label = $"{_resolutions[Mathf.RoundToInt(Value)].width}x{_resolutions[Mathf.RoundToInt(Value)].height}";
        _resolution = _resolutions[Mathf.RoundToInt(Value)];
        _displayApplier.SetResolution(_resolution, this, this);
    }
}