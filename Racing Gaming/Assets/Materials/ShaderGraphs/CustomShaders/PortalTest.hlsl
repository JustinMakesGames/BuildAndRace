struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

CBUFFER_START(UnityPerMaterial)
float _Speed;
float2 _Pivot;
CBUFFER_END

//variables


//functions
float2 rotateUv(float2 uv, float2 center, float degrees)
{
    float radAngle = radians(degrees);
    float2x2 rotationMatrix = float2x2( float2(cos(radAngle), -sin(radAngle)), float2(sin(radAngle), cos(radAngle) ));
    uv -= center;
    uv = mul(uv, rotationMatrix);
    uv *= center;
    return uv;
}

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.uv = IN.uv;
    return OUT;
}

float3 frag(Varyings IN) : SV_Target
{
    //float2 color = rotateUv(IN.uv, 1);
    
    float3 color = float3(rotateUv(IN.uv, _Pivot, _Time.y * _Speed), 0);
    return color;
}