Shader "Custom/CandleFlame"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlameColor1 ("Flame Color 1 (Core)", Color) = (1, 1, 0.8, 1)
        _FlameColor2 ("Flame Color 2 (Mid)", Color) = (1, 0.5, 0.1, 1)
        _FlameColor3 ("Flame Color 3 (Outer)", Color) = (1, 0.1, 0.0, 1)
        _FlickerSpeed ("Flicker Speed", Range(0.5, 5.0)) = 2.0
        _FlickerAmount ("Flicker Amount", Range(0.0, 0.5)) = 0.1
        _Brightness ("Brightness", Range(1.0, 5.0)) = 2.0
        _GlowIntensity ("Glow Intensity", Range(0.0, 2.0)) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FlameColor1;
            float4 _FlameColor2;
            float4 _FlameColor3;
            float _FlickerSpeed;
            float _FlickerAmount;
            float _Brightness;
            float _GlowIntensity;

            // Función de ruido simplificada
            float noise(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float smoothNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                
                float a = noise(i);
                float b = noise(i + float2(1.0, 0.0));
                float c = noise(i + float2(0.0, 1.0));
                float d = noise(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for(int i = 0; i < 4; i++)
                {
                    value += amplitude * smoothNoise(p * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            v2f vert (appdata v)
            {
                v2f o;
                
                // Movimiento ondulante de la llama
                float time = _Time.y * _FlickerSpeed;
                float flicker = sin(time * 3.0 + v.vertex.y * 2.0) * _FlickerAmount;
                float wave = sin(time * 2.0 + v.vertex.y * 3.0) * _FlickerAmount * 0.6;
                
                v.vertex.x += (flicker + wave) * v.vertex.y;
                v.vertex.z += wave * 0.3 * v.vertex.y;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y * _FlickerSpeed;
                
                // Crear forma de llama
                float2 centered = uv - float2(0.5, 0.0);
                float dist = length(centered);
                
                // Forma vertical de llama (más ancha abajo, puntiaguda arriba)
                float flameShape = smoothstep(0.5, 0.0, dist) * smoothstep(0.0, 0.8, uv.y);
                flameShape *= smoothstep(1.0, 0.6, uv.y); // Puntiaguda arriba
                
                // Añadir turbulencia con ruido
                float2 noiseCoord = float2(uv.x * 3.0, uv.y * 4.0 - time * 0.5);
                float turbulence = fbm(noiseCoord);
                
                // Parpadeo sutil
                float flicker = sin(time * 4.0) * 0.1 + 0.9;
                float flicker2 = sin(time * 6.0 + 1.5) * 0.05 + 0.95;
                
                // Aplicar turbulencia a la forma
                flameShape *= (0.7 + turbulence * 0.3) * flicker * flicker2;
                
                // Gradiente de colores (núcleo brillante -> medio -> exterior)
                float gradient = smoothstep(0.0, 0.3, uv.y) * smoothstep(1.0, 0.7, uv.y);
                
                // Núcleo muy brillante (blanco-amarillo)
                float core = smoothstep(0.3, 0.0, dist) * smoothstep(0.0, 0.4, uv.y) * smoothstep(0.8, 0.3, uv.y);
                
                // Mezclar colores según la distancia del centro
                float3 color = _FlameColor1.rgb * core * 1.5;
                color += _FlameColor2.rgb * gradient * (1.0 - core);
                color += _FlameColor3.rgb * flameShape * (1.0 - gradient - core);
                
                // Añadir brillo intenso
                color *= _Brightness;
                color += _FlameColor1.rgb * core * _GlowIntensity;
                
                // Alpha basado en la forma de la llama
                float alpha = flameShape * smoothstep(0.0, 0.2, uv.y);
                
                return fixed4(color, alpha);
            }
            ENDCG
        }
        
        // Pass adicional para el brillo (glow)
        Pass
        {
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _FlameColor1;
            float4 _FlameColor2;
            float _FlickerSpeed;
            float _FlickerAmount;
            float _GlowIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                
                // Mismo movimiento que el pass anterior
                float time = _Time.y * _FlickerSpeed;
                float flicker = sin(time * 3.0 + v.vertex.y * 2.0) * _FlickerAmount;
                float wave = sin(time * 2.0 + v.vertex.y * 3.0) * _FlickerAmount * 0.6;
                
                v.vertex.x += (flicker + wave) * v.vertex.y;
                v.vertex.z += wave * 0.3 * v.vertex.y;
                
                // Expandir ligeramente para el glow
                v.vertex.xyz *= 1.2;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 centered = uv - float2(0.5, 0.0);
                float dist = length(centered);
                
                // Glow suave
                float glow = smoothstep(0.6, 0.0, dist) * smoothstep(0.0, 0.5, uv.y) * smoothstep(1.0, 0.5, uv.y);
                
                float3 glowColor = lerp(_FlameColor2.rgb, _FlameColor1.rgb, glow);
                
                return fixed4(glowColor * glow * _GlowIntensity * 0.5, 0.0);
            }
            ENDCG
        }
    }
}