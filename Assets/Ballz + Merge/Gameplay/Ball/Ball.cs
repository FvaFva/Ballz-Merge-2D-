using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.Gameplay.BallSpace
{
    public class Ball : CyclicBehavior, ILevelLoader, IInitializable
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
        private Dictionary<BallState, Action> _enterStates;
        private Dictionary<BallState, Action> _leftStates;

        public Vector2 Position => _transform.position;
        public event Action EnterGame;
        public event Action LeftGame;
        public event Action EnterAIM;
        public event Action LeftAIM;
        public event Action EnterAwait;
        public event Action LeftAwait;

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

            _enterStates = new Dictionary<BallState, Action>
            {
                {_inGame, ()=>{ EnterGame?.Invoke(); }},
                {_aim, ()=>{ EnterAIM?.Invoke(); }},
                {_inAwait, ()=>{ EnterAwait?.Invoke(); }},
                {_simulation, ()=>{} }
            };

            _leftStates = new Dictionary<BallState, Action>
            {
                {_inGame, ()=>{ LeftGame?.Invoke(); }},
                {_aim, ()=>{ LeftAIM?.Invoke(); }},
                {_inAwait, ()=>{ LeftAwait?.Invoke(); }},
                {_simulation, ()=>{ } }
            };
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
                _leftStates[_current]();
            }

            _current = newState;

            if (_current != null)
            {
                _current.TargetAchieved += OnTargetStateReached;
                _current.Enter();
                _enterStates[_current]();
            }
        }
    }
}