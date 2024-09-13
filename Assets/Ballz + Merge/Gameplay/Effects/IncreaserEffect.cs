using UnityEngine;

public class IncreaserEffect : BaseEffect
{
    public override void Play(Vector3 position)
    {
        Transform.position = position;
        Effect.Play();
        base.Play(position);
    }
}
