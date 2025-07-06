using UnityEngine;

public interface IBallVolumeViewData
{
    public string Name { get; }
    public string Description { get; }
    public int Value { get; }
    public Sprite Icon { get; }
    public BallVolume Volume { get; }
    public bool IsEqual<Type>() where Type : BallVolume;
}
