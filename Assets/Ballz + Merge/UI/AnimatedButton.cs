using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private const float Duration = 0.125f;
    private const float PressedStateScale = 0.9f;
    private const float HighlightedStateScale = 1.05f;
    private const float StartScale = 1f;

    [SerializeField] private ButtonView _buttonView;

    private Transform _transform;
    private bool _isPointerDown;
    private bool _isPointerEnter;

    private void Awake()
    {
        _transform = transform;
        _isPointerDown = false;
        _isPointerEnter = false;
    }

    private void OnDisable()
    {
        _transform.localScale = Vector3.one * StartScale;
        DOTween.Kill(_transform);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        _buttonView.ChangeParameters(PressedStateScale, ColorType.StartColor, Duration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;

        if (_isPointerEnter)
            _transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
        else
            _transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);

        _buttonView.ChangeParameters(StartScale, ColorType.StartColor, Duration);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerEnter = true;

        if (_isPointerDown || isActiveAndEnabled == false)
            return;

        _transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
        _buttonView.ChangeParameters(StartScale, ColorType.TargetColor, Duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;

        if (_isPointerDown || isActiveAndEnabled == false)
            return;

        _transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);
        _buttonView.ChangeParameters(StartScale, ColorType.StartColor, Duration);
    }
}