using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeMicAdditionalView : MonoBehaviour
{
    private const float Frame = 0.04f;
    private const float AnimationTime = 0.15f;
    private const float WriteTime = 0.75f;
    private const float PumpScale = 1.25f;

    [SerializeField] private List<Button> _triggers;
    [SerializeField] private TMP_Text _descriptionView;
    [SerializeField] private RectPumper _pumper;

    private RectTransform _transform;
    private bool _isOpened;
    private bool _isNeedCallback;

    private Vector2 _originalAnchorMin;
    private Vector2 _originalAnchorMax;
    private Vector2 _openedAnchorMin;
    private Vector2 _openedAnchorMax;
    private Tween _currentTween;

    public event Action<bool> Performed;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _originalAnchorMin = _transform.anchorMin;
        _originalAnchorMax = _transform.anchorMax;
        _openedAnchorMin = new Vector2(Frame, Frame);
        _openedAnchorMax = new Vector2(1 - Frame, 1 - Frame);
        _isNeedCallback = true;
    }

    private void OnEnable()
    {
        foreach(var trigger in _triggers)
            trigger.AddListener(ChangeVew);

        SetBaseView();
    }

    private void OnDisable()
    {
        foreach (var trigger in _triggers)
            trigger.RemoveListener(ChangeVew);

        SetBaseView();
    }

    public void Show(string description)
    {
        _descriptionView.text = description;
        _isNeedCallback = true;
    }

    public void Unperformed()
    {
        if(_isOpened)
        {
            _isNeedCallback = false;
            ChangeVew();
        }
    }

    public void SetBaseView()
    {
        StopCurrentAnimation();

        if (_isOpened)
            Performed?.Invoke(false);

        _isOpened = false;
        _transform.anchorMin = _originalAnchorMin;
        _transform.anchorMax = _originalAnchorMax;
        _transform.localScale = Vector3.one;
        _descriptionView.gameObject.SetActive(false);
        _descriptionView.maxVisibleCharacters = 0;
    }

    private void ChangeVew()
    {
        _pumper.enabled = _isOpened;
        _isOpened = !_isOpened;
        _descriptionView.gameObject.SetActive(_isOpened);
        _descriptionView.maxVisibleCharacters = 0;

        if (_isOpened)
            RestartSizeAnimation(_openedAnchorMin, _openedAnchorMax);
        else
            RestartSizeAnimation(_originalAnchorMin, _originalAnchorMax);
    }

    private void StopCurrentAnimation()
    {
        if (_currentTween != null && _currentTween.active)
            _currentTween.Kill();

        _currentTween = null;
    }

    private void RestartSizeAnimation(Vector2 anchorMin, Vector2 anchorMax)
    {
        StopCurrentAnimation();
        _currentTween = DOTween.Sequence()
                .Join(_transform.DOAnchorMin(anchorMin, AnimationTime))
                .Join(_transform.DOAnchorMax(anchorMax, AnimationTime))
                .Join(DOVirtual.Vector2(_transform.offsetMin, Vector2.zero, AnimationTime,
                    val => _transform.offsetMin = val))
                .Join(DOVirtual.Vector2(_transform.offsetMax, Vector2.zero, AnimationTime,
                    val => _transform.offsetMax = val))
                .OnComplete(OnComplete);
    }

    private void OnComplete()
    {
        _currentTween = DOTween.Sequence()
            .Join(_transform.DOScale(PumpScale, AnimationTime)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo));

        if(_isOpened)
        {
            _currentTween.OnComplete(() =>
                _currentTween = DOTween.To(()=> _descriptionView.maxVisibleCharacters
                , x => _descriptionView.maxVisibleCharacters = x
                , _descriptionView.text.Length
                , WriteTime)
                .SetEase(Ease.Linear)
                .OnStart(CallbackOnPerformed));
        }
        else
        {
            CallbackOnPerformed();
        }
    }

    private void CallbackOnPerformed()
    {
        if(_isNeedCallback)
            Performed?.Invoke(_isOpened);

        _isNeedCallback = true;
    }
}
