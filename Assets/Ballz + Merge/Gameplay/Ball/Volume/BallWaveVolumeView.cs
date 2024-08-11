using System;
using System.Collections.Generic;
using UnityEngine;

public class BallWaveVolumeView : MonoBehaviour
{
    [SerializeField] private BallWaveVolume _source;
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private InfoPanel _infoPanelPrefab;

    private BallVolumesMap _map;
    private Dictionary<BallVolumesTypes, InfoPanel> _views;

    private void Awake()
    {
        _views = new Dictionary<BallVolumesTypes, InfoPanel>();
        _map = new BallVolumesMap();

        foreach (BallVolumesTypes volume in Enum.GetValues(typeof(BallVolumesTypes)))
            _views.Add(volume, Instantiate(_infoPanelPrefab, _viewPort).Init(0, GetLabelOfVolume(volume)));
    }

    private void OnEnable()
    {
        _source.Updated += OnSourceUpdate;
        _source.Changed += UpdateValue;
    }

    private void OnDisable()
    {
        _source.Updated -= OnSourceUpdate;
        _source.Changed -= UpdateValue;
    }

    private void OnSourceUpdate(IDictionary<BallVolumesTypes, float> valuePairs)
    {
        foreach (var newValue in valuePairs)
            UpdateValue(newValue.Key, newValue.Value);
    }

    private void UpdateValue(BallVolumesTypes type, float value)
    {
        if (value.Equals(0))
            _views[type].Hide();
        else
            _views[type].Show(GetValueOfVolume(type, value));
    }

    private string GetValueOfVolume(BallVolumesTypes type, float value)
    {
        var volume = _map.GetVolume(type);

        if (volume == null || volume.Counting == BallVolumeCountingTypes.Chance)
            return $"{(int)(value * 100)}%";
        else
            return $"{(int)value}";
    }

    private string GetLabelOfVolume(BallVolumesTypes type)
    {
        var volume = _map.GetVolume(type);

        if (volume == null)
            return $"{type}:";
        else
            return $"{volume.Name} {volume.Counting}:";
    }
}