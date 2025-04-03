using UnityEngine;

public struct PositionScaleProperty
{
    public Vector3 Position;
    public Vector3 Scale;

    public PositionScaleProperty(Vector2 position, Vector2 scale)
    {
        Position = position;
        Scale = scale;
    }
}
