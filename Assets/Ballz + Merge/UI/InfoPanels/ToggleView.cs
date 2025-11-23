using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Shadow))]
public class ToggleView : MonoBehaviour
{
    [SerializeField] private ButtonColorType _startColor;
    [SerializeField] private ButtonColorType _newColor;
    [SerializeField] private UIParticle _particle;

    private Image _image;
    private GameColors _gameColors;
    private bool _isSelected;

    private void OnEnable()
    {
        if (_isSelected)
            Select();
    }

    public void Initialize()
    {
        _image = GetComponent<Image>();
        _particle.Init();
    }

    public void ApplyColors(GameColors gameColors = null)
    {
        _gameColors = gameColors;
    }

    public void Select()
    {
        _image.color = _gameColors.GetForButton(_newColor);
        _particle.gameObject.SetActive(true);
        _particle.Play();
        _isSelected = true;
    }

    public void Unselect()
    {
        _image.color = _gameColors.GetForButton(_startColor);
        _particle.Stop();
        _particle.gameObject.SetActive(false);
        _isSelected = false;
    }
}