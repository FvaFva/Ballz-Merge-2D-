using TMPro;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class DropViewSuffix : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        public void UpdateView(string suffix)
        {
            bool isEmpty = suffix == null || suffix.Equals(string.Empty);
            gameObject.SetActive(!isEmpty);
            _label.text = suffix;
        }
    }
}
