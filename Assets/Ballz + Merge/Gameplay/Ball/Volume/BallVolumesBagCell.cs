using BallzMerge.Gameplay.Level;

public struct BallVolumesBagCell
{
    public DropRarity Rarity;
    public BallVolume Volume;
    public string Name;

    public BallVolumesBagCell(BallVolume volume, DropRarity rarity)
    {
        Rarity = rarity;
        Volume = volume;
        Name = volume.Type.ToString();
    }

    public bool IsEqual(BallVolumesTypes type)
    {
        return Volume.Type.Equals(type);
    }

    public bool IsEqual(BallVolumesSpecies species)
    {
        return Volume.Species.Equals(species);
    }

    public bool IsEqual(BallVolumesBagCell bagCell)
    {
        return Volume == bagCell.Volume && Rarity == bagCell.Rarity;
    }

    public bool IsEmpty()
    {
        return Rarity == null || Volume == null;
    }

    public int Value => IsEmpty() ? 0 : Rarity.Weight;
}