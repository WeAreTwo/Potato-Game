Shader "URP/Outline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    
    CGINCLUDE
        #include "UnityCG.cginc"
    
        sampler2D _MainTex;  
        
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 screenPosition : TEXCOORD1;
        };
   
        v2f Vert(appdata v)
        {
            v2f o;
            o.position = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            o.screenPosition = ComputeScreenPos(o.position);
            return o;
        }

        float4 Frag (v2f i) : SV_Target
        {
            //base texture 
            float4 Color = tex2D(_MainTex, i.uv);
            
            float2 screenPos = i.screenPosition.xy / i.screenPosition.w;

            return Color;        
        }
    ENDCG
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Tags { "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            CGPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
            ENDCG
        }
    }
}
