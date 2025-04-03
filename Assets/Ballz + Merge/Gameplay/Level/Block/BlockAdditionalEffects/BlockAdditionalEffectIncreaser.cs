using BallzMerge.Gameplay.BlockSpace;
using System.Collections;
using UnityEngine;

public class BlockAdditionalEffectIncreaser : BlockAdditionalEffectBase
{
    private const int Power = 1;

    [SerializeField] private ParticleSystem _particleEffect;
    [SerializeField] private ParticleSystem _hitImpact;

    private Transform _particleTransform;
    private Transform _hitImpactTransform;

    public override void HandleWave()
    {
        Block block = ActiveBlocks.GetRandomBlock(Current, true);

        if (block == null)
            return;

        _hitImpactTransform.position = Current.WorldPosition;
        _hitImpactTransform.LookAt(block.transform);
        _hitImpact.Play();
        StartCoroutine(DelayedActionNumberChanged(block));
    }

    protected override bool TryActivate()
    {
        _particleEffect.Play();
        Current.ConnectEffect();
        return true;
    }

    protected override void HandleUpdate() => _particleTransform.position = Current.WorldPosition;

    protected override void Init()
    {
        _particleTransform = _particleEffect.transform;
        _hitImpactTransform = _hitImpact.transform;
    }

    protected override void HandleDeactivate()
    {
        _particleEffect.Stop();
    }

    private IEnumerator DelayedActionNumberChanged(Block block)
    {
        yield return new WaitForSeconds(_hitImpact.main.duration);
        block.ChangeNumber(Power);
        block.PlayShakeAnimation();
        EffectsPool.SpawnEffect(BlockAdditionalEffectEvents.Increase, block.WorldPosition);
    }
}
