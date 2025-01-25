using System.Collections.Generic;
using UnityEngine;

public class EffectPool
{
    private Queue<InteractionEffect> _pool = new Queue<InteractionEffect>();

    public void SpawnEffect(Vector3 position, Quaternion rotation, InteractionEffect prefab, Transform parent)
    {
        InteractionEffect effect;

        if (_pool.TryDequeue(out effect))
            effect.SetPosition(position, rotation);
        else
            effect = Object.Instantiate(prefab, position, rotation, parent);

        effect.Played += OnPlayedEffect;
        effect.Play(position);
    }

    private void OnPlayedEffect(EffectBase baseEffect)
    {
        _pool.Enqueue((InteractionEffect)baseEffect);
        baseEffect.Played -= OnPlayedEffect;
    }
}
