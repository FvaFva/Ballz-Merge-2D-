using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DestructiveShell", menuName = "Bellz+Merge/Ball/Volume/OnHit/DestructiveShell", order = 51)]
public class BallVolumeDestructiveShell : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, int value, Action<bool> callback)
    {
        data.Block.ChangeNumber(-value);
        callback(true);
    }
}
