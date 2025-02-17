using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueView : MonoBehaviour, IDisposable
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private string _key;

    private int _countOfPresets;
    private float _step;
    private int _preset;
    private bool _isStepByStep;

    public event Action<string, float> ValueChanged;

    public void Dispose()
    {
        _slider.onValueChanged.RemoveListener(SetPreset);
        _slider.onValueChanged.RemoveListener(SetValue);
    }

    public void SetStartValues(float value, string label)
    {
        _slider.value = 0;

        if (_isStepByStep)
        {
            _preset = Mathf.RoundToInt(value);

            for (float i = _preset; i > 0; i--)
                _slider.value += _step;
        }
        else
        {
            _slider.value = value;
        }

        SetLabel(label);
    }

    public void SetProperty(int? countOfPresets = null, string header = "", string key = "")
    {
        bool isNewKey = string.IsNullOrEmpty(key) == false;
        _key = isNewKey ? key : _key;

        if (CheckStepByStep(countOfPresets))
        {
            SetStep((int)countOfPresets);
            _slider.onValueChanged.AddListener(SetPreset);
        }
        else
        {
            _slider.onValueChanged.AddListener(SetValue);
        }

        if (string.IsNullOrEmpty(header) == false)
            _header.text = header;
        else if (isNewKey)
            _header.text = _key;
    }

    private bool CheckStepByStep(int? countOfPresets)
    {
        _isStepByStep = countOfPresets is not null;
        return _isStepByStep;
    }

    private void SetStep(int countOfPresets)
    {
        _countOfPresets = Math.Max(0, countOfPresets);
        _step = _countOfPresets.Equals(0) ? 0 : _slider.maxValue / countOfPresets;
    }

    private void SetValue(float value)
    {
        ValueChanged?.Invoke(_key, value);
    }

    private void SetPreset(float value)
    {
        decimal stepDec = (decimal)_step;
        decimal valueDec = (decimal)value;
        decimal snappedValueDec = Math.Round(valueDec / stepDec) * stepDec;

        float snappedValue = (float)snappedValueDec;

        if (value != snappedValue)
        {
            value = snappedValue;
            _preset = Mathf.RoundToInt(value / _step);
        }

        _slider.value = value;
        ValueChanged?.Invoke(_key, _preset);
    }

    public void SetLabel(string label)
    {
        _label.text = label;
    }
}