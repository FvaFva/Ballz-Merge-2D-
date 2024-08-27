using System;
using UnityEngine;
using Zenject;

public class BallStrikeVectorReader : BallComponent
{
    [SerializeField]private Camera _camera;

    [Inject] private MainInputMap _userInput;

    private Vector3 _vector;
    private Transform _transform;
    private float _cameraY;
    private Action<Vector3> _currentProcessor;

    public event Action<Vector3> Changed;
    public event Action<Vector3> Dropped;

    private void Awake()
    {
        _transform = transform;
        _cameraY = _camera.transform.position.y;
        _currentProcessor = DesktopProcessor;
    }

    private void Update()
    {
        Vector3 old = _vector;
        _currentProcessor(old);
    }

    public void ChangeToAndroid()
    {
        _currentProcessor = AndroidProcessor;
    }

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

    private void AndroidProcessor(Vector3 old)
    {
        _vector = _userInput.MainInput.StrikeVector.ReadValue<Vector2>();

        if (old.Equals(_vector) == false)
        {
            if (_vector == Vector3.zero)
            {
                _vector = old;
                Vector3 dropVector = GetVector();
                _vector = Vector3.zero;
                Dropped(dropVector);
            }
            else
            {
                Changed?.Invoke(GetVector());
            }
        }
    }

    private void DesktopProcessor(Vector3 old)
    {
        _vector = GetMouseVector(_userInput.MainInput.StrikeVector.ReadValue<Vector2>());

        if (old.Equals(_vector) == false)
            Changed?.Invoke(GetVector());
    }
}
