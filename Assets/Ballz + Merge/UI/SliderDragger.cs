using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDragger : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SliderHandle _sliderHandle;

    private bool _isDragging;

    public event Action<bool> Handled;

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnHandled(true);
        _sliderHandle.Press();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnHandled(false);
        _sliderHandle.Release();
    }

    private void OnHandled(bool state)
    {
        _isDragging = state;
        Handled?.Invoke(_isDragging);
        _sliderHandle.SetDraggingState(_isDragging);
    }
}
