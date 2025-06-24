using BallzMerge.Data;
using BallzMerge.Root;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BallzMerge.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour, ISceneEnterPoint
    {
        [SerializeField] private UIView _view;
        [SerializeField] private Button _startGame;
        [SerializeField] private Button _continueGame;
        [SerializeField] private AnimatedButton _continueGameButtonView;
        [SerializeField] private List<CyclicBehavior> _behaviors;

        [Inject] private UIRootView _rootUI;
        [Inject] private DataBaseSource _db;

        private List<IInitializable> _initializedComponents;
        private List<IDependentScreenOrientation> _orientators;
        private Action<SceneExitData> _callback;

        public IEnumerable<IInitializable> InitializedComponents => _initializedComponents;
        public IEnumerable<IDependentScreenOrientation> Orientators => _orientators;

        public bool IsAvailable { get; private set; }


        private void Start()
        {
            IsAvailable = true;
            _continueGame.interactable = _db.Saves.CheckSaves();
            _continueGameButtonView.SetState(_continueGame.interactable);
        }

        private void Awake()
        {
            _initializedComponents = new List<IInitializable>();
            _orientators = new List<IDependentScreenOrientation>();

            foreach (var component in _behaviors)
            {
                if (component is IInitializable componentInstance)
                    _initializedComponents.Add(componentInstance);

                if (component is IDependentScreenOrientation orientator)
                    _orientators.Add(orientator);
            }
        }

        private void OnEnable()
        {
            _rootUI.EscapeMenu.QuitRequired += LeftScene;
            _startGame.AddListener(OnStartRequire);
            _continueGame.AddListener(OnContinueRequire);
        }

        private void OnDisable()
        {
            _rootUI.EscapeMenu.QuitRequired -= LeftScene;
            _startGame.RemoveListener(OnStartRequire);
            _continueGame.RemoveListener(OnContinueRequire);
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