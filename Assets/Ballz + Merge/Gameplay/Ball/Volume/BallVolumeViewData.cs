using UnityEngine;

public struct BallVolumeViewData : IBallVolumeViewData
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public int Value { get; private set; }

    public Sprite Icon { get; private set; }

    public BallVolumeViewData(BallVolumesBagCell bag, int value)
    {
        Name = bag.Name;
        Description = bag.Volume.GetDescription(value);
        Value = value;
        Icon = bag.Volume.Icon;
    }
    public BallVolumeViewData(BallVolume volume, int value)
    {
        Name = volume.Name;
        Description = volume.GetDescription(value);
        Value = value;
        Icon = volume.Icon;
    }
}
