using UnityEngine;

public struct PositionScaleProperty
{
    public Vector2 Position;
    public Vector2 Scale;

    public PositionScaleProperty(Vector2 position, Vector2 scale)
    {
        Position = position;
        Scale = scale;
    }
}
