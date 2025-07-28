using System;
using BallzMerge.Gameplay.Level;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerImpulse", menuName = "Bellz+Merge/Ball/Volume/OnHit/PowerImpulse", order = 51)]
public class BallVolumePowerImpulse : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, int value, Action<bool> callback)
    {
        bool isExtraWent = false;

        for (int i = value; i > 0; i--)
        {
            Vector2Int extraBlockPosition = data.Block.GridPosition + data.Direction * i;
            var extraBlock = Blocks.GetAtPosition(extraBlockPosition);

            if (extraBlock != null && extraBlock.CanMove(data.Direction))
            {
                extraBlock.Move(data.Direction, BlockMoveActionType.ChangePosition);
                isExtraWent = true;
            }
        }

        callback(isExtraWent);
    }
}
