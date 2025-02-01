using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueView : MonoBehaviour
{
    private const int Ten = 10;
    private const int Min = 0;
    private const int Max = 3;

    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private string _suffix;
    [SerializeField] private string _key;
    [SerializeField, Range(Min, Max)] private int _additionalZero;
    [SerializeField, Range(Min, Max)] private int _pointsAfterDot;

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

    public void SetProperty(string header = "", string suffix = "", string key = "", int additionalZero = int.MinValue, int pointsAfterDot = int.MinValue)
    {
        bool isNewKey = string.IsNullOrEmpty(key) == false;
        _key = isNewKey ? key : _key;

        if (string.IsNullOrEmpty(header))
            _header.text = header;
        else if(isNewKey)
            _header.text = ConvertKeyToLabel();

        _suffix = string.IsNullOrEmpty(suffix) ? _suffix : suffix;
        _additionalZero = additionalZero.Equals(int.MinValue) ? _additionalZero : Mathf.Clamp(additionalZero, Min, Max);
        _pointsAfterDot = pointsAfterDot.Equals(int.MinValue) ? _pointsAfterDot : Mathf.Clamp(pointsAfterDot, Min, Max); ;
    }

    private string ConvertKeyToLabel()
    {
        return _key;
    }

    private void SetLabel(float value)
    {
        _label.text = (_multiplier * value).ToString($"F{_pointsAfterDot}") + _suffix;
        ValueChanged?.Invoke(_key, value);
    }
}
