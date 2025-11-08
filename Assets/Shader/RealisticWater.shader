Shader "Custom/RealisticWater"
{
    Properties
    {
        _WaterColor ("Water Color (Shallow)", Color) = (0.1, 0.5, 0.7, 0.8)
        _DeepWaterColor ("Deep Water Color", Color) = (0.0, 0.2, 0.4, 1.0)
        _FresnelColor ("Fresnel Color", Color) = (0.7, 0.9, 1.0, 1.0)
        
        _WaveSpeed ("Wave Speed", Range(0.1, 5.0)) = 1.0
        _WaveHeight ("Wave Height", Range(0.0, 2.0)) = 0.3
        _WaveFrequency ("Wave Frequency", Range(0.5, 10.0)) = 2.0
        
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.9
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        
        _RefractionStrength ("Refraction Strength", Range(0.0, 0.5)) = 0.1
        _FresnelPower ("Fresnel Power", Range(1.0, 10.0)) = 5.0
        _Transparency ("Transparency", Range(0.0, 1.0)) = 0.7
        
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamAmount ("Foam Amount", Range(0.0, 1.0)) = 0.3
        _FoamSpeed ("Foam Speed", Range(0.1, 3.0)) = 1.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 viewDir;
            float3 worldNormal;
            INTERNAL_DATA
        };

        half4 _WaterColor;
        half4 _DeepWaterColor;
        half4 _FresnelColor;
        half4 _FoamColor;
        
        half _WaveSpeed;
        half _WaveHeight;
        half _WaveFrequency;
        half _Glossiness;
        half _Metallic;
        half _RefractionStrength;
        half _FresnelPower;
        half _Transparency;
        half _FoamAmount;
        half _FoamSpeed;

        // Función de ruido mejorada
        float hash(float2 p)
        {
            p = frac(p * float2(123.34, 456.21));
            p += dot(p, p + 45.32);
            return frac(p.x * p.y);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);
            
            float a = hash(i);
            float b = hash(i + float2(1.0, 0.0));
            float c = hash(i + float2(0.0, 1.0));
            float d = hash(i + float2(1.0, 1.0));
            
            return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
        }

        float fbm(float2 p)
        {
            float value = 0.0;
            float amplitude = 0.5;
            
            for(int i = 0; i < 4; i++)
            {
                value += amplitude * noise(p);
                p *= 2.0;
                amplitude *= 0.5;
            }
            return value;
        }

        // Función de ondas Gerstner
        float3 GerstnerWave(float2 pos, float time, float wavelength, float2 direction)
        {
            float k = 2.0 * 3.14159 / wavelength;
            float c = sqrt(9.8 / k);
            float2 d = normalize(direction);
            float f = k * (dot(d, pos) - c * time);
            float a = 0.5 / k;
            
            return float3(
                d.x * a * cos(f),
                a * sin(f),
                d.y * a * cos(f)
            );
        }

        void vert(inout appdata_full v)
        {
            float time = _Time.y * _WaveSpeed;
            float2 worldPos = mul(unity_ObjectToWorld, v.vertex).xz;
            
            // Múltiples ondas Gerstner para movimiento realista
            float3 wave1 = GerstnerWave(worldPos, time, 4.0, float2(1, 0));
            float3 wave2 = GerstnerWave(worldPos, time * 0.8, 6.0, float2(0, 1));
            float3 wave3 = GerstnerWave(worldPos, time * 1.2, 3.0, float2(0.7, 0.7));
            
            float3 displacement = (wave1 + wave2 + wave3) * _WaveHeight;
            v.vertex.xyz += displacement;
            
            // Recalcular normales aproximadas
            float3 tangent = float3(1, wave1.y + wave2.y + wave3.y, 0);
            float3 binormal = float3(0, wave1.y + wave2.y + wave3.y, 1);
            v.normal = normalize(cross(binormal, tangent));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float time = _Time.y;
            float2 worldUV = IN.worldPos.xz * _WaveFrequency;
            
            // Generar normales con ruido
            float2 offset1 = float2(time * 0.05, time * 0.03) * _WaveSpeed;
            float2 offset2 = float2(-time * 0.03, time * 0.04) * _WaveSpeed;
            
            float noise1 = fbm(worldUV * 0.5 + offset1);
            float noise2 = fbm(worldUV * 0.7 + offset2);
            
            // Normal map procedural
            float3 normal1 = float3(
                noise1 * 2.0 - 1.0,
                1.0,
                noise2 * 2.0 - 1.0
            );
            
            o.Normal = normalize(normal1);
            
            // Efecto Fresnel
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);
            
            // Profundidad simulada
            float depth = saturate(noise1 * 0.5 + 0.5);
            float3 waterColor = lerp(_WaterColor.rgb, _DeepWaterColor.rgb, depth);
            
            // Mezclar con color Fresnel en los bordes
            waterColor = lerp(waterColor, _FresnelColor.rgb, fresnel * 0.5);
            
            // Espuma en las crestas de las olas
            float foamNoise = fbm(worldUV * 3.0 + float2(time * _FoamSpeed, 0));
            float foam = step(1.0 - _FoamAmount, foamNoise) * step(0.3, IN.worldPos.y);
            waterColor = lerp(waterColor, _FoamColor.rgb, foam);
            
            o.Albedo = waterColor;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            // Transparencia con Fresnel
            o.Alpha = lerp(_Transparency, 1.0, fresnel * 0.3);
        }
        ENDCG
    }
    
    FallBack "Transparent/Diffuse"
}