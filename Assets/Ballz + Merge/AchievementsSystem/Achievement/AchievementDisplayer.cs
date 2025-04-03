using BallzMerge.Achievement;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDisplayer : MonoBehaviour
{
    private const float PopupDuration = 5f;
    private const float StartShift = 200f;
    private const float NextShift = 210f;
    private const float AnimationDuration = 0.5f;

    private RectTransform _container;
    private AchievementView _achievementView;
    private Queue<AchievementView> _activePopups = new Queue<AchievementView>();
    private Vector2 _startPosition;
    private Vector2 _nextPosition;
    private string _currentLabel;
    private string _currentDescription;
    private Sprite _currentImage;
    private int _currentStep;
    private int _currentMaxTargets;

    public AchievementDisplayer Init(AchievementView achievementView, RectTransform container)
    {
        _achievementView = achievementView;
        _container = container;
        _startPosition = new Vector2(0, StartShift);
        _nextPosition = new Vector2(0, NextShift);
        return this;
    }

    public void SpawnView(string label, string description, Sprite image, int currentStep, int maxTargets)
    {
        _currentLabel = label;
        _currentDescription = description;
        _currentImage = image;
        _currentStep = currentStep;
        _currentMaxTargets = maxTargets;
        ShowAchievement();
    }

    private void ShowAchievement()
    {
        AchievementView achievementView = Instantiate(_achievementView, _container);
        achievementView.SetData(_currentLabel, _currentDescription, _currentImage, _currentStep, _currentMaxTargets);
        _activePopups.Enqueue(achievementView);

        if (_activePopups.Count > 1)
            _startPosition += _nextPosition;

        achievementView.RectTransform.DOAnchorPos(_startPosition, AnimationDuration).OnComplete(() => StartCoroutine(WaitCoroutine(achievementView))).SetEase(Ease.InOutQuad);
    }

    private IEnumerator WaitCoroutine(AchievementView achievementView)
    {
        yield return new WaitForSeconds(PopupDuration);
        HideAchievement(achievementView);
    }

    private void HideAchievement(AchievementView achievementView)
    {
        _activePopups.Dequeue();

        if (_activePopups.Count > 1)
            _startPosition -= _nextPosition;

        achievementView.RectTransform.DOAnchorPos(_container.anchoredPosition, AnimationDuration).OnComplete(() => Destroy(achievementView.gameObject)).SetEase(Ease.InOutQuad);
    }
}