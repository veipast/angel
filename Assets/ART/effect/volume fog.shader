

Shader "QKX/volume fog" {
    Properties {
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Mask("Mask Texture", 2D) = "white" {}
        _FoamSmooth("FoamSmooth", Range(0,80)) = 0

        /*_Density("Density", Range(0,180)) = 0
        _Speed("Speed", Range(0,20)) = 0
        _Amount("Amount", Range(0,1)) = 0
        _Color ("Color", Color) = (0.5,0.5,0.5,0.5)*/
    }

    Category {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull back Lighting Off ZWrite Off

        SubShader {
            Pass {

                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile_particles
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                sampler2D _MainTex,_Mask;
                float4 _MainTex_ST,_Mask_ST;
                fixed4 _TintColor;
                float _FoamSmooth;
                /*float _Density,_Speed,_Amount;
                fixed4 _Color;*/

                struct appdata  {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 projPos : TEXCOORD2;

                };

                

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.projPos = ComputeScreenPos (o.vertex);
                    o.color = v.color * _TintColor;
                    o.uv= v.uv;
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
                
                fixed4 frag (v2f i) : SV_Target
                {
                    fixed4 Mask= tex2D(_Mask, i.uv);


                    /*float2 uv = (floor((i.uv*1.0+1.0) * _Density) / (_Density - 1)+(_Time.r*0.001*_Speed)*float2(0.1,0.1));
                    float2 uv2 = uv + 0.2127+uv.x*0.3713*uv.y;
                    float2 uv3 = 4.789*sin(489.123*(uv2));
                    float uv4 = frac(uv3.x*uv3.y*(1+uv2.x));
                    float uv5 = saturate(((1.0 - _Amount)*3.333333+-2.333333));
                    float uv6 = (uv5*uv5);
                    float4 uv7 = saturate((smoothstep( _Amount, 1.0, uv4 )+uv6))*saturate((round((i.uv.r*100.0+0.0))*round((i.uv.g*100.0+0.0))))*_Color;*/

                    fixed4 col = tex2D(_MainTex, i.uv)*i.color;
                    fixed depthSample = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                    fixed depth = LinearEyeDepth(depthSample);//???????????????????????????
                    fixed smooth = smoothstep( 0.0 , 1.0 , (depth - i.projPos.w) / (_FoamSmooth - 0.0));
                    fixed formsidesmoothform = lerp( 0, 1 , smooth);
                    UNITY_APPLY_FOG(i.fogCoord, col);

                    return fixed4(col.xyz, formsidesmoothform*col.a*Mask.a);
                }
                ENDCG
            }
        }
    }
}
