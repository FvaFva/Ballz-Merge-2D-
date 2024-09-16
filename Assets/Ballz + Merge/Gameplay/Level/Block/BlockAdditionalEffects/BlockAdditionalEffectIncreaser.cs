using BallzMerge.Gameplay.BlockSpace;
using System.Collections;
using UnityEngine;

public class BlockAdditionalEffectIncreaser : BlockAdditionalEffectBase
{
    [SerializeField] private ParticleSystem _particleEffect;
    [SerializeField] private ParticleSystem _hitImpact;

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
        Block block = _activeBlocks.GetRandomBlock(Current, true);

        if (block == null)
            return;

        _hitImpact.transform.position = Current.WorldPosition;
        _hitImpact.transform.LookAt(block.transform);
        _hitImpact.Play();
        StartCoroutine(DelayedActionNumberChanged(block));
    }

    protected override bool TryInit(BlocksInGame blocks)
    {
        if (blocks == null)
            return false;

        _activeBlocks = blocks;
        Current.ConnectEffect();
        return true;
    }

    private IEnumerator DelayedActionNumberChanged(Block block)
    {
        yield return new WaitForSeconds(_hitImpact.main.duration);
        InvokeActionNumberChanged(block, 1, true);
        block.ShakeScale();
    }
}
