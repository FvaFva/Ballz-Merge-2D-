using UnityEngine;

public class InteractionEffect : EffectBase
{
    public void SetPosition(Vector3 position, Quaternion rotation)
    {
        Transform.position = position;
        Transform.rotation = rotation;
    }
}
