using UnityEngine;
using Zenject;

public class ScreenRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 0.5f;
    [SerializeField] private Vector2 _rotationLimits = new Vector2(-45f, 45f);
    [SerializeField] private float _idleTime = 0.3f;

    [Inject] private MainInputMap _userInput;

    private Vector2 _currentRotation;
    private Quaternion _startRotation;
    private float _idleTimer;
    private bool _isGyroAvailable;

    private void Start()
    {
        _startRotation = Quaternion.Euler(Vector3.zero);
        _isGyroAvailable = SystemInfo.supportsGyroscope;
    }

    private void Update()
    {
        Vector2 mouseDelta = _userInput.MainInput.ScreenRotation.ReadValue<Vector2>();

        if (mouseDelta.sqrMagnitude > 0.01f)
        {
            _idleTimer = 0;
            _currentRotation.x -= mouseDelta.y * _rotationSpeed * Time.deltaTime;
            _currentRotation.y += mouseDelta.x * _rotationSpeed * Time.deltaTime;

            _currentRotation.x = Mathf.Clamp(_currentRotation.x, _rotationLimits.x, _rotationLimits.y);
            _currentRotation.y = Mathf.Clamp(_currentRotation.y, _rotationLimits.x, _rotationLimits.y);

            transform.eulerAngles = _currentRotation;
        }
        else if (_isGyroAvailable)
        {
            Quaternion deviceRotation = Input.gyro.attitude;
            transform.rotation = new Quaternion(deviceRotation.x, deviceRotation.y, deviceRotation.z, deviceRotation.w);
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
        _currentRotation = Vector3.Lerp(_currentRotation, _startRotation.eulerAngles, _rotationSpeed * Time.deltaTime);
        transform.eulerAngles = _currentRotation;
    }
}
