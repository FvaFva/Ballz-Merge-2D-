using BallzMerge.Gameplay.BlockSpace;
using UnityEngine;

public class BlockAdditionalEffectIncreaser : BlockAdditionalEffectBase
{
    [SerializeField] private ParticleSystem _particleFirst;

    public override void HandleEvent(BlockAdditionalEffectEventProperty property)
    {
        if (property.Current != Current)
        {
            InvokeActionNumberChanged(property.Current, property.Count);
            property.Current.ShakeScale();
        }
    }

    public override void HandleWave()
    {
        if (Current == null)
            Deactivate();
    }

    protected override bool TryInit(BlocksInGame blocks)
    {
        if (blocks == null)
            return false;

        Current.ConnectEffect();
        return true;
    }
}
