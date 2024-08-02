using UnityEngine;

public class BallHitImpact : BallComponent
{
    [SerializeField] private BallCollisionHandler _collisionHandler;
    [SerializeField] private AudioSourceHandler _audio;
    [SerializeField] private ParticleSystem _explosion;
    [SerializeField] private BallView _view;

    private void OnEnable()
    {
        _collisionHandler.Hit += OnHit;
    }

    private void OnDisable()
    {
        _collisionHandler.Hit -= OnHit;
    }

    private void OnHit(Vector2 hitPosition)
    {
        _explosion.Play();
        _audio.Play();
        _view.ShowHit(hitPosition);
    }
}
