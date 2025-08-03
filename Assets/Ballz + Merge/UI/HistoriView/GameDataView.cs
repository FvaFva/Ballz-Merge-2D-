using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameDataView : MonoBehaviour
{
    [SerializeField] private TMP_Text _volumesLabel;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _number;
    [SerializeField] private TMP_Text _toggleLabel;
    [SerializeField] private RectTransform _volumesParent;
    [SerializeField] private GameDataVolumeMicView _volumeMicViewPrefab;

    private List<GameDataVolumeMicView> _allViews;

    public GameDataView Init()
    {
        _volumesLabel.gameObject.SetActive(false);
        _allViews = new List<GameDataVolumeMicView>();
        return this;
    }

    public void Hide()
    {
        foreach (var item in _allViews)
            item.Hide();

        gameObject.SetActive(false);
    }

    public void Show(string toggleLabel, int score, int number, Dictionary<string, int> volumes)
    {
        _toggleLabel.text = toggleLabel;
        _score.text = score.ToString();
        _number.text = number.ToString();
        gameObject.SetActive(true);

        int countData = volumes.Count;

        if (countData > _allViews.Count)
            GenerateViews(countData - _allViews.Count);

        int current = 0;

        foreach (var item in volumes)
            _allViews[current++].Show(item.Key, item.Value);
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_volumeMicViewPrefab, _volumesParent).Init());
    }
}