using System;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public struct Drop
    {
        public Drop (BallVolume volume, DropRarity rarity)
        {
            Volume = volume;
            Rarity = rarity;
        }

        public BallVolume Volume;
        public DropRarity Rarity;
        public readonly Sprite Icon => Volume.Icon;
        public readonly string Name => Volume.Name;
        public readonly string Description => Volume.GetDescription(Rarity);
        public readonly Color Color => Rarity.Color;
        public readonly string Suffix => Volume.GetSuffix(Rarity);
        public readonly bool IsEmpty => Volume == null || Rarity == null;
    }
}