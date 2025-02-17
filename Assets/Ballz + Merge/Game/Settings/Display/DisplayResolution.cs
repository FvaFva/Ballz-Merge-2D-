using BallzMerge.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DisplayResolution : IGameSettingData
{
    private Resolution[] _resolutions;
    private DisplayApplier _displayApplier;
    private Resolution _resolution;

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    public DisplayResolution(string name, DisplayApplier displayApplier)
    {
        Name = name;
        _displayApplier = displayApplier;

        _resolutions = Screen.resolutions
            .Select(res => new Resolution { width = res.width, height = res.height })
            .Distinct()
            .ToArray();

        CountOfPresets = _resolutions.Length - 1;

        Value = _resolutions.ToList().FindIndex(res => res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height);
        _resolution = _resolutions[Mathf.RoundToInt(Value)];
        _displayApplier.SetResolution(_resolution);
    }

    public void Change(float value)
    {
        Value = value;
        Label = $"{_resolutions[Mathf.RoundToInt(Value)].width}x{_resolutions[Mathf.RoundToInt(Value)].height}";
        _resolution = _resolutions[Mathf.RoundToInt(Value)];
        _displayApplier.SetResolution(_resolution);
    }
}