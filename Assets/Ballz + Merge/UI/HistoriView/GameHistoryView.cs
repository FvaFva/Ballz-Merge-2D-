using BallzMerge.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHistoryView : CyclicBehavior, IInitializable, IInfoPanelView
{
    private const int CountPreload = 20;

    [SerializeField] private Button _toggle;
    [SerializeField] private TMP_Text _toggleLabel;
    [SerializeField] private GameDataView _gameDataPrefab;
    [SerializeField] private RectTransform _dataParent;

    private string[] _toggleLabels = { "ID", "Date" };
    private int _index = 0;
    private List<GameHistoryData> _data;
    private List<GameDataView> _allViews = new List<GameDataView>();
    private RectTransform _rootParent;
    private RectTransform _transform;

    public void Show(RectTransform showcase)
    {
        _toggle.onClick.AddListener(ChangeStateView);
        gameObject.SetActive(true);
        _transform.SetParent(showcase, false);
    }

    public bool SetData(List<GameHistoryData> data)
    {
        if (data == null || data.Count == 0)
            return false;

        _data = data;

        if (_data.Count > _allViews.Count)
            GenerateViews(_data.Count - _allViews.Count);

        Show();

        return true;
    }

    public void Hide()
    {
        foreach(var view in _allViews)
            view.Hide();

        _transform.SetParent(_rootParent, false);
        gameObject.SetActive(false);
        _toggle.onClick.RemoveListener(ChangeStateView);
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

    private void ChangeStateView()
    {
        _index = (_index + 1) % _toggleLabels.Length;
        _toggleLabel.text = _toggleLabels[_index];
        Show();
    }

    private string GetData(GameHistoryData data)
    {
        return _index == 0 ? data.ID : data.Date;
    }
}