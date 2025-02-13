using BallzMerge.Data;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DisplayResolution : IGameSettingData
{
    private const int MaxValue = 1;

    private float _currentValue = 0f;
    private float _step;
    private int _preset;
    private Resolution[] _resolutions;
    private DisplayApplier _displayApplier;
    private Resolution _resolution;

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }

    public DisplayResolution(string name, DisplayApplier displayApplier)
    {
        Name = name;
        _displayApplier = displayApplier;

        _resolutions = Screen.resolutions
            .Select(res => new Resolution { width = res.width, height = res.height })
            .Distinct()
            .ToArray();

        if (_resolutions.Length > 1)
            _step = (float)MaxValue / (_resolutions.Length - 1);
        else
            _step = 0;

        _preset = _resolutions.ToList().FindIndex(res => res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height);
        _resolution = _resolutions[_preset];
        _displayApplier.SetResolution(_resolution);

        for (float i = _preset; i > 0; i--)
            _currentValue += _step;

        Value = _currentValue;
        Label = $"{_resolutions[_preset].width}x{_resolutions[_preset].height}";
    }

    public void Change(float value)
    {
        if (_step == 0)
            _preset = 0;
        else
            SetPreset(value);

        Value = _currentValue;
        Label = $"{_resolutions[_preset].width}x{_resolutions[_preset].height}";
        _resolution = _resolutions[_preset];
        _displayApplier.SetResolution(_resolution);
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