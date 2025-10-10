using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Shadow))]
public class PanelToggleView : MonoBehaviour
{
    [SerializeField] private Color _newColor;
    [SerializeField] private UIParticle _particle;

    private Image _image;
    private Color _startColor;
    private bool _isSelected;

    private void OnEnable()
    {
        if (_isSelected)
            Select();
    }

    public void Initialize()
    {
        _image = GetComponent<Image>();
        _startColor = _image.color;
        _particle.Init();
    }

    public void Select()
    {
        _image.color = _newColor;
        _particle.gameObject.SetActive(true);
        _particle.Play();
        _isSelected = true;
    }

    public void Unselect()
    {
        _image.color = _startColor;
        _particle.Stop();
        _particle.gameObject.SetActive(false);
        _isSelected = false;
    }
}