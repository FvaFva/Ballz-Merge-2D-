using UnityEngine;

public struct BallVolumeViewData : IBallVolumeViewData
{
    public BallVolumeViewData(BallVolume volume, int value)
    {
        Name = volume.Name;
        Description = volume.GetDescription(value);
        Value = value;
        Icon = volume.Icon;
        RarityColor = default;
        Volume = volume;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int Value { get; private set; }

    public Sprite Icon { get; private set; }

    public BallVolume Volume { get; private set; }

    public Color RarityColor { get; private set; }

    public bool IsEqual<Type>() where Type : BallVolume
    {
        return Volume is Type;
    }
}
