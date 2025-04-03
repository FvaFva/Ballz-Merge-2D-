using BallzMerge.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayApplier : IDisposable
{
    private Button _applyButton;
    private Resolution _resolution;
    private FullScreenMode _fullScreenMode;
    private IGameSettingData _resolutionData;
    private IGameSettingData _displayModeData;

    public event Action<IGameSettingData> Applied;

    public DisplayApplier(Button applyButton)
    {
        _applyButton = applyButton;
        _applyButton.onClick.AddListener(SetResolution);
    }

    public void Dispose()
    {
        _applyButton.onClick.RemoveListener(SetResolution);
    }

    public void SetResolution(Resolution resolution, IGameSettingData settingData, float value)
    {
        _resolution = resolution;
        _resolutionData = settingData;
    }

    public void SetScreenMode(FullScreenMode fullScreenMode, IGameSettingData settingData, float value)
    {
        _fullScreenMode = fullScreenMode;
        _displayModeData = settingData;
    }

    private void SetResolution()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _fullScreenMode);
        Applied?.Invoke(_displayModeData);
        Applied?.Invoke(_resolutionData);
    }
}