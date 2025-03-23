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
#if UNITY_ANDROID && !UNITY_EDITOR
            SQLiteLoader.Init();
#endif
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
            string sceneName = ScenesNames.MAINMENU;
#if UNITY_EDITOR
            DebugScenesChecker checker = new DebugScenesChecker();

            if (checker.IsItDebug(ref sceneName, _primary.Hub.Get<DevelopersScenes>()))
                return;
#endif
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void InitMinorComponents()
        {
            _rootView = GenerateDontDestroyFromHub<UIRootView>();
            GenerateDontDestroyFromHub<GlobalEffects>();
            _gameSettings = new GameSettings(_rootView.SettingsMenu, _primary, _rootView.InfoPanelShowcase);
            _sceneLoader = new SceneLoader(_rootView.LoadScreen, SceneExitCallBack, _gameSettings, _primary.OrientationObserver);
        }

        private void BindToContainerPrimaryComponents()
        {
            BindSingleton<IGamePauseController, TimeScaler>(_primary.TimeScaler);
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
            BindSingleton(_rootView.EscapeMenu);
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
            if (exitData.IsGameQuit)
                QuitGame();
            else if (string.Empty.Equals(exitData.TargetScene) == false)
                LoadScene(exitData.TargetScene);
        }
    }
}