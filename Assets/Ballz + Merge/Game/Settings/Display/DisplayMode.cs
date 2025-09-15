using BallzMerge.Data;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMode : IGameSettingData
{
    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    private List<DisplayType> _modes;
    private DisplayApplier _displayApplier;
    private FullScreenMode _fullScreenMode;

    public DisplayMode(string name)
    {
        Name = name;

        _modes = new List<DisplayType>
        {
            { new DisplayType(FullScreenMode.FullScreenWindow, "Windowed Fullscreen") },
            { new DisplayType(FullScreenMode.ExclusiveFullScreen, "Fullscreen") },
            { new DisplayType(FullScreenMode.Windowed, "Windowed") }
        };

        CountOfPresets = _modes.Count - 1;

        for (int i = 0; i < _modes.Count; i++)
        {
            if (_modes[i].FullScreenMode == Screen.fullScreenMode)
            {
                Value = i;
                break;
            }
        }

        _fullScreenMode = _modes[Mathf.RoundToInt(Value)].FullScreenMode;
    }

    public void SetDisplayApplier(DisplayApplier displayApplier)
    {
        _displayApplier = displayApplier;
    }

    public void Get(float value)
    {
        Value = CountOfPresets < value ? (float)CountOfPresets : value;
        Change(Value);
    }

    public void Change(float value)
    {
        Value = value;
        Label = _modes[Mathf.RoundToInt(Value)].DisplayName;
        _fullScreenMode = _modes[Mathf.RoundToInt(Value)].FullScreenMode;
        _displayApplier.SetScreenMode(_fullScreenMode, this);
    }
}