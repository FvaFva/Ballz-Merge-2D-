using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class BallShooter : BallComponent
{
    private const float MinSqrMagnitude = 0.001f;

    [SerializeField] private float _force;
    [SerializeField] private BallStrikeVectorReader _vectorReader;

    [Inject] private MainInputMap _userInput;

    private bool _isOverUI;

    private void OnEnable()
    {
        _userInput.MainInput.Shot.performed += OnPlayerShootOrder;
        _vectorReader.Dropped += OnInputVectorDrop;
    }

    private void OnDisable()
    {
        _userInput.MainInput.Shot.performed -= OnPlayerShootOrder;
        _vectorReader.Dropped -= OnInputVectorDrop;
    }

    private void Update()
    {
        _isOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    private void OnPlayerShootOrder(InputAction.CallbackContext ctx)
    {
        if (_isOverUI)
            return;

        Shot(_vectorReader.GetVector());
    }

    private void OnInputVectorDrop(Vector3 dropVector)
    {
        if (dropVector.sqrMagnitude <= MinSqrMagnitude)
            Shot(Vector3.up);
        else
            Shot(dropVector);
    }

    private void Shot(Vector3 vector)
    {
        MyBody.AddForce(vector * _force, ForceMode2D.Force);
        ActivateTrigger();
    }
}
