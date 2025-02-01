using BallzMerge.Root.Settings;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BallzMerge.Root
{
    public class SceneLoader
    {
        private const float SceneGeneratePrecent = 0.3f;
        private const float SecondsCheckTime = 0.05f;

        [Inject] private TargetSceneEntryPointContainer _targetSceneEntryPoint;

        private readonly GameSettings _settings;
        private readonly WaitForSeconds _checkTime;
        private readonly LoadScreen _loadView;
        private readonly Action<SceneExitData> _sceneExit;

        public SceneLoader(LoadScreen loadView, Action<SceneExitData> sceneExit, GameSettings settings)
        {
            _checkTime = new WaitForSeconds(SecondsCheckTime);
            _loadView = loadView;
            _sceneExit = sceneExit;
            _settings = settings;
            ProjectContext.Instance.Container.Inject(this);
        }

        public IEnumerator LoadScene(string name)
        {
            _loadView.Show();
            _targetSceneEntryPoint.Clear();

            foreach (var _ in LoadSceneFromBoot(name))
                yield return _checkTime;

            foreach (var _ in InitScene())
                yield return _checkTime;

            _settings.ReadData();
        }

        private IEnumerable LoadSceneFromBoot(string name)
        {
            SceneManager.LoadSceneAsync(ScenesNames.BOOT);
            _loadView.MoveProgress(SceneGeneratePrecent, SecondsCheckTime);
            yield return null;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
            _loadView.MoveProgress(SceneGeneratePrecent, SecondsCheckTime);
            yield return null;

            while (asyncLoad.progress < 0.9f)
            {
                _loadView.MoveProgress(SceneGeneratePrecent, SecondsCheckTime);
                yield return null;
            }

            _loadView.MoveProgress(SceneGeneratePrecent, 1);
            yield return null;
        }

        private IEnumerable InitScene()
        {
            while (_targetSceneEntryPoint?.IsReady() == false)
            {
                _loadView.MoveProgress(1, SecondsCheckTime);
                yield return null;
            }

            foreach (IInitializable initComponent in _targetSceneEntryPoint.Current.InitializedComponents)
            {
                _loadView.MoveProgress(1, SecondsCheckTime);
                initComponent.Init();
                yield return null;
            }

            _targetSceneEntryPoint.Current.Init(_sceneExit);
            _loadView.MoveProgress(1, 1);
            yield return null;
            _loadView.Hide();
        }
    }
}
