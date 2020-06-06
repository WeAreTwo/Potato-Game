﻿Shader "URP/BaseUnlit"
{
    Properties
    {
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        [InsideColor] _InsideColor("InsideColor", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
        //[BlueNoiseMap] _BlueNoiseMap("BlueNoiseMap", 2D) = "white" {}
        [BlueNoiseMapScale] _BlueNoiseMapScale("BlueNoiseMapScale", float) = 1
        //[DetailMap] _DetailMap("DetailMap", 2D) = "white" {}
        [DetailAmount] _DetailAmount("DetailAmount", float) = 0.5
        [DetailScale] _DetailScale("DetailScale", float) = 0.5
        [NoiseScale] _NoiseScale("NoiseScale", float) = 0.5
        [LightStepThreshold] _LightStepThreshold("Light Step Threshold", float) = 0.5
        
        [TestParam] _TestParam("TestParam", float) = 0.5
        
        
        
        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        
        _ReceiveShadows("Receive Shadows", Float) = 1.0

        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
    }

    SubShader
    {
    
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            LOD 300
            
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _OCCLUSIONMAP

            #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _RECEIVE_SHADOWS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "noise.hlsl"
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
                float4 shadowCoord  : TEXCOORD1; // compute shadow coord per-vertex for the main light
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
            half4 _InsideColor;
            float _LightStepThreshold;
            float _BlueNoiseMapScale;
            float _DetailAmount;
            float _DetailScale;
            float _NoiseScale;
            
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
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs vNormalInputs = GetVertexNormalInputs(IN.normal);
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                
                // shadow coord for the main light is computed in vertex.
                // If cascades are enabled, LWRP will resolve shadows in screen space
                // and this coord will be the uv coord of the screen space shadow texture.
                // Otherwise LWRP will resolve shadows in light space (no depth pre-pass and shadow collect pass)
                // In this case shadowCoord will be the position in light space.
                OUT.shadowCoord = GetShadowCoord(vertexInput);
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
                //float4 ambientColor = float4(0.1,0.1,0.1,0.1) + (_BaseColor * 0.05);
                float4 ambientColor = (_BaseColor * 0.9);
                float4 white = float4(1,1,1,1);
                
                // Main light is the brightest directional light.
                // It is shaded outside the light loop and it has a specific set of variables and shading path
                // so we can be as fast as possible in the case when there's only a single directional light
                // You can pass optionally a shadowCoord (computed per-vertex). If so, shadowAttenuation will be
                // computed.
                Light mainLight = GetMainLight(IN.shadowCoord);

                float3 lightDirection = mainLight.direction;
                float4 lightColor = float4(mainLight.color, 1);
                half shadowAtten = mainLight.shadowAttenuation;
                half distanceAttenuation = mainLight.distanceAttenuation;
                
                float attenuation = dot( IN.worldNorm, lightDirection);
                //attenuation = smoothstep( blueNoiseTex, _LightStepThreshold , attenuation);
                //attenuation = step( blueNoiseTex - _LightStepThreshold , attenuation);
                
                float4 base = baseTex * _BaseColor * lightColor;
                //float4 baseWithDetails = lerp(base, ambientColor, details);
                float4 baseWithDetails = lerp(ambientColor, base, shadowAtten);
                //float noiseMask = clamp(cnoise(IN.uv / _NoiseScale), 0.1, 1.2);
                
                output = lerp(ambientColor, baseWithDetails , attenuation);
                //output = lerp(output, _InsideColor, noiseMask);
                
                return output;
                
            }
            ENDHLSL
        }
        
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/Meta"

        
    }
    FallBack "Unlit"
}