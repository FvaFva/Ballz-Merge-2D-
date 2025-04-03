using System;
using System.Collections;
using UnityEngine;

public abstract class EffectBase : MonoBehaviour
{
    [SerializeField] protected ParticleSystem Effect;

    private WaitForSeconds _particlesLifetime;

    protected Transform Transform {  get; private set; }

    public event Action<EffectBase> Played;

    private void Awake()
    {
        _particlesLifetime = new WaitForSeconds(Effect.main.duration);
        Transform = transform;
    }

    public virtual void Play(Vector3 position)
    {
        StartCoroutine(WaitPlayback());
        transform.position = position;
        Effect.Play();
    }

    private IEnumerator WaitPlayback()
    {
        yield return _particlesLifetime;
        Played?.Invoke(this);
    }
}
