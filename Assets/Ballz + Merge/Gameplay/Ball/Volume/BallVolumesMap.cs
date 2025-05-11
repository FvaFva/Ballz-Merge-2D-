using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallVolumesMap", menuName = "Bellz+Merge/Drop/BallVolumeMap", order = 51)]
public class BallVolumesMap : ScriptableObject
{
    [SerializeField] private List<BallVolume> _ballVolumes;
    [SerializeField] private List<DropRarity> _rarities;

    private Dictionary<BallVolumesTypes, BallVolume> _volumesByType;
    private Dictionary<string, BallVolume> _volumesByString;
    private Dictionary<int, DropRarity> _raritiesWeights;

    public void ReBuild()
    {
        _volumesByType = new Dictionary<BallVolumesTypes, BallVolume>();
        _volumesByString = new Dictionary<string, BallVolume>();
        _raritiesWeights = new Dictionary<int, DropRarity>();

        foreach (BallVolume volume in _ballVolumes)
        {
            if (volume != null)
            {
                _volumesByType.Add(volume.Type, volume);
                _volumesByString.Add(volume.Type.ToString(), volume);
            }
        }

        foreach(DropRarity rarity in _rarities)
        {
            if (rarity != null)
                _raritiesWeights.Add(rarity.Weight, rarity);
        }
    }

    public DropRarity GetRarity(int weight)
    {
        if (_raritiesWeights.ContainsKey(weight))
            return _raritiesWeights[weight];

        return null;
    }

    public BallVolume GetVolume(BallVolumesTypes type)
    {
        if (_volumesByType.ContainsKey(type))
            return _volumesByType[type];
        return null;
    }

    public BallVolume GetVolume(string typeName)
    {
        if (_volumesByString.ContainsKey(typeName))
            return _volumesByString[typeName];
        return null;
    }
}
