using BallzMerge.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SliderProperty : DependentColorUI, IDisposable
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private Image _fillImage;
    [SerializeField] private SliderHandle _sliderHandle;
    [SerializeField] private List<DependentColorUI> _dependentColorUIs;
    [SerializeField] private string _key;

    public IGameSettingData SettingData { get; private set; }

    private int _countOfPresets;
    private float _step;
    private int _preset;
    private bool _isStepByStep;
    private GameColors _gameColors;
    private ValueChanger _valueChanger;
    private bool _isDragging;
    private float _lastValue;
    private bool _isGradient;
    private bool _isActive;
    private Dictionary<SliderPostInitType, Action> _postInitTypeActions;

    public event Action<string, float> ValueChanged;

    public void Dispose()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        SettingData.StateChanged -= SetSliderState;
        _sliderHandle.SliderHandled -= OnSliderHandled;
        _slider.onValueChanged.RemoveListener(SetHandleColor);
        SettingData.StateChanged -= (bool _) => SetHandleColor(_slider.value);
    }

    public void Init(IGameSettingData settingData)
    {
        _isActive = true;
        name = settingData.Name;
        _valueChanger = new ValueChanger();
        _sliderHandle.SliderHandled += OnSliderHandled;
        SettingData = settingData;
        SettingData.StateChanged += SetSliderState;

        _postInitTypeActions = new Dictionary<SliderPostInitType, Action>
        {
            { SliderPostInitType.None, () => { } },
            { SliderPostInitType.GenerateTexture, () => CreateGradientTexture() }
        };
    }

    public override void ApplyColors(GameColors gameColors)
    {
        _gameColors = gameColors;
        _sliderHandle.ApplyColors(_gameColors);

        if (_isActive)
            SetSliderState(true);

        else
            SetSliderState(false);

        if (_isGradient)
        {
            SetHandleColor(_slider.value);
            _slider.onValueChanged.AddListener(SetHandleColor);
            SettingData.StateChanged += (bool _) => SetHandleColor(_slider.value);
        }
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

    public SliderProperty SetProperty(SliderPostInitType postInitType, int? countOfPresets, string header, string key)
    {
        bool isNewKey = string.IsNullOrEmpty(key) == false;
        _key = isNewKey ? key : _key;
        _postInitTypeActions[postInitType]();

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

    private void CreateGradientTexture()
    {
        int width = 256;
        Texture2D gradientTexture = new Texture2D(width, 1, TextureFormat.RGB24, false);
        gradientTexture.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < width; x++)
        {
            float t = x / (float)(width - 1);
            Color color = Color.HSVToRGB(t, 1f, 1f);
            gradientTexture.SetPixel(x, 0, color);
        }

        gradientTexture.Apply();
        _rawImage.enabled = true;
        _fillImage.enabled = false;
        _rawImage.texture = gradientTexture;
        _isGradient = true;
    }

    private void SetHandleColor(float value)
    {
        if (_isActive)
            _sliderHandle.SetColor(Color.HSVToRGB(value, 1f, 1f));
    }

    private void OnSliderHandled(bool isDown)
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
        _isActive = state;
        _slider.interactable = state;

        if (_gameColors != null)
        {
            _sliderHandle.SetState(_isActive);
            _fillImage.color = _gameColors.GetForAccessibilitySliderState()[_isActive];

            foreach (var dependentColorUI in _dependentColorUIs)
                dependentColorUI.ApplyColors(_gameColors);
        }
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