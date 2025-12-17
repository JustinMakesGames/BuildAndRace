Shader "Custom/Test" {
    Properties {
        _Color ("Tint", Color) = (0 ,0 ,0 ,1)
        _SecondColor ("Second Color", Color) = (0, 0, 0, 0)
        _Brightness ("Brightness", Float) = 5.0
        _Scale ("Scale", Range(1, 100)) = 1
    }

    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off

        Pass {

            //Notes:
            // positionOS = posObjSpace van model.
            //in de vert2Frag stage maak je posClipSpace aan.
            //in de vertex shader convert je posClipSpace naar posHomogenousSpace

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _SecondColor;
                float _Brightness;
                float _Scale;
            CBUFFER_END
            
            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 position : SV_POSITION;       
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o = (v2f)0;
                o.position = TransformObjectToHClip(v.vertex.xyz);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET {
                float3 scaledWldPos = floor(i.worldPos / _Scale);
                float checkerboard = floor(scaledWldPos.x) + floor(scaledWldPos.z);
                checkerboard = frac(checkerboard * 0.5);
                checkerboard *= 2;
                float4 color = lerp(_Color, _SecondColor, checkerboard); 
                return color * _Brightness;
            }
            
            ENDHLSL
        }
    }
   
}
