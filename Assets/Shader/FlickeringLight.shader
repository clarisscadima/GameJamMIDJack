Shader "Custom/FlickeringLight"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,0.8,1)
        _EmissionColor("Emission Color", Color) = (1,1,0.8,1)
        _EmissionIntensity("Emission Intensity", Range(0,10)) = 3
        _FlickerSpeed("Flicker Speed", Range(0,20)) = 5
        _Gloss("Glossiness", Range(0,1)) = 0.8
        _Metallic("Metallic", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input { float2 uv_MainTex; };

        fixed4 _BaseColor;
        fixed4 _EmissionColor;
        half _EmissionIntensity;
        half _FlickerSpeed;
        half _Gloss;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _BaseColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Gloss;
            float flicker = abs(sin(_Time.y * _FlickerSpeed)) * _EmissionIntensity;
            o.Emission = _EmissionColor.rgb * flicker;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
