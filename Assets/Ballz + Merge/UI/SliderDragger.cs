using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDragger : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public event Action<bool> Handled;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Handled?.Invoke(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Handled?.Invoke(false);
    }
}
