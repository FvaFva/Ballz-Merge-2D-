using BallzMerge.Gameplay;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BallStrikeVectorReader : BallComponent
{
    private const float ClampCoefficient = 3.5f;

    [SerializeField] private CamerasOperator _operator;
    [SerializeField] private RectTransform _inputZone;
    [SerializeField] private RectTransform _stickZone;
    [SerializeField] private RectTransform _cancelZone;

    [Inject] private MainInputMap _userInput;

    private Vector3 _vector;
    private Vector2 _touchPoint;
    private Vector2 _startStickAnchorPosition;

    public event Action<Vector3> Changed;
    public event Action<Vector3> Dropped;
    public event Action Canceled;

    private void Start()
    {
        _startStickAnchorPosition = _stickZone.anchoredPosition;
    }

    private void OnEnable()
    {
        _userInput.MainInput.Shot.started += OnShotStarted;
        _inputZone.gameObject.SetActive(true);
        _cancelZone.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _userInput.MainInput.Shot.started -= OnShotStarted;
        _inputZone.gameObject.SetActive(false);
        _cancelZone.gameObject.SetActive(false);
    }

    public Vector3 GetVector()
    {
        Vector3 final = _vector;
        return final.normalized * -1;
    }

    private void OnShotStarted(InputAction.CallbackContext context) => StartCoroutine(HandleEndOfFrame(EndFrameShotStarted));

    private void OnShotCancelled(InputAction.CallbackContext context) => StartCoroutine(HandleEndOfFrame(EndFrameShotCancelled));

    private IEnumerator HandleEndOfFrame(Action action)
    {
        yield return new WaitForEndOfFrame();

        action.Invoke();
    }

    private void EndFrameShotStarted()
    {
        _touchPoint = _userInput.MainInput.StrikePosition.ReadValue<Vector2>();

        if (IsCursorIn(_touchPoint, _inputZone))
        {
            Vector2 localPoint = GetConvertToLocalVector(_touchPoint, _inputZone);
            Vector2 clampedPosition = ClampToRect(_inputZone, localPoint);
            _stickZone.anchoredPosition = clampedPosition;
            _userInput.MainInput.StrikeVector.performed += OnStrikeVectorPerformed;
            _userInput.MainInput.Shot.canceled += OnShotCancelled;
            _cancelZone.gameObject.SetActive(true);
        }
    }

    private void EndFrameShotCancelled()
    {
        _userInput.MainInput.Shot.canceled -= OnShotCancelled;

        _cancelZone.gameObject.SetActive(false);
        _stickZone.anchoredPosition = _startStickAnchorPosition;
        _touchPoint = _userInput.MainInput.StrikePosition.ReadValue<Vector2>();

        if (IsCursorIn(_touchPoint, _cancelZone))
        {
            _userInput.MainInput.StrikeVector.performed -= CheckGetOutFromZone;
            _inputZone.gameObject.SetActive(true);
            Canceled?.Invoke();
            return;
        }

        _userInput.MainInput.StrikeVector.performed -= OnStrikeVectorPerformed;
        Vector3 dropVector = GetVector();
        _vector = Vector3.zero;
        Dropped?.Invoke(dropVector);
    }

    private void OnStrikeVectorPerformed(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();

        if (direction.Equals(_vector) == false)
        {
            _vector = direction - _touchPoint;
            Changed?.Invoke(GetVector());
        }

        Vector2 localPoint = GetConvertToLocalVector(direction, _inputZone);
        Vector2 clampedPosition = ClampToRect(_inputZone, localPoint);
        _stickZone.anchoredPosition = clampedPosition;

        if (IsCursorIn(direction, _cancelZone))
        {
            _userInput.MainInput.StrikeVector.performed -= OnStrikeVectorPerformed;
            _userInput.MainInput.StrikeVector.performed += CheckGetOutFromZone;
            _inputZone.gameObject.SetActive(false);
        }
    }

    private void CheckGetOutFromZone(InputAction.CallbackContext context)
    {
        Vector2 position = context.ReadValue<Vector2>();

        if (IsCursorIn(position, _cancelZone))
            return;

        _userInput.MainInput.StrikeVector.performed -= CheckGetOutFromZone;
        _userInput.MainInput.StrikeVector.performed += OnStrikeVectorPerformed;
        _inputZone.gameObject.SetActive(true);
    }

    private bool IsCursorIn(Vector2 point, RectTransform zone)
    {
        Vector3 localPoint = GetConvertToLocalVector(point, zone);
        return zone.rect.Contains(localPoint);
    }

    private Vector2 GetConvertToLocalVector(Vector2 point, RectTransform rectTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, point, _operator.UI, out Vector2 localPoint);
        return localPoint;
    }

    private static Vector2 ClampToRect(RectTransform zone, Vector2 targetPosition)
    {
        Vector2 size = zone.rect.size;
        Vector2 clampSize = size / ClampCoefficient;

        float clampedX = Mathf.Clamp(targetPosition.x, -clampSize.x, clampSize.x);
        float clampedY = Mathf.Clamp(targetPosition.y, -clampSize.y, clampSize.y);

        return new Vector2(clampedX, clampedY);
    }
}
