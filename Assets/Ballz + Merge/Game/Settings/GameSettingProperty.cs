using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameSettingProperty
{
    [SerializeField] private SliderValueView _sliderValueView;
    [SerializeField] private GameSettingType _settingType;
    [SerializeField] private float _height;

    public GameSettingType SettingType => _settingType;
    public SliderValueView SliderValueView => _sliderValueView;
    public float Height => _height;

    public void SetSliderView(SliderValueView sliderValueView)
    {
        _sliderValueView = sliderValueView;
    }
}