Shader "Custom/UIBlendTwoTextures_NoDoubling"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {} // Основная текстура
        _Texture2 ("Second Texture", 2D) = "white" {} // Вторая текстура
        _BlendAmount ("Blend Amount", Range(0, 1)) = 0.5 // Смешивание
        _Color ("Color Tint", Color) = (1, 1, 1, 1) // Цветовой фильтр
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "Canvas"="UI" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Входные данные
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Шейдерные параметры
            sampler2D _MainTex;
            sampler2D _Texture2;
            float _BlendAmount;
            fixed4 _Color;

            // Вершинный шейдер
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // Перевод координат объекта в экранные
                o.uv = v.uv; // UV-координаты
                return o;
            }

            // Фрагментный шейдер
            fixed4 frag (v2f i) : SV_Target
            {
                // Текстуры
                fixed4 tex1 = tex2D(_MainTex, i.uv);
                fixed4 tex2 = tex2D(_Texture2, i.uv);

                // Смешивание текстур
                fixed4 blended = lerp(tex1, tex2, _BlendAmount);

                // Применение цвета
                blended *= _Color;

                // Учет прозрачности
                blended.a = tex1.a * (1.0 - _BlendAmount) + tex2.a * _BlendAmount;

                return blended;
            }
            ENDCG
        }
    }
}