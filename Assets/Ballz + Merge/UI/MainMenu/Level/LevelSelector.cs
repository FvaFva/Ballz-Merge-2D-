using System;
using System.Collections.Generic;
using System.Linq;
using BallzMerge.Data;
using UnityEngine;
using Zenject;

public class LevelSelector : DependentColorUI, IInfoPanelView, IInitializable, IDependentScreenOrientation
{
    [SerializeField] private RectTransform _box;
    [SerializeField] private RectTransform _difficultBox;
    [SerializeField] private LevelSelectionView _prefab;
    [SerializeField] private LevelDifficultView _difficultPrefab;
    [SerializeField] private LevelView _level;
    [SerializeField] private AdaptiveLayoutGroupStretching _body;
    [SerializeField] private List<DependentColorUI> _dependentColorUIs;

    [Inject] private LevelSettingsMap _map;
    [Inject] private LevelSettingsContainer _container;
    [Inject] private DataBaseSource _data;

    private List<LevelSelectionView> _selectors;
    private List<LevelDifficultView> _difficultViews;
    private List<int> _completedLevels;
    private RectTransform _parent;
    private RectTransform _transform;
    private LevelDifficultView _currentDifficult;

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

        foreach (var difficultView in _difficultViews)
            difficultView.Selected += OnDifficultSelected;
    }

    private void OnDisable()
    {
        _level.Chose -= OnChose;

        foreach (var selector in _selectors)
            selector.Selected -= OnSelect;

        foreach (var difficultView in _difficultViews)
            difficultView.Selected -= OnDifficultSelected;
    }

    public void Init()
    {
        _transform = transform as RectTransform;
        _parent = _transform.parent as RectTransform;
        _selectors = new List<LevelSelectionView>();
        _difficultViews = new List<LevelDifficultView>();

        foreach (var level in _map.Available)
            _selectors.Add(Instantiate(_prefab, _box).Init(level));

        List<LevelDifficult> difficultyLevels = _map.Available.Select(level => level.Difficult).Distinct().ToList();

        foreach (var difficult in difficultyLevels)
            _difficultViews.Add(Instantiate(_difficultPrefab, _difficultBox).Init(difficult));
    }

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;

        foreach (var selector in _selectors)
            selector.ApplyColors(GameColors);

        foreach (var difficultView in _difficultViews)
            difficultView.ApplyColors(GameColors);

        foreach (var dependentColorUI in _dependentColorUIs)
            dependentColorUI.ApplyColors(GameColors);

        OnDifficultSelected(_difficultViews[0]);
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

    private void OnDifficultSelected(LevelDifficultView difficultView)
    {
        _currentDifficult?.Unselect();
        _currentDifficult = difficultView;
        _currentDifficult.Select();
        foreach (var selector in _selectors)
        {
            bool isActive = selector.Data.Difficult == difficultView.LevelDifficult;
            selector.SetState(isActive);
        }
    }
}
