using System;
using System.Collections.Generic;
using BallzMerge.Gameplay.Level;

[Serializable]
public struct DropEntry
{
    public DropRarity Rarity;
    public List<BallVolume> Volumes;
}