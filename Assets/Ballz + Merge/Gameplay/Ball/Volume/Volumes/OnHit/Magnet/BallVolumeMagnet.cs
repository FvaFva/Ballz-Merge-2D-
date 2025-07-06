using System;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using UnityEngine;

[CreateAssetMenu(fileName = "Magnet", menuName = "Bellz+Merge/Ball/Volume/OnHit/Magnet", order = 51)]
public class BallVolumeMagnet : BallVolumeOnHit
{
    private BlockMagneticObserver _magneticObserver;

    public override void Explore(BallVolumeHitData data, DropRarity rarity, Action<bool> callback)
    {
        _magneticObserver.Activate(data, rarity, callback);
    }

    protected override void Init()
    {
        _magneticObserver = Container.Instantiate<BlockMagneticObserver>();
    }
}
