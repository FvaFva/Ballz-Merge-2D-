using Zenject;
using UnityEngine;
using BallzMerge.Gameplay.BallSpace;

public class BallGameZoneLeaverHandler : BallComponent
{
    [SerializeField] private BallCollisionHandler _collisionHandler;

    [Inject] private BallWaveVolume _ballVolume;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
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

        MyBody.velocity = Vector2.zero;
        _transform.position += new Vector3(0, 0.1f);
        ActivateTrigger();
    }
}