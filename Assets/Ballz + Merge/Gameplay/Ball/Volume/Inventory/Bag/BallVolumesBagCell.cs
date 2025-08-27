using BallzMerge.Gameplay.Level;
using System;
using UnityEngine;

public class BallVolumesBagCell<T> : IBallVolumesBagCell<T> where T : BallVolume
{
    public BallVolumesBagCell(T volume, DropRarity rarity, int? id = null)
    {
        ID = id != null ? (int)id : 0;
        Rarity = rarity;
        Volume = volume;
        Value = volume.GetValue(rarity);
        Description = Volume.GetDescription(Rarity);
        Name = Volume.Name;
    }

    public int ID { get; private set; }
    public string Name { get; private set; }
    public DropRarity Rarity { get; private set; }
    public T Volume { get; private set; }
    public int Value { get; private set; }
    public Action<bool> ViewCallback { get; private set; }
    public string Description  {get; private set; }
    public Sprite Icon => Volume.Icon;
    public Color RarityColor => Rarity.Color;

    BallVolume IBallVolumeViewData.Volume => Volume;

    public void SetID(int id)
    {
        ID = id;
    }

    public void SetCallback(Action<bool> callback)
    {
        ViewCallback = callback;
    }

    public bool IsEqual<Type>() where Type : BallVolume => Volume is Type;

    public bool IsEqual(Type type) => type.IsInstanceOfType(Volume);
    
}