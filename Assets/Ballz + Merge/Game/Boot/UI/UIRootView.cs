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

        private UIView _sceneUI;

        public LoadScreen LoadScreen => _loadingScreen;
        public UserQuestioner Questioner => _questioner;
        public EscapeMenu EscapeMenu => _escapeMenu;
        public GameSettingsMenu SettingsMenu => _settingsMenu;
        public InfoPanelShowcase InfoPanelShowcase => _infoPanelShowcase;
        public PopupDisplayer PopupsDisplayer => _popupDisplayer;
        public UIRootContainers Containers => _containers;

        public event Action<IInfoPanelView> PanelTriggered;

        private void Awake()
        {
            _loadingScreen.Hide();
        }

        private void OnEnable()
        {
            _infoPanelShowcase.UIViewStateChanged += ChangeStateUIView;
            _infoPanelShowcase.ClosePanelTriggered += OnClosePanelTriggered;
        }

        private void OnDisable()
        {
            _infoPanelShowcase.UIViewStateChanged -= ChangeStateUIView;
            _infoPanelShowcase.ClosePanelTriggered -= OnClosePanelTriggered;
        }

        public void AttachSceneUI(UIView sceneUI, Camera uICamera = null)
        {
            _sceneUI = sceneUI;
            ChangeStateUIView(true);
            _escapeMenu.UpdateButtonView(_sceneUI.IsUseSettingsQuiteButton, _sceneUI.IsUseSettingsMaineMenuButton);

            if(uICamera != null)
            {
                _mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                _mainCanvas.worldCamera = uICamera;
                _mainCanvas.planeDistance = StandardDistance;
            }

            _containers.TakeNewItems(sceneUI.Items);
        }

        public void PackItemInContainer(UIRootContainerItem item)
        {
            _containers.PackItem(item);
        }

        public void UnpackItemFromContainer(UIRootContainerItem item)
        {
            _containers.UnpackItem(item);
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

        private void OnClosePanelTriggered(IInfoPanelView panelView)
        {
            PanelTriggered?.Invoke(panelView);
        }
    }
}