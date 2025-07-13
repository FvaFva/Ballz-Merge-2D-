using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ColossalPush", menuName = "Bellz+Merge/Ball/Volume/OnHit/ColossalPush", order = 51)]
public class BallVolumeColossalPush : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, int value, Action<bool> callback)
    {
        bool isWent = false;

        for (int i = 0; i < value; i++)
        {
            if (data.Block.CanMove(data.Direction))
            {
                isWent = true;
                data.Block.Move(data.Direction, BlockMoveActionType.Move);
            }
            else
            {
                callback.Invoke(isWent);
                return;
            }
        }
    }
}
