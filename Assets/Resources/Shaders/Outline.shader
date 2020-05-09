Shader "URP/Outline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Delta("Delta", float) = 0.1
        _DepthDistance("Depth Distance ", float) = 10
        _OutlineColor("Outline Color", Color) = (0,0,0,0)
        _OutlineThreshold("Outline Threshold", float) = 0.5
        _ColorFilterSensitivity("ColorFilterSensitivity", float) = 0.5
        _OutlineThickness("_OutlineThickness", float) = 0.5
        _DepthSensitivity("_DepthSensitivity", float) = 0.5
        _NormalsSensitivity("_NormalsSensitivity", float) = 0.5
        _ColorSensitivity("_ColorSensitivity", float) = 0.5
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
                float4 _MainTex_TexelSize;
                
                TEXTURE2D(_CameraDepthTexture);
                SAMPLER(sampler_CameraDepthTexture);
                
                TEXTURE2D(_CameraDepthNormalsTexture);
                SAMPLER(sampler_CameraDepthNormalsTexture);
                
                //CBUFFER_START(UnityPerMaterial)               
                float _Delta;
                float _DepthDistance;
                float _OutlineThreshold;
                float _ColorFilterSensitivity;
                float _OutlineThickness;
                float _DepthSensitivity;
                float _NormalsSensitivity;
                float _ColorSensitivity;
                float4 _OutlineColor;
                //CBUFFER_END
                
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
                
                //ref: UnityURPRenderingExamples
                float SampleDepth(float2 uv)
                {
                    return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r * _DepthDistance;
                }
                float3 SampleColor(float2 uv)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex , uv).rgb;
                }                
                float3 SampleNormal(float2 uv)
                {
                    float4 normals = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uv);
                    return DecodeNormal(normals).rgb;
                    //return decoded.r;
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
                
                float3 sampleNeighbours(float2 uv)
                {
                    float2 samples[9];
                    float3 colorSamples[9];
                    float3 normalSamples[9];
                    
                    samples[0] = uv + float2(0.0,0.0); //center
                    samples[1] = uv + float2(0.0,1.0); //top
                    samples[2] = uv + float2(0.0,-1.0); //bottom
                    samples[3] = uv + float2(-1.0,0.0); //left
                    samples[4] = uv + float2(0.0,1.0); //right
                    samples[5] = uv + float2(-1.0,1.0); //topLeft
                    samples[6] = uv + float2(1.0,1.0); //topRight
                    samples[7] = uv + float2(-1.0,-1.0); //bottomLeft
                    samples[8] = uv + float2(1.0,-1.0); //bottomRight
                    
                    for(uint i = 0; i < 9; i++)
                    {
                        colorSamples[i] = SampleColor(samples[i]);
                        normalSamples[i] = SampleNormal(samples[i]);
                        if(max(0,dot(normalSamples[0],normalSamples[i])) > _ColorFilterSensitivity)
                        {
                            return 0;
                        }
                    }
              
                    return colorSamples[0].rgb;
                }
                
                //https://alexanderameye.github.io/outlineshader
                float Outline_float(float2 UV, float OutlineThickness, float DepthSensitivity, float NormalsSensitivity, float ColorSensitivity, float4 OutlineColor, float4 Out)
                {
                    float halfScaleFloor = floor(OutlineThickness * 0.5);
                    float halfScaleCeil = ceil(OutlineThickness * 0.5);
                    float2 Texel = (1.0) / float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                
                    float2 uvSamples[4];
                    float depthSamples[4];
                    float3 normalSamples[4], colorSamples[4];
                
                    uvSamples[0] = UV - float2(Texel.x, Texel.y) * halfScaleFloor;
                    uvSamples[1] = UV + float2(Texel.x, Texel.y) * halfScaleCeil;
                    uvSamples[2] = UV + float2(Texel.x * halfScaleCeil, -Texel.y * halfScaleFloor);
                    uvSamples[3] = UV + float2(-Texel.x * halfScaleFloor, Texel.y * halfScaleCeil);
                
                    for(int i = 0; i < 4 ; i++)
                    {
                        depthSamples[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]).r;
                        normalSamples[i] = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamples[i]));
                        colorSamples[i] = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvSamples[i]);
                    }
                
                    // Depth
                    float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
                    float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
                    float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
                    float depthThreshold = (1/DepthSensitivity) * depthSamples[0];
                    edgeDepth = edgeDepth > depthThreshold ? 1 : 0;
                
                    // Normals
                    float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
                    float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
                    float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
                    edgeNormal = edgeNormal > (1/NormalsSensitivity) ? 1 : 0;
                
                    // Color
                    float3 colorFiniteDifference0 = colorSamples[1] - colorSamples[0];
                    float3 colorFiniteDifference1 = colorSamples[3] - colorSamples[2];
                    float edgeColor = sqrt(dot(colorFiniteDifference0, colorFiniteDifference0) + dot(colorFiniteDifference1, colorFiniteDifference1));
                    edgeColor = edgeColor > (1/ColorSensitivity) ? 1 : 0;
                
                    return max(edgeDepth, max(edgeNormal, edgeColor));
                    float edge = max(edgeDepth, max(edgeNormal, edgeColor));
                
                    float4 original = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvSamples[0]);	
                    return Out = ((1 - edge) * original) + (edge * lerp(original, OutlineColor,  OutlineColor.a));
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
                    
                    //Sample neighbours 
                    float3 ColorFilter = sampleNeighbours(screenPos);
                    
                    //Outline
                    float edge = Outline_float(screenPos,  _OutlineThickness,  _DepthSensitivity,  _NormalsSensitivity,  _ColorSensitivity, _OutlineColor, Color);
                    float4 outline = edge * _OutlineColor;
                    
                    //SOBEL operation
                    float s = pow(1 - saturate(sobel(screenPos)), 50);
                    s = step(1/_OutlineThreshold,s);
                    //s = step(0.1, s);
                    //return float4(1,0,0,0);
                    //return rawDepth;
                    //return sceneCameraSpaceDepth;
                    //return half4(s.xxx, 1);
    
                    return lerp(Color, _OutlineColor, edge);
                    return Color * s;        
                }
            ENDHLSL
        }

    }
    FallBack "Diffuse"
}
