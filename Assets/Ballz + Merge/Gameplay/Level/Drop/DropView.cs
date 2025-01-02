using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.Gameplay.Level
{
    public class DropView : MonoBehaviour
    {
        private const string ShineColorProperty = "_ShinyColor";

        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _colorView;
        [SerializeField] private Button _activator;
        [SerializeField] private RectTransform _additionalInfo;
        [SerializeField] private List<Image> _shineMasks;

        private Drop _current;
        private Sprite _default;
        private List<Material> _shineMaterials;

        public event Action<Drop> Selected;

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

        public void CashMaterials()
        {
            _shineMaterials = new List<Material>();

            foreach (Image shineMask in _shineMasks)
            {
                Material material = new Material(shineMask.material);
                shineMask.material = material;
                _shineMaterials.Add(material);
            }
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
            _additionalInfo.gameObject.SetActive(_current.IsReducible);
            _name.text = _current.Name;
            _description.text = _current.Description;
            _value.text = _current.Rarity.Weight.ToString();
            _icon.sprite = _current.Icon;
            _colorView.color = _current.Color;

            foreach (Material shineMaterial in _shineMaterials)
                shineMaterial.SetColor(ShineColorProperty, _current.Color);
        }

        private void Hide()
        {
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
                Selected?.Invoke(_current);
        }
    }
}