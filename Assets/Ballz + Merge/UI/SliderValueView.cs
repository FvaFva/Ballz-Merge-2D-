using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueView : MonoBehaviour
{
    private const int Ten = 10;

    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private string _suffix;
    [SerializeField, Range(0, 3)] private int _additionalZero;
    [SerializeField, Range(0, 3)] private int _pointsAfterDot;

    private int _multiplier;

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

    private void SetLabel(float value)
    {
        _label.text = (_multiplier * value).ToString($"F{_pointsAfterDot}") + _suffix;
    }
}
