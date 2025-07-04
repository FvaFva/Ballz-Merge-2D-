using System;
using BallzMerge.Gameplay.Level;

public abstract class BallVolumeOnHit : BallVolume
{
    public abstract void Explore(BallVolumeHitData data, DropRarity rarity, Action<bool> callback);
}
