using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Gameplay.Level
{
    public class DropView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _colorView;
        [SerializeField] private Button _activator;
        [SerializeField] private RectTransform _additionalInfo;

        private Drop _current;
        private float _count;
        private Sprite _default;

        public event Action<Drop, float> Selected;

        private void Awake()
        {
            _default = _icon.sprite;
        }

        private void OnEnable()
        {
            _activator.AddListener(OnSelect);
        }

        private void OnDisable()
        {
            _activator.RemoveListener(OnSelect);
        }

        public void Show(Drop drop)
        {
            _current = drop;

            if (_current == null)
                Hide();
            else
                Activate();
        }

        private void Activate()
        {
            _count = _current.GetRandomCount();
            _additionalInfo.gameObject.SetActive(_current.IsReducible);
            _name.text = _current.Name;
            _description.text = _current.Description;
            _value.text = (_count * 100).ToString("F0");
            _icon.sprite = _current.Icon;
            _colorView.color = _current.Color;
        }

        private void Hide()
        {
            _count = 0;
            _name.text = string.Empty;
            _description.text = string.Empty;
            _value.text = string.Empty;
            _colorView.color = Color.gray;
            _icon.sprite = _default;
            _additionalInfo.gameObject.SetActive(false);
        }

        private void OnSelect()
        {
            if (_current != null)
                Selected?.Invoke(_current, _count);
        }
    }
}