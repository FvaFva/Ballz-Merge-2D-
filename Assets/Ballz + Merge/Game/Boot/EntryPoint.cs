using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BallzMerge.Root
{
    using BallzMerge.Achievement;
    using BallzMerge.Data;
    using Settings;
    using UnityEngine.Audio;

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
        private TimeScaler _timeScaler;

        private EntryPoint()
        {
            InitPrimaryComponents();
            BindToContainerPrimaryComponents();
            InitMinorComponents();
            BindToContainerMinorComponents();
        }

        private void RunGame()
        {
            string sceneName = ScenesNames.MAINMENU;
#if UNITY_EDITOR
            if (CheckDebugScene(out sceneName) == false)
                return;
#endif
            LoadScene(sceneName);
        }

        private bool CheckDebugScene(out string sceneName)
        {
            sceneName = SceneManager.GetActiveScene().name;
            
            if (_hub.Get<DevelopersScenes>().Scenes.Contains(sceneName) == false
                && sceneName != ScenesNames.BOOT
                && sceneName != ScenesNames.GAMEPLAY)
                return false;

            return _hub.Get<DevelopersScenes>().Scenes.Contains(sceneName) || sceneName == ScenesNames.GAMEPLAY;
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

        private void InitPrimaryComponents()
        {
            _data = new DataBaseSource();
            _timeScaler = new TimeScaler();
            _userInput = new MainInputMap();
            _userInput.Enable();
            _hub = new ResourcesHub();
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
        }

        private void InitMinorComponents()
        {
            _rootView = GenerateDontDestroyFromHub<UIRootView>();
            GenerateDontDestroyFromHub<GlobalEffects>();
            _gameSettings = new GameSettings(_rootView.SettingsMenu, _data.Settings, _hub.Get<AudioMixer>(), _timeScaler);
            _sceneLoader = new SceneLoader(_rootView.LoadScreen, SceneExitCallBack, _gameSettings);
        }

        private void BindToContainerPrimaryComponents()
        {
            BindSingleton<IGamePauseController, TimeScaler>(_timeScaler);
            BindSingleton(_data);
            BindSingleton(_userInput);
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
            T prefab = _hub.Get<T>();
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