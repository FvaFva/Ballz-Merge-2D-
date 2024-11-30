using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ScreenInteraction : MonoBehaviour
{
    [SerializeField] private EffectsPool _effectsPool;

    [Inject] private MainInputMap _userInput;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _userInput.MainInput.ScreenInteract.performed += OnScreenInteract;
    }

    private void OnDisable()
    {
        _userInput.MainInput.ScreenInteract.performed -= OnScreenInteract;
    }

    private void OnScreenInteract(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = _userInput.MainInput.StrikePosition.ReadValue<Vector2>();
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -10f));
        _effectsPool.GetEffect(worldPosition, Quaternion.identity);
    }
}
