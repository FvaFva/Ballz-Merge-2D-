using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow), typeof(Image))]
public class ButtonView : MonoBehaviour
{
    [SerializeField] private ButtonColorType _buttonColorType;
    [SerializeField] private ShadowColorType _shadowColorType;

    private RectTransform _transform;
    private Tween _shadowTween;
    private Image _image;
    private Shadow _shadow;
    private TMP_Text _label;
    private ButtonShaderView _shaderView;
    private Dictionary<ColorType, Color> _shadowColors;
    private GameColors _gameColors;

    private bool _isInited;
    private bool _isActive;

    public bool IsActive => _isActive;

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
        _shaderView = GetComponentInChildren<ButtonShaderView>(true);
        _shaderView.PerformIfNotNull(shaderView => shaderView.Init());

        _shadowColors = new Dictionary<ColorType, Color>
        {
            { ColorType.StartColor, _image.color },
            { ColorType.TargetColor, _image.color }
        };
    }

    public void SetActiveState(bool state)
    {
        _isActive = state;
    }

    public void ApplyColors(GameColors colors = null)
    {
        _gameColors = colors;

        if (_isActive)
            _image.color = _gameColors.GetForButton(_buttonColorType);

        _shadowColors = new Dictionary<ColorType, Color>
        {
            { ColorType.StartColor, _gameColors.GetForShadow(_shadowColorType, 0f) },
            { ColorType.TargetColor, _gameColors.GetForShadow(_shadowColorType, 1f) }
        };
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

    public void SetDefaultColor()
    {
        _image.color = _gameColors.GetForButton(_buttonColorType);
        _label.PerformIfNotNull(label => label.color = _gameColors.GetForLabel());
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