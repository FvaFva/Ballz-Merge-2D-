using System;
using System.Collections.Generic;
using UnityEngine;


namespace BallzMerge.Achievement
{
    [CreateAssetMenu(fileName = "New Achievement settings", menuName = "Bellz+Merge/Achievements/AchievementSettings", order = 51)]
    public class AchievementSettings : ScriptableObject
    {
        [SerializeField] private List<int> _targets;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _image;
        [SerializeField] private AchievementIDs _iD;

        public int MaxTargets => _targets.Count;
        public string Name => _name;
        public AchievementIDs ID => _iD;

        public event Action Completed;

        public int CheckReachedTarget(int target, int current)
        {
            for (int i = current; i < _targets.Count; i++)
            {
                if (_targets[i] == target)
                    return i + 1;
            }

            return 0;
        }

        public void CheckDestinationTarget(int current)
        {
            if (current == _targets.Count)
                Completed?.Invoke();
        }
    }
}