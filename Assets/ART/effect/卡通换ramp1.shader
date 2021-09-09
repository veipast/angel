Shader "Unlit/ToolShader1"
{
    Properties
    {
        _MainTex("MainTex",2D)="white"{}
        _MainColor("MainColor",Color)=(1,1,1,1)
        [Enum(Off, 0, On, 1)] _Yellow("Yellow",Float)=0
        _Color("Color",Color)=(1,1,1,1)
        _RampTex ("RampTex", 2D) = "white" {}
        _AmbInt("AmbInt", Range(0,1)) = 1
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
         [Toggle(SHADOW)] _Fancy ("shadow?", Float) = 0
         [Toggle(Gray)] _Gray ("gray?", Float) = 0
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
             #pragma multi_compile _ _YELLOW_ON
            //  #pragma shader_feature _ _YELLOW_ON
             #pragma shader_feature SHADOW
             #pragma shader_feature Gray

            #include "UnityCG.cginc"
            #include "Lighting.cginc" 
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            sampler2D _RampTex;
            float _AmbInt,_Cutoff;
            fixed4 _Color,_MainColor;
            float _Yellow;
            

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos :TEXCOORD2;
                SHADOW_COORDS(3)
            };



            v2f vert (a2v v)
            {
                v2f o;
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f o) : SV_Target
            {

                fixed  shadow = SHADOW_ATTENUATION(o);
                fixed3 ambient = _AmbInt;
                
                fixed3 worldNormalDir = normalize(o.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));
                float Ndiff = saturate(dot(worldNormalDir, worldLightDir));
                float2 uv = float2(1-(Ndiff*0.5+0.5),0);
                float3 ramp = tex2D(_RampTex,uv); 
                fixed3 diffuse = ramp  * _LightColor0.rgb;


                float4 col = tex2D(_MainTex,o.uv)*_MainColor;
                clip(col.a - _Cutoff);

                

                 #if SHADOW
                 col *= shadow;
                 #endif

                fixed3 tempColor = (diffuse + ambient)*col;
                // #if _YELLOW_ON
                    
                     tempColor+=pow(_Color.rgb,2)*_Yellow;
                    //   return float4(tempColor,1);
                     
                //  #endif

                 

                 #if Gray
                 float grey = dot(tempColor, fixed3(0.22, 0.707, 0.071));
                 tempColor = float3(grey, grey, grey); 
                 #endif

                

                
                
                  return float4(tempColor,col.a);
                    
            }
            ENDCG
        }
    }
    FallBack "Specular"
}
