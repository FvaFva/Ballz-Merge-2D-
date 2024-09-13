using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "BallVolumesMap", menuName = "Bellz+Merge/Drop/BallVolumeMap", order = 51)]
public class BallVolumesMap :ScriptableObject
{
    [SerializeField] private List<BallVolume> _ballVolumes = new List<BallVolume>();

    private Dictionary<BallVolumesTypes, BallVolume> _volumesByType;
    private Dictionary<string, BallVolume> _volumesByString;

    public void ReBuild()
    {
        _volumesByType = new Dictionary<BallVolumesTypes, BallVolume>();
        _volumesByString = new Dictionary<string, BallVolume>();

        foreach (BallVolume volume in _ballVolumes)
        {
            if (volume != null)
            {
                _volumesByType.Add(volume.Type, volume);
                _volumesByString.Add(volume.Type.ToString(), volume);
            }
        }
    }

    public string GetTypifiedChance(BallVolumesTypes type, float value)
    {
        return GetTypifiedChance(GetVolume(type), value);
    }

    public string GetTypifiedChance(BallVolume volume, float value)
    {
        if (volume == null || volume.Counting == BallVolumeCountingTypes.Chance)
            return $"{(int)(value * 100)}%";
        else
            return $"{(int)value}";
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
