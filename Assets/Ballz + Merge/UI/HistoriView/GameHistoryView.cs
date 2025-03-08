using BallzMerge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHistoryView : CyclicBehavior, IInitializable, IInfoPanelView
{
    private const int CountPreload = 20;

    [SerializeField] private ButtonToggle _dateID;
    [SerializeField] private ButtonToggle _score;
    [SerializeField] private ButtonToggle _number;
    [SerializeField] private GameDataView _gameDataPrefab;
    [SerializeField] private RectTransform _dataParent;

    private readonly List<ButtonToggle> _toggles = new List<ButtonToggle>();
    private readonly string[] _toggleLabels = { "ID", "Date", "(↑)", "(↓)" };
    private readonly List<GameDataView> _allViews = new List<GameDataView>();

    private ButtonToggle _currentToggle;
    private List<GameHistoryData> _data;
    private RectTransform _rootParent;
    private RectTransform _transform;

    public void Show(RectTransform showcase)
    {
        gameObject.SetActive(true);
        _transform.SetParent(showcase, false);
    }

    public bool SetData(List<GameHistoryData> data)
    {
        if (data == null || data.Count == 0)
            return false;

        _data = data;
        _toggles.Add(_dateID.Initialize(_toggleLabels[0], _toggleLabels[1], ChangeStateView));
        _toggles.Add(_score.Initialize(_toggleLabels[2], _toggleLabels[3], OrderScore));
        _toggles.Add(_number.Initialize(_toggleLabels[2], _toggleLabels[3], OrderNumber));

        if (_data.Count > _allViews.Count)
            GenerateViews(_data.Count - _allViews.Count);

        _dateID.ChangeState();

        return true;
    }

    public void Hide()
    {
        foreach (var view in _allViews)
            view.Hide();

        _transform.SetParent(_rootParent, false);
        gameObject.SetActive(false);

        foreach (var toggle in _toggles)
            toggle.Close();
    }

    public void Init()
    {
        _transform = (RectTransform)transform;
        _rootParent = (RectTransform)_transform.parent;
        GenerateViews(CountPreload);
        Hide();
    }

    private void Show()
    {
        for (int i = 0; i < _data.Count; i++)
            _allViews[i].Show(_data[i].GetDateOrID(_dateID.State), _data[i].Score, _data[i].Number, _data[i].Volumes);
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_gameDataPrefab, _dataParent).Init());
    }

    private void ChangeStateView(ButtonToggle _) => Show();

    private void OrderScore(ButtonToggle toggle) => ToggleSort(toggle, x => x.Score);

    private void OrderNumber(ButtonToggle toggle) => ToggleSort(toggle, x => x.Number);

    private void ToggleSort<T>(ButtonToggle toggle, Func<GameHistoryData, T> keySelector)
    {
        _data = toggle.State ? _data.OrderBy(keySelector).ToList() : _data.OrderByDescending(keySelector).ToList();
        Show();
        ResetCurrentToggleLabel(toggle);
    }

    private void ResetCurrentToggleLabel(ButtonToggle toggle)
    {
        if (_currentToggle != null && _currentToggle != toggle)
            _currentToggle.ResetLabel();

        _currentToggle = toggle;
    }
}