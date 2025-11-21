using UnityEngine;

public static class Palette
{
    public static Color FromHSL(float h, float s, float l)
    {
        // Перевод HSL → RGB
        float c = (1 - Mathf.Abs(2 * l - 1)) * s;
        float x = c * (1 - Mathf.Abs((h * 6) % 2 - 1));
        float m = l - c / 2;

        float r = 0, g = 0, b = 0;

        if (h < 1f / 6f) { r = c; g = x; }
        else if (h < 2f / 6f) { r = x; g = c; }
        else if (h < 3f / 6f) { g = c; b = x; }
        else if (h < 4f / 6f) { g = x; b = c; }
        else if (h < 5f / 6f) { r = x; b = c; }
        else { r = c; b = x; }

        return new Color(r + m, g + m, b + m);
    }

    public static Color Shade(Color baseColor, float newLightness)
    {
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        // переводим HSV → HSL
        float l = v * (1 - s / 2f);
        float sl = (l == 0 || l == 1) ? 0 : (v - l) / Mathf.Min(l, 1 - l);

        return FromHSL(h, sl, newLightness);
    }

    public static float WrapHue(float h) => (h % 1f + 1f) % 1f;
}
