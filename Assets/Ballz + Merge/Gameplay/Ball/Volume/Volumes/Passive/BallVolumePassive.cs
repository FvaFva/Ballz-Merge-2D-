using BallzMerge.Gameplay.Level;

public class BallVolumePassive : BallVolume
{
    private const int DefaultWeight = 1;

    public override string GetDescription(DropRarity _) => GetDescription(DefaultWeight);

    public override int GetValue(DropRarity rarity) => DefaultWeight;
}
