using GooglePlayGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallzMerge.GooglePGS
{
    using Achievement;

    public class AchievementsConnector : AchievementsProcessorBase
    {
        [SerializeField] private Authenticator _authenticator;

        private bool _isReadyToWork;
        private Dictionary<string, AchievementData> _achievementDescriptions;

        private void Awake()
        {
            _achievementDescriptions = new Dictionary<string, AchievementData>();
            _isReadyToWork = _authenticator.IsAuthenticated;
        }

        private void OnEnable()
        {
            _authenticator.Authenticated += InitializeAchievementsDescriptions;
        }

        private void OnDisable()
        {
            _authenticator.Authenticated -= InitializeAchievementsDescriptions;
        }

        public override void IssueAchievement(Achievement achievement, Action<bool> callback)
        {
            if (_isReadyToWork == false)
                return;

            PlayGamesPlatform.Instance.ReportProgress(achievement.GoogleId, 100, callback);
        }

        public override void RequirePlayerAchievementsData(Action<AchievementData[]> callback)
        {
            if (_isReadyToWork == false)
                return;

            PlayGamesPlatform.Instance.LoadAchievements(achievements =>
            {
                callback?.Invoke(_achievementDescriptions.Join(achievements, achievementDescript => achievementDescript.Key, playerAchievement => playerAchievement.id, (achievementDescript, playerAchievement) => achievementDescript.Value).ToArray());
            });
        }

        private void InitializeAchievementsDescriptions()
        {
            _isReadyToWork = _authenticator.IsAuthenticated;

            if (_isReadyToWork == false)
                return;

            PlayGamesPlatform.Instance.LoadAchievementDescriptions(achievements =>
            {
                foreach (var achievement in achievements)
                    _achievementDescriptions.Add(achievement.id, new AchievementData(achievement));
            });
        }
    }
}
