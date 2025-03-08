using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Achievement
{
    public class AchievementView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _icon;

        public RectTransform RectTransform => (RectTransform)gameObject.transform;

        public void SetData(string label, string description, Sprite image)
        {
            _label.text = label;
            _description.text = description;
            _icon.sprite = image;
        }
    }
}