using BallzMerge.Gameplay.Level;

public class BallVolumePassive : BallVolume
{
    public override string GetDescription(DropRarity rarity) => GetDescription(rarity.Weight);

    public override int GetValue(DropRarity rarity) => rarity.Weight;
}
