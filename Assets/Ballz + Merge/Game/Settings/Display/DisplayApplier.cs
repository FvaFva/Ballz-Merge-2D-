using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayApplier : IDisposable
{
    private Button _applyButton;
    private Resolution _resolution;
    private FullScreenMode _fullScreenMode;

    public DisplayApplier(Button applyButton)
    {
        _applyButton = applyButton;
        _applyButton.onClick.AddListener(SetResolution);
    }

    public void Dispose()
    {
        _applyButton.onClick.RemoveListener(SetResolution);
    }

    public void SetResolution(Resolution resolution)
    {
        _resolution = resolution;
    }

    public void SetScreenMode(FullScreenMode fullScreenMode)
    {
        _fullScreenMode = fullScreenMode;
    }

    private void SetResolution()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _fullScreenMode);
    }
}