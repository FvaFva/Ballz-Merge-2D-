using UnityEngine;

public static class WCAG
{
    public static float Luminance(Color c)
    {
        float Linear(float v)
        {
            return (v <= 0.03928f) ? (v / 12.92f) : Mathf.Pow((v + 0.055f) / 1.055f, 2.4f);
        }

        float r = Linear(c.r);
        float g = Linear(c.g);
        float b = Linear(c.b);

        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    public static float Contrast(Color a, Color b)
    {
        float L1 = Luminance(a);
        float L2 = Luminance(b);
        return (Mathf.Max(L1, L2) + 0.05f) / (Mathf.Min(L1, L2) + 0.05f);
    }

    public static Color EnsureContrast(Color baseColor, bool darker, float target = 4.5f)
    {
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        // ищем значение V, обеспечивающее контраст
        for (int i = 0; i < 50; i++)
        {
            v += darker ? -0.02f : 0.02f;
            v = Mathf.Clamp01(v);

            Color c = Color.HSVToRGB(h, s, v);
            if (Contrast(baseColor, c) >= target)
                return c;
        }

        return Color.HSVToRGB(h, s, v);
    }
}
