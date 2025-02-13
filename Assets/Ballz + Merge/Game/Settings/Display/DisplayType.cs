using UnityEngine;

public struct DisplayType
{
    public FullScreenMode FullScreenMode;
    public string DisplayName;

    public DisplayType(FullScreenMode fullScreenMode, string displayName)
    {
        FullScreenMode = fullScreenMode;
        DisplayName = displayName;
    }
}
