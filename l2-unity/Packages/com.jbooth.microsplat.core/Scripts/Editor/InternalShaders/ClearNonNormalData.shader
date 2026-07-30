//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

Shader "Hidden/MicroSplat/ClearNonNormalData" 
{
   Properties {
      _MainTex ("Base (RGB)", 2D) = "bump" {}
   }



   SubShader {
      Pass {
         ZTest Always Cull Off ZWrite Off
            
         CGPROGRAM
         #pragma vertex vert_img
         #pragma fragment frag
         #include "UnityCG.cginc"
         #include "UnityCG.cginc"
   
      struct v2f {
         float4 pos : SV_POSITION;
         float2 uv : TEXCOORD0;
      };
      
      sampler2D _MainTex;
      float _Swizzle;


      half BlendOverlay(half base, half blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
         
          
      fixed4 frag(v2f_img i) : SV_Target
      {
         half4 data = tex2D(_MainTex, i.uv);
         if (_Swizzle > 0.5)
         {
            data.ga = data.gr;
         }
         data.r = 0;
         data.b = 1;
         return data;
      }
         ENDCG
      }

   }

   Fallback off

}