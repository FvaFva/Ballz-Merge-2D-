using BallzMerge.Data;
using UnityEngine;

public class QualityPreset : IGameSettingData
{
    private const int MaxValue = 1;

    private float _currentValue = 0f;
    private float _step;
    private int _preset;

    public string Name { get; private set; }
    public float Value { get; private set; }

    public QualityPreset(string name)
    {
        Name = name;

        if (QualitySettings.count != 1)
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

        Debug.Log(Value);
        QualitySettings.SetQualityLevel(_preset, true);
    }

    private void SetPreset(float value)
    {
        decimal stepDecimal = (decimal)_step;
        decimal currentDecimal = (decimal)_currentValue;

        decimal nextStepDecimal = currentDecimal + stepDecimal;
        decimal prevStepDecimal = currentDecimal - stepDecimal;

        float nextStep = (float)nextStepDecimal;
        float prevStep = (float)prevStepDecimal;
        float threshold = _step / 2;

        if (value > _currentValue && value >= nextStep - threshold)
        {
            _currentValue = nextStep;
            _preset++;
        }
        else if (value < _currentValue && value <= prevStep + threshold)
        {
            _currentValue = prevStep;
            _preset--;
        }

        Value = _currentValue;
    }
}