using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class ImageView : MonoBehaviour
{
    private Shadow _shadow;
    private Transform _transform;
    private Color _shadowColor;
    private Tween _shadowTween;
    private Vector3 _startScale;
    private float _startAlpha;

    private void Awake()
    {
        _transform = transform;
        _shadow = GetComponent<Shadow>();
        _shadowColor = _shadow.effectColor;
        _startScale = _transform.localScale;
        _startAlpha = _shadow.effectColor.a;
    }

    private void OnDisable()
    {
        StopAllAnimations();
    }

    public void SetSize(float value, float duration)
    {
        _transform.DOScale(value, duration);
    }

    public void SetSize(Vector2 size)
    {
        _transform.localScale = size;
    }

    public void SetShadow(float value, float duration)
    {
        _shadowTween = DOTween.To(
                () => _shadow.effectColor,
                x => _shadow.effectColor = x,
                new Color(_shadowColor.r, _shadowColor.g, _shadowColor.b, value),
                duration
            ).SetEase(Ease.InOutQuad);
    }

    public void SetShadow(float value)
    {
        _shadow.effectColor = new Color(_shadowColor.r, _shadowColor.g, _shadowColor.b, value);
    }

    private void StopAllAnimations()
    {
        DOTween.Kill(_transform);
        _transform.localScale = _startScale;
        _shadow.effectColor = new Color(_shadowColor.r, _shadowColor.g, _shadowColor.b, _startAlpha);

        if (_shadowTween != null && _shadowTween.IsActive())
        {
            _shadowTween.Kill();
            _shadowTween = null;
        }
    }
}
