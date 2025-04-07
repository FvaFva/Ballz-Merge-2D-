using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Achievement
{
    public class PopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _icon;

        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = (RectTransform)transform;
        }

        public void SetData(string label, string description, Sprite image, int currentStep = 0, int maxTargets = 0)
        {
            _label.text = label;
            _description.text = currentStep != 0 ? $"({currentStep}/{maxTargets})\n{description}" : description;
            _icon.sprite = image;
        }
    }
}