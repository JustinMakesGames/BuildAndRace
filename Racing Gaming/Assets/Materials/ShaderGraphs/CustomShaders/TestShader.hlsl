#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//Notes:
// positionOS = posObjSpace van model.
//in de vert2Frag stage maak je posClipSpace aan.
//in de vertex shader convert je posClipSpace naar posHomogenousSpace
CBUFFER_START(UnityPerMaterial)
    float4 _Color;
    float4 _SecondColor;
    float _Brightness;
    float _Scale;
    float _ColorMap_ST;
    float _Test;
CBUFFER_END

TEXTURE2D(_ColorMap);
SAMPLER(sampler_ColorMap);

uint index = 0;

//Macro's
#define APPLY_BRIGHTNESS(color, brightness, alphaValue) (color *= float4(color.rgb * brightness, alphaValue));
//Functions
float2 tileUv(float2 uv, float scale)
{
    uv *= scale;
    uv = uv - floor(uv); //2.5 - floor(2.5) = 0.5   
    return uv;
}

float drawBox(float2 st, float2 size, float smoothness)
{   
    //st allows for squares to be rounded all the way around
    //calculating the size for squares
    size = float2(0.5, 0.5) - size * 0.5;
    float2 aa = float2(smoothness*0.5, smoothness*0.5);
    //creating a smooth uv tile by applying smoothstep on both axis.
    float2 uv = smoothstep(size, size+aa, st);
    uv *= smoothstep(size, size+aa, float2(1.0, 1.0) - st);
    return uv.x * uv.y;
}

float applyScrollAnim(float2 uv, float2 direction, float frameSpeed)
{
    return uv.y += _Time.y * direction * frameSpeed;
}

struct Attributes {
    float4 vertexOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Interpolators {
    float4 positionCS : SV_POSITION;       
    float2 uv : TEXCOORD0;
};

Interpolators vert(Attributes input) {
    Interpolators output = (Interpolators)0;
    output.positionCS = TransformObjectToHClip(input.vertexOS.xyz);
    output.uv = input.uv;
    return output;
}

float4 frag(Interpolators input) : SV_TARGET {
    //generating glowing pixels and scrolling animation.
    float2 uv = input.uv;
    //uv.y = applyScrollAnim(uv, float2(1, 0), 0.1);
    uv = tileUv(uv, _Scale);
    float gridColor = drawBox(uv, float2(0.8, 0.8), 0.5) * _Brightness;
    float3 color = lerp(_Color, _SecondColor, gridColor);
    //sampling the grayscale texture.
    half4 colorMap = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, input.uv);
    return float4(color, 1);
}


