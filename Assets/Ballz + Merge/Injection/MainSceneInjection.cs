using UnityEngine;
using Zenject;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using BallzMerge.Root;
using BallzMerge.Achievement;
using BallzMerge.Gameplay;

public class MainSceneInjection : MonoInstaller
{
    [Header("Project context bind")]
    [SerializeField] private GameCycler _loader;
    [SerializeField] private BallVolumesCageView _ballVolumesCageView;

    [Header("Container Bind")]
    [SerializeField] private Ball _ball;
    [SerializeField] private PhysicGrid _physicsGrid;
    [SerializeField] private BlocksBinder _blocksBus;
    [SerializeField] private BallWaveVolume _ballLevelVolume;
    [SerializeField] private AchievementSettingsGameBinder _achievementSource;
    [SerializeField] private CamerasOperator _operator;
    [SerializeField] private BallVolumeInventoryActivator _ballVolumeCarrierActivator;

    [Inject] private TargetSceneEntryPointContainer _entryPointBinder;

    public override void InstallBindings()
    {
        Container.Bind<Ball>().FromInstance(_ball.PreLoad()).AsSingle().NonLazy();
        Container.Bind<PhysicGrid>().FromInstance(_physicsGrid).AsSingle().NonLazy();
        Container.Bind<BlocksBinder>().FromInstance(_blocksBus).AsSingle().NonLazy();
        Container.Bind<BallWaveVolume>().FromInstance(_ballLevelVolume).AsSingle().NonLazy();
        Container.Bind<AchievementSettingsGameBinder>().FromInstance(_achievementSource).AsSingle().NonLazy();
        Container.Bind<CamerasOperator>().FromInstance(_operator).AsSingle().NonLazy();
        Container.Bind<ISceneEnterPoint>().To<GameCycler>().FromInstance(_loader).AsSingle().NonLazy();

        Container.Bind<BlocksInGame>().FromNew().AsSingle().NonLazy();
        Container.Bind<BlocksMover>().FromNew().AsSingle().NonLazy();
        
        Container.Inject(_ballVolumeCarrierActivator);

        ProjectContext.Instance.Container.Inject(_ballVolumesCageView);
        _entryPointBinder.Set(_loader);
    }
}