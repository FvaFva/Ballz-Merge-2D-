using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BallzMerge.Root
{
    using BallzMerge.Achievement;
    using BallzMerge.Data;
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

        private Coroutines _coroutines;
        private UIRootView _rootView;
        private SceneLoader _sceneLoader;
        private MainInputMap _userInput;
        private ResourcesHub _hub;
        private GameSettings _gameSettings;
        private DataBaseSource _data;
        private EffectPool _effectPool;

        private EntryPoint()
        {
            InitComponents();
            BindToContainer();
        }

        private void RunGame()
        {
            string targetScene = ScenesNames.MAINMENU;

#if UNITY_EDITOR
            string sceneName = SceneManager.GetActiveScene().name;

            if (_hub.Get<DevelopersScenes>().Scenes.Contains(sceneName) == false
                && sceneName != ScenesNames.BOOT
                && sceneName != ScenesNames.GAMEPLAY)
                return;

            if (_hub.Get<DevelopersScenes>().Scenes.Contains(sceneName) || sceneName == ScenesNames.GAMEPLAY)
                targetScene = sceneName;
#endif
            LoadScene(targetScene);
        }

        private void LoadScene(string targetScene)
        {
            _rootView.ClearSceneUI();
            _coroutines.StartCoroutine(_sceneLoader.LoadScene(targetScene));
        }

        private void QuitGame()
        {
            _userInput.Disable();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void InitComponents()
        {
            _data = new DataBaseSource();
            BindSingleton(_data);
            _userInput = new MainInputMap();
            _effectPool = new EffectPool();
            _userInput.Enable();
            BindSingleton(_userInput);
            BindSingleton(_effectPool);
            TimeScaler timeScaler = new TimeScaler();
            BindSingleton<IGamePauseController, TimeScaler>(timeScaler);
            _hub = new ResourcesHub();

            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            _rootView = GenerateDontDestroyFromHub<UIRootView>();
            GenerateDontDestroyFromHub<GlobalEffects>();
            BindSingleton(GenerateDontDestroyFromHub<AchievementsBus>());

            _gameSettings = new GameSettings(_rootView.EscapeMenu, _data.Settings, timeScaler);
            _sceneLoader = new SceneLoader(_rootView.LoadScreen, SceneExitCallBack);
        }

        private T GenerateDontDestroyFromHub<T>() where T : Component
        {
            T prefab = _hub.Get<T>();
            T instance = ProjectContext.Instance.Container.InstantiatePrefabForComponent<T>(prefab);
            Object.DontDestroyOnLoad(instance.gameObject);
            return instance;
        }

        private void BindToContainer()
        {
            BindSingleton(_rootView.Questioner);
            BindSingleton(_rootView);
            BindSingleton(_rootView.InfoPanelShowcase);
            BindSingleton(_rootView.EscapeMenu);
            BindSingleton(_gameSettings.AudioSettings);
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