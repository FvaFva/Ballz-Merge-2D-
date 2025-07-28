using System;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using UnityEngine;

[CreateAssetMenu(fileName = "MagneticField", menuName = "Bellz+Merge/Ball/Volume/OnHit/MagneticField", order = 51)]
public class BallVolumeMagneticField : BallVolumeOnHit
{
    private BlockMagneticObserver _magneticObserver;

    public override void Explore(BallVolumeHitData data, int value, Action<bool> callback)
    {
        _magneticObserver.Activate(data, value, callback);
    }

    protected override void Init()
    {
        _magneticObserver = Container.Instantiate<BlockMagneticObserver>();
    }
}
