using R3;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementSettingsGameBinder : CyclicBehavior, IInitializable
    {
        [SerializeField] private AchievementView _achievementView;
        [SerializeField] private RectTransform _achievementContainer;

        [Inject] private DiContainer _container;
        [Inject] private AchievementsBus _bus;

        private AchievementDisplayer _displayer;
        private List<AchievementObserverBase> _observers = new List<AchievementObserverBase>();

        private void OnEnable()
        {
            foreach (var observer in _observers)
            {
                observer.ReachedStep += OnStepReached;
                observer.ChangedPoints += OnPointsChanged;
                observer.ReachedAchievement += OnReachedAchievement;
            }
        }

        private void OnDisable()
        {
            foreach (var observer in _observers)
            {
                observer.ReachedStep -= OnStepReached;
                observer.ChangedPoints -= OnPointsChanged;
                observer.ReachedAchievement -= OnReachedAchievement;
            }
        }

        public void Init()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            _displayer = new AchievementDisplayer(_achievementView, _achievementContainer);

            foreach (var setting in _bus.GetSettings())
            {
                switch (setting.Key.ID.Internal)
                {
                    case AchievementsTypes.wavesSpawned:
                        CreateObserver<AchievementObserverWaveSpawned>(setting);
                        break;
                    case AchievementsTypes.chainsHits:
                        CreateObserver<AchievementObserverChainHitBlocks>(setting);
                        break;
                    case AchievementsTypes.blocksDestroy:
                        CreateObserver<AchievementObserverBlocksDestroyer>(setting);
                        break;
                }
            }
        }

        private void CreateObserver<T>(KeyValuePair<AchievementSettings, AchievementPointsStep> setting) where T : AchievementObserverBase
        {
            var newObserver = _container.Instantiate<T>(new object[] { setting.Key, setting.Value });
            _observers.Add(newObserver);
            newObserver.Construct();
            newObserver.ChangedPoints += OnPointsChanged;
            newObserver.ReachedStep += OnStepReached;
            newObserver.ReachedAchievement += OnReachedAchievement;
        }

        private void OnPointsChanged(AchievementsTypes type, int points)
        {
            _bus.AddPoints(type, points);
        }

        private void OnStepReached(AchievementsTypes type, AchievementPointsStep pointsStep, AchievementObserverBase observer)
        {
            _bus.AddSteps(type, pointsStep);
            _displayer.SpawnView(observer.Property.Name, observer.Property.Description, observer.Property.Image);
        }

        private void OnReachedAchievement(AchievementsTypes type, AchievementObserverBase observer)
        {
            _bus.ReachAchievement(type);
            _displayer.SpawnView(observer.Property.Name, observer.Property.Description, observer.Property.Image);
        }
    }
}