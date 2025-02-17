using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Shadow))]
public class PanelToggleView : MonoBehaviour
{
    [SerializeField] private Color _newColor;

    private Image _image;
    private Shadow _shadow;
    private Color _imageColor;
    private Color _shadowColor;
    private Color _startColor;

    public event Action Initialized;

    public void Initialize()
    {
        _shadow = GetComponent<Shadow>();
        _image = GetComponent<Image>();
        _imageColor = _image.color;
        _startColor = _imageColor;
        _shadowColor = _shadow.effectColor;
        Initialized?.Invoke();
    }

    public void Select()
    {
        _imageColor = _newColor;
        _shadowColor.a = 1f;
        ApplyChanges();
    }

    public void Unselect()
    {
        _imageColor = _startColor;
        _shadowColor.a = 0f;
        ApplyChanges();
    }

    private void ApplyChanges()
    {
        _image.color = _imageColor;
        _shadow.effectColor = _shadowColor;
    }
}