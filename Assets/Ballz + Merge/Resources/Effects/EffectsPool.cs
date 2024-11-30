using System.Collections.Generic;
using UnityEngine;

public class EffectsPool : CyclicBehavior, IInitializable
{
    [SerializeField] private EffectBase _effectPrefab;
    [SerializeField] private int _poolSize = 5;

    private Queue<EffectBase> _pool = new Queue<EffectBase>();

    public void Init()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            EffectBase newEffect = Instantiate(_effectPrefab);
            newEffect.gameObject.SetActive(false);
            _pool.Enqueue(newEffect);
        }
    }

    public EffectBase GetEffect(Vector3 position, Quaternion rotation)
    {
        EffectBase returnEffect;

        if (_pool.Count > 0 && !_pool.Peek().gameObject.activeSelf)
            returnEffect = _pool.Dequeue();
        else
            returnEffect = Instantiate(_effectPrefab, transform);

        returnEffect.transform.position = position;
        returnEffect.transform.rotation = rotation;
        returnEffect.gameObject.SetActive(true);
        _pool.Enqueue(returnEffect);
        returnEffect.Play(position);
        returnEffect.Played += OnPlayedEffect;
        return returnEffect;
    }

    private void OnPlayedEffect(EffectBase baseEffect)
    {
        baseEffect.gameObject.SetActive(false);
        baseEffect.Played -= OnPlayedEffect;
    }
}
