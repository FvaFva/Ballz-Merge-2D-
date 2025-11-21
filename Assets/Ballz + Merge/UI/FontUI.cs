using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class FontUI : DependentColorUI
{
    private TMP_Text _text;
    private bool _isInited;

    public override void ApplyColors(GameColors gameColors)
    {
        GameColors = gameColors;
        Init();

        Material material = new Material(_text.fontMaterial);
        _text.fontMaterial = material;
        material.SetColor("_OutlineColor", GameColors.GetForBackground(BackgroundColorType.Outline));
    }

    private void Init()
    {
        if (_isInited)
            return;

        _isInited = true;
        _text = GetComponent<TMP_Text>();
    }
}