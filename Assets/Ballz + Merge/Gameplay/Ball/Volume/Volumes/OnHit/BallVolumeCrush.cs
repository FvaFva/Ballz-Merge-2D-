using System;
using BallzMerge.Gameplay.Level;

public class BallVolumeCrush : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, DropRarity rarity, Action<bool> callback)
    {
        if(rarity.Weight >= 3)
        {
            data.Block.Destroy();
            callback(true);
            return;
        }

        var nextPosition = data.Block.GridPosition + data.Direction;
        bool isOutside = _grid.IsOutside(nextPosition);

        if (isOutside || (rarity.Weight == 2 && _blocks.HaveAtPosition(nextPosition)))
        {
            data.Block.Destroy();
            callback(true);
            return;
        }
        
        callback(false);
    }
}
