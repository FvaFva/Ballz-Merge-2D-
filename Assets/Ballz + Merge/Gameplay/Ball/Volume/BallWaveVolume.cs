using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BallWaveVolume : CyclicBehavior, IWaveUpdater, IInitializable, ILevelFinisher
{
    [SerializeField] private DropSelector _dropSelector;

    private Dictionary<BallVolumesTypes, float> _volumes;

    public BallGlobalVolume GlobalVolumes {  get; private set; }
    public IDictionary<BallVolumesTypes, float> Volumes => _volumes;
    public event Action<IDictionary<BallVolumesTypes, float>> Updated;
    public event Action<BallVolumesTypes, float> Changed;

    public void Init()
    {
        GlobalVolumes = new BallGlobalVolume(_dropSelector);
        GlobalVolumes.Changed += UpdateWave;
        _volumes = new Dictionary<BallVolumesTypes, float>();

        foreach (var volume in GlobalVolumes.Volumes)
            _volumes.Add(volume.Key, volume.Value);

        Updated?.Invoke(_volumes);
    }

    public void UpdateWave()
    {
        foreach (var volume in GlobalVolumes.Volumes)
            _volumes[volume.Key] = volume.Value;

        Updated?.Invoke(_volumes);
    }

    public float GetMaxVolume(BallVolumesTypes ballVolume) => GlobalVolumes.Volumes[ballVolume];

    public bool CheckVolume(BallVolumesTypes ballVolume)
    {
        float volume = _volumes[ballVolume];

        if(volume ==  0)
            return false;
        
        if(volume >= 1)
        {
            _volumes[ballVolume]--;
            Changed?.Invoke(ballVolume, _volumes[ballVolume]);
            return true;
        }

        float dice = UnityEngine.Random.Range(0f, 1f);

        if(volume >= dice)
        {
            _volumes[ballVolume] = 0;
            Changed?.Invoke(ballVolume, _volumes[ballVolume]);
            return true;
        }

        return false;
    }

    public void FinishLevel()
    {
        GlobalVolumes.Changed -= UpdateWave;
        Init();
    }
}
