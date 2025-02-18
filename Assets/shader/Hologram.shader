Shader "Custom/HologramShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}                      
        _TintColor ("Tint Color", Color) = (1,1,0,1)              
        _Transparency("Transparency", Range(0,1)) = 0.5             
        _NoiseTex ("Noise Texture", 2D) = "white" {}                
        _FresnelPower("Fresnel Power", Range(1,5)) = 2             
        _FresnelColor("Fresnel Color", Color) = (1,1,0,1)           
        _ScanSpeed ("Scan Speed", Range(0, 10)) = 1                
        _ScanIntensity ("Scan Intensity", Range(0, 5)) = 0.5        
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
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
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Campiona la texture di base e applica il tint
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

                // Calcolo migliorato del Fresnel
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fresnel = pow(1.0 - abs(dot(i.worldNormal, viewDir)), _FresnelPower);
                col.rgb += fresnel * _FresnelColor.rgb;

                // Miglioramento dell'effetto rumore
                float noise = tex2D(_NoiseTex, i.uv * 5.0 + _Time.y * 0.1).r;
                col.rgb += noise * 0.15;

                // Effetto scanline migliorato
                float scanline = sin((i.worldPos.y * 10.0 + _Time.y * _ScanSpeed) * 3.14159) * _ScanIntensity;
                col.rgb += scanline;

                // Imposta la trasparenza
                col.a = _Transparency;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
