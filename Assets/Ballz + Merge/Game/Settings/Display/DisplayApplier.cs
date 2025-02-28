using UnityEngine;

public class DisplayApplier
{
    private Resolution _resolution;
    private FullScreenMode _fullScreenMode;

    public void SetResolution(Resolution resolution)
    {
        _resolution = resolution;
        SetResolution();
    }

    public void SetScreenMode(FullScreenMode fullScreenMode)
    {
        _fullScreenMode = fullScreenMode;
        SetResolution();
    }

    private void SetResolution()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _fullScreenMode);
    }
}