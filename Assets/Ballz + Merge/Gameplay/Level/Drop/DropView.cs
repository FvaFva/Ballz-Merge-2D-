using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

namespace BallzMerge.Gameplay.Level
{
    public class DropView : MonoBehaviour
    {
        private const string ShineColorProperty = "_ShinyColor";

        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private DropViewSuffix _suffix;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _colorView;
        [SerializeField] private Button _selector;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private List<Image> _shineMasks;

        private Drop _current;
        private Sprite _default;
        private List<Material> _shineMaterials;
        private RectTransform _activatorTransform;
        private MainModule _mainModule;
        private ShapeModule _shapeModule;

        public event Action<Drop> Selected;

        private void Awake()
        {
            _default = _icon.sprite;
            _activatorTransform = (RectTransform)_selector.transform;
            _shapeModule = _particles.shape;
            _mainModule = _particles.main;
        }

        private void OnEnable()
        {
            Vector2 scale = _activatorTransform.rect.size * _activatorTransform.lossyScale;
            _shapeModule.scale = new Vector3(scale.x, scale.y, 0f);
            _mainModule.startColor = _current.Color;
            _selector.AddListener(OnSelect);
        }

        private void OnDisable()
        {
            _selector.RemoveListener(OnSelect);
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

            if (_current.IsEmpty)
                Hide();
            else
                Activate();
        }

        private void Activate()
        {
            _selector.enabled = true;
            _suffix.UpdateView(_current.Suffix);
            _name.text = _current.Name;
            _description.text = _current.Description;
            _icon.sprite = _current.Icon;
            _colorView.color = _current.Color;

            foreach (Material shineMaterial in _shineMaterials)
                shineMaterial.SetColor(ShineColorProperty, _current.Color);
        }

        private void Hide()
        {
            _name.text = string.Empty;
            _description.text = string.Empty;
            _colorView.color = Color.gray;
            _icon.sprite = _default;
            _suffix.gameObject.SetActive(false);
        }

        private void OnSelect()
        {
            if (_current.IsEmpty)
                return;
            
            _selector.enabled = false;
            Selected?.Invoke(_current);
        }
    }
}