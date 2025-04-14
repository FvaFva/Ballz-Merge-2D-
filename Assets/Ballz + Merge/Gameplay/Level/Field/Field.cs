using Unity.VisualScripting;
using UnityEngine;

namespace BallzMerge.Gameplay.Level.Field
{
    public class Field : CyclicBehavior, IWaveUpdater, ILevelStarter, IInitializable
    {
        [SerializeField] private ParticleSystem _field;
        [SerializeField] private int _particlesForLevel;

        private ParticleSystem.EmissionModule _emission;
        private ParticleSystem.MinMaxCurve _curve;

        private float _baseParticles;

        private void Awake()
        {
            _emission = _field.emission;
            _curve = _emission.rateOverTime;
        }

        public void UpdateWave()
        {
            _curve = new ParticleSystem.MinMaxCurve(_curve.constant += _particlesForLevel);
            _emission.rateOverTime = _curve;
        }

        public void StartLevel()
        {
            SetRate(_baseParticles);
        }

        public void Init()
        {
            _baseParticles = _curve.constant;
        }

        private void SetRate(float newRate)
        {
            _curve = new ParticleSystem.MinMaxCurve(newRate);
            _emission.rateOverTime = _curve;
        }
    }
}
