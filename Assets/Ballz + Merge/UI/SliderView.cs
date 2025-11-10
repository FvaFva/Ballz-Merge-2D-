using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class SliderView : MonoBehaviour
{
    private Image _image;
    private Transform _transform;
    private Color _startViewColor;

    public void Init()
    {
        _transform = transform;
        _image = GetComponent<Image>();
        _startViewColor = _image.color;
    }

    public void ChangeViewColor(Color color)
    {
        _image.color = color;
    }

    public void SetDefaultColor()
    {
        _image.color = _startViewColor;
    }

    public void ChangeParameters(float newScale, float duration)
    {
        _transform.DOScale(new Vector3(newScale, 1f, 1f), duration).SetEase(Ease.InOutQuad);
    }
}
