using UnityEngine;
using Zenject;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;

public class MainSceneInjection : MonoInstaller
{
    [Header("Project context bind")]
    [SerializeField] private GameCycler _loader;

    [Header("Bind")]
    [SerializeField] private Ball _ball;
    [SerializeField] private PhysicGrid _physicsGrid;
    [SerializeField] private UserQuestioner _questioner;
    [SerializeField] private BlocksBus _blocksBus;
    [SerializeField] private BallWaveVolume _ballLevelVolume;

    [Header("Factory")]
    [SerializeField] private Block _blockPrefab;

    [Header("Inject")]
    [SerializeField] private BlocksSpawner _blocksSpawner;
    [SerializeField] private PlayerScore _score;

    public override void InstallBindings()
    {
        Container.Bind<MainInputMap>().FromNew().AsSingle().NonLazy();

        Container.Bind<Ball>().FromInstance(_ball).AsSingle().NonLazy();
        Container.Bind<PhysicGrid>().FromInstance(_physicsGrid).AsSingle().NonLazy();
        Container.Bind<UserQuestioner>().FromInstance(_questioner).AsSingle().NonLazy();
        Container.Bind<BlocksBus>().FromInstance(_blocksBus).AsSingle().NonLazy();
        Container.Bind<BallWaveVolume>().FromInstance(_ballLevelVolume).AsSingle().NonLazy();

        Container.Bind<Block>().FromComponentInNewPrefab(_blockPrefab).AsTransient();

        Container.InjectGameObject(_blocksSpawner.gameObject);
        Container.InjectGameObject(_score.gameObject);

        ProjectContext.Instance.Container.Unbind<GameCycler>();
        ProjectContext.Instance.Container.Bind<GameCycler>().FromInstance(_loader).NonLazy();
    }
}