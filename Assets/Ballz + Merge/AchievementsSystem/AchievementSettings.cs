using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "New Achievement settings", menuName = "Bellz+Merge/Achievements/AchievementSettings", order = 51)]
public class AchievementSettings : ScriptableObject
{
    [SerializeField] private List<int> _targets;
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _image;

    public int MaxTargets => _targets.Count;

    public event Action Completed;

    public int CheckReachedTarget(int target)
    {
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == target)
            {
                CheckDestinationTarget(i + 1);
                return i + 1;
            }
        }

        return 0;
    }

    private void CheckDestinationTarget(int target)
    {
        if (target == _targets.Count)
            Completed?.Invoke();
    }
}
