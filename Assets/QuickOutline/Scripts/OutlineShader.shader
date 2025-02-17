Shader "Custom/Outline"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,0,1) // Giallo di default
        _Width ("Width", Range(0.001, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }
            
            Cull Front // Rende visibile il contorno all'esterno
            ZWrite On
            ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            float _Width;
            float4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                v.vertex.xyz += norm * _Width;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color; // Colore del contorno
            }
            ENDCG
        }
    }
}
