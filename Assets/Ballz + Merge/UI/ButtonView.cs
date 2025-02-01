using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class ButtonView : MonoBehaviour
{
    private const string BLEND_PROPERTY = "_BlendAmount";

    private RectTransform _transform;
    private Tween _shadowTween;
    private Image _image;
    private Material _imageMaterial;
    private Shadow _shadow;
    private Dictionary<ColorType, Color> _colors;

    private void Awake()
    {
        _colors = new Dictionary<ColorType, Color>();
        _transform = (RectTransform)transform;
        _shadow = GetComponent<Shadow>();
        _image = GetComponent<Image>();
        _imageMaterial = new Material(_image.material);
        _image.material = _imageMaterial;
        _colors.Add(ColorType.StartColor, _shadow.effectColor);
        var targetColor = _shadow.effectColor;
        targetColor.a = 1f;
        _colors.Add(ColorType.TargetColor, targetColor);
    }

    private void OnDisable()
    {
        StopAllAnimations();
    }

    public void SetDefault()
    {
        if(isActiveAndEnabled == false) 
            return;

        StopAllAnimations();
        _imageMaterial?.SetFloat(BLEND_PROPERTY, 0);
        _transform.localScale = Vector3.one;
        _shadow.effectColor = _colors[ColorType.StartColor];
    }

    public void ChangeParameters(float newScale, ColorType colorType, float duration)
    {
        _transform.DOScale(newScale, duration);
        ChangeColor(_colors[colorType], duration);
    }

    public void ChangeMaterialBlend(float newValue, float duration)
    {
        _imageMaterial.DOFloat(newValue, BLEND_PROPERTY, duration).SetEase(Ease.InOutQuad);
    }

    private void ChangeColor(Color newColor, float duration)
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