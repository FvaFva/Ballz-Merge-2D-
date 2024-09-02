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
        [SerializeField] private Button _quite;

        [Inject] private UIRootView _rootUI;

        private List<IInitializable> _initializable;
        private Action<SceneExitData> _callback;

        public IEnumerable<IInitializable> InitializedComponents => _initializable;

        public bool IsAvailable {  get; private set; }

        private void Start()
        {
            IsAvailable = true;
        }

        private void Awake()
        {
            _initializable = new List<IInitializable>();
        }

        private void OnEnable()
        {
            _startGame.AddListener(OnStartRequire);
            _quite.AddListener(OnQuiteRequire);
        }

        private void OnDisable()
        {
            _startGame.RemoveListener(OnStartRequire);
            _quite.RemoveListener(OnQuiteRequire);
        }

        private void OnDestroy()
        {
            IsAvailable = false;
        }

        public void Init(Action<SceneExitData> callback)
        {
            _rootUI.AttachSceneUI(_view);
            _callback = callback;
        }

        private void OnQuiteRequire()
        {
            _callback?.Invoke(new SceneExitData(true));
        }

        private void OnStartRequire()
        {
            _callback?.Invoke(new SceneExitData(ScenesNames.GAMEPLAY));
        }
    }
}