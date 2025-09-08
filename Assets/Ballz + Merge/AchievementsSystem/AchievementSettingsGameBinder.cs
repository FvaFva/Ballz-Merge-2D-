using BallzMerge.Root;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementSettingsGameBinder : CyclicBehavior, IInitializable, ILevelCompleter, IDependentSettings
    {
        private const string Message = "New Achievement!";

        [Inject] private DiContainer _container;
        [Inject] private AchievementsBus _bus;
        [Inject] private UIRootView _rootView;
        [Inject] private LevelSettingsMap _map;

        private PopupDisplayer _displayer;
        private AchievementObserverLevelsCompleted _levelCompletedObserver;
        private AchievementObserverIncreaserCompleted _increaserCompletedObserver;
        private AchievementObserverBandlerCompleted _bandlerCompletedObserver;
        private List<AchievementObserverBase> _observers = new List<AchievementObserverBase>();
        private int _levelID;

        private void Awake()
        {
            _displayer = _rootView.PopupsDisplayer;
        }

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

        public void ApplySettings(LevelSettings settings)
        {
            _levelID = settings.ID;
        }

        public void Complete()
        {
            _levelCompletedObserver.OnLevelCompleted();
            _increaserCompletedObserver.OnIncreaserCompleted();
            _bandlerCompletedObserver.OnBandlerCompleted();
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
                    case AchievementsTypes.levelComplete:
                        CreateLevelCompleteObserver(setting);
                        break;
                    case AchievementsTypes.increaserComplete:
                        _increaserCompletedObserver = CreateObserver<AchievementObserverIncreaserCompleted>(setting);
                        break;
                    case AchievementsTypes.bandlerComplete:
                        _bandlerCompletedObserver = CreateObserver<AchievementObserverBandlerCompleted>(setting);
                        break;

                }
            }
        }

        private void CreateLevelCompleteObserver(KeyValuePair<AchievementSettings, AchievementPointsStep> setting)
        {
            List<int> steps = new List<int>();
            steps.Add(_map.Available.Count - (_map.Available.Count - 1));
            steps.Add(_map.Available.Count / 2);
            steps.Add(_map.Available.Count);
            setting.Key.SetSteps(steps);
            _levelCompletedObserver = CreateObserver<AchievementObserverLevelsCompleted>(setting);
            _levelCompletedObserver.SetCurrentLevelID(_levelID);
        }

        private T CreateObserver<T>(KeyValuePair<AchievementSettings, AchievementPointsStep> setting) where T : AchievementObserverBase
        {
            var newObserver = _container.Instantiate<T>(new object[] { setting.Key, setting.Value });
            _observers.Add(newObserver);
            newObserver.Construct();
            newObserver.ChangedPoints += OnPointsChanged;
            newObserver.ReachedStep += OnStepReached;
            newObserver.ReachedAchievement += OnReachedAchievement;
            return newObserver;
        }

        private void OnPointsChanged(AchievementsTypes type, int points)
        {
            _bus.AddPoints(type, points);
        }

        private void OnStepReached(AchievementsTypes type, AchievementPointsStep pointsStep, AchievementData achievementData)
        {
            _bus.AddSteps(type, pointsStep);
            _displayer.ShowPopup(achievementData, pointsStep.Step);
        }

        private void OnReachedAchievement(AchievementsTypes type, AchievementData achievementData)
        {
            _bus.ReachAchievement(type);
            _displayer.ShowPopup(achievementData, message: Message);
        }
    }
}