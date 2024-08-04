using System;
using System.Collections.Generic;
using UnityEngine;

public class BallWaveVolumeView : MonoBehaviour
{
    [SerializeField] private BallWaveVolume _source;
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private InfoPanel _infoPanelPrefab;

    private Dictionary<BallVolumesTypes, InfoPanel> _views;

    private void Awake()
    {
        _views = new Dictionary<BallVolumesTypes, InfoPanel>();

        foreach (BallVolumesTypes volume in Enum.GetValues(typeof(BallVolumesTypes)))
            _views.Add(volume, Instantiate(_infoPanelPrefab, _viewPort).Init(0, volume.ToString()));
    }

    private void OnEnable()
    {
        _source.Updated += OnSourceUpdate;
        _source.Changed += OnSourceChanged;
    }

    private void OnDisable()
    {
        _source.Updated -= OnSourceUpdate;
        _source.Changed -= OnSourceChanged;
    }

    private void OnSourceUpdate(IDictionary<BallVolumesTypes, float> valuePairs)
    {
        foreach (var newValue in valuePairs)
            _views[newValue.Key].Show(newValue.Value * 100);
    }

    private void OnSourceChanged(BallVolumesTypes type, float value) => _views[type].Show(value);
}