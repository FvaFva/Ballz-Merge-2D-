using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDisposable
{
    private const float StartScale = 1f;
    private const float PressedStateScale = 0.9f;
    private const float HighlightedStateScale = 1.05f;
    private const float Duration = 0.125f;

    [SerializeField] private SliderDragger _sliderDragger;
    [SerializeField] private SliderView _sliderView;

    private Dictionary<bool, Action> _sliderViewStateActions;
    private Dictionary<bool, Action> _handledStateActions;
    private Transform _transform;
    private bool _isPointerDown;
    private bool _isPointerEnter;
    private bool _isDragging;

    public event Action<bool> SliderHandled;

    public void Init()
    {
        _sliderViewStateActions = new Dictionary<bool, Action>
        {
            { true, ActivateSliderView },
            { false, DeactivateSliderView }
        };

        _handledStateActions = new Dictionary<bool, Action>
        {
            { true, Press },
            { false, Release }
        };

        _transform = transform;
        _sliderView.Init();
        _sliderDragger.Handled += SetDraggingState;
    }

    public void Dispose()
    {
        _sliderDragger.Handled -= SetDraggingState;
    }

    public void SetDraggingState(bool isDragging)
    {
        _isDragging = isDragging;
        _handledStateActions.GetValueOrDefault(isDragging)?.Invoke();
        SliderHandled?.Invoke(isDragging);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isDragging || _isPointerDown)
            return;

        _isPointerEnter = true;
        _transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
        _sliderView.ChangeParameters(StartScale, Duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;

        if (_isDragging || _isPointerDown)
            return;

        _transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);
        SetDefault();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isDragging)
            return;

        Press();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging)
            return;

        Release();
    }

    public void Press()
    {
        if (_isDragging)
            return;

        _isPointerDown = true;
        _sliderView.ChangeParameters(PressedStateScale, Duration);
    }

    public void Release()
    {
        _isPointerDown = false;

        if (_isPointerEnter)
            _transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
        else
            _transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);

        SetDefault();
    }

    public void SetState(bool state)
    {
        _sliderViewStateActions.GetValueOrDefault(state)?.Invoke();
        enabled = state;
    }

    private void SetDefault()
    {
        _sliderView.ChangeParameters(StartScale, Duration);
    }

    private void ActivateSliderView()
    {
        _sliderView.SetDefaultColor();
    }

    private void DeactivateSliderView()
    {
        _sliderView.ChangeViewColor(Color.white);
    }
}
