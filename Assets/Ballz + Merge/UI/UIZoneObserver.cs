using UnityEngine;
using UnityEngine.EventSystems;

public class UIZoneObserver : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsIn { get; private set; }

    public RectTransform Transform { get; private set; }

    private void Awake()
    {
        Transform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        IsIn = false;
    }

    public void OnPointerDown(PointerEventData eventData) => IsIn = true;

    public void OnPointerEnter(PointerEventData eventData) => IsIn = true;

    public void OnPointerExit(PointerEventData eventData) => IsIn = false;

    public Vector2 GetCenterInWorld(Camera uiCamera)
    {
        var temp = Transform.TransformPoint(Transform.rect.center.x, Transform.rect.yMax, 0);
        return RectTransformUtility.WorldToScreenPoint(uiCamera, temp);
    }

    public bool IsPointIn(Camera uiCamera, Vector2 point)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(Transform, point, uiCamera);
    }
}