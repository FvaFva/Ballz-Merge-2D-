using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Achievement
{
    [CreateAssetMenu(fileName = "New Achievement settings", menuName = "Bellz+Merge/Achievements/AchievementSettings", order = 51)]
    public class AchievementSettings : ScriptableObject
    {
        [SerializeField] private List<int> _steps;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _image;
        [SerializeField] private AchievementKey _iD;

        public int MaxTargets => _steps.Count;
        public string Name => _name;
        public string Description => _description;
        public Sprite Image => _image;
        public AchievementKey ID => _iD;

        public bool CheckReachedNewSteps(int points, ref int step)
        {
            for (int i = step; i < _steps.Count; i++)
            {
                if (points >= _steps[i])
                {
                    step++;
                    return true;
                }
            }

            return false;
        }

        public bool CheckReachedAchievement(int step) => step == _steps.Count - 1;
    }
}