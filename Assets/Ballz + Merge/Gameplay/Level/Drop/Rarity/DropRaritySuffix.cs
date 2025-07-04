using System;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [Serializable]
    public struct DropRaritySuffix
    {
        public int Weight;
        [TextArea(1, 3)] public string Suffix;
    }
}
