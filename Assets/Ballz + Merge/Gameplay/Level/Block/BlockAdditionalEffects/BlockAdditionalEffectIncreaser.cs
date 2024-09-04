using BallzMerge.Gameplay.BlockSpace;
using UnityEngine;

public class BlockAdditionalEffectIncreaser : BlockAdditionalEffectBase
{
    [SerializeField] private ParticleSystem _particleEffect;

    private BlocksInGame _activeBlocks;

    private void FixedUpdate()
    {
        if (Current != null)
            _particleEffect.transform.position = Current.WorldPosition;
    }

    public override void HandleEvent(BlockAdditionalEffectEventProperty property)
    {
        if (property.EffectEvents == BlockAdditionalEffectEvents.Destroy && property.Current == Current)
            Deactivate();
    }

    public override void HandleWave()
    {
        Block block = _activeBlocks.GetRandomBlock();
        InvokeActionNumberChanged(block, 1, true);
        block.ShakeScale();
    }

    protected override bool TryInit(BlocksInGame blocks)
    {
        if (blocks == null)
            return false;

        _activeBlocks = blocks;
        Current.ConnectEffect();
        return true;
    }
}
