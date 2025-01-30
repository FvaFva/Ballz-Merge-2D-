using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class ButtonView : MonoBehaviour
{
    private RectTransform _transform;
    private Tween _shadowTween;
    private Shadow _shadow;
    private Color _startColor;
    private Color _targetColor;

    private void Start()
    {
        _transform = (RectTransform)transform;
        _shadow = GetComponent<Shadow>();
        _startColor = _shadow.effectColor;
        _targetColor = _startColor;
        _targetColor.a = 1f;
    }

    private void OnDisable()
    {
        DOTween.Kill(_transform);

        if (_shadowTween != null && _shadowTween.IsActive())
        {
            _shadowTween.Kill();
            _shadowTween = null;
        }
    }

    public void ChangeParameters(float newScale, ColorType colorType, float duration)
    {
        if (isActiveAndEnabled == false)
            return;

        _transform.DOScale(newScale, duration);

        if (colorType == ColorType.StartColor)
            ChangeColor(_startColor, duration);
        else if (colorType == ColorType.TargetColor)
            ChangeColor(_targetColor, duration);
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
}