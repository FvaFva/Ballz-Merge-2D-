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
            _views.Add(volume, Instantiate(_infoPanelPrefab, _viewPort).Init(0, $"{volume} chance:"));
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
            _views[type].Show($"{(int)(value * 100)}%");
    }
}