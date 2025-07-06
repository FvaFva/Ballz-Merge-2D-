using System;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using Zenject;

public abstract class BallVolumeOnHit : BallVolume
{
    protected BlocksInGame Blocks { get; private set; }
    protected BallWaveVolume BallWaveVolume { get; private set; }
    protected GridSettings Grid { get; private set; }
    protected DiContainer Container;

    public override int GetValue(DropRarity rarity) => rarity.Weight;
    
    public override string GetDescription(DropRarity rarity) => GetDescription(rarity.Weight);

    public void Init(BlocksInGame blocks, BallWaveVolume ballWaveVolume, GridSettings grid, DiContainer diContainer)
    {
        Blocks = blocks;
        Grid = grid;
        BallWaveVolume = ballWaveVolume;
        Container = diContainer;
        Init();
    }

    public abstract void Explore(BallVolumeHitData data, DropRarity rarity, Action<bool> callback);

    protected virtual void Init() { }
}
