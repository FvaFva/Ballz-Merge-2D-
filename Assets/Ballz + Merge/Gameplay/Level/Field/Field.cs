using UnityEngine;

namespace BallzMerge.Gameplay.Level.Field
{
    public class Field : CyclicBehavior, IWaveUpdater
    {
        [SerializeField] private ParticleSystem _field;
        [SerializeField] private int _particlesForLevel;

        private ParticleSystem.EmissionModule _emission;
        private ParticleSystem.MinMaxCurve _curve;

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
    }
}
