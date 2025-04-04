using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BallSpace
{
    public class BallVectorPredictor : CyclicBehavior, IInitializable
    {
        private const float StepTime = 0.02f;
        private const int MaxStepCount = 1200;

        [SerializeField] private VirtualWorldFactory _factory;

        [Inject] private Ball _ballOriginal;

        private PhysicsScene2D _physicsScene;
        private BallStrikeVectorReader _vectorReader;
        private BallSimulation _ballSimulated;

        public event Action<IEnumerable<Vector3>> Predicted;

        private void OnEnable()
        {
            if (_vectorReader != null)
            {
                _vectorReader.Changed += Predict;
                _vectorReader.Canceled += OnCanceled;
            }
        }

        private void OnDisable()
        {
            if (_vectorReader != null)
            {
                _vectorReader.Changed -= Predict;
                _vectorReader.Canceled -= OnCanceled;
            }
        }

        public void Init()
        {
            _vectorReader = _ballOriginal.GetBallComponent<BallStrikeVectorReader>();
            _vectorReader.Changed += Predict;
            _vectorReader.Canceled += OnCanceled;
            _ballSimulated = _factory.CreateBall(_ballOriginal);
            _physicsScene = _factory.GetPhysicScene();
        }

        private void Predict(Vector3 direction)
        {
            List<Vector3> prediction = new List<Vector3>();
            _ballSimulated.Restart(_ballOriginal.Position, direction);
            float simulationSteps = MaxStepCount;

            while (_ballSimulated.CollisionsLeft > 0 && --simulationSteps > 0)
            {
                _physicsScene.Simulate(StepTime);
                prediction.Add(_ballSimulated.Position);
            }

            Predicted?.Invoke(prediction);
        }

        private void OnCanceled()
        {
            Predicted?.Invoke(new List<Vector3>());
        }
    }
}
