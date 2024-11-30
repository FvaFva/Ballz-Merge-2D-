using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BallStrikeVectorReader : BallComponent
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RectTransform _inputRotationZone;

    [Inject] private MainInputMap _userInput;

    private Vector3 _vector;
    private Transform _transform;
    private float _cameraY;

    public event Action<Vector3> Changed;
    public event Action<Vector3> Dropped;

    private void Awake()
    {
        _transform = transform;
        _cameraY = _camera.transform.position.y;
    }

    private void OnEnable()
    {
        _userInput.MainInput.Shot.started += OnShotStarted;
    }

    private void OnDisable()
    {
        _userInput.MainInput.Shot.started -= OnShotStarted;
    }

    /*private void Update()
    {
        Vector3 old = _vector;
        DesktopProcessor(old);
    }*/

    public Vector3 GetVector()
    {
        Vector3 final = _vector;
        final.z = 0;
        return final.normalized * -1;
    }

    private Vector3 GetMouseVector(Vector3 input)
    {
        input.z = _cameraY;
        return _camera.ScreenToWorldPoint(input) - _transform.position;
    }

    private void OnShotStarted(InputAction.CallbackContext context)
    {
        var currentTouchPosition = _userInput.MainInput.StrikePosition.ReadValue<Vector2>();

        Vector3 worldTouchPosition = _camera.ScreenToWorldPoint(currentTouchPosition);

        Vector2 localTouchPosition = _inputRotationZone.InverseTransformPoint(worldTouchPosition);

        if (_inputRotationZone.rect.Contains(localTouchPosition))
        {
            _userInput.MainInput.StrikeVector.performed += OnStrikeVectorPerformed;
            _userInput.MainInput.Shot.canceled += OnShotCancelled;
        }
    }

    private void OnShotCancelled(InputAction.CallbackContext context)
    {
        _userInput.MainInput.StrikeVector.performed -= OnStrikeVectorPerformed;
        _userInput.MainInput.Shot.canceled -= OnShotCancelled;

        Vector3 dropVector = GetVector();
        _vector = Vector3.zero;
        Dropped?.Invoke(dropVector);
    }

    private void OnStrikeVectorPerformed(InputAction.CallbackContext context)
    {
        AndroidProcessor(context.ReadValue<Vector2>());
    }

    private void AndroidProcessor(Vector3 direction)
    {
        if (direction.Equals(_vector) == false)
        {
            _vector = GetMouseVector(direction);
            Changed?.Invoke(GetVector());
        }
    }

    private void DesktopProcessor(Vector3 old)
    {
        _vector = GetMouseVector(_userInput.MainInput.StrikeVector.ReadValue<Vector2>());

        if (old.Equals(_vector) == false)
            Changed?.Invoke(GetVector());
    }
}
