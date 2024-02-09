Shader "Hidden/KriptoFX/KWS/FluidSimulation"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma enable_d3d11_debug_symbols

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "../KWS_WaterVariables.cginc"

            #pragma multi_compile _ CAN_USE_NEXT_LOD
            #pragma shader_feature KW_FLUIDS_PREBAKE_SIM

            sampler2D _MainTex;
            sampler2D _BordersTex;
            float4 _MainTex_TexelSize;
            float MouseClicked;
            float2 _MousePos;
            float _MouseSize;
            float2 _MouseDir;

            float4 Test;
            float4 Test2;

            float3 _CurrentPositionOffset;
            float3 _LastPositionOffset;
            float _AreaSize;
            float _FlowSpeed;
            sampler2D _FluidsNextLod;
            float4 _FluidsNextLod_TexelSize;
            float _FoamTexelOffset;
            float3 _CurrentFluidMapWorldPos;
            sampler2D KW_FluidsPrebaked;
            float KW_FluidsRequiredReadPrebakedSim;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = float4(v.vertex.xy - 0.5, 0, 0.5);
                o.uv = v.uv;
                if (_ProjectionParams.x < 0) o.uv.y = 1 - o.uv.y;

                o.uv = o.uv + _CurrentPositionOffset.xz;
                o.uv2 = o.uv;
                float2 worldUV = o.uv * _AreaSize - _AreaSize * 0.5;
                o.worldPos = float3(worldUV.x, 0, worldUV.y) + _CurrentFluidMapWorldPos;
                return o;
            }

            struct FragmentOutput
            {
                half4 dest0 : SV_Target0;
                half4 dest1 : SV_Target1;
            };

            FragmentOutput  frag(v2f i)
            {
               float K = 0.15;
               float v = 0.06;
               float dt = 0.13;

               float4  data = tex2D(_MainTex, i.uv);
               float4 tr = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x, 0)); // tex right
               float4 tl = tex2D(_MainTex, i.uv - float2(_MainTex_TexelSize.x, 0)); // tex left
               float4 tu = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y)); // tex up
               float4 td = tex2D(_MainTex, i.uv - float2(0, _MainTex_TexelSize.y)); // tex down

               float3 dx = (tr.xyz - tl.xyz) * 0.5; //delta
               float3 dy = (tu.xyz - td.xyz) * 0.5;
               float2 densDif = float2(dx.z, dy.z);

               data.z -= _FlowSpeed * 0.5 * dt * dot(float3(densDif, dx.x + dy.y), data.xyz); //density. _MainTex.z为流体粒子密度. 密度基本只受这一行代码影响
                // 新密度 = 旧密度 - 流走的总密度. 流走的总密度 = 流走的速度 * 周围像素密度变化幅度 

               float2 laplacian = tu.xy + td.xy + tr.xy + tl.xy - 4.0 * data.xy; // 由该句可以看出_MainTex.xy为流体速度. laplacian为拉普拉斯算子.
               float2 viscForce = v * laplacian; // 上一帧流体速度带来的粘性力
               // 可以看出, laplacian.xy是四周像素速度与中心像素速度的差值.
               // 可以把上下左右四个像素想象为四个流体微团, 当它们x方向上速度平均大于中心流体微团时, 对中心流体微团起带动作用. 反之起拖累作用.

               data.xyw = tex2D(_MainTex, i.uv - 2.0 * dt * data.xy * _MainTex_TexelSize.xy * _FlowSpeed).xyw; //adfloattion. 该帧该像素的速度以流动至该处的上一帧其他位置像素的速度为基础

               float2 newForce = 0; // 因为flowmap带来的这一帧的流体外力
               float2 flowMapUV = (i.worldPos.xz - KW_FlowMapOffset.xz) / KW_FlowMapSize + 0.5;
               float2 flowmap = KW_FlowMapTex.Sample(sampler_linear_repeat, flowMapUV) * 2 - 1;

               newForce.xy += (flowmap) * 0.03 * _FlowSpeed;  // newforce以flowmap中信息起代替重力的加速作用

               data.xy += _FlowSpeed * 1.5 * dt * (viscForce.xy - K / dt * densDif + newForce); //update velocity. 在上面计算的基础速度之上增加受力带来的新速度. 粘性力起阻滞作用, newforce起加速作用
                // 可以注意到流速方向和flowmap的正方向一致
               data.xy = max(0, abs(data.xy) - 1e-4) * sign(data.xy); //linear velocity decay

               data.w = (tr.y - tl.y - (tu.x - td.x)); // data.w为涡的速度幅值, X向速度在Y向的梯度(tr.y - tl.y)和Y向速度在X向的梯度(tu.x - td.x)越大时涡越大.
               float2 vort = float2(abs(tu.w) - abs(td.w), abs(tl.w) - abs(tr.w)); // vortex:涡. 北半球逆时针, 因此以上和左为正方向
               vort *= _FlowSpeed * 0.14 / length(vort + 1e-9) * data.w; // 等效于乘了vort的单位向量, 因此vort只提供旋涡速度的方向

               data.xy += vort;
               //data.z += (vort.x + vort.y) * 0.25;
                
               float2 depthUV = (i.worldPos.xz - KW_FluidsDepthPos.xz) / KW_FluidsDepthOrthoSize + 0.5;
               float depth = KW_FluidsDepthTex.SampleLevel(sampler_linear_clamp, depthUV, 0).r * KW_FluidsDepth_Near_Far_Dist.z - KW_FluidsDepth_Near_Far_Dist.y;

               data.xy *= depth < 0.0;
               data.xyz *= 0.9999;

               bool isOutArea = i.uv.x < 0.01 || i.uv.x > 0.99 || i.uv.y < 0.01 || i.uv.y > 0.99;

