Shader "Unlit/SimpleOutlineSh"
{
    Properties
    {
        [HDR] _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Range(0, 10)) = 0.1
        _ZTest("ZTest", Int) = 0
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent+110"
            "RenderType" = "Transparent"
            "DisableBatching" = "True"
        }

        Pass 
        {
            Name "Fill"
            Cull Off
            ZTest [_ZTest]
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            Stencil 
            {
                Ref 1
                Comp NotEqual
            }

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
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata v)
            {
                v2f o;
                
                float3 viewPos = UnityObjectToViewPos(v.vertex);
                float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                
                float3 scaledOffset = viewNormal * _OutlineWidth * 0.001 * -viewPos.z;
                
                o.pos = UnityViewToClipPos(viewPos + scaledOffset);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
