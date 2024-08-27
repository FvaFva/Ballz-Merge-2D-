using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Achievement
{
    internal class AchievementView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;

        public bool IsWithIcon { get; private set; }

        public void Show(string text, Texture2D texture)
        {
            _label.text = text;
            IsWithIcon = texture != null;

            if (IsWithIcon)
                _icon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}