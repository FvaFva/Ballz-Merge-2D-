using System;
using System.Collections.Generic;
using UnityEngine;

public class BallVolumesPassiveView : CyclicBehavior, ILevelFinisher, IInitializable
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private BallVolumesMap _map;
    [SerializeField] private BallWaveVolume _volumes;
    [SerializeField] private BallVolumeView _prefab;

    private readonly Dictionary<Type, BallVolumeView> _volumesMap = new Dictionary<Type, BallVolumeView>();

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
        foreach (var volume in _map.GetAllByType<BallVolumePassive>())
            _volumesMap.Add(volume.GetType(), Instantiate(_prefab, _content).Deactivate());

        gameObject.SetActive(true);
    }

    public void FinishLevel()
    {
        foreach (var view in _volumesMap.Values)
            view.Show(null);
    }

    private void ChangedInBag(IBallVolumesBagCell<BallVolume> ballVolume)
    {
        if (ballVolume.Volume is BallVolumePassive passive)
        {
            var type = ballVolume.Volume.GetType();
            int value = _volumes.GetPassiveValue(type);
            _volumesMap[type].Show(new BallVolumeViewData(passive, value));
        }
    }

    private void UpdateAllValues()
    {
        foreach (var volumeView in _volumesMap)
        {
            var volume = _map.GetVolumeByType(volumeView.Key);
            int value = _volumes.GetPassiveValue(volumeView.Key);
            volumeView.Value.Show(new BallVolumeViewData(volume, value));
        }
    }

    private void OnViewTriggered(BallVolumeView view)
    {
        VolumeActivated?.Invoke(view.CurrentData);
    }
}
