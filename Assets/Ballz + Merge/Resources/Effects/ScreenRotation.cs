using UnityEngine;
using Zenject;

public class ScreenRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 0.75f;
    [SerializeField] private Vector2 _rotationLimits = new Vector2(-45f, 45f);
    [SerializeField] private float _idleTime = 0.3f;

    [Inject] private MainInputMap _userInput;

    private Vector2 _currentRotation;
    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private float _idleTimer;
    private bool _isAccelAvailable;

    private void Start()
    {
        _startRotation = Quaternion.Euler(Vector3.zero);
        _isAccelAvailable = SystemInfo.supportsAccelerometer;
        Debug.Log("Accelerometer supported: " + SystemInfo.supportsAccelerometer);
    }

    private void Update()
    {
        Vector2 rotationDelta = _userInput.MainInput.ScreenRotation.ReadValue<Vector2>();

        if (rotationDelta.sqrMagnitude > 0.01f)
        {
            _idleTimer = 0;
            _currentRotation.x -= rotationDelta.y * _rotationSpeed * Time.deltaTime;
            _currentRotation.y += rotationDelta.x * _rotationSpeed * Time.deltaTime;

            _currentRotation.x = Mathf.Clamp(_currentRotation.x, _rotationLimits.x, _rotationLimits.y);
            _currentRotation.y = Mathf.Clamp(_currentRotation.y, _rotationLimits.x, _rotationLimits.y);

            transform.eulerAngles = _currentRotation;
        }
        /*else if (_isAccelAvailable)
        {
            Vector3 accel = Input.acceleration;
            Debug.Log($"Accel: {accel.x:F3}, {accel.y:F3}, {accel.z:F3}");

            float xRot = Mathf.Clamp(accel.y * 90f, -90f, 90f);
            float yRot = Mathf.Clamp(accel.x * 90f, -90f, 90f);

            _targetRotation = Quaternion.Euler(xRot, -yRot, 0f);

            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 5f);
        }*/
        else
        {
            _idleTimer += Time.deltaTime;

            if (_idleTimer >= _idleTime)
                ResetRotation();
        }
    }

    private void ResetRotation()
    {
        _currentRotation = Vector3.Lerp(_currentRotation, _startRotation.eulerAngles, _rotationSpeed * Time.deltaTime);
        transform.eulerAngles = _currentRotation;
    }
}
