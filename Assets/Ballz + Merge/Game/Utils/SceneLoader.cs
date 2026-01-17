using BallzMerge.Root.Settings;
using BallzMerge.ScreenOrientations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BallzMerge.Root
{
    public class SceneLoader
    {
        private const float SceneGeneratePercent = 0.3f;
        private const float SecondsCheckTime = 0.2f;

        [Inject] private TargetSceneEntryPointContainer _targetSceneEntryPoint;

        private bool _isLoad;

        private readonly GameSettings _settings;
        private readonly WaitForSeconds _checkTime;
        private readonly LoadScreen _loadView;
        private readonly Action<SceneExitData> _sceneExit;
        private readonly ScreenOrientationObserver _orientationObserver;
        private readonly UIReorganizer _uiReorganizer;
        private readonly Action _updateViewButtons;

        public SceneLoader(LoadScreen loadView, UIReorganizer uiReorganizer, Action updateViewButtons, Action<SceneExitData> sceneExit, GameSettings settings, ScreenOrientationObserver orientationObserver)
        {
            _checkTime = new WaitForSeconds(SecondsCheckTime);
            _updateViewButtons = updateViewButtons;
            _loadView = loadView;
            _uiReorganizer = uiReorganizer;
            _sceneExit = sceneExit;
            _settings = settings;
            _orientationObserver = orientationObserver;
            _isLoad = false;
            ProjectContext.Instance.Container.Inject(this);
        }

        public void SetLoad(bool state)
        {
            _isLoad = state;
        }

        public IEnumerator LoadScene(string name, bool isAutoEntering = false)
        {
            _targetSceneEntryPoint.Clear();
            _orientationObserver.CheckOutScene();
            _settings.CheckOutScene();
            _loadView.Show();

            foreach (var _ in LoadSceneFromBoot(name))
                yield return _checkTime;

            foreach (var _ in InitScene())
                yield return _checkTime;

            if (isAutoEntering)
            {
                _loadView.Hide();
                AfterLoad();
            }
            else
            {
                _loadView.WaitToClick(AfterLoad);
            }
        }

        private void AfterLoad()
        {
            _targetSceneEntryPoint.Current.Init(_sceneExit, _isLoad);
            _isLoad = false;
            _settings.ConnectSliders();
            _settings.LoadData();
            _uiReorganizer.ConnectToSetting(_settings.SceneSetting);
            _settings.OnGlobalSettingChanged();
            _updateViewButtons();
            _targetSceneEntryPoint.Current.AfterLoad();
        }

        private IEnumerable LoadSceneFromBoot(string name)
        {
            SceneManager.LoadSceneAsync(ScenesNames.BOOT);
            _loadView.MoveProgress(SceneGeneratePercent, SecondsCheckTime);
            yield return null;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
            _loadView.MoveProgress(SceneGeneratePercent, SecondsCheckTime);
            yield return null;

            while (asyncLoad.progress < 0.9f)
            {
                _loadView.MoveProgress(SceneGeneratePercent, SecondsCheckTime);
                yield return null;
            }

            _loadView.MoveProgress(SceneGeneratePercent, 1);
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

            foreach (IDependentScreenOrientation orientated in _targetSceneEntryPoint.Current.OrientationDepends)
                _orientationObserver.CheckInSceneElements(orientated);

            yield return null;

            foreach (IDependentSceneSettings sceneSetting in _targetSceneEntryPoint.Current.SettingsDepends)
                _settings.CheckInSceneElement(sceneSetting);

            _loadView.MoveProgress(1, 1);
            yield return null;
        }
    }
}
