using System;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [Serializable]
    public struct DropRaritySuffix
    {
        public DropRarity Rarity;
        [TextArea(1, 3)] public string Suffix;
    }
}
