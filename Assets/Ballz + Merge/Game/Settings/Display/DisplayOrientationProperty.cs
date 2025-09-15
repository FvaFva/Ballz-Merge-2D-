using UnityEngine;

public struct DisplayOrientationProperty
{
    public ScreenOrientation Orientation;
    public string OrientationName;

    public DisplayOrientationProperty(ScreenOrientation orientation, string orientationName)
    {
        Orientation = orientation;
        OrientationName = orientationName;
    }
}
