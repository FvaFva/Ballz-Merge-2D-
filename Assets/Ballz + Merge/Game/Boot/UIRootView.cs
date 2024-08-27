using UnityEngine;

namespace BallzMerge.Root
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private LoadScreen _loadingScreen;

        public LoadScreen LoadScreen => _loadingScreen;

        private void Awake()
        {
            _loadingScreen.Hide();
        }
    }
}