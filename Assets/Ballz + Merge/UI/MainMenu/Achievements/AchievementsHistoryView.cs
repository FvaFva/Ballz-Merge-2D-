using BallzMerge.Achievement;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsHistoryView : DependentColorUI, IInitializable, IInfoPanelView
{
    [SerializeField] private PopupView _achievementViewPrefab;
    [SerializeField] private RectTransform _dataParent;
    [SerializeField] private List<DependentColorUI> _dependentColorUIs;

    private readonly List<PopupView> _allViews = new List<PopupView>();
    private RectTransform _rootParent;
    private RectTransform _transform;
    private List<KeyValuePair<AchievementSettings, AchievementPointsStep>> _achievementData;

    public void Init()
    {
        _transform = (RectTransform)transform;
        _rootParent = (RectTransform)_transform.parent;
        Hide();
    }

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;

        foreach (var dependentColorUI in _dependentColorUIs)
            dependentColorUI.ApplyColors(GameColors);

        foreach (var view in _allViews)
            view.ApplyColors(GameColors);
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
        {
            PopupView popupView = Instantiate(_achievementViewPrefab, _dataParent);
            popupView.ApplyColors(GameColors);
            _allViews.Add(popupView);
        }
    }
}