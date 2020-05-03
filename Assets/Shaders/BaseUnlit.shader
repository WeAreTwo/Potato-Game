Shader "URP/BaseUnlit"
{
    //Basic URP Unlit template from 
    //ref : https://gist.github.com/phi-lira/10159a824e4e522060c47e21762941bb
    Properties
    {
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
        [LightStepThreshold] _LightStepThreshold("Light Step Threshold", float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

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
            
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _BaseColor;
            float _LightStepThreshold;
            CBUFFER_END

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
                //BASE TEXTURE
                float4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                
                Light mainLight = GetMainLight();
                float3 lightDirection = mainLight.direction;
                float attenuation = dot( IN.worldNorm, lightDirection);
                attenuation = step( _LightStepThreshold , attenuation);
                
                
                return baseTex * _BaseColor * attenuation;
                //return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor * IN.atten;
            }
            ENDHLSL
        }
    }
}