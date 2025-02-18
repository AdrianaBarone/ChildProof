Shader "Custom/GlowingWalls"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.0, 1.0, 1.0, 1.0)
        _EmissionColor ("Emission Color", Color) = (0.0, 1.0, 1.0, 1.0)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2.0
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1.0
        _NoiseSpeed ("Noise Speed", Range(0, 5)) = 1.0
        _FogIntensity ("Fog Intensity", Range(0, 1)) = 0.5
        _DetailIntensity ("Detail Intensity", Range(0, 5)) = 1.0
        _ParallaxDepth ("Parallax Depth", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float _GlowIntensity;
            float4 _BaseColor;
            float4 _EmissionColor;
            float _NoiseScale;
            float _NoiseSpeed;
            float _FogIntensity;
            float _DetailIntensity;
            float _ParallaxDepth;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = v.uv * _NoiseScale;
                o.viewDir = normalize(TransformWorldToViewDir(v.vertex.xyz));
                return o;
            }
            
            float random(float2 p) {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            float noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }
            
            float2 parallaxOffset(float2 uv, float3 viewDir)
            {
                float height = noise(uv);
                return uv + (viewDir.xy * height * _ParallaxDepth);
            }
            
            half4 frag (v2f i) : SV_Target
            {
                float2 parallaxUV = parallaxOffset(i.uv, i.viewDir);
                float glowEffect = noise(parallaxUV + _Time.y * _NoiseSpeed) * _DetailIntensity;
                glowEffect = saturate(glowEffect * _GlowIntensity);
                float4 finalColor = _BaseColor + (_EmissionColor * glowEffect);
                
                float fog = exp(-i.pos.z * _FogIntensity);
                finalColor.rgb = lerp(finalColor.rgb, float3(1.0, 1.0, 1.0), 1.0 - fog);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}
