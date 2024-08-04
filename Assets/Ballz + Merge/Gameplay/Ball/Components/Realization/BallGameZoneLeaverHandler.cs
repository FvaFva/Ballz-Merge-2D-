using Zenject;
using UnityEngine;

public class BallGameZoneLeaverHandler : BallComponent
{
    [SerializeField] private BallCollisionHandler _collisionHandler;

    [Inject] private BallWaveVolume _ballVolume;

    private Rigidbody2D _rb;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _rb = GetRigidbody();
    }

    private void OnEnable()
    {
        _collisionHandler.GameZoneLeft += HandlePlayZoneLeaving;
    }

    private void OnDisable()
    {
        _collisionHandler.GameZoneLeft -= HandlePlayZoneLeaving;
    }

    private void HandlePlayZoneLeaving()
    {
        if (_ballVolume.CheckVolume(BallVolumesTypes.BotBounce))
            return;

        _rb.velocity = Vector2.zero;
        _transform.position += new Vector3(0, 0.1f);
        ActivateTrigger();
    }
}