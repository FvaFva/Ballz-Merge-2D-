using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private const float Duration = 0.125f;
    private const float PressedStateScale = 0.9f;
    private const float HighlightedStateScale = 1.05f;
    private const float StartScale = 1f;

    [SerializeField] private ButtonView _buttonView;
    [SerializeField] private RectPumper _pumper;
    [SerializeField] private RectTransform _header;

    private bool _isPointerDown;
    private bool _isPointerEnter;
    private Transform _transform;
    private Action<bool> _viewChanger = (bool state) => { };

    private void Awake()
    {
        _transform = transform;
        _isPointerDown = false;
        _isPointerEnter = false;
        _buttonView.Init();
        _buttonView.SetDefault();

        if (_pumper != null && _header != null)
            _viewChanger = (bool state) => { _pumper.enabled = state; _header.gameObject.SetActive(state); };
        else if(_pumper != null)
            _viewChanger = (bool state) => { _pumper.enabled = state; };
        else if (_header != null)
            _viewChanger = (bool state) => { _header.gameObject.SetActive(state); };
    }

    private void OnEnable()
    {
        _transform.localScale = Vector3.one * StartScale;
        _viewChanger(true);
        _buttonView.enabled = true;
    }

    private void OnDisable()
    {
        DOTween.Kill(_transform);
        _viewChanger(false);
        _buttonView.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        _buttonView.ChangeBlendMaterial(0f, Duration);
        _buttonView.ChangeParameters(PressedStateScale, ColorType.StartColor, Duration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;

        if (_isPointerEnter)
        {
            _transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
            _buttonView.ChangeParameters(StartScale, ColorType.TargetColor, Duration);
            _buttonView.ChangeBlendMaterial(1f, Duration);
        }
        else
        {
            _transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);
            _buttonView.ChangeParameters(StartScale, ColorType.StartColor, Duration);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerEnter = true;

        if (_isPointerDown)
            return;

        _transform.DOScale(HighlightedStateScale, Duration).SetEase(Ease.InOutQuad);
        _buttonView.ChangeBlendMaterial(1f, Duration);
        _buttonView.ChangeParameters(StartScale, ColorType.TargetColor, Duration);
        _viewChanger(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;

        if (_isPointerDown)
            return;

        _transform.DOScale(StartScale, Duration).SetEase(Ease.InOutQuad);
        _buttonView.ChangeBlendMaterial(0f, Duration);
        _buttonView.ChangeParameters(StartScale, ColorType.StartColor, Duration);
        _viewChanger(true);
    }
}