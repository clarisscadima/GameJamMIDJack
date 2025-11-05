Shader "Custom/InfernalDemonFire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FireTex ("Fire Noise", 2D) = "white" {}
        _DistortionTex ("Distortion", 2D) = "white" {}
        
        _FireColor1 ("Fire Core", Color) = (1, 0.3, 0, 1)
        _FireColor2 ("Fire Mid", Color) = (1, 0.6, 0, 1)
        _FireColor3 ("Fire Edge", Color) = (1, 1, 0.3, 1)
        _BurnColor ("Burn Glow", Color) = (2, 0.5, 0.1, 1)
        
        _FireSpeed ("Fire Speed", Range(0, 5)) = 1.5
        _FireScale ("Fire Scale", Range(0.1, 10)) = 2.0
        _DistortionStrength ("Distortion", Range(0, 1)) = 0.3
        _EmissionStrength ("Emission Power", Range(1, 20)) = 8.0
        
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3.0
        _FresnelIntensity ("Fresnel Intensity", Range(0, 5)) = 2.0
        
        _VertexDisplacement ("Vertex Wave", Range(0, 0.5)) = 0.08
        _DisplacementSpeed ("Wave Speed", Range(0, 5)) = 2.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.5
        
        sampler2D _MainTex;
        sampler2D _FireTex;
        sampler2D _DistortionTex;
        
        fixed4 _FireColor1;
        fixed4 _FireColor2;
        fixed4 _FireColor3;
        fixed4 _BurnColor;
        
        float _FireSpeed;
        float _FireScale;
        float _DistortionStrength;
        float _EmissionStrength;
        float _FresnelPower;
        float _FresnelIntensity;
        float _VertexDisplacement;
        float _DisplacementSpeed;
        
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 viewDir;
            float3 worldNormal;
        };
        
        // Ruido mejorado para movimiento orgánico
        float noise(float3 p)
        {
            return frac(sin(dot(p, float3(12.9898, 78.233, 45.543))) * 43758.5453);
        }
        
        float fbm(float3 p)
        {
            float f = 0.0;
            float w = 0.5;
            for (int i = 0; i < 5; i++)
            {
                f += w * noise(p);
                p *= 2.0;
                w *= 0.5;
            }
            return f;
        }
        
        void vert(inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float time = _Time.y * _DisplacementSpeed;
            
            // Múltiples capas de desplazamiento para movimiento de llamas
            float displacement = fbm(worldPos * 3.0 + float3(0, time * 1.5, 0)) * 0.5;
            displacement += fbm(worldPos * 5.0 - float3(time * 0.8, time * 2.0, 0)) * 0.3;
            displacement += sin(worldPos.y * 8.0 + time * 3.0) * 0.2;
            
            v.vertex.xyz += v.normal * displacement * _VertexDisplacement;
        }
        
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float time = _Time.y * _FireSpeed;
            
            // Coordenadas para el fuego con múltiples capas
            float2 uv1 = IN.worldPos.xy * _FireScale + float2(0, time * 1.2);
            float2 uv2 = IN.worldPos.yz * _FireScale * 0.7 + float2(time * 0.8, time * 1.5);
            float2 uv3 = IN.worldPos.xz * _FireScale * 1.3 + float2(time * -0.6, time * 0.9);
            
            // Distorsión para movimiento orgánico
            float2 distortion = tex2D(_DistortionTex, IN.worldPos.xy * 2.0 + time * 0.3).rg;
            distortion = (distortion - 0.5) * _DistortionStrength;
            
            // Tres capas de fuego para profundidad
            float fire1 = tex2D(_FireTex, uv1 + distortion).r;
            float fire2 = tex2D(_FireTex, uv2 + distortion * 0.7).r;
            float fire3 = tex2D(_FireTex, uv3 + distortion * 1.3).r;
            
            // Combinar capas con pesos diferentes
            float fireMask = fire1 * 0.5 + fire2 * 0.3 + fire3 * 0.2;
            fireMask = saturate(fireMask * 1.5);
            
            // Animación vertical del fuego (sube desde abajo)
            float verticalGradient = saturate((IN.worldPos.y - _WorldSpaceCameraPos.y) * 0.5 + 0.5);
            fireMask *= pow(verticalGradient, 0.7);
            
            // Gradiente de color de fuego multi-color
            fixed4 fireColor;
            if (fireMask < 0.3)
                fireColor = lerp(_FireColor1, _FireColor2, fireMask / 0.3);
            else if (fireMask < 0.7)
                fireColor = lerp(_FireColor2, _FireColor3, (fireMask - 0.3) / 0.4);
            else
                fireColor = lerp(_FireColor3, _BurnColor, (fireMask - 0.7) / 0.3);
            
            // Fresnel para bordes brillantes
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal)), _FresnelPower);
            fresnel *= _FresnelIntensity;
            
            // Chispas procedurales
            float sparks = fbm(IN.worldPos * 20.0 + float3(0, time * 5.0, 0));
            sparks = pow(saturate(sparks), 8.0) * 3.0;
            
            // Color base con textura
            fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
            
            // Output final
            o.Albedo = mainColor.rgb * 0.1; // Oscurecer el modelo base
            o.Metallic = 0.2;
            o.Smoothness = 0.3;
            
            // Emisión EXTREMA con todas las capas combinadas
            o.Emission = fireColor.rgb * fireMask * _EmissionStrength;
            o.Emission += _BurnColor.rgb * fresnel * 2.0;
            o.Emission += float3(1, 0.8, 0.3) * sparks;
            o.Emission += fireColor.rgb * pow(fireMask, 2.0) * 5.0; // Hotspots extra brillantes
            
            o.Alpha = 1.0;
        }
        ENDCG
    }
    
    FallBack "Diffuse"
}