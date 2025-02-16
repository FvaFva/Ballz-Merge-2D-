using BallzMerge.Gameplay.Level;

public struct BallVolumesBagCell
{
    public DropRarity Rarity;
    public BallVolume Volume;
    public string Name;
    public readonly bool IsInited;

    public BallVolumesBagCell(BallVolume volume, DropRarity rarity)
    {
        Rarity = rarity;
        Volume = volume;
        IsInited = true;
        Name = volume.Type.ToString();
    }

    public bool IsEqual(BallVolumesTypes type)
    {
        return IsInited && Volume.Type.Equals(type);
    }

    public bool IsEqual(BallVolumesSpecies species)
    {
        return IsInited && Volume.Species.Equals(species);
    }

    public bool IsEqual(BallVolumesBagCell bagCell)
    {
        return IsInited && Volume == bagCell.Volume && Rarity == bagCell.Rarity;
    }

    public int Value => IsInited ? Rarity.Weight : 0;
}