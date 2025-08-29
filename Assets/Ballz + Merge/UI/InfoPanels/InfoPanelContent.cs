using System;
using BallzMerge.Root.Audio;
using DG.Tweening;
using UnityEngine;

public class InfoPanelContent : MonoBehaviour
{
    private const float OpenDuration = 0.3f;
    private const float CloseDuration = 0.2f;
    private const float OversizeScale = 1.2f;

    [SerializeField] private AudioSourceHandler _audio;
    [SerializeField] private CanvasGroup _canvas;

    private RectTransform _transform;
    private Sequence _seq;

    private void Start()
    {
        _transform = (RectTransform)transform;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _seq?.Kill();
        _transform.localScale = Vector3.one;
        _canvas.alpha = 0f;
    }

    public void Open()
    {
        _audio.Play(1);
        _seq?.Kill();
        _canvas.alpha = 0f;
        _transform.localScale = Vector3.one * OversizeScale;
        gameObject.SetActive(true);

        _seq = DOTween.Sequence()
            .Join(_transform.DOScale(1f, OpenDuration).SetEase(Ease.OutBack))
            .Join(_canvas.DOFade(1f, OpenDuration))
            .SetUpdate(true);
    }

    public void Close(Action callback)
    {
        _audio.Play(2);
        _seq?.Kill();
        _seq = DOTween.Sequence()
            .Join(_transform.DOScale(OversizeScale, CloseDuration).SetEase(Ease.InCubic))
            .Join(_canvas.DOFade(0f, CloseDuration))
            .SetUpdate(true)
            .OnComplete(() => OnAnimationCloseFinish(callback));
    }

    private void OnAnimationCloseFinish(Action close)
    {
        _transform.localScale = Vector3.one;
        close();
        gameObject.SetActive(false);
    }
}
