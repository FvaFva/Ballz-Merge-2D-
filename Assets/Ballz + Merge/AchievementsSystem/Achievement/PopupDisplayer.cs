using BallzMerge.Achievement;
using BallzMerge.Root.Audio;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDisplayer : DependentColorUI
{
    private const float PopupDuration = 5f;
    private const float AnimationDuration = 0.5f;

    [SerializeField] private AudioSourceHandler _audio;
    [SerializeField] private RectTransform _container;
    [SerializeField] private PopupView _achievementView;

    private Queue<PopupView> _activePopups = new Queue<PopupView>();
    private Vector2 _startPosition;
    private Vector2 _currentPosition;
    private Vector2 _nextPosition;
    private string _currentMessage;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
    }

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;

        foreach (var popup in _activePopups)
            popup.ApplyColors(GameColors);
    }

    public void UpdatePositions()
    {
        float height = Mathf.Abs(_rectTransform.anchoredPosition.y);
        _startPosition = new Vector2(0, height);
        _currentPosition = _startPosition;
        _nextPosition = new Vector2(0, height - (height * 0.15f));

        if (_activePopups.Count == 0)
            return;

        foreach (var popup in _activePopups)
        {
            popup.RectTransform.anchoredPosition = _currentPosition;
            _currentPosition += _nextPosition;
        }

        _currentPosition = _startPosition;
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