using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BallzMerge.Root
{
    public class LoadScreen : MonoBehaviour
    {
        [SerializeField] private Slider _progress;

        public void Show()
        {
            _progress.value = 0;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void MoveProgress(float target, float step)
        {
            _progress.value = Mathf.Lerp(_progress.value, target, step);
        }

        public void MoveProgress(float target)
        {
            _progress.value = target;
        }
    }
}
