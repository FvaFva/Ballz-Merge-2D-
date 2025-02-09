using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay.BallSpace
{
    public class Ball : CyclicBehavior, ILevelStarter, IInitializable
    {
        [SerializeField] private BallState _simulation;
        [SerializeField] private BallState _aim;
        [SerializeField] private BallState _inGame;
        [SerializeField] private BallState _inAwait;
        [SerializeField] private Rigidbody2D _myBody;

        private List<BallComponent> _components;
        private Transform _transform;
        private BallState _current;
        private Vector3 _start;
        private bool _loaded;

        public Vector2 Position => _transform.position;
        public event Action LeftGame;
        public event Action EnterGame;

        public Ball PreLoad()
        {
            if (_loaded == false)
            {
                _loaded = true;
                _transform = transform;
                _start = _transform.position;
                BuildStates();
                BuildComponents();
            }

            return this;
        }

        public void Init()
        {
            _aim.Exit();
            _simulation.Exit();
            _inGame.Exit();
            _inAwait.Exit();
            ChangeState(null);
        }

        public void StartLevel()
        {
            ChangeState(_aim);
            _transform.position = _start;
        }

        public void EnterSimulation()
        {
            if (_current != null)
                return;

            Init();
            ChangeState(_simulation);
        }

        public T GetBallComponent<T>() where T : BallComponent
        {
            foreach (BallComponent component in _components)
            {
                if (component is T target)
                    return target;
            }

            return null;
        }

        private void BuildStates()
        {
            _inAwait.SetTarget(_aim);
            _aim.SetTarget(_inGame);
            _inGame.SetTarget(_inAwait);
        }

        private void BuildComponents()
        {
            var partOfComponents = _aim.GetActiveComponents().Union(_inGame.GetActiveComponents());
            partOfComponents = partOfComponents.Union(_inAwait.GetActiveComponents());
            _components = partOfComponents.Union(_simulation.GetActiveComponents()).ToList();

            foreach (BallComponent component in _components)
                component.SetBody(_myBody);
        }

        private void OnTargetStateReached(BallState newState)
        {
            ChangeState(newState);
        }

        private void ChangeState(BallState newState)
        {
            if (_current != null)
            {
                _current.TargetAchieved -= OnTargetStateReached;
                _current.Exit();
            }

            if (_current == _inGame)
                LeftGame?.Invoke();

            _current = newState;

            if (_current == _inGame)
                EnterGame?.Invoke();

            if (_current != null)
            {
                _current.TargetAchieved += OnTargetStateReached;
                _current.Enter();
            }
        }
    }
}