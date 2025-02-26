using BallzMerge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameHistoryView : CyclicBehavior, IInitializable, IInfoPanelView
{
    private const int CountPreload = 20;

    [SerializeField] private List<ButtonToggle> _toggles;
    [SerializeField] private GameDataView _gameDataPrefab;
    [SerializeField] private RectTransform _dataParent;

    private Dictionary<ButtonToggle, Action<ButtonToggle>> _togglesDictionary = new();
    private string[] _toggleLabels = { "ID", "Date" };
    private string[] _symbols = { "(↑)", "(↓)" };
    private int _index = 0;
    private bool _isScoreAscending = true;
    private bool _isNumberAscending = true;
    private ButtonToggle _currentToggle;
    private List<GameHistoryData> _data;
    private List<GameDataView> _allViews = new List<GameDataView>();
    private RectTransform _rootParent;
    private RectTransform _transform;

    public void Show(RectTransform showcase)
    {
        foreach (var toggle in _togglesDictionary)
            toggle.Key.Initialize(_symbols[0], _symbols[1], toggle.Value);

        gameObject.SetActive(true);
        _transform.SetParent(showcase, false);
    }

    public bool SetData(List<GameHistoryData> data)
    {
        if (data == null || data.Count == 0)
            return false;

        _data = data;
        _togglesDictionary.Add(_toggles[0], ChangeStateView);
        _togglesDictionary.Add(_toggles[1], OrderNumber);
        _togglesDictionary.Add(_toggles[2], OrderScore);

        if (_data.Count > _allViews.Count)
            GenerateViews(_data.Count - _allViews.Count);

        Show();

        return true;
    }

    public void Hide()
    {
        foreach (var view in _allViews)
            view.Hide();

        _transform.SetParent(_rootParent, false);
        gameObject.SetActive(false);

        foreach (var toggle in _togglesDictionary)
            toggle.Key.Close();
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
            _allViews[i].Show(GetData(_data[i]), _data[i].Score, _data[i].Number, _data[i].Volumes);
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_gameDataPrefab, _dataParent).Init());
    }

    private void ChangeStateView(ButtonToggle buttonToggle)
    {
        _index = (_index + 1) % _toggleLabels.Length;
        //_toggleLabel.text = _toggleLabels[_index];

        if (_currentToggle != null)
            _currentToggle.ResetLabel();

        _currentToggle = buttonToggle;
        Show();
    }

    private void OrderScore(ButtonToggle buttonToggle)
    {
        ToggleSort(ref _isScoreAscending, x => x.Score);
        SetNewLabel(buttonToggle);
    }

    private void OrderNumber(ButtonToggle buttonToggle)
    {
        ToggleSort(ref _isNumberAscending, x => x.Number);
        SetNewLabel(buttonToggle);
    }

    private void ToggleSort<T>(ref bool ascending, Func<GameHistoryData, T> keySelector)
    {
        _data = ascending ? _data.OrderBy(keySelector).ToList() : _data.OrderByDescending(keySelector).ToList();
        ascending = !ascending;
        Show();
    }

    private void SetNewLabel(ButtonToggle buttonToggle)
    {
        if (_currentToggle != null && _currentToggle != buttonToggle)
            _currentToggle.ResetLabel();

        _currentToggle = buttonToggle;
    }

    private string GetData(GameHistoryData data)
    {
        return _index == 0 ? data.ID : data.Date;
    }
}