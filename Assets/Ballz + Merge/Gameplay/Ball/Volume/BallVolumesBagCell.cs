using BallzMerge.Gameplay.Level;
using System;

public class BallVolumesBagCell
{
    public int ID { get; private set; }
    public string Name { get; private set; }
    public DropRarity Rarity { get; private set; }
    public BallVolume Volume { get; private set; }
    public int Value => Rarity.Weight;
    public Action<bool> ViewCallback { get; private set; }

    public BallVolumesBagCell(BallVolume volume, DropRarity rarity, int? id = null)
    {
        ID = id != null ? (int)id : 0;
        Rarity = rarity;
        Volume = volume;
        Name = volume.Type.ToString();
    }

    public void SetID(int id)
    {
        ID = id;
    }

    public void SetCallback(Action<bool> callback)
    {
        ViewCallback = callback;
    }

    public bool IsEqual(BallVolumesTypes type)
    {
        return Volume.Type.Equals(type);
    }
}