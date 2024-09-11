using BallzMerge.Gameplay.Level;
using BallzMerge.Root;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Project injection", menuName = "Bellz+Merge/Injection/ProjectInjection", order = 51)]
public class ProjectInjection : ScriptableObjectInstaller
{
    [SerializeField] private GridSettings _gridSettings;
    [SerializeField] private BallVolumesMap _ballVolumesMap;

    public override void InstallBindings()
    {
        Container.Bind<GridSettings>().FromInstance(_gridSettings).AsSingle().NonLazy();
        Container.Bind<BallVolumesMap>().FromInstance(_ballVolumesMap).AsSingle().NonLazy();
        Container.Bind<TargetSceneEntryPointContainer>().FromNew().AsSingle().NonLazy();
        _ballVolumesMap.ReBuild();
    }
}