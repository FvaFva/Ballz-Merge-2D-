using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow), typeof(Image))]
public class ButtonView : MonoBehaviour
{
    private const string BLEND_PROPERTY = "_BlendAmount";

    private RectTransform _transform;
    private Tween _shadowTween;
    private Image _image;
    private Material _imageMaterial;
    private Shadow _shadow;
    private Dictionary<ColorType, Color> _colors;

    public event Action Initialized;

    private void Awake()
    {
        _colors = new Dictionary<ColorType, Color>();
        _transform = (RectTransform)transform;
        _shadow = GetComponent<Shadow>();
        _image = GetComponent<Image>();
        _imageMaterial = new Material(_image.material);
        _image.material = _imageMaterial;
        _colors.Add(ColorType.StartColor, _shadow.effectColor);
        Color targetColor = _shadow.effectColor;
        targetColor.a = 1f;
        _colors.Add(ColorType.TargetColor, targetColor);
    }

    private void OnEnable()
    {
        Initialized?.Invoke();
    }

    private void OnDisable()
    {
        StopAllAnimations();
    }

    public void SetDefault()
    {
        StopAllAnimations();

        if (_imageMaterial.HasProperty(BLEND_PROPERTY))
            _imageMaterial.SetFloat(BLEND_PROPERTY, 0);

        _transform.localScale = Vector3.one;
        _shadow.effectColor = _colors[ColorType.StartColor];
    }

    public void ChangeParameters(float newScale, ColorType colorType, float duration)
    {
        _transform.DOScale(newScale, duration);
        ChangeShadowColor(_colors[colorType], duration);
    }

    public void ChangeBlendMaterial(float newValue, float duration)
    {
        if (_imageMaterial.HasProperty(BLEND_PROPERTY))
            _imageMaterial.DOFloat(newValue, BLEND_PROPERTY, duration).SetEase(Ease.InOutQuad);
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
        DOTween.Kill(_imageMaterial);

        if (_shadowTween != null && _shadowTween.IsActive())
        {
            _shadowTween.Kill();
            _shadowTween = null;
        }
    }
}