using BallzMerge.Gameplay.Level;
using BallzMerge.Root;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Project injection", menuName = "Bellz+Merge/Injection/ProjectInjection", order = 51)]
public class ProjectInjection : ScriptableObjectInstaller
{
    [SerializeField] private GridSettings _gridSettings;
    [SerializeField] private LevelSettingsContainer _levelSettings;
    [SerializeField] private LevelSettingsMap _levelMap;

    public override void InstallBindings()
    {
        Container.Bind<GridSettings>().FromInstance(_gridSettings).AsSingle().NonLazy();
        Container.Bind<LevelSettingsContainer>().FromInstance(_levelSettings).AsSingle().NonLazy();
        Container.Bind<LevelSettingsMap>().FromInstance(_levelMap).AsSingle().NonLazy();
        Container.Bind<TargetSceneEntryPointContainer>().FromNew().AsSingle().NonLazy();
    }
}