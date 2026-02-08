Shader "Ret/Ret-MasterPostProcess"
{
    Properties
    {
        _PixelationFactor("Pixelation", Float) = 0.5
        _ContrastBoost("Contrast", Float) = 1.3
        _DitheringScale("Dither Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            ZTest Always ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _PixelationFactor, _ContrastBoost, _DitheringScale, _HighResDitherMatrix;
            float3 _ColorResolution, _DitherResolution;

            float GetDitherThreshold(uint2 pPos) {
                const int dPS1[16] = {-4,0,-3,1, 2,-2,3,-1, -3,1,-4,0, 3,-1,2,-2};
                return dPS1[(pPos.x % 4) + (pPos.y % 4) * 4] * 0.125f;
            }

            float4 Frag (Varyings input) : SV_Target
            {
                // 1. PIXELATION
                float2 pixelScaling = _ScreenParams.xy * _PixelationFactor;
                float2 uv = floor(input.texcoord * pixelScaling) / pixelScaling;

                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                // 2. CONTRAST BOOST (This fixes the flat look)
                col.rgb = pow(abs(col.rgb), _ContrastBoost);

                // 3. DITHERING
                uint2 ditherPos = uint2(input.texcoord * _ScreenParams.xy * _DitheringScale);
                float threshold = GetDitherThreshold(ditherPos);
                
                // Color Quantization (The "Crunch")
                float3 cStep = 1.0f / max(_ColorResolution, 1.0f);
                col.rgb = floor(col.rgb / cStep) * cStep;
                
                // Apply Dither
                float3 dStep = 1.0f / max(_DitherResolution, 1.0f);
                col.rgb += threshold * dStep;

                return col;
            }
            ENDHLSL
        }
    }
}