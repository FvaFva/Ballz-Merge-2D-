using System;
using BallzMerge.Gameplay.Level;
using UnityEngine;

[CreateAssetMenu(fileName = "Crusher", menuName = "Bellz+Merge/Ball/Volume/OnHit/Crusher", order = 51)]
public class BallVolumeCrusher : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, DropRarity rarity, Action<bool> callback)
    {
        if (rarity.Weight >= 3)
        {
            data.Block.Destroy();
            callback(true);
            return;
        }

        var nextPosition = data.Block.GridPosition + data.Direction;
        bool isOutside = Grid.IsOutside(nextPosition);

        if (isOutside || (rarity.Weight == 2 && Blocks.HaveAtPosition(nextPosition)))
        {
            data.Block.Destroy();
            callback(true);
            return;
        }

        callback(false);
    }
}
