using BallzMerge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueView : MonoBehaviour, IDisposable
{
    [SerializeField] private List<SliderProperty> _slidersProperty;
    [SerializeField] private Button _applyButton;

    public Button ApplyButton => _applyButton;
    public RectTransform RectTransform { get; private set; }

    private Dictionary<IGameSettingData, SliderProperty> _slidersTypes = new Dictionary<IGameSettingData, SliderProperty>();

    public event Action<string, float> ValueChanged;

    public void Init(IGameSettingData settingData)
    {
        SliderProperty sliderProperty = _slidersProperty.Where(sp => sp.SettingData == null).FirstOrDefault();
        sliderProperty.Init(settingData);
        _slidersTypes.Add(sliderProperty.SettingData, sliderProperty);
        sliderProperty.ValueChanged += SetValue;
        RectTransform = (RectTransform)transform;
    }

    public void Dispose()
    {
        foreach (SliderProperty sliderProperty in _slidersTypes.Values)
            sliderProperty.ValueChanged -= SetValue;
    }

    public void SetStartValues(IGameSettingData settingData)
    {
        SliderProperty sliderProperty = _slidersTypes.Values.Where(sp => sp.SettingData == settingData).FirstOrDefault();
        sliderProperty.SetLabel(settingData.Label);
        _slidersTypes[settingData] = sliderProperty.SetValue(settingData.Value);
    }

    public SliderValueView SetProperty(IGameSettingData settingData, int? countOfPresets = null, string header = "", string key = "")
    {
        SliderProperty sliderProperty = _slidersTypes.Values.Where(sp => sp.SettingData == settingData).FirstOrDefault();
        _slidersTypes[settingData] = sliderProperty.SetProperty(countOfPresets, header, key);

        return this;
    }

    private void SetValue(string key, float value)
    {
        ValueChanged?.Invoke(key, value);
    }

    public void SetLabel(IGameSettingData settingData)
    {
        SliderProperty sliderProperty = _slidersTypes.Values.Where(sp => sp.SettingData == settingData).FirstOrDefault();
        _slidersTypes[settingData] = sliderProperty.SetLabel(settingData.Label);
    }
}