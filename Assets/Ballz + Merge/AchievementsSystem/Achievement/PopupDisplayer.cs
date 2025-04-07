using BallzMerge.Achievement;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDisplayer : MonoBehaviour
{
    private const float PopupDuration = 5f;
    private const float StartShift = 200f;
    private const float NextShift = 210f;
    private const float AnimationDuration = 0.5f;

    [SerializeField] private RectTransform _container;
    [SerializeField] private PopupView _achievementView;

    private Queue<PopupView> _activePopups = new Queue<PopupView>();
    private Vector2 _startPosition;
    private Vector2 _nextPosition;
    private string _currentMessage;

    private void Awake()
    {
        _startPosition = new Vector2(0, StartShift);
        _nextPosition = new Vector2(0, NextShift);
    }

    public void ShowPopup(AchievementData achievementData, int currentStep = 0, string message = null)
    {
        _currentMessage = message ?? achievementData.Name;

        PopupView achievementView = Instantiate(_achievementView, _container);

        if (message == null)
            achievementView.SetData(achievementData.Name, achievementData.Description, achievementData.Image, currentStep, achievementData.MaxTargets);
        else
            achievementView.SetData(_currentMessage, achievementData.Name, achievementData.Image);

        _activePopups.Enqueue(achievementView);

        if (_activePopups.Count > 1)
            _startPosition += _nextPosition;

        achievementView.RectTransform.DOAnchorPos(_startPosition, AnimationDuration).OnComplete(() => StartCoroutine(WaitCoroutine(achievementView))).SetEase(Ease.InOutQuad);
    }

    private IEnumerator WaitCoroutine(PopupView achievementView)
    {
        yield return new WaitForSeconds(PopupDuration);
        HideAchievement(achievementView);
    }

    private void HideAchievement(PopupView achievementView)
    {
        _activePopups.Dequeue();

        if (_activePopups.Count > 1)
            _startPosition -= _nextPosition;

        achievementView.RectTransform.DOAnchorPos(_container.anchoredPosition, AnimationDuration).OnComplete(() => Destroy(achievementView.gameObject)).SetEase(Ease.InOutQuad);
    }
}