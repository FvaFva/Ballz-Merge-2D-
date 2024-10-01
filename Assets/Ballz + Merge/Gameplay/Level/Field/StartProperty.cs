using UnityEngine;

public struct StartProperty
{
    public Vector2 ColliderOffset;
    public Vector2 ColliderSize;

    public StartProperty(Vector2 offset, Vector2 size)
    {
        ColliderOffset = offset;
        ColliderSize = size;
    }
}
