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

        private EntryPoint()
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            _rootView = Object.Instantiate(Resources.Load<UIRootView>("UIRoot"));
            Object.DontDestroyOnLoad(_rootView.gameObject);

            ProjectContext.Instance.Container.Bind<UIRootView>().FromInstance(_rootView).NonLazy();

            _sceneLoader = new SceneLoader(_rootView.LoadScreen);
        }

        private void RunGame()
        {
#if UNITY_EDITOR
            string sceneName = SceneManager.GetActiveScene().name;

            if (sceneName == ScenesNames.GAMEPLAY || sceneName != ScenesNames.BOOT)
                return;
#endif
            _coroutines.StartCoroutine(_sceneLoader.LoadScene(ScenesNames.GAMEPLAY));
        }
    }
}