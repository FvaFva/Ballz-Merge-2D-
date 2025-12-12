using UnityEngine;

namespace BallzMerge.Root
{
    using Data;
    using Settings;
    using System;

    public class UIRootView : MonoBehaviour
    {
        private const int StandardDistance = 0;

        [SerializeField] private LoadScreen _loadingScreen;
        [SerializeField] private UIRootContainers _containers;
        [SerializeField] private UserQuestioner _questioner;
        [SerializeField] private EscapeMenu _escapeMenu;
        [SerializeField] private InfoPanelShowcase _infoPanelShowcase;
        [SerializeField] private GameSettingsMenu _settingsMenu;
        [SerializeField] private PopupDisplayer _popupDisplayer;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private UIReorganizer _uiReorganizer;

        private UIView _sceneUI;

        public LoadScreen LoadScreen => _loadingScreen;
        public UserQuestioner Questioner => _questioner;
        public EscapeMenu EscapeMenu => _escapeMenu;
        public GameSettingsMenu SettingsMenu => _settingsMenu;
        public InfoPanelShowcase InfoPanelShowcase => _infoPanelShowcase;
        public PopupDisplayer PopupsDisplayer => _popupDisplayer;
        public UIRootContainers Containers => _containers;
        public UIReorganizer UIReorganizer => _uiReorganizer;

        private void Awake()
        {
            _loadingScreen.Hide();
        }

        private void OnEnable()
        {
            _infoPanelShowcase.UIViewStateChanged += ChangeStateUIView;
        }

        private void OnDisable()
        {
            _infoPanelShowcase.UIViewStateChanged -= ChangeStateUIView;
        }

        public void AttachSceneUI(UIView sceneUI, Camera uICamera = null)
        {
            _sceneUI = sceneUI;
            _containers.SetSettings(_sceneUI.RootContainerBehavior);
            ChangeStateUIView(true);
            _infoPanelShowcase.InitOpenButton(true);

            if(uICamera != null)
            {
                _mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                _mainCanvas.worldCamera = uICamera;
                _mainCanvas.planeDistance = StandardDistance;
            }

            _containers.TakeNewItems(sceneUI.Items);
        }

        public void UpdateViewButtons()
        {
            _escapeMenu.UpdateButtonView(_sceneUI.IsUseSettingsQuiteButton, _sceneUI.IsUseSettingsMaineMenuButton);
        }

        public void ClearSceneUI()
        {
            _infoPanelShowcase.InitOpenButton(false);
            _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (_sceneUI is not null)
            {
                _sceneUI.LeftRoot();
                _sceneUI = null;
            }
        }

        private void ChangeStateUIView(bool state)
        {
            _sceneUI.ChangeState(state);
        }
    }
}