Shader "Ret/BasicLit"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        _DebugForceLight("Debug: Force White", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalPipeline" 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry"
        }
        
        Cull Off 

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // These tell Unity 6 to actually send the light data buffer
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            sampler2D _BaseMap;
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _DebugForceLight;
            CBUFFER_END

            Varyings vert(Attributes input) {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = input.uv;
                return output;
            }

            float4 frag(Varyings input) : SV_Target {
                float3 normal = normalize(input.normalWS);
                
                // 1. Get Main Light (Sun)
                Light mainLight = GetMainLight();
                float3 lightResult = saturate(dot(normal, mainLight.direction)) * mainLight.color;

                // 2. Get Additional Lights (The Flashlight)
                // We use the 'uint' loop specifically for Unity 6
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0u; i < lightCount; ++i) {
                    // Modern URP 6 requires the shadowMask parameter (half4(1,1,1,1))
                    Light light = GetAdditionalLight(i, input.positionWS, half4(1,1,1,1));
                    float diffuse = saturate(dot(normal, light.direction));
                    lightResult += diffuse * light.color * light.distanceAttenuation;
                }

                // 3. Debug Override
                lightResult = lerp(lightResult, float3(1,1,1), _DebugForceLight);

                float4 tex = tex2D(_BaseMap, input.uv) * _BaseColor;
                return float4(tex.rgb * lightResult, 1.0);
            }
            ENDHLSL
        }
    }
}