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

    public bool IsEqual(BallVolumesBagCell bagCell)
    {
        return IsInited && Volume == bagCell.Volume && Rarity == bagCell.Rarity;
    }
}