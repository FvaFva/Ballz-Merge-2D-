using BallzMerge.Data;
using System;
using UnityEngine;

public class QualityPreset : IGameSettingData
{
    private const int MaxValue = 1;

    private float _currentValue = 0f;
    private float _step;
    private int _preset;

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }

    public event Action<IGameSettingData> ValueChanged;

    public QualityPreset(string name)
    {
        Name = name;

        if (QualitySettings.count > 1)
            _step = (float)MaxValue / (QualitySettings.count - 1);
        else
            _step = 0;
    }

    public void Change(float value)
    {
        if (_step == 0)
            _preset = 0;
        else
            SetPreset(value);

        QualitySettings.SetQualityLevel(_preset, true);
        Value = _currentValue;
        ValueChanged?.Invoke(this);
        Label = QualitySettings.names[_preset];
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