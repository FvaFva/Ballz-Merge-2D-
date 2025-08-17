using System;
using System.Collections.Generic;
using BallzMerge.Data;
using UnityEngine;
using Zenject;

public class LevelSelector : CyclicBehavior, IInfoPanelView, IInitializable
{
    [SerializeField] private RectTransform _box;
    [SerializeField] private LevelSelectionView _prefab;
    [SerializeField] private LevelView _level;

    [Inject] private LevelSettingsMap _map;
    [Inject] private LevelSettingsContainer _container;
    [Inject] private DataBaseSource _data;

    private List<LevelSelectionView> _selectors;
    private RectTransform _parent;
    private RectTransform _transform;

    public event Action Selected;

    private void OnEnable()
    {
        _level.Chose += OnChose;

        foreach (var selector in _selectors)
            selector.Selected += OnSelect;
    }

    private void OnDisable()
    {
        _level.Chose -= OnChose;

        foreach (var selector in _selectors)
            selector.Selected -= OnSelect;
    }

    public void Init()
    {
        _transform = transform as RectTransform;
        _parent = _transform.parent as RectTransform;
        _selectors = new List<LevelSelectionView>();
        var completedLevels = _data.History.GetCompleted();

        foreach (var level in _map.Available)
            _selectors.Add(Instantiate(_prefab, _box).Show(level, completedLevels.Contains(level.ID)));
    }

    public void Show(RectTransform showcase)
    {
        _transform.SetParent(showcase);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _transform.SetParent(_parent);
    }

    private void OnSelect(LevelSettings level)
    {
        _level.Show(level);
    }

    private void OnChose(LevelSettings level)
    {
        _container.Change(level);
        Selected?.Invoke();
    }
}
