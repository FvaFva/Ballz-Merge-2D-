using BallzMerge.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDataView : MonoBehaviour
{
    private const int CountPreload = 4;

    [SerializeField] private TMP_Text _id;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _date;
    [SerializeField] private TMP_Text _number;
    [SerializeField] private RectTransform _volumesParent;
    [SerializeField] private GameDataVolumeMicView _volumeMicViewPrefab;

    private List<GameDataVolumeMicView> _allViews;

    public GameDataView Init()
    {
        _allViews = new List<GameDataVolumeMicView>();
        GenerateViews(CountPreload);
        return this;
    }

    public void Hide()
    {
        foreach(var item in _allViews)
            item.Hide();

        gameObject.SetActive(false);
    }

    public void Show(GameHistoryData data)
    {
        _id.text = data.ID;
        _date.text = data.Date;
        _score.text = data.Score.ToString();
        _number.text = data.Number.ToString();
        gameObject.SetActive(true);

        int countData = data.Volumes.Count;

        if (countData > _allViews.Count)
            GenerateViews(countData - _allViews.Count);

        int current = 0;

        foreach(var item in data.Volumes)
            _allViews[current++].Show(item.Key, item.Value);
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i< count; i++)
            _allViews.Add(Instantiate(_volumeMicViewPrefab, _volumesParent).Init());
    }
}
