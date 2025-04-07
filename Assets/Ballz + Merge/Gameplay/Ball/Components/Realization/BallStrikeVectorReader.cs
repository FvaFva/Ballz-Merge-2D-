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
    [SerializeField] private RectTransform _cancelZone;

    [Inject] private MainInputMap _userInput;

    private Vector3 _vector;
    private Vector2 _touchPoint;

    public event Action<Vector3> Changed;
    public event Action<Vector3> Dropped;
    public event Action Canceled;

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

        if (direction.Equals(_vector) == false)
        {
            _vector = direction - _touchPoint;
            Changed?.Invoke(GetVector());
        }
    }

    private bool IsCursorIn(RectTransform zone)
    {
        _touchPoint = _userInput.MainInput.StrikePosition.ReadValue<Vector2>();
        Vector3 worldTouchPosition = _operator.UI.ScreenToWorldPoint(_touchPoint);
        Vector2 localTouchPosition = zone.InverseTransformPoint(worldTouchPosition);
        return zone.rect.Contains(localTouchPosition);
    }
}
