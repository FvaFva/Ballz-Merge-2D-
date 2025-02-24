using BallzMerge.Root;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BallzMerge.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour, ISceneEnterPoint
    {
        [SerializeField] private UIView _view;
        [SerializeField] private Button _startGame;
        [SerializeField] private List<CyclicBehavior> _behaviors;

        [Inject] private UIRootView _rootUI;

        private List<IInitializable> _initializedComponents;
        private Action<SceneExitData> _callback;
        private MainInputMap _userInput;

        public IEnumerable<IInitializable> InitializedComponents => _initializedComponents;

        public bool IsAvailable {  get; private set; }

        private void Start()
        {
            IsAvailable = true;
        }

        private void Awake()
        {
            _initializedComponents = new List<IInitializable>();

            foreach (var component in _behaviors)
            {
                if(component is IInitializable componentInstance)
                    _initializedComponents.Add(componentInstance);
            }
        }

        private void OnEnable()
        {
            _rootUI.EscapeMenu.QuitRequired += LeftScene;
            _startGame.AddListener(OnStartRequire);
        }

        private void OnDisable()
        {
            _rootUI.EscapeMenu.QuitRequired -= LeftScene;
            _startGame.RemoveListener(OnStartRequire);
        }

        private void OnDestroy()
        {
            IsAvailable = false;
        }

        public void Init(Action<SceneExitData> callback)
        {
            _rootUI.AttachSceneUI(_view);
            _callback = callback;
            //_userInput = new MainInputMap();
            //_userInput.Enable();
            //ProjectContext.Instance.Container.Bind<MainInputMap>().FromInstance(_userInput).AsSingle().NonLazy();
        }

        private void OnStartRequire()
        {
            LeftScene(new SceneExitData(ScenesNames.GAMEPLAY));
        }

        private void LeftScene(SceneExitData exitData)
        {
            _callback?.Invoke(exitData);
            //_userInput.Disable();
        }
    }
}