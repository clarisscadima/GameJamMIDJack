Shader "Custom/LightBulbEffect"
{
    Properties
    {
        _Color("Light Color", Color) = (1,1,0.8,1)
        _Intensity("Light Intensity", Range(0,10)) = 3
        _Range("Light Range", Range(0.1,5)) = 2
        _Gloss("Glossiness", Range(0,1)) = 0.8
        _Metallic("Metallic", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            fixed4 _Color;
            half _Intensity;
            half _Range;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 normal = normalize(i.worldNormal);
                float ndv = saturate(dot(viewDir, normal));
                float glow = pow(ndv, _Range) * _Intensity;
                return fixed4(_Color.rgb * glow, 1.0);
            }
            ENDCG
        }
    }
}
