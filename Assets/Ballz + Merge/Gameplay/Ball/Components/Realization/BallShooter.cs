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
        if (Application.platform == RuntimePlatform.Android)
            _userInput.MainInput.Shot.performed += ctx => OnPlayerShootOrder();

        _vectorReader.Dropped += OnInputVectorDrop;
    }

    private void OnDisable()
    {
        _userInput.MainInput.Shot.performed -= ctx => OnPlayerShootOrder();
        _vectorReader.Dropped -= OnInputVectorDrop;
    }

    private void Update()
    {
        _isOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    private void OnPlayerShootOrder()
    {
        if (_isOverUI)
            return;

        Shot(_vectorReader.GetDirection());
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
