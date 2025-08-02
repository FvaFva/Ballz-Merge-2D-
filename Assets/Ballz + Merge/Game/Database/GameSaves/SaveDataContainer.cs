
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveDataContainer
{
    private readonly Dictionary<string, float> _main;

    public readonly List<SavedBlock> Blocks;
    public readonly List<SavedVolume> Volumes;
    public readonly List<SavedBlockEffect> BlockEffects;

    public IEnumerable<KeyValuePair<string, float>> Main => _main;

    public readonly bool IsLoaded;

    public SaveDataContainer()
    {
        _main = new Dictionary<string, float>();
        Blocks = new List<SavedBlock>();
        Volumes = new List<SavedVolume>();
        BlockEffects = new List<SavedBlockEffect>();
        IsLoaded = false;
    }

    public SaveDataContainer(IEnumerable<KeyValuePair<string, float>> main, IEnumerable<SavedBlock> blocks, IEnumerable<SavedVolume> volumes, IEnumerable<SavedBlockEffect> effects)
    {
        _main = main.ToDictionary(k => k.Key, v => v.Value);
        Blocks = new List<SavedBlock>(blocks);
        Volumes = new List<SavedVolume>(volumes);
        BlockEffects = new List<SavedBlockEffect>(effects);
        IsLoaded = true;
    }

    public int GetInt(string key) => Mathf.RoundToInt(Get(key));
    
    public float Get(string key)
    {
        if (_main.TryGetValue(key, out float value))
            return value;
        else
            return default;
    }

    public void Set(string key, float value) => _main[key] = value;
}
