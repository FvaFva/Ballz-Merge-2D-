using BallzMerge.Data;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMode : IGameSettingData
{
    private List<DisplayType> _modes;
    private DisplayApplier _displayApplier;
    private FullScreenMode _fullScreenMode;

    public string Name { get; private set; }

    public float Value { get; private set; }

    public string Label { get; private set; }

    public int? CountOfPresets { get; private set; }

    public DisplayMode(string name, DisplayApplier displayApplier)
    {
        Name = name;
        _displayApplier = displayApplier;

        _modes = new List<DisplayType>
        {
            { new DisplayType(FullScreenMode.FullScreenWindow, "Windowed fullscreen") },
            { new DisplayType(FullScreenMode.ExclusiveFullScreen, "Fullscreen") },
            { new DisplayType(FullScreenMode.Windowed, "Windowed") }
        };

        CountOfPresets = _modes.Count - 1;

        for (int i = _modes.Count - 1; i > 0; i--)
        {
            if (_modes[i].FullScreenMode == Screen.fullScreenMode)
            {
                Value = i;
                break;
            }
        }

        _fullScreenMode = _modes[Mathf.RoundToInt(Value)].FullScreenMode;
        _displayApplier.SetScreenMode(_fullScreenMode);
    }

    public void Change(float value)
    {
        Value = value;
        Label = _modes[Mathf.RoundToInt(Value)].DisplayName;
        _fullScreenMode = _modes[Mathf.RoundToInt(Value)].FullScreenMode;
        _displayApplier.SetScreenMode(_fullScreenMode);
    }
}