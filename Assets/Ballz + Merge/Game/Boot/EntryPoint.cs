using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BallzMerge.Root
{
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

        private EntryPoint()
        {
            InitComponents();
            BindToContainer();
            Inject();
        }

        private void RunGame()
        {
            string targetScene = ScenesNames.MAINMENU;

#if UNITY_EDITOR
            string sceneName = SceneManager.GetActiveScene().name;

            if (_hub.Get<DevelopersScenes>(_hub.DEVELOPERS_SCENES).Scenes.Contains(sceneName) == false
                && sceneName != ScenesNames.BOOT
                && sceneName != ScenesNames.GAMEPLAY)
                return;

            if (_hub.Get<DevelopersScenes>(_hub.DEVELOPERS_SCENES).Scenes.Contains(sceneName) || sceneName == ScenesNames.GAMEPLAY)
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
            _userInput = new MainInputMap();
            _userInput.Enable();
            BindSingleton(_userInput);
            TimeScaler timeScaler = new TimeScaler();
            BindSingleton<IGamePauseController, TimeScaler>(timeScaler);
            _hub = new ResourcesHub();

            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            _rootView = ProjectContext.Instance.Container.InstantiatePrefabResourceForComponent<UIRootView>(_hub.ROOT_UI);
            Object.DontDestroyOnLoad(_rootView.gameObject);

            _gameSettings = new GameSettings(_rootView.EscapeMenu, _data.Settings, timeScaler);
            _sceneLoader = new SceneLoader(_rootView.LoadScreen, SceneExitCallBack);
        }

        private void BindToContainer()
        {
            BindSingleton(_rootView.Questioner);
            BindSingleton(_rootView);
            BindSingleton(_rootView.InfoPanelShowcase);
            BindSingleton(_rootView.EscapeMenu);
            BindSingleton(_gameSettings.AudioSettings);
            BindSingleton(_data);
        }

        private void BindSingleton<TSingleton>(TSingleton singleton) => ProjectContext.Instance.Container.Bind<TSingleton>().FromInstance(singleton).AsSingle().NonLazy();
        private void BindSingleton<TSingletonInterface, TSingletonRealization>(TSingletonRealization singleton) where TSingletonRealization : TSingletonInterface => ProjectContext.Instance.Container.Bind<TSingletonInterface>().To<TSingletonRealization>().FromInstance(singleton).AsSingle().NonLazy();

        private void Inject()
        {
            /*ProjectContext.Instance.Container.Inject(_rootView.Questioner);
            ProjectContext.Instance.Container.Inject(_rootView.EscapeMenu);
            ProjectContext.Instance.Container.Inject(_rootView.InfoPanelShowcase);*/
        }

        private void SceneExitCallBack(SceneExitData exitData)
        {
            if (exitData.IsGameQuit)
                QuitGame();
            else if (string.Empty.Equals(exitData.TargetScene) == false)
                LoadScene(exitData.TargetScene);
        }
    }
}