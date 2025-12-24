Shader"Custom/Test" {
    Properties {
        [MainTexture] _ColorMap ("Texture", 2D) = "white" {}
        _Color ("First Color", Color) = (0 ,0 ,0 ,1)
        _SecondColor ("Second Color", Color) = (0, 0, 0, 0)
        _Brightness ("Brightness", Float) = 5.0
        _Scale ("Scale", Float) = 3.0
        _Test ("Test", Range(0.0, 100)) = 1.0
    }

    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off

        Pass {
            HLSLPROGRAM
            #include_with_pragmas "Assets/Materials/ShaderGraphs/CustomShaders/TestShader.hlsl" 
            ENDHLSL
        }
    }
   
}
