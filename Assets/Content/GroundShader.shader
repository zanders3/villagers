Shader "Custom/GroundShader" {
	Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;

            struct vertexInput {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(vertexInput i){
                v2f o;
                o.position = mul(UNITY_MATRIX_MVP, i.vertex);
                o.uv = i.vertex.xz;
                return o;
            }
            float4 frag(v2f i) : COLOR {
                return tex2D(_MainTex, i.uv * 0.25);
            }
            ENDCG
        }
    }
}