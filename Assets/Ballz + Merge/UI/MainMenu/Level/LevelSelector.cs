using System;
using System.Collections.Generic;
using BallzMerge.Data;
using UnityEngine;
using Zenject;

public class LevelSelector : DependentColorUI, IInfoPanelView, IInitializable, IDependentScreenOrientation
{
    [SerializeField] private RectTransform _box;
    [SerializeField] private LevelSelectionView _prefab;
    [SerializeField] private LevelView _level;
    [SerializeField] private AdaptiveLayoutGroupStretching _body;
    [SerializeField] private List<DependentColorUI> dependentColorUIs;

    [Inject] private LevelSettingsMap _map;
    [Inject] private LevelSettingsContainer _container;
    [Inject] private DataBaseSource _data;

    private List<LevelSelectionView> _selectors;
    private List<int> _completedLevels;
    private RectTransform _parent;
    private RectTransform _transform;

    public event Action Selected;

    private void OnEnable()
    {
        _level.Chose += OnChose;

        foreach (var selector in _selectors)
            selector.Selected += OnSelect;

        _completedLevels = _data.History.GetCompleted();
        int count = 0;

        foreach (var level in _map.Available)
            _selectors[count++].ShowStatus(_completedLevels.Contains(level.ID));
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

        foreach (var level in _map.Available)
            _selectors.Add(Instantiate(_prefab, _box).Init(level));
    }

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;

        foreach (var selector in _selectors)
            selector.ApplyColors(GameColors);

        foreach (var dependentColorUI in dependentColorUIs)
            dependentColorUI.ApplyColors(GameColors);

        _level.ApplyColors(GameColors);
    }

    public void UpdateScreenOrientation(bool isVertical)
    {
        _body.UpdateScreenOrientation(isVertical);
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
