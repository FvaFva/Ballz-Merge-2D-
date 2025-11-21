using BallzMerge.Data;
using System;
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
    private ScreenMode _screenMode;

    public event Action<bool> StateChanged;
    public event Action Changed;

    public DisplayMode(string name)
    {
        Name = name;

        _modes = new List<DisplayType>
        {
            { new DisplayType(ScreenMode.Fullscreen, "Fullscreen") },
            { new DisplayType(ScreenMode.WindowedFullscreen, "Windowed Fullscreen") },
            { new DisplayType(ScreenMode.Windowed, "Windowed") },
            { new DisplayType(ScreenMode.Resizable, "Resizable")}
        };

        CountOfPresets = _modes.Count - 1;
        _screenMode = _modes[0].ScreenMode;
    }

    public void SetDisplayApplier(DisplayApplier displayApplier)
    {
        _displayApplier = displayApplier;
    }

    public void Load(float value)
    {
        Value = CountOfPresets < value ? (float)CountOfPresets : value;
        Change(Value);
        _displayApplier.LoadScreenMode();
    }

    public void Change(float value)
    {
        Value = value;
        Label = _modes[Mathf.RoundToInt(Value)].DisplayName;
        _screenMode = _modes[Mathf.RoundToInt(Value)].ScreenMode;
        _displayApplier.SetScreenMode(_screenMode, this);
    }
}