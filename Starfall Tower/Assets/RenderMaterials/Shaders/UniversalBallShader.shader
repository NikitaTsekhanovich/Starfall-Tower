Shader "Custom/UniversalBallShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Color1 ("Color 1", Color) = (1, 0, 0, 1)
        _Color2 ("Color 2", Color) = (0, 0, 1, 1)
        _TimeSpeed ("Time Speed", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _NewColor;
        fixed4 _Color1;
        fixed4 _Color2;
        float _TimeSpeed;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float lerpValue = (sin(_Time.y * _TimeSpeed) + 1.0) * 0.5;  // Нормализуем значение от 0 до 1
            fixed4 color = lerp(_Color1, _Color2, lerpValue); // Интерполяция между двумя цветами

            o.Albedo = color.rgb;  // Устанавливаем цвет
            o.Alpha = color.a; 
            // _NewColor = _Color;
            //
            // _NewColor.r = sin(1 * _Time.y);
            // _NewColor.g = sin(2 * _Time.y);
            // _NewColor.b = sin(3 * _Time.y);
            //
            // fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _NewColor;
            // o.Albedo = c.rgb;
            // // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            // o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
