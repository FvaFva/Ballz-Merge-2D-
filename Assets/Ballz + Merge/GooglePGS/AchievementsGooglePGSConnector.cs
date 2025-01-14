using GooglePlayGames;
using System;
using System.Linq;
using UnityEngine;

namespace BallzMerge.GooglePGS
{
    using Achievement;

    public class AchievementsGooglePGSConnector
    {
        [SerializeField] private Authenticator _authenticator;

        private bool _isReadyToWork;

        private void Awake()
        {
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

        public void IssueAchievement(AchievementKyes achievement, Action<bool> callback)
        {
            if (_isReadyToWork == false)
                return;

            PlayGamesPlatform.Instance.ReportProgress(achievement.Google, 100, callback);
        }

        public void RequirePlayerAchievementsData(Action<(string, bool)[]> callback)
        {
            if (_isReadyToWork == false)
                return;

            PlayGamesPlatform.Instance.LoadAchievements(achievements =>
            {
                callback?.Invoke(achievements.Select(achievement => (achievement.id, achievement.completed)).ToArray());
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
                    Debug.Log($"{achievement.id} {achievement.points}");
            });
        }
    }
}
