Shader "Custom/AquariumGlass"
{
    Properties
    {
        _WaterColor ("Water Color", Color) = (0.1, 0.4, 0.6, 0.3)
        _GlassColor ("Glass Tint", Color) = (0.9, 0.95, 1.0, 0.1)
        _RefractStrength ("Refraction Strength", Range(0.0, 0.5)) = 0.15
        _FresnelPower ("Fresnel Power", Range(1.0, 10.0)) = 3.0
        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.95
        _Thickness ("Glass Thickness", Range(0.0, 0.5)) = 0.05
        
        _WaveSpeed ("Wave Speed", Range(0.0, 2.0)) = 0.3
        _WaveAmount ("Wave Amount", Range(0.0, 0.1)) = 0.02
        _WaveFrequency ("Wave Frequency", Range(1.0, 10.0)) = 3.0
        
        _CausticTex ("Caustic Texture (Optional)", 2D) = "white" {}
        _CausticSpeed ("Caustic Speed", Range(0.0, 2.0)) = 0.5
        _CausticIntensity ("Caustic Intensity", Range(0.0, 2.0)) = 0.8
        
        _WaterLevel ("Water Level", Range(0.0, 1.0)) = 0.85
        _BubbleAmount ("Bubble Amount", Range(0.0, 1.0)) = 0.3
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
        // Pass 1: Interior del agua
        GrabPass { "_GrabTexture" }
        
        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert
        #pragma target 3.0

        sampler2D _GrabTexture;
        sampler2D _CausticTex;
        
        struct Input
        {
            float2 uv_CausticTex;
            float3 worldPos;
            float3 viewDir;
            float4 screenPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        half4 _WaterColor;
        half4 _GlassColor;
        half _RefractStrength;
        half _FresnelPower;
        half _Glossiness;
        half _Thickness;
        half _WaveSpeed;
        half _WaveAmount;
        half _WaveFrequency;
        half _CausticSpeed;
        half _CausticIntensity;
        half _WaterLevel;
        half _BubbleAmount;

        // Función de ruido
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
            
            for(int i = 0; i < 3; i++)
            {
                value += amplitude * noise(p);
                p *= 2.0;
                amplitude *= 0.5;
            }
            return value;
        }

        void vert(inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float time = _Time.y * _WaveSpeed;
            
            // Solo aplicar ondas en la superficie superior del agua
            float isTopFace = step(0.4, v.normal.y);
            
            // Ondas sutiles en la superficie
            float wave = sin(worldPos.x * _WaveFrequency + time * 2.0) * 
                         cos(worldPos.z * _WaveFrequency + time * 1.5);
            
            v.vertex.y += wave * _WaveAmount * isTopFace;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 worldPos = IN.worldPos;
            float time = _Time.y;
            
            // Determinar si estamos en la parte con agua o sin agua
            float3 localPos = mul(unity_WorldToObject, float4(worldPos, 1.0)).xyz;
            float waterMask = step(localPos.y, _WaterLevel * 2.0 - 1.0);
            
            // Efecto Fresnel para el vidrio
            float3 viewDir = normalize(IN.viewDir);
            float fresnel = pow(1.0 - saturate(dot(viewDir, IN.worldNormal)), _FresnelPower);
            
            // Distorsión de refracción
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float2 distortion = (fbm(worldPos.xz * 2.0 + time * 0.1) - 0.5) * _RefractStrength;
            screenUV += distortion * waterMask;
            
            // Cáusticas (patrones de luz en el agua)
            float2 causticUV1 = worldPos.xz * 0.5 + float2(time * _CausticSpeed, 0);
            float2 causticUV2 = worldPos.xz * 0.7 + float2(0, time * _CausticSpeed * 0.8);
            
            float caustic1 = tex2D(_CausticTex, causticUV1).r;
            float caustic2 = tex2D(_CausticTex, causticUV2).r;
            float caustics = (caustic1 + caustic2) * 0.5 * _CausticIntensity * waterMask;
            
            // Burbujas ocasionales
            float bubbleNoise = fbm(worldPos.xz * 5.0 + float2(0, time * 2.0));
            float bubbles = step(1.0 - _BubbleAmount * 0.1, bubbleNoise) * waterMask * 0.3;
            
            // Color base
            float3 glassColor = _GlassColor.rgb;
            float3 waterTint = _WaterColor.rgb;
            
            // Mezclar vidrio y agua según la máscara
            float3 baseColor = lerp(glassColor, waterTint, waterMask * 0.7);
            
            // Añadir cáusticas y burbujas
            baseColor += caustics * float3(0.8, 0.9, 1.0);
            baseColor += bubbles;
            
            // Aplicar Fresnel (reflejos en los bordes)
            baseColor = lerp(baseColor, float3(1, 1, 1), fresnel * 0.3);
            
            o.Albedo = baseColor;
            o.Metallic = 0.0;
            o.Smoothness = _Glossiness;
            
            // Transparencia: más opaco con agua, más transparente arriba
            float alpha = lerp(0.05, _WaterColor.a, waterMask);
            alpha += fresnel * 0.2;
            o.Alpha = alpha;
            
            // Normal map sutil para distorsión
            float3 normalNoise = float3(
                fbm(worldPos.xz * 3.0 + time * 0.1) * 2.0 - 1.0,
                1.0,
                fbm(worldPos.xz * 3.0 + float2(5.2, 1.3) + time * 0.1) * 2.0 - 1.0
            );
            o.Normal = normalize(normalNoise) * waterMask;
        }
        ENDCG
    }
    
    FallBack "Transparent/Diffuse"
}