using System;
using System.Collections.Generic;
using System.Linq;
using BallzMerge.Gameplay.Level;
using UnityEngine;

[Serializable]
public class DropSettings
{
    [SerializeField] private List<DropEntry> _entries;

    private Dictionary<DropRarity, List<BallVolume>> _dropMap;

    public DropSettings()
    {
        _entries = new List<DropEntry>();
        _dropMap = new Dictionary<DropRarity, List<BallVolume>>();
    }

    public DropSettings(DropSettings source)
    {
        _entries = source._entries.ToList();
        _dropMap = new Dictionary<DropRarity, List<BallVolume>>();

        foreach (var drop in source._dropMap)
            _dropMap.Add(drop.Key, drop.Value.ToList());
    }

    public List<Drop> GetPool()
    {
        var pool = new List<Drop>();

        foreach (var drop in _entries)
        {
            foreach (BallVolume volume in drop.Volumes)
            {
                for (int i = 0; i < drop.Rarity.CountInPool; i++)
                    pool.Add(new Drop(volume, drop.Rarity));
            }
        }

        return pool;
    }

    public void OnAfterDeserialize()
    {
        _dropMap = new Dictionary<DropRarity, List<BallVolume>>();
        foreach (var e in _entries)
            _dropMap[e.Rarity] = e.Volumes ?? new List<BallVolume>();
    }

    public void OnBeforeSerialize()
    {
        _entries.Clear();
        foreach (var kv in _dropMap)
            _entries.Add(new DropEntry {Rarity  = kv.Key, Volumes = kv.Value ?? new List<BallVolume>()});
    }

    public Dictionary<DropRarity, List<BallVolume>> DropMap
    {
        get
        {
            if (_dropMap == null) OnAfterDeserialize();
            return _dropMap;
        }
    }
}
