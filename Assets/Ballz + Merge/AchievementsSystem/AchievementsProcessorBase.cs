using System;
using UnityEngine;

namespace BallzMerge.Achievement
{
    public abstract class AchievementsProcessorBase : MonoBehaviour
    {
        public abstract void IssueAchievement(Achievement achievement, Action<bool> callback);
        public abstract void RequirePlayerAchievementsData(Action<AchievementData[]> callback);
    }
}
