using BallzMerge.Gameplay;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BallStrikeVectorReader : BallComponent
{
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
        if (IsCursorIn(_inputZone))
        {
            _userInput.MainInput.StrikeVector.performed += OnStrikeVectorPerformed;
            _userInput.MainInput.Shot.canceled += OnShotCancelled;
            _cancelZone.gameObject.SetActive(true);
        }
    }

    private void EndFrameShotCancelled()
    {
        _userInput.MainInput.StrikeVector.performed -= OnStrikeVectorPerformed;
        _userInput.MainInput.Shot.canceled -= OnShotCancelled;

        _cancelZone.gameObject.SetActive(false);
        _stickZone.anchoredPosition = _startStickAnchorPosition;

        if (IsCursorIn(_cancelZone))
        {
            Canceled?.Invoke();
            return;
        }

        Vector3 dropVector = GetVector();
        _vector = Vector3.zero;
        Dropped?.Invoke(dropVector); 
    }

    private void OnStrikeVectorPerformed(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();

        // Ограничиваем движение внутри прямоугольника
        Vector2 localPoint = GetConvertToLocalVector(_inputZone);
        Vector2 clampedPosition = ClampToRect(localPoint);
        _stickZone.anchoredPosition = clampedPosition;

        if (direction.Equals(_vector) == false)
        {
            _vector = direction - _touchPoint;
            Changed?.Invoke(GetVector());
        }
    }

    private bool IsCursorIn(RectTransform zone)
    {
        Vector3 localPoint = GetConvertToLocalVector(zone);
        return zone.rect.Contains(localPoint);
    }

    private Vector2 GetConvertToLocalVector(RectTransform rectTransform)
    {
        _touchPoint = _userInput.MainInput.StrikePosition.ReadValue<Vector2>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, _touchPoint, _operator.UI, out Vector2 localPoint);
        return localPoint;
    }

    private Vector2 ClampToRect(Vector2 targetPosition)
    {
        Vector2 size = _inputZone.rect.size;
        Vector2 halfSize = size / 3.5f;

        // Ограничиваем позицию в пределах прямоугольника
        float clampedX = Mathf.Clamp(targetPosition.x, -halfSize.x, halfSize.x);
        float clampedY = Mathf.Clamp(targetPosition.y, -halfSize.y, halfSize.y);

        return new Vector2(clampedX, clampedY);
    }
}
