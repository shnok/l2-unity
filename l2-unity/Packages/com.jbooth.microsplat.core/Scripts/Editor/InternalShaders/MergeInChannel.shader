//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

Shader "Hidden/MicroSplat/MergeInChannel" 
{

   Properties
   {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _TargetTex("Target", 2D) = "white" {}
      _TargetChannel("Target Channel", Int) = 0
      _MergeChannel("Merge Channel", Int) = 0
      _Invert("Invert", Int) = 0
   }

   SubShader {
      Pass {
         ZTest Always Cull Off ZWrite Off
            
         CGPROGRAM
         #pragma vertex vert_img
         #pragma fragment frag
         #include "UnityCG.cginc"
         #include "UnityCG.cginc"

          sampler2D _MainTex;
          sampler2D _TargetTex;
          float4 _MainTex_TexelSize;
          int _TargetChannel;
          int _MergeChannel;
          int _Invert;

         float4 frag(v2f_img i) : SV_Target
         {
             float4 main = tex2D(_MainTex, i.uv);
             float4 target = tex2D(_TargetTex, i.uv);

             // why not array access? Throws compile error!
             // target[_TargetChannel] = main[_MergeChannel];

             if (_TargetChannel == 0)
             {
                if (_MergeChannel == 0)
                    target.r = main.r;
                else if (_MergeChannel == 1)
                    target.r = main.g;
                else if (_MergeChannel == 2)
                    target.r = main.b;
                else
                    target.r = main.a;
             }
             else if (_TargetChannel == 1)
             {
                if (_MergeChannel == 0)
                    target.g = main.r;
                else if (_MergeChannel == 1)
                    target.g = main.g;
                else if (_MergeChannel == 2)
                    target.g = main.b;
                else
                    target.g = main.a;
             }
             else if (_TargetChannel == 2)
             {
                if (_MergeChannel == 0)
                    target.b = main.r;
                else if (_MergeChannel == 1)
                    target.b = main.g;
                else if (_MergeChannel == 2)
                    target.b = main.b;
                else
                    target.b = main.a;
             }
             else
             {
                if (_MergeChannel == 0)
                    target.a = main.r;
                else if (_MergeChannel == 1)
                    target.a = main.g;
                else if (_MergeChannel == 2)
                    target.a = main.b;
                else
                    target.a = main.a;
             }

             
             if (_Invert > 0.5)
             {
                if (_TargetChannel == 0)
                   target.r = 1.0 - target.r;
                else if (_TargetChannel == 1)
                   target.g = 1.0 - target.g;
                else if (_TargetChannel == 1)
                   target.b = 1.0 - target.b;
                else
                   target.a = 1.0 - target.a;
             }

             return target;
         }
         ENDCG
      }

   }

   Fallback off

}