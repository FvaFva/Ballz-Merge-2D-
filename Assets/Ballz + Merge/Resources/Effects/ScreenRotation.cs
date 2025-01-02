using DG.Tweening;
using UnityEngine;
using Zenject;

public class ScreenRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 2.5f;
    [SerializeField] private Vector2 _rotationLimits = new Vector2(-45f, 45f);
    [SerializeField] private float _idleTime = 0.3f;

    [Inject] private MainInputMap _userInput;

    private Vector2 _currentRotation;
    private Quaternion _startRotation;
    private float _idleTimer;
    private Tween _resetTween;

    private void OnEnable()
    {
        _startRotation = transform.rotation;
    }

    private void OnDisable()
    {
        _resetTween?.Kill();
        _resetTween = null;
    }

    private void Update()
    {
        Vector2 mouseDelta = _userInput.MainInput.ScreenRotation.ReadValue<Vector2>();
        Debug.Log(mouseDelta);

        if (mouseDelta.sqrMagnitude > 0.01f)
        {
            if (_resetTween != null && _resetTween.IsActive())
                _resetTween.Kill();

            _idleTimer = 0;
            _currentRotation.x = Mathf.Clamp(_currentRotation.x - mouseDelta.y * _rotationSpeed * Time.deltaTime, _rotationLimits.x, _rotationLimits.y);
            _currentRotation.y += mouseDelta.x * _rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
        }
        else
        {
            _idleTimer += Time.deltaTime;

            if (_idleTimer >= _idleTime)
                ResetRotation();
        }
    }

    private void ResetRotation()
    {
        _resetTween = transform.DORotateQuaternion(_startRotation, 2f).OnComplete(() =>
        {
            _currentRotation = _startRotation.eulerAngles;
            _idleTimer = 0;
        });
    }
}
