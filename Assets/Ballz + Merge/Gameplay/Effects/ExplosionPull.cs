using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ExplosionPull : CyclicBehaviour, IInitializable
{
    [SerializeField] private ExplosionEffect _prefab;
    [SerializeField] private Transform _effectParent;
    [SerializeField] private int _countPreload;

    private Queue<ExplosionEffect> _effects = new Queue<ExplosionEffect>();

    public void Init()
    {
        for (int i = 0; i < _countPreload; i++)
            _effects.Enqueue(Generate());
    }

    public ExplosionEffect SpawnEffect(Vector3 position)
    {
        ExplosionEffect effect = new();

        if (_effects.TryDequeue(out effect) == false)
            effect = Generate();

        effect.Played += Deactivated;
        effect.Play(position);

        return effect;
    }

    private void Deactivated(ExplosionEffect effect)
    {
        _effects.Enqueue(effect);
        effect.Played -= Deactivated;
    }

    private ExplosionEffect Generate()
    {
        ExplosionEffect explosion = Instantiate(_prefab);
        explosion.Initialize(_effectParent);
        return explosion;
    }
}
