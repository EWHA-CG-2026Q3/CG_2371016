Shader "CG/S17_MyTextureAbs"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END

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

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Quad upper-left UV quarter (0..0.5, 0.5..1) remaps
                // to the texture lower-right quarter (0.5..1, 0..0.5).
                bool inQuadUpperLeft = input.uv.x >= 0.0 && input.uv.x <= 0.5 &&
                                       input.uv.y >= 0.5 && input.uv.y <= 1.0;
                if (inQuadUpperLeft)
                {
                    float2 textureUV = float2(input.uv.x + 0.5, input.uv.y - 0.5);
                    return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, textureUV);
                }
                return half4(0.5, 0.5, 0.5, 1.0);
            }
            ENDHLSL
        }
    }
}
