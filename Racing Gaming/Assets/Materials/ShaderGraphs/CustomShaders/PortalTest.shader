Shader"Custom/PortalTest"
{
    Properties
    {
        _Speed("Degrees per frame or something", float ) = 100
        _Test("Test", float ) = 0
        _Strength("Twirl strength", Range (0, 1000)) = 0 
        _Pivot("Pivot position", Vector) = (0, 0, 0, 0)
        _Color("Color", Color) = (1, 1, 1, 1)

    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/Materials/ShaderGraphs/CustomShaders/PortalTest.hlsl"
            
            ENDHLSL
        }
    }
}
