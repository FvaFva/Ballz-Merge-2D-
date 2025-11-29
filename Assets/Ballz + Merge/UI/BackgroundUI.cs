using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundUI : DependentColorUI
{
    [SerializeField] private BackgroundColorType _backgroundColorType;

    private Image _image;
    private bool _isInited;

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;
        Init();
        float alpha = _image.color.a;
        Color color = GameColors.GetForBackground(_backgroundColorType);
        color.a = alpha;
        _image.color = color;
        
    }

    private void Init()
    {
        if (_isInited)
            return;

        _isInited = true;
        _image = GetComponent<Image>();
    }
}