using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameDataView : MonoBehaviour
{
    [SerializeField] private TMP_Text _volumesLabel;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _number;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _toggleLabel;
    [SerializeField] private RectTransform _volumesParent;
    [SerializeField] private Image _backgroundView;
    [SerializeField] private GameDataVolumeMicView _volumeMicViewPrefab;

    private List<GameDataVolumeMicView> _allViews;
    private Color LostColor = new(0.9176471f, 0.345098f, 0.345098f);
    private Color CompleteColor = new(0.5607843f, 0.9843137f, 0.3686275f);

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

    public void Show(string toggleLabel, int score, int number, int level, Dictionary<string, int> volumes, bool isCompleted)
    {
        _toggleLabel.text = toggleLabel;
        _score.text = score.ToString();
        _number.text = number.ToString();
        _level.text = level.ToString();
        _backgroundView.color = isCompleted ? CompleteColor : LostColor;
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