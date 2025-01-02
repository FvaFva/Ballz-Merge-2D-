using UnityEngine;
using Zenject;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using BallzMerge.Root;
using R3;

public class MainSceneInjection : MonoInstaller
{
    [Header("Project context bind")]
    [SerializeField] private GameCycler _loader;

    [Header("Bind")]
    [SerializeField] private Ball _ball;
    [SerializeField] private PhysicGrid _physicsGrid;
    [SerializeField] private BlocksBus _blocksBus;
    [SerializeField] private BallWaveVolume _ballLevelVolume;

    [Header("Factory")]
    [SerializeField] private Block _blockPrefab;

    [Header("Inject")]
    [SerializeField] private BlocksSpawner _blocksSpawner;
    [SerializeField] private PlayerScore _score;
    [SerializeField] private GameAnimationSkipper _animationSkipper;
    [SerializeField] private AchievementSettingsBinder _achievementSource;

    [Inject] private TargetSceneEntryPointContainer _entryPointBinder;

    public override void InstallBindings()
    {
        Container.Bind<Ball>().FromInstance(_ball.PreLoad()).AsSingle().NonLazy();
        Container.Bind<PhysicGrid>().FromInstance(_physicsGrid).AsSingle().NonLazy();
        Container.Bind<BlocksBus>().FromInstance(_blocksBus).AsSingle().NonLazy();
        Container.Bind<BallWaveVolume>().FromInstance(_ballLevelVolume).AsSingle().NonLazy();
        Container.Bind<AchievementSettingsBinder>().FromInstance(_achievementSource).AsSingle().NonLazy();
 
        Container.Bind<BlocksInGame>().FromNew().AsSingle().NonLazy();
        Container.Bind<Block>().FromComponentInNewPrefab(_blockPrefab).AsTransient();
        Container.Bind<ISceneEnterPoint>().To<GameCycler>().FromInstance(_loader).AsSingle().NonLazy();

        Container.Inject(_blocksSpawner);
        Container.Inject(_score);
        Container.Inject(_animationSkipper);

        _entryPointBinder.Set(_loader);
        Container.Bind<Subject<Unit>>().FromInstance(new Subject<Unit>()); // AppConstants.EXIT_SCENE_REQUEST_TAG

    }
}