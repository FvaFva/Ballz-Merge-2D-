using UnityEngine;

public class InfoPanelView : MonoBehaviour, IInfoPanelView
{
    private RectTransform _transform;
    private RectTransform _baseParent;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        _transform = (RectTransform)transform;
        _baseParent = (RectTransform)_transform.parent;
    }

    public void ChangeAnchors(Vector2 minAnchor, Vector2 maxAnchor)
    {
        _transform.anchorMin = minAnchor;
        _transform.anchorMax = maxAnchor;
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