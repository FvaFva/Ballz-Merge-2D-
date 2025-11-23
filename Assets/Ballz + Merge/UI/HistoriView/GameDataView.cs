using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameDataView : DependentColorUI
{
    [SerializeField] private TMP_Text _volumesLabel;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _number;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _toggleLabel;
    [SerializeField] private RectTransform _volumesParent;
    [SerializeField] private Image _backgroundView;
    [SerializeField] private GameDataVolumeMicView _volumeMicViewPrefab;
    [SerializeField] private List<BackgroundUI> _backgroundUIs;

    private List<GameDataVolumeMicView> _allViews;

    public GameDataView Init()
    {
        _volumesLabel.gameObject.SetActive(false);
        _allViews = new List<GameDataVolumeMicView>();
        return this;
    }

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;

        foreach (var backgroundUI in _backgroundUIs)
            backgroundUI.ApplyColors(GameColors);
    }

    public void Hide()
    {
        foreach (var item in _allViews)
            item.Hide();

        gameObject.SetActive(false);
    }

    public void Show(string toggleLabel, int score, int number, int level, Dictionary<string, List<int>> volumes, bool isCompleted)
    {
        _toggleLabel.text = toggleLabel;
        _score.text = score.ToString();
        _number.text = number.ToString();
        _level.text = level.ToString();
        _backgroundView.color = isCompleted ? GameColors.GetForDataView(DataViewType.Complete) : GameColors.GetForDataView(DataViewType.Lost);
        gameObject.SetActive(true);

        int current = 0;

        var result = volumes.ToDictionary(
            volume => volume.Key,
            volume => volume.Value
                .GroupBy(x => x)
                .Select(group => new { Value = group.Key, Count = group.Count() })
                .ToList()
        );

        int countData = result.Values.Sum(count => count.Count);

        if (countData > _allViews.Count)
            GenerateViews(countData - _allViews.Count);

        foreach (var volume in result)
        {
            foreach (var item in volume.Value)
            {
                _allViews[current++].Show(volume.Key, item.Value, item.Count);
            }
        }
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_volumeMicViewPrefab, _volumesParent).Init());
    }
}