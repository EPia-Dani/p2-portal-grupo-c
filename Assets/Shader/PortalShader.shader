Shader "Hidden/PortalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex("Mask texture", 2D) = "white" {}
        _FrontTex("Front texture", 2D) = "white" {}   // <-- NUEVA TEXTURA
        _Cutout("Cutout", Range(0.0, 1.0)) = 0.5 
    }

    SubShader
    { 
        Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
        Lighting Off
        Cull Back
        ZWrite On
        ZTest Less

        Fog{ Mode Off }

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos: TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _FrontTex;      // <-- sampler nueva textura

            float _Cutout;

            fixed4 frag (v2f i) : SV_Target
            {
                i.screenPos /= i.screenPos.w;

                fixed4 mask = tex2D(_MaskTex, i.uv);
                if (mask.a < _Cutout)
                    clip(-1);

                fixed4 mainColor = tex2D(_MainTex, float2(i.screenPos.x, i.screenPos.y));
                fixed4 frontColor = tex2D(_FrontTex, i.uv);   // <-- uv del objeto (puedes cambiar)

        
                fixed4 finalColor = lerp(mainColor, frontColor, frontColor.a);

                return finalColor;
            }
            ENDCG
        }
    }
}

