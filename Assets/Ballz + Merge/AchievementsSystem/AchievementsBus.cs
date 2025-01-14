using BallzMerge.Data;
using BallzMerge.GooglePGS;
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

        private AchievementsGooglePGSConnector _google;
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
            _db.Achievement.AddPoints(internalKey, points);
        }

        public void Apply(AchievementsTypes internalKey, AchievementPointsStep data)
        {
            if (_db.Achievement.TryWrite(internalKey, data))
            {
                if (_isUsingGoogle)
                    Debug.Log("Google got achievement");

                Debug.Log("A new achievement was just written!");
            }
        }

        public IDictionary<AchievementSettings, AchievementPointsStep> GetSettings() =>
            _settings
            .Join(_db.Achievement.GetAll(), d1 => d1.ID.Internal, d2 => d2.Key, (d1, d2) => new { settings = d1, pointStep = d2.Value})
            .ToDictionary(x=> x.settings, x=>x.pointStep);
        
    }
}
