using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonShaderView : MonoBehaviour
{
    private const string TEXTURE_COLOR_PROPERTY = "_TextureColor";
    private const string SHINY_COLOR_PROPERTY = "_ShinyColor";
    private const string SHINY_BRIGHTNESS_PROPERTY = "_ShinyBrightness";
    private const string MASK_PROPERTY = "_IsMask";

    [SerializeField] private bool _isMask = false;
    [SerializeField, Range(0, 10)] private float _shinyBrightness = 5f;

    private ButtonColorType _buttonColorType;
    private Image _image;
    private Material _imageMaterial;
    private GameColors _gameColors;
    private bool _isInited;

    public void Init(ButtonColorType buttonColorType = default)
    {
        if (_isInited)
            return;

        _isInited = true;
        _image = GetComponent<Image>();
        _imageMaterial = new Material(_image.material);
        _image.material = _imageMaterial;
        _buttonColorType = buttonColorType;
    }

    public void ApplyColors(GameColors colors)
    {
        _gameColors = colors;
        SetColors();
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void SetShinyColor(Color color)
    {
        _imageMaterial.PerformIfPropertyExist(SHINY_COLOR_PROPERTY, material => material.SetColor(SHINY_COLOR_PROPERTY, color));
    }

    public void ChangeSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetButtonType(ButtonColorType buttonColorType)
    {
        _buttonColorType = buttonColorType;
        SetColors();
    }

    private void SetColors()
    {
        _imageMaterial.PerformIfPropertyExist(SHINY_COLOR_PROPERTY, material => material.SetColor(SHINY_COLOR_PROPERTY, _gameColors.GetForButtonShader(_buttonColorType)));
        _imageMaterial.PerformIfPropertyExist(TEXTURE_COLOR_PROPERTY, material => material.SetColor(TEXTURE_COLOR_PROPERTY, _gameColors.GetForTexture(0.75f)));
        _imageMaterial.PerformIfPropertyExist(SHINY_BRIGHTNESS_PROPERTY, material => material.SetFloat(SHINY_BRIGHTNESS_PROPERTY, _shinyBrightness));
        _imageMaterial.PerformIfPropertyExist(MASK_PROPERTY, material => material.SetFloat(MASK_PROPERTY, _isMask == false ? 0 : 1));
    }
}
