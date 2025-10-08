using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Shadow))]
public class PanelToggleView : MonoBehaviour
{
    [SerializeField] private Color _newColor;

    private Image _image;
    private Color _startColor;

    public void Initialize()
    {
        _image = GetComponent<Image>();
        _startColor = _image.color;
    }

    public void Select()
    {
        _image.color = _newColor;
    }

    public void Unselect()
    {
        _image.color = _startColor;
    }
}