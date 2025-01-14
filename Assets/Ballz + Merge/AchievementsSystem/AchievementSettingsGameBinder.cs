using System.Collections.Generic;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementSettingsGameBinder : CyclicBehavior, IInitializable
    {
        [Inject] private DiContainer _container;
        [Inject] private AchievementsBus _bus;

        private List<AchievementObserverBase> _observers = new List<AchievementObserverBase>();

        private void OnEnable()
        {
            foreach (var observer in _observers)
            {
                observer.ReachedStep += OnSteReached;
                observer.ChangedPoints += OnPointsChanged;
            }
        }

        private void OnDisable()
        {
            foreach (var observer in _observers)
            {
                observer.ReachedStep -= OnSteReached;
                observer.ChangedPoints -= OnPointsChanged;
            }
        }

        public void Init()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
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
            newObserver.ReachedStep += OnSteReached;
        }

        private void OnPointsChanged(AchievementsTypes type, int points)
        {
            _bus.AddPoints(type, points);
        }

        private void OnSteReached(AchievementsTypes type, AchievementPointsStep pointsStep)
        {
            _bus.Apply(type, pointsStep);
        }
    }
}