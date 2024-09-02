using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Root
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private LoadScreen _loadingScreen;
        [SerializeField] private RectTransform _sceneContainer;
        [SerializeField] private UserQuestioner _questioner;

        private List<UIView> _sceneUi;

        public LoadScreen LoadScreen => _loadingScreen;
        public UserQuestioner Questioner => _questioner;

        private void Awake()
        {
            _loadingScreen.Hide();
            _sceneUi = new List<UIView>();
        }

        public void AttachSceneUI(UIView ui)
        {
            ui.MoveToContainer(_sceneContainer);
            _sceneUi.Add(ui);
        }

        public void ClearSceneUI()
        {
            foreach (var ui in _sceneUi)
                ui.LeftRoot();
        }
    }
}