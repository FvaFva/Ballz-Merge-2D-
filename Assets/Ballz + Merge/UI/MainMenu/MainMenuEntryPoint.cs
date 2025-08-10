using BallzMerge.Data;
using BallzMerge.Root;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour, ISceneEnterPoint
    {
        [SerializeField] private UIView _view;
        [SerializeField] private LevelSelectorOperator _levelSelector;
        [SerializeField] private LevelContinueView _levelLoader;
        [SerializeField] private LevelInGame _level;
        [SerializeField] private List<CyclicBehavior> _behaviors;

        [Inject] private UIRootView _rootUI;
        [Inject] private DataBaseSource _db;

        private List<IInitializable> _initializedComponents;
        private List<IDependentScreenOrientation> _orientationDependObjects;
        private Action<SceneExitData> _callback;

        public IEnumerable<IInitializable> InitializedComponents => _initializedComponents;
        public IEnumerable<IDependentScreenOrientation> OrientationDepends => _orientationDependObjects;

        public bool IsAvailable { get; private set; }

        private void Start()
        {
            IsAvailable = true;
            var load = _db.Saves.Get();

            if (load.IsLoaded)
            {
                _level.Load(load);
                _levelLoader.ChangeState(true, _level.Current.Title);
            }
            else
            {
                _levelLoader.ChangeState(false);
            }
        }

        private void Awake()
        {
            _initializedComponents = new List<IInitializable>();
            _orientationDependObjects = new List<IDependentScreenOrientation>();

            foreach (var component in _behaviors)
            {
                if (component is IInitializable componentInstance)
                    _initializedComponents.Add(componentInstance);

                if (component is IDependentScreenOrientation temp)
                    _orientationDependObjects.Add(temp);
            }
        }

        private void OnEnable()
        {
            _rootUI.EscapeMenu.QuitRequired += LeftScene;
            _levelSelector.Selected += OnStartRequire;
            _levelLoader.Selected += OnContinueRequire;
        }

        private void OnDisable()
        {
            _rootUI.EscapeMenu.QuitRequired -= LeftScene;
            _levelSelector.Selected -= OnStartRequire;
            _levelLoader.Selected -= OnContinueRequire;
        }

        private void OnDestroy()
        {
            IsAvailable = false;
        }

        public void Init(Action<SceneExitData> callback, bool isLoad)
        {
            _view.Init();
            _rootUI.AttachSceneUI(_view);
            _callback = callback;
        }

        private void OnStartRequire()
        {
            LeftScene(new SceneExitData(ScenesNames.GAMEPLAY));
        }

        private void OnContinueRequire()
        {
            LeftScene(new SceneExitData(ScenesNames.GAMEPLAY, true));
        }

        private void LeftScene(SceneExitData exitData)
        {
            _callback?.Invoke(exitData);
        }
    }
}