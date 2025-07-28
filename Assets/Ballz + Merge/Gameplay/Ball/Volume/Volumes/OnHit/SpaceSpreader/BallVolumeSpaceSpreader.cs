using System;
using BallzMerge.Gameplay.BlockSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "SpaceSpreader", menuName = "Bellz+Merge/Ball/Volume/OnHit/SpaceSpreader", order = 51)]
public class BallVolumeSpaceSpreader : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, int value, Action<bool> callback)
    {
        int blockHorizontal = data.Block.GridPosition.x;
        int rightBoard = Grid.Size.x - 1;

        if (data.Direction.x == 0 || (blockHorizontal != 0 && blockHorizontal != rightBoard))
        {
            callback.Invoke(false);
            return;
        }

        Vector2Int targetPosition = new Vector2Int(blockHorizontal != 0 ? 0 : rightBoard, data.Block.GridPosition.y);
        var blockInTargetPosition = Blocks.GetAtPosition(targetPosition);

        if (IsNeedMove(data.Block, blockInTargetPosition, value, out bool success))
            data.Block.MoveTo(targetPosition, BlockMoveActionType.ChangePosition);

        callback.Invoke(success);
    }

    private bool IsNeedMove(Block first, Block second, int rarity, out bool success)
    {
        success = second is null;

        if (success || rarity == Common)
            return success;

        success = Blocks.TryMergeBlocks(first, second);

        if (success || rarity == Rar)
            return !success;

        success = second.CanMove(Vector2Int.up);
        return success;
    }
}
