Shader "URP/BaseUnlit"
{
    Properties
    {
        [MainColor] _BaseColor("BaseColor", Color) = (1,1,1,1)
        [InsideColor] _InsideColor("InsideColor", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("BaseMap", 2D) = "white" {}
        //[BlueNoiseMap] _BlueNoiseMap("BlueNoiseMap", 2D) = "white" {}
        [BlueNoiseMapScale] _BlueNoiseMapScale("BlueNoiseMapScale", float) = 1
        //[DetailMap] _DetailMap("DetailMap", 2D) = "white" {}
        //[DetailAmount] _DetailAmount("DetailAmount", float) = 0.5
        //[DetailScale] _DetailScale("DetailScale", float) = 0.5
        [NoiseScale] _NoiseScale("NoiseScale", float) = 0.5        
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
            
            #define PI 3.14      
            
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
                float3 worldPos    : TEXCOORD3;
                float3 viewPos    : TEXCOORD4;
                float3 clipPos    : TEXCOORD5;
                float3 ndcPos    : TEXCOORD6;
                float4 screenPosition : TEXCOORD7;
                float3 normal           : NORMAL;
                float4 positionHCS  : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);               
            TEXTURE2D(_DetailMap);
            SAMPLER(sampler_DetailMap);            
            TEXTURE2D(_BlueNoiseMap);
            SAMPLER(sampler_BlueNoiseMap);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _DetailMap_ST;
            float4 _BlueNoiseMap_ST;
            
            half4 _BaseColor;
            half4 _InsideColor;
            float _LightStepThreshold;
            float _BlueNoiseMapScale;
            float _DetailAmount;
            float _DetailScale;
            float _NoiseScale;
            float _AttenStrength;
            
            float _DirtDistance;
            float _DirtCutOff;
            
            float _TestParam;
            CBUFFER_END
            
            //BLENDING FUNCTIONS 
            float4 screen(float4 colorOne, float4 colorTwo, float fac)
            {
                float facm = 1- fac;
                return  1 - ((1 - colorOne) * (facm + fac * (1 - colorTwo)));
            }
            
            float Shaping_Sharp(float x)
            {
                return 1.0 - pow(abs(sin(PI * x/2.0)), 0.5);
            }
            
            float Shaping_Smooth(float x)
            {
                return pow(cos(PI * x / 2.0), 0.5);
            }
            
            float3 rgb_to_hsv_no_clip(float3 RGB)
            {
                float3 HSV;
                
                float minChannel, maxChannel;
                if (RGB.x > RGB.y) {
                maxChannel = RGB.x;
                minChannel = RGB.y;
                }
                else {
                maxChannel = RGB.y;
                minChannel = RGB.x;
                }
                
                if (RGB.z > maxChannel) maxChannel = RGB.z;
                if (RGB.z < minChannel) minChannel = RGB.z;
                
                HSV.xy = 0;
                HSV.z = maxChannel;
                float delta = maxChannel - minChannel;             //Delta RGB value
                if (delta != 0) {                    // If gray, leave H  S at zero
                   HSV.y = delta / HSV.z;
                   float3 delRGB;
                   delRGB = (HSV.zzz - RGB + 3*delta) / (6.0*delta);
                   if      ( RGB.x == HSV.z ) HSV.x = delRGB.z - delRGB.y;
                   else if ( RGB.y == HSV.z ) HSV.x = ( 1.0/3.0) + delRGB.x - delRGB.z;
                   else if ( RGB.z == HSV.z ) HSV.x = ( 2.0/3.0) + delRGB.y - delRGB.x;
                }
                return (HSV);
            }
            
            
            float3 hsv_to_rgb(float3 HSV)
            {
                float3 RGB = HSV.z;
                
                float var_h = HSV.x * 6;
                float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
                float var_1 = HSV.z * (1.0 - HSV.y);
                float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
                float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
                if      (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
                else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
                else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
                else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
                else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
                else                 { RGB = float3(HSV.z, var_1, var_2); }
               
                return (RGB);
            }
            
            float InvertNormalized(float value)
            {
                return 1 - value;            
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs vNormalInputs = GetVertexNormalInputs(IN.normal);
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                
                // shadow coord for the main light is computed in vertex.
                // If cascades are enabled, LWRP will resolve shadows in screen space
                // and this coord will be the uv coord of the screen space shadow texture.
                // Otherwise LWRP will resolve shadows in light space (no depth pre-pass and shadow collect pass)
                // In this case shadowCoord will be the position in light space.
                OUT.screenPosition = ComputeScreenPos(OUT.positionHCS);
                OUT.shadowCoord = GetShadowCoord(vertexInput);
                OUT.normal = IN.normal;
                OUT.worldNorm = vNormalInputs.normalWS;
                OUT.worldPos = vertexInput.positionWS;
                OUT.viewPos = vertexInput.positionVS;
                OUT.clipPos = vertexInput.positionCS;
                OUT.ndcPos = vertexInput.positionNDC;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 output;
                
                //depth Handling
                //float existingDepth01 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, IN.screenPosition).r;
                float existingDepth01 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, IN.screenPosition.xy / IN.screenPosition.w).r ;
                float existingDepthLinear = LinearEyeDepth(existingDepth01, _ZBufferParams);
                float depthDifference = existingDepthLinear - IN.screenPosition.w;
                // Add in the fragment shader, above the existing surfaceNoise declaration.
                float foamDepthDifference01 = saturate(depthDifference / _DirtDistance);
                //float foamDepthDifference01 = saturate(depthDifference);
                float surfaceNoiseCutoff = foamDepthDifference01 * _DirtCutOff;
                
              
                float surfaceNoise = cnoise(IN.uv) > surfaceNoiseCutoff ? 1 : 0;
                
                //TRIPLANAR
                // GET TRIPLANAR UVS
                float2 uv_noise = float2(cnoise(IN.uv), cnoise(IN.uv));
                float2 uv_base = TRANSFORM_TEX(IN.worldPos.xy, _DetailMap);
                float2 uv_front = TRANSFORM_TEX(IN.worldPos.xy, _DetailMap);
                float2 uv_side = TRANSFORM_TEX(IN.worldPos.zy, _DetailMap);
                float2 uv_top = TRANSFORM_TEX(IN.worldPos.xz, _DetailMap);
                
                // SAMPLE THE SAME TEX FROM 3 DIFF UV IN OBJECT SPACE 
                //read texture at uv position of the three projections
                float4 col_base = SAMPLE_TEXTURE2D(_DetailMap,sampler_DetailMap, uv_base/ _DetailScale);
                float4 col_front = SAMPLE_TEXTURE2D(_DetailMap,sampler_DetailMap, uv_front/ _DetailScale);
                float4 col_side = SAMPLE_TEXTURE2D(_DetailMap,sampler_DetailMap, uv_side/ _DetailScale);
                float4 col_top = SAMPLE_TEXTURE2D(_DetailMap,sampler_DetailMap, uv_top/ _DetailScale);
                
                //generate weights from world normals
                float3 weights = IN.normal;
                //show texture on both sides of the object (positive and negative)
                weights = abs(weights);
                weights = pow(weights, 64);                
                weights = weights / (weights.x + weights.y + weights.z);
                
                //combine weights with projected colors
                col_front *= weights.z;
                col_side *= weights.x;
                col_top *= weights.y;
                
                //combine the projected colors
                float col = (col_front + col_side + col_top).r;
                
                //BASE TEXTURE
                float baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv).r;                
                
                //DETAIL TEXTURE
                float detailTex = SAMPLE_TEXTURE2D(_DetailMap, sampler_DetailMap, IN.uv/_DetailScale).r;
                col = step(_DetailAmount, col);
                
                //BLUE NOISE MAP 
                float blueNoiseTex = SAMPLE_TEXTURE2D(_BlueNoiseMap, sampler_BlueNoiseMap, IN.uv/ _BlueNoiseMapScale).r;
                
                //ABIENT COLOR (SHADOW COLOR)
                float4 ambientColor = float4(rgb_to_hsv_no_clip(_BaseColor),1);
                //ambientColor.r = InvertNormalized(ambientColor.r) ;
                ambientColor = float4(hsv_to_rgb(ambientColor), 1);
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
                float attenuationFromView = dot( IN.worldNorm, GetCameraPositionWS()) * _AttenStrength;
                float detailAtten = saturate(attenuationFromView);
                detailAtten = Shaping_Smooth(detailAtten);
                
                attenuation = step( 0.3 , attenuation); // TODO uniform global variable here needed 
                
                //float4 base = baseTex * _BaseColor * lightColor;
                float4 base = _BaseColor * lightColor;
                float4 baseWithDetails = lerp(ambientColor, base, shadowAtten);
                
                //return surfaceNoiseCutoff;
                return output = lerp(ambientColor ,  baseWithDetails, attenuation  );
                return output = lerp(ambientColor ,  baseWithDetails, attenuation  ) + surfaceNoise * half4(0,1,0,0);
                
                return lerp(output, float4(0,0,0,0), (normalize(attenuationFromView)));
                return lerp(output, float4(0,0,0,0), (normalize(col * attenuationFromView)));
                return col * attenuationFromView;
                return col;
                return detailTex;
                return output;
                return float4(1,1,1,1) * attenuationFromView;
                return float4(IN.worldNorm,1);
                
            }
            ENDHLSL
        }
        
        //UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/Meta"

        
    }
    FallBack "Unlit"
}