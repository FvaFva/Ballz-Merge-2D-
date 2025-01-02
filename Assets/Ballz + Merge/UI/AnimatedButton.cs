using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private const float Duration = 0.125f;
    private const float PressedStateScale = 0.9f;
    private const float HighlightedStateScale = 1.05f;
    private const float StartScale = 1f;

    private RectTransform _transform;
    private Shadow _shadow;
    private Color _startColor;
    private Color _targetColor;
    private bool _isPointerDown;
    private bool _isPointerEnter;

    private void Start()
    {
        _transform = (RectTransform)transform;
        _shadow = GetComponent<Shadow>();
        _startColor = _shadow.effectColor;
        _targetColor = _startColor;
        _targetColor.a = 1f;
        _isPointerDown = false;
        _isPointerEnter = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        ChangeParameters(PressedStateScale, _startColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;

        if (_isPointerEnter)
            ChangeParameters(HighlightedStateScale, _targetColor);
        else
            ChangeParameters(StartScale, _startColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerEnter = true;

        if (_isPointerDown)
            return;

        ChangeParameters(HighlightedStateScale, _targetColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;

        if (_isPointerDown)
            return;

        ChangeParameters(StartScale, _startColor);
    }

    private void ChangeParameters(float newScale, Color newColor)
    {
        _transform.DOScale(newScale, Duration).SetEase(Ease.InOutQuad);

        DOTween.To(
            () => _shadow.effectColor,           // Текущее значение цвета
            x => _shadow.effectColor = x,       // Применяем новое значение
            newColor,                       // Целевой цвет
            Duration                           // Длительность
        ).SetEase(Ease.InOutQuad);
    }
}