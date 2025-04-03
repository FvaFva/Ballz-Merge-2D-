using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ScreenInteraction : MonoBehaviour
{
    [SerializeField] private InteractionEffect _effectPrefab;

    [Inject] private MainInputMap _userInput;
    [Inject] private EffectPool _effectPool;

    private void OnEnable() => _userInput.MainInput.ScreenInteract.performed += OnScreenInteract;

    private void OnDisable() => _userInput.MainInput.ScreenInteract.performed -= OnScreenInteract;

    private void OnScreenInteract(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = _userInput.MainInput.StrikePosition.ReadValue<Vector2>();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        _effectPool.SpawnEffect(worldPosition, Quaternion.identity, _effectPrefab, gameObject.transform);
    }
}
