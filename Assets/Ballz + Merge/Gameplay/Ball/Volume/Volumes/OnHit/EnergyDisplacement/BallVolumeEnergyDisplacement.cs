using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergyDisplacement", menuName = "Bellz+Merge/Ball/Volume/OnHit/EnergyDisplacement", order = 51)]
public class BallVolumeEnergyDisplacement : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, int value, Action<bool> callback)
    {
        Vector2Int extraBlockPosition = data.Block.GridPosition + data.Direction;
        var extraBlock = Blocks.GetAtPosition(extraBlockPosition);

        if (extraBlock is null)
        {
            callback?.Invoke(false);
            return;
        }

        int countDisplacement = Mathf.Min(value, data.Block.Number);
        data.Block.ChangeNumber(-countDisplacement);
        extraBlock.ChangeNumber(countDisplacement);
        callback?.Invoke(true);
    }
}
