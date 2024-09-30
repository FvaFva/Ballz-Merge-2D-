using UnityEngine;

namespace BallzMerge.Root
{
    using Settings;

    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private LoadScreen _loadingScreen;
        [SerializeField] private RectTransform _sceneContainer;
        [SerializeField] private UserQuestioner _questioner;
        [SerializeField] private EscapeMenu _escapeMenu;
        [SerializeField] private InfoPanelShowcase _infoPanelShowcase;

        private UIView _sceneUI;

        public LoadScreen LoadScreen => _loadingScreen;
        public UserQuestioner Questioner => _questioner;
        public EscapeMenu EscapeMenu => _escapeMenu;
        public InfoPanelShowcase InfoPanelShowcase => _infoPanelShowcase;

        private void Awake()
        {
            _loadingScreen.Hide();
        }

        public void AttachSceneUI(UIView sceneUI)
        {
            _sceneUI = sceneUI;
            _sceneUI.MoveToContainer(_sceneContainer);
            _escapeMenu.UpdateButtonView(_sceneUI.IsUseSettingsQuiteButton, _sceneUI.IsUseSettingsMaineMenuButton);
        }

        public void ClearSceneUI()
        {
            if (_sceneUI is not null)
            {
                _sceneUI.LeftRoot();
                _sceneUI = null;
            }
        }
    }
}