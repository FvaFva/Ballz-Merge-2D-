using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow), typeof(Image))]
public class ButtonView : MonoBehaviour
{
    private const string BLEND_PROPERTY = "_BlendAmount";
    private const string COLOR_PROPERTY = "_Color";

    private RectTransform _transform;
    private Tween _shadowTween;
    private Image _image;
    private Material _imageMaterial;
    private Shadow _shadow;
    private TMP_Text _label;
    private Dictionary<ColorType, Color> _shadowColors;
    private Color _startMaterialColor;
    private Color _startLabelColor;

    private void OnDisable()
    {
        SetDefault();
    }

    public void Init()
    {
        _shadowColors = new Dictionary<ColorType, Color>();
        _transform = (RectTransform)transform;
        _shadow = GetComponent<Shadow>();
        _image = GetComponent<Image>();
        _imageMaterial = new Material(_image.material);
        _image.material = _imageMaterial;
        _label = GetComponentInChildren<TMP_Text>();
        _shadowColors.Add(ColorType.StartColor, _shadow.effectColor);
        _startMaterialColor = _imageMaterial.GetColor(COLOR_PROPERTY);
        _label.PerformIfNotNull(label => _startLabelColor = label.color);
        Color targetShadowColor = _shadow.effectColor;
        targetShadowColor.a = 1f;
        _shadowColors.Add(ColorType.TargetColor, targetShadowColor);
    }

    public void SetDefault()
    {
        StopAllAnimations();

        if (_imageMaterial.HasProperty(BLEND_PROPERTY))
            _imageMaterial.SetFloat(BLEND_PROPERTY, 0);

        _transform.localScale = Vector3.one;
        _shadow.effectColor = _shadowColors[ColorType.StartColor];
    }

    public void ChangeParameters(float newScale, ColorType colorType, float duration)
    {
        _transform.DOScale(newScale, duration);
        ChangeShadowColor(_shadowColors[colorType], duration);
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

    public void ChangeMaterialColor(Color color)
    {
        _imageMaterial.color = color;
    }

    public void ChangeLabelColor(Color color)
    {
        _label.PerformIfNotNull(label => label.color = color);
    }

    public void SetDefaultColor()
    {
        _imageMaterial.color = _startMaterialColor;
        _label.color = _startLabelColor;
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