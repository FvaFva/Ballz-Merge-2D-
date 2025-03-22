using BallzMerge.Data;
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

    public DisplayResolution(string name)
    {
        Name = name;

        _resolutions = Screen.resolutions
            .Select(res => new Resolution { width = res.width, height = res.height })
            .Distinct()
            .ToArray();

        CountOfPresets = _resolutions.Length - 1;

        Value = _resolutions.ToList().FindIndex(res => res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height);
        _resolution = _resolutions[Mathf.RoundToInt(Value)];
    }

    public void SetDisplayApplier(DisplayApplier displayApplier)
    {
        _displayApplier = displayApplier;
    }

    public void Change(float value)
    {
        Value = value;
        Label = $"{_resolutions[Mathf.RoundToInt(Value)].width}x{_resolutions[Mathf.RoundToInt(Value)].height}";
        _resolution = _resolutions[Mathf.RoundToInt(Value)];
        _displayApplier.SetResolution(_resolution, this, Value);
    }
}