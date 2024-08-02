using System;
using System.Collections.Generic;

public class BallGlobalVolume
{
    private DropSelector _dropSelector;
    private Dictionary<BallVolumesTypes, float> _volumes;

    public BallGlobalVolume(DropSelector dropSelector)
    {
        _volumes = new Dictionary<BallVolumesTypes, float>();

        foreach(BallVolumesTypes volume in Enum.GetValues(typeof(BallVolumesTypes)))
            _volumes.Add(volume, 0);

        _dropSelector = dropSelector;
        _dropSelector.DropSelected += ApplyVolume;
    }

    public IDictionary<BallVolumesTypes, float> Volumes => _volumes;
    public event Action Changed;

    private void ApplyVolume(BallVolumesTypes volume, float count)
    {
        _volumes[volume] += count;
        Changed?.Invoke();
    }
}