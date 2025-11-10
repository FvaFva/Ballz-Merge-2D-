using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow), typeof(Image))]
public class ButtonView : MonoBehaviour
{
    private RectTransform _transform;
    private Tween _shadowTween;
    private Image _image;
    private Shadow _shadow;
    private TMP_Text _label;
    private ButtonShaderView _shaderView;
    private Dictionary<ColorType, Color> _shadowColors;
    private Color _startViewColor;
    private Color _startLabelColor;

    public void Init()
    {
        _shadowColors = new Dictionary<ColorType, Color>();
        _transform = (RectTransform)transform;
        _shadow = GetComponent<Shadow>();
        _image = GetComponent<Image>();
        _label = GetComponentInChildren<TMP_Text>();
        _shaderView = GetComponentInChildren<ButtonShaderView>(true);
        _shadowColors.Add(ColorType.StartColor, _shadow.effectColor);
        _startViewColor = _image.color;
        _label.PerformIfNotNull(label => _startLabelColor = label.color);
        Color targetShadowColor = _shadow.effectColor;
        targetShadowColor.a = 1f;
        _shadowColors.Add(ColorType.TargetColor, targetShadowColor);
        _shaderView.PerformIfNotNull(shaderView => shaderView.Init());
    }

    public void SetDefault()
    {
        StopAllAnimations();

        SetShaderView(false);

        _transform.localScale = Vector3.one;
        _shadow.effectColor = _shadowColors[ColorType.StartColor];
    }

    public void ChangeParameters(float newScale, ColorType colorType, float duration)
    {
        _transform.DOScale(newScale, duration);
        ChangeShadowColor(_shadowColors[colorType], duration);
    }

    public void SetShaderView(bool state)
    {
        _shaderView.PerformIfNotNull(shaderView => shaderView.SetActive(state));
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

    public void ChangeViewColor(Color color)
    {
        _image.color = color;
    }

    public void ChangeSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void ChangeLabelColor(Color color)
    {
        _label.PerformIfNotNull(label => label.color = color);
    }

    public void SetDefaultColor()
    {
        _image.color = _startViewColor;
        _label.PerformIfNotNull(label => label.color = _startLabelColor);
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