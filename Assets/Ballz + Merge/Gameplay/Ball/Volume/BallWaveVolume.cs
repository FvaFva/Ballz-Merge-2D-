using System.Collections.Generic;
using UnityEngine;

public class BallWaveVolume : CyclicBehaviour, IWaveUpdater
{
    [SerializeField] private DropSelector _dropSelector;

    private BallGlobalVolume _globalVolumes;
    private Dictionary<BallVolumesTypes, float> _volumes;

    public override void Init()
    {
        _globalVolumes = new BallGlobalVolume(_dropSelector);
        _globalVolumes.Changed += UpdateWave;
        _volumes = new Dictionary<BallVolumesTypes, float>();

        foreach (var volume in _globalVolumes.Volumes)
            _volumes.Add(volume.Key, volume.Value);
    }

    public void UpdateWave()
    {
        foreach (var volume in _globalVolumes.Volumes)
            _volumes[volume.Key] = volume.Value;
    }

    public bool CheckValue(BallVolumesTypes ballVolume)
    {
        float volume = _volumes[ballVolume];

        if(volume ==  0)
            return false;
        
        if(volume >= 1)
        {
            _volumes[ballVolume]--;
            return true;
        }

        float dice = Random.Range(0f, 1f);

        if(volume >= dice)
        {
            _volumes[ballVolume] = 0;
            return true;
        }

        return false;
    }
}
