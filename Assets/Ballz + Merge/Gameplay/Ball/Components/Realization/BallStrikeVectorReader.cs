using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BallStrikeVectorReader : BallComponent
{
    [SerializeField] private StickView _view;

    [Inject] private MainInputMap _userInput;

    private Vector3 _vector;
    private Vector2 _touchPoint;
    private Vector2 _inputCentre;
    private WaitForEndOfFrame _delay = new WaitForEndOfFrame();

    public event Action<Vector3> Changed;
    public event Action<Vector3> Dropped;
    public event Action Canceled;

    private void OnEnable()
    {
        _userInput.MainInput.StrikeVector.performed += VectorMoved;
        _userInput.MainInput.Shot.started += OnShotStarted;
        _view.SetActive(true);
    }

    private void OnDisable()
    {
        _userInput.MainInput.StrikeVector.performed -= VectorMoved;
        _userInput.MainInput.Shot.started -= OnShotStarted;
        _view.SetActive(false);
    }

    public Vector3 GetDirection()
    {
        Vector3 final = _vector;
        return final.normalized * -1;
    }

    private void VectorMoved(InputAction.CallbackContext context)
    {
        _touchPoint = context.ReadValue<Vector2>();
        _view.ApplyPosition(_touchPoint);
    }

    private void OnShotStarted(InputAction.CallbackContext context) => StartCoroutine(HandleEndOfFrame(EndFrameShotStarted));
    private void OnShotCancelled(InputAction.CallbackContext context) => StartCoroutine(HandleEndOfFrame(EndFrameShotCancelled));
    private void OnStrikeVectorPerformed(InputAction.CallbackContext context) => StartCoroutine(HandleEndOfFrame(EndFrameOnStrikeVectorPerformed));

    private IEnumerator HandleEndOfFrame(Action action)
    {
        yield return _delay;
        action.Invoke();
    }

    private void EndFrameShotStarted()
    {
        _inputCentre = _view.GetCenterPosition();

        if (_view.IsInZone)
        {
            _userInput.MainInput.StrikeVector.performed += OnStrikeVectorPerformed;
            _userInput.MainInput.Shot.canceled += OnShotCancelled;
            _view.EnterAim();
        }
    }

    private void EndFrameShotCancelled()
    {
        _userInput.MainInput.Shot.canceled -= OnShotCancelled;
        _userInput.MainInput.StrikeVector.performed -= OnStrikeVectorPerformed;

        if (_view.IsInZone)
        {
            Canceled?.Invoke();
            _view.EnterMonitoring();
        }
        else
        {
            Vector3 dropVector = GetDirection();
            _vector = Vector3.zero;
            Dropped?.Invoke(dropVector);
        }
    }

    private void EndFrameOnStrikeVectorPerformed()
    {
        if (!_view.IsInZone)
        {
            var newVector = _touchPoint - _inputCentre;

            if (newVector.Equals(_vector) == false)
            {
                _vector = newVector;
                Changed?.Invoke(GetDirection());
            }
        }
    }
}
