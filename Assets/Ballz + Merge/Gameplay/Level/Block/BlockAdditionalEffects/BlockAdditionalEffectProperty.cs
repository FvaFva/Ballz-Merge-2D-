using System;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    [Serializable]
    public struct BlockAdditionalEffectProperty
    {
        public BlockAdditionalEffectBase Prefab;
        [Range(0, 1)] public float ChanceToPerform;
    }
}
