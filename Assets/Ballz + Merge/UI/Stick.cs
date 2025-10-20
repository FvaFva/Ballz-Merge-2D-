using System;
using System.Collections.Generic;
using BallzMerge.Gameplay.BallSpace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Stick : CyclicBehavior, IInitializable, IDisposable
{
    private const float StartStickPosition = 0.5f;
    private const float VisibleImage = 0.75f;
    private const float HideVisibleImage = 0f;
    private const float StartScale = 1f;
    private const float HighlightedStateScale = 1.05f;
    private const float Duration = 0.125f;

    [SerializeField] private Slider _slider;
    [SerializeField] private SliderHandle _sliderHandle;
    [SerializeField] private UIZoneObserver _inputZone;
    [SerializeField] private UIZoneObserver _cancelZone;
    [SerializeField] private Image _visibleImage;

    [Inject] private Ball _ball;

    private Dictionary<bool, Color> _visibleImageState;

    public event Action<float> StickValueChanged;
    public event Action<bool> StickHandled;

    public bool IsInZone { get; private set; }

    private void Awake()
    {
        Color visibleColor = _visibleImage.color;
        visibleColor.a = VisibleImage;
        Color hideColor = _visibleImage.color;
        hideColor.a = HideVisibleImage;
        _visibleImageState = new Dictionary<bool, Color>
        {
            { true, visibleColor },
            { false, hideColor }
        };

        _sliderHandle.Init();
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _sliderHandle.SliderHandled += OnSliderHandled;
        _inputZone.SetState(true);
    }

    public void Init()
    {
        _ball.EnterAIM += EnterMonitoring;
    }

    public void Dispose()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _sliderHandle.SliderHandled -= OnSliderHandled;
        _cancelZone.Entered -= OnEnteredCancelZone;
        _ball.EnterAIM -= EnterMonitoring;
        _inputZone.SetState(false);
        _cancelZone.SetState(false);
    }

    private void OnSliderValueChanged(float newValue) => StickValueChanged?.Invoke(newValue);
    private void OnSliderHandled(bool isHandled) => StickHandled?.Invoke(isHandled);

    public void EnterMonitoring()
    {
        TransitState(false);
        _inputZone.SetState(true);
        _cancelZone.Entered -= OnEnteredCancelZone;
        _inputZone.Entered += OnEnteredInputZone;
        SetBaseStick();
    }

    public void EnterAim()
    {
        TransitState(true);
        _inputZone.Entered -= OnEnteredInputZone;
        _cancelZone.Entered += OnEnteredCancelZone;
    }

    public void EnterShooting()
    {
        TransitState(false);
    }

    private void TransitState(bool state)
    {
        IsInZone = false;
        SetVisibleImage(false);
        _cancelZone.SetState(state);
        _slider.interactable = state;
    }

    private void OnEnteredCancelZone(bool state)
    {
        IsInZone = state;
        SetVisibleImage(state);
        _cancelZone.SetSize(state ? HighlightedStateScale : StartScale, Duration);
    }

    private void OnEnteredInputZone(bool state)
    {
        _inputZone.SetSize(state ? HighlightedStateScale : StartScale, Duration);
    }

    private void SetBaseStick()
    {
        _slider.SetValueWithoutNotify(StartStickPosition);
        _inputZone.SetSize(StartScale, Duration);
        _cancelZone.SetSize(StartScale, Duration);
    }

    private void SetVisibleImage(bool isVisible)
    {
        DOTween.To(
                () => _visibleImage.color,
                x => _visibleImage.color = x,
                _visibleImageState[isVisible],
                Duration
            ).SetEase(Ease.InOutQuad);
    }
}
