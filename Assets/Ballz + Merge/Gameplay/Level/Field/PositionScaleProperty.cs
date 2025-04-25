using UnityEngine;

public struct PositionScaleProperty
{
    public Vector3 Position;
    public Vector3 Scale;
    public float UnionScale;

    public PositionScaleProperty(float positionX, float positionY, float scaleX, float scaleY)
    {
        Position = new Vector3(positionX, positionY);
        Scale = new Vector3(scaleX, scaleY);
        UnionScale = 1;
    }

    public PositionScaleProperty(float unionScale, Vector3 position)
    {
        UnionScale = unionScale;
        Position = position;
        Scale = Vector3.one;
    }

    public PositionScaleProperty(float unionScale)
    {
        UnionScale = unionScale;
        Position = Vector3.zero;
        Scale = Vector3.one;
    }

    public PositionScaleProperty Add(float unionScale, Vector3 position)
    {
        Position += position;
        UnionScale += unionScale;
        return this;
    }
}
