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
    private Color ShadowMain = new(0.01960782f, 0.8652754f, 0.9254902f); //#05DDEC
    private Color ShadowUI = new(0.01960782f, 0.8652754f, 0.9254902f); //#05DDEC
    private Color ButtonMainShader = new(0f, 1f, 1f); //#00FFFF
    private Color ButtonUIShader = new(0.01960784f, 0.8666667f, 0.9254902f); //#05DDEC
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
    private readonly Dictionary<DataViewType, Color> _dataViewColors;

    private Dictionary<ButtonColorType, Color> _shadowColors;
    private Dictionary<ButtonColorType, Color> _buttonViewColors;
    private Dictionary<BackgroundColorType, Color> _backgroundColors;
    private Dictionary<ButtonColorType, Color> _buttonShaderColors;
    private bool _isAccent;
    private float _previousValue;

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
        if (_isAccent)
            return;

        Value = value;
        SetColors(Value);
    }

    public void Change(float value)
    {
        Value = value;
        SetColors(Value);
        Changed?.Invoke();
    }

    public void ChangeState(bool state)
    {
        StateChanged?.Invoke(state);
    }

    public void GetAndroidDynamicAccent()
    {
        _isAccent = true;
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var resources = activity.Call<AndroidJavaObject>("getResources"))
        using (var theme = activity.Call<AndroidJavaObject>("getTheme"))
        {
            // Попытка получить динамический цвет Android 12+
            int id = resources.Call<int>("getIdentifier",
                "system_accent1_500", // основной динамический акцент
                "color",
                "android");

            if (id != 0)
            {
                int colorInt = resources.Call<int>("getColor", id, theme);

                float a = ((colorInt >> 24) & 0xff) / 255f;
                float r = ((colorInt >> 16) & 0xff) / 255f;
                float g = ((colorInt >> 8) & 0xff) / 255f;
                float b = (colorInt & 0xff) / 255f;

                ApplyMainColor(r, g, b, a);
            }
            else
            {
                // Fallback: старый Accent (до Android 12)
                using (var typedValue = new AndroidJavaObject("android.util.TypedValue"))
                {
                    int colorAccentAttr = resources.GetStatic<int>("com.android.internal.R$attr.colorAccent");

                    if (theme.Call<bool>("resolveAttribute", colorAccentAttr, typedValue, true))
                    {
                        int colorInt = typedValue.Get<int>("data");

                        float a = ((colorInt >> 24) & 0xff) / 255f;
                        float r = ((colorInt >> 16) & 0xff) / 255f;
                        float g = ((colorInt >> 8) & 0xff) / 255f;
                        float b = (colorInt & 0xff) / 255f;

                        ApplyMainColor(r, g, b, a);
                    }
                }
            }
        }
    }


    public Color GetForButtonView(ButtonColorType buttonColorType)
    {
        return _buttonViewColors[buttonColorType];
    }

    public Color GetForButtonShader(ButtonColorType buttonColorType)
    {
        return _buttonShaderColors[buttonColorType];
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

    public Color GetForShadow(ButtonColorType buttonColorType, float alpha)
    {
        Color color = _shadowColors[buttonColorType];
        color.a = alpha;
        return color;
    }

    public Color GetForTexture(float alpha)
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

    public void ReturnGameColor()
    {
        if (!_isAccent)
            return;

        Value = _previousValue;
        SetColors(Value);
        _isAccent = false;
    }

    private void ApplyMainColor(float r, float g, float b, float a)
    {
        Main = new Color(r, g, b, a);
        Color.RGBToHSV(Main, out float H, out float S, out float V);
        _previousValue = Value;
        SetColors(H);
    }

    private void SetColors(float value)
    {
        Main = Palette.FromHSL(value, Saturation, BaseLight);
        Background = Palette.FromHSL(value, Saturation, 0.85f);
        Fill = Palette.FromHSL(value, Saturation, 0.20f);
        UI = GenerateAccent(Main, value, Saturation);
        ShadowUI = Palette.FromHSL(ComputeAccentHue(value), Saturation, 0.22f);
        ShadowMain = Palette.FromHSL(value, Saturation, 0.22f);
        FontOutline = Palette.FromHSL(value, Saturation, 0.20f);
        ButtonMainShader = Palette.FromHSL(value, Saturation, 0.6f);
        ButtonUIShader = Palette.FromHSL(ComputeAccentHue(value), Saturation, 0.52f);
        ApplyColors();
    }

    private void ApplyColors()
    {
        _shadowColors = new Dictionary<ButtonColorType, Color>
        {
            { ButtonColorType.UI, ShadowUI },
            { ButtonColorType.Main, ShadowMain },
            { ButtonColorType.Green, ShadowGreen },
            { ButtonColorType.Red, ShadowRed },
            { ButtonColorType.Close, ShadowClose }
        };

        _buttonViewColors = new Dictionary<ButtonColorType, Color>
        {
            { ButtonColorType.Main, Main },
            { ButtonColorType.UI, UI },
            { ButtonColorType.Green, Green },
            { ButtonColorType.Red, Red },
            { ButtonColorType.Close, Close }
        };

        _buttonShaderColors = new Dictionary<ButtonColorType, Color>
        {
            { ButtonColorType.Main, ButtonMainShader },
            { ButtonColorType.UI, ButtonUIShader },
            { ButtonColorType.Green, White },
            { ButtonColorType.Red, White },
            { ButtonColorType.Close, White }
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
        if (h < 0.055f || h > 0.945f)
            return Palette.WrapHue(h + 0.11f);

        if (h >= 0.10f && h < 0.28f)
            return Palette.WrapHue(h - 0.07f);

        if (h >= 0.28f && h < 0.42f)
            return Palette.WrapHue(h - 0.10f);

        if (h >= 0.38f && h < 0.42f)
            return Palette.WrapHue(h - 0.20f);

        if (h >= 0.42f && h < 0.55f)
            return Palette.WrapHue(h + 0.08f);

        if (h >= 0.78 && h < 0.82f)
            return Palette.WrapHue(h + 0.10f);

        return Palette.WrapHue(h + 0.05f);
    }

    private Color GenerateAccent(Color baseColor, float h, float saturation)
    {
        float accentHue = ComputeAccentHue(h);
        float accentLightness = 0.55f;

        Color accent = Palette.FromHSL(accentHue, saturation, accentLightness);

        if (WCAG.Contrast(accent, baseColor) < 4.5f)
        {
            float l1 = accentLightness + 0.20f;
            float l2 = accentLightness - 0.20f;

            Color brighter = Palette.FromHSL(accentHue, saturation, Mathf.Clamp01(l1));
            Color darker = Palette.FromHSL(accentHue, saturation, Mathf.Clamp01(l2));

            accent = (WCAG.Contrast(brighter, baseColor) > WCAG.Contrast(darker, baseColor))
                ? brighter
                : darker;
        }

        return accent;
    }

}