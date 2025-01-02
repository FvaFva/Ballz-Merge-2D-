using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    private Queue<InteractionEffect> _pool = new Queue<InteractionEffect>();

    public InteractionEffect GetEffect(Vector3 position, Quaternion rotation, InteractionEffect effect, Transform parent)
    {
        InteractionEffect returnEffect;

        if (_pool.Count > 0)
            returnEffect = _pool.Dequeue();
        else
            returnEffect = Instantiate(effect, parent);

        returnEffect.transform.position = position;
        returnEffect.transform.rotation = rotation;
        returnEffect.Play(position);
        returnEffect.Played += OnPlayedEffect;
        return returnEffect;
    }

    private void OnPlayedEffect(EffectBase baseEffect)
    {
        _pool.Enqueue((InteractionEffect)baseEffect);
        baseEffect.Played -= OnPlayedEffect;
    }
}
