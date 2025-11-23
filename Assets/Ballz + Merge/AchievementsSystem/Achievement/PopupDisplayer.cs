using BallzMerge.Achievement;
using BallzMerge.Root.Audio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDisplayer : DependentColorUI
{
    private const float PopupDuration = 5f;
    private const float StartShift = 200f;
    private const float NextShift = 210f;
    private const float AnimationDuration = 0.5f;

    [SerializeField] private AudioSourceHandler _audio;
    [SerializeField] private RectTransform _container;
    [SerializeField] private PopupView _achievementView;

    private Queue<PopupView> _activePopups = new Queue<PopupView>();
    private Vector2 _startPosition;
    private Vector2 _currentPosition;
    private Vector2 _nextPosition;
    private string _currentMessage;

    private void Awake()
    {
        _startPosition = new Vector2(0, StartShift);
        _currentPosition = new Vector2(0, StartShift);
        _nextPosition = new Vector2(0, NextShift);
    }

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;

        foreach (var popup in _activePopups)
            popup.ApplyColors(GameColors);
    }

    public void ShowPopup(AchievementData achievementData, int currentStep = 0, string message = null)
    {
        _currentMessage = message ?? achievementData.Name;
        _audio.Play();

        PopupView achievementView = Instantiate(_achievementView, _container);
        achievementView.ApplyColors(GameColors);

        if (message == null)
            achievementView.SetData(achievementData.Name, achievementData.Description, achievementData.Image, currentStep, achievementData.MaxTargets);
        else
            achievementView.SetData(_currentMessage, achievementData.Name, achievementData.Image);

        _activePopups.Enqueue(achievementView);

        if (_activePopups.Count > 1)
            _currentPosition += _nextPosition;

        achievementView.RectTransform.DOAnchorPos(_currentPosition, AnimationDuration).OnComplete(() => StartCoroutine(WaitCoroutine(achievementView))).SetEase(Ease.InOutQuad);
    }

    private IEnumerator WaitCoroutine(PopupView achievementView)
    {
        yield return new WaitForSeconds(PopupDuration);
        HideAchievement(achievementView);
    }

    private void HideAchievement(PopupView achievementView)
    {
        _activePopups.Dequeue();

        achievementView.RectTransform.DOAnchorPos(_container.anchoredPosition, AnimationDuration).OnComplete(() => Destroy(achievementView.gameObject)).SetEase(Ease.InOutQuad);

        if (_activePopups.Count >= 1)
        {
            _currentPosition -= _nextPosition;

            foreach (var activePopup in _activePopups)
            {
                if (activePopup != achievementView)
                {
                    float clampValue = Mathf.Max(((Vector2)activePopup.RectTransform.anchoredPosition - _nextPosition).y, _startPosition.y);
                    Debug.Log($"ClampValue: {clampValue}");
                    activePopup.RectTransform.DOAnchorPos(new Vector2(transform.position.x, clampValue), AnimationDuration);
                }
            }
        }
        else
        {
            _currentPosition = _startPosition;
        }
    }
}