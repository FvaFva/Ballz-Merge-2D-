using BallzMerge.Data;
using BallzMerge.Root;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameHistoryView : CyclicBehavior, IInitializable, IInfoPanelView
{
    private const int CountOfIteration = 3;

    [SerializeField] private ButtonToggle _dateID;
    [SerializeField] private ButtonToggle _score;
    [SerializeField] private ButtonToggle _number;
    [SerializeField] private ButtonToggle _level;
    [SerializeField] private GameDataView _gameDataPrefab;
    [SerializeField] private RectTransform _dataParent;
    [SerializeField] private UIRootContainerItem _eraseButtonItem;
    [SerializeField] private Button _eraseButton;

    private readonly List<ButtonToggle> _toggles = new List<ButtonToggle>();
    private readonly string[] _toggleLabels = { "ID", "Date", "(↑)", "(↓)" };
    private readonly List<GameDataView> _allViews = new List<GameDataView>();

    private LoadScreen _loadScreen;
    private ButtonToggle _currentToggle;
    private List<GameHistoryData> _data;
    private RectTransform _rootParent;
    private RectTransform _transform;
    private UnityAction _action = () => { };

    public void Show(RectTransform showcase)
    {
        gameObject.SetActive(true);
        _eraseButton.gameObject.SetActive(true);
        _eraseButton.onClick.AddListener(_action);
        _loadScreen.Show();
        _transform.SetParent(showcase, false);
        StartCoroutine(Show());
    }

    public bool SetData(List<GameHistoryData> data, LoadScreen loadScreen, UnityAction action)
    {
        if (data == null || data.Count == 0)
            return false;

        _data = data;
        _loadScreen = loadScreen;
        _action = action;
        _toggles.Add(_dateID.Initialize(_toggleLabels[0], _toggleLabels[1]));
        _toggles.Add(_score.Initialize(_toggleLabels[2], _toggleLabels[3]));
        _toggles.Add(_number.Initialize(_toggleLabels[2], _toggleLabels[3]));
        _toggles.Add(_level.Initialize(_toggleLabels[2], _toggleLabels[3]));
        _dateID.ChangeState();
        _dateID.SetTrigger(ChangeStateView);
        _score.SetTrigger(OrderScore);
        _number.SetTrigger(OrderNumber);
        _level.SetTrigger(OrderLevel);

        if (_data.Count > _allViews.Count)
            GenerateViews(_data.Count - _allViews.Count);

        return true;
    }

    public void Hide()
    {
        foreach (var view in _allViews)
            view.Hide();

        _transform.SetParent(_rootParent, false);
        _eraseButton.onClick.RemoveListener(_action);
        _eraseButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Init()
    {
        _transform = (RectTransform)transform;
        _rootParent = (RectTransform)_transform.parent;
        Hide();
    }

    private IEnumerator Show()
    {
        foreach (var view in _allViews)
            view.Hide();

        _loadScreen.MoveProgress(0f);
        _loadScreen.Show();

        int total = _data.Count;
        double batchSize = Math.Ceiling((double)total / CountOfIteration);

        for (int i = 0; i < total; i++)
        {
            _allViews[i].Show(_data[i].GetDateOrID(_dateID.State),
                _data[i].Score,
                _data[i].Number,
                _data[i].Level,
                _data[i].Volumes,
                _data[i].IsCompleted);

            if (i % batchSize == 0 || i == total - 1)
            {
                float progress = (float)(i + 1) / total;
                _loadScreen.MoveProgress(progress);
                yield return null;
            }
        }

        _loadScreen.Hide();
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_gameDataPrefab, _dataParent).Init());
    }

    private void ChangeStateView(ButtonToggle _) => StartCoroutine(Show());

    private void OrderScore(ButtonToggle toggle) => ToggleSort(toggle, x => x.Score);

    private void OrderNumber(ButtonToggle toggle) => ToggleSort(toggle, x => x.Number);

    private void OrderLevel(ButtonToggle toggle) => ToggleSort(toggle, x => x.Level);

    private void ToggleSort<T>(ButtonToggle toggle, Func<GameHistoryData, T> keySelector)
    {
        _data = toggle.State ? _data.OrderBy(keySelector).ToList() : _data.OrderByDescending(keySelector).ToList();
        StartCoroutine(Show());
        SetNewToggleLabel(toggle);
    }

    private void SetNewToggleLabel(ButtonToggle toggle)
    {
        if (_currentToggle != null && _currentToggle != toggle)
            _currentToggle.ResetLabel();

        _currentToggle = toggle;
    }
}