#if CAN_USE_NEXT_LOD
               if (isOutArea)
               {
                   float2 fluidsUV_lod1 = (i.worldPos.xz - KW_FluidsMapWorldPosition_lod1.xz) / KW_FluidsMapAreaSize_lod1 + 0.5;
                   data = tex2D(_FluidsNextLod, fluidsUV_lod1);

               }
               if (KW_FluidsRequiredReadPrebakedSim) data.xyzw = tex2D(KW_FluidsPrebaked, depthUV).xyzw * float4(40, 40, 100, 20) - float4(20, 20, 0, 10);
#else
               // if (isOutArea) data.xyzw = 0;
                if (isOutArea || KW_FluidsRequiredReadPrebakedSim)
                {
                    data.xyzw = tex2D(KW_FluidsPrebaked, depthUV).xyzw * float4(40, 40, 100, 20) - float4(20, 20, 0, 10);
                    if (depthUV.x < 0.05 || depthUV.x > 0.95 || depthUV.y < 0.05 || depthUV.y > 0.95) data.xyzw = 0;
                }

 #endif

 #if KW_FLUIDS_PREBAKE_SIM
                float2 maskAlphaUV = 1 - abs(flowMapUV * 2 - 1); // 参照后面泡沫uv处的注释
                float maskAlpha = saturate((maskAlphaUV.x * maskAlphaUV.y - 0.001) * 20);
                data *= maskAlpha;
 #endif

                data = clamp(data, float4(-20, -20, 0.5, -10), float4(20, 20, 100, 10));

                FragmentOutput o;
                o.dest0 = data;
                // o.dest0.xy = depth < 0.0;

                 float3 texelOffset = float3(_MainTex_TexelSize.xy * _FoamTexelOffset, 0);

                 tr = tex2D(_MainTex, i.uv + texelOffset.xz);
                 tl = tex2D(_MainTex, i.uv - texelOffset.xz);
                 tu = tex2D(_MainTex, i.uv + texelOffset.zy);
                 td = tex2D(_MainTex, i.uv - texelOffset.zy);
                 float divergence = saturate(max(length(tr.xy - tl.xy), length(tu.xy - td.xy)) - 0.05); // 散度

                 i.uv = 1 - abs(i.uv * 2 - 1); // 将原来左下角(0,0)右上角为(1,1)的uv幅值分布改为了四角(0,0)中心(1,1)
                 float lerpLod = saturate((i.uv.x * i.uv.y - 0.01) * 3); // 放大中心的泡沫强度, 减小边缘的泡沫强度

                 o.dest1 = divergence * lerpLod * 2;

                 return o;
              }
              ENDHLSL
          }

          Pass //remap prebake data to 0-1 range
          {
              HLSLPROGRAM


              #pragma vertex vert
              #pragma fragment frag

              #include "UnityCG.cginc"

              #include "../KWS_WaterVariables.cginc"

              sampler2D _MainTex;

              struct appdata
              {
                  float4 vertex : POSITION;
                  float2 uv : TEXCOORD0;
              };

              struct v2f
              {
                  float2 uv : TEXCOORD0;
                  float4 vertex : SV_POSITION;
              };

              v2f vert(appdata v)
              {
                  v2f o;
                  o.vertex = float4(v.vertex.xy - 0.5, 0, 0.5);
                  o.uv = float2(v.uv.x, 1 - v.uv.y);
                  return o;
              }

              float4  frag(v2f i) : SV_Target
              {
                  // data = clamp(data, float4(-20, -20, 0.5, -10), float4(20, 20, 100, 10));
                 float4 data = (tex2D(_MainTex, i.uv) + float4(20, 20, 0, 10)) / float4(40.0, 40.0, 100.0, 20);

                 return data;
              }
                ENDHLSL
          }
    }
}
