using System;
using UnityEngine;

[Serializable]
public class GameSettingProperty
{
    [SerializeField] private SliderValueView _sliderValueView;
    [SerializeField] private SettingTypeProperty _settingTypeProperty;
    [SerializeField] private float _height;

    public GameSettingType SettingType => _settingTypeProperty.GameSettingType;
    public SliderPostInitType PostInitType => _settingTypeProperty.PostInitType;
    public SliderValueView SliderValueView => _sliderValueView;
    public float Height => _height;

    public void SetSliderView(SliderValueView sliderValueView)
    {
        _sliderValueView = sliderValueView;
    }
}