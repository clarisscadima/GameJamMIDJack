Shader "Custom/GlassWindow"
{
    Properties
    {
        _GlassColor("Glass Color", Color) = (1,1,1,0.2)
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        _Gloss("Glossiness", Range(0,1)) = 0.8
        _Refraction("Refraction Strength", Range(0,0.1)) = 0.02
        _FresnelPower("Fresnel Power", Range(0.1,5)) = 2.0
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Cutoff("Alpha Cutoff (not used)", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 200
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass { "_GrabTexture" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _GrabTexture;
        sampler2D _NormalMap;

        struct Input
        {
            float2 uv_NormalMap;
            float3 viewDir;
            float4 screenPos;
        };

        fixed4 _GlassColor;
        fixed4 _SpecularColor;
        half _Gloss;
        half _Refraction;
        half _FresnelPower;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 nmap = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            float3 worldNormal = normalize(nmap);
            float ndv = saturate(dot(normalize(IN.viewDir), worldNormal));
            float fresnel = pow(1 - ndv, _FresnelPower);
            float2 grabUV = IN.screenPos.xy / IN.screenPos.w;
            float2 offset = worldNormal.xy * _Refraction;
            fixed4 grab = tex2D(_GrabTexture, grabUV + offset);
            fixed3 baseColor = lerp(grab.rgb, _GlassColor.rgb, _GlassColor.a);
            o.Albedo = baseColor;
            o.Alpha = _GlassColor.a;
            o.Smoothness = _Gloss;
            o.Metallic = 0.0;
            o.Emission = _SpecularColor.rgb * fresnel * 0.5;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}

