using System;
using System.Collections.Generic;
using UnityEngine;

public class BallVolumesPassiveView : CyclicBehavior, ILevelFinisher, IInitializable
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private BallVolumesMap _map;
    [SerializeField] private BallWaveVolume _volumes;
    [SerializeField] private BallVolumeView _prefab;

    private readonly Dictionary<BallVolumesTypes, BallVolumeView> _volumesMap = new Dictionary<BallVolumesTypes, BallVolumeView>();

    public event Action<IBallVolumeViewData> VolumeActivated;

    private void OnEnable()
    {
        _volumes.Bag.Added += ChangedInBag;
        _volumes.Bag.Removed += ChangedInBag;
        _volumes.Bag.Loaded += ChangedInBag;

        foreach (var view in _volumesMap.Values)
            view.Triggered += OnViewTriggered;

        UpdateAllValues();
    }

    private void OnDisable()
    {
        _volumes.Bag.Added -= ChangedInBag;
        _volumes.Bag.Removed -= ChangedInBag;
        _volumes.Bag.Loaded -= ChangedInBag;

        foreach (var view in _volumesMap.Values)
            view.Triggered -= OnViewTriggered;
    }

    public void Init()
    {
        foreach (var volume in _map.GetBySpecies(BallVolumesSpecies.Passive))
            _volumesMap.Add(volume.Type, Instantiate(_prefab, _content).Deactivate());

        gameObject.SetActive(true);
    }

    public void FinishLevel()
    {
        foreach (var view in _volumesMap.Values)
            view.Show(null);
    }

    private void ChangedInBag(BallVolumesBagCell ballVolume)
    {
        if (ballVolume.Volume.Species == BallVolumesSpecies.Passive)
        {
            int value = _volumes.GetPassiveValue(ballVolume.Volume.Type);
            _volumesMap[ballVolume.Volume.Type].Show(new BallVolumeViewData(ballVolume, value));
        }
    }

    private void UpdateAllValues()
    {
        foreach (var volumeView in _volumesMap)
        {
            var volume = _map.GetVolume(volumeView.Key);
            int value = _volumes.GetPassiveValue(volumeView.Key);
            volumeView.Value.Show(new BallVolumeViewData(volume, value));
        }
    }

    private void OnViewTriggered(BallVolumeView view)
    {
        VolumeActivated?.Invoke(view.CurrentData);
    }
}
