using BallzMerge.Gameplay.Level;
using System;

public struct BallVolumesBagCell
{
    public DropRarity Rarity;
    public BallVolume Volume;
    public string Name;
    public readonly bool IsInited;
    public int Value => IsInited ? Rarity.Weight : 0;
    public Action<bool> ViewCallback;

    public BallVolumesBagCell(BallVolume volume, DropRarity rarity)
    {
        Rarity = rarity;
        Volume = volume;
        IsInited = true;
        Name = volume.Type.ToString();
        ViewCallback = null;
    }

    public bool IsEqual(BallVolumesTypes type)
    {
        return IsInited && Volume.Type.Equals(type);
    }

    public bool IsEqual(BallVolumesSpecies species)
    {
        return IsInited && Volume.Species.Equals(species);
    }

    public override bool Equals(object bagCell)
    {
        if (bagCell is not BallVolumesBagCell other)
            return false;

        return IsInited && Volume == other.Volume && Rarity == other.Rarity;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Volume, Rarity);
    }
}