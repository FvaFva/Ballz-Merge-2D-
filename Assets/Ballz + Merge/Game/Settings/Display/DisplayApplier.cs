using BallzMerge.Data;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DisplayApplier : IDisposable
{
    private const float ApplyDelay = 0.2f;

    private Button _applyButton;
    private Resolution _resolution;
    private FullScreenMode _fullScreenMode;
    private IGameSettingData _resolutionData;
    private IGameSettingData _displayModeData;
    private DisplayResolution _displayResolution;

    private bool _isResolutionLoaded;
    private bool _isScreenModeLoaded;
    private bool _isEverythingLoaded;
    private bool _isResizable;

    public event Action<IGameSettingData> Applied;

    public DisplayApplier(Button applyButton)
    {
        _applyButton = applyButton;
        _applyButton.onClick.AddListener(ApplyResolutionAndTriggerChanges);
        _isEverythingLoaded = true;
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
        _isResolutionLoaded = false;
        _isScreenModeLoaded = false;

        if (_fullScreenMode == FullScreenMode.MaximizedWindow)
        {
            _fullScreenMode = FullScreenMode.Windowed;
            SetResolutionAndResize(true);
            _fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else
        {
            SetResolutionAndResize(false);
        }

        _displayResolution.ChangeState(!_isResizable);
    }

    private void SetResolutionAndResize(bool resizeState)
    {
        if (_isResizable == resizeState)
            return;

        _isResizable = resizeState;
        SetResolution();
        CoroutineRunner.Instance.StartCoroutine(SetResizeNextFrame(_isResizable));
    }

    private void SetResolution()
    {
        if (_isEverythingLoaded)
            return;

        _isEverythingLoaded = true;
        Screen.SetResolution(_resolution.width, _resolution.height, _fullScreenMode);
        Applied?.Invoke(_displayModeData);
        Applied?.Invoke(_resolutionData);
    }

    private IEnumerator SetResizeNextFrame(bool state)
    {
        yield return new WaitForSeconds(ApplyDelay);

        PlatformRunner.RunOnWindowsMacPlatform(
        windowsAction: () =>
        {
            WindowResizerWindows.SetResizable(state);
        },
        macAction: () =>
        {
            WindowResizerMac.SetResizable(state);
        });
    }
}