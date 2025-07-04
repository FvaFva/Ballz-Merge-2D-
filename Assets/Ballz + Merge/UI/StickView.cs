using System;
using System.Collections;
using System.Collections.Generic;
using BallzMerge.Gameplay;
using UnityEngine;
using Zenject;

public class StickView : MonoBehaviour
{
    private const float ClampCoefficient = 3.5f;
    private const float BackgroundMoveDelay = 0.06f;
    private const float MinRange = 10f;

    [SerializeField] private UIZoneObserver _inputZone;
    [SerializeField] private UIZoneObserver _cancelZone;
    [SerializeField] private RectTransform _stickZone;
    [SerializeField] private RectTransform _stickBackground;
    [SerializeField] private ImageAnimator _inputZoneAnimator;
    [SerializeField] private ImageAnimator _cancelZoneAnimator;

    [Inject] private CamerasOperator _cameras;

    private WaitForSeconds _delay = new WaitForSeconds(BackgroundMoveDelay);
    private Queue<Coroutine> _backMoves;
    private Action _action;
    private Queue<Action> _transitionsZoneReactions;
    private Vector2 _position;
    private Vector2 _stickPosition;
    private Vector2 _startStickAnchorPosition;
    private UIZoneObserver _monitoredZone;
    private bool _previousInZone;

    public bool IsInZone { get; private set; }

    private void Awake()
    {
        _backMoves = new Queue<Coroutine>();
        _transitionsZoneReactions = new Queue<Action>();
        _startStickAnchorPosition = _stickZone.anchoredPosition;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _inputZone.SetActiveIfNotNull(true);
        EnterMonitoring();
    }

    private void OnDisable()
    {
        for (int i = _backMoves.Count - 1; i >= 0; i--)
            StopCoroutine(_backMoves.Dequeue());

        _inputZone.SetActiveIfNotNull(false);
        _cancelZone.SetActiveIfNotNull(false);
    }

    public void EnterAim()
    {
        _monitoredZone = _cancelZone;
        TransitState(_cancelZoneAnimator, true);
        _action = AimAction;
        _inputZoneAnimator.Press();
    }

    public void EnterMonitoring()
    {
        _monitoredZone = _inputZone;
        _inputZone.gameObject.SetActive(true);
        _inputZoneAnimator.SetDefault();
        TransitState(_inputZoneAnimator, false);
        SetBaseStick();
        _action = () => { };
    }

    public Vector2 GetCenterPosition()
    {
        return _inputZone.GetCenterInWorld(_cameras.UI);
    }

    private void TransitState(ImageAnimator monitored, bool cancelState)
    {
        IsInZone = false;
        _cancelZone.gameObject.SetActive(cancelState);
        _transitionsZoneReactions.Clear();
        _transitionsZoneReactions.Enqueue(monitored.Highlight);
        _transitionsZoneReactions.Enqueue(monitored.DoDefault);
    }

    public void ApplyPosition(Vector2 position)
    {
        _position = position;
        _previousInZone = IsInZone;
        IsInZone = _monitoredZone.IsIn;
        CheckAnimation();
        _action();
    }

    private static Vector2 ClampToRect(RectTransform zone, Vector2 targetPosition)
    {
        Vector2 size = zone.rect.size;
        Vector2 clampSize = size / ClampCoefficient;

        float clampedX = Mathf.Clamp(targetPosition.x, -clampSize.x, clampSize.x);
        float clampedY = Mathf.Clamp(targetPosition.y, -clampSize.y, clampSize.y);

        return new Vector2(clampedX, clampedY);
    }

    private void CheckAnimation()
    {
        if (_previousInZone != IsInZone)
        {
            var current = _transitionsZoneReactions.Dequeue();
            _transitionsZoneReactions.Enqueue(current);
            current();
        }
    }

    private Vector2 GetConvertToLocalVector(Vector2 point, RectTransform rectTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, point, _cameras.UI, out Vector2 localPoint);
        return localPoint;
    }

    private void AimAction()
    {
        bool isAimActive = !IsInZone;

        if (_previousInZone != IsInZone)
        {
            _inputZone.gameObject.SetActive(isAimActive);
            SetBaseStick();
        }

        if(isAimActive)
        {
            Vector2 localPoint = GetConvertToLocalVector(_position, _inputZone.Transform);
            Vector2 clampedPosition = ClampToRect(_inputZone.Transform, localPoint);
            float range = Vector2.Distance(_stickPosition, clampedPosition);

            if (range > MinRange)
            {
                _stickPosition = clampedPosition;
                _stickZone.anchoredPosition = _stickPosition;
                _backMoves.Enqueue(StartCoroutine(BackMove(_stickPosition)));
            }
        }
    }

    private void SetBaseStick()
    {
        _stickZone.anchoredPosition = _startStickAnchorPosition;
        _stickBackground.anchoredPosition = _startStickAnchorPosition;
    }

    private IEnumerator BackMove(Vector2 position)
    {
        yield return _delay;
        _stickBackground.anchoredPosition = position;
        StopCoroutine(_backMoves.Dequeue());
    }
}
