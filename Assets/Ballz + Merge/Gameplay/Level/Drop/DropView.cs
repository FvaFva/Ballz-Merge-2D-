using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

namespace BallzMerge.Gameplay.Level
{
    public class DropView : DependentColorUI
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _colorView;
        [SerializeField] private Button _selector;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private ButtonShaderView _shaderView;
        [SerializeField] private List<DependentColorUI> _backgroundUIs;

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

        public override void ApplyColors(GameColors gameColors)
        {
            foreach (var backgroundUI in _backgroundUIs)
                backgroundUI.ApplyColors(gameColors);

            _shaderView.ApplyColors(gameColors);
        }

        public void InitMaterial()
        {
            _shaderView.Init();
        }

        public void Show(Drop drop)
        {
            _current = drop;

            if (_current.IsEmpty)
                Hide();
            else
                Activate();
        }

        public void Deactivate()
        {
            _selector.enabled = false;
        }

        private void Activate()
        {
            _selector.enabled = true;
            _name.text = _current.Name;
            _description.text = _current.Description;
            _icon.sprite = _current.Icon;
            _colorView.color = _current.Color;
            _shaderView.SetShinyColor(_current.Color);
        }

        private void Hide()
        {
            _name.text = string.Empty;
            _description.text = string.Empty;
            _colorView.color = Color.gray;
            _icon.sprite = _default;
        }

        private void OnSelect()
        {
            if (_current.IsEmpty)
                return;

            Selected?.Invoke(_current);
        }
    }
}