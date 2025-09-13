using System.Collections;
using UnityEngine;

namespace BallzMerge.Gameplay.BallSpace
{
    public class BallTroubleshooter : BallComponent
    {
        private const float DelaySeconds = 0.3f;

        [SerializeField] private int _maxHitsPerShoot;
        [SerializeField] private float _minForce;
        [SerializeField] private BallCollisionHandler _collisionHandler;
        [SerializeField] private ParticleSystem _collapseEffect;
        [SerializeField] private ParticleSystem _expandEffect;

        private int _restHits;
        private Vector3 _startPosition;
        private Transform _transform;
        private WaitForSeconds _delay;

        private void Awake()
        {
            _transform = transform;
            _delay = new WaitForSeconds(DelaySeconds);
        }

        private void OnEnable()
        {
            _collisionHandler.Hit += OnHit;
        }

        private void OnDisable()
        {
            _collisionHandler.Hit -= OnHit;
        }

        public override void ChangeActivity(bool isActive)
        {
            if (isActive)
            {
                _restHits = _maxHitsPerShoot;
                _startPosition = _transform.position;
            }

            base.ChangeActivity(isActive);
        }

        private void OnHit(Vector2 _)
        {
            if (--_restHits == 0 || MyBody.linearVelocity.sqrMagnitude < _minForce)
                StartCoroutine(GameExit());
        }

        private IEnumerator GameExit()
        {
            MyBody.linearVelocity = Vector3.zero;
            _collapseEffect.Play();
            yield return _delay;
            _transform.position = _startPosition;
            _expandEffect.Play();
            yield return _delay;
            ActivateTrigger();
        }
    }
}
