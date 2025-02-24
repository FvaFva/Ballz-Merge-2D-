using UnityEngine;

namespace BallzMerge.Root
{
    using Data;
    using Settings;
    using System;

    public class UIRootView : MonoBehaviour, IDisposable
    {
        private const int StandardDistance = 0;

        [SerializeField] private LoadScreen _loadingScreen;
        [SerializeField] private RectTransform _sceneContainer;
        [SerializeField] private UserQuestioner _questioner;
        [SerializeField] private EscapeMenu _escapeMenu;
        [SerializeField] private InfoPanelShowcase _infoPanelShowcase;
        [SerializeField] private GameSettingsMenu _settingsMenu;
        [SerializeField] private Canvas _mainCanvas;

        private UIView _sceneUI;

        public LoadScreen LoadScreen => _loadingScreen;
        public UserQuestioner Questioner => _questioner;
        public EscapeMenu EscapeMenu => _escapeMenu;
        public GameSettingsMenu SettingsMenu => _settingsMenu;
        public InfoPanelShowcase InfoPanelShowcase => _infoPanelShowcase;

        private void Awake()
        {
            _loadingScreen.Hide();
        }

        public void Dispose()
        {
            _infoPanelShowcase.UIViewStateChanged -= ChangeStateUIView;
        }

        public void AttachSceneUI(UIView sceneUI, Camera uICamera = null)
        {
            _sceneUI = sceneUI;
            ChangeStateUIView(true);
            _infoPanelShowcase.UIViewStateChanged += ChangeStateUIView;
            _escapeMenu.UpdateButtonView(_sceneUI.IsUseSettingsQuiteButton, _sceneUI.IsUseSettingsMaineMenuButton);

            if(uICamera != null)
            {
                _mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                _mainCanvas.worldCamera = uICamera;
                _mainCanvas.planeDistance = StandardDistance;
            }
        }

        public void ClearSceneUI()
        {
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