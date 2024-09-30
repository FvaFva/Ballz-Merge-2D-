using BallzMerge.Data;
using System.Collections.Generic;
using UnityEngine;

public class GameHistoryView : CyclicBehavior, IInitializable, IInfoPanelView
{
    private const int CountPreload = 20;

    [SerializeField] private GameDataView _gameDataPrefab;
    [SerializeField] private RectTransform _dataParent;

    private List<GameDataView> _allViews = new List<GameDataView>();
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

        int dataCount = data.Count;

        if (dataCount > _allViews.Count)
            GenerateViews(dataCount - _allViews.Count);

        for (int i = 0; i < dataCount; i++)
            _allViews[i].Show(data[i]);

        return true;
    }

    public void Hide()
    {
        foreach(var view in _allViews)
            view.Hide();

        _transform.SetParent(_rootParent, false);
        gameObject.SetActive(false);
    }

    public void Init()
    {
        _transform = (RectTransform)transform;
        _rootParent = (RectTransform)_transform.parent;
        GenerateViews(CountPreload);
        Hide();
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_gameDataPrefab, _dataParent).Init());
    }
}