Shader "Custom/OrganicHoloInterference"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1, 0.5, 0, 1)
        _Transparency ("Transparency", Range(0, 1)) = 0.7
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _FresnelPower ("Fresnel Power", Range(1, 5)) = 2
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 0, 1)
        _ParallaxStrength ("Parallax Strength", Range(0, 1)) = 0.1
        _ScanSpeed ("Scan Speed", Range(0, 10)) = 1
        _ScanIntensity ("Scan Intensity", Range(0, 5)) = 0.5
        _NoiseScale ("Noise Scale", Range(0, 5)) = 1.0
        _DepthEffect ("Depth Effect", Range(0, 10)) = 2.0
        _Softness ("Softness", Range(0, 1)) = 0.5
        _OrganicNoise ("Organic Noise", Range(0, 1)) = 0.3
        _EmissionColor ("Emission Color", Color) = (0, 0, 1, 1)
        _EmissionStrength ("Emission Strength", Range(0, 5)) = 1.0
        _WobbleStrength ("Wobble Strength", Range(0, 1)) = 0.2
        _WobbleSpeed ("Wobble Speed", Range(0, 5)) = 1.0
        _InterferenceStrength ("Interference Strength", Range(0, 1)) = 0.5
        _InterferenceSpeed ("Interference Speed", Range(0, 5)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 300

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _TintColor;
            float _Transparency;
            float _FresnelPower;
            float4 _FresnelColor;
            float _ScanSpeed;
            float _ScanIntensity;
            float _ParallaxStrength;
            float _NoiseScale;
            float _DepthEffect;
            float _Softness;
            float _OrganicNoise;
            float4 _EmissionColor;
            float _EmissionStrength;
            float _WobbleStrength;
            float _WobbleSpeed;
            float _InterferenceStrength;
            float _InterferenceSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                // Apply parallax offset for depth effect
                float3 parallaxOffset = normalize(o.worldNormal) * _ParallaxStrength;
                o.pos.xyz += parallaxOffset;

                // Wobble effect with smooth oscillations
                float wobbleX = sin(_Time.y * _WobbleSpeed + o.worldPos.x * 0.5) * _WobbleStrength;
                float wobbleY = cos(_Time.y * _WobbleSpeed + o.worldPos.y * 0.5) * _WobbleStrength;
                o.uv.x += wobbleX;
                o.uv.y += wobbleY;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture and apply tint
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

                // Soft fresnel effect with smoother transition
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fresnel = pow(1.0 - abs(dot(i.worldNormal, viewDir)), _FresnelPower);
                fresnel = smoothstep(0.1, 1.0, fresnel);  // Make fresnel transition softer
                col.rgb += fresnel * _FresnelColor.rgb;

                // Organic noise effect with Perlin-like characteristics
                float organicNoise = tex2D(_NoiseTex, i.uv * _NoiseScale + _Time.y * 0.05).r;
                organicNoise = smoothstep(0.0, 1.0, organicNoise * _OrganicNoise);  // Smooth noise
                col.rgb += organicNoise * 0.2;

                // Apply interference effect (dynamic distortion)
                float interference = sin((_Time.y * _InterferenceSpeed + i.worldPos.x * 0.3)) * _InterferenceStrength;
                col.rgb += interference;  // Apply the interference effect as distortion

                // Apply softer scanline effect with dynamic depth
                float scanline = sin((i.worldPos.y * 15.0 + _Time.y * _ScanSpeed) * 3.14159) * _ScanIntensity;
                float depthMod = pow(1.0 - length(i.worldPos - _WorldSpaceCameraPos) * _DepthEffect, 2.0);
                scanline *= smoothstep(0.0, 1.0, depthMod);  // Soften scanline effect
                col.rgb += scanline;

                // Apply general softness
                col.rgb = lerp(col.rgb, col.rgb * (1.0 - _Softness), 0.2); // Introduce more blending for soft edges

                // Add emission effect
                col.rgb += _EmissionColor.rgb * _EmissionStrength; // Apply emission with strength

                // Set transparency (alpha)
                col.a = _Transparency;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
