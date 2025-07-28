using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ExtremePressure", menuName = "Bellz+Merge/Ball/Volume/OnHit/ExtremePressure", order = 51)]
public class BallVolumeExtremePressure : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, int value, Action<bool> callback)
    {
        if (value >= 3)
        {
            data.Block.Destroy();
            callback(true);
            return;
        }

        var nextPosition = data.Block.GridPosition + data.Direction;
        bool isOutside = Grid.IsOutside(nextPosition);

        if (isOutside || (value == 2 && Blocks.HaveAtPosition(nextPosition)))
        {
            data.Block.Destroy();
            callback(true);
            return;
        }

        callback(false);
    }
}
