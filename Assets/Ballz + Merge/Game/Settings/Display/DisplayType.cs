using UnityEngine;

public struct DisplayType
{
    public ScreenMode ScreenMode;
    public string DisplayName;

    public DisplayType(ScreenMode screenMode, string displayName)
    {
        ScreenMode = screenMode;
        DisplayName = displayName;
    }
}
