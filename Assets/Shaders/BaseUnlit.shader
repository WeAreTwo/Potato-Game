Shader "URP/BaseUnlit"
{
    Properties
    {
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
        [BlueNoiseMap] _BlueNoiseMap("BlueNoiseMap", 2D) = "white" {}
        [BlueNoiseMapScale] _BlueNoiseMapScale("BlueNoiseMapScale", float) = 1
        [DetailMap] _DetailMap("DetailMap", 2D) = "white" {}
        [DetailAmount] _DetailAmount("DetailAmount", float) = 0.5
        [DetailScale] _DetailScale("DetailScale", float) = 0.5
        [LightStepThreshold] _LightStepThreshold("Light Step Threshold", float) = 0.5
        
        [TestParam] _TestParam("TestParam", float) = 0.5
    }

    SubShader
    {
    
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            LOD 300

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normal       : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float  atten        : TEXCOORD1;
                float3 worldNorm    : TEXCOORD2;
                float4 positionHCS  : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);               
            TEXTURE2D(_DetailMap);
            SAMPLER(sampler_DetailMap);            
            TEXTURE2D(_BlueNoiseMap);
            SAMPLER(sampler_BlueNoiseMap);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _BaseColor;
            float _LightStepThreshold;
            float _BlueNoiseMapScale;
            float _DetailAmount;
            float _DetailScale;
            
            float _TestParam;
            CBUFFER_END
            
            //BLENDING FUNCTIONS 
            float4 screen(float4 colorOne, float4 colorTwo, float fac)
            {
                float facm = 1- fac;
                return  1 - ((1 - colorOne) * (facm + fac * (1 - colorTwo)));
            }
            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexNormalInputs vNormalInputs = GetVertexNormalInputs(IN.normal);
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                
                Light mainLight = GetMainLight();
                float3 lightDirection = mainLight.direction;                
                
                float attenuation;
                //attenuation = dot( vNormalInputs.normalWS, lightDirection);
                //attenuation = step(0.2 , attenuation);
                
                OUT.atten = attenuation;
                OUT.worldNorm = vNormalInputs.normalWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 output;
                //BASE TEXTURE
                float baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).r;                
                
                //DETAIL TEXTURE
                float detailTex = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, IN.uv/_DetailScale).r;
                float details = step(_DetailAmount, detailTex);
                
                //BLUE NOISE MAP 
                float blueNoiseTex = SAMPLE_TEXTURE2D(_BlueNoiseMap, sampler_BlueNoiseMap, IN.uv/ _BlueNoiseMapScale).r;
                
                //ABIENT COLOR (SHADOW COLOR)
                //float4 ambientColor = float4(0.1,0.1,0.1,0.1);
                float4 ambientColor = float4(0.1,0.1,0.1,0.1) + (_BaseColor * 0.05);
                
                Light mainLight = GetMainLight();
                float3 lightDirection = mainLight.direction;
                float4 lightColor = float4(mainLight.color, 1);
                
                float attenuation = dot( IN.worldNorm, lightDirection);
                //attenuation = smoothstep( blueNoiseTex, _LightStepThreshold , attenuation);
                attenuation = step( blueNoiseTex - _LightStepThreshold , attenuation);
                
                float4 base = baseTex * _BaseColor * lightColor;
                float4 baseWithDetails = lerp(base, ambientColor, details);
                output = lerp(ambientColor, baseWithDetails , attenuation);
                
                
                return output;
                
                //return baseTex * _BaseColor * attenuation * lightColor;
                //return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor * IN.atten;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Unlit"
}