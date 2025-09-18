using BallzMerge.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class SliderProperty : IDisposable
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private AnimatedButton _animatedButton;
    [SerializeField] private Image _fillImage;
    [SerializeField] private string _key;

    public IGameSettingData SettingData { get; private set; }

    private int _countOfPresets;
    private float _step;
    private int _preset;
    private bool _isStepByStep;
    private Color _startFillImageColor;
    private Dictionary<bool, Color> StateColors;

    public event Action<string, float> ValueChanged;

    public void Dispose()
    {
        _slider.onValueChanged.RemoveListener(SetPreset);
        _slider.onValueChanged.RemoveListener(OnValueChanged);
        SettingData.StateChanged -= SetSliderState;
    }

    public void Init(IGameSettingData settingData)
    {
        SettingData = settingData;
        SettingData.StateChanged += SetSliderState;
        _startFillImageColor = _fillImage.color;

        StateColors = new Dictionary<bool, Color>
        {
            { true, _startFillImageColor },
            { false, Color.white }
        };
    }

    public SliderProperty SetValue(float value)
    {
        if (_isStepByStep)
        {
            _slider.value = 0;
            _preset = Mathf.RoundToInt(value);

            for (float i = _preset; i > 0; i--)
                _slider.value += _step;
        }
        else
        {
            _slider.value = value;
        }

        return this;
    }

    public SliderProperty SetProperty(int? countOfPresets, string header, string key)
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
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        if (string.IsNullOrEmpty(header) == false)
            _header.text = header;
        else if (isNewKey)
            _header.text = _key;

        return this;
    }

    public SliderProperty SetLabel(string label)
    {
        _label.text = label;
        return this;
    }

    private void SetSliderState(bool state)
    {
        _slider.interactable = state;
        _animatedButton.SetState(state);
        _fillImage.color = StateColors[state];
    }

    private void OnValueChanged(float value)
    {
        ValueChanged?.Invoke(_key, value);
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

    private void SetPreset(float value)
    {
        value = Mathf.RoundToInt(value / _step) * _step;
        _preset = Mathf.RoundToInt(value / _step);

        _slider.value = value;
        ValueChanged?.Invoke(_key, _preset);
    }
}