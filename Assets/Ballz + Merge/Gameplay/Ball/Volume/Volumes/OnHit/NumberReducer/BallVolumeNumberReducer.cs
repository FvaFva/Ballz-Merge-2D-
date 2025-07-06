using System;
using BallzMerge.Gameplay.Level;
using UnityEngine;

[CreateAssetMenu(fileName = "NumberReducer", menuName = "Bellz+Merge/Ball/Volume/OnHit/NumberReducer", order = 51)]
public class BallVolumeNumberReducer : BallVolumeOnHit
{
    public override void Explore(BallVolumeHitData data, DropRarity rarity, Action<bool> callback)
    {
        data.Block.ChangeNumber(-rarity.Weight);
        callback(true);
    }
}
