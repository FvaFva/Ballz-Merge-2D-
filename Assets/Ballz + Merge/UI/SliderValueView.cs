using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueView : MonoBehaviour
{
    private const int Ten = 10;

    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private string _suffix;
    [SerializeField] private string _key;
    [SerializeField, Range(0, 3)] private int _additionalZero;
    [SerializeField, Range(0, 3)] private int _pointsAfterDot;

    private int _multiplier;

    public event Action<string, float> ValueChanged;

    private void Awake()
    {
        _multiplier = (int)Math.Pow(Ten, _additionalZero);
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(SetLabel);
        SetLabel(_slider.value);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(SetLabel);
    }

    public void SetValue(float value)
    {
        _slider.value = value;
        SetLabel(value);
    }

    public void SetProperty(string label = "", string suffix = "", string key = "", int additionalZero = 0, int pointsAfterDot = 0)
    {
        _header.text = string.IsNullOrEmpty(label) ? _label.text : label;
        _suffix = string.IsNullOrEmpty(suffix) ? _suffix : suffix;
        _key = string.IsNullOrEmpty(key) ? _key : key;
        _additionalZero = additionalZero.Equals(0) ? _additionalZero : additionalZero;
        _pointsAfterDot = pointsAfterDot.Equals(0) ? _pointsAfterDot : pointsAfterDot;
    }

    private void SetLabel(float value)
    {
        _label.text = (_multiplier * value).ToString($"F{_pointsAfterDot}") + _suffix;
        ValueChanged?.Invoke(_key, value);
    }
}
