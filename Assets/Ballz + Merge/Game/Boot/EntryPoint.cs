using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BallzMerge.Root
{
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
            _userInput = new MainInputMap();
            _userInput.Enable();
            _hub = new ResourcesHub();

            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            _rootView = Object.Instantiate(_hub.Get<UIRootView>(_hub.ROOT_UI));
            Object.DontDestroyOnLoad(_rootView.gameObject);

            _sceneLoader = new SceneLoader(_rootView.LoadScreen, SceneExitCallBack);
        }

        private void BindToContainer()
        {
            ProjectContext.Instance.Container.Bind<UIRootView>().FromInstance(_rootView).NonLazy();
            ProjectContext.Instance.Container.Bind<UserQuestioner>().FromInstance(_rootView.Questioner).AsSingle().NonLazy();
            ProjectContext.Instance.Container.Bind<AudioSettings>().FromNew().AsSingle().NonLazy();
            ProjectContext.Instance.Container.Bind<MainInputMap>().FromInstance(_userInput).AsSingle().NonLazy();
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