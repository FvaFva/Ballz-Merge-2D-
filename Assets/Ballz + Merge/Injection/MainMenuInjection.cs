using BallzMerge.Root;
using UnityEngine;
using Zenject;

namespace BallzMerge.MainMenu
{
    public class MainMenuInjection : MonoInstaller
    {
        [SerializeField] private MainMenuEntryPoint _entryPoint;
        [SerializeField] private LevelSelector _levelSelector;

        [Inject] private TargetSceneEntryPointContainer _entryPointBinder;

        public override void InstallBindings()
        {
            Container.Bind<ISceneEnterPoint>().To<MainMenuEntryPoint>().FromInstance(_entryPoint).AsSingle().NonLazy();
            Container.Inject(_levelSelector);
            _entryPointBinder.Set(_entryPoint);
        }
    }
}