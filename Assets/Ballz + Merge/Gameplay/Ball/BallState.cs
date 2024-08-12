using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BallState
{
    [SerializeField] private List<BallComponent> _activeComponents;
    [SerializeField] private List<BallComponent> _triggers;
    
    private BallState _target;

    public event Action<BallState> TargetAchieved;

    public void SetTarget(BallState target)
    {
        if (_target == null)
            _target = target;
    }

    public void Enter()
    {
        foreach (BallComponent component in _activeComponents)
            component.ChangeActivity(true);

        foreach (BallComponent component in _triggers)
            component.Triggered += OnTriggered;
    }

    public void Exit() 
    {
        foreach (BallComponent component in _activeComponents)
            component.ChangeActivity(false);

        foreach (BallComponent component in _triggers)
            component.Triggered -= OnTriggered;
    }

    public IEnumerable<BallComponent> GetActiveComponents()
    {
        return _activeComponents.Union(_triggers);
    }

    private void OnTriggered()
    {
        TargetAchieved?.Invoke(_target);
    }
}