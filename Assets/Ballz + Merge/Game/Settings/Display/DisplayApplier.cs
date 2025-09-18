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
    private DisplayResolution _displayResolution;

    private bool _isResolutionLoaded;
    private bool _isScreenModeLoaded;
    private bool _isEverythingLoaded;

    public event Action<IGameSettingData> Applied;

    public DisplayApplier(Button applyButton)
    {
        _applyButton = applyButton;
        _applyButton.onClick.AddListener(ApplyResolutionAndTriggerChanges);
    }

    public void Dispose()
    {
        _applyButton.onClick.RemoveListener(ApplyResolutionAndTriggerChanges);
    }

    public void SetResolution(Resolution resolution, IGameSettingData settingData, DisplayResolution displayResolution)
    {
        _resolution = resolution;
        _resolutionData = settingData;
        _displayResolution = displayResolution;
    }

    public void SetLoadResolution()
    {
        _isResolutionLoaded = true;
        SetLoadResolutionAndScreenMode();
    }

    public void SetScreenMode(FullScreenMode fullScreenMode, IGameSettingData settingData)
    {
        _fullScreenMode = fullScreenMode;
        _displayModeData = settingData;
    }

    public void SetLoadScreenMode()
    {
        _isScreenModeLoaded = true;
        SetLoadResolutionAndScreenMode();
    }

    private void SetLoadResolutionAndScreenMode()
    {
        if (_isResolutionLoaded == true && _isScreenModeLoaded == true)
            ApplyResolution();
    }

    private void ApplyResolutionAndTriggerChanges()
    {
        _isEverythingLoaded = false;
        ApplyResolution();
    }

    private void ApplyResolution()
    {
        if (_isEverythingLoaded)
            return;

        if (_fullScreenMode == FullScreenMode.MaximizedWindow)
        {
            _fullScreenMode = FullScreenMode.Windowed;
            WindowResizer.SetResizable(true);
            _displayResolution.ChangeState(false);
            SetResolution();
            _fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else
        {
            WindowResizer.SetResizable(false);
            _displayResolution.ChangeState(true);
            SetResolution();
        }
    }

    private void SetResolution()
    {
        _isEverythingLoaded = true;
        Screen.SetResolution(_resolution.width, _resolution.height, _fullScreenMode);
        Applied?.Invoke(_displayModeData);
        Applied?.Invoke(_resolutionData);
    }
}