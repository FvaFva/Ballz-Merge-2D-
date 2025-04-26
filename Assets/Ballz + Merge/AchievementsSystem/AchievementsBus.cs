using BallzMerge.Data;
#if UNITY_ANDROID
using BallzMerge.GooglePGS;
#endif
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementsBus : MonoBehaviour
    {
        [SerializeField] private List<AchievementSettings> _settings;
        [Inject] private DataBaseSource _db;

#if UNITY_ANDROID
        private AchievementsGooglePGSConnector _google;
#endif
        private bool _isUsingGoogle;
        private bool _isUsingSteam;
        private bool _isUsingApple;
        private bool _isUsingUnity;

        public void ConnectGoogle()
        {
            Debug.Log("pu-pu-pum");
            _isUsingGoogle = true;
        }

        public void AddPoints(AchievementsTypes internalKey, int points)
        {
            _db.Achievements.AddPoints(internalKey, points);
        }

        public void AddSteps(AchievementsTypes internalKey, AchievementPointsStep data)
        {
            if (_db.Achievements.TryWrite(internalKey, data))
            {
                if (_isUsingGoogle)
                    Debug.Log("Google got achievement");

                Debug.Log($"A new achievement was just written! {internalKey} - {data.Points} of {data.Step}");
            }
            else
            {
                Debug.Log($"A new achievement cant written! {internalKey} - {data.Points} of {data.Step}");
            }
        }

        public void ReachAchievement(AchievementsTypes internalKey)
        {
            Debug.Log($"Achievement {internalKey} complete");
        }

        public IDictionary<AchievementSettings, AchievementPointsStep> GetSettings() =>
            _settings
            .GroupJoin(_db.Achievements.GetAll(), d1 => d1.ID.Internal, d2 => d2.Key, (d1, d2) => new { settings = d1, pointStep = d2.FirstOrDefault().Value })
            .ToDictionary(x=> x.settings, x=>x.pointStep);
        
    }
}
