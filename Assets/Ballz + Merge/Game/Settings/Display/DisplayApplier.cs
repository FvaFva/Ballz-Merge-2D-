using BallzMerge.Data;
using System;
using System.Collections;
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
            if (WindowResizer.IsResizable == false)
            {
                _fullScreenMode = FullScreenMode.Windowed;
                SetResolution();
                CoroutineRunner.Instance.StartCoroutine(SetResizeNextFrame(true));
                _fullScreenMode = FullScreenMode.MaximizedWindow;
            }

            _displayResolution.ChangeState(false);
        }
        else
        {
            SetResolution();

            if (WindowResizer.IsResizable == true)
                CoroutineRunner.Instance.StartCoroutine(SetResizeNextFrame(false));

            _displayResolution.ChangeState(true);
        }
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
        yield return null;
        WindowResizer.SetResizable(state);
    }
}