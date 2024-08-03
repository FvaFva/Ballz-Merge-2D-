using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : CyclicBehaviour, ILevelFinisher, ILevelStarter, IInitializable
{
    [SerializeField] private BallState _simulation;
    [SerializeField] private BallState _aim;
    [SerializeField] private BallState _inGame;
    [SerializeField] private List<BallComponent> _components;

    private Transform _transform;
    private BallState _current;
    private Vector3 _start;

    public bool IsInAim { get; private set; }
    public Vector2 Position => _transform.position;
    public event Action EnterAim;

    private void Awake()
    {
        _transform = transform;
        _aim.SetTarget(_inGame);
        _inGame.SetTarget(_aim);
        _start = _transform.position;
    }

    public void Init()
    {
        ChangeState(null);
    }

    public void StartLevel()
    {
        ChangeState(_aim);
        _transform.position = _start;
    }

    public void FinishLevel()
    {
        ChangeState(null);
    }

    public void EnterSimulation()
    {
        if (_current == null)
            ChangeState(_simulation);
    }

    public T GetBallComponent<T>() where T : BallComponent
    {
        foreach(BallComponent component in _components)
        {
            if(component is T target)
                return target;
        }

        return null;
    }

    private void OnTargetStateReached(BallState newState)
    {
        ChangeState(newState);
    }

    private void ChangeState(BallState newState)
    {
        if(_current != null)
        {
            _current.TargetAchieved -= OnTargetStateReached;
            _current.Exit();
        }

        _current = newState;
        IsInAim = _current == _aim;

        if (IsInAim)
            EnterAim?.Invoke();

        if (_current != null)
        {
            _current.TargetAchieved += OnTargetStateReached;
            _current.Enter();
        }
    }
}