using UnityEngine;
using BallzMerge.Gameplay.BallSpace;

public class BallGameZoneLeaverHandler : BallComponent
{
    [SerializeField] private BallCollisionHandler _collisionHandler;
    [SerializeField] private BallWaveVolume _ballVolume;

    private readonly Vector3 _shift = new Vector3(0, 0.1f);
    private Transform _transform;
    private int _countBounce = 0;

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

    public override void ChangeActivity(bool state)
    {
        base.ChangeActivity(state);

        if (state)
            _countBounce = _ballVolume.GetPassiveValue<BallVolumeBounceField>();
    }

    private void HandlePlayZoneLeaving()
    {
        if (_countBounce-- != 0)
            return;

        MyBody.linearVelocity = Vector2.zero;
        _transform.position += _shift;
        ActivateTrigger();
    }
}