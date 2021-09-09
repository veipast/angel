Shader "QKX/UI"
{
    Properties
    {
        
        
        // _KuanDu("KuanDu",range(0,20))=6
        // _shunshizhen("shunshizhen",float)=1
        // _nishizhen("nishizhen",float)=1
        // _speed("speed",float) = 1
        [HideInInspector]_MainTex("MainTex",2D)="white"{}
        
        
    }
    SubShader
    {
        Tags {"Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag      
            #include "UnityCG.cginc"

            sampler2D _MainTex;     
            // float _KuanDu;
            // float _speed;
            // float _shunshizhen;
            // float _nishizhen;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv:TEXCOORD;
                fixed4 color:COLOR;
            };
            
            struct v2f
            {
                float4 uv : TEXCOORD0;              
                float4 pos : SV_POSITION;                              
                fixed4 color:COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o=(v2f)0;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.uv.zw =v.uv;
                // o.uv.xy = v.uv+ _Time.y*_speed;
                // o.color=v.color;    
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {           
                fixed4 tex=tex2D(_MainTex,i.uv.zw); 
                return tex;
                // float4 r = i.uv.x*_shunshizhen+i.uv.y*_nishizhen;
                // float4 c=saturate(1-abs(frac(r)-0.5)*_KuanDu); 
                // //return  half4(tex.rgb+c,tex.a);//（rgb，a）分开计算的方法
                // return tex+tex.a*c*i.color;        
            }
            ENDCG
        }
    }
}
