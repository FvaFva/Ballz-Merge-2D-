using BallzMerge.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMode : IGameSettingData
{
    private const int MaxValue = 1;
    private const int ModesCount = 3;

    private List<DisplayType> _modes;
    private float _currentValue = 0f;
    private float _step;
    private int _preset;
    private DisplayApplier _displayApplier;
    private FullScreenMode _fullScreenMode;

    public string Name { get; private set; }

    public float Value { get; private set; }

    public string Label { get; private set; }

    public DisplayMode(string name, DisplayApplier displayApplier)
    {
        Name = name;
        _displayApplier = displayApplier;

        _modes = new List<DisplayType>
        {
            { new DisplayType(FullScreenMode.FullScreenWindow, "В окне без рамки") },
            { new DisplayType(FullScreenMode.ExclusiveFullScreen, "Полноэкранный режим") },
            { new DisplayType(FullScreenMode.Windowed, "Оконный режим") }
        };

        _step = (float)MaxValue / (ModesCount - 1);

        for (int i = _modes.Count - 1; i > 0; i--)
        {
            _currentValue += _step;

            if (_modes[i].FullScreenMode == Screen.fullScreenMode)
            {
                _preset = i;
                break;
            }
        }

        Value = _currentValue;
        Label = _modes[_preset].DisplayName;
        _fullScreenMode = _modes[_preset].FullScreenMode;
        _displayApplier.SetScreenMode(_fullScreenMode);
    }

    public void Change(float value)
    {
        if (_step == 0)
            _preset = 0;
        else
            SetPreset(value);

        Value = _currentValue;
        Label = _modes[_preset].DisplayName;
        _fullScreenMode = _modes[_preset].FullScreenMode;
        _displayApplier.SetScreenMode(_fullScreenMode);
    }

    private void SetPreset(float value)
    {
        decimal stepDec = (decimal)_step;
        decimal valueDec = (decimal)value;
        decimal snappedValueDec = Math.Round(valueDec / stepDec) * stepDec;

        float snappedValue = (float)snappedValueDec;

        if (_currentValue != snappedValue)
        {
            _currentValue = snappedValue;
            _preset = Mathf.RoundToInt(_currentValue / _step);
        }
    }
}
