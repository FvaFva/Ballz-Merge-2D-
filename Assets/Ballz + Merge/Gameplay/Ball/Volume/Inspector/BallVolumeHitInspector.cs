using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BallVolumeHitInspector
{
    Func<Block, Vector2Int, float, bool> _mover;
    private readonly BlocksInGame _blocks;
    private readonly BallWaveVolume _ballLevelVolume;
    private readonly Dictionary<BallVolumesTypes, Action<BallVolumeHitData, DropRarity>> _map;

    public BallVolumeHitInspector(BlocksInGame blocks, BallWaveVolume ballLevelVolume, Func<Block, Vector2Int, float, bool> mover)
    {
        _blocks = blocks;
        _ballLevelVolume = ballLevelVolume;
        _map = new Dictionary<BallVolumesTypes, Action<BallVolumeHitData, DropRarity>>();
        _mover = mover;
        Bind();
    }

    public void Explore(BallVolumeHitData data)
    {
        var nextVolume = _ballLevelVolume.GetCageValue();

        if (nextVolume.IsInited)
            _map[nextVolume.Volume.Type].Invoke(data, nextVolume.Rarity);
    }

    private void Bind()
    {
        _map.Add(BallVolumesTypes.Crush, Crush);
        _map.Add(BallVolumesTypes.NumberReductor, NumberReductor);
        _map.Add(BallVolumesTypes.MoveIncreaser, MoveIncreaser);
        _map.Add(BallVolumesTypes.Magnet, Magnet);
    }

    private void Crush(BallVolumeHitData data, DropRarity rarity)
    {
        data.Block.Destroy();
    }

    private void NumberReductor(BallVolumeHitData data, DropRarity rarity)
    {
        data.Block.ChangeNumber(-rarity.Weight);
    }

    private void MoveIncreaser(BallVolumeHitData data, DropRarity rarity)
    {
        _mover?.Invoke(data.Block, data.Direction, rarity.Weight + 1);
    }

    private void Magnet(BallVolumeHitData data, DropRarity rarity)
    {

    }
}
