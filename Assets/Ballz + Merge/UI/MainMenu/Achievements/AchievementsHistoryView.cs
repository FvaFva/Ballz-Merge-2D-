using BallzMerge.Achievement;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsHistoryView : CyclicBehavior, IInitializable, IInfoPanelView
{
    [SerializeField] private AchievementView _achievementViewPrefab;
    [SerializeField] private RectTransform _dataParent;
    [SerializeField] private Color _achievementBackgroundColor;

    private readonly List<AchievementView> _allViews = new List<AchievementView>();
    private RectTransform _rootParent;
    private RectTransform _transform;
    private List<KeyValuePair<AchievementSettings, AchievementPointsStep>> _achievementData;

    public void Init()
    {
        _transform = (RectTransform)transform;
        _rootParent = (RectTransform)_transform.parent;
        Hide();
    }

    public bool SetData(IDictionary<AchievementSettings, AchievementPointsStep> achievementData)
    {
        _achievementData = new List<KeyValuePair<AchievementSettings, AchievementPointsStep>>(achievementData);

        if (_achievementData.Count > _allViews.Count)
            GenerateViews(_achievementData.Count - _allViews.Count);

        return true;
    }

    public void Show(RectTransform showcase)
    {
        for (int i = 0; i < _achievementData.Count; i++)
            _allViews[i].SetData(_achievementData[i].Key.Name, _achievementData[i].Key.Description, _achievementData[i].Key.Image, _achievementData[i].Value.Step, _achievementData[i].Key.MaxTargets);

        gameObject.SetActive(true);
        _transform.SetParent(showcase, false);
    }

    public void Hide()
    {
        _transform.SetParent(_rootParent, false);
        gameObject.SetActive(false);
    }

    private void GenerateViews(int count)
    {
        for (int i = 0; i < count; i++)
            _allViews.Add(Instantiate(_achievementViewPrefab, _dataParent).SetBackgroundColor(_achievementBackgroundColor));
    }
}