using UnityEngine;

public class PlayZoneBoard : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _view;

    private Transform _transform;
    private Vector2 _basePosition;
    private Vector2 _baseScale;

    public void MarkAsVirtual()
    {
        _view.enabled = false;
    }

    public void Init()
    {
        _transform = transform;
        _basePosition = _transform.localPosition;
        _baseScale = _transform.localScale;
    }

    public void SetBase()
    {
        _transform.localPosition = _basePosition;
        _transform.localScale = _baseScale;
    }

    public void AddProperty(PositionScaleProperty property)
    {
        _transform.localPosition += property.Position;
        _transform.localScale += property.Scale;
    }

    public void Move(Vector3 step)
    {
        _transform.localPosition += step;
    }
}