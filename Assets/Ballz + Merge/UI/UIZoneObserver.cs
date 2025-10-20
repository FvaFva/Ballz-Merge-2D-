using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIZoneObserver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _zoneView;

    private Transform _transform;

    public event Action<bool> Entered;

    private void Awake()
    {
        _transform = transform;
    }

    public void OnPointerEnter(PointerEventData eventData) => Entered?.Invoke(true);

    public void OnPointerExit(PointerEventData eventData) => Entered?.Invoke(false);

    public void SetState(bool state)
    {
        _zoneView.SetActive(state);
    }

    public void SetSize(float scale, float duration) => _transform.DOScale(scale, duration);
}