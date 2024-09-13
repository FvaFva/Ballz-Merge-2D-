using BallzMerge.Gameplay.BlockSpace;
using System;
using System.Collections;
using UnityEngine;

public abstract class BaseEffect : MonoBehaviour
{
    [SerializeField] protected ParticleSystem Effect;
    [SerializeField] protected BlockAdditionalEffectEvents CurrentEvent;

    protected Transform Transform;

    private WaitForSeconds _particlesLifetime;

    public BlockAdditionalEffectEvents ResponsibleEvent => CurrentEvent;

    public event Action<BaseEffect> Played;

    private void Awake()
    {
        Transform = transform;
        _particlesLifetime = new WaitForSeconds(Effect.main.duration);
    }

    public void Initialize(Transform parent)
    {
        Transform.parent = parent;
    }

    public virtual void Play(Vector3 position)
    {
        StartCoroutine(WaitPlayback());
    }

    private IEnumerator WaitPlayback()
    {
        yield return _particlesLifetime;
        Played?.Invoke(this);
    }
}
