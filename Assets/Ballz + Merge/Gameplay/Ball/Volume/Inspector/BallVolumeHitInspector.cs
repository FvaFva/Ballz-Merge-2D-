using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BallVolumeHitInspector
{
    [Inject] private readonly BlocksInGame _blocks;
    [Inject] private readonly BallWaveVolume _ballLevelVolume;
    [Inject] private readonly GridSettings _grid;

    private readonly BlocksMover _mover;
    private readonly BlockMagneticObserver _magneticObserver;
    private readonly Dictionary<BallVolumesTypes, Action<BallVolumeHitData, DropRarity>> _map;

    [Inject]
    public BallVolumeHitInspector(BlocksMover mover, DiContainer diContainer)
    {
        _map = new Dictionary<BallVolumesTypes, Action<BallVolumeHitData, DropRarity>>();
        _mover = mover;
        _magneticObserver = diContainer.Instantiate<BlockMagneticObserver>();
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
        _map.Add(BallVolumesTypes.Magnet, _magneticObserver.Activate);
    }

    private void Crush(BallVolumeHitData data, DropRarity rarity)
    {
        if(rarity.Weight >= 3)
        {
            data.Block.Destroy();
            return;
        }

        var nextPosition = data.Block.GridPosition + data.Direction;
        bool isOutside = _grid.IsOutside(nextPosition);

        if (isOutside)
            data.Block.Destroy();
        else if(rarity.Weight == 2 && _blocks.HaveAtPosition(nextPosition))
            data.Block.Destroy();
    }

    private void NumberReductor(BallVolumeHitData data, DropRarity rarity)
    {
        data.Block.ChangeNumber(-rarity.Weight);
    }

    private void MoveIncreaser(BallVolumeHitData data, DropRarity rarity)
    {
        for (int i = rarity.Weight; i > 0; i--)
        {
            Vector2Int extraBlockPosition = data.Block.GridPosition + data.Direction * i;
            var extraBlock = _blocks.GetAtPosition(extraBlockPosition);

            if(extraBlock != null)
                _mover.Try(extraBlock, data.Direction);
        }
    }
}
