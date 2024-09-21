using UnityEngine;

public struct StartProperty
{
    public Vector3 ColliderOffset;
    public Vector3 ColliderSize;

    public StartProperty(Vector3 offset, Vector3 size)
    {
        ColliderOffset = offset;
        ColliderSize = size;
    }
}
