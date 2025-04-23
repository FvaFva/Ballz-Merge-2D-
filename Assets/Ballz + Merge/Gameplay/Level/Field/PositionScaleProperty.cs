using UnityEngine;

public struct PositionScaleProperty
{
    public Vector3 Position;
    public Vector3 Scale;

    public PositionScaleProperty(float positionX, float positionY, float scaleX, float scaleY)
    {
        Position = new Vector3(positionX, positionY);
        Scale = new Vector3(scaleX, scaleY);
    }
}
