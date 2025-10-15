using BallzMerge.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class SliderProperty : IDisposable
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private Image _fillImage;
    [SerializeField] private SliderDragger _sliderDragger;
    [SerializeField] private SliderHandle _sliderHandle;
    [SerializeField] private string _key;

    public IGameSettingData SettingData { get; private set; }

    private int _countOfPresets;
    private float _step;
    private int _preset;
    private bool _isStepByStep;
    private Color _startFillImageColor;
    private Dictionary<bool, Color> StateColors;
    private ValueChanger _valueChanger;
    private bool _isDragging;
    private float _lastValue;
    private Coroutine _animationRoute;

    public event Action<string, float> ValueChanged;

    public void Dispose()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        SettingData.StateChanged -= SetSliderState;
        _sliderDragger.Handled -= OnStickHandled;
    }

    public void Init(IGameSettingData settingData)
    {
        _valueChanger = new ValueChanger();
        _sliderDragger.Handled += OnStickHandled;
        _sliderHandle.Init();
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
            _slider.SetValueWithoutNotify(0);
            _preset = Mathf.RoundToInt(value);

            for (float i = _preset; i > 0; i--)
                _slider.SetValueWithoutNotify(_slider.value + _step);
        }
        else
        {
            _slider.SetValueWithoutNotify(value);
        }

        _lastValue = _slider.value;

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
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
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

    private void OnStickHandled(bool isDown)
    {
        if (isDown)
            OnPointerDown();
        else
            OnPointerUp();
    }

    private void OnPointerDown()
    {
        _isDragging = true;
        _valueChanger.Stop();
    }

    private void OnPointerUp()
    {
        _isDragging = false;
    }

    private void OnSliderValueChanged(float value)
    {
        if (_isDragging)
        {
            ValueChanged?.Invoke(_key, value);
            _lastValue = value;
            return;
        }

        AnimateTo(value);
    }

    private void SetPreset(float value)
    {
        float target = Mathf.RoundToInt(value / _step) * _step;
        _preset = Mathf.RoundToInt(target / _step);

        if (_isDragging)
        {
            _slider.SetValueWithoutNotify(target);
            ValueChanged?.Invoke(_key, _preset);
            _lastValue = target;
            return;
        }

        AnimateTo(target, _preset);
    }

    private void AnimateTo(float target, int? preset = null)
    {
        ValueChanged?.Invoke(_key, preset == null ? target : (int)preset);
        _valueChanger.ChangeValueOverTime(_lastValue, target, OnValueAnimationChanged, () => OnAnimationEnded(target), 0.3f);
    }

    private void OnValueAnimationChanged(float newValue)
    {
        _slider.SetValueWithoutNotify(newValue);
        _lastValue = newValue;
    }

    private void OnAnimationEnded(float target)
    {
        _slider.SetValueWithoutNotify(target);
    }

    private void SetSliderState(bool state)
    {
        _slider.interactable = state;
        _sliderHandle.SetState(state);
        _fillImage.color = StateColors[state];
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
}