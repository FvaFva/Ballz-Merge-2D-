using UnityEngine;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementSettingsBinder : CyclicBehavior, IInitializable
    {
        [SerializeField] private AchievementSettings _blocksDestroyerSettings;
        [SerializeField] private AchievementSettings _waveSpawnedSettings;
        [SerializeField] private AchievementSettings _chainHitBlocksSettings;

        [Inject] private DiContainer _container;

        private AchievementObserverBlocksDestroyer _blocksDestroyer;
        private AchievementObserverWaveSpawned _waveSpawned;
        private AchievementObserverChainHitBlocks _chainHitBlocks;

        public void Init()
        {
            InstantiateSettings();
            Construct();
        }

        private void InstantiateSettings()
        {
            _blocksDestroyer = _container.Instantiate<AchievementObserverBlocksDestroyer>(new object[] { _blocksDestroyerSettings });
            _waveSpawned = _container.Instantiate<AchievementObserverWaveSpawned>(new object[] { _waveSpawnedSettings });
            _chainHitBlocks = _container.Instantiate<AchievementObserverChainHitBlocks>(new object[] { _chainHitBlocksSettings });
        }

        private void Construct()
        {
            _blocksDestroyer.Construct();
            _waveSpawned.Construct();
            _chainHitBlocks.Construct();
        }
    }
}