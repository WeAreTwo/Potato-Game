Shader "URP/Outline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    
    CGINCLUDE

    ENDCG
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Tags { "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                
                
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_CameraDepthTexture);
                SAMPLER(sampler_CameraDepthTexture);
                
                CBUFFER_START(UnityPerMaterial)               
                float _Delta;
                CBUFFER_END
                
                struct Attributes
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
        
                struct Varyings
                {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 screenPosition : TEXCOORD1;
                };
                
                //ref: UnityURPRenderingExamples
                float SampleDepth(float2 uv)
                {
                    return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
                    //return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                }
                
                float sobel (float2 uv) 
                {
                    float2 delta = float2(_Delta, _Delta);
                    
                    float hr = 0;
                    float vt = 0;
                    
                    hr += SampleDepth(uv + float2(-1.0, -1.0) * delta) *  1.0;
                    hr += SampleDepth(uv + float2( 1.0, -1.0) * delta) * -1.0;
                    hr += SampleDepth(uv + float2(-1.0,  0.0) * delta) *  2.0;
                    hr += SampleDepth(uv + float2( 1.0,  0.0) * delta) * -2.0;
                    hr += SampleDepth(uv + float2(-1.0,  1.0) * delta) *  1.0;
                    hr += SampleDepth(uv + float2( 1.0,  1.0) * delta) * -1.0;
                    
                    vt += SampleDepth(uv + float2(-1.0, -1.0) * delta) *  1.0;
                    vt += SampleDepth(uv + float2( 0.0, -1.0) * delta) *  2.0;
                    vt += SampleDepth(uv + float2( 1.0, -1.0) * delta) *  1.0;
                    vt += SampleDepth(uv + float2(-1.0,  1.0) * delta) * -1.0;
                    vt += SampleDepth(uv + float2( 0.0,  1.0) * delta) * -2.0;
                    vt += SampleDepth(uv + float2( 1.0,  1.0) * delta) * -1.0;
                    
                    return sqrt(hr * hr + vt * vt);
                }
           
                Varyings Vert(Attributes v)
                {
                    Varyings o;
                    o.position = TransformObjectToHClip(v.vertex);
                    o.uv = v.uv;
                    o.screenPosition = ComputeScreenPos(o.position);
                    return o;
                }
        
                float4 Frag (Varyings i) : SV_Target
                {
                    //SCREEN PARAM
                    float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                    
                    //DEPTH
                    float sceneCameraSpaceDepth = LinearEyeDepth(tex2Dproj(sampler_CameraDepthTexture, i.screenPosition).r, _ZBufferParams);
                                    
                    //BASE TEXTURE 
                    float4 Color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex , screenPos);
                    
                    float s = pow(1 - saturate(sobel(i.uv)), 50);
                    //s = step(0.1, s);
                    //return float4(1,0,0,0);
                    return sceneCameraSpaceDepth;
                    return half4(s.xxx, 1);
                    
                    return Color * s;        
                }
            ENDHLSL
        }

    }
    FallBack "Diffuse"
}
