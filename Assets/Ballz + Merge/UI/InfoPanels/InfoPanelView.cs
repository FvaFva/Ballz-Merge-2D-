using UnityEngine;

public class InfoPanelView : MonoBehaviour, IInfoPanelView
{
    private RectTransform _transform;
    private RectTransform _baseParent;

    public void Start()
    {
        _transform = (RectTransform)transform;
        _baseParent = (RectTransform)_transform.parent;
        gameObject.SetActive(false);
    }

    public void Show(RectTransform showcase)
    {
        _transform.SetParent(showcase);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _transform.SetParent(_baseParent);
        gameObject.SetActive(false);
    }
}