//properties
CBUFFER_START(UnityPerMaterial)
float _Speed;
float2 _Pivot;
float _Strength;
float _Test;
float _Color;
float _HoleSize;
CBUFFER_END

//structs
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


//variables
static const float two_pi = 6.28318530718;
//functions
float2 rotateUv(float2 uv, float2 center, float degrees)
{
    float radAngle = radians(degrees);
    float2x2 rotationMatrix = float2x2( 
        float2(cos(radAngle), -sin(radAngle)), 
        float2(sin(radAngle), cos(radAngle) ));
    //offset uv coordinates by 0.5 to make it rotate around center
    uv -= center;
    uv = mul(uv, rotationMatrix);
    //shifting the coordinates back after rotating so the colors arent offset as well.
    uv += center;
    return uv;
}

float2 twirlUv(float2 uv, float2 center, float degrees)
{
    uv -= center;
    float radAngle = radians(degrees);
    radAngle *= length(uv) * 10;
    float2x2 rotationMatrix = float2x2(
        float2(cos(radAngle), -sin(radAngle)), 
        float2(sin(radAngle), cos(radAngle)));
    uv = mul(uv, rotationMatrix);
    uv += center;
    return uv;
}

float2 toPolar(float2 caCoords, float2 center)
{
    caCoords -= center;
    float distance = length(caCoords);
    float angle = atan2(caCoords.x, caCoords.y);
    caCoords = float2(angle / two_pi, distance);
    return caCoords;
}

float OneMinus(float x)
{
    return x = 1 - x;
}

float rand(float3 vec)
{
    //convert vec3d to scalar 1d with dot.
    float random = dot(vec, float3(12.9898, 78.233, 37.719));
    //take fraction of random so value isnt too high.
    random = frac(random);
    return random;
}

float Noise() 
{
    //float randomColor = rand(stuff.worldPos);
    return randomColor;
}

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.uv = IN.uv;
    return OUT;
}

half4 frag(Varyings IN) : SV_Target
{
    float2 centeredUv = IN.uv * 2 - 1;
    float2 polarUv = toPolar(IN.uv, _Pivot);
    float2 twirledUv = twirlUv(IN.uv, _Pivot, _Strength);
    float portalMask = OneMinus(polarUv.y);
    portalMask = 1 - smoothstep(0.5, _Test, length(centeredUv));
    float color = twirledUv * portalMask;
    float holeMask = pow(polarUv.y, _HoleSize);
    float2 nUv = noise(IN.uv);
    color *= holeMask;
    
    float strength = 10;
    color *= strength;
    float randomColor = Noise();
    return half4(randomColor, randomColor, randomColor, 1);
}