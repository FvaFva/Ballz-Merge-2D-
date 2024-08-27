using System;
using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionEffect;

    private Transform _transform;
    private WaitForSeconds _particlesLifetime;

    public event Action<ExplosionEffect> Played;

    private void Awake()
    {
        _transform = transform;
        _particlesLifetime = new WaitForSeconds(_explosionEffect.main.duration);
    }

    public void Initialize(Transform parent)
    {
        _transform.parent = parent;
    }

    public void Play(Vector3 position)
    {
        _transform.position = position;
        _explosionEffect.Play();
        StartCoroutine(WaitPlayback());
    }

    private IEnumerator WaitPlayback()
    {
        yield return _particlesLifetime;
        Played?.Invoke(this);
    }
}
