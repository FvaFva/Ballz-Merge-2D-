using BallzMerge.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameColors : IGameSettingData
{
    public const float Saturation = 0.75f;
    public const float BaseLight = 0.55f;

    public string Name { get; private set; }
    public float Value { get; private set; }
    public string Label { get; private set; }
    public int? CountOfPresets { get; private set; }

    private Color Main = new(0.4470588f, 0.2980392f, 1f); //#724CFF
    private Color Background = new(0.5254902f, 0.5490196f, 1f); //#868CFF
    private Color Fill = new(0.372549f, 0.007843138f, 0.6588235f); //#5F02A8
    private Color UI = new(0f, 0.5058824f, 0.7647059f); //#0081C3
    private Color ShadowUI = new(0.01960782f, 0.8652754f, 0.9254902f); //#05DDEC
    private Color FontOutline = new(0.4286905f, 0.4396572f, 0.6866854f); //HDR

    private readonly Color White = new(1f, 1f, 1f); //#FFFFFF
    private readonly Color Black = new(0f, 0f, 0f); //#000000
    private readonly Color LightWhite = new(0.8117647f, 0.8235294f, 1f); //#CFD2FF
    private readonly Color Green = new(0.5607843f, 0.9843137f, 0.3686275f); //#8FFB5E
    private readonly Color ShadowGreen = new(0.6933428f, 0.9245283f, 0.5913492f); //#B1EC97
    private readonly Color Red = new(0.9176471f, 0.345098f, 0.345098f); //#EA5858
    private readonly Color ShadowRed = new(0.8641509f, 0.5788181f, 0.5788181f); //#DC9494
    private readonly Color Close = new(0.5471698f, 0.004208999f, 0f); //#8C0100
    private readonly Color ShadowClose = new(1f, 0f, 0f); //#FF0000

    private Dictionary<ShadowColorType, Color> _shadowColors;
    private Dictionary<ButtonColorType, Color> _buttonColors;
    private Dictionary<BackgroundColorType, Color> _backgroundColors;
    private Dictionary<DataViewType, Color> _dataViewColors;

    public event Action<bool> StateChanged;
    public event Action Changed;

    public GameColors(string name)
    {
        Name = name;
        ApplyColors();

        _dataViewColors = new Dictionary<DataViewType, Color>
        {
            { DataViewType.Complete, Green },
            { DataViewType.Lost, Red }
        };
    }

    public void Load(float value)
    {
        Value = value;
        Main = Palette.FromHSL(Value, Saturation, BaseLight);
        Background = Palette.FromHSL(Value, Saturation, 0.85f);
        Fill = Palette.FromHSL(Value, Saturation, 0.20f);
        UI = GenerateAccent(Main, Value, Saturation);
        ShadowUI = Palette.FromHSL(ComputeAccentHue(Value), Saturation, 0.22f);
        FontOutline = Palette.FromHSL(Value, Saturation, 0.85f);
        ApplyColors();
    }

    public void Change(float value)
    {
        Value = value;
        Main = Palette.FromHSL(Value, Saturation, BaseLight);
        Background = Palette.FromHSL(Value, Saturation, 0.85f);
        Fill = Palette.FromHSL(Value, Saturation, 0.20f);
        UI = GenerateAccent(Main, Value, Saturation);
        ShadowUI = Palette.FromHSL(ComputeAccentHue(Value), Saturation, 0.22f);
        FontOutline = Palette.FromHSL(Value, Saturation, 0.85f);
        ApplyColors();
        Changed?.Invoke();
    }

    public Color GetForButton(ButtonColorType buttonColorType)
    {
        return _buttonColors[buttonColorType];
    }

    public Color GetForSliderHandle()
    {
        return Fill;
    }

    public Color GetForBackground(BackgroundColorType backgroundColorType)
    {
        return _backgroundColors[backgroundColorType];
    }

    public Color GetForLabel()
    {
        return White;
    }

    public Color GetForShadow(ShadowColorType shadowColorType, float alpha)
    {
        Color color = _shadowColors[shadowColorType];
        color.a = alpha;
        return color;
    }

    public Color GetForStick(float alpha)
    {
        Color color = Black;
        color.a = alpha;
        return color;
    }

    public Color GetForDataView(DataViewType dataViewType)
    {
        return _dataViewColors[dataViewType];
    }

    public Dictionary<bool, Color> GetForAccessibilityState()
    {
        return new Dictionary<bool, Color> { { false, Black }, { true, White } };
    }

    public Dictionary<bool, Color> GetForAccessibilitySliderState()
    {
        return new Dictionary<bool, Color> { { false, White }, { true, Fill } };
    }

    private void ApplyColors()
    {
        _shadowColors = new Dictionary<ShadowColorType, Color>
        {
            { ShadowColorType.UI, ShadowUI },
            { ShadowColorType.Green, ShadowGreen },
            { ShadowColorType.Red, ShadowRed },
            { ShadowColorType.Close, ShadowClose }
        };

        _buttonColors = new Dictionary<ButtonColorType, Color>
        {
            { ButtonColorType.Main, Main },
            { ButtonColorType.UI, UI },
            { ButtonColorType.Green, Green },
            { ButtonColorType.Red, Red },
            { ButtonColorType.Close, Close }
        };

        _backgroundColors = new Dictionary<BackgroundColorType, Color>
        {
            { BackgroundColorType.Main, Main },
            { BackgroundColorType.Secondary, Background },
            { BackgroundColorType.Fill, Fill },
            { BackgroundColorType.LightFill, LightWhite },
            { BackgroundColorType.Outline, FontOutline }
        };
    }

    private float ComputeAccentHue(float h)
    {
        // Красный диапазон (очень высокая путаница оттенков)
        if (h < 0.055f || h > 0.945f)
            return Palette.WrapHue(h + 0.11f);  // +40°

        // Желтовато-зелёный
        if (h >= 0.10f && h < 0.28f)
            return Palette.WrapHue(h - 0.07f);  // -25°

        // Чистый зелёный (самая слабая контрастность в природе)
        if (h >= 0.28f && h < 0.42f)
            return Palette.WrapHue(h - 0.10f);  // -35°

        // Бирюза / Голубой — высокая близость контрастов
        if (h >= 0.42f && h < 0.55f)
            return Palette.WrapHue(h + 0.08f);  // +30°

        // Белые зоны не требуют коррекции
        return Palette.WrapHue(h + 0.05f);  // +18°
    }

    private Color GenerateAccent(Color baseColor, float h, float saturation)
    {
        float accentHue = ComputeAccentHue(h);
        float accentLightness = 0.55f;

        Color accent = Palette.FromHSL(accentHue, saturation, accentLightness);

        // Гарантируем контраст
        if (WCAG.Contrast(accent, baseColor) < 4.5f)
        {
            float l1 = accentLightness + 0.20f;
            float l2 = accentLightness - 0.20f;

            Color brighter = Palette.FromHSL(accentHue, saturation, Mathf.Clamp01(l1));
            Color darker = Palette.FromHSL(accentHue, saturation, Mathf.Clamp01(l2));

            // выбираем лучший вариант
            accent = (WCAG.Contrast(brighter, baseColor) > WCAG.Contrast(darker, baseColor))
                ? brighter
                : darker;
        }

        return accent;
    }

}