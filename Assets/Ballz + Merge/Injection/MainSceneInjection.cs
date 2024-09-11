using UnityEngine;
using Zenject;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using BallzMerge.Root;

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
    [SerializeField] private BallWaveVolumeView _waveVolumeView;

    [Inject] private TargetSceneEntryPointContainer _entryPointBinder;

    public override void InstallBindings()
    {
        Container.Bind<Ball>().FromInstance(_ball).AsSingle().NonLazy();
        Container.Bind<PhysicGrid>().FromInstance(_physicsGrid).AsSingle().NonLazy();
        Container.Bind<BlocksBus>().FromInstance(_blocksBus).AsSingle().NonLazy();
        Container.Bind<BallWaveVolume>().FromInstance(_ballLevelVolume).AsSingle().NonLazy();

        Container.Bind<Block>().FromComponentInNewPrefab(_blockPrefab).AsTransient();
        Container.Bind<ISceneEnterPoint>().To<GameCycler>().FromInstance(_loader).AsSingle().NonLazy();

        Container.Inject(_blocksSpawner);
        Container.Inject(_score);
        Container.Inject(_waveVolumeView);

        _entryPointBinder.Set(_loader);
    }
}