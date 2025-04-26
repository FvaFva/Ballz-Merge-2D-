using BallzMerge.Root.Settings;
using BallzMerge.ScreenOrientations;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private Dictionary<string, float> _loadData;

        private readonly GameSettings _settings;
        private readonly WaitForSeconds _checkTime;
        private readonly LoadScreen _loadView;
        private readonly Action<SceneExitData> _sceneExit;
        private readonly ScreenOrientationObserver _orientationObserver;

        public SceneLoader(LoadScreen loadView, Action<SceneExitData> sceneExit, GameSettings settings, ScreenOrientationObserver orientationObserver)
        {
            _checkTime = new WaitForSeconds(SecondsCheckTime);
            _loadView = loadView;
            _sceneExit = sceneExit;
            _settings = settings;
            _orientationObserver = orientationObserver;
            ProjectContext.Instance.Container.Inject(this);
        }

        public void AddLoadData(IDictionary<string, float> loadData)
        {
            _loadData = new Dictionary<string, float>(loadData);
        }

        public IEnumerator LoadScene(string name)
        {
            _loadView.Show();
            _targetSceneEntryPoint.Clear();
            _orientationObserver.CheckOutScene();

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

            foreach (IDependentScreenOrientation orientator in _targetSceneEntryPoint.Current.Orientators)
                _orientationObserver.CheckInSceneElements(orientator);

            _targetSceneEntryPoint.Current.Init(_sceneExit, _loadData);
            _loadView.MoveProgress(1, 1);
            yield return null;
            _loadView.Hide();
            _loadData = null;
        }
    }
}
