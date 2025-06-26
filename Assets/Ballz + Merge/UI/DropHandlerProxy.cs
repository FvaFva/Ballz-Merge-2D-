using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandlerProxy : MonoBehaviour, IDropHandler
{
    public event Action<PointerEventData> Dropped;

    public void OnDrop(PointerEventData eventData)
    {
        Dropped?.Invoke(eventData);
    }
}
