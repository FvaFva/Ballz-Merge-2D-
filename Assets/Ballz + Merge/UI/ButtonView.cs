using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow), typeof(Image))]
public class ButtonView : MonoBehaviour
{
    [SerializeField] private ButtonColorType _buttonColorType;

    private RectTransform _transform;
    private Tween _shadowTween;
    private Image _image;
    private Shadow _shadow;
    private TMP_Text _label;
    private ButtonShaderView _shaderView;
    private Dictionary<ColorType, Func<Color>> _shadowColors;
    private GameColors _gameColors;
    private Material _labelFontMaterial;

    private bool _isInited;
    private bool _isActive;

    public void Init()
    {
        if (_isInited)
            return;

        _isInited = true;
        _isActive = true;
        _transform = (RectTransform)transform;
        _shadow = GetComponent<Shadow>();
        _image = GetComponent<Image>();
        _label = GetComponentInChildren<TMP_Text>();

        if (_label != null)
        {
            _labelFontMaterial = new Material(_label.fontMaterial);
            _label.fontMaterial = _labelFontMaterial;
        }

        _shaderView = GetComponentInChildren<ButtonShaderView>(true);
        _shaderView.PerformIfNotNull(shaderView => shaderView.Init(_buttonColorType));

        _shadowColors = new Dictionary<ColorType, Func<Color>>
        {
            { ColorType.StartColor, () => _image.color },
            { ColorType.TargetColor, () => _image.color }
        };
    }

    public void SetActiveState(bool state)
    {
        _isActive = state;
    }

    public void ApplyColors(GameColors colors)
    {
        _gameColors = colors;

        if (_isActive)
            _image.color = _gameColors.GetForButtonView(_buttonColorType);

        _shadowColors = new Dictionary<ColorType, Func<Color>>
        {
            { ColorType.StartColor, () => _gameColors.GetForShadow(_buttonColorType, 0f) },
            { ColorType.TargetColor, () => _gameColors.GetForShadow(_buttonColorType, 1f) }
        };

        _shaderView.PerformIfNotNull(shaderView => shaderView.ApplyColors(_gameColors));
    }

    public void SetDefault()
    {
        StopAllAnimations();

        SetShaderView(false);

        _transform.localScale = Vector3.one;
        _shadow.effectColor = _shadowColors[ColorType.StartColor]();
    }

    public void ChangeParameters(float newScale, ColorType colorType, float duration)
    {
        _transform.DOScale(newScale, duration);
        ChangeShadowColor(_shadowColors[colorType](), duration);
    }

    public void SetShaderView(bool state)
    {
        _shaderView.PerformIfNotNull(shaderView => shaderView.SetActive(state));
    }

    public void SetButtonType(ButtonColorType buttonColorType)
    {
        _buttonColorType = buttonColorType;
        _shaderView.PerformIfNotNull(shaderView => shaderView.SetButtonType(_buttonColorType));
    }

    public void ChangeViewColor(Color color)
    {
        _image.color = color;
    }

    public void ChangeSprite(Sprite mainSprite, Sprite shaderSprite)
    {
        Init();
        _image.sprite = mainSprite;
        _shaderView.ChangeSprite(shaderSprite);
    }

    public void ChangeLabelColor(Color color)
    {
        _label.PerformIfNotNull(label => label.color = color);
    }

    public void ChangeLabelOutline(Color color)
    {
        if (_label != null)
        {
            _labelFontMaterial.SetColor("_OutlineColor", color);
            _label.gameObject.SetActive(false);
            _label.gameObject.SetActive(true);
        }
    }

    public void SetDefaultColor()
    {
        _image.color = _gameColors.GetForButtonView(_buttonColorType);
        _label.PerformIfNotNull(label => label.color = _gameColors.GetForLabel());

        if (_label != null)
        {
            _labelFontMaterial.SetColor("_OutlineColor", _gameColors.GetForAccessibilityState()[false]);
            _label.gameObject.SetActive(false);
            _label.gameObject.SetActive(true);
        }
    }

    private void ChangeShadowColor(Color newColor, float duration)
    {
        _shadowTween = DOTween.To(
                () => _shadow.effectColor,
                x => _shadow.effectColor = x,
                newColor,
                duration
            ).SetEase(Ease.InOutQuad);
    }

    private void StopAllAnimations()
    {
        DOTween.Kill(_transform);

        if (_shadowTween != null && _shadowTween.IsActive())
        {
            _shadowTween.Kill();
            _shadowTween = null;
        }
    }
}