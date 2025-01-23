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

    private bool _isPointerDown;
    private bool _isPointerEnter;

    private void Start()
    {
        _isPointerDown = false;
        _isPointerEnter = false;
    }

    private void OnDisable()
    {
        transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);
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
            transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
        else
            transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);

        _buttonView.ChangeParameters(StartScale, ColorType.StartColor, Duration);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerEnter = true;

        if (_isPointerDown)
            return;

        transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
        _buttonView.ChangeParameters(StartScale, ColorType.TargetColor, Duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;

        if (_isPointerDown)
            return;

        transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);
        _buttonView.ChangeParameters(StartScale, ColorType.StartColor, Duration);
    }
}