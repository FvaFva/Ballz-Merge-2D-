using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Project injection", menuName = "Bellz+Merge/Injection/ProjectInjection", order = 51)]
public class ProjectInjection : ScriptableObjectInstaller
{
    [SerializeField] private GridSettings _gridSettings;

    public override void InstallBindings()
    {
        Container.Bind<GridSettings>().FromInstance(_gridSettings).AsSingle().NonLazy();
    }
}