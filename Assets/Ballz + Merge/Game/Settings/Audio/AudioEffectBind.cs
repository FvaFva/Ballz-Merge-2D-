using System;
using UnityEngine;

namespace BallzMerge.Root.Audio
{
    [Serializable]
    public struct AudioEffectBind
    {
        public AudioEffectsTypes Type;
        public AudioClip Effect;
    }
}
