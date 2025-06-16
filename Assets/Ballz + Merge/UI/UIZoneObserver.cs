using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIZoneObserver : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsIn { get; private set; }
    public event Action<bool> IsPressed;
    public RectTransform Transform { get; private set; }

    private void Awake()
    {
        Transform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        IsIn = false;
    }
    
    public void OnPointerDown(PointerEventData eventData) => IsPressed?.Invoke(true);

    public void OnPointerUp(PointerEventData eventData) => IsPressed?.Invoke(false);

    public void OnPointerEnter(PointerEventData eventData) => IsIn = true;

    public void OnPointerExit(PointerEventData eventData) => IsIn = false;

    public Vector2 GetCenterInWorld(Camera uiCamera)
    {
        var temp = Transform.TransformPoint(Transform.rect.center.x, Transform.rect.yMax, 0);
        return RectTransformUtility.WorldToScreenPoint(uiCamera, temp);
    }
}