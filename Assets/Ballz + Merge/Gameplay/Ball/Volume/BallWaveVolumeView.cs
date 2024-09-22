using System;
using System.Collections.Generic;
using UnityEngine;

public class BallWaveVolumeView : MonoBehaviour
{
    [SerializeField] private bool _isLoadGlobalVolumesOnEnable;
    [SerializeField] private BallWaveVolume _source;
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private GameDataVolumeMicView _viewPrefab;

    private Dictionary<BallVolumesTypes, GameDataVolumeMicView> _views;

    private void Awake()
    {
        _views = new Dictionary<BallVolumesTypes, GameDataVolumeMicView>();

        foreach (BallVolumesTypes volume in Enum.GetValues(typeof(BallVolumesTypes)))
            _views.Add(volume, Instantiate(_viewPrefab, _viewPort).Init().Hide());
    }

    private void OnEnable()
    {
        _source.Updated += OnSourceUpdate;
        _source.Changed += UpdateValue;

        if(_isLoadGlobalVolumesOnEnable)
            OnSourceUpdate(_source.GlobalVolumes.Volumes);
        else
            OnSourceUpdate(_source.Volumes);
    }

    private void OnDisable()
    {
        _source.Updated -= OnSourceUpdate;
        _source.Changed -= UpdateValue;
    }

    private void OnSourceUpdate(IDictionary<BallVolumesTypes, float> valuePairs)
    {
        if (valuePairs == null) 
            return;

        foreach (var newValue in valuePairs)
            UpdateValue(newValue.Key, newValue.Value);
    }

    private void UpdateValue(BallVolumesTypes type, float value)
    {
        if (value.Equals(0))
            _views[type].Hide();
        else
            _views[type].Show(type, value);
    }
}