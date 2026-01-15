Shader"Custom/PortalTest"
{
    Properties
    {
        _Speed("Degrees per frame or something", float ) = 100
        _Pivot("Pivot position", Vector) = (0, 0, 0, 0)
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
