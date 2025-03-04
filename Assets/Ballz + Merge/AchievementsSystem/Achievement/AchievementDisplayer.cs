using BallzMerge.Achievement;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDisplayer
{
    private Queue<AchievementView> _achievementViews = new Queue<AchievementView>();
    private RectTransform _container;
    private AchievementView _achievementView;

    public AchievementDisplayer(AchievementView achievementView, RectTransform container)
    {
        _achievementView = achievementView;
        _container = container;
    }

    public void SpawnView(string label, string description, Sprite image)
    {
        if (_achievementViews.TryDequeue(out AchievementView achievementView))
        {
            achievementView.Show(label, description, image);
        }
        else
        {
            achievementView = Object.Instantiate(_achievementView, _container);
            _achievementViews.Enqueue(achievementView);
            achievementView.Show(label, description, image);
        }
    }
}
