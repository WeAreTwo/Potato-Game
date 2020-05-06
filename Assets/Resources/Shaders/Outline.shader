Shader "URP/Outline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Delta("Delta", float) = 0.1
        _DepthDistance("Depth Distance ", float) = 10
        _OutlineThreshold("Outline Threshold", float) = 0.5
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
                
                TEXTURE2D(_CameraDepthNormalsTexture);
                SAMPLER(sampler_CameraDepthNormalsTexture);
                
                CBUFFER_START(UnityPerMaterial)               
                float _Delta;
                float _DepthDistance;
                float _OutlineThreshold;
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
                    //return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
                    return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r * _DepthDistance;
                }
                
                float sobel (float2 uv) 
                {
                    float2 delta = float2(_Delta * 0.0005, _Delta * 0.0005);
                    
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
                
                //https://alexanderameye.github.io/outlineshader for decoding normals 
                float3 DecodeNormal(float4 enc)
                {
                    float kScale = 1.7777;
                    float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
                    float g = 2.0 / dot(nn.xyz,nn.xyz);
                    float3 n;
                    n.xy = g*nn.xy;
                    n.z = g-1;
                    return n;
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
                    float rawDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenPos).r;
                    float4 rawDepthNormal = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, screenPos);
                    float3 decodedNormals = DecodeNormal(rawDepthNormal).rgb;
                                    
                    //BASE TEXTURE 
                    float4 Color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex , screenPos);
                    
                    float s = pow(1 - saturate(sobel(screenPos)), 50);
                    s = step(1/_OutlineThreshold,s);
                    //s = step(0.1, s);
                    //return float4(1,0,0,0);
                    //return rawDepth;
                    //return sceneCameraSpaceDepth;
                    //return half4(s.xxx, 1);
                    
                    return Color * s;        
                }
            ENDHLSL
        }

    }
    FallBack "Diffuse"
}
