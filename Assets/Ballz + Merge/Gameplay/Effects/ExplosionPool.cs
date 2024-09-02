using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : CyclicBehavior, IInitializable
{
    [SerializeField] private BaseEffect _prefab;
    [SerializeField] private Transform _effectParent;
    [SerializeField] private int _countPreload;

    private Queue<BaseEffect> _effects = new Queue<BaseEffect>();

    public void Init()
    {
        for (int i = 0; i < _countPreload; i++)
            _effects.Enqueue(Generate());
    }

    public void SpawnEffect(Vector3 position)
    {
        BaseEffect effect = null;

        if (_effects.TryDequeue(out effect) == false)
            effect = Generate();

        effect.Played += Deactivated;
        effect.Play(position);
    }

    private void Deactivated(BaseEffect effect)
    {
        _effects.Enqueue(effect);
        effect.Played -= Deactivated;
    }

    private BaseEffect Generate()
    {
        BaseEffect explosion = Instantiate(_prefab);
        explosion.Initialize(_effectParent);
        return explosion;
    }
}
