using System;
using BallzMerge.Gameplay.Level;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveIncreaser", menuName = "Bellz+Merge/Ball/Volume/OnHit/MoveIncreaser", order = 51)]
public class BallVolumeMoveIncreaser : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, DropRarity rarity, Action<bool> callback)
    {
        bool isExtraWent = false;

        for (int i = rarity.Weight; i > 0; i--)
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
