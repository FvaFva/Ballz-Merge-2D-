using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "BallVolumesMap", menuName = "Bellz+Merge/Drop/BallVolumeMap", order = 51)]
public class BallVolumesMap : ScriptableObject
{
    [SerializeField] private List<BallVolume> _ballVolumes;
    [SerializeField] private List<DropRarity> _rarities;

    private Dictionary<Type, BallVolume> _volumesByType;
    private Dictionary<string, BallVolume> _volumesByName;
    private Dictionary<int, DropRarity> _raritiesWeights;

    public void ReBuild()
    {
        _volumesByType = _ballVolumes.ToDictionary(v => v.GetType(), v => v);
        _volumesByName = _ballVolumes.ToDictionary(v=> v.Name, v => v);
        _raritiesWeights = _rarities.ToDictionary(r => r.Weight, r => r);
    }

    public void InitOnHit(BlocksInGame blocks, BallWaveVolume waveVolume, GridSettings grid, DiContainer sceneContainer)
    {
        foreach (var volume in _ballVolumes)
        {
            if (volume is BallVolumeOnHit hit)
                hit.Init(blocks, waveVolume, grid, sceneContainer);
        }
    }

    public DropRarity GetRarity(int weight)
    {
        if (_raritiesWeights.ContainsKey(weight))
            return _raritiesWeights[weight];

        return null;
    }

    public T GetVolume<T>() where T : BallVolume
    {
        var type = typeof(T);
        return (T)GetVolumeByType(type);
    }

    public BallVolume GetVolumeByType(Type type)
    {
        if (_volumesByType.ContainsKey(type))
            return _volumesByType[type];

        return null;
    }

    public BallVolume GetVolumeByName(string name)
    {
        if (_volumesByName.ContainsKey(name))
            return _volumesByName[name];

        return null;
    }

    public IEnumerable<T> GetAllByType <T>() where T : BallVolume
    {
        return _ballVolumes.Where(v => v is T).Select(v => (T)v);
    }
}
