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
        [SerializeField] private AchievementKyes _iD;

        public int MaxTargets => _steps.Count;
        public string Name => _name;
        public AchievementKyes ID => _iD;

        public int CheckReachedNewSteps(AchievementPointsStep pointsStep)
        {
            int stepsReached = 0;

            for (int i = pointsStep.Step; i < _steps.Count; i++)
            {
                if (_steps[i] >= pointsStep.Points)
                    stepsReached ++;
            }

            return stepsReached;
        }
    }
}