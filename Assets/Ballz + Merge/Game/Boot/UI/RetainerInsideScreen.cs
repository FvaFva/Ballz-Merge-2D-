using UnityEngine;

public class RetainerInsideScreen : MonoBehaviour
{
    [SerializeField] private Canvas _mainCanvas;

    private Vector2 _canvasMin;
    private Vector2 _canvasMax;
    private Vector3 _initialPosition;
    private RectTransform _transformCanvas;
    private RectTransform _transform;
    private bool _isShifted;

    private void Awake()
    {
        _transformCanvas = (RectTransform)_mainCanvas.transform;
        _transform = (RectTransform)transform;
        _initialPosition = _transform.anchoredPosition;
        _canvasMax = new Vector2(_transformCanvas.rect.width * 0.5f, _transformCanvas.rect.height * 0.5f);
        _canvasMin = _canvasMax * -1;
    }

    public void UpdatePosition()
    {
        Vector2 offset = CalculateOffset();

        if (offset != Vector2.zero)
        {
            _transform.anchoredPosition += offset;
            _isShifted = true;
        }
        else if (_isShifted)
        {
            _transform.anchoredPosition = _initialPosition;
            _isShifted = false;
        }
    }

    private Vector2 CalculateOffset()
    {
        Vector2 offset = Vector2.zero;
        _transform.anchoredPosition = _initialPosition;
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_transformCanvas, _transform);

        if (bounds.min.x < _canvasMin.x)
            offset.x = _canvasMin.x - bounds.min.x;
        else if (bounds.max.x > _canvasMax.x)
            offset.x = _canvasMax.x - bounds.max.x;

        if (bounds.min.y < _canvasMin.y)
            offset.y = _canvasMin.y - bounds.min.y;
        else if (bounds.max.y > _canvasMax.y)
            offset.y = _canvasMax.y - bounds.max.y;

        return offset;
    }
}