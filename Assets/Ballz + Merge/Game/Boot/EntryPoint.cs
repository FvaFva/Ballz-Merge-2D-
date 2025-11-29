using UnityEngine;
using Zenject;

namespace BallzMerge.Root
{
    using Achievement;
    using Settings;

    public class EntryPoint
    {
        private static EntryPoint _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Enter()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _instance = new EntryPoint();
            _instance.RunGame();
        }

        private UIRootView _rootView;
        private SceneLoader _sceneLoader;
        private GameSettings _gameSettings;
        private OwnerPrimaryComponents _primary;

        private EntryPoint()
        {
            _primary = new OwnerPrimaryComponents();
            BindToContainerPrimaryComponents();
            InitMinorComponents();
            BindToContainerMinorComponents();
        }

        private void RunGame()
        {
            string sceneName = ScenesNames.MAIN_MENU;

            PlatformRunner.Run(null,
            editorAction: () =>
            {
                var checker = new DebugScenesChecker();

                if (checker.IsItDebug(ref sceneName, _primary.Hub.Get<DevelopersScenes>()))
                    return;
            });

            LoadScene(sceneName);
        }

        private void LoadScene(string targetScene)
        {
            _rootView.ClearSceneUI();
            _primary.Coroutines.StartCoroutine(_sceneLoader.LoadScene(targetScene));
        }

        private void QuitGame()
        {
            _primary.UserInput.Disable();

            void EditorAction() => PlatformRunner.QuitPlayMode();
            void NonEditorAction() => Application.Quit();
            PlatformRunner.Run(NonEditorAction, editorAction : EditorAction);
        }

        private void InitMinorComponents()
        {
            _rootView = GenerateDontDestroyFromHub<UIRootView>();
            _rootView.Containers.Init();
            _primary.OrientationObserver.CheckInRoot(_rootView.Containers);
            _primary.OrientationObserver.CheckInRoot(_rootView.UIReorganizer);
            GlobalEffects globalEffects = GenerateDontDestroyFromHub<GlobalEffects>();
            _gameSettings = new GameSettings(_rootView.SettingsMenu, _primary, _rootView.InfoPanelShowcase, _rootView.Questioner, globalEffects);
            _sceneLoader = new SceneLoader(_rootView.LoadScreen, _rootView.UIReorganizer, _rootView.UpdateViewButtons, SceneExitCallBack, _gameSettings, _primary.OrientationObserver);
            var volumeMap = _primary.Hub.Get<BallVolumesMap>();
            volumeMap.ReBuild();
            BindSingleton(volumeMap);
        }

        private void BindToContainerPrimaryComponents()
        {
            BindSingleton<IGameTimeOwner, TimeScaler>(_primary.TimeScaler);
            BindSingleton(_primary.Data);
            BindSingleton(_primary.UserInput);
            BindSingleton(new EffectPool());
            BindSingleton(GenerateDontDestroyFromHub<AchievementsBus>());
        }

        private void BindToContainerMinorComponents()
        {
            BindSingleton(_rootView.Questioner);
            BindSingleton(_rootView);
            BindSingleton(_rootView.InfoPanelShowcase);
            BindSingleton(_gameSettings.SoundVolumeGlobal);
        }

        private T GenerateDontDestroyFromHub<T>() where T : Component
        {
            T prefab = _primary.Hub.Get<T>();
            T instance = ProjectContext.Instance.Container.InstantiatePrefabForComponent<T>(prefab);
            instance.transform.SetParent(null);
            Object.DontDestroyOnLoad(instance.gameObject);
            return instance;
        }

        private void BindSingleton<TSingleton>(TSingleton singleton) => ProjectContext.Instance.Container.Bind<TSingleton>().FromInstance(singleton).AsSingle().NonLazy();
        private void BindSingleton<TSingletonInterface, TSingletonRealization>(TSingletonRealization singleton) where TSingletonRealization : TSingletonInterface => ProjectContext.Instance.Container.Bind<TSingletonInterface>().To<TSingletonRealization>().FromInstance(singleton).AsSingle().NonLazy();

        private void SceneExitCallBack(SceneExitData exitData)
        {
            if (exitData.Save != null)
                _primary.Saver.SaveGame(exitData.Save);

            if (exitData.History.IsEmpty() == false)
                _primary.Data.History.SaveResult(exitData.History);

            if (exitData.IsLoad)
                _sceneLoader.SetLoad(_primary.Data.Saves.CheckSaves());

            if (exitData.IsGameQuit)
                QuitGame();
            else if (string.Empty.Equals(exitData.TargetScene) == false && exitData.TargetScene != null)
                LoadScene(exitData.TargetScene);
        }
    }
}