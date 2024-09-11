using BallzMerge.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHistoryView : CyclicBehavior, IInitializable
{
    private const int CountPreload = 20;

    [SerializeField] private Button _close;
    [SerializeField] private GameDataView _gameDataPrefab;
    [SerializeField] private RectTransform _dataParent;

    private List<GameDataView> _allViews = new List<GameDataView>();

    private void OnEnable()
    {
        _close.AddListener(Hide);
    }

    private void OnDisable()
    {
        _close.RemoveListener(Hide);
    }

    public void Show(List<GameHistoryData> data)
    {
        if (data == null || data.Count == 0)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        int dataCount = data.Count;

        if (dataCount > _allViews.Count)
            GenerateViews(dataCount - _allViews.Count);

        for (int i = 0; i < dataCount; i++)
            _allViews[i].Show(data[i]);
    }

    public void Hide()
    {
        foreach(var view in _allViews)
            view.Hide();

        gameObject.SetActive(false);
    }

    public void Init()
    {
        GenerateViews(CountPreload);
        Hide();
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_gameDataPrefab, _dataParent).Init());
    }
}