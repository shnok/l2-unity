////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//
// Auto-generated shader code, don't hand edit!
//
//   Unity Version: 2022.2.1f1
//   MicroSplat Version: 3.9
//   Render Pipeline: URP2022
//   Platform: OSXEditor
////////////////////////////////////////


Shader "MicroSplat"
{
   Properties
   {
      [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
      [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
      [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
            [HideInInspector] _Control0 ("Control0", 2D) = "red" {}
      

      // Splats
      [NoScaleOffset]_Diffuse ("Diffuse Array", 2DArray) = "white" {}
      [NoScaleOffset]_NormalSAO ("Normal Array", 2DArray) = "bump" {}
      [NoScaleOffset]_PerTexProps("Per Texture Properties", 2D) = "black" {}
      [HideInInspector] _TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" {}
      [HideInInspector] _PerPixelNormal("Per Pixel Normal", 2D) = "bump" {}
      _Contrast("Blend Contrast", Range(0.01, 0.99)) = 0.4
      _UVScale("UV Scales", Vector) = (45, 45, 0, 0)

      // for Unity 2020.3 bug
      _MainTex("Unity Bug", 2D) = "white" {}


   }
   SubShader
   {
            Tags {"RenderPipeline" = "UniversalPipeline"  "RenderType" = "UniversalLitShader" "Queue" = "Geometry+100" "IgnoreProjector" = "False"  "TerrainCompatible" = "true" "SplatCount" = "4"}
      

      
        Pass
        {
            Name "Universal Forward"
            Tags 
            { 
                "LightMode" = "UniversalForward"
            }
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            

            

            HLSLPROGRAM

               #pragma vertex Vert
   #pragma fragment Frag

            #pragma target 3.5

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma instancing_options norenderinglayer assumeuniformscaling nomatrices nolightprobe nolightmap
    
            // Keywords
            #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ _LIGHT_COOKIES
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            #pragma multi_compile _ _FORWARD_PLUS
            
            // GraphKeywords: <None>

            #define SHADER_PASS SHADERPASS_FORWARD
            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define _FOG_FRAGMENT 1
            #define _PASSFORWARD 1

            #if _SIMPLELIT
               #define _SPECULAR_COLOR
            #endif


            
      #define _MICROSPLAT 1
      #define _MICROTERRAIN 1
      #define _USEGRADMIP 1
      #define _MAX4TEXTURES 1
      #define _BRANCHSAMPLES 1
      #define _BRANCHSAMPLESAGR 1
      #define _MSRENDERLOOP_UNITYURP2022 1

#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


   #define _URP 1



            // this has to be here or specular color will be ignored. Not in SG code
            #if _SIMPLELIT
               #define _SPECULAR_COLOR
            #endif


            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #if VERSION_GREATER_EQUAL(12, 0)
               #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #endif
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            

        

               #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, float3x3(d.worldSpaceTangent, cross(d.worldSpaceTangent, d.worldSpaceNormal), d.worldSpaceNormal))

      
      #define UnityObjectToWorldNormal(normal) mul(GetObjectToWorldMatrix(), normal)

      #define _WorldSpaceLightPos0 _MainLightPosition
      
      #define UNITY_DECLARE_TEX2D(name) TEXTURE2D(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2D_NOSAMPLER(name) TEXTURE2D(name);
      #define UNITY_DECLARE_TEX2DARRAY(name) TEXTURE2D_ARRAY(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(name) TEXTURE2D_ARRAY(name);

      #define UNITY_SAMPLE_TEX2DARRAY(tex,coord)            SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, coord.xy, coord.z)
      #define UNITY_SAMPLE_TEX2DARRAY_LOD(tex,coord,lod)    SAMPLE_TEXTURE2D_ARRAY_LOD(tex, sampler##tex, coord.xy, coord.z, lod)
      #define UNITY_SAMPLE_TEX2D(tex, coord)                SAMPLE_TEXTURE2D(tex, sampler##tex, coord)
      #define UNITY_SAMPLE_TEX2D_SAMPLER(tex, samp, coord)  SAMPLE_TEXTURE2D(tex, sampler##samp, coord)

      #if defined(UNITY_COMPILER_HLSL)
         #define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
      #else
         #define UNITY_INITIALIZE_OUTPUT(type,name)
      #endif

      #define sampler2D_float sampler2D
      #define sampler2D_half sampler2D

      

      // data across stages, stripped like the above.
      struct VertexToPixel
      {
         float4 pos : SV_POSITION;
         float3 worldPos : TEXCOORD0;
         float3 worldNormal : TEXCOORD1;
         float4 worldTangent : TEXCOORD2;
          float4 texcoord0 : TEXCCOORD3;
         #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
             float4 texcoord1 : TEXCCOORD4;
            // float4 texcoord2 : TEXCCOORD5;
         #endif
         // float4 texcoord3 : TEXCCOORD6;
         // float4 screenPos : TEXCOORD7;
         // float4 vertexColor : COLOR;

         // float4 extraV2F0 : TEXCOORD13;
         // float4 extraV2F1 : TEXCOORD14;
         // float4 extraV2F2 : TEXCOORD15;
         // float4 extraV2F3 : TEXCOORD16;
         // float4 extraV2F4 : TEXCOORD17;
         // float4 extraV2F5 : TEXCOORD18;
         // float4 extraV2F6 : TEXCOORD19;
         // float4 extraV2F7 : TEXCOORD20;

         
         #if defined(LIGHTMAP_ON)
            float2 lightmapUV : TEXCOORD8;
         #endif
         #if defined(DYNAMICLIGHTMAP_ON)
            float2 dynamicLightmapUV : TEXCOORD9;
         #endif
         #if !defined(LIGHTMAP_ON)
            float3 sh : TEXCOORD10;
         #endif

         float4 fogFactorAndVertexLight : TEXCOORD11;
         float4 shadowCoord : TEXCOORD12;

         #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
         #endif
         #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
         #endif
         #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
         #endif
         #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
         #endif
      };


         // TEMPLATE_SHARED
            
            // data describing the user output of a pixel
            struct Surface
            {
               half3 Albedo;
               half Height;
               half3 Normal;
               half Smoothness;
               half3 Emission;
               half Metallic;
               half3 Specular;
               half Occlusion;
               half Alpha;
               // HDRP Only
               half SpecularOcclusion;
               half SubsurfaceMask;
               half Thickness;
               half CoatMask;
               half Anisotropy;
               half IridescenceMask;
               half IridescenceThickness;
            };

            // data the user might need, this will grow to be big. But easy to strip
            struct ShaderData
            {
               float3 localSpacePosition;
               float3 localSpaceNormal;
               float3 localSpaceTangent;
        
               float3 worldSpacePosition;
               float3 worldSpaceNormal;
               float3 worldSpaceTangent;

               float3 worldSpaceViewDir;
               float3 tangentSpaceViewDir;

               float4 texcoord0;
               float4 texcoord1;
               float4 texcoord2;
               float4 texcoord3;

               float2 screenUV;
               float4 screenPos;

               float4 vertexColor;

               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;

               float3x3 TBNMatrix;
            };

            struct VertexData
            {
               #if SHADER_TARGET > 30 && _PLANETCOMPUTE
 //              // uint vertexID : SV_VertexID;
               #endif
               float4 vertex : POSITION;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                  float4 tangent : TANGENT;
                  float4 texcoord1 : TEXCOORD1;
                  float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD4; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity    : TEXCOORD5; // Add Precomputed Velocity (Alembic computes velocities on runtime side).
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessVertex 
            {
               float4 vertex : INTERNALTESSPOS;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float4 tangent : TANGENT;
               float4 texcoord1 : TEXCOORD1;
               float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               // float4 extraV2F0 : TEXCOORD4;
               // float4 extraV2F1 : TEXCOORD5;
               // float4 extraV2F2 : TEXCOORD6;
               // float4 extraV2F3 : TEXCOORD7;
               // float4 extraV2F4 : TEXCOORD8;
               // float4 extraV2F5 : TEXCOORD9;
               // float4 extraV2F6 : TEXCOORD10;
               // float4 extraV2F7 : TEXCOORD11;

               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD12; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity : TEXCOORD13;
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
               UNITY_VERTEX_OUTPUT_STEREO
            };

            struct ExtraV2F
            {
               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;
            };


            float3 WorldToTangentSpace(ShaderData d, float3 normal)
            {
               return mul(d.TBNMatrix, normal);
            }

            float3 TangentToWorldSpace(ShaderData d, float3 normal)
            {
               return mul(normal, d.TBNMatrix);
            }

            // in this case, make standard more like SRPs, because we can't fix
            // GetWorldToObjectMatrix() in HDRP, since it already does macro-fu there

            #if _STANDARD
               float3 TransformWorldToObject(float3 p) { return mul(GetWorldToObjectMatrix(), float4(p, 1)); };
               float3 TransformObjectToWorld(float3 p) { return mul(GetObjectToWorldMatrix(), float4(p, 1)); };
               float4 TransformWorldToObject(float4 p) { return mul(GetWorldToObjectMatrix(), p); };
               float4 TransformObjectToWorld(float4 p) { return mul(GetObjectToWorldMatrix(), p); };
               float4x4 GetWorldToObjectMatrix() { return GetWorldToObjectMatrix(); }
               float4x4 GetObjectToWorldMatrix() { return GetObjectToWorldMatrix(); }
            #endif

            float3 GetCameraWorldPosition()
            {
               #if _HDRP
                  return GetCameraRelativePositionWS(_WorldSpaceCameraPos);
               #else
                  return _WorldSpaceCameraPos;
               #endif
            }

            
             #if _HDRP

               half3 UnpackNormalmapRGorAG(half4 packednormal)
               {
                     // This do the trick
                  packednormal.x *= packednormal.w;

                  half3 normal;
                  normal.xy = packednormal.xy * 2 - 1;
                  normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                  return normal;
               }
               half3 UnpackNormal(half4 packednormal)
               {
                  #if defined(UNITY_NO_DXT5nm)
                     return packednormal.xyz * 2 - 1;
                  #else
                     return UnpackNormalmapRGorAG(packednormal);
                  #endif
               }
               #endif
               #if _HDRP || _URP

               half3 UnpackScaleNormal(half4 packednormal, half scale)
               {
                 #ifndef UNITY_NO_DXT5nm
                   // Unpack normal as DXT5nm (1, y, 1, x) or BC5 (x, y, 0, 1)
                   // Note neutral texture like "bump" is (0, 0, 1, 1) to work with both plain RGB normal and DXT5nm/BC5
                   packednormal.x *= packednormal.w;
                 #endif
                   half3 normal;
                   normal.xy = (packednormal.xy * 2 - 1) * scale;
                   normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                   return normal;
               }  

             #endif


            void GetSun(out float3 lightDir, out float3 color)
            {
               lightDir = float3(0.5, 0.5, 0);
               color = 1;
               #if _HDRP
                  if (_DirectionalLightCount > 0)
                  {
                     DirectionalLightData light = _DirectionalLightDatas[0];
                     lightDir = -light.forward.xyz;
                     color = light.color;
                  }
               #elif _STANDARD
                  lightDir = normalize(_WorldSpaceLightPos0.xyz);
                  color = _LightColor0.rgb;
               #elif _URP
                  Light light = GetMainLight();
                  lightDir = light.direction;
                  color = light.color;
               #endif
            }

     



            
         CBUFFER_START(UnityPerMaterial)

            

      #if _MESHSUBARRAY
         half4 _MeshSubArrayIndexes;
      #endif

      float4 _Diffuse_TexelSize;
      float4 _NormalSAO_TexelSize;

      #if _HYBRIDHEIGHTBLEND
         float _HybridHeightBlendDistance;
      #endif

      #if _PACKINGHQ
         float4 _SmoothAO_TexelSize;
      #endif

      #ifdef _ALPHATEST_ON
      float4 _TerrainHolesTexture_TexelSize;
      #endif

      #if _USESPECULARWORKFLOW
         float4 _Specular_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         float4 _EmissiveMetal_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         half _EmissiveMult;
      #endif

      #if _AUTONORMAL
         half _AutoNormalHeightScale;
      #endif

      float4 _UVScale; // scale and offset

      half _Contrast;
      
      

       #if _VSSHADOWMAP
         float4 gVSSunDirection;
      #endif

      #if _FORCELOCALSPACE && _PLANETVECTORS
         float4x4 _PQSToLocal;
      #endif

      #if _ORIGINSHIFT
         float4x4 _GlobalOriginMTX;
      #endif

      float4 _Control0_TexelSize;
      #if _CUSTOMSPLATTEXTURES
         float4 _CustomControl0_TexelSize;
      #endif
      float4 _PerPixelNormal_TexelSize;

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         float2 _NoiseUVParams;
      #endif

      float4 _PerTexProps_TexelSize;

      #if _SURFACENORMALS  
         float3 surfTangent;
         float3 surfBitangent;
         float3 surfNormal;
      #endif



         CBUFFER_END

         

         

               

      #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, data.TBN)
      

      



   
// In Unity 2020.3LTS, Unity will spew tons of errors about missing this sampler in
// URP, even though it shouldn't be required.
TEXTURE2D(_MainTex);

      // globals, outside of CBuffer, but used by more than one module
      float3 _gGlitterLightDir;
      float3 _gGlitterLightWorldPos;
      half3 _gGlitterLightColor;

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
         float4    _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
         float4    _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
         float4    _TerrainNormalmapTexture_TexelSize;
      #endif

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
          TEXTURE2D(_TerrainHeightmapTexture);
          TEXTURE2D(_TerrainNormalmapTexture);
      #endif

      UNITY_INSTANCING_BUFFER_START(Terrain)
          UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
      UNITY_INSTANCING_BUFFER_END(Terrain)          


      

      // dynamic branching helpers, for regular and aggressive branching
      // debug mode shows how many samples using branching will save us. 
      //
      // These macros are always used instead of the UNITY_BRANCH macro
      // to maintain debug displays and allow branching to be disabled
      // on as granular level as we want. 
      
      #if _BRANCHSAMPLES
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++; if (w > 0)
         #else
            #define MSBRANCH(w) UNITY_BRANCH if (w > 0)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++;
         #else
            #define MSBRANCH(w) 
         #endif
      #endif
      
      #if _BRANCHSAMPLESAGR
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER ||_DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++; if (w > 0.001)
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++; if (w > 0.001)
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++; if (w > 0.001)
         #else
            #define MSBRANCHTRIPLANAR(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHCLUSTER(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHOTHER(w) UNITY_BRANCH if (w > 0.001)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++;
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++;
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++;
         #else
            #define MSBRANCHTRIPLANAR(w)
            #define MSBRANCHCLUSTER(w)
            #define MSBRANCHOTHER(w)
         #endif
      #endif

      #if _DEBUG_SAMPLECOUNT
         int _sampleCount;
         #define COUNTSAMPLE { _sampleCount++; }
      #else
         #define COUNTSAMPLE
      #endif

      #if _DEBUG_PROCLAYERS
         int _procLayerCount;
         #define COUNTPROCLAYER { _procLayerCount++; }
      #else
         #define COUNTPROCLAYER
      #endif


      #if _DEBUG_USE_TOPOLOGY
         TEXTURE2D(_DebugWorldPos);
         TEXTURE2D(_DebugWorldNormal);
      #endif
      

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         TEXTURE2D(_NoiseUV);
      #endif

      #if _PACKINGHQ
         UNITY_DECLARE_TEX2DARRAY(_SmoothAO);
      #endif

      #if _USESPECULARWORKFLOW
         UNITY_DECLARE_TEX2DARRAY(_Specular);
      #endif

      #if _USEEMISSIVEMETAL
         UNITY_DECLARE_TEX2DARRAY(_EmissiveMetal);
      #endif

      TEXTURE2D(_PerPixelNormal);

      SamplerState shared_linear_clamp_sampler;
      SamplerState shared_point_clamp_sampler;
      
      TEXTURE2D(_Control0);
      #if _CUSTOMSPLATTEXTURES
         TEXTURE2D(_CustomControl0);
         #if !_MAX4TEXTURES
         TEXTURE2D(_CustomControl1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_CustomControl2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_CustomControl3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_CustomControl7);
         #endif
      #else
         #if !_MAX4TEXTURES
         TEXTURE2D(_Control1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_Control2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_Control3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_Control7);
         #endif
      #endif

      TEXTURE2D_FLOAT(_PerTexProps);
   
      struct DecalLayer
      {
         float3 uv;
         float2 dx;
         float2 dy;
         int decalIndex;
         bool dynamic; 
      };

      struct DecalOutput
      {
         DecalLayer l0;
         DecalLayer l1;
         DecalLayer l2;
         DecalLayer l3;
         
         half4 Weights;
         half4 Indexes;
         half4 fxLevels;
         
      };
      

      struct TriGradMipFormat
      {
         float4 d0;
         float4 d1;
         float4 d2;
      };

      float InverseLerp(float x, float y, float v) { return (v-x)/max(y-x, 0.001); }
      float2 InverseLerp(float2 x, float2 y, float2 v) { return (v-x)/max(y-x, float2(0.001, 0.001)); }
      float3 InverseLerp(float3 x, float3 y, float3 v) { return (v-x)/max(y-x, float3(0.001, 0.001, 0.001)); }
      float4 InverseLerp(float4 x, float4 y, float4 v) { return (v-x)/max(y-x, float4(0.001, 0.001, 0.001, 0.001)); }
      

      // 2019.3 holes
      #ifdef _ALPHATEST_ON
          TEXTURE2D(_TerrainHolesTexture);
          
          void ClipHoles(float2 uv)
          {
              float hole = SAMPLE_TEXTURE2D(_TerrainHolesTexture, shared_linear_clamp_sampler, uv).r;
              COUNTSAMPLE
              clip(hole < 0.5f ? -1 : 1);
          }
      #endif

      
      #if _TRIPLANAR
         #if _USEGRADMIP
            #define MIPFORMAT TriGradMipFormat
            #define INITMIPFORMAT (TriGradMipFormat)0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float3
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float3
         #endif
      #else
         #if _USEGRADMIP
            #define MIPFORMAT float4
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float
         #endif
      #endif

      float2 TotalOne(float2 v) { return v * (1.0 / max(v.x + v.y, 0.001)); }
      float3 TotalOne(float3 v) { return v * (1.0 / max(v.x + v.y + v.z, 0.001)); }
      float4 TotalOne(float4 v) { return v * (1.0 / max(v.x + v.y + v.z + v.w, 0.001)); }

      float2 RotateUV(float2 uv, float amt)
      {
         uv -=0.5;
         float s = sin ( amt);
         float c = cos ( amt );
         float2x2 mtx = float2x2( c, -s, s, c);
         mtx *= 0.5;
         mtx += 0.5;
         mtx = mtx * 2-1;
         uv = mul ( uv, mtx );
         uv += 0.5;
         return uv;
      }

      float4 DecodeToFloat4(float v)
      {
         uint vi = (uint)(v * (256.0f * 256.0f * 256.0f * 256.0f));
         int ex = (int)(vi / (256 * 256 * 256) % 256);
         int ey = (int)((vi / (256 * 256)) % 256);
         int ez = (int)((vi / (256)) % 256);
         int ew = (int)(vi % 256);
         float4 e = float4(ex / 255.0, ey / 255.0, ez / 255.0, ew / 255.0);
         return e;
      }

      

      struct Input 
      {
         ShaderData shaderData;
         float2 uv_Control0;
         float2 uv2_Diffuse;

         float worldHeight;
         float3 worldUpVector;

         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         float4 color;
         float3x3 TBN;

         // vertex/digger workflow data
         half4 w0;
         half4 w1;
         half4 w2;
         half4 w3;
         half4 w4;
         half4 w5;
         half4 w6;
         
         // megasplat data
         half4 layer0;
         half4 layer1;
         half3 baryWeights;
         half4 scatter0;
         half4 scatter1;

         // wetness, puddles, streams, lava from vertex or megasplat
         half4 fx;
         // snow min, snow max
         half4 fx2;


      };
      
      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
         half3 pN0;
         half3 pN1;
         half3 pN2;
         half3 pN3;
         half3 axisSign;
         Input IN;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;

         half4 cluster0;
         half4 cluster1;
         half4 cluster2;
         half4 cluster3;

      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
         #if _USESPECULARWORKFLOW
         half3 Specular;
         #endif
         half Alpha;
         
      };


      

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         #if _SURFACENORMALS
            half3 surf0;
            half3 surf1;
            half3 surf2;
            half3 surf3;
         #endif

         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
         

         #if _USEEMISSIVEMETAL || _GLOBALEMIS || _GLOBALSMOOTHAOMETAL || _PERTEXSSS || _PERTEXRIMLIGHT
            half4 emisMetal0;
            half4 emisMetal1;
            half4 emisMetal2;
            half4 emisMetal3;
         #endif

         #if _USESPECULARWORKFLOW
            half3 specular0;
            half3 specular1;
            half3 specular2;
            half3 specular3;
         #endif
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
         #if _SURFACENORMALS
            s.surf0 = half3(0,0,1);
            s.surf1 = half3(0,0,1);
            s.surf2 = half3(0,0,1);
            s.surf3 = half3(0,0,1);
         #endif
      }

       float3 GetGlobalLightDir(Input i)
      {
         float3 lightDir = float3(1,0,0);

         #if _HDRP || PASS_DEFERRED
            lightDir = normalize(_gGlitterLightDir.xyz);
         #elif _URP
            lightDir = GetMainLight().direction;
         #else
            #ifndef USING_DIRECTIONAL_LIGHT
               lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            #else
               lightDir = normalize(_WorldSpaceLightPos0.xyz);
            #endif
         #endif
         return lightDir;
      }

      float3x3 GetTBN(Input i)
      {
         return i.TBN;
      }
      
      float3 GetGlobalLightDirTS(Input i)
      {
         float3 lightDirWS = GetGlobalLightDir(i);
         return mul(GetTBN(i), lightDirWS);
      }
      
      half3 GetGlobalLightColor()
      {
         #if _HDRP || PASS_DEFERRED
            return _gGlitterLightColor;
         #elif _URP
            return (GetMainLight().color);
         #else
            return _LightColor0.rgb;
         #endif
      }

      

      half3 FuzzyShade(half3 color, half3 normal, half coreMult, half edgeMult, half power, float3 viewDir)
      {
         half dt = saturate(dot(viewDir, normal));
         half dark = 1.0 - (coreMult * dt);
         half edge = pow(1-dt, power) * edgeMult;
         return color * (dark + edge);
      }

      half3 ComputeSSS(Input i, float3 V, float3 N, half3 tint, half thickness, half distortion, half scale, half power)
      {
         float3 L = GetGlobalLightDir(i);
         half3 lightColor = GetGlobalLightColor();
         float3 H = normalize(L + N * distortion);
         float VdotH = pow(saturate(dot(V, -H)), power) * scale;
         float3 I =  (VdotH) * thickness;
         return lightColor * I * tint;
      }


      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif
      

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##3 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv3.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #endif

      half2 BlendNormal2(half2 base, half2 blend) { return normalize(half3(base.xy + blend.xy, 1)).xy; } 
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }
      half3 BlendLighterColor(half3 s, half3 d) { return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d; } 
      
      
      #if _SURFACENORMALS  

      #define HALF_EPS 4.8828125e-4    // 2^-11, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)

      

      void ConstructSurfaceGradientTBN(Input i)
      {
         float3x3 tbn = GetTBN(i);
         float3 t = tbn[0];
         float3 b = tbn[1];
         float3 n = tbn[2];

         surfNormal = n;//mul(GetWorldToObjectMatrix(), float4(n, 1)).xyz;
         surfTangent = t;//mul(GetWorldToObjectMatrix(), float4(t, 1)).xyz;
         surfBitangent = b;//cross(surfNormal, surfTangent);
         
         float renormFactor = 1.0 / length(surfNormal);
         surfNormal    *= renormFactor;
         surfTangent   *= renormFactor;
         surfBitangent *= renormFactor;
      }
      
      half3 SurfaceGradientFromTBN(half2 deriv)
      {
          return deriv.x * surfTangent + deriv.y * surfBitangent;
      }

      // Input: vM is tangent space normal in [-1;1].
      // Output: convert vM to a derivative.
      half2 TspaceNormalToDerivative(half3 vM)
      {
         const half scale = 1.0/128.0;
         
         // Ensure vM delivers a positive third component using abs() and
         // constrain vM.z so the range of the derivative is [-128; 128].
         const half3 vMa = abs(vM);
         const half z_ma = max(vMa.z, scale*max(vMa.x, vMa.y));

         return -half2(vM.x, vM.y)/z_ma;
      }

      // Used to produce a surface gradient from the gradient of a volume
      // bump function such as 3D Perlin noise. Equation 2 in [Mik10].
      half3 SurfgradFromVolumeGradient(half3 grad)
      {
         return grad - dot(surfNormal, grad) * surfNormal;
      }

      half3 SurfgradFromTriplanarProjection(half3 pN, half2 xPlaneTN, half2 yPlaneTN, half2 zPlaneTN)
      {
         const half w0 = pN.x;
         const half w1 = pN.y;
         const half w2 = pN.z;
         
         // X-plane tangent normal to gradient derivative
         xPlaneTN = xPlaneTN * 2.0 - 1.0;
         half xPlaneRcpZ = rsqrt(max(1 - dot(xPlaneTN.x, xPlaneTN.x) - dot(xPlaneTN.y, xPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_xplane = xPlaneTN * -xPlaneRcpZ;

         // Y-plane tangent normal to gradient derivative
         yPlaneTN = yPlaneTN * 2.0 - 1.0;
         half yPlaneRcpZ = rsqrt(max(1 - dot(yPlaneTN.x, yPlaneTN.x) - dot(yPlaneTN.y, yPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_yplane = yPlaneTN * -yPlaneRcpZ;

         // Z-plane tangent normal to gradient derivative
         zPlaneTN = zPlaneTN * 2.0 - 1.0;
         half zPlaneRcpZ = rsqrt(max(1 - dot(zPlaneTN.x, zPlaneTN.x) - dot(zPlaneTN.y, zPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_zplane = zPlaneTN * -zPlaneRcpZ;

         // Assume deriv xplane, deriv yplane, and deriv zplane are
         // sampled using (z,y), (x,z), and (x,y), respectively.
         // Positive scales of the lookup coordinate will work
         // as well, but for negative scales the derivative components
         // will need to be negated accordingly.
         float3 grad = float3(w2*d_zplane.x + w1*d_yplane.x,
                              w2*d_zplane.y + w0*d_xplane.y,
                              w0*d_xplane.x + w1*d_yplane.y);

         return SurfgradFromVolumeGradient(grad);
      }

      half3 ConvertNormalToGradient(half3 normal)
      {
         half2 deriv = TspaceNormalToDerivative(normal);

         return SurfaceGradientFromTBN(deriv);
      }

      half3 ConvertNormal2ToGradient(half2 packedNormal)
      {
         half2 tNormal = packedNormal;
         half rcpZ = rsqrt(max(1 - dot(tNormal.x, tNormal.x) - dot(tNormal.y, tNormal.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
         half2 deriv = tNormal * -rcpZ;
         return SurfaceGradientFromTBN(deriv);
      }


      half3 ResolveNormalFromSurfaceGradient(half3 gradient)
      {
         return normalize(surfNormal - gradient);
      }
      

      #endif // _SURFACENORMALS

      void BlendNormalPerTex(inout RawSamples o, half2 noise, float4 fades)
      {
         #if _SURFACENORMALS
            float3 grad = ConvertNormal2ToGradient(noise.xy);
            o.surf0 += grad * fades.x;
            o.surf1 += grad * fades.y;
            #if !_MAX2LAYER
               o.surf2 += grad * fades.z;
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.surf3 += grad * fades.w;
            #endif
         #else
            o.normSAO0.xy = lerp(o.normSAO0.xy, BlendNormal2(o.normSAO0.xy, noise.xy), fades.x);
            o.normSAO1.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #if !_MAX2LAYER
               o.normSAO2.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO2.xy, noise.xy), fades.y);
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.normSAO3.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #endif
         #endif
      }
      
     
      
      half3 BlendNormal3(half3 n1, half3 n2)
      {
         n1 += float3( 0,  0, 1);
         n2 *= float3(-1, -1, 1);
         return n1*dot(n1, n2) / n1.z - n2;
      }
      
      half2 TransformTriplanarNormal(Input IN, float3x3 t2w, half3 axisSign, half3 absVertNormal,
               half3 pN, half2 a0, half2 a1, half2 a2)
      {
         
         a0 = a0 * 2 - 1;
         a1 = a1 * 2 - 1;
         a2 = a2 * 2 - 1;

         a0.x *= axisSign.x;
         a1.x *= axisSign.y;
         a2.x *= axisSign.z;
         
         half3 n0 = half3(a0.xy, 1);
         half3 n1 = half3(a1.xy, 1);
         half3 n2 = half3(a2.xy, 1);

         float3 wn = IN.worldNormal;

         n0 = BlendNormal3(half3(wn.zy, absVertNormal.x), n0);
         n1 = BlendNormal3(half3(wn.xz, absVertNormal.y), n1 * float3(-1, 1, 1)); 
         n2 = BlendNormal3(half3(wn.xy, absVertNormal.z), n2);
  
         n0.z *= axisSign.x;
         n1.z *= axisSign.y;
         n2.z *= -axisSign.z;

         half3 worldNormal = (n0.zyx * pN.x + n1.xzy * pN.y + n2.xyz * pN.z);
         return mul(t2w, worldNormal).xy;

      }
      
      // funcs
      
      inline half MSLuminance(half3 rgb)
      {
         #ifdef UNITY_COLORSPACE_GAMMA
            return dot(rgb, half3(0.22, 0.707, 0.071));
         #else
            return dot(rgb, half3(0.0396819152, 0.458021790, 0.00609653955));
         #endif
      }
      
      
      float2 Hash2D( float2 x )
      {
          float2 k = float2( 0.3183099, 0.3678794 );
          x = x*k + k.yx;
          return -1.0 + 2.0*frac( 16.0 * k*frac( x.x*x.y*(x.x+x.y)) );
      }

      float Noise2D(float2 p )
      {
         float2 i = floor( p );
         float2 f = frac( p );
         
         float2 u = f*f*(3.0-2.0*f);

         return lerp( lerp( dot( Hash2D( i + float2(0.0,0.0) ), f - float2(0.0,0.0) ), 
                           dot( Hash2D( i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                      lerp( dot( Hash2D( i + float2(0.0,1.0) ), f - float2(0.0,1.0) ), 
                           dot( Hash2D( i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
      }
      
      float FBM2D(float2 uv)
      {
         float f = 0.5000*Noise2D( uv ); uv *= 2.01;
         f += 0.2500*Noise2D( uv ); uv *= 1.96;
         f += 0.1250*Noise2D( uv ); 
         return f;
      }
      
      float3 Hash3D( float3 p )
      {
         p = float3( dot(p,float3(127.1,311.7, 74.7)),
                 dot(p,float3(269.5,183.3,246.1)),
                 dot(p,float3(113.5,271.9,124.6)));

         return -1.0 + 2.0*frac(sin(p)*437.5453123);
      }

      float Noise3D( float3 p )
      {
         float3 i = floor( p );
         float3 f = frac( p );
         
         float3 u = f*f*(3.0-2.0*f);

         return lerp( lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,0.0) ), f - float3(0.0,0.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,0.0) ), f - float3(1.0,0.0,0.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,0.0) ), f - float3(0.0,1.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,0.0) ), f - float3(1.0,1.0,0.0) ), u.x), u.y),
                      lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,1.0) ), f - float3(0.0,0.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,1.0) ), f - float3(1.0,0.0,1.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,1.0) ), f - float3(0.0,1.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,1.0) ), f - float3(1.0,1.0,1.0) ), u.x), u.y), u.z );
      }
      
      float FBM3D(float3 uv)
      {
         float f = 0.5000*Noise3D( uv ); uv *= 2.01;
         f += 0.2500*Noise3D( uv ); uv *= 1.96;
         f += 0.1250*Noise3D( uv ); 
         return f;
      }
      
     
      
      float GetSaturation(float3 c)
      {
         float mi = min(min(c.x, c.y), c.z);
         float ma = max(max(c.x, c.y), c.z);
         return (ma - mi)/(ma + 1e-7);
      }

      // Better Color Lerp, does not have darkening issue
      float3 BetterColorLerp(float3 a, float3 b, float x)
      {
         float3 ic = lerp(a, b, x) + float3(1e-6,0.0,0.0);
         float sd = abs(GetSaturation(ic) - lerp(GetSaturation(a), GetSaturation(b), x));
    
         float3 dir = normalize(float3(2.0 * ic.x - ic.y - ic.z, 2.0 * ic.y - ic.x - ic.z, 2.0 * ic.z - ic.y - ic.x));
         float lgt = dot(float3(1.0, 1.0, 1.0), ic);
    
         float ff = dot(dir, normalize(ic));
    
         const float dsp_str = 1.5;
         ic += dsp_str * dir * sd * ff * lgt;
         return saturate(ic);
      }
      
      
      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);
             half4 weights = half4(iWeights.x * max(h0,0.001), iWeights.y * max(h1,0.001), iWeights.z * max(h2,0.001), iWeights.w * max(h3,0.001));
             
             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);

             weights = TotalOne(weights);
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #elif _MAX20TEXTURES
         #define TEXCOUNT 20
      #elif _MAX24TEXTURES
         #define TEXCOUNT 24
      #elif _MAX28TEXTURES
         #define TEXCOUNT 28
      #elif _MAX32TEXTURES
         #define TEXCOUNT 32
      #else
         #define TEXCOUNT 16
      #endif

      #if _DECAL_SPLAT
      
      void DoMergeDecalSplats(half4 iWeights, half4 iIndexes, inout half4 indexes, inout half4 weights)
      {
         for (int i = 0; i < 4; ++i)
         {
            half w = iWeights[i];
            half index = iIndexes[i];
            if (w > weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = index;
            }
            else if (w > weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = index;
            }
            else if (w > weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = index;
            }
            else if (w > weights[3])
            {
               weights[3] = w;
               indexes[3] = index;
            }
         }

      }
      #endif


      void Setup(out half4 weights, float2 uv, out Config config, half4 w0, half4 w1, half4 w2, half4 w3, half4 w4, half4 w5, half4 w6, half4 w7, float3 worldPos, DecalOutput decalOutput)
      {
         config = (Config)0;
         half4 indexes = 0;

         config.uv = uv;

         #if _WORLDUV
         uv = worldPos.xz;
         #endif

         #if _DISABLESPLATMAPS
            float2 scaledUV = uv;
         #else
            float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;
         #endif

         // if only 4 textures, and blending 4 textures, skip this whole thing..
         // this saves about 25% of the ALU of the base shader on low end. However if
         // we rely on sorted texture weights (distance resampling) we have to sort..
         float4 defaultIndexes = float4(0,1,2,3);
         #if _MESHSUBARRAY
            defaultIndexes = _MeshSubArrayIndexes;
         #endif

         #if _MESHSUBARRAY && !_DECAL_SPLAT || (_MAX4TEXTURES && !_MAX3LAYER && !_MAX2LAYER && !_DISTANCERESAMPLE && !_POM && !_DECAL_SPLAT)
            weights = w0;
            config.uv0 = float3(scaledUV, defaultIndexes.x);
            config.uv1 = float3(scaledUV, defaultIndexes.y);
            config.uv2 = float3(scaledUV, defaultIndexes.z);
            config.uv3 = float3(scaledUV, defaultIndexes.w);
            return;
         #endif

         #if _DISABLESPLATMAPS
            weights = float4(1,0,0,0);
            return;
         #else
            half splats[TEXCOUNT];

            splats[0] = w0.x;
            splats[1] = w0.y;
            splats[2] = w0.z;
            splats[3] = w0.w;
            #if !_MAX4TEXTURES
               splats[4] = w1.x;
               splats[5] = w1.y;
               splats[6] = w1.z;
               splats[7] = w1.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES
               splats[8] = w2.x;
               splats[9] = w2.y;
               splats[10] = w2.z;
               splats[11] = w2.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
               splats[12] = w3.x;
               splats[13] = w3.y;
               splats[14] = w3.z;
               splats[15] = w3.w;
            #endif
            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[16] = w4.x;
               splats[17] = w4.y;
               splats[18] = w4.z;
               splats[19] = w4.w;
            #endif
            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[20] = w5.x;
               splats[21] = w5.y;
               splats[22] = w5.z;
               splats[23] = w5.w;
            #endif
            #if _MAX28TEXTURES || _MAX32TEXTURES
               splats[24] = w6.x;
               splats[25] = w6.y;
               splats[26] = w6.z;
               splats[27] = w6.w;
            #endif
            #if _MAX32TEXTURES
               splats[28] = w7.x;
               splats[29] = w7.y;
               splats[30] = w7.z;
               splats[31] = w7.w;
            #endif



            weights[0] = 0;
            weights[1] = 0;
            weights[2] = 0;
            weights[3] = 0;
            indexes[0] = 0;
            indexes[1] = 0;
            indexes[2] = 0;
            indexes[3] = 0;

            int i = 0;
            for (i = 0; i < TEXCOUNT; ++i)
            {
               half w = splats[i];
               if (w >= weights[0])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = weights[0];
                  indexes[1] = indexes[0];
                  weights[0] = w;
                  indexes[0] = i;
               }
               else if (w >= weights[1])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = w;
                  indexes[1] = i;
               }
               else if (w >= weights[2])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = w;
                  indexes[2] = i;
               }
               else if (w >= weights[3])
               {
                  weights[3] = w;
                  indexes[3] = i;
               }
            }

            #if _DECAL_SPLAT
               DoMergeDecalSplats(decalOutput.Weights, decalOutput.Indexes, weights, indexes); 
            #endif


            
            
            // clamp and renormalize
            #if _MAX2LAYER
               weights.zw = 0;
               weights.xy = TotalOne(weights.xy);
            #elif _MAX3LAYER
               weights.w = 0;
               weights.xyz = TotalOne(weights.xyz);
            #elif !_DISABLEHEIGHTBLENDING || _NORMALIZEWEIGHTS // prevents black when painting, which the unity shader does not prevent.
               weights = normalize(weights);
            #endif
            

            config.uv0 = float3(scaledUV, indexes.x);
            config.uv1 = float3(scaledUV, indexes.y);
            config.uv2 = float3(scaledUV, indexes.z);
            config.uv3 = float3(scaledUV, indexes.w);


         #endif //_DISABLESPLATMAPS


      }

      float3 HeightToNormal(float height, float3 worldPos)
      {
         float3 dx = ddx(worldPos);
         float3 dy = ddy(worldPos);
         float3 crossX = cross(float3(0,1,0), dx);
         float3 crossY = cross(float3(0,1,0), dy);
         float3 d = abs(dot(crossY, dx));
         float3 n = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
         n.z *= -1;
         return normalize((d * float3(0,1,0)) - n).xzy;
      }
      
      float ComputeMipLevel(float2 uv, float2 textureSize)
      {
         uv *= textureSize;
         float2  dx_vtc        = ddx(uv);
         float2  dy_vtc        = ddy(uv);
         float delta_max_sqr   = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
         return 0.5 * log2(delta_max_sqr);
      }

      inline half2 UnpackNormal2(half4 packednormal)
      {
          return packednormal.wy * 2 - 1;
         
      }

      half3 TriplanarHBlend(half h0, half h1, half h2, half3 pN, half contrast)
      {
         half3 blend = pN / dot(pN, half3(1,1,1));
         float3 heights = float3(h0, h1, h2) + (blend * 3.0);
         half height_start = max(max(heights.x, heights.y), heights.z) - contrast;
         half3 h = max(heights - height_start.xxx, half3(0,0,0));
         blend = h / dot(h, half3(1,1,1));
         return blend;
      }
      

      void ClearAllButAlbedo(inout MicroSplatLayer o, half3 display)
      {
         o.Albedo = display.rgb;
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

      void ClearAllButAlbedo(inout MicroSplatLayer o, half display)
      {
         o.Albedo = half3(display, display, display);
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

     

      half MicroShadow(float3 lightDir, half3 normal, half ao, half strength)
      {
         half shadow = saturate(abs(dot(normal, lightDir)) + (ao * ao * 2.0) - 1.0);
         return 1 - ((1-shadow) * strength);
      }
      

      void DoDebugOutput(inout MicroSplatLayer l)
      {
         #if _DEBUG_OUTPUT_ALBEDO
            ClearAllButAlbedo(l, l.Albedo);
         #elif _DEBUG_OUTPUT_NORMAL
            // oh unit shader compiler normal stripping, how I hate you so..
            // must multiply by albedo to stop the normal from being white. Why, fuck knows?
            ClearAllButAlbedo(l, float3(l.Normal.xy * 0.5 + 0.5, l.Normal.z * saturate(l.Albedo.z+1)));
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            ClearAllButAlbedo(l, l.Smoothness.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_METAL
            ClearAllButAlbedo(l, l.Metallic.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_AO
            ClearAllButAlbedo(l, l.Occlusion.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_EMISSION
            ClearAllButAlbedo(l, l.Emission * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_HEIGHT
            ClearAllButAlbedo(l, l.Height.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_SPECULAR && _USESPECULARWORKFLOW
            ClearAllButAlbedo(l, l.Specular * saturate(l.Albedo.z+1));
         #elif _DEBUG_BRANCHCOUNT_WEIGHT
            ClearAllButAlbedo(l, _branchWeightCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TRIPLANAR
            ClearAllButAlbedo(l, _branchTriplanarCount / 24 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_CLUSTER
            ClearAllButAlbedo(l, _branchClusterCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_OTHER
            ClearAllButAlbedo(l, _branchOtherCount / 8 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TOTAL
            l.Albedo.r = _branchWeightCount / 12;
            l.Albedo.g = _branchTriplanarCount / 24;
            l.Albedo.b = _branchClusterCount / 12;
            ClearAllButAlbedo(l, (l.Albedo.r + l.Albedo.g + l.Albedo.b + (_branchOtherCount / 8)) / 4); 
         #elif _DEBUG_OUTPUT_MICROSHADOWS
            ClearAllButAlbedo(l,l.Albedo); 
         #elif _DEBUG_SAMPLECOUNT
            float sdisp = (float)_sampleCount / max(_SampleCountDiv, 1);
            half3 sdcolor = float3(sdisp, sdisp > 1 ? 1 : 0, 0);
            ClearAllButAlbedo(l, sdcolor * saturate(l.Albedo.z + 1));
         #elif _DEBUG_PROCLAYERS
            ClearAllButAlbedo(l, (float)_procLayerCount / (float)_PCLayerCount * saturate(l.Albedo.z + 1));
         #endif
      }


      // abstraction around sampler mode
      #if _USELODMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_LOD(tex, sampler##tex, u, l.x)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u, l.x)
      #elif _USEGRADMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_GRAD(tex, sampler##tex, u, l.xy, l.zw)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY_GRAD(tex, ss, u.xy, u.z, l.xy, l.zw)
      #else
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, u.xy, u.z)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u.xy, y.z)
      #endif


      #define MICROSPLAT_SAMPLE_DIFFUSE(u, cl, l) MICROSPLAT_SAMPLE(_Diffuse, u, l)
      #define MICROSPLAT_SAMPLE_EMIS(u, cl, l) MICROSPLAT_SAMPLE(_EmissiveMetal, u, l)
      #define MICROSPLAT_SAMPLE_DIFFUSE_LOD(u, cl, l) UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, u, l)
      

      #if _PACKINGHQ
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) half4(MICROSPLAT_SAMPLE(_NormalSAO, u, l).ga, MICROSPLAT_SAMPLE(_SmoothAO, u, l).ga).brag
      #else
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) MICROSPLAT_SAMPLE(_NormalSAO, u, l)
      #endif

      #if _USESPECULARWORKFLOW
         #define MICROSPLAT_SAMPLE_SPECULAR(u, cl, l) MICROSPLAT_SAMPLE(_Specular, u, l)
      #endif
      
      struct SimpleTriplanarConfig
      {
         float3 pn;
         float2 uv0;
         float2 uv1;
         float2 uv2;
      };
         
      void PrepSimpleTriplanarConfig(inout SimpleTriplanarConfig tc, float3 worldPos, float3 normal, float contrast)
      {
         tc.pn = pow(abs(normal), contrast);
         tc.pn = tc.pn / (tc.pn.x + tc.pn.y + tc.pn.z);
         
         half3 axisSign = sign(normal);

         tc.uv0 = worldPos.zy * axisSign.x;
         tc.uv1 = worldPos.xz * axisSign.y;
         tc.uv2 = worldPos.xy * axisSign.z;
      }
      
      #define SimpleTriplanarSample(tex, tc, scale) (SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv0 * scale) * tc.pn.x + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv1 * scale) * tc.pn.y + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv2 * scale) * tc.pn.z)
      #define SimpleTriplanarSampleLOD(tex, tc, scale, lod) (SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv0 * scale, lod) * tc.pn.x + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv1 * scale, lod) * tc.pn.y + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv2 * scale, lod) * tc.pn.z)
      #define SimpleTriplanarSampleGrad(tex, tc, scale) (SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv0 * scale, ddx(tc.uv0) * scale, ddy(tc.uv0) * scale) * tc.pn.x + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv1 * scale, ddx(tc.uv1) * scale, ddy(tc.uv1) * scale) * tc.pn.y + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv2 * scale, ddx(tc.uv2) * scale, ddy(tc.uv2) * scale) * tc.pn.z)
   
      
      inline half3 MicroSplatDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
      {
          specColor = lerp (half3(0,0,0), albedo, metallic);
          oneMinusReflectivity = (1-metallic);
          return albedo * oneMinusReflectivity;
      }







      Input DescToInput(ShaderData IN)
      {
        Input s = (Input)0;
        s.shaderData = IN;
        s.TBN = IN.TBNMatrix;
        s.worldNormal = IN.worldSpaceNormal;
        s.worldPos = IN.worldSpacePosition;
        s.viewDir = IN.tangentSpaceViewDir;
        s.uv_Control0 = IN.texcoord0.xy;

        s.worldUpVector = float3(0,1,0);
        s.worldHeight = IN.worldSpacePosition.y;
  
        #if _PLANETVECTORS
            float3 rwp = mul(_PQSToLocal, float4(IN.worldSpacePosition, 1));
            s.worldHeight = distance(rwp, float3(0,0,0));
            s.worldUpVector = normalize(rwp);
        #endif

        #if _MICROMESH && _MESHUV2
            s.uv2_Diffuse = IN.texcoord1.xy;
        #endif

        #if _MEGASPLAT
           UnpackMegaSplat(s, IN);
        #endif
   
        #if _MICROVERTEXMESH || _MICRODIGGERMESH
            UnpackVertexWorkflow(s, IN);
        #endif

        #if _PLANETVECTORS
           DoPlanetDataInputCopy(s, IN);
        #endif
        
        return s;
     }
     


      void SampleAlbedo(inout Config config, inout TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
         
            half4 contrasts = _Contrast.xxxx;
            #if _PERTEXTRIPLANARCONTRAST
               SAMPLE_PER_TEX(ptc, 9.5, config, half4(1,0.5,0,0));
               contrasts = half4(ptc0.y, ptc1.y, ptc2.y, ptc3.y);
            #endif


            #if _PERTEXTRIPLANAR
               SAMPLE_PER_TEX(pttri, 9.5, config, half4(0,0,0,0));
            #endif

            {
               // For per-texture triplanar, we modify the view based blending factor of the triplanar
               // such that you get a pure blend of either top down projection, or with the top down projection
               // removed and renormalized. This causes dynamic flow control optimizations to kick in and avoid
               // the extra texture samples while keeping the code simple. Yay..

               // We also only have to do this in the Albedo, because the pN values will be adjusted after the
               // albedo is sampled, causing future samples to use this data. 
              
               #if _PERTEXTRIPLANAR
                  if (pttri0.x > 0.66)
                  {
                     tc.pN0 = half3(0,1,0);
                  }
                  else if (pttri0.x > 0.33)
                  {
                     tc.pN0.y = 0;
                     tc.pN0.xz = TotalOne(tc.pN0.xz);
                  }
               #endif


               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[0], config.cluster0, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[1], config.cluster0, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[2], config.cluster0, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN0;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN0, contrasts.x);
                  tc.pN0 = bf;
               #endif

               s.albedo0 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            MSBRANCH(weights.y)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri1.x > 0.66)
                  {
                     tc.pN1 = half3(0,1,0);
                  }
                  else if (pttri1.x > 0.33)
                  {
                     tc.pN1.y = 0;
                     tc.pN1.xz = TotalOne(tc.pN1.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[0], config.cluster1, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[1], config.cluster1, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  COUNTSAMPLE
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[2], config.cluster1, d2);
               }
               half3 bf = tc.pN1;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN1, contrasts.x);
                  tc.pN1 = bf;
               #endif


               s.albedo1 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri2.x > 0.66)
                  {
                     tc.pN2 = half3(0,1,0);
                  }
                  else if (pttri2.x > 0.33)
                  {
                     tc.pN2.y = 0;
                     tc.pN2.xz = TotalOne(tc.pN2.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[0], config.cluster2, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[1], config.cluster2, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[2], config.cluster2, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN2;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN2, contrasts.x);
                  tc.pN2 = bf;
               #endif
               

               s.albedo2 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {

               #if _PERTEXTRIPLANAR
                  if (pttri3.x > 0.66)
                  {
                     tc.pN3 = half3(0,1,0);
                  }
                  else if (pttri3.x > 0.33)
                  {
                     tc.pN3.y = 0;
                     tc.pN3.xz = TotalOne(tc.pN3.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[0], config.cluster3, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[1], config.cluster3, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[2], config.cluster3, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN3;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN3, contrasts.x);
               tc.pN3 = bf;
               #endif

               s.albedo3 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif

         #else
            s.albedo0 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv0, config.cluster0, mipLevel);
            COUNTSAMPLE

            MSBRANCH(weights.y)
            {
               s.albedo1 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv1, config.cluster1, mipLevel);
               COUNTSAMPLE
            }
            #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.albedo2 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               } 
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.albedo3 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
            #endif
         #endif

         #if _PERTEXHEIGHTOFFSET || _PERTEXHEIGHTCONTRAST
            SAMPLE_PER_TEX(ptHeight, 10.5, config, 1);

            #if _PERTEXHEIGHTOFFSET
               s.albedo0.a = saturate(s.albedo0.a + ptHeight0.b - 1);
               s.albedo1.a = saturate(s.albedo1.a + ptHeight1.b - 1);
               s.albedo2.a = saturate(s.albedo2.a + ptHeight2.b - 1);
               s.albedo3.a = saturate(s.albedo3.a + ptHeight3.b - 1);
            #endif
            #if _PERTEXHEIGHTCONTRAST
               s.albedo0.a = saturate(pow(s.albedo0.a + 0.5, abs(ptHeight0.a)) - 0.5);
               s.albedo1.a = saturate(pow(s.albedo1.a + 0.5, abs(ptHeight1.a)) - 0.5);
               s.albedo2.a = saturate(pow(s.albedo2.a + 0.5, abs(ptHeight2.a)) - 0.5);
               s.albedo3.a = saturate(pow(s.albedo3.a + 0.5, abs(ptHeight3.a)) - 0.5);
            #endif
         #endif
      }
      
      
      
      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif

         #if _NONORMALMAP || _AUTONORMAL
            s.normSAO0 = half4(0,0, 0, 1);
            s.normSAO1 = half4(0,0, 0, 1);
            s.normSAO2 = half4(0,0, 0, 1);
            s.normSAO3 = half4(0,0, 0, 1);
            return;
         #endif

         
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
            
            half3 absVertNormal = abs(tc.IN.worldNormal);
            float3x3 t2w = tc.IN.TBN;
            
            
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[0], config.cluster0, d0).agrb;
                  COUNTSAMPLE
               }            
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[1], config.cluster0, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[2], config.cluster0, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf0 = SurfgradFromTriplanarProjection(tc.pN0, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO0.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN0, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO0.zw = a0.zw * tc.pN0.x + a1.zw * tc.pN0.y + a2.zw * tc.pN0.z;
            }
            MSBRANCH(weights.y)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[0], config.cluster1, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[1], config.cluster1, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[2], config.cluster1, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf1 = SurfgradFromTriplanarProjection(tc.pN1, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO1.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN1, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO1.zw = a0.zw * tc.pN1.x + a1.zw * tc.pN1.y + a2.zw * tc.pN1.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);

               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[0], config.cluster2, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[1], config.cluster2, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[2], config.cluster2, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf2 = SurfgradFromTriplanarProjection(tc.pN2, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO2.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN2, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO2.zw = a0.zw * tc.pN2.x + a1.zw * tc.pN2.y + a2.zw * tc.pN2.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[0], config.cluster3, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[1], config.cluster3, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[2], config.cluster3, d2).agrb;
                  COUNTSAMPLE
               }

               #if _SURFACENORMALS
                  s.surf3 = SurfgradFromTriplanarProjection(tc.pN3, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO3.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN3, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO3.zw = a0.zw * tc.pN3.x + a1.zw * tc.pN3.y + a2.zw * tc.pN3.z;
            }
            #endif

         #else
            s.normSAO0 = MICROSPLAT_SAMPLE_NORMAL(config.uv0, config.cluster0, mipLevel).agrb;
            COUNTSAMPLE
            s.normSAO0.xy = s.normSAO0.xy * 2 - 1;

            #if _SURFACENORMALS
               s.surf0 = ConvertNormal2ToGradient(s.normSAO0.xy);
            #endif

            MSBRANCH(weights.y)
            {
               s.normSAO1 = MICROSPLAT_SAMPLE_NORMAL(config.uv1, config.cluster1, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO1.xy = s.normSAO1.xy * 2 - 1;

               #if _SURFACENORMALS
                  s.surf1 = ConvertNormal2ToGradient(s.normSAO1.xy);
               #endif
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               s.normSAO2 = MICROSPLAT_SAMPLE_NORMAL(config.uv2, config.cluster2, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf2 = ConvertNormal2ToGradient(s.normSAO2.xy);
               #endif
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               s.normSAO3 = MICROSPLAT_SAMPLE_NORMAL(config.uv3, config.cluster3, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf3 = ConvertNormal2ToGradient(s.normSAO3.xy);
               #endif
            }
            #endif
         #endif
      }

      void SampleEmis(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USEEMISSIVEMETAL
            #if _TRIPLANAR
            
               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  s.emisMetal0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }

                  s.emisMetal1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.emisMetal0 = MICROSPLAT_SAMPLE_EMIS(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.emisMetal1 = MICROSPLAT_SAMPLE_EMIS(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
                  MSBRANCH(weights.z)
                  {
                     s.emisMetal2 = MICROSPLAT_SAMPLE_EMIS(config.uv2, config.cluster2, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  MSBRANCH(weights.w)
                  {
                     s.emisMetal3 = MICROSPLAT_SAMPLE_EMIS(config.uv3, config.cluster3, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
            #endif
         #endif
      }
      
      void SampleSpecular(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USESPECULARWORKFLOW
            #if _TRIPLANAR

               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.specular0 = MICROSPLAT_SAMPLE_SPECULAR(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.specular1 = MICROSPLAT_SAMPLE_SPECULAR(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.specular2 = MICROSPLAT_SAMPLE_SPECULAR(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.specular3 = MICROSPLAT_SAMPLE_SPECULAR(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
               #endif
            #endif
         #endif
      }

      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex, DecalOutput decalOutput)
      {
         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         half4 albedo = 0;
         half4 normSAO = half4(0,0,0,1);
         half3 surfGrad = half3(0,0,0);
         half4 emisMetal = 0;
         half3 specular = 0;
         
         float worldHeight = i.worldPos.y;
         float3 upVector = float3(0,1,0);
         
         #if _GLOBALTINT || _GLOBALNORMALS || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _GLOBALSPECULAR
            float globalSlopeFilter = 1;
            #if _GLOBALSLOPEFILTER
               float2 gfilterUV = float2(1 - saturate(dot(worldNormalVertex, upVector) * 0.5 + 0.49), 0.5);
               globalSlopeFilter = SAMPLE_TEXTURE2D(_GlobalSlopeTex, sampler_Diffuse, gfilterUV).a;
            #endif
         #endif

         // declare outside of branchy areas..
         half4 fxLevels = half4(0,0,0,0);
         half burnLevel = 0;
         half wetLevel = 0;
         half3 waterNormalFoam = half3(0, 0, 0);
         half porosity = 0.4;
         float streamFoam = 1.0f;
         half pud = 0;
         half snowCover = 0;
         half SSSThickness = 0;
         half3 SSSTint = half3(1,1,1);
         float traxBuffer = 0;
         float3 traxNormal = 0;
         float2 noiseUV = 0;
         
         

         #if _SPLATFADE
         MSBRANCHOTHER(1 - saturate(camDist - _SplatFade.y))
         {
         #endif

         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE || _SNOWFOOTSTEPS
            traxBuffer = SampleTraxBuffer(i.worldPos, worldNormalVertex, traxNormal);
         #endif
         
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
            #if _MICROMESH
               fxLevels = SampleFXLevels(InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, config.uv), wetLevel, burnLevel, traxBuffer);
            #elif _MICROVERTEXMESH || _MICRODIGGERMESH  || _MEGASPLAT
               fxLevels = ProcessFXLevels(i.fx, traxBuffer);
            #else
               fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel, traxBuffer);
            #endif
         #endif

         #if _DECAL
            fxLevels = max(fxLevels, decalOutput.fxLevels);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         

         MIPFORMAT albedoLOD = INITMIPFORMAT
         MIPFORMAT normalLOD = INITMIPFORMAT
         MIPFORMAT emisLOD = INITMIPFORMAT
         MIPFORMAT specLOD = INITMIPFORMAT
         MIPFORMAT origAlbedoLOD = INITMIPFORMAT;

         #if _TRIPLANAR && !_DISABLESPLATMAPS
            PrepTriplanar(i.shaderData.texcoord0, worldNormalVertex, i.worldPos, config, tc, weights, albedoLOD, normalLOD, emisLOD, origAlbedoLOD);
            tc.IN = i;
         #endif
         
         
         #if !_TRIPLANAR && !_DISABLESPLATMAPS
            #if _USELODMIP
               albedoLOD = ComputeMipLevel(config.uv0.xy, _Diffuse_TexelSize.zw);
               normalLOD = ComputeMipLevel(config.uv0.xy, _NormalSAO_TexelSize.zw);
               #if _USEEMISSIVEMETAL
                  emisLOD = ComputeMipLevel(config.uv0.xy, _EmissiveMetal_TexelSize.zw);
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = ComputeMipLevel(config.uv0.xy, _Specular_TexelSize.zw);;
               #endif
            #elif _USEGRADMIP
               albedoLOD = float4(ddx(config.uv0.xy), ddy(config.uv0.xy));
               normalLOD = albedoLOD;
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
            #endif

            origAlbedoLOD = albedoLOD;
         #endif

         #if _PERTEXCURVEWEIGHT
           SAMPLE_PER_TEX(ptCurveWeight, 19.5, config, half4(0.5,1,1,1));
           weights.x = lerp(smoothstep(0.5 - ptCurveWeight0.r, 0.5 + ptCurveWeight0.r, weights.x), weights.x, ptCurveWeight0.r*2);
           weights.y = lerp(smoothstep(0.5 - ptCurveWeight1.r, 0.5 + ptCurveWeight1.r, weights.y), weights.y, ptCurveWeight1.r*2);
           weights.z = lerp(smoothstep(0.5 - ptCurveWeight2.r, 0.5 + ptCurveWeight2.r, weights.z), weights.z, ptCurveWeight2.r*2);
           weights.w = lerp(smoothstep(0.5 - ptCurveWeight3.r, 0.5 + ptCurveWeight3.r, weights.w), weights.w, ptCurveWeight3.r*2);
           weights = TotalOne(weights);
         #endif
         
         

         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR && !_DISABLESPLATMAPS
            
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif

            // fix for pertex uv scale using gradient sampler and weight blended derivatives
            #if _USEGRADMIP
               albedoLOD = albedoLOD * ptUVScale0.rgrg * weights.x + 
                           albedoLOD * ptUVScale1.rgrg * weights.y + 
                           albedoLOD * ptUVScale2.rgrg * weights.z + 
                           albedoLOD * ptUVScale3.rgrg * weights.w;
               normalLOD = albedoLOD;
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
            #endif
         #endif

         #if _PERTEXUVROTATION && !_TRIPLANAR && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptUVRot, 16.5, config, half4(0,0,0,0));
            config.uv0.xy = RotateUV(config.uv0.xy, ptUVRot0.x);
            config.uv1.xy = RotateUV(config.uv1.xy, ptUVRot1.x);
            #if !_MAX2LAYER
               config.uv2.xy = RotateUV(config.uv2.xy, ptUVRot2.x);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = RotateUV(config.uv3.xy, ptUVRot0.x);
            #endif
         #endif

         
         o.Alpha = 1;

         
         #if _POM && !_DISABLESPLATMAPS
            DoPOM(i, config, tc, albedoLOD, weights, camDist, worldNormalVertex);
         #endif
         

         SampleAlbedo(config, tc, samples, albedoLOD, weights);

         #if _NOISEHEIGHT
            ApplyNoiseHeight(samples, config.uv, config, i.worldPos, worldNormalVertex);
         #endif
         
         #if _STREAMS || (_PARALLAX && !_DISABLESPLATMAPS)
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(i, config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX && !_DISABLESPLATMAPS
            DoParallax(i, earlyHeight, config, tc, samples, weights, camDist);
         #endif


         // Blend results
         #if _PERTEXINTERPCONTRAST && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = clamp(contrast + _Contrast, 0.0001, 1.0); 
            half cnt = contrast.x * weights.x + contrast.y * weights.y + contrast.z * weights.z + contrast.w * weights.w;
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, cnt);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif

         #if _HYBRIDHEIGHTBLEND
            heightWeights = lerp(heightWeights, TotalOne(weights), saturate(camDist/max(1.0, _HybridHeightBlendDistance)));
         #endif

         
         // rescale derivatives after height weighting. Basically, in gradmip mode we blend the mip levels,
         // but this is before height mapping is sampled, so reblending them after alpha will make sure the other
         // channels (normal, etc) are sharper, which likely matters most.. 
         #if _PERTEXUVSCALEOFFSET && !_DISABLESPLATMAPS
            #if _TRIPLANAR
               #if _USEGRADMIP
                  SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));

                  albedoLOD.d0 = origAlbedoLOD.d0 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d0 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d0 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d0 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d1 = origAlbedoLOD.d1 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d1 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d1 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d1 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d2 = origAlbedoLOD.d2 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d2 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d2 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d2 * ptUVScale3.xyxy * heightWeights.w;
               
                  normalLOD.d0 = albedoLOD.d0;
                  normalLOD.d1 = albedoLOD.d1;
                  normalLOD.d2 = albedoLOD.d2;
               
                  #if _USEEMISSIVEMETAL
                     emisLOD.d0 = albedoLOD.d0;
                     emisLOD.d1 = albedoLOD.d1;
                     emisLOD.d2 = albedoLOD.d2;
                  #endif
               #endif // gradmip
            #else // not triplanar
               // fix for pertex uv scale using gradient sampler and weight blended derivatives
               #if _USEGRADMIP
                  albedoLOD = origAlbedoLOD * ptUVScale0.rgrg * heightWeights.x + 
                              origAlbedoLOD * ptUVScale1.rgrg * heightWeights.y + 
                              origAlbedoLOD * ptUVScale2.rgrg * heightWeights.z + 
                              origAlbedoLOD * ptUVScale3.rgrg * heightWeights.w;
                  normalLOD = albedoLOD;
                  #if _USEEMISSIVEMETAL
                     emisLOD = albedoLOD;
                  #endif
                  #if _USESPECULARWORKFLOW
                     specLOD = albedoLOD;
                  #endif
               #endif
            #endif
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples, albedoLOD, heightWeights);
         #endif


         SampleNormal(config, tc, samples, normalLOD, heightWeights);

         #if _USEEMISSIVEMETAL
            SampleEmis(config, tc, samples, emisLOD, heightWeights);
         #endif

         #if _USESPECULARWORKFLOW
            SampleSpecular(config, tc, samples, specLOD, heightWeights);
         #endif

         #if _DISTANCERESAMPLE && !_DISABLESPLATMAPS
            DistanceResample(samples, config, tc, camDist, i.viewDir, fxLevels, albedoLOD, i.worldPos, heightWeights, worldNormalVertex);
         #endif

         #if _STARREACHFORMAT
            samples.normSAO0.w = length(samples.normSAO0.xy);
            samples.normSAO1.w = length(samples.normSAO1.xy);
            samples.normSAO2.w = length(samples.normSAO2.xy);
            samples.normSAO3.w = length(samples.normSAO3.xy);
         #endif

         // PerTexture sampling goes here, passing the samples structure
         
         #if _PERTEXMICROSHADOWS || _PERTEXFUZZYSHADE
            SAMPLE_PER_TEX(ptFuzz, 17.5, config, half4(0, 0, 1, 1));
         #endif

         #if _PERTEXMICROSHADOWS
            #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP)
            {
               half3 lightDir = GetGlobalLightDirTS(i);
               half4 microShadows = half4(1,1,1,1);
               microShadows.x = MicroShadow(lightDir, half3(samples.normSAO0.xy, 1), samples.normSAO0.a, ptFuzz0.a);
               microShadows.y = MicroShadow(lightDir, half3(samples.normSAO1.xy, 1), samples.normSAO1.a, ptFuzz1.a);
               microShadows.z = MicroShadow(lightDir, half3(samples.normSAO2.xy, 1), samples.normSAO2.a, ptFuzz2.a);
               microShadows.w = MicroShadow(lightDir, half3(samples.normSAO3.xy, 1), samples.normSAO3.a, ptFuzz3.a);
               samples.normSAO0.a *= microShadows.x;
               samples.normSAO1.a *= microShadows.y;
               #if !_MAX2LAYER
                  samples.normSAO2.a *= microShadows.z;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.normSAO3.a *= microShadows.w;
               #endif

               
               #if _DEBUG_OUTPUT_MICROSHADOWS
               o.Albedo = BlendWeights(microShadows.x, microShadows.y, microShadows.z, microShadows.a, heightWeights);
               return o;
               #endif

               

               
            }
            #endif

         #endif // _PERTEXMICROSHADOWS


         #if _PERTEXFUZZYSHADE
            samples.albedo0.rgb = FuzzyShade(samples.albedo0.rgb, half3(samples.normSAO0.rg, 1), ptFuzz0.r, ptFuzz0.g, ptFuzz0.b, i.viewDir);
            samples.albedo1.rgb = FuzzyShade(samples.albedo1.rgb, half3(samples.normSAO1.rg, 1), ptFuzz1.r, ptFuzz1.g, ptFuzz1.b, i.viewDir);
            #if !_MAX2LAYER
               samples.albedo2.rgb = FuzzyShade(samples.albedo2.rgb, half3(samples.normSAO2.rg, 1), ptFuzz2.r, ptFuzz2.g, ptFuzz2.b, i.viewDir);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = FuzzyShade(samples.albedo3.rgb, half3(samples.normSAO3.rg, 1), ptFuzz3.r, ptFuzz3.g, ptFuzz3.b, i.viewDir);
            #endif
         #endif

         #if _PERTEXSATURATION && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptSaturattion, 9.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = lerp(MSLuminance(samples.albedo0.rgb), samples.albedo0.rgb, ptSaturattion0.a);
            samples.albedo1.rgb = lerp(MSLuminance(samples.albedo1.rgb), samples.albedo1.rgb, ptSaturattion1.a);
            #if !_MAX2LAYER
               samples.albedo2.rgb = lerp(MSLuminance(samples.albedo2.rgb), samples.albedo2.rgb, ptSaturattion2.a);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = lerp(MSLuminance(samples.albedo3.rgb), samples.albedo3.rgb, ptSaturattion3.a);
            #endif
         
         #endif
         
         #if _PERTEXTINT && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif
         
         #if _PCHEIGHTGRADIENT || _PCHEIGHTHSV || _PCSLOPEGRADIENT || _PCSLOPEHSV
            ProceduralGradients(i, samples, config, worldHeight, worldNormalVertex);
         #endif

         
         

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXCOLORINTENSITY
            SAMPLE_PER_TEX(ptCI, 23.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = saturate(samples.albedo0.rgb * (1 + ptCI0.rrr));
            samples.albedo1.rgb = saturate(samples.albedo1.rgb * (1 + ptCI1.rrr));
            #if !_MAX2LAYER
               samples.albedo2.rgb = saturate(samples.albedo2.rgb * (1 + ptCI2.rrr));
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = saturate(samples.albedo3.rgb * (1 + ptCI3.rrr));
            #endif
         #endif

         #if (_PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = saturate(((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5);
               samples.albedo1.rgb = saturate(((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5);
               #if !_MAX2LAYER
                 samples.albedo2.rgb = saturate(((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5);
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb = saturate(samples.albedo0.rgb + ptBC0.rrr);
               samples.albedo1.rgb = saturate(samples.albedo1.rgb + ptBC1.rrr);
               #if !_MAX2LAYER
                  samples.albedo2.rgb = saturate(samples.albedo2.rgb + ptBC2.rrr);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(samples.albedo3.rgb + ptBC3.rrr);
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if (_PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR && !_DISABLESPLATMAPS
            #if _SURFACENORMALS
               samples.surf0 *= perTexMatSettings0.r;
               samples.surf1 *= perTexMatSettings1.r;
               samples.surf2 *= perTexMatSettings2.r;
               samples.surf3 *= perTexMatSettings3.r;
            #else
               samples.normSAO0.xy *= perTexMatSettings0.r;
               samples.normSAO1.xy *= perTexMatSettings1.r;
               samples.normSAO2.xy *= perTexMatSettings2.r;
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR && !_DISABLESPLATMAPS
            samples.normSAO0.a = pow(abs(samples.normSAO0.a), perTexMatSettings0.b);
            samples.normSAO1.a = pow(abs(samples.normSAO1.a), perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(abs(samples.normSAO2.a), perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(abs(samples.normSAO3.a), perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR && !_DISABLESPLATMAPS
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         
         #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP) 
          #if _PERTEXSSS
          {
            SAMPLE_PER_TEX(ptSSS, 18.5, config, half4(1, 1, 1, 1)); // tint, thickness
            half4 vals = ptSSS0 * heightWeights.x + ptSSS1 * heightWeights.y + ptSSS2 * heightWeights.z + ptSSS3 * heightWeights.w;
            SSSThickness = vals.a;
            SSSTint = vals.rgb;
          }
          #endif
         #endif

         #if _PERTEXRIMLIGHT
         {
            SAMPLE_PER_TEX(ptRimA, 26.5, config, half4(1, 1, 1, 1));
            SAMPLE_PER_TEX(ptRimB, 27.5, config, half4(1, 1, 1, 0));
            samples.emisMetal0.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO0.xy, 1))), max(0.0001, ptRimA0.g)) * ptRimB0.rgb * ptRimB0.a;
            samples.emisMetal1.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO1.xy, 1))), max(0.0001, ptRimA1.g)) * ptRimB1.rgb * ptRimB1.a;
            samples.emisMetal2.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO2.xy, 1))), max(0.0001, ptRimA2.g)) * ptRimB2.rgb * ptRimB2.a;
            samples.emisMetal3.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO3.xy, 1))), max(0.0001, ptRimA3.g)) * ptRimB3.rgb * ptRimB3.a;
         }
         #endif



         #if (((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)) && !_DISABLESPLATMAPS
            ApplyDetailDistanceNoisePerTex(samples, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         
         #if _GLOBALNOISEUV
            // noise defaults so that a value of 1, 1 is 4 pixels in size and moves the uvs by 1 pixel max.
            #if _CUSTOMSPLATTEXTURES
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #else
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif
         #endif

         
         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE
            ApplyTrax(samples, config, i.worldPos, traxBuffer, traxNormal);
         #endif

         #if (_ANTITILEARRAYDETAIL || _ANTITILEARRAYDISTANCE || _ANTITILEARRAYNORMAL) && !_DISABLESPLATMAPS
            ApplyAntiTilePerTex(samples, config, camDist, i.worldPos, worldNormalVertex, heightWeights);
         #endif

         #if _GEOMAP && !_DISABLESPLATMAPS
         GeoTexturePerTex(samples, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif
         
         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH && !_DISABLESPLATMAPS
         GlobalTintTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH && !_DISABLESPLATMAPS
         GlobalNormalTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && _PERTEXGLOBALSAOMSTRENGTH && !_DISABLESPLATMAPS
         GlobalSAOMTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALEMIS && _PERTEXGLOBALEMISSTRENGTH && !_DISABLESPLATMAPS
         GlobalEmisTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && _PERTEXGLOBALSPECULARSTRENGTH && !_DISABLESPLATMAPS && _USESPECULARWORKFLOW
         GlobalSpecularTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _PERTEXMETALLIC && !_DISABLESPLATMAPS
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         #if _GLITTER && !_DISABLESPLATMAPS
            DoGlitter(i, samples, config, camDist, worldNormalVertex, i.worldPos);
         #endif
         
         // Blend em..
         #if _DISABLESPLATMAPS
            // If we don't sample from the _Diffuse, then the shader compiler will strip the sampler on
            // some platforms, which will cause everything to break. So we sample from the lowest mip
            // and saturate to 1 to keep the cost minimal. Annoying, but the compiler removes the texture
            // and sampler, even though the sampler is still used.
            albedo = saturate(UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, float3(0,0,0), 12) + 1);
            albedo.a = 0.5; // make height something we can blend with for the combined mesh mode, since it still height blends.
            normSAO = half4(0,0,0,1);
         #else
            albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
            normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);

            #if _SURFACENORMALS
               surfGrad = BlendWeights(samples.surf0, samples.surf1, samples.surf2, samples.surf3, heightWeights);
            #endif

            #if (_USEEMISSIVEMETAL || _PERTEXRIMLIGHT) && !_DISABLESPLATMAPS
               emisMetal = BlendWeights(samples.emisMetal0, samples.emisMetal1, samples.emisMetal2, samples.emisMetal3, heightWeights);
            #endif

            #if _USESPECULARWORKFLOW && !_DISABLESPLATMAPS
               specular = BlendWeights(samples.specular0, samples.specular1, samples.specular2, samples.specular3, heightWeights);
            #endif

            #if _PERTEXOUTLINECOLOR
               SAMPLE_PER_TEX(ptOutlineColor, 28.5, config, half4(0.5, 0.5, 0.5, 1));
               half4 outlineColor = BlendWeights(ptOutlineColor0, ptOutlineColor1, ptOutlineColor2, ptOutlineColor3, heightWeights);
               half4 tstr = saturate(abs(heightWeights - 0.5) * 2);
               half transitionBlend = min(min(min(tstr.x, tstr.y), tstr.z), tstr.w);
               albedo.rgb = lerp(albedo.rgb * outlineColor.rgb * 2, albedo.rgb, outlineColor.a * transitionBlend);
            #endif
         #endif



         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
            o.Alpha = 1.0;
            if (config.uv0.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.x;
            else if (config.uv1.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.y;
            else if (config.uv2.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.z;
            else if (config.uv3.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.w;
         #endif



         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
            ApplyDetailDistanceNoise(albedo.rgb, normSAO, surfGrad, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         #if _SPLATFADE
         }
         #endif

         #if _SPLATFADE
            
            float2 sfDX = ddx(config.uv * _UVScale);
            float2 sfDY = ddy(config.uv * _UVScale);

            MSBRANCHOTHER(camDist - _SplatFade.x)
            {
               float falloff = saturate(InverseLerp(_SplatFade.x, _SplatFade.y, camDist));
               half4 sfalb = SAMPLE_TEXTURE2D_ARRAY_GRAD(_Diffuse, sampler_Diffuse, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY);
               COUNTSAMPLE
               albedo.rgb = lerp(albedo.rgb, sfalb.rgb, falloff);

               #if !_NONORMALMAP && !_AUTONORMAL
                  half4 sfnormSAO = SAMPLE_TEXTURE2D_ARRAY_GRAD(_NormalSAO, sampler_NormalSAO, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY).agrb;
                  COUNTSAMPLE
                  sfnormSAO.xy = sfnormSAO.xy * 2 - 1;

                  normSAO = lerp(normSAO, sfnormSAO, falloff);
  
                  #if _SURFACENORMALS
                     surfGrad = lerp(surfGrad, ConvertNormal2ToGradient(sfnormSAO.xy), falloff);
                  #endif
               #endif
              
            }
         #endif

         #if _AUTONORMAL
            float3 autoNormal = HeightToNormal(albedo.a * _AutoNormalHeightScale, i.worldPos);
            normSAO.xy = autoNormal;
            normSAO.z = 0;
            normSAO.w = (autoNormal.z * autoNormal.z);
         #endif
 


         #if _MESHCOMBINED
            SampleMeshCombined(albedo, normSAO, surfGrad, emisMetal, specular, o.Alpha, SSSThickness, SSSTint, config, heightWeights);
         #endif

         #if _ISOBJECTSHADER
            SampleObjectShader(i, albedo, normSAO, surfGrad, emisMetal, specular, config);
         #endif

         #if _GEOMAP
            GeoTexture(albedo.rgb, normSAO, surfGrad, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif

         
         #if _SCATTER
            ApplyScatter(
               #if _MEGASPLAT
               config, 
               #endif
               i, albedo, normSAO, surfGrad, config.uv, camDist);
         #endif

         #if _DECAL
            DoDecalBlend(decalOutput, albedo, normSAO, surfGrad, emisMetal, i.uv_Control0);
         #endif
         

         #if _GLOBALTINT && !_PERTEXGLOBALTINTSTRENGTH
            GlobalTintTexture(albedo.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _VSGRASSMAP
            VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMALSTRENGTH
            GlobalNormalTexture(normSAO, surfGrad, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && !_PERTEXGLOBALSAOMSTRENGTH
            GlobalSAOMTexture(normSAO, emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALEMIS && !_PERTEXGLOBALEMISSTRENGTH
            GlobalEmisTexture(emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && !_PERTEXGLOBALSPECULARSTRENGTH && _USESPECULARWORKFLOW
            GlobalSpecularTexture(specular.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         
         
         o.Albedo = albedo.rgb;
         o.Height = albedo.a;

         #if _NONORMALMAP
            o.Normal = half3(0,0,1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #elif _SURFACENORMALS
            o.Normal = ResolveNormalFromSurfaceGradient(surfGrad);
            o.Normal = mul(GetTBN(i), o.Normal);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #else
            o.Normal = half3(normSAO.xy, 1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;       
         #endif


         

         #if _USEEMISSIVEMETAL || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _PERTEXRIMLIGHT
           #if _USEEMISSIVEMETAL
	           emisMetal.rgb *= _EmissiveMult;
	        #endif
           
           o.Emission += emisMetal.rgb;
           o.Metallic = emisMetal.a;
	        
         #endif

         #if _USESPECULARWORKFLOW
            o.Specular = specular;
         #endif

         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(i, o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         
         #if _SNOW
         snowCover = DoSnow(i, o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, 
            config, weights, SSSTint, SSSThickness, traxBuffer, traxNormal);
         #endif

         #if _PERTEXSSS || _MESHCOMBINEDUSESSS || (_SNOW && _SNOWSSS)
         {
            half3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

            o.Emission += ComputeSSS(i, worldView, WorldNormalVector(i, o.Normal),
               SSSTint, SSSThickness, _SSSDistance, _SSSScale, _SSSPower);
         }
         #endif
         
         #if _SNOWGLITTER
            DoSnowGlitter(i, config, o, camDist, worldNormalVertex, snowCover);
         #endif

         #if _WINDPARTICULATE || _SNOWPARTICULATE
            DoWindParticulate(i, o, config, weights, camDist, worldNormalVertex, snowCover);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         #if _SPECULARFADE
         {
            float specFade = saturate((i.worldPos.y - _SpecularFades.x) / max(_SpecularFades.y - _SpecularFades.x, 0.0001));
            o.Metallic *= specFade;
            o.Smoothness *= specFade;
         }
         #endif

         #if _VSSHADOWMAP
         VSShadowTexture(o, i, config, camDist);
         #endif
         
         #if _TOONWIREFRAME
         ToonWireframe(config.uv, o.Albedo, camDist);
         #endif

        #if _SEETHROUGHSHADER
            SeethroughShader(o.Albedo, o.Emission, o.Alpha, i.worldPos, o.Normal, i.worldNormal);                   
        #endif


         #if _DEBUG_TRAXBUFFER
            ClearAllButAlbedo(o, half3(traxBuffer, 0, 0) * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMALVERTEX
            ClearAllButAlbedo(o, worldNormalVertex * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMAL
            ClearAllButAlbedo(o,  WorldNormalVector(i, o.Normal) * saturate(o.Albedo.z+1));
         #endif

         #if _DEBUG_MEGABARY && _MEGASPLAT
            o.Albedo = i.baryWeights.xyz;
         #endif


         return o;
      }
      
      void SampleSplats(float2 controlUV, inout half4 w0, inout half4 w1, inout half4 w2, inout half4 w3, inout half4 w4, inout half4 w5, inout half4 w6, inout half4 w7)
      {
         #if _CUSTOMSPLATTEXTURES
            #if !_MICROMESH
            controlUV = (controlUV * (_CustomControl0_TexelSize.zw - 1.0f) + 0.5f) * _CustomControl0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_CustomControl0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_CustomControl1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_CustomControl2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_CustomControl3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_CustomControl4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_CustomControl5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_CustomControl6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_CustomControl7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #else
            #if !_MICROMESH
            controlUV = (controlUV * (_Control0_TexelSize.zw - 1.0f) + 0.5f) * _Control0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_Control0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_Control1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_Control2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_Control3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_Control4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_Control5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_Control6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_Control7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #endif
      }   


      

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
         #if _MEGANOUV
            i.uv_Control0 = i.worldPos.xz;
         #endif
         
         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);
          
         #if _FORCELOCALSPACE
            worldNormalVertex = mul((float3x3)GetWorldToObjectMatrix(), worldNormalVertex).xyz;
            i.worldPos = i.worldPos -  mul(GetObjectToWorldMatrix(), float4(0,0,0,1)).xyz;
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ORIGINSHIFT
             i.worldPos = i.worldPos + mul(_GlobalOriginMTX, float4(0,0,0,1)).xyz;
             i.worldHeight = i.worldPos.y;
         #endif

         #if _DEBUG_USE_TOPOLOGY
            i.worldPos = SAMPLE_TEXTURE2D(_DebugWorldPos, sampler_Diffuse, i.uv_Control0);
            worldNormalVertex = SAMPLE_TEXTURE2D(_DebugWorldNormal, sampler_Diffuse, i.uv_Control0);
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ALPHABELOWHEIGHT && !_TBDISABLEALPHAHOLES
            ClipWaterLevel(i.worldPos);
         #endif

         #if !_TBDISABLEALPHAHOLES && defined(_ALPHATEST_ON)
            // UNITY 2019.3 holes
            ClipHoles(i.uv_Control0);
         #endif


         float2 origUV = i.uv_Control0;

         #if _MICROMESH && _MESHUV2
         float2 controlUV = i.uv2_Diffuse;
         #else
         float2 controlUV = i.uv_Control0;
         #endif


         #if _MICROMESH
            controlUV = InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, controlUV);
         #endif

         half4 weights = half4(1,0,0,0);

         Config config = (Config)0;
         UNITY_INITIALIZE_OUTPUT(Config,config);
         config.uv = origUV;

         DecalOutput decalOutput = (DecalOutput)0;
         #if _DECAL
            decalOutput = DoDecals(i.uv_Control0, i.worldPos, camDist, worldNormalVertex);
         #endif

         #if _SURFACENORMALS
         // Initialize the surface gradient basis vectors
         ConstructSurfaceGradientTBN(i);
         #endif
        


         #if _SPLATFADE
         MSBRANCHOTHER(_SplatFade.y - camDist)
         #endif // _SPLATFADE
         {
            #if !_DISABLESPLATMAPS

               // Sample the splat data, from textures or vertices, and setup the config..
               #if _MICRODIGGERMESH
                  DiggerSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLAT
                  MegaSplatVertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLATTEXTURE
                   MegaSplatTextureSetup(controlUV, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MICROVERTEXMESH
                  VertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif !_PROCEDURALTEXTURE || _PROCEDURALBLENDSPLATS
                  half4 w0 = 0; half4 w1 = 0; half4 w2 = 0; half4 w3 = 0; half4 w4 = 0; half4 w5 = 0; half4 w6 = 0; half4 w7 = 0;
                  SampleSplats(controlUV, w0, w1, w2, w3, w4, w5, w6, w7);
                  Setup(weights, origUV, config, w0, w1, w2, w3, w4, w5, w6, w7, i.worldPos, decalOutput);
               #endif

               #if _PROCEDURALTEXTURE
                  float3 procNormal = worldNormalVertex;
                  float3 worldPos = i.worldPos;
                  ProceduralSetup(i, worldPos, i.worldHeight, procNormal, i.worldUpVector, weights, origUV, config, ddx(origUV), ddy(origUV), ddx(worldPos), ddy(worldPos), decalOutput);
               #endif
            #else // _DISABLESPLATMAPS
                Setup(weights, origUV, config, half4(1,0,0,0), 0, 0, 0, 0, 0, 0, 0, i.worldPos, decalOutput);
            #endif

            #if _SLOPETEXTURE
               SlopeTexture(config, weights, worldNormalVertex);
            #endif
         } // _SPLATFADE else case


         #if _TOONFLATTEXTURE
            float2 quv = floor(origUV * _ToonTerrainSize);
            float2 fuv = frac(origUV * _ToonTerrainSize);
            #if !_TOONFLATTEXTUREQUAD
               quv = Hash2D((fuv.x > fuv.y) ? quv : quv * 0.333);
            #endif
            float2 uvq = quv / _ToonTerrainSize;
            config.uv0.xy = uvq;
            config.uv1.xy = uvq;
            config.uv2.xy = uvq;
            config.uv3.xy = uvq;
         #endif
         
         #if (_TEXTURECLUSTER2 || _TEXTURECLUSTER3) && !_DISABLESPLATMAPS
            PrepClusters(origUV, config, i.worldPos, worldNormalVertex);
         #endif

         #if (_ALPHAHOLE || _ALPHAHOLETEXTURE) && !_DISABLESPLATMAPS && !_TBDISABLEALPHAHOLES
         ClipAlphaHole(config, weights);
         #endif


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex, decalOutput);

         // On windows, sometimes the shared samplers gets stripped, so we have to do this crap.
         // We sample from the lowest mip, so it shouldn't cost much, but still, I hate this, wtf..
         float stripVal = saturate(SAMPLE_TEXTURE2D_LOD(_Diffuse, sampler_Diffuse, config.uv0, 11).r + 2);
         stripVal *= saturate(SAMPLE_TEXTURE2D_LOD(_NormalSAO, sampler_NormalSAO, config.uv0, 11).r + 2);
         l.Albedo *= stripVal;
         l.Normal *= stripVal;

         #if _PROCEDURALTEXTURE
            ProceduralTextureDebugOutput(l, weights, config);
         #endif


         return l;

      }



   





float4 ConstructTerrainTangent(float3 normal, float3 positiveZ)
{
    // Consider a flat terrain. It should have tangent be (1, 0, 0) and bitangent be (0, 0, 1) as the UV of the terrain grid mesh is a scale of the world XZ position.
    // In CreateTangentToWorld function (in SpaceTransform.hlsl), it is cross(normal, tangent) * sgn for the bitangent vector.
    // It is not true in a left-handed coordinate system for the terrain bitangent, if we provide 1 as the tangent.w. It would produce (0, 0, -1) instead of (0, 0, 1).
    // Also terrain's tangent calculation was wrong in a left handed system because cross((0,0,1), terrainNormalOS) points to the wrong direction as negative X.
    // Therefore all the 4 xyzw components of the tangent needs to be flipped to correct the tangent frame.
    // (See TerrainLitData.hlsl - GetSurfaceAndBuiltinData)
    float3 tangent = cross(normal, positiveZ);
    return float4(tangent, -1);
}



void TerrainInstancing(inout float4 vertex, inout float3 normal, inout float2 uv)
{
#if _MICROTERRAIN && defined(UNITY_INSTANCING_ENABLED) && !_TERRAINBLENDABLESHADER
   
    float2 patchVertex = vertex.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;

    float2 sampleUV = (uv / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

    float height = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, shared_linear_clamp_sampler, sampleUV, 0));
   
    vertex.xz = sampleCoords * _TerrainHeightmapScale.xz;
    vertex.y = height * _TerrainHeightmapScale.y;

    
    normal = float3(0, 1, 0);

#endif
}


void ApplyMeshModification(inout VertexData input)
{
   #if _MICROTERRAIN && !_TERRAINBLENDABLESHADER
      float2 uv = input.texcoord0.xy;
      TerrainInstancing(input.vertex, input.normal, uv);
      input.texcoord0.xy = uv;
   #endif
   #if _PERPIXNORMAL && !_TERRAINBLENDABLESHADER
      input.normal = float3(0,1,0);
   #endif

}

// called by the template, so we can remove tangent from VertexData
void ApplyTerrainTangent(inout VertexToPixel input)
{
   #if (_MICROTERRAIN || _PERPIXNORMAL) && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif

   // digger meshes ain't got no tangent either..
   #if _MICRODIGGERMESH && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif
}


void ModifyVertex(inout VertexData v, inout ExtraV2F d)
{
   ApplyMeshModification(v);

   #if _MICROVERTEXMESH || _MICRODIGGERMESH
      EncodeVertexWorkflow(v, d);
   #elif _MEGASPLAT
      EncodeMegaSplatVertex(v, d);
   #endif

}


void ModifyTessellatedVertex(inout VertexData v, inout ExtraV2F d)
{
   #if _TESSDISTANCE
      v.vertex.xyz += OffsetVertex(v, d);
   #endif
}

float3 GetTessFactors ()
{
   #if _TESSDISTANCE
      return float3(_TessData2.x, _TessData2.y, _TessData1.x);
   #endif
   return 0;
}


        


    
    void SurfaceFunction(inout Surface o, inout ShaderData d)
    {
       
        float3 worldNormalVertex = d.worldSpaceNormal;

        #if (defined(UNITY_INSTANCING_ENABLED) && _MICROTERRAIN && !_TERRAINBLENDABLESHADER)
            float2 sampleCoords = (d.texcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            float3 geomBitangent = normalize(cross(geomNormal, geomTangent)) * -1;
            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

         #elif _PERPIXNORMAL &&  (_MICROTERRAIN || _MICROMESHTERRAIN) && !_TERRAINBLENDABLESHADER
            float2 sampleCoords = (d.texcoord0.xy * _PerPixelNormal_TexelSize.zw + 0.5f) * _PerPixelNormal_TexelSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_PerPixelNormal, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            
            float3 geomBitangent = normalize(cross(geomTangent, geomNormal)) * -1;
            #if _MICROMESHTERRAIN
                geomBitangent *= -1;
            #endif

            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

        #endif

        #if _TOONPOLYEDGE
           FlatShade(d);
        #endif

         Input i = DescToInput(d);

         
         
         #if _SRPTERRAINBLEND
            MicroSplatLayer l = BlendWithTerrain(d);

               #if _DEBUG_WORLDNORMAL
                  ClearAllButAlbedo(l, normalize(TangentToWorldSpace(d, l.Normal)) * saturate(l.Albedo.z+1));
               #endif
         #else
            MicroSplatLayer l = SurfImpl(i, worldNormalVertex);
         #endif

        DoDebugOutput(l);




        o.Albedo = l.Albedo;
        o.Normal = l.Normal;
        o.Smoothness = l.Smoothness;
        o.Occlusion = l.Occlusion;
        o.Metallic = l.Metallic;
        o.Emission = l.Emission;
        #if _USESPECULARWORKFLOW
        o.Specular = l.Specular;
        #endif
        o.Height = l.Height;
        o.Alpha = l.Alpha;


    }



        



         // SHADERDESC

         ShaderData CreateShaderData(VertexToPixel i)
         {
            ShaderData d = (ShaderData)0;
            d.worldSpacePosition = i.worldPos;

            d.worldSpaceNormal = i.worldNormal;
            d.worldSpaceTangent = i.worldTangent.xyz;
            float3 bitangent = cross(i.worldTangent.xyz, i.worldNormal) * i.worldTangent.w * -1;
            

            d.TBNMatrix = float3x3(d.worldSpaceTangent, bitangent, d.worldSpaceNormal);
            d.worldSpaceViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);
             d.texcoord0 = i.texcoord0;
            #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                d.texcoord1 = i.texcoord1;
               // d.texcoord2 = i.texcoord2;
            #endif
            // d.texcoord3 = i.texcoord3;
            // d.vertexColor = i.vertexColor;

            // these rarely get used, so we back transform them. Usually will be stripped.
            #if _HDRP
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(GetCameraRelativePositionWS(i.worldPos), 1));
            #else
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(i.worldPos, 1));
            #endif
            // d.localSpaceNormal = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldNormal));
            // d.localSpaceTangent = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldTangent.xyz));

            // d.screenPos = i.screenPos;
            // d.screenUV = i.screenPos.xy / i.screenPos.w;

            // d.extraV2F0 = i.extraV2F0;
            // d.extraV2F1 = i.extraV2F1;
            // d.extraV2F2 = i.extraV2F2;
            // d.extraV2F3 = i.extraV2F3;
            // d.extraV2F4 = i.extraV2F4;
            // d.extraV2F5 = i.extraV2F5;
            // d.extraV2F6 = i.extraV2F6;
            // d.extraV2F7 = i.extraV2F7;

            return d;
         }
         // CHAINS

            void ChainModifyVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               ModifyVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }

            void ChainModifyTessellatedVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               // d.extraV2F0 = v2p.extraV2F0;
               // d.extraV2F1 = v2p.extraV2F1;
               // d.extraV2F2 = v2p.extraV2F2;
               // d.extraV2F3 = v2p.extraV2F3;
               // d.extraV2F4 = v2p.extraV2F4;
               // d.extraV2F5 = v2p.extraV2F5;
               // d.extraV2F6 = v2p.extraV2F6;
               // d.extraV2F7 = v2p.extraV2F7;

               ModifyTessellatedVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }


            void ChainFinalColorForward(inout Surface l, inout ShaderData d, inout half4 color)
            {
               
            }

            void ChainFinalGBufferStandard(inout Surface s, inout ShaderData d, inout half4 GBuffer0, inout half4 GBuffer1, inout half4 GBuffer2, inout half4 outEmission, inout half4 outShadowMask)
            {
               
            }


         
         #if _PASSSHADOW
            float3 _LightDirection;
            float3 _LightPosition;
         #endif

         // vertex shader
         VertexToPixel Vert (VertexData v)
         {
           
           VertexToPixel o = (VertexToPixel)0;

           UNITY_SETUP_INSTANCE_ID(v);
           UNITY_TRANSFER_INSTANCE_ID(v, o);
           UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


#if !_TESSELLATION_ON
           ChainModifyVertex(v, o);
#endif

            o.texcoord0 = v.texcoord0;

           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               o.texcoord1 = v.texcoord1;
              // o.texcoord2 = v.texcoord2;
           #endif

           // o.texcoord3 = v.texcoord3;
           // o.vertexColor = v.vertexColor;
           
           VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
           o.worldPos = TransformObjectToWorld(v.vertex.xyz);
           o.worldNormal = TransformObjectToWorldNormal(v.normal);

           
           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float2 uv1 = v.texcoord1.xy;
               float2 uv2 = v.texcoord2.xy;
               o.worldTangent = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);
           #else
               float2 uv1 = v.texcoord0.xy;
               float2 uv2 = uv1;
           #endif

           // MS Only
           ApplyTerrainTangent(o);
           

          #if _PASSSHADOW
              #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                 float3 lightDirectionWS = normalize(_LightPosition - o.worldPos);
              #else
                 float3 lightDirectionWS = _LightDirection;
              #endif
              // Define shadow pass specific clip position for Universal
              o.pos = TransformWorldToHClip(ApplyShadowBias(o.worldPos, o.worldNormal, lightDirectionWS));
              #if UNITY_REVERSED_Z
                  o.pos.z = min(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #else
                  o.pos.z = max(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #endif
          #elif _PASSMETA
              #if _MICROTERRAIN
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), v.texcoord0.xy, v.texcoord0.xy, unity_LightmapST, unity_DynamicLightmapST);
              #else
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), uv1, uv2, unity_LightmapST, unity_DynamicLightmapST);
              #endif
          #else
              o.pos = TransformWorldToHClip(o.worldPos);
          #endif



          // o.screenPos = ComputeScreenPos(o.pos, _ProjectionParams.x);
          

          #if _PASSFORWARD || _PASSGBUFFER
              #if _MICROTERRAIN
                 OUTPUT_LIGHTMAP_UV(v.texcoord0.xy, unity_LightmapST, o.lightmapUV);
              #else
                 OUTPUT_LIGHTMAP_UV(uv1, unity_LightmapST, o.lightmapUV);
              #endif
              OUTPUT_SH(o.worldNormal, o.sh);
              #if defined(DYNAMICLIGHTMAP_ON)
                   o.dynamicLightmapUV.xy = uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
              #endif
          #endif

          #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
              half fogFactor = 0;
              #if defined(_FOG_FRAGMENT)
                fogFactor = ComputeFogFactor(o.pos.z);
              #endif
              #if _BAKEDLIT
                  half3 vertexLight = VertexLighting(o.worldPos, o.worldNormal);
                  o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
              #else
                  o.fogFactorAndVertexLight = half4(fogFactor, 0,0,0);
              #endif
          #endif

          #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             o.shadowCoord = GetShadowCoord(vertexInput);
          #endif

           return o;
         }


         

#if _UNLIT
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Unlit.hlsl"  
#endif

         // fragment shader
         half4 Frag (VertexToPixel IN
            #ifdef _DEPTHOFFSET_ON
              , out float outputDepth : SV_Depth
            #endif
         ) : SV_Target
         {
           UNITY_SETUP_INSTANCE_ID(IN);
           UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

           ShaderData d = CreateShaderData(IN);
           Surface l = (Surface)0;

           #ifdef _DEPTHOFFSET_ON
              l.outputDepth = outputDepth;
           #endif

           l.Albedo = half3(0.5, 0.5, 0.5);
           l.Normal = float3(0,0,1);
           l.Occlusion = 1;
           l.Alpha = 1;

           SurfaceFunction(l, d);

           #ifdef _DEPTHOFFSET_ON
              outputDepth = l.outputDepth;
           #endif

           #if defined(_USESPECULAR) || _SIMPLELIT
              float3 specular = l.Specular;
              float metallic = 0;
           #else   
              float3 specular = 0;
              float metallic = l.Metallic;
           #endif


            
           
            InputData inputData = (InputData)0;

            inputData.positionWS = IN.worldPos;
            inputData.normalWS = mul(l.Normal, d.TBNMatrix);
            inputData.viewDirectionWS = SafeNormalize(d.worldSpaceViewDir);


            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                  inputData.shadowCoord = IN.shadowCoord;
            #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                  inputData.shadowCoord = TransformWorldToShadowCoord(IN.worldPos);
            #else
                  inputData.shadowCoord = float4(0, 0, 0, 0);
            #endif

            #if _BAKEDLIT
               inputData.fogCoord = IN.fogFactorAndVertexLight.x;
            #elif VERSION_GREATER_EQUAL(12, 0)
               inputData.fogCoord = InitializeInputDataFog(float4(IN.worldPos, 1.0), IN.fogFactorAndVertexLight.x);
            #else
               inputData.fogCoord = IN.fogFactorAndVertexLight.x;
            #endif

            inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

            #if defined(DYNAMICLIGHTMAP_ON)
                inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.dynamicLightmapUV.xy, IN.sh, inputData.normalWS);
            #else
                inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.sh, inputData.normalWS);
            #endif

            inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.pos);
            #if !_BAKEDLIT && defined(LIGHTMAP_ON)
               inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUV);
            #endif

            #if defined(DEBUG_DISPLAY)
                #if defined(DYNAMICLIGHTMAP_ON)
                  inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
                #endif
                #if defined(LIGHTMAP_ON)
                  inputData.staticLightmapUV = IN.lightmapUV;
                #else
                  inputData.vertexSH = IN.sh;
                #endif
            #endif

            SurfaceData surface         = (SurfaceData)0;
            surface.albedo              = l.Albedo;
            surface.metallic            = saturate(metallic);
            surface.specular            = specular;
            surface.smoothness          = saturate(l.Smoothness),
            surface.occlusion           = l.Occlusion,
            surface.emission            = l.Emission,
            surface.alpha               = saturate(l.Alpha);
            surface.clearCoatMask       = 0;
            surface.clearCoatSmoothness = 1;

            half4 color = half4(l.Albedo, l.Alpha);
            #ifdef _DBUFFER
               #if _BAKEDLIT
                  ApplyDecalToBaseColorAndNormal(IN.pos, color, inputData.normalWS);
               #else
                  ApplyDecalToSurfaceData(IN.pos, surface, inputData);
               #endif
            #endif

            #if !_UNLIT
               #if _SIMPLELIT
                  color = UniversalFragmentBlinnPhong(
                     inputData,
                     surface);
               #elif _BAKEDLIT
                  color = UniversalFragmentBakedLit(inputData, color.rgb, color.a, l.Normal);
               #else
                  color = UniversalFragmentPBR(inputData, surface);
               #endif
               color.rgb = MixFog(color.rgb, inputData.fogCoord);

            #else
               #ifdef _DBUFFER
                  ApplyDecalToSurfaceData(IN.pos, surface, inputData);
               #endif
               color = UniversalFragmentUnlit(inputData, l.Albedo, l.Alpha);
            #endif
            ChainFinalColorForward(l, d, color);

            return color;

         }

         ENDHLSL

      }


      
        Pass
        {
            Name "GBuffer"
            Tags
            {
               "LightMode" = "UniversalGBuffer"
            }
           
             Blend One Zero
             ZTest LEqual
             ZWrite On

            

            HLSLPROGRAM

               #pragma vertex Vert
   #pragma fragment Frag

            #pragma target 3.5

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma instancing_options norenderinglayer assumeuniformscaling nomatrices nolightprobe nolightmap
            
        
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile _ _GBUFFER_NORMALS_OCT
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _RENDER_PASS_ENABLED
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _WRITE_RENDERING_LAYERS
            #define _FOG_FRAGMENT 1

            
      #define _MICROSPLAT 1
      #define _MICROTERRAIN 1
      #define _USEGRADMIP 1
      #define _MAX4TEXTURES 1
      #define _BRANCHSAMPLES 1
      #define _BRANCHSAMPLESAGR 1
      #define _MSRENDERLOOP_UNITYURP2022 1

#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


   #define _URP 1



            #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
            #define SHADERPASS SHADERPASS_GBUFFER
            #define _PASSGBUFFER 1

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #if VERSION_GREATER_EQUAL(12, 0)
               #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #endif
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            

            

                  #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, float3x3(d.worldSpaceTangent, cross(d.worldSpaceTangent, d.worldSpaceNormal), d.worldSpaceNormal))

      
      #define UnityObjectToWorldNormal(normal) mul(GetObjectToWorldMatrix(), normal)

      #define _WorldSpaceLightPos0 _MainLightPosition
      
      #define UNITY_DECLARE_TEX2D(name) TEXTURE2D(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2D_NOSAMPLER(name) TEXTURE2D(name);
      #define UNITY_DECLARE_TEX2DARRAY(name) TEXTURE2D_ARRAY(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(name) TEXTURE2D_ARRAY(name);

      #define UNITY_SAMPLE_TEX2DARRAY(tex,coord)            SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, coord.xy, coord.z)
      #define UNITY_SAMPLE_TEX2DARRAY_LOD(tex,coord,lod)    SAMPLE_TEXTURE2D_ARRAY_LOD(tex, sampler##tex, coord.xy, coord.z, lod)
      #define UNITY_SAMPLE_TEX2D(tex, coord)                SAMPLE_TEXTURE2D(tex, sampler##tex, coord)
      #define UNITY_SAMPLE_TEX2D_SAMPLER(tex, samp, coord)  SAMPLE_TEXTURE2D(tex, sampler##samp, coord)

      #if defined(UNITY_COMPILER_HLSL)
         #define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
      #else
         #define UNITY_INITIALIZE_OUTPUT(type,name)
      #endif

      #define sampler2D_float sampler2D
      #define sampler2D_half sampler2D

      

      // data across stages, stripped like the above.
      struct VertexToPixel
      {
         float4 pos : SV_POSITION;
         float3 worldPos : TEXCOORD0;
         float3 worldNormal : TEXCOORD1;
         float4 worldTangent : TEXCOORD2;
          float4 texcoord0 : TEXCCOORD3;
         #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
             float4 texcoord1 : TEXCCOORD4;
            // float4 texcoord2 : TEXCCOORD5;
         #endif
         // float4 texcoord3 : TEXCCOORD6;
         // float4 screenPos : TEXCOORD7;
         // float4 vertexColor : COLOR;

         // float4 extraV2F0 : TEXCOORD13;
         // float4 extraV2F1 : TEXCOORD14;
         // float4 extraV2F2 : TEXCOORD15;
         // float4 extraV2F3 : TEXCOORD16;
         // float4 extraV2F4 : TEXCOORD17;
         // float4 extraV2F5 : TEXCOORD18;
         // float4 extraV2F6 : TEXCOORD19;
         // float4 extraV2F7 : TEXCOORD20;

         
         #if defined(LIGHTMAP_ON)
            float2 lightmapUV : TEXCOORD8;
         #endif
         #if defined(DYNAMICLIGHTMAP_ON)
            float2 dynamicLightmapUV : TEXCOORD9;
         #endif
         #if !defined(LIGHTMAP_ON)
            float3 sh : TEXCOORD10;
         #endif

         float4 fogFactorAndVertexLight : TEXCOORD11;
         float4 shadowCoord : TEXCOORD12;

         #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
         #endif
         #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
         #endif
         #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
         #endif
         #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
         #endif
      };



            // TEMPLATE_SHARED
            
            // data describing the user output of a pixel
            struct Surface
            {
               half3 Albedo;
               half Height;
               half3 Normal;
               half Smoothness;
               half3 Emission;
               half Metallic;
               half3 Specular;
               half Occlusion;
               half Alpha;
               // HDRP Only
               half SpecularOcclusion;
               half SubsurfaceMask;
               half Thickness;
               half CoatMask;
               half Anisotropy;
               half IridescenceMask;
               half IridescenceThickness;
            };

            // data the user might need, this will grow to be big. But easy to strip
            struct ShaderData
            {
               float3 localSpacePosition;
               float3 localSpaceNormal;
               float3 localSpaceTangent;
        
               float3 worldSpacePosition;
               float3 worldSpaceNormal;
               float3 worldSpaceTangent;

               float3 worldSpaceViewDir;
               float3 tangentSpaceViewDir;

               float4 texcoord0;
               float4 texcoord1;
               float4 texcoord2;
               float4 texcoord3;

               float2 screenUV;
               float4 screenPos;

               float4 vertexColor;

               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;

               float3x3 TBNMatrix;
            };

            struct VertexData
            {
               #if SHADER_TARGET > 30 && _PLANETCOMPUTE
 //              // uint vertexID : SV_VertexID;
               #endif
               float4 vertex : POSITION;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                  float4 tangent : TANGENT;
                  float4 texcoord1 : TEXCOORD1;
                  float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD4; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity    : TEXCOORD5; // Add Precomputed Velocity (Alembic computes velocities on runtime side).
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessVertex 
            {
               float4 vertex : INTERNALTESSPOS;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float4 tangent : TANGENT;
               float4 texcoord1 : TEXCOORD1;
               float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               // float4 extraV2F0 : TEXCOORD4;
               // float4 extraV2F1 : TEXCOORD5;
               // float4 extraV2F2 : TEXCOORD6;
               // float4 extraV2F3 : TEXCOORD7;
               // float4 extraV2F4 : TEXCOORD8;
               // float4 extraV2F5 : TEXCOORD9;
               // float4 extraV2F6 : TEXCOORD10;
               // float4 extraV2F7 : TEXCOORD11;

               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD12; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity : TEXCOORD13;
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
               UNITY_VERTEX_OUTPUT_STEREO
            };

            struct ExtraV2F
            {
               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;
            };


            float3 WorldToTangentSpace(ShaderData d, float3 normal)
            {
               return mul(d.TBNMatrix, normal);
            }

            float3 TangentToWorldSpace(ShaderData d, float3 normal)
            {
               return mul(normal, d.TBNMatrix);
            }

            // in this case, make standard more like SRPs, because we can't fix
            // GetWorldToObjectMatrix() in HDRP, since it already does macro-fu there

            #if _STANDARD
               float3 TransformWorldToObject(float3 p) { return mul(GetWorldToObjectMatrix(), float4(p, 1)); };
               float3 TransformObjectToWorld(float3 p) { return mul(GetObjectToWorldMatrix(), float4(p, 1)); };
               float4 TransformWorldToObject(float4 p) { return mul(GetWorldToObjectMatrix(), p); };
               float4 TransformObjectToWorld(float4 p) { return mul(GetObjectToWorldMatrix(), p); };
               float4x4 GetWorldToObjectMatrix() { return GetWorldToObjectMatrix(); }
               float4x4 GetObjectToWorldMatrix() { return GetObjectToWorldMatrix(); }
            #endif

            float3 GetCameraWorldPosition()
            {
               #if _HDRP
                  return GetCameraRelativePositionWS(_WorldSpaceCameraPos);
               #else
                  return _WorldSpaceCameraPos;
               #endif
            }

            
             #if _HDRP

               half3 UnpackNormalmapRGorAG(half4 packednormal)
               {
                     // This do the trick
                  packednormal.x *= packednormal.w;

                  half3 normal;
                  normal.xy = packednormal.xy * 2 - 1;
                  normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                  return normal;
               }
               half3 UnpackNormal(half4 packednormal)
               {
                  #if defined(UNITY_NO_DXT5nm)
                     return packednormal.xyz * 2 - 1;
                  #else
                     return UnpackNormalmapRGorAG(packednormal);
                  #endif
               }
               #endif
               #if _HDRP || _URP

               half3 UnpackScaleNormal(half4 packednormal, half scale)
               {
                 #ifndef UNITY_NO_DXT5nm
                   // Unpack normal as DXT5nm (1, y, 1, x) or BC5 (x, y, 0, 1)
                   // Note neutral texture like "bump" is (0, 0, 1, 1) to work with both plain RGB normal and DXT5nm/BC5
                   packednormal.x *= packednormal.w;
                 #endif
                   half3 normal;
                   normal.xy = (packednormal.xy * 2 - 1) * scale;
                   normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                   return normal;
               }  

             #endif


            void GetSun(out float3 lightDir, out float3 color)
            {
               lightDir = float3(0.5, 0.5, 0);
               color = 1;
               #if _HDRP
                  if (_DirectionalLightCount > 0)
                  {
                     DirectionalLightData light = _DirectionalLightDatas[0];
                     lightDir = -light.forward.xyz;
                     color = light.color;
                  }
               #elif _STANDARD
                  lightDir = normalize(_WorldSpaceLightPos0.xyz);
                  color = _LightColor0.rgb;
               #elif _URP
                  Light light = GetMainLight();
                  lightDir = light.direction;
                  color = light.color;
               #endif
            }

     



            
            CBUFFER_START(UnityPerMaterial)

               

      #if _MESHSUBARRAY
         half4 _MeshSubArrayIndexes;
      #endif

      float4 _Diffuse_TexelSize;
      float4 _NormalSAO_TexelSize;

      #if _HYBRIDHEIGHTBLEND
         float _HybridHeightBlendDistance;
      #endif

      #if _PACKINGHQ
         float4 _SmoothAO_TexelSize;
      #endif

      #ifdef _ALPHATEST_ON
      float4 _TerrainHolesTexture_TexelSize;
      #endif

      #if _USESPECULARWORKFLOW
         float4 _Specular_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         float4 _EmissiveMetal_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         half _EmissiveMult;
      #endif

      #if _AUTONORMAL
         half _AutoNormalHeightScale;
      #endif

      float4 _UVScale; // scale and offset

      half _Contrast;
      
      

       #if _VSSHADOWMAP
         float4 gVSSunDirection;
      #endif

      #if _FORCELOCALSPACE && _PLANETVECTORS
         float4x4 _PQSToLocal;
      #endif

      #if _ORIGINSHIFT
         float4x4 _GlobalOriginMTX;
      #endif

      float4 _Control0_TexelSize;
      #if _CUSTOMSPLATTEXTURES
         float4 _CustomControl0_TexelSize;
      #endif
      float4 _PerPixelNormal_TexelSize;

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         float2 _NoiseUVParams;
      #endif

      float4 _PerTexProps_TexelSize;

      #if _SURFACENORMALS  
         float3 surfTangent;
         float3 surfBitangent;
         float3 surfNormal;
      #endif



            CBUFFER_END

            

            

                  

      #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, data.TBN)
      

      



   
// In Unity 2020.3LTS, Unity will spew tons of errors about missing this sampler in
// URP, even though it shouldn't be required.
TEXTURE2D(_MainTex);

      // globals, outside of CBuffer, but used by more than one module
      float3 _gGlitterLightDir;
      float3 _gGlitterLightWorldPos;
      half3 _gGlitterLightColor;

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
         float4    _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
         float4    _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
         float4    _TerrainNormalmapTexture_TexelSize;
      #endif

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
          TEXTURE2D(_TerrainHeightmapTexture);
          TEXTURE2D(_TerrainNormalmapTexture);
      #endif

      UNITY_INSTANCING_BUFFER_START(Terrain)
          UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
      UNITY_INSTANCING_BUFFER_END(Terrain)          


      

      // dynamic branching helpers, for regular and aggressive branching
      // debug mode shows how many samples using branching will save us. 
      //
      // These macros are always used instead of the UNITY_BRANCH macro
      // to maintain debug displays and allow branching to be disabled
      // on as granular level as we want. 
      
      #if _BRANCHSAMPLES
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++; if (w > 0)
         #else
            #define MSBRANCH(w) UNITY_BRANCH if (w > 0)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++;
         #else
            #define MSBRANCH(w) 
         #endif
      #endif
      
      #if _BRANCHSAMPLESAGR
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER ||_DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++; if (w > 0.001)
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++; if (w > 0.001)
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++; if (w > 0.001)
         #else
            #define MSBRANCHTRIPLANAR(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHCLUSTER(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHOTHER(w) UNITY_BRANCH if (w > 0.001)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++;
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++;
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++;
         #else
            #define MSBRANCHTRIPLANAR(w)
            #define MSBRANCHCLUSTER(w)
            #define MSBRANCHOTHER(w)
         #endif
      #endif

      #if _DEBUG_SAMPLECOUNT
         int _sampleCount;
         #define COUNTSAMPLE { _sampleCount++; }
      #else
         #define COUNTSAMPLE
      #endif

      #if _DEBUG_PROCLAYERS
         int _procLayerCount;
         #define COUNTPROCLAYER { _procLayerCount++; }
      #else
         #define COUNTPROCLAYER
      #endif


      #if _DEBUG_USE_TOPOLOGY
         TEXTURE2D(_DebugWorldPos);
         TEXTURE2D(_DebugWorldNormal);
      #endif
      

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         TEXTURE2D(_NoiseUV);
      #endif

      #if _PACKINGHQ
         UNITY_DECLARE_TEX2DARRAY(_SmoothAO);
      #endif

      #if _USESPECULARWORKFLOW
         UNITY_DECLARE_TEX2DARRAY(_Specular);
      #endif

      #if _USEEMISSIVEMETAL
         UNITY_DECLARE_TEX2DARRAY(_EmissiveMetal);
      #endif

      TEXTURE2D(_PerPixelNormal);

      SamplerState shared_linear_clamp_sampler;
      SamplerState shared_point_clamp_sampler;
      
      TEXTURE2D(_Control0);
      #if _CUSTOMSPLATTEXTURES
         TEXTURE2D(_CustomControl0);
         #if !_MAX4TEXTURES
         TEXTURE2D(_CustomControl1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_CustomControl2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_CustomControl3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_CustomControl7);
         #endif
      #else
         #if !_MAX4TEXTURES
         TEXTURE2D(_Control1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_Control2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_Control3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_Control7);
         #endif
      #endif

      TEXTURE2D_FLOAT(_PerTexProps);
   
      struct DecalLayer
      {
         float3 uv;
         float2 dx;
         float2 dy;
         int decalIndex;
         bool dynamic; 
      };

      struct DecalOutput
      {
         DecalLayer l0;
         DecalLayer l1;
         DecalLayer l2;
         DecalLayer l3;
         
         half4 Weights;
         half4 Indexes;
         half4 fxLevels;
         
      };
      

      struct TriGradMipFormat
      {
         float4 d0;
         float4 d1;
         float4 d2;
      };

      float InverseLerp(float x, float y, float v) { return (v-x)/max(y-x, 0.001); }
      float2 InverseLerp(float2 x, float2 y, float2 v) { return (v-x)/max(y-x, float2(0.001, 0.001)); }
      float3 InverseLerp(float3 x, float3 y, float3 v) { return (v-x)/max(y-x, float3(0.001, 0.001, 0.001)); }
      float4 InverseLerp(float4 x, float4 y, float4 v) { return (v-x)/max(y-x, float4(0.001, 0.001, 0.001, 0.001)); }
      

      // 2019.3 holes
      #ifdef _ALPHATEST_ON
          TEXTURE2D(_TerrainHolesTexture);
          
          void ClipHoles(float2 uv)
          {
              float hole = SAMPLE_TEXTURE2D(_TerrainHolesTexture, shared_linear_clamp_sampler, uv).r;
              COUNTSAMPLE
              clip(hole < 0.5f ? -1 : 1);
          }
      #endif

      
      #if _TRIPLANAR
         #if _USEGRADMIP
            #define MIPFORMAT TriGradMipFormat
            #define INITMIPFORMAT (TriGradMipFormat)0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float3
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float3
         #endif
      #else
         #if _USEGRADMIP
            #define MIPFORMAT float4
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float
         #endif
      #endif

      float2 TotalOne(float2 v) { return v * (1.0 / max(v.x + v.y, 0.001)); }
      float3 TotalOne(float3 v) { return v * (1.0 / max(v.x + v.y + v.z, 0.001)); }
      float4 TotalOne(float4 v) { return v * (1.0 / max(v.x + v.y + v.z + v.w, 0.001)); }

      float2 RotateUV(float2 uv, float amt)
      {
         uv -=0.5;
         float s = sin ( amt);
         float c = cos ( amt );
         float2x2 mtx = float2x2( c, -s, s, c);
         mtx *= 0.5;
         mtx += 0.5;
         mtx = mtx * 2-1;
         uv = mul ( uv, mtx );
         uv += 0.5;
         return uv;
      }

      float4 DecodeToFloat4(float v)
      {
         uint vi = (uint)(v * (256.0f * 256.0f * 256.0f * 256.0f));
         int ex = (int)(vi / (256 * 256 * 256) % 256);
         int ey = (int)((vi / (256 * 256)) % 256);
         int ez = (int)((vi / (256)) % 256);
         int ew = (int)(vi % 256);
         float4 e = float4(ex / 255.0, ey / 255.0, ez / 255.0, ew / 255.0);
         return e;
      }

      

      struct Input 
      {
         ShaderData shaderData;
         float2 uv_Control0;
         float2 uv2_Diffuse;

         float worldHeight;
         float3 worldUpVector;

         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         float4 color;
         float3x3 TBN;

         // vertex/digger workflow data
         half4 w0;
         half4 w1;
         half4 w2;
         half4 w3;
         half4 w4;
         half4 w5;
         half4 w6;
         
         // megasplat data
         half4 layer0;
         half4 layer1;
         half3 baryWeights;
         half4 scatter0;
         half4 scatter1;

         // wetness, puddles, streams, lava from vertex or megasplat
         half4 fx;
         // snow min, snow max
         half4 fx2;


      };
      
      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
         half3 pN0;
         half3 pN1;
         half3 pN2;
         half3 pN3;
         half3 axisSign;
         Input IN;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;

         half4 cluster0;
         half4 cluster1;
         half4 cluster2;
         half4 cluster3;

      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
         #if _USESPECULARWORKFLOW
         half3 Specular;
         #endif
         half Alpha;
         
      };


      

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         #if _SURFACENORMALS
            half3 surf0;
            half3 surf1;
            half3 surf2;
            half3 surf3;
         #endif

         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
         

         #if _USEEMISSIVEMETAL || _GLOBALEMIS || _GLOBALSMOOTHAOMETAL || _PERTEXSSS || _PERTEXRIMLIGHT
            half4 emisMetal0;
            half4 emisMetal1;
            half4 emisMetal2;
            half4 emisMetal3;
         #endif

         #if _USESPECULARWORKFLOW
            half3 specular0;
            half3 specular1;
            half3 specular2;
            half3 specular3;
         #endif
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
         #if _SURFACENORMALS
            s.surf0 = half3(0,0,1);
            s.surf1 = half3(0,0,1);
            s.surf2 = half3(0,0,1);
            s.surf3 = half3(0,0,1);
         #endif
      }

       float3 GetGlobalLightDir(Input i)
      {
         float3 lightDir = float3(1,0,0);

         #if _HDRP || PASS_DEFERRED
            lightDir = normalize(_gGlitterLightDir.xyz);
         #elif _URP
            lightDir = GetMainLight().direction;
         #else
            #ifndef USING_DIRECTIONAL_LIGHT
               lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            #else
               lightDir = normalize(_WorldSpaceLightPos0.xyz);
            #endif
         #endif
         return lightDir;
      }

      float3x3 GetTBN(Input i)
      {
         return i.TBN;
      }
      
      float3 GetGlobalLightDirTS(Input i)
      {
         float3 lightDirWS = GetGlobalLightDir(i);
         return mul(GetTBN(i), lightDirWS);
      }
      
      half3 GetGlobalLightColor()
      {
         #if _HDRP || PASS_DEFERRED
            return _gGlitterLightColor;
         #elif _URP
            return (GetMainLight().color);
         #else
            return _LightColor0.rgb;
         #endif
      }

      

      half3 FuzzyShade(half3 color, half3 normal, half coreMult, half edgeMult, half power, float3 viewDir)
      {
         half dt = saturate(dot(viewDir, normal));
         half dark = 1.0 - (coreMult * dt);
         half edge = pow(1-dt, power) * edgeMult;
         return color * (dark + edge);
      }

      half3 ComputeSSS(Input i, float3 V, float3 N, half3 tint, half thickness, half distortion, half scale, half power)
      {
         float3 L = GetGlobalLightDir(i);
         half3 lightColor = GetGlobalLightColor();
         float3 H = normalize(L + N * distortion);
         float VdotH = pow(saturate(dot(V, -H)), power) * scale;
         float3 I =  (VdotH) * thickness;
         return lightColor * I * tint;
      }


      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif
      

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##3 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv3.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #endif

      half2 BlendNormal2(half2 base, half2 blend) { return normalize(half3(base.xy + blend.xy, 1)).xy; } 
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }
      half3 BlendLighterColor(half3 s, half3 d) { return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d; } 
      
      
      #if _SURFACENORMALS  

      #define HALF_EPS 4.8828125e-4    // 2^-11, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)

      

      void ConstructSurfaceGradientTBN(Input i)
      {
         float3x3 tbn = GetTBN(i);
         float3 t = tbn[0];
         float3 b = tbn[1];
         float3 n = tbn[2];

         surfNormal = n;//mul(GetWorldToObjectMatrix(), float4(n, 1)).xyz;
         surfTangent = t;//mul(GetWorldToObjectMatrix(), float4(t, 1)).xyz;
         surfBitangent = b;//cross(surfNormal, surfTangent);
         
         float renormFactor = 1.0 / length(surfNormal);
         surfNormal    *= renormFactor;
         surfTangent   *= renormFactor;
         surfBitangent *= renormFactor;
      }
      
      half3 SurfaceGradientFromTBN(half2 deriv)
      {
          return deriv.x * surfTangent + deriv.y * surfBitangent;
      }

      // Input: vM is tangent space normal in [-1;1].
      // Output: convert vM to a derivative.
      half2 TspaceNormalToDerivative(half3 vM)
      {
         const half scale = 1.0/128.0;
         
         // Ensure vM delivers a positive third component using abs() and
         // constrain vM.z so the range of the derivative is [-128; 128].
         const half3 vMa = abs(vM);
         const half z_ma = max(vMa.z, scale*max(vMa.x, vMa.y));

         return -half2(vM.x, vM.y)/z_ma;
      }

      // Used to produce a surface gradient from the gradient of a volume
      // bump function such as 3D Perlin noise. Equation 2 in [Mik10].
      half3 SurfgradFromVolumeGradient(half3 grad)
      {
         return grad - dot(surfNormal, grad) * surfNormal;
      }

      half3 SurfgradFromTriplanarProjection(half3 pN, half2 xPlaneTN, half2 yPlaneTN, half2 zPlaneTN)
      {
         const half w0 = pN.x;
         const half w1 = pN.y;
         const half w2 = pN.z;
         
         // X-plane tangent normal to gradient derivative
         xPlaneTN = xPlaneTN * 2.0 - 1.0;
         half xPlaneRcpZ = rsqrt(max(1 - dot(xPlaneTN.x, xPlaneTN.x) - dot(xPlaneTN.y, xPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_xplane = xPlaneTN * -xPlaneRcpZ;

         // Y-plane tangent normal to gradient derivative
         yPlaneTN = yPlaneTN * 2.0 - 1.0;
         half yPlaneRcpZ = rsqrt(max(1 - dot(yPlaneTN.x, yPlaneTN.x) - dot(yPlaneTN.y, yPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_yplane = yPlaneTN * -yPlaneRcpZ;

         // Z-plane tangent normal to gradient derivative
         zPlaneTN = zPlaneTN * 2.0 - 1.0;
         half zPlaneRcpZ = rsqrt(max(1 - dot(zPlaneTN.x, zPlaneTN.x) - dot(zPlaneTN.y, zPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_zplane = zPlaneTN * -zPlaneRcpZ;

         // Assume deriv xplane, deriv yplane, and deriv zplane are
         // sampled using (z,y), (x,z), and (x,y), respectively.
         // Positive scales of the lookup coordinate will work
         // as well, but for negative scales the derivative components
         // will need to be negated accordingly.
         float3 grad = float3(w2*d_zplane.x + w1*d_yplane.x,
                              w2*d_zplane.y + w0*d_xplane.y,
                              w0*d_xplane.x + w1*d_yplane.y);

         return SurfgradFromVolumeGradient(grad);
      }

      half3 ConvertNormalToGradient(half3 normal)
      {
         half2 deriv = TspaceNormalToDerivative(normal);

         return SurfaceGradientFromTBN(deriv);
      }

      half3 ConvertNormal2ToGradient(half2 packedNormal)
      {
         half2 tNormal = packedNormal;
         half rcpZ = rsqrt(max(1 - dot(tNormal.x, tNormal.x) - dot(tNormal.y, tNormal.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
         half2 deriv = tNormal * -rcpZ;
         return SurfaceGradientFromTBN(deriv);
      }


      half3 ResolveNormalFromSurfaceGradient(half3 gradient)
      {
         return normalize(surfNormal - gradient);
      }
      

      #endif // _SURFACENORMALS

      void BlendNormalPerTex(inout RawSamples o, half2 noise, float4 fades)
      {
         #if _SURFACENORMALS
            float3 grad = ConvertNormal2ToGradient(noise.xy);
            o.surf0 += grad * fades.x;
            o.surf1 += grad * fades.y;
            #if !_MAX2LAYER
               o.surf2 += grad * fades.z;
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.surf3 += grad * fades.w;
            #endif
         #else
            o.normSAO0.xy = lerp(o.normSAO0.xy, BlendNormal2(o.normSAO0.xy, noise.xy), fades.x);
            o.normSAO1.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #if !_MAX2LAYER
               o.normSAO2.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO2.xy, noise.xy), fades.y);
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.normSAO3.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #endif
         #endif
      }
      
     
      
      half3 BlendNormal3(half3 n1, half3 n2)
      {
         n1 += float3( 0,  0, 1);
         n2 *= float3(-1, -1, 1);
         return n1*dot(n1, n2) / n1.z - n2;
      }
      
      half2 TransformTriplanarNormal(Input IN, float3x3 t2w, half3 axisSign, half3 absVertNormal,
               half3 pN, half2 a0, half2 a1, half2 a2)
      {
         
         a0 = a0 * 2 - 1;
         a1 = a1 * 2 - 1;
         a2 = a2 * 2 - 1;

         a0.x *= axisSign.x;
         a1.x *= axisSign.y;
         a2.x *= axisSign.z;
         
         half3 n0 = half3(a0.xy, 1);
         half3 n1 = half3(a1.xy, 1);
         half3 n2 = half3(a2.xy, 1);

         float3 wn = IN.worldNormal;

         n0 = BlendNormal3(half3(wn.zy, absVertNormal.x), n0);
         n1 = BlendNormal3(half3(wn.xz, absVertNormal.y), n1 * float3(-1, 1, 1)); 
         n2 = BlendNormal3(half3(wn.xy, absVertNormal.z), n2);
  
         n0.z *= axisSign.x;
         n1.z *= axisSign.y;
         n2.z *= -axisSign.z;

         half3 worldNormal = (n0.zyx * pN.x + n1.xzy * pN.y + n2.xyz * pN.z);
         return mul(t2w, worldNormal).xy;

      }
      
      // funcs
      
      inline half MSLuminance(half3 rgb)
      {
         #ifdef UNITY_COLORSPACE_GAMMA
            return dot(rgb, half3(0.22, 0.707, 0.071));
         #else
            return dot(rgb, half3(0.0396819152, 0.458021790, 0.00609653955));
         #endif
      }
      
      
      float2 Hash2D( float2 x )
      {
          float2 k = float2( 0.3183099, 0.3678794 );
          x = x*k + k.yx;
          return -1.0 + 2.0*frac( 16.0 * k*frac( x.x*x.y*(x.x+x.y)) );
      }

      float Noise2D(float2 p )
      {
         float2 i = floor( p );
         float2 f = frac( p );
         
         float2 u = f*f*(3.0-2.0*f);

         return lerp( lerp( dot( Hash2D( i + float2(0.0,0.0) ), f - float2(0.0,0.0) ), 
                           dot( Hash2D( i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                      lerp( dot( Hash2D( i + float2(0.0,1.0) ), f - float2(0.0,1.0) ), 
                           dot( Hash2D( i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
      }
      
      float FBM2D(float2 uv)
      {
         float f = 0.5000*Noise2D( uv ); uv *= 2.01;
         f += 0.2500*Noise2D( uv ); uv *= 1.96;
         f += 0.1250*Noise2D( uv ); 
         return f;
      }
      
      float3 Hash3D( float3 p )
      {
         p = float3( dot(p,float3(127.1,311.7, 74.7)),
                 dot(p,float3(269.5,183.3,246.1)),
                 dot(p,float3(113.5,271.9,124.6)));

         return -1.0 + 2.0*frac(sin(p)*437.5453123);
      }

      float Noise3D( float3 p )
      {
         float3 i = floor( p );
         float3 f = frac( p );
         
         float3 u = f*f*(3.0-2.0*f);

         return lerp( lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,0.0) ), f - float3(0.0,0.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,0.0) ), f - float3(1.0,0.0,0.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,0.0) ), f - float3(0.0,1.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,0.0) ), f - float3(1.0,1.0,0.0) ), u.x), u.y),
                      lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,1.0) ), f - float3(0.0,0.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,1.0) ), f - float3(1.0,0.0,1.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,1.0) ), f - float3(0.0,1.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,1.0) ), f - float3(1.0,1.0,1.0) ), u.x), u.y), u.z );
      }
      
      float FBM3D(float3 uv)
      {
         float f = 0.5000*Noise3D( uv ); uv *= 2.01;
         f += 0.2500*Noise3D( uv ); uv *= 1.96;
         f += 0.1250*Noise3D( uv ); 
         return f;
      }
      
     
      
      float GetSaturation(float3 c)
      {
         float mi = min(min(c.x, c.y), c.z);
         float ma = max(max(c.x, c.y), c.z);
         return (ma - mi)/(ma + 1e-7);
      }

      // Better Color Lerp, does not have darkening issue
      float3 BetterColorLerp(float3 a, float3 b, float x)
      {
         float3 ic = lerp(a, b, x) + float3(1e-6,0.0,0.0);
         float sd = abs(GetSaturation(ic) - lerp(GetSaturation(a), GetSaturation(b), x));
    
         float3 dir = normalize(float3(2.0 * ic.x - ic.y - ic.z, 2.0 * ic.y - ic.x - ic.z, 2.0 * ic.z - ic.y - ic.x));
         float lgt = dot(float3(1.0, 1.0, 1.0), ic);
    
         float ff = dot(dir, normalize(ic));
    
         const float dsp_str = 1.5;
         ic += dsp_str * dir * sd * ff * lgt;
         return saturate(ic);
      }
      
      
      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);
             half4 weights = half4(iWeights.x * max(h0,0.001), iWeights.y * max(h1,0.001), iWeights.z * max(h2,0.001), iWeights.w * max(h3,0.001));
             
             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);

             weights = TotalOne(weights);
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #elif _MAX20TEXTURES
         #define TEXCOUNT 20
      #elif _MAX24TEXTURES
         #define TEXCOUNT 24
      #elif _MAX28TEXTURES
         #define TEXCOUNT 28
      #elif _MAX32TEXTURES
         #define TEXCOUNT 32
      #else
         #define TEXCOUNT 16
      #endif

      #if _DECAL_SPLAT
      
      void DoMergeDecalSplats(half4 iWeights, half4 iIndexes, inout half4 indexes, inout half4 weights)
      {
         for (int i = 0; i < 4; ++i)
         {
            half w = iWeights[i];
            half index = iIndexes[i];
            if (w > weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = index;
            }
            else if (w > weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = index;
            }
            else if (w > weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = index;
            }
            else if (w > weights[3])
            {
               weights[3] = w;
               indexes[3] = index;
            }
         }

      }
      #endif


      void Setup(out half4 weights, float2 uv, out Config config, half4 w0, half4 w1, half4 w2, half4 w3, half4 w4, half4 w5, half4 w6, half4 w7, float3 worldPos, DecalOutput decalOutput)
      {
         config = (Config)0;
         half4 indexes = 0;

         config.uv = uv;

         #if _WORLDUV
         uv = worldPos.xz;
         #endif

         #if _DISABLESPLATMAPS
            float2 scaledUV = uv;
         #else
            float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;
         #endif

         // if only 4 textures, and blending 4 textures, skip this whole thing..
         // this saves about 25% of the ALU of the base shader on low end. However if
         // we rely on sorted texture weights (distance resampling) we have to sort..
         float4 defaultIndexes = float4(0,1,2,3);
         #if _MESHSUBARRAY
            defaultIndexes = _MeshSubArrayIndexes;
         #endif

         #if _MESHSUBARRAY && !_DECAL_SPLAT || (_MAX4TEXTURES && !_MAX3LAYER && !_MAX2LAYER && !_DISTANCERESAMPLE && !_POM && !_DECAL_SPLAT)
            weights = w0;
            config.uv0 = float3(scaledUV, defaultIndexes.x);
            config.uv1 = float3(scaledUV, defaultIndexes.y);
            config.uv2 = float3(scaledUV, defaultIndexes.z);
            config.uv3 = float3(scaledUV, defaultIndexes.w);
            return;
         #endif

         #if _DISABLESPLATMAPS
            weights = float4(1,0,0,0);
            return;
         #else
            half splats[TEXCOUNT];

            splats[0] = w0.x;
            splats[1] = w0.y;
            splats[2] = w0.z;
            splats[3] = w0.w;
            #if !_MAX4TEXTURES
               splats[4] = w1.x;
               splats[5] = w1.y;
               splats[6] = w1.z;
               splats[7] = w1.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES
               splats[8] = w2.x;
               splats[9] = w2.y;
               splats[10] = w2.z;
               splats[11] = w2.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
               splats[12] = w3.x;
               splats[13] = w3.y;
               splats[14] = w3.z;
               splats[15] = w3.w;
            #endif
            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[16] = w4.x;
               splats[17] = w4.y;
               splats[18] = w4.z;
               splats[19] = w4.w;
            #endif
            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[20] = w5.x;
               splats[21] = w5.y;
               splats[22] = w5.z;
               splats[23] = w5.w;
            #endif
            #if _MAX28TEXTURES || _MAX32TEXTURES
               splats[24] = w6.x;
               splats[25] = w6.y;
               splats[26] = w6.z;
               splats[27] = w6.w;
            #endif
            #if _MAX32TEXTURES
               splats[28] = w7.x;
               splats[29] = w7.y;
               splats[30] = w7.z;
               splats[31] = w7.w;
            #endif



            weights[0] = 0;
            weights[1] = 0;
            weights[2] = 0;
            weights[3] = 0;
            indexes[0] = 0;
            indexes[1] = 0;
            indexes[2] = 0;
            indexes[3] = 0;

            int i = 0;
            for (i = 0; i < TEXCOUNT; ++i)
            {
               half w = splats[i];
               if (w >= weights[0])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = weights[0];
                  indexes[1] = indexes[0];
                  weights[0] = w;
                  indexes[0] = i;
               }
               else if (w >= weights[1])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = w;
                  indexes[1] = i;
               }
               else if (w >= weights[2])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = w;
                  indexes[2] = i;
               }
               else if (w >= weights[3])
               {
                  weights[3] = w;
                  indexes[3] = i;
               }
            }

            #if _DECAL_SPLAT
               DoMergeDecalSplats(decalOutput.Weights, decalOutput.Indexes, weights, indexes); 
            #endif


            
            
            // clamp and renormalize
            #if _MAX2LAYER
               weights.zw = 0;
               weights.xy = TotalOne(weights.xy);
            #elif _MAX3LAYER
               weights.w = 0;
               weights.xyz = TotalOne(weights.xyz);
            #elif !_DISABLEHEIGHTBLENDING || _NORMALIZEWEIGHTS // prevents black when painting, which the unity shader does not prevent.
               weights = normalize(weights);
            #endif
            

            config.uv0 = float3(scaledUV, indexes.x);
            config.uv1 = float3(scaledUV, indexes.y);
            config.uv2 = float3(scaledUV, indexes.z);
            config.uv3 = float3(scaledUV, indexes.w);


         #endif //_DISABLESPLATMAPS


      }

      float3 HeightToNormal(float height, float3 worldPos)
      {
         float3 dx = ddx(worldPos);
         float3 dy = ddy(worldPos);
         float3 crossX = cross(float3(0,1,0), dx);
         float3 crossY = cross(float3(0,1,0), dy);
         float3 d = abs(dot(crossY, dx));
         float3 n = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
         n.z *= -1;
         return normalize((d * float3(0,1,0)) - n).xzy;
      }
      
      float ComputeMipLevel(float2 uv, float2 textureSize)
      {
         uv *= textureSize;
         float2  dx_vtc        = ddx(uv);
         float2  dy_vtc        = ddy(uv);
         float delta_max_sqr   = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
         return 0.5 * log2(delta_max_sqr);
      }

      inline half2 UnpackNormal2(half4 packednormal)
      {
          return packednormal.wy * 2 - 1;
         
      }

      half3 TriplanarHBlend(half h0, half h1, half h2, half3 pN, half contrast)
      {
         half3 blend = pN / dot(pN, half3(1,1,1));
         float3 heights = float3(h0, h1, h2) + (blend * 3.0);
         half height_start = max(max(heights.x, heights.y), heights.z) - contrast;
         half3 h = max(heights - height_start.xxx, half3(0,0,0));
         blend = h / dot(h, half3(1,1,1));
         return blend;
      }
      

      void ClearAllButAlbedo(inout MicroSplatLayer o, half3 display)
      {
         o.Albedo = display.rgb;
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

      void ClearAllButAlbedo(inout MicroSplatLayer o, half display)
      {
         o.Albedo = half3(display, display, display);
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

     

      half MicroShadow(float3 lightDir, half3 normal, half ao, half strength)
      {
         half shadow = saturate(abs(dot(normal, lightDir)) + (ao * ao * 2.0) - 1.0);
         return 1 - ((1-shadow) * strength);
      }
      

      void DoDebugOutput(inout MicroSplatLayer l)
      {
         #if _DEBUG_OUTPUT_ALBEDO
            ClearAllButAlbedo(l, l.Albedo);
         #elif _DEBUG_OUTPUT_NORMAL
            // oh unit shader compiler normal stripping, how I hate you so..
            // must multiply by albedo to stop the normal from being white. Why, fuck knows?
            ClearAllButAlbedo(l, float3(l.Normal.xy * 0.5 + 0.5, l.Normal.z * saturate(l.Albedo.z+1)));
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            ClearAllButAlbedo(l, l.Smoothness.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_METAL
            ClearAllButAlbedo(l, l.Metallic.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_AO
            ClearAllButAlbedo(l, l.Occlusion.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_EMISSION
            ClearAllButAlbedo(l, l.Emission * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_HEIGHT
            ClearAllButAlbedo(l, l.Height.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_SPECULAR && _USESPECULARWORKFLOW
            ClearAllButAlbedo(l, l.Specular * saturate(l.Albedo.z+1));
         #elif _DEBUG_BRANCHCOUNT_WEIGHT
            ClearAllButAlbedo(l, _branchWeightCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TRIPLANAR
            ClearAllButAlbedo(l, _branchTriplanarCount / 24 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_CLUSTER
            ClearAllButAlbedo(l, _branchClusterCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_OTHER
            ClearAllButAlbedo(l, _branchOtherCount / 8 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TOTAL
            l.Albedo.r = _branchWeightCount / 12;
            l.Albedo.g = _branchTriplanarCount / 24;
            l.Albedo.b = _branchClusterCount / 12;
            ClearAllButAlbedo(l, (l.Albedo.r + l.Albedo.g + l.Albedo.b + (_branchOtherCount / 8)) / 4); 
         #elif _DEBUG_OUTPUT_MICROSHADOWS
            ClearAllButAlbedo(l,l.Albedo); 
         #elif _DEBUG_SAMPLECOUNT
            float sdisp = (float)_sampleCount / max(_SampleCountDiv, 1);
            half3 sdcolor = float3(sdisp, sdisp > 1 ? 1 : 0, 0);
            ClearAllButAlbedo(l, sdcolor * saturate(l.Albedo.z + 1));
         #elif _DEBUG_PROCLAYERS
            ClearAllButAlbedo(l, (float)_procLayerCount / (float)_PCLayerCount * saturate(l.Albedo.z + 1));
         #endif
      }


      // abstraction around sampler mode
      #if _USELODMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_LOD(tex, sampler##tex, u, l.x)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u, l.x)
      #elif _USEGRADMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_GRAD(tex, sampler##tex, u, l.xy, l.zw)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY_GRAD(tex, ss, u.xy, u.z, l.xy, l.zw)
      #else
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, u.xy, u.z)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u.xy, y.z)
      #endif


      #define MICROSPLAT_SAMPLE_DIFFUSE(u, cl, l) MICROSPLAT_SAMPLE(_Diffuse, u, l)
      #define MICROSPLAT_SAMPLE_EMIS(u, cl, l) MICROSPLAT_SAMPLE(_EmissiveMetal, u, l)
      #define MICROSPLAT_SAMPLE_DIFFUSE_LOD(u, cl, l) UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, u, l)
      

      #if _PACKINGHQ
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) half4(MICROSPLAT_SAMPLE(_NormalSAO, u, l).ga, MICROSPLAT_SAMPLE(_SmoothAO, u, l).ga).brag
      #else
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) MICROSPLAT_SAMPLE(_NormalSAO, u, l)
      #endif

      #if _USESPECULARWORKFLOW
         #define MICROSPLAT_SAMPLE_SPECULAR(u, cl, l) MICROSPLAT_SAMPLE(_Specular, u, l)
      #endif
      
      struct SimpleTriplanarConfig
      {
         float3 pn;
         float2 uv0;
         float2 uv1;
         float2 uv2;
      };
         
      void PrepSimpleTriplanarConfig(inout SimpleTriplanarConfig tc, float3 worldPos, float3 normal, float contrast)
      {
         tc.pn = pow(abs(normal), contrast);
         tc.pn = tc.pn / (tc.pn.x + tc.pn.y + tc.pn.z);
         
         half3 axisSign = sign(normal);

         tc.uv0 = worldPos.zy * axisSign.x;
         tc.uv1 = worldPos.xz * axisSign.y;
         tc.uv2 = worldPos.xy * axisSign.z;
      }
      
      #define SimpleTriplanarSample(tex, tc, scale) (SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv0 * scale) * tc.pn.x + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv1 * scale) * tc.pn.y + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv2 * scale) * tc.pn.z)
      #define SimpleTriplanarSampleLOD(tex, tc, scale, lod) (SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv0 * scale, lod) * tc.pn.x + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv1 * scale, lod) * tc.pn.y + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv2 * scale, lod) * tc.pn.z)
      #define SimpleTriplanarSampleGrad(tex, tc, scale) (SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv0 * scale, ddx(tc.uv0) * scale, ddy(tc.uv0) * scale) * tc.pn.x + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv1 * scale, ddx(tc.uv1) * scale, ddy(tc.uv1) * scale) * tc.pn.y + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv2 * scale, ddx(tc.uv2) * scale, ddy(tc.uv2) * scale) * tc.pn.z)
   
      
      inline half3 MicroSplatDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
      {
          specColor = lerp (half3(0,0,0), albedo, metallic);
          oneMinusReflectivity = (1-metallic);
          return albedo * oneMinusReflectivity;
      }







      Input DescToInput(ShaderData IN)
      {
        Input s = (Input)0;
        s.shaderData = IN;
        s.TBN = IN.TBNMatrix;
        s.worldNormal = IN.worldSpaceNormal;
        s.worldPos = IN.worldSpacePosition;
        s.viewDir = IN.tangentSpaceViewDir;
        s.uv_Control0 = IN.texcoord0.xy;

        s.worldUpVector = float3(0,1,0);
        s.worldHeight = IN.worldSpacePosition.y;
  
        #if _PLANETVECTORS
            float3 rwp = mul(_PQSToLocal, float4(IN.worldSpacePosition, 1));
            s.worldHeight = distance(rwp, float3(0,0,0));
            s.worldUpVector = normalize(rwp);
        #endif

        #if _MICROMESH && _MESHUV2
            s.uv2_Diffuse = IN.texcoord1.xy;
        #endif

        #if _MEGASPLAT
           UnpackMegaSplat(s, IN);
        #endif
   
        #if _MICROVERTEXMESH || _MICRODIGGERMESH
            UnpackVertexWorkflow(s, IN);
        #endif

        #if _PLANETVECTORS
           DoPlanetDataInputCopy(s, IN);
        #endif
        
        return s;
     }
     


      void SampleAlbedo(inout Config config, inout TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
         
            half4 contrasts = _Contrast.xxxx;
            #if _PERTEXTRIPLANARCONTRAST
               SAMPLE_PER_TEX(ptc, 9.5, config, half4(1,0.5,0,0));
               contrasts = half4(ptc0.y, ptc1.y, ptc2.y, ptc3.y);
            #endif


            #if _PERTEXTRIPLANAR
               SAMPLE_PER_TEX(pttri, 9.5, config, half4(0,0,0,0));
            #endif

            {
               // For per-texture triplanar, we modify the view based blending factor of the triplanar
               // such that you get a pure blend of either top down projection, or with the top down projection
               // removed and renormalized. This causes dynamic flow control optimizations to kick in and avoid
               // the extra texture samples while keeping the code simple. Yay..

               // We also only have to do this in the Albedo, because the pN values will be adjusted after the
               // albedo is sampled, causing future samples to use this data. 
              
               #if _PERTEXTRIPLANAR
                  if (pttri0.x > 0.66)
                  {
                     tc.pN0 = half3(0,1,0);
                  }
                  else if (pttri0.x > 0.33)
                  {
                     tc.pN0.y = 0;
                     tc.pN0.xz = TotalOne(tc.pN0.xz);
                  }
               #endif


               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[0], config.cluster0, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[1], config.cluster0, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[2], config.cluster0, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN0;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN0, contrasts.x);
                  tc.pN0 = bf;
               #endif

               s.albedo0 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            MSBRANCH(weights.y)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri1.x > 0.66)
                  {
                     tc.pN1 = half3(0,1,0);
                  }
                  else if (pttri1.x > 0.33)
                  {
                     tc.pN1.y = 0;
                     tc.pN1.xz = TotalOne(tc.pN1.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[0], config.cluster1, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[1], config.cluster1, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  COUNTSAMPLE
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[2], config.cluster1, d2);
               }
               half3 bf = tc.pN1;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN1, contrasts.x);
                  tc.pN1 = bf;
               #endif


               s.albedo1 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri2.x > 0.66)
                  {
                     tc.pN2 = half3(0,1,0);
                  }
                  else if (pttri2.x > 0.33)
                  {
                     tc.pN2.y = 0;
                     tc.pN2.xz = TotalOne(tc.pN2.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[0], config.cluster2, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[1], config.cluster2, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[2], config.cluster2, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN2;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN2, contrasts.x);
                  tc.pN2 = bf;
               #endif
               

               s.albedo2 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {

               #if _PERTEXTRIPLANAR
                  if (pttri3.x > 0.66)
                  {
                     tc.pN3 = half3(0,1,0);
                  }
                  else if (pttri3.x > 0.33)
                  {
                     tc.pN3.y = 0;
                     tc.pN3.xz = TotalOne(tc.pN3.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[0], config.cluster3, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[1], config.cluster3, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[2], config.cluster3, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN3;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN3, contrasts.x);
               tc.pN3 = bf;
               #endif

               s.albedo3 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif

         #else
            s.albedo0 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv0, config.cluster0, mipLevel);
            COUNTSAMPLE

            MSBRANCH(weights.y)
            {
               s.albedo1 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv1, config.cluster1, mipLevel);
               COUNTSAMPLE
            }
            #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.albedo2 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               } 
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.albedo3 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
            #endif
         #endif

         #if _PERTEXHEIGHTOFFSET || _PERTEXHEIGHTCONTRAST
            SAMPLE_PER_TEX(ptHeight, 10.5, config, 1);

            #if _PERTEXHEIGHTOFFSET
               s.albedo0.a = saturate(s.albedo0.a + ptHeight0.b - 1);
               s.albedo1.a = saturate(s.albedo1.a + ptHeight1.b - 1);
               s.albedo2.a = saturate(s.albedo2.a + ptHeight2.b - 1);
               s.albedo3.a = saturate(s.albedo3.a + ptHeight3.b - 1);
            #endif
            #if _PERTEXHEIGHTCONTRAST
               s.albedo0.a = saturate(pow(s.albedo0.a + 0.5, abs(ptHeight0.a)) - 0.5);
               s.albedo1.a = saturate(pow(s.albedo1.a + 0.5, abs(ptHeight1.a)) - 0.5);
               s.albedo2.a = saturate(pow(s.albedo2.a + 0.5, abs(ptHeight2.a)) - 0.5);
               s.albedo3.a = saturate(pow(s.albedo3.a + 0.5, abs(ptHeight3.a)) - 0.5);
            #endif
         #endif
      }
      
      
      
      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif

         #if _NONORMALMAP || _AUTONORMAL
            s.normSAO0 = half4(0,0, 0, 1);
            s.normSAO1 = half4(0,0, 0, 1);
            s.normSAO2 = half4(0,0, 0, 1);
            s.normSAO3 = half4(0,0, 0, 1);
            return;
         #endif

         
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
            
            half3 absVertNormal = abs(tc.IN.worldNormal);
            float3x3 t2w = tc.IN.TBN;
            
            
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[0], config.cluster0, d0).agrb;
                  COUNTSAMPLE
               }            
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[1], config.cluster0, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[2], config.cluster0, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf0 = SurfgradFromTriplanarProjection(tc.pN0, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO0.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN0, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO0.zw = a0.zw * tc.pN0.x + a1.zw * tc.pN0.y + a2.zw * tc.pN0.z;
            }
            MSBRANCH(weights.y)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[0], config.cluster1, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[1], config.cluster1, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[2], config.cluster1, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf1 = SurfgradFromTriplanarProjection(tc.pN1, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO1.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN1, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO1.zw = a0.zw * tc.pN1.x + a1.zw * tc.pN1.y + a2.zw * tc.pN1.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);

               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[0], config.cluster2, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[1], config.cluster2, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[2], config.cluster2, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf2 = SurfgradFromTriplanarProjection(tc.pN2, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO2.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN2, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO2.zw = a0.zw * tc.pN2.x + a1.zw * tc.pN2.y + a2.zw * tc.pN2.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[0], config.cluster3, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[1], config.cluster3, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[2], config.cluster3, d2).agrb;
                  COUNTSAMPLE
               }

               #if _SURFACENORMALS
                  s.surf3 = SurfgradFromTriplanarProjection(tc.pN3, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO3.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN3, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO3.zw = a0.zw * tc.pN3.x + a1.zw * tc.pN3.y + a2.zw * tc.pN3.z;
            }
            #endif

         #else
            s.normSAO0 = MICROSPLAT_SAMPLE_NORMAL(config.uv0, config.cluster0, mipLevel).agrb;
            COUNTSAMPLE
            s.normSAO0.xy = s.normSAO0.xy * 2 - 1;

            #if _SURFACENORMALS
               s.surf0 = ConvertNormal2ToGradient(s.normSAO0.xy);
            #endif

            MSBRANCH(weights.y)
            {
               s.normSAO1 = MICROSPLAT_SAMPLE_NORMAL(config.uv1, config.cluster1, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO1.xy = s.normSAO1.xy * 2 - 1;

               #if _SURFACENORMALS
                  s.surf1 = ConvertNormal2ToGradient(s.normSAO1.xy);
               #endif
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               s.normSAO2 = MICROSPLAT_SAMPLE_NORMAL(config.uv2, config.cluster2, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf2 = ConvertNormal2ToGradient(s.normSAO2.xy);
               #endif
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               s.normSAO3 = MICROSPLAT_SAMPLE_NORMAL(config.uv3, config.cluster3, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf3 = ConvertNormal2ToGradient(s.normSAO3.xy);
               #endif
            }
            #endif
         #endif
      }

      void SampleEmis(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USEEMISSIVEMETAL
            #if _TRIPLANAR
            
               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  s.emisMetal0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }

                  s.emisMetal1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.emisMetal0 = MICROSPLAT_SAMPLE_EMIS(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.emisMetal1 = MICROSPLAT_SAMPLE_EMIS(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
                  MSBRANCH(weights.z)
                  {
                     s.emisMetal2 = MICROSPLAT_SAMPLE_EMIS(config.uv2, config.cluster2, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  MSBRANCH(weights.w)
                  {
                     s.emisMetal3 = MICROSPLAT_SAMPLE_EMIS(config.uv3, config.cluster3, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
            #endif
         #endif
      }
      
      void SampleSpecular(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USESPECULARWORKFLOW
            #if _TRIPLANAR

               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.specular0 = MICROSPLAT_SAMPLE_SPECULAR(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.specular1 = MICROSPLAT_SAMPLE_SPECULAR(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.specular2 = MICROSPLAT_SAMPLE_SPECULAR(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.specular3 = MICROSPLAT_SAMPLE_SPECULAR(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
               #endif
            #endif
         #endif
      }

      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex, DecalOutput decalOutput)
      {
         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         half4 albedo = 0;
         half4 normSAO = half4(0,0,0,1);
         half3 surfGrad = half3(0,0,0);
         half4 emisMetal = 0;
         half3 specular = 0;
         
         float worldHeight = i.worldPos.y;
         float3 upVector = float3(0,1,0);
         
         #if _GLOBALTINT || _GLOBALNORMALS || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _GLOBALSPECULAR
            float globalSlopeFilter = 1;
            #if _GLOBALSLOPEFILTER
               float2 gfilterUV = float2(1 - saturate(dot(worldNormalVertex, upVector) * 0.5 + 0.49), 0.5);
               globalSlopeFilter = SAMPLE_TEXTURE2D(_GlobalSlopeTex, sampler_Diffuse, gfilterUV).a;
            #endif
         #endif

         // declare outside of branchy areas..
         half4 fxLevels = half4(0,0,0,0);
         half burnLevel = 0;
         half wetLevel = 0;
         half3 waterNormalFoam = half3(0, 0, 0);
         half porosity = 0.4;
         float streamFoam = 1.0f;
         half pud = 0;
         half snowCover = 0;
         half SSSThickness = 0;
         half3 SSSTint = half3(1,1,1);
         float traxBuffer = 0;
         float3 traxNormal = 0;
         float2 noiseUV = 0;
         
         

         #if _SPLATFADE
         MSBRANCHOTHER(1 - saturate(camDist - _SplatFade.y))
         {
         #endif

         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE || _SNOWFOOTSTEPS
            traxBuffer = SampleTraxBuffer(i.worldPos, worldNormalVertex, traxNormal);
         #endif
         
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
            #if _MICROMESH
               fxLevels = SampleFXLevels(InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, config.uv), wetLevel, burnLevel, traxBuffer);
            #elif _MICROVERTEXMESH || _MICRODIGGERMESH  || _MEGASPLAT
               fxLevels = ProcessFXLevels(i.fx, traxBuffer);
            #else
               fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel, traxBuffer);
            #endif
         #endif

         #if _DECAL
            fxLevels = max(fxLevels, decalOutput.fxLevels);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         

         MIPFORMAT albedoLOD = INITMIPFORMAT
         MIPFORMAT normalLOD = INITMIPFORMAT
         MIPFORMAT emisLOD = INITMIPFORMAT
         MIPFORMAT specLOD = INITMIPFORMAT
         MIPFORMAT origAlbedoLOD = INITMIPFORMAT;

         #if _TRIPLANAR && !_DISABLESPLATMAPS
            PrepTriplanar(i.shaderData.texcoord0, worldNormalVertex, i.worldPos, config, tc, weights, albedoLOD, normalLOD, emisLOD, origAlbedoLOD);
            tc.IN = i;
         #endif
         
         
         #if !_TRIPLANAR && !_DISABLESPLATMAPS
            #if _USELODMIP
               albedoLOD = ComputeMipLevel(config.uv0.xy, _Diffuse_TexelSize.zw);
               normalLOD = ComputeMipLevel(config.uv0.xy, _NormalSAO_TexelSize.zw);
               #if _USEEMISSIVEMETAL
                  emisLOD = ComputeMipLevel(config.uv0.xy, _EmissiveMetal_TexelSize.zw);
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = ComputeMipLevel(config.uv0.xy, _Specular_TexelSize.zw);;
               #endif
            #elif _USEGRADMIP
               albedoLOD = float4(ddx(config.uv0.xy), ddy(config.uv0.xy));
               normalLOD = albedoLOD;
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
            #endif

            origAlbedoLOD = albedoLOD;
         #endif

         #if _PERTEXCURVEWEIGHT
           SAMPLE_PER_TEX(ptCurveWeight, 19.5, config, half4(0.5,1,1,1));
           weights.x = lerp(smoothstep(0.5 - ptCurveWeight0.r, 0.5 + ptCurveWeight0.r, weights.x), weights.x, ptCurveWeight0.r*2);
           weights.y = lerp(smoothstep(0.5 - ptCurveWeight1.r, 0.5 + ptCurveWeight1.r, weights.y), weights.y, ptCurveWeight1.r*2);
           weights.z = lerp(smoothstep(0.5 - ptCurveWeight2.r, 0.5 + ptCurveWeight2.r, weights.z), weights.z, ptCurveWeight2.r*2);
           weights.w = lerp(smoothstep(0.5 - ptCurveWeight3.r, 0.5 + ptCurveWeight3.r, weights.w), weights.w, ptCurveWeight3.r*2);
           weights = TotalOne(weights);
         #endif
         
         

         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR && !_DISABLESPLATMAPS
            
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif

            // fix for pertex uv scale using gradient sampler and weight blended derivatives
            #if _USEGRADMIP
               albedoLOD = albedoLOD * ptUVScale0.rgrg * weights.x + 
                           albedoLOD * ptUVScale1.rgrg * weights.y + 
                           albedoLOD * ptUVScale2.rgrg * weights.z + 
                           albedoLOD * ptUVScale3.rgrg * weights.w;
               normalLOD = albedoLOD;
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
            #endif
         #endif

         #if _PERTEXUVROTATION && !_TRIPLANAR && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptUVRot, 16.5, config, half4(0,0,0,0));
            config.uv0.xy = RotateUV(config.uv0.xy, ptUVRot0.x);
            config.uv1.xy = RotateUV(config.uv1.xy, ptUVRot1.x);
            #if !_MAX2LAYER
               config.uv2.xy = RotateUV(config.uv2.xy, ptUVRot2.x);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = RotateUV(config.uv3.xy, ptUVRot0.x);
            #endif
         #endif

         
         o.Alpha = 1;

         
         #if _POM && !_DISABLESPLATMAPS
            DoPOM(i, config, tc, albedoLOD, weights, camDist, worldNormalVertex);
         #endif
         

         SampleAlbedo(config, tc, samples, albedoLOD, weights);

         #if _NOISEHEIGHT
            ApplyNoiseHeight(samples, config.uv, config, i.worldPos, worldNormalVertex);
         #endif
         
         #if _STREAMS || (_PARALLAX && !_DISABLESPLATMAPS)
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(i, config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX && !_DISABLESPLATMAPS
            DoParallax(i, earlyHeight, config, tc, samples, weights, camDist);
         #endif


         // Blend results
         #if _PERTEXINTERPCONTRAST && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = clamp(contrast + _Contrast, 0.0001, 1.0); 
            half cnt = contrast.x * weights.x + contrast.y * weights.y + contrast.z * weights.z + contrast.w * weights.w;
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, cnt);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif

         #if _HYBRIDHEIGHTBLEND
            heightWeights = lerp(heightWeights, TotalOne(weights), saturate(camDist/max(1.0, _HybridHeightBlendDistance)));
         #endif

         
         // rescale derivatives after height weighting. Basically, in gradmip mode we blend the mip levels,
         // but this is before height mapping is sampled, so reblending them after alpha will make sure the other
         // channels (normal, etc) are sharper, which likely matters most.. 
         #if _PERTEXUVSCALEOFFSET && !_DISABLESPLATMAPS
            #if _TRIPLANAR
               #if _USEGRADMIP
                  SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));

                  albedoLOD.d0 = origAlbedoLOD.d0 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d0 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d0 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d0 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d1 = origAlbedoLOD.d1 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d1 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d1 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d1 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d2 = origAlbedoLOD.d2 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d2 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d2 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d2 * ptUVScale3.xyxy * heightWeights.w;
               
                  normalLOD.d0 = albedoLOD.d0;
                  normalLOD.d1 = albedoLOD.d1;
                  normalLOD.d2 = albedoLOD.d2;
               
                  #if _USEEMISSIVEMETAL
                     emisLOD.d0 = albedoLOD.d0;
                     emisLOD.d1 = albedoLOD.d1;
                     emisLOD.d2 = albedoLOD.d2;
                  #endif
               #endif // gradmip
            #else // not triplanar
               // fix for pertex uv scale using gradient sampler and weight blended derivatives
               #if _USEGRADMIP
                  albedoLOD = origAlbedoLOD * ptUVScale0.rgrg * heightWeights.x + 
                              origAlbedoLOD * ptUVScale1.rgrg * heightWeights.y + 
                              origAlbedoLOD * ptUVScale2.rgrg * heightWeights.z + 
                              origAlbedoLOD * ptUVScale3.rgrg * heightWeights.w;
                  normalLOD = albedoLOD;
                  #if _USEEMISSIVEMETAL
                     emisLOD = albedoLOD;
                  #endif
                  #if _USESPECULARWORKFLOW
                     specLOD = albedoLOD;
                  #endif
               #endif
            #endif
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples, albedoLOD, heightWeights);
         #endif


         SampleNormal(config, tc, samples, normalLOD, heightWeights);

         #if _USEEMISSIVEMETAL
            SampleEmis(config, tc, samples, emisLOD, heightWeights);
         #endif

         #if _USESPECULARWORKFLOW
            SampleSpecular(config, tc, samples, specLOD, heightWeights);
         #endif

         #if _DISTANCERESAMPLE && !_DISABLESPLATMAPS
            DistanceResample(samples, config, tc, camDist, i.viewDir, fxLevels, albedoLOD, i.worldPos, heightWeights, worldNormalVertex);
         #endif

         #if _STARREACHFORMAT
            samples.normSAO0.w = length(samples.normSAO0.xy);
            samples.normSAO1.w = length(samples.normSAO1.xy);
            samples.normSAO2.w = length(samples.normSAO2.xy);
            samples.normSAO3.w = length(samples.normSAO3.xy);
         #endif

         // PerTexture sampling goes here, passing the samples structure
         
         #if _PERTEXMICROSHADOWS || _PERTEXFUZZYSHADE
            SAMPLE_PER_TEX(ptFuzz, 17.5, config, half4(0, 0, 1, 1));
         #endif

         #if _PERTEXMICROSHADOWS
            #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP)
            {
               half3 lightDir = GetGlobalLightDirTS(i);
               half4 microShadows = half4(1,1,1,1);
               microShadows.x = MicroShadow(lightDir, half3(samples.normSAO0.xy, 1), samples.normSAO0.a, ptFuzz0.a);
               microShadows.y = MicroShadow(lightDir, half3(samples.normSAO1.xy, 1), samples.normSAO1.a, ptFuzz1.a);
               microShadows.z = MicroShadow(lightDir, half3(samples.normSAO2.xy, 1), samples.normSAO2.a, ptFuzz2.a);
               microShadows.w = MicroShadow(lightDir, half3(samples.normSAO3.xy, 1), samples.normSAO3.a, ptFuzz3.a);
               samples.normSAO0.a *= microShadows.x;
               samples.normSAO1.a *= microShadows.y;
               #if !_MAX2LAYER
                  samples.normSAO2.a *= microShadows.z;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.normSAO3.a *= microShadows.w;
               #endif

               
               #if _DEBUG_OUTPUT_MICROSHADOWS
               o.Albedo = BlendWeights(microShadows.x, microShadows.y, microShadows.z, microShadows.a, heightWeights);
               return o;
               #endif

               

               
            }
            #endif

         #endif // _PERTEXMICROSHADOWS


         #if _PERTEXFUZZYSHADE
            samples.albedo0.rgb = FuzzyShade(samples.albedo0.rgb, half3(samples.normSAO0.rg, 1), ptFuzz0.r, ptFuzz0.g, ptFuzz0.b, i.viewDir);
            samples.albedo1.rgb = FuzzyShade(samples.albedo1.rgb, half3(samples.normSAO1.rg, 1), ptFuzz1.r, ptFuzz1.g, ptFuzz1.b, i.viewDir);
            #if !_MAX2LAYER
               samples.albedo2.rgb = FuzzyShade(samples.albedo2.rgb, half3(samples.normSAO2.rg, 1), ptFuzz2.r, ptFuzz2.g, ptFuzz2.b, i.viewDir);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = FuzzyShade(samples.albedo3.rgb, half3(samples.normSAO3.rg, 1), ptFuzz3.r, ptFuzz3.g, ptFuzz3.b, i.viewDir);
            #endif
         #endif

         #if _PERTEXSATURATION && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptSaturattion, 9.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = lerp(MSLuminance(samples.albedo0.rgb), samples.albedo0.rgb, ptSaturattion0.a);
            samples.albedo1.rgb = lerp(MSLuminance(samples.albedo1.rgb), samples.albedo1.rgb, ptSaturattion1.a);
            #if !_MAX2LAYER
               samples.albedo2.rgb = lerp(MSLuminance(samples.albedo2.rgb), samples.albedo2.rgb, ptSaturattion2.a);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = lerp(MSLuminance(samples.albedo3.rgb), samples.albedo3.rgb, ptSaturattion3.a);
            #endif
         
         #endif
         
         #if _PERTEXTINT && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif
         
         #if _PCHEIGHTGRADIENT || _PCHEIGHTHSV || _PCSLOPEGRADIENT || _PCSLOPEHSV
            ProceduralGradients(i, samples, config, worldHeight, worldNormalVertex);
         #endif

         
         

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXCOLORINTENSITY
            SAMPLE_PER_TEX(ptCI, 23.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = saturate(samples.albedo0.rgb * (1 + ptCI0.rrr));
            samples.albedo1.rgb = saturate(samples.albedo1.rgb * (1 + ptCI1.rrr));
            #if !_MAX2LAYER
               samples.albedo2.rgb = saturate(samples.albedo2.rgb * (1 + ptCI2.rrr));
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = saturate(samples.albedo3.rgb * (1 + ptCI3.rrr));
            #endif
         #endif

         #if (_PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = saturate(((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5);
               samples.albedo1.rgb = saturate(((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5);
               #if !_MAX2LAYER
                 samples.albedo2.rgb = saturate(((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5);
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb = saturate(samples.albedo0.rgb + ptBC0.rrr);
               samples.albedo1.rgb = saturate(samples.albedo1.rgb + ptBC1.rrr);
               #if !_MAX2LAYER
                  samples.albedo2.rgb = saturate(samples.albedo2.rgb + ptBC2.rrr);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(samples.albedo3.rgb + ptBC3.rrr);
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if (_PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR && !_DISABLESPLATMAPS
            #if _SURFACENORMALS
               samples.surf0 *= perTexMatSettings0.r;
               samples.surf1 *= perTexMatSettings1.r;
               samples.surf2 *= perTexMatSettings2.r;
               samples.surf3 *= perTexMatSettings3.r;
            #else
               samples.normSAO0.xy *= perTexMatSettings0.r;
               samples.normSAO1.xy *= perTexMatSettings1.r;
               samples.normSAO2.xy *= perTexMatSettings2.r;
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR && !_DISABLESPLATMAPS
            samples.normSAO0.a = pow(abs(samples.normSAO0.a), perTexMatSettings0.b);
            samples.normSAO1.a = pow(abs(samples.normSAO1.a), perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(abs(samples.normSAO2.a), perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(abs(samples.normSAO3.a), perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR && !_DISABLESPLATMAPS
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         
         #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP) 
          #if _PERTEXSSS
          {
            SAMPLE_PER_TEX(ptSSS, 18.5, config, half4(1, 1, 1, 1)); // tint, thickness
            half4 vals = ptSSS0 * heightWeights.x + ptSSS1 * heightWeights.y + ptSSS2 * heightWeights.z + ptSSS3 * heightWeights.w;
            SSSThickness = vals.a;
            SSSTint = vals.rgb;
          }
          #endif
         #endif

         #if _PERTEXRIMLIGHT
         {
            SAMPLE_PER_TEX(ptRimA, 26.5, config, half4(1, 1, 1, 1));
            SAMPLE_PER_TEX(ptRimB, 27.5, config, half4(1, 1, 1, 0));
            samples.emisMetal0.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO0.xy, 1))), max(0.0001, ptRimA0.g)) * ptRimB0.rgb * ptRimB0.a;
            samples.emisMetal1.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO1.xy, 1))), max(0.0001, ptRimA1.g)) * ptRimB1.rgb * ptRimB1.a;
            samples.emisMetal2.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO2.xy, 1))), max(0.0001, ptRimA2.g)) * ptRimB2.rgb * ptRimB2.a;
            samples.emisMetal3.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO3.xy, 1))), max(0.0001, ptRimA3.g)) * ptRimB3.rgb * ptRimB3.a;
         }
         #endif



         #if (((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)) && !_DISABLESPLATMAPS
            ApplyDetailDistanceNoisePerTex(samples, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         
         #if _GLOBALNOISEUV
            // noise defaults so that a value of 1, 1 is 4 pixels in size and moves the uvs by 1 pixel max.
            #if _CUSTOMSPLATTEXTURES
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #else
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif
         #endif

         
         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE
            ApplyTrax(samples, config, i.worldPos, traxBuffer, traxNormal);
         #endif

         #if (_ANTITILEARRAYDETAIL || _ANTITILEARRAYDISTANCE || _ANTITILEARRAYNORMAL) && !_DISABLESPLATMAPS
            ApplyAntiTilePerTex(samples, config, camDist, i.worldPos, worldNormalVertex, heightWeights);
         #endif

         #if _GEOMAP && !_DISABLESPLATMAPS
         GeoTexturePerTex(samples, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif
         
         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH && !_DISABLESPLATMAPS
         GlobalTintTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH && !_DISABLESPLATMAPS
         GlobalNormalTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && _PERTEXGLOBALSAOMSTRENGTH && !_DISABLESPLATMAPS
         GlobalSAOMTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALEMIS && _PERTEXGLOBALEMISSTRENGTH && !_DISABLESPLATMAPS
         GlobalEmisTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && _PERTEXGLOBALSPECULARSTRENGTH && !_DISABLESPLATMAPS && _USESPECULARWORKFLOW
         GlobalSpecularTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _PERTEXMETALLIC && !_DISABLESPLATMAPS
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         #if _GLITTER && !_DISABLESPLATMAPS
            DoGlitter(i, samples, config, camDist, worldNormalVertex, i.worldPos);
         #endif
         
         // Blend em..
         #if _DISABLESPLATMAPS
            // If we don't sample from the _Diffuse, then the shader compiler will strip the sampler on
            // some platforms, which will cause everything to break. So we sample from the lowest mip
            // and saturate to 1 to keep the cost minimal. Annoying, but the compiler removes the texture
            // and sampler, even though the sampler is still used.
            albedo = saturate(UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, float3(0,0,0), 12) + 1);
            albedo.a = 0.5; // make height something we can blend with for the combined mesh mode, since it still height blends.
            normSAO = half4(0,0,0,1);
         #else
            albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
            normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);

            #if _SURFACENORMALS
               surfGrad = BlendWeights(samples.surf0, samples.surf1, samples.surf2, samples.surf3, heightWeights);
            #endif

            #if (_USEEMISSIVEMETAL || _PERTEXRIMLIGHT) && !_DISABLESPLATMAPS
               emisMetal = BlendWeights(samples.emisMetal0, samples.emisMetal1, samples.emisMetal2, samples.emisMetal3, heightWeights);
            #endif

            #if _USESPECULARWORKFLOW && !_DISABLESPLATMAPS
               specular = BlendWeights(samples.specular0, samples.specular1, samples.specular2, samples.specular3, heightWeights);
            #endif

            #if _PERTEXOUTLINECOLOR
               SAMPLE_PER_TEX(ptOutlineColor, 28.5, config, half4(0.5, 0.5, 0.5, 1));
               half4 outlineColor = BlendWeights(ptOutlineColor0, ptOutlineColor1, ptOutlineColor2, ptOutlineColor3, heightWeights);
               half4 tstr = saturate(abs(heightWeights - 0.5) * 2);
               half transitionBlend = min(min(min(tstr.x, tstr.y), tstr.z), tstr.w);
               albedo.rgb = lerp(albedo.rgb * outlineColor.rgb * 2, albedo.rgb, outlineColor.a * transitionBlend);
            #endif
         #endif



         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
            o.Alpha = 1.0;
            if (config.uv0.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.x;
            else if (config.uv1.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.y;
            else if (config.uv2.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.z;
            else if (config.uv3.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.w;
         #endif



         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
            ApplyDetailDistanceNoise(albedo.rgb, normSAO, surfGrad, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         #if _SPLATFADE
         }
         #endif

         #if _SPLATFADE
            
            float2 sfDX = ddx(config.uv * _UVScale);
            float2 sfDY = ddy(config.uv * _UVScale);

            MSBRANCHOTHER(camDist - _SplatFade.x)
            {
               float falloff = saturate(InverseLerp(_SplatFade.x, _SplatFade.y, camDist));
               half4 sfalb = SAMPLE_TEXTURE2D_ARRAY_GRAD(_Diffuse, sampler_Diffuse, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY);
               COUNTSAMPLE
               albedo.rgb = lerp(albedo.rgb, sfalb.rgb, falloff);

               #if !_NONORMALMAP && !_AUTONORMAL
                  half4 sfnormSAO = SAMPLE_TEXTURE2D_ARRAY_GRAD(_NormalSAO, sampler_NormalSAO, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY).agrb;
                  COUNTSAMPLE
                  sfnormSAO.xy = sfnormSAO.xy * 2 - 1;

                  normSAO = lerp(normSAO, sfnormSAO, falloff);
  
                  #if _SURFACENORMALS
                     surfGrad = lerp(surfGrad, ConvertNormal2ToGradient(sfnormSAO.xy), falloff);
                  #endif
               #endif
              
            }
         #endif

         #if _AUTONORMAL
            float3 autoNormal = HeightToNormal(albedo.a * _AutoNormalHeightScale, i.worldPos);
            normSAO.xy = autoNormal;
            normSAO.z = 0;
            normSAO.w = (autoNormal.z * autoNormal.z);
         #endif
 


         #if _MESHCOMBINED
            SampleMeshCombined(albedo, normSAO, surfGrad, emisMetal, specular, o.Alpha, SSSThickness, SSSTint, config, heightWeights);
         #endif

         #if _ISOBJECTSHADER
            SampleObjectShader(i, albedo, normSAO, surfGrad, emisMetal, specular, config);
         #endif

         #if _GEOMAP
            GeoTexture(albedo.rgb, normSAO, surfGrad, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif

         
         #if _SCATTER
            ApplyScatter(
               #if _MEGASPLAT
               config, 
               #endif
               i, albedo, normSAO, surfGrad, config.uv, camDist);
         #endif

         #if _DECAL
            DoDecalBlend(decalOutput, albedo, normSAO, surfGrad, emisMetal, i.uv_Control0);
         #endif
         

         #if _GLOBALTINT && !_PERTEXGLOBALTINTSTRENGTH
            GlobalTintTexture(albedo.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _VSGRASSMAP
            VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMALSTRENGTH
            GlobalNormalTexture(normSAO, surfGrad, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && !_PERTEXGLOBALSAOMSTRENGTH
            GlobalSAOMTexture(normSAO, emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALEMIS && !_PERTEXGLOBALEMISSTRENGTH
            GlobalEmisTexture(emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && !_PERTEXGLOBALSPECULARSTRENGTH && _USESPECULARWORKFLOW
            GlobalSpecularTexture(specular.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         
         
         o.Albedo = albedo.rgb;
         o.Height = albedo.a;

         #if _NONORMALMAP
            o.Normal = half3(0,0,1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #elif _SURFACENORMALS
            o.Normal = ResolveNormalFromSurfaceGradient(surfGrad);
            o.Normal = mul(GetTBN(i), o.Normal);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #else
            o.Normal = half3(normSAO.xy, 1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;       
         #endif


         

         #if _USEEMISSIVEMETAL || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _PERTEXRIMLIGHT
           #if _USEEMISSIVEMETAL
	           emisMetal.rgb *= _EmissiveMult;
	        #endif
           
           o.Emission += emisMetal.rgb;
           o.Metallic = emisMetal.a;
	        
         #endif

         #if _USESPECULARWORKFLOW
            o.Specular = specular;
         #endif

         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(i, o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         
         #if _SNOW
         snowCover = DoSnow(i, o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, 
            config, weights, SSSTint, SSSThickness, traxBuffer, traxNormal);
         #endif

         #if _PERTEXSSS || _MESHCOMBINEDUSESSS || (_SNOW && _SNOWSSS)
         {
            half3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

            o.Emission += ComputeSSS(i, worldView, WorldNormalVector(i, o.Normal),
               SSSTint, SSSThickness, _SSSDistance, _SSSScale, _SSSPower);
         }
         #endif
         
         #if _SNOWGLITTER
            DoSnowGlitter(i, config, o, camDist, worldNormalVertex, snowCover);
         #endif

         #if _WINDPARTICULATE || _SNOWPARTICULATE
            DoWindParticulate(i, o, config, weights, camDist, worldNormalVertex, snowCover);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         #if _SPECULARFADE
         {
            float specFade = saturate((i.worldPos.y - _SpecularFades.x) / max(_SpecularFades.y - _SpecularFades.x, 0.0001));
            o.Metallic *= specFade;
            o.Smoothness *= specFade;
         }
         #endif

         #if _VSSHADOWMAP
         VSShadowTexture(o, i, config, camDist);
         #endif
         
         #if _TOONWIREFRAME
         ToonWireframe(config.uv, o.Albedo, camDist);
         #endif

        #if _SEETHROUGHSHADER
            SeethroughShader(o.Albedo, o.Emission, o.Alpha, i.worldPos, o.Normal, i.worldNormal);                   
        #endif


         #if _DEBUG_TRAXBUFFER
            ClearAllButAlbedo(o, half3(traxBuffer, 0, 0) * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMALVERTEX
            ClearAllButAlbedo(o, worldNormalVertex * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMAL
            ClearAllButAlbedo(o,  WorldNormalVector(i, o.Normal) * saturate(o.Albedo.z+1));
         #endif

         #if _DEBUG_MEGABARY && _MEGASPLAT
            o.Albedo = i.baryWeights.xyz;
         #endif


         return o;
      }
      
      void SampleSplats(float2 controlUV, inout half4 w0, inout half4 w1, inout half4 w2, inout half4 w3, inout half4 w4, inout half4 w5, inout half4 w6, inout half4 w7)
      {
         #if _CUSTOMSPLATTEXTURES
            #if !_MICROMESH
            controlUV = (controlUV * (_CustomControl0_TexelSize.zw - 1.0f) + 0.5f) * _CustomControl0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_CustomControl0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_CustomControl1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_CustomControl2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_CustomControl3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_CustomControl4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_CustomControl5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_CustomControl6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_CustomControl7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #else
            #if !_MICROMESH
            controlUV = (controlUV * (_Control0_TexelSize.zw - 1.0f) + 0.5f) * _Control0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_Control0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_Control1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_Control2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_Control3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_Control4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_Control5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_Control6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_Control7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #endif
      }   


      

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
         #if _MEGANOUV
            i.uv_Control0 = i.worldPos.xz;
         #endif
         
         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);
          
         #if _FORCELOCALSPACE
            worldNormalVertex = mul((float3x3)GetWorldToObjectMatrix(), worldNormalVertex).xyz;
            i.worldPos = i.worldPos -  mul(GetObjectToWorldMatrix(), float4(0,0,0,1)).xyz;
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ORIGINSHIFT
             i.worldPos = i.worldPos + mul(_GlobalOriginMTX, float4(0,0,0,1)).xyz;
             i.worldHeight = i.worldPos.y;
         #endif

         #if _DEBUG_USE_TOPOLOGY
            i.worldPos = SAMPLE_TEXTURE2D(_DebugWorldPos, sampler_Diffuse, i.uv_Control0);
            worldNormalVertex = SAMPLE_TEXTURE2D(_DebugWorldNormal, sampler_Diffuse, i.uv_Control0);
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ALPHABELOWHEIGHT && !_TBDISABLEALPHAHOLES
            ClipWaterLevel(i.worldPos);
         #endif

         #if !_TBDISABLEALPHAHOLES && defined(_ALPHATEST_ON)
            // UNITY 2019.3 holes
            ClipHoles(i.uv_Control0);
         #endif


         float2 origUV = i.uv_Control0;

         #if _MICROMESH && _MESHUV2
         float2 controlUV = i.uv2_Diffuse;
         #else
         float2 controlUV = i.uv_Control0;
         #endif


         #if _MICROMESH
            controlUV = InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, controlUV);
         #endif

         half4 weights = half4(1,0,0,0);

         Config config = (Config)0;
         UNITY_INITIALIZE_OUTPUT(Config,config);
         config.uv = origUV;

         DecalOutput decalOutput = (DecalOutput)0;
         #if _DECAL
            decalOutput = DoDecals(i.uv_Control0, i.worldPos, camDist, worldNormalVertex);
         #endif

         #if _SURFACENORMALS
         // Initialize the surface gradient basis vectors
         ConstructSurfaceGradientTBN(i);
         #endif
        


         #if _SPLATFADE
         MSBRANCHOTHER(_SplatFade.y - camDist)
         #endif // _SPLATFADE
         {
            #if !_DISABLESPLATMAPS

               // Sample the splat data, from textures or vertices, and setup the config..
               #if _MICRODIGGERMESH
                  DiggerSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLAT
                  MegaSplatVertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLATTEXTURE
                   MegaSplatTextureSetup(controlUV, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MICROVERTEXMESH
                  VertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif !_PROCEDURALTEXTURE || _PROCEDURALBLENDSPLATS
                  half4 w0 = 0; half4 w1 = 0; half4 w2 = 0; half4 w3 = 0; half4 w4 = 0; half4 w5 = 0; half4 w6 = 0; half4 w7 = 0;
                  SampleSplats(controlUV, w0, w1, w2, w3, w4, w5, w6, w7);
                  Setup(weights, origUV, config, w0, w1, w2, w3, w4, w5, w6, w7, i.worldPos, decalOutput);
               #endif

               #if _PROCEDURALTEXTURE
                  float3 procNormal = worldNormalVertex;
                  float3 worldPos = i.worldPos;
                  ProceduralSetup(i, worldPos, i.worldHeight, procNormal, i.worldUpVector, weights, origUV, config, ddx(origUV), ddy(origUV), ddx(worldPos), ddy(worldPos), decalOutput);
               #endif
            #else // _DISABLESPLATMAPS
                Setup(weights, origUV, config, half4(1,0,0,0), 0, 0, 0, 0, 0, 0, 0, i.worldPos, decalOutput);
            #endif

            #if _SLOPETEXTURE
               SlopeTexture(config, weights, worldNormalVertex);
            #endif
         } // _SPLATFADE else case


         #if _TOONFLATTEXTURE
            float2 quv = floor(origUV * _ToonTerrainSize);
            float2 fuv = frac(origUV * _ToonTerrainSize);
            #if !_TOONFLATTEXTUREQUAD
               quv = Hash2D((fuv.x > fuv.y) ? quv : quv * 0.333);
            #endif
            float2 uvq = quv / _ToonTerrainSize;
            config.uv0.xy = uvq;
            config.uv1.xy = uvq;
            config.uv2.xy = uvq;
            config.uv3.xy = uvq;
         #endif
         
         #if (_TEXTURECLUSTER2 || _TEXTURECLUSTER3) && !_DISABLESPLATMAPS
            PrepClusters(origUV, config, i.worldPos, worldNormalVertex);
         #endif

         #if (_ALPHAHOLE || _ALPHAHOLETEXTURE) && !_DISABLESPLATMAPS && !_TBDISABLEALPHAHOLES
         ClipAlphaHole(config, weights);
         #endif


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex, decalOutput);

         // On windows, sometimes the shared samplers gets stripped, so we have to do this crap.
         // We sample from the lowest mip, so it shouldn't cost much, but still, I hate this, wtf..
         float stripVal = saturate(SAMPLE_TEXTURE2D_LOD(_Diffuse, sampler_Diffuse, config.uv0, 11).r + 2);
         stripVal *= saturate(SAMPLE_TEXTURE2D_LOD(_NormalSAO, sampler_NormalSAO, config.uv0, 11).r + 2);
         l.Albedo *= stripVal;
         l.Normal *= stripVal;

         #if _PROCEDURALTEXTURE
            ProceduralTextureDebugOutput(l, weights, config);
         #endif


         return l;

      }



   





float4 ConstructTerrainTangent(float3 normal, float3 positiveZ)
{
    // Consider a flat terrain. It should have tangent be (1, 0, 0) and bitangent be (0, 0, 1) as the UV of the terrain grid mesh is a scale of the world XZ position.
    // In CreateTangentToWorld function (in SpaceTransform.hlsl), it is cross(normal, tangent) * sgn for the bitangent vector.
    // It is not true in a left-handed coordinate system for the terrain bitangent, if we provide 1 as the tangent.w. It would produce (0, 0, -1) instead of (0, 0, 1).
    // Also terrain's tangent calculation was wrong in a left handed system because cross((0,0,1), terrainNormalOS) points to the wrong direction as negative X.
    // Therefore all the 4 xyzw components of the tangent needs to be flipped to correct the tangent frame.
    // (See TerrainLitData.hlsl - GetSurfaceAndBuiltinData)
    float3 tangent = cross(normal, positiveZ);
    return float4(tangent, -1);
}



void TerrainInstancing(inout float4 vertex, inout float3 normal, inout float2 uv)
{
#if _MICROTERRAIN && defined(UNITY_INSTANCING_ENABLED) && !_TERRAINBLENDABLESHADER
   
    float2 patchVertex = vertex.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;

    float2 sampleUV = (uv / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

    float height = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, shared_linear_clamp_sampler, sampleUV, 0));
   
    vertex.xz = sampleCoords * _TerrainHeightmapScale.xz;
    vertex.y = height * _TerrainHeightmapScale.y;

    
    normal = float3(0, 1, 0);

#endif
}


void ApplyMeshModification(inout VertexData input)
{
   #if _MICROTERRAIN && !_TERRAINBLENDABLESHADER
      float2 uv = input.texcoord0.xy;
      TerrainInstancing(input.vertex, input.normal, uv);
      input.texcoord0.xy = uv;
   #endif
   #if _PERPIXNORMAL && !_TERRAINBLENDABLESHADER
      input.normal = float3(0,1,0);
   #endif

}

// called by the template, so we can remove tangent from VertexData
void ApplyTerrainTangent(inout VertexToPixel input)
{
   #if (_MICROTERRAIN || _PERPIXNORMAL) && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif

   // digger meshes ain't got no tangent either..
   #if _MICRODIGGERMESH && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif
}


void ModifyVertex(inout VertexData v, inout ExtraV2F d)
{
   ApplyMeshModification(v);

   #if _MICROVERTEXMESH || _MICRODIGGERMESH
      EncodeVertexWorkflow(v, d);
   #elif _MEGASPLAT
      EncodeMegaSplatVertex(v, d);
   #endif

}


void ModifyTessellatedVertex(inout VertexData v, inout ExtraV2F d)
{
   #if _TESSDISTANCE
      v.vertex.xyz += OffsetVertex(v, d);
   #endif
}

float3 GetTessFactors ()
{
   #if _TESSDISTANCE
      return float3(_TessData2.x, _TessData2.y, _TessData1.x);
   #endif
   return 0;
}


        


    
    void SurfaceFunction(inout Surface o, inout ShaderData d)
    {
       
        float3 worldNormalVertex = d.worldSpaceNormal;

        #if (defined(UNITY_INSTANCING_ENABLED) && _MICROTERRAIN && !_TERRAINBLENDABLESHADER)
            float2 sampleCoords = (d.texcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            float3 geomBitangent = normalize(cross(geomNormal, geomTangent)) * -1;
            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

         #elif _PERPIXNORMAL &&  (_MICROTERRAIN || _MICROMESHTERRAIN) && !_TERRAINBLENDABLESHADER
            float2 sampleCoords = (d.texcoord0.xy * _PerPixelNormal_TexelSize.zw + 0.5f) * _PerPixelNormal_TexelSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_PerPixelNormal, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            
            float3 geomBitangent = normalize(cross(geomTangent, geomNormal)) * -1;
            #if _MICROMESHTERRAIN
                geomBitangent *= -1;
            #endif

            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

        #endif

        #if _TOONPOLYEDGE
           FlatShade(d);
        #endif

         Input i = DescToInput(d);

         
         
         #if _SRPTERRAINBLEND
            MicroSplatLayer l = BlendWithTerrain(d);

               #if _DEBUG_WORLDNORMAL
                  ClearAllButAlbedo(l, normalize(TangentToWorldSpace(d, l.Normal)) * saturate(l.Albedo.z+1));
               #endif
         #else
            MicroSplatLayer l = SurfImpl(i, worldNormalVertex);
         #endif

        DoDebugOutput(l);




        o.Albedo = l.Albedo;
        o.Normal = l.Normal;
        o.Smoothness = l.Smoothness;
        o.Occlusion = l.Occlusion;
        o.Metallic = l.Metallic;
        o.Emission = l.Emission;
        #if _USESPECULARWORKFLOW
        o.Specular = l.Specular;
        #endif
        o.Height = l.Height;
        o.Alpha = l.Alpha;


    }



        



            // SHADERDESC

         ShaderData CreateShaderData(VertexToPixel i)
         {
            ShaderData d = (ShaderData)0;
            d.worldSpacePosition = i.worldPos;

            d.worldSpaceNormal = i.worldNormal;
            d.worldSpaceTangent = i.worldTangent.xyz;
            float3 bitangent = cross(i.worldTangent.xyz, i.worldNormal) * i.worldTangent.w * -1;
            

            d.TBNMatrix = float3x3(d.worldSpaceTangent, bitangent, d.worldSpaceNormal);
            d.worldSpaceViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);
             d.texcoord0 = i.texcoord0;
            #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                d.texcoord1 = i.texcoord1;
               // d.texcoord2 = i.texcoord2;
            #endif
            // d.texcoord3 = i.texcoord3;
            // d.vertexColor = i.vertexColor;

            // these rarely get used, so we back transform them. Usually will be stripped.
            #if _HDRP
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(GetCameraRelativePositionWS(i.worldPos), 1));
            #else
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(i.worldPos, 1));
            #endif
            // d.localSpaceNormal = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldNormal));
            // d.localSpaceTangent = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldTangent.xyz));

            // d.screenPos = i.screenPos;
            // d.screenUV = i.screenPos.xy / i.screenPos.w;

            // d.extraV2F0 = i.extraV2F0;
            // d.extraV2F1 = i.extraV2F1;
            // d.extraV2F2 = i.extraV2F2;
            // d.extraV2F3 = i.extraV2F3;
            // d.extraV2F4 = i.extraV2F4;
            // d.extraV2F5 = i.extraV2F5;
            // d.extraV2F6 = i.extraV2F6;
            // d.extraV2F7 = i.extraV2F7;

            return d;
         }
         // CHAINS

            void ChainModifyVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               ModifyVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }

            void ChainModifyTessellatedVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               // d.extraV2F0 = v2p.extraV2F0;
               // d.extraV2F1 = v2p.extraV2F1;
               // d.extraV2F2 = v2p.extraV2F2;
               // d.extraV2F3 = v2p.extraV2F3;
               // d.extraV2F4 = v2p.extraV2F4;
               // d.extraV2F5 = v2p.extraV2F5;
               // d.extraV2F6 = v2p.extraV2F6;
               // d.extraV2F7 = v2p.extraV2F7;

               ModifyTessellatedVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }


            void ChainFinalColorForward(inout Surface l, inout ShaderData d, inout half4 color)
            {
               
            }

            void ChainFinalGBufferStandard(inout Surface s, inout ShaderData d, inout half4 GBuffer0, inout half4 GBuffer1, inout half4 GBuffer2, inout half4 outEmission, inout half4 outShadowMask)
            {
               
            }


            
         #if _PASSSHADOW
            float3 _LightDirection;
            float3 _LightPosition;
         #endif

         // vertex shader
         VertexToPixel Vert (VertexData v)
         {
           
           VertexToPixel o = (VertexToPixel)0;

           UNITY_SETUP_INSTANCE_ID(v);
           UNITY_TRANSFER_INSTANCE_ID(v, o);
           UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


#if !_TESSELLATION_ON
           ChainModifyVertex(v, o);
#endif

            o.texcoord0 = v.texcoord0;

           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               o.texcoord1 = v.texcoord1;
              // o.texcoord2 = v.texcoord2;
           #endif

           // o.texcoord3 = v.texcoord3;
           // o.vertexColor = v.vertexColor;
           
           VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
           o.worldPos = TransformObjectToWorld(v.vertex.xyz);
           o.worldNormal = TransformObjectToWorldNormal(v.normal);

           
           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float2 uv1 = v.texcoord1.xy;
               float2 uv2 = v.texcoord2.xy;
               o.worldTangent = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);
           #else
               float2 uv1 = v.texcoord0.xy;
               float2 uv2 = uv1;
           #endif

           // MS Only
           ApplyTerrainTangent(o);
           

          #if _PASSSHADOW
              #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                 float3 lightDirectionWS = normalize(_LightPosition - o.worldPos);
              #else
                 float3 lightDirectionWS = _LightDirection;
              #endif
              // Define shadow pass specific clip position for Universal
              o.pos = TransformWorldToHClip(ApplyShadowBias(o.worldPos, o.worldNormal, lightDirectionWS));
              #if UNITY_REVERSED_Z
                  o.pos.z = min(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #else
                  o.pos.z = max(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #endif
          #elif _PASSMETA
              #if _MICROTERRAIN
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), v.texcoord0.xy, v.texcoord0.xy, unity_LightmapST, unity_DynamicLightmapST);
              #else
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), uv1, uv2, unity_LightmapST, unity_DynamicLightmapST);
              #endif
          #else
              o.pos = TransformWorldToHClip(o.worldPos);
          #endif



          // o.screenPos = ComputeScreenPos(o.pos, _ProjectionParams.x);
          

          #if _PASSFORWARD || _PASSGBUFFER
              #if _MICROTERRAIN
                 OUTPUT_LIGHTMAP_UV(v.texcoord0.xy, unity_LightmapST, o.lightmapUV);
              #else
                 OUTPUT_LIGHTMAP_UV(uv1, unity_LightmapST, o.lightmapUV);
              #endif
              OUTPUT_SH(o.worldNormal, o.sh);
              #if defined(DYNAMICLIGHTMAP_ON)
                   o.dynamicLightmapUV.xy = uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
              #endif
          #endif

          #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
              half fogFactor = 0;
              #if defined(_FOG_FRAGMENT)
                fogFactor = ComputeFogFactor(o.pos.z);
              #endif
              #if _BAKEDLIT
                  half3 vertexLight = VertexLighting(o.worldPos, o.worldNormal);
                  o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
              #else
                  o.fogFactorAndVertexLight = half4(fogFactor, 0,0,0);
              #endif
          #endif

          #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             o.shadowCoord = GetShadowCoord(vertexInput);
          #endif

           return o;
         }


            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"


            // fragment shader
            FragmentOutput Frag (VertexToPixel IN
            #ifdef _DEPTHOFFSET_ON
              , out float outputDepth : SV_Depth
            #endif
            )
            {
               UNITY_SETUP_INSTANCE_ID(IN);
               UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

               ShaderData d = CreateShaderData(IN);
               Surface l = (Surface)0;

               #ifdef _DEPTHOFFSET_ON
                  l.outputDepth = outputDepth;
               #endif

               l.Albedo = half3(0.5, 0.5, 0.5);
               l.Normal = float3(0,0,1);
               l.Occlusion = 1;
               l.Alpha = 1;

               SurfaceFunction(l, d);

               #ifdef _DEPTHOFFSET_ON
                  outputDepth = l.outputDepth;
               #endif

               #if defined(_USESPECULAR) || _SIMPLELIT
                  float3 specular = l.Specular;
                  float metallic = 0;
               #else   
                  float3 specular = 0;
                  float metallic = l.Metallic;
               #endif


            
           
               InputData inputData = (InputData)0;

               inputData.positionWS = IN.worldPos;
               inputData.normalWS = mul(l.Normal, d.TBNMatrix);
               inputData.viewDirectionWS = SafeNormalize(d.worldSpaceViewDir);


               #if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                   inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
               #else
                   inputData.shadowCoord = float4(0, 0, 0, 0);
               #endif

   
               InitializeInputDataFog(float4(IN.worldPos, 1.0), IN.fogFactorAndVertexLight.x);
               inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

               #if defined(DYNAMICLIGHTMAP_ON)
                  inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.dynamicLightmapUV.xy, IN.sh, inputData.normalWS);
               #else
                  inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.sh, inputData.normalWS);
               #endif

               inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.pos);
               #if defined(LIGHTMAP_ON)
                  inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUV);
               #endif

               #if defined(DEBUG_DISPLAY)
                   #if defined(DYNAMICLIGHTMAP_ON)
                     inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
                   #endif
                   #if defined(LIGHTMAP_ON)
                     inputData.staticLightmapUV = IN.lightmapUV;
                   #else
                     inputData.vertexSH = IN.sh;
                   #endif
               #endif

               #ifdef _DBUFFER
                   ApplyDecal(IN.pos,
                       l.Albedo,
                       specular,
                       inputData.normalWS,
                       metallic,
                       l.Occlusion,
                       l.Smoothness);
               #endif

               BRDFData brdfData;
               InitializeBRDFData(l.Albedo, metallic, specular, l.Smoothness, l.Alpha, brdfData);
               Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
               MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
               half3 color = GlobalIllumination(brdfData, inputData.bakedGI, l.Occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);

               return BRDFDataToGbuffer(brdfData, inputData, l.Smoothness, l.Emission + color, l.Occlusion);
            }

         ENDHLSL

      }


      
        Pass
        {
            Name "ShadowCaster"
            Tags 
            { 
                "LightMode" = "ShadowCaster"
            }
           
            // Render State
            Blend One Zero, One Zero
            Cull Back
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>

            

            HLSLPROGRAM

               #pragma vertex Vert
   #pragma fragment Frag

            #pragma target 3.5

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ _ALPHATEST_ON
        
            #define _NORMAL_DROPOFF_TS 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define SHADERPASS_SHADOWCASTER

            #define _PASSSHADOW 1

            
      #define _MICROSPLAT 1
      #define _MICROTERRAIN 1
      #define _USEGRADMIP 1
      #define _MAX4TEXTURES 1
      #define _BRANCHSAMPLES 1
      #define _BRANCHSAMPLESAGR 1
      #define _MSRENDERLOOP_UNITYURP2022 1

#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


   #define _URP 1


                 
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

                  #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, float3x3(d.worldSpaceTangent, cross(d.worldSpaceTangent, d.worldSpaceNormal), d.worldSpaceNormal))

      
      #define UnityObjectToWorldNormal(normal) mul(GetObjectToWorldMatrix(), normal)

      #define _WorldSpaceLightPos0 _MainLightPosition
      
      #define UNITY_DECLARE_TEX2D(name) TEXTURE2D(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2D_NOSAMPLER(name) TEXTURE2D(name);
      #define UNITY_DECLARE_TEX2DARRAY(name) TEXTURE2D_ARRAY(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(name) TEXTURE2D_ARRAY(name);

      #define UNITY_SAMPLE_TEX2DARRAY(tex,coord)            SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, coord.xy, coord.z)
      #define UNITY_SAMPLE_TEX2DARRAY_LOD(tex,coord,lod)    SAMPLE_TEXTURE2D_ARRAY_LOD(tex, sampler##tex, coord.xy, coord.z, lod)
      #define UNITY_SAMPLE_TEX2D(tex, coord)                SAMPLE_TEXTURE2D(tex, sampler##tex, coord)
      #define UNITY_SAMPLE_TEX2D_SAMPLER(tex, samp, coord)  SAMPLE_TEXTURE2D(tex, sampler##samp, coord)

      #if defined(UNITY_COMPILER_HLSL)
         #define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
      #else
         #define UNITY_INITIALIZE_OUTPUT(type,name)
      #endif

      #define sampler2D_float sampler2D
      #define sampler2D_half sampler2D

      

      // data across stages, stripped like the above.
      struct VertexToPixel
      {
         float4 pos : SV_POSITION;
         float3 worldPos : TEXCOORD0;
         float3 worldNormal : TEXCOORD1;
         float4 worldTangent : TEXCOORD2;
          float4 texcoord0 : TEXCCOORD3;
         #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
             float4 texcoord1 : TEXCCOORD4;
            // float4 texcoord2 : TEXCCOORD5;
         #endif
         // float4 texcoord3 : TEXCCOORD6;
         // float4 screenPos : TEXCOORD7;
         // float4 vertexColor : COLOR;

         // float4 extraV2F0 : TEXCOORD13;
         // float4 extraV2F1 : TEXCOORD14;
         // float4 extraV2F2 : TEXCOORD15;
         // float4 extraV2F3 : TEXCOORD16;
         // float4 extraV2F4 : TEXCOORD17;
         // float4 extraV2F5 : TEXCOORD18;
         // float4 extraV2F6 : TEXCOORD19;
         // float4 extraV2F7 : TEXCOORD20;

         
         #if defined(LIGHTMAP_ON)
            float2 lightmapUV : TEXCOORD8;
         #endif
         #if defined(DYNAMICLIGHTMAP_ON)
            float2 dynamicLightmapUV : TEXCOORD9;
         #endif
         #if !defined(LIGHTMAP_ON)
            float3 sh : TEXCOORD10;
         #endif

         float4 fogFactorAndVertexLight : TEXCOORD11;
         float4 shadowCoord : TEXCOORD12;

         #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
         #endif
         #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
         #endif
         #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
         #endif
         #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
         #endif
      };

         
            // TEMPLATE_SHARED
            
            // data describing the user output of a pixel
            struct Surface
            {
               half3 Albedo;
               half Height;
               half3 Normal;
               half Smoothness;
               half3 Emission;
               half Metallic;
               half3 Specular;
               half Occlusion;
               half Alpha;
               // HDRP Only
               half SpecularOcclusion;
               half SubsurfaceMask;
               half Thickness;
               half CoatMask;
               half Anisotropy;
               half IridescenceMask;
               half IridescenceThickness;
            };

            // data the user might need, this will grow to be big. But easy to strip
            struct ShaderData
            {
               float3 localSpacePosition;
               float3 localSpaceNormal;
               float3 localSpaceTangent;
        
               float3 worldSpacePosition;
               float3 worldSpaceNormal;
               float3 worldSpaceTangent;

               float3 worldSpaceViewDir;
               float3 tangentSpaceViewDir;

               float4 texcoord0;
               float4 texcoord1;
               float4 texcoord2;
               float4 texcoord3;

               float2 screenUV;
               float4 screenPos;

               float4 vertexColor;

               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;

               float3x3 TBNMatrix;
            };

            struct VertexData
            {
               #if SHADER_TARGET > 30 && _PLANETCOMPUTE
 //              // uint vertexID : SV_VertexID;
               #endif
               float4 vertex : POSITION;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                  float4 tangent : TANGENT;
                  float4 texcoord1 : TEXCOORD1;
                  float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD4; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity    : TEXCOORD5; // Add Precomputed Velocity (Alembic computes velocities on runtime side).
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessVertex 
            {
               float4 vertex : INTERNALTESSPOS;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float4 tangent : TANGENT;
               float4 texcoord1 : TEXCOORD1;
               float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               // float4 extraV2F0 : TEXCOORD4;
               // float4 extraV2F1 : TEXCOORD5;
               // float4 extraV2F2 : TEXCOORD6;
               // float4 extraV2F3 : TEXCOORD7;
               // float4 extraV2F4 : TEXCOORD8;
               // float4 extraV2F5 : TEXCOORD9;
               // float4 extraV2F6 : TEXCOORD10;
               // float4 extraV2F7 : TEXCOORD11;

               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD12; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity : TEXCOORD13;
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
               UNITY_VERTEX_OUTPUT_STEREO
            };

            struct ExtraV2F
            {
               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;
            };


            float3 WorldToTangentSpace(ShaderData d, float3 normal)
            {
               return mul(d.TBNMatrix, normal);
            }

            float3 TangentToWorldSpace(ShaderData d, float3 normal)
            {
               return mul(normal, d.TBNMatrix);
            }

            // in this case, make standard more like SRPs, because we can't fix
            // GetWorldToObjectMatrix() in HDRP, since it already does macro-fu there

            #if _STANDARD
               float3 TransformWorldToObject(float3 p) { return mul(GetWorldToObjectMatrix(), float4(p, 1)); };
               float3 TransformObjectToWorld(float3 p) { return mul(GetObjectToWorldMatrix(), float4(p, 1)); };
               float4 TransformWorldToObject(float4 p) { return mul(GetWorldToObjectMatrix(), p); };
               float4 TransformObjectToWorld(float4 p) { return mul(GetObjectToWorldMatrix(), p); };
               float4x4 GetWorldToObjectMatrix() { return GetWorldToObjectMatrix(); }
               float4x4 GetObjectToWorldMatrix() { return GetObjectToWorldMatrix(); }
            #endif

            float3 GetCameraWorldPosition()
            {
               #if _HDRP
                  return GetCameraRelativePositionWS(_WorldSpaceCameraPos);
               #else
                  return _WorldSpaceCameraPos;
               #endif
            }

            
             #if _HDRP

               half3 UnpackNormalmapRGorAG(half4 packednormal)
               {
                     // This do the trick
                  packednormal.x *= packednormal.w;

                  half3 normal;
                  normal.xy = packednormal.xy * 2 - 1;
                  normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                  return normal;
               }
               half3 UnpackNormal(half4 packednormal)
               {
                  #if defined(UNITY_NO_DXT5nm)
                     return packednormal.xyz * 2 - 1;
                  #else
                     return UnpackNormalmapRGorAG(packednormal);
                  #endif
               }
               #endif
               #if _HDRP || _URP

               half3 UnpackScaleNormal(half4 packednormal, half scale)
               {
                 #ifndef UNITY_NO_DXT5nm
                   // Unpack normal as DXT5nm (1, y, 1, x) or BC5 (x, y, 0, 1)
                   // Note neutral texture like "bump" is (0, 0, 1, 1) to work with both plain RGB normal and DXT5nm/BC5
                   packednormal.x *= packednormal.w;
                 #endif
                   half3 normal;
                   normal.xy = (packednormal.xy * 2 - 1) * scale;
                   normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                   return normal;
               }  

             #endif


            void GetSun(out float3 lightDir, out float3 color)
            {
               lightDir = float3(0.5, 0.5, 0);
               color = 1;
               #if _HDRP
                  if (_DirectionalLightCount > 0)
                  {
                     DirectionalLightData light = _DirectionalLightDatas[0];
                     lightDir = -light.forward.xyz;
                     color = light.color;
                  }
               #elif _STANDARD
                  lightDir = normalize(_WorldSpaceLightPos0.xyz);
                  color = _LightColor0.rgb;
               #elif _URP
                  Light light = GetMainLight();
                  lightDir = light.direction;
                  color = light.color;
               #endif
            }

     



            
            CBUFFER_START(UnityPerMaterial)

               

      #if _MESHSUBARRAY
         half4 _MeshSubArrayIndexes;
      #endif

      float4 _Diffuse_TexelSize;
      float4 _NormalSAO_TexelSize;

      #if _HYBRIDHEIGHTBLEND
         float _HybridHeightBlendDistance;
      #endif

      #if _PACKINGHQ
         float4 _SmoothAO_TexelSize;
      #endif

      #ifdef _ALPHATEST_ON
      float4 _TerrainHolesTexture_TexelSize;
      #endif

      #if _USESPECULARWORKFLOW
         float4 _Specular_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         float4 _EmissiveMetal_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         half _EmissiveMult;
      #endif

      #if _AUTONORMAL
         half _AutoNormalHeightScale;
      #endif

      float4 _UVScale; // scale and offset

      half _Contrast;
      
      

       #if _VSSHADOWMAP
         float4 gVSSunDirection;
      #endif

      #if _FORCELOCALSPACE && _PLANETVECTORS
         float4x4 _PQSToLocal;
      #endif

      #if _ORIGINSHIFT
         float4x4 _GlobalOriginMTX;
      #endif

      float4 _Control0_TexelSize;
      #if _CUSTOMSPLATTEXTURES
         float4 _CustomControl0_TexelSize;
      #endif
      float4 _PerPixelNormal_TexelSize;

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         float2 _NoiseUVParams;
      #endif

      float4 _PerTexProps_TexelSize;

      #if _SURFACENORMALS  
         float3 surfTangent;
         float3 surfBitangent;
         float3 surfNormal;
      #endif



            CBUFFER_END

            

            

                  

      #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, data.TBN)
      

      



   
// In Unity 2020.3LTS, Unity will spew tons of errors about missing this sampler in
// URP, even though it shouldn't be required.
TEXTURE2D(_MainTex);

      // globals, outside of CBuffer, but used by more than one module
      float3 _gGlitterLightDir;
      float3 _gGlitterLightWorldPos;
      half3 _gGlitterLightColor;

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
         float4    _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
         float4    _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
         float4    _TerrainNormalmapTexture_TexelSize;
      #endif

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
          TEXTURE2D(_TerrainHeightmapTexture);
          TEXTURE2D(_TerrainNormalmapTexture);
      #endif

      UNITY_INSTANCING_BUFFER_START(Terrain)
          UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
      UNITY_INSTANCING_BUFFER_END(Terrain)          


      

      // dynamic branching helpers, for regular and aggressive branching
      // debug mode shows how many samples using branching will save us. 
      //
      // These macros are always used instead of the UNITY_BRANCH macro
      // to maintain debug displays and allow branching to be disabled
      // on as granular level as we want. 
      
      #if _BRANCHSAMPLES
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++; if (w > 0)
         #else
            #define MSBRANCH(w) UNITY_BRANCH if (w > 0)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++;
         #else
            #define MSBRANCH(w) 
         #endif
      #endif
      
      #if _BRANCHSAMPLESAGR
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER ||_DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++; if (w > 0.001)
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++; if (w > 0.001)
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++; if (w > 0.001)
         #else
            #define MSBRANCHTRIPLANAR(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHCLUSTER(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHOTHER(w) UNITY_BRANCH if (w > 0.001)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++;
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++;
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++;
         #else
            #define MSBRANCHTRIPLANAR(w)
            #define MSBRANCHCLUSTER(w)
            #define MSBRANCHOTHER(w)
         #endif
      #endif

      #if _DEBUG_SAMPLECOUNT
         int _sampleCount;
         #define COUNTSAMPLE { _sampleCount++; }
      #else
         #define COUNTSAMPLE
      #endif

      #if _DEBUG_PROCLAYERS
         int _procLayerCount;
         #define COUNTPROCLAYER { _procLayerCount++; }
      #else
         #define COUNTPROCLAYER
      #endif


      #if _DEBUG_USE_TOPOLOGY
         TEXTURE2D(_DebugWorldPos);
         TEXTURE2D(_DebugWorldNormal);
      #endif
      

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         TEXTURE2D(_NoiseUV);
      #endif

      #if _PACKINGHQ
         UNITY_DECLARE_TEX2DARRAY(_SmoothAO);
      #endif

      #if _USESPECULARWORKFLOW
         UNITY_DECLARE_TEX2DARRAY(_Specular);
      #endif

      #if _USEEMISSIVEMETAL
         UNITY_DECLARE_TEX2DARRAY(_EmissiveMetal);
      #endif

      TEXTURE2D(_PerPixelNormal);

      SamplerState shared_linear_clamp_sampler;
      SamplerState shared_point_clamp_sampler;
      
      TEXTURE2D(_Control0);
      #if _CUSTOMSPLATTEXTURES
         TEXTURE2D(_CustomControl0);
         #if !_MAX4TEXTURES
         TEXTURE2D(_CustomControl1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_CustomControl2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_CustomControl3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_CustomControl7);
         #endif
      #else
         #if !_MAX4TEXTURES
         TEXTURE2D(_Control1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_Control2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_Control3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_Control7);
         #endif
      #endif

      TEXTURE2D_FLOAT(_PerTexProps);
   
      struct DecalLayer
      {
         float3 uv;
         float2 dx;
         float2 dy;
         int decalIndex;
         bool dynamic; 
      };

      struct DecalOutput
      {
         DecalLayer l0;
         DecalLayer l1;
         DecalLayer l2;
         DecalLayer l3;
         
         half4 Weights;
         half4 Indexes;
         half4 fxLevels;
         
      };
      

      struct TriGradMipFormat
      {
         float4 d0;
         float4 d1;
         float4 d2;
      };

      float InverseLerp(float x, float y, float v) { return (v-x)/max(y-x, 0.001); }
      float2 InverseLerp(float2 x, float2 y, float2 v) { return (v-x)/max(y-x, float2(0.001, 0.001)); }
      float3 InverseLerp(float3 x, float3 y, float3 v) { return (v-x)/max(y-x, float3(0.001, 0.001, 0.001)); }
      float4 InverseLerp(float4 x, float4 y, float4 v) { return (v-x)/max(y-x, float4(0.001, 0.001, 0.001, 0.001)); }
      

      // 2019.3 holes
      #ifdef _ALPHATEST_ON
          TEXTURE2D(_TerrainHolesTexture);
          
          void ClipHoles(float2 uv)
          {
              float hole = SAMPLE_TEXTURE2D(_TerrainHolesTexture, shared_linear_clamp_sampler, uv).r;
              COUNTSAMPLE
              clip(hole < 0.5f ? -1 : 1);
          }
      #endif

      
      #if _TRIPLANAR
         #if _USEGRADMIP
            #define MIPFORMAT TriGradMipFormat
            #define INITMIPFORMAT (TriGradMipFormat)0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float3
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float3
         #endif
      #else
         #if _USEGRADMIP
            #define MIPFORMAT float4
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float
         #endif
      #endif

      float2 TotalOne(float2 v) { return v * (1.0 / max(v.x + v.y, 0.001)); }
      float3 TotalOne(float3 v) { return v * (1.0 / max(v.x + v.y + v.z, 0.001)); }
      float4 TotalOne(float4 v) { return v * (1.0 / max(v.x + v.y + v.z + v.w, 0.001)); }

      float2 RotateUV(float2 uv, float amt)
      {
         uv -=0.5;
         float s = sin ( amt);
         float c = cos ( amt );
         float2x2 mtx = float2x2( c, -s, s, c);
         mtx *= 0.5;
         mtx += 0.5;
         mtx = mtx * 2-1;
         uv = mul ( uv, mtx );
         uv += 0.5;
         return uv;
      }

      float4 DecodeToFloat4(float v)
      {
         uint vi = (uint)(v * (256.0f * 256.0f * 256.0f * 256.0f));
         int ex = (int)(vi / (256 * 256 * 256) % 256);
         int ey = (int)((vi / (256 * 256)) % 256);
         int ez = (int)((vi / (256)) % 256);
         int ew = (int)(vi % 256);
         float4 e = float4(ex / 255.0, ey / 255.0, ez / 255.0, ew / 255.0);
         return e;
      }

      

      struct Input 
      {
         ShaderData shaderData;
         float2 uv_Control0;
         float2 uv2_Diffuse;

         float worldHeight;
         float3 worldUpVector;

         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         float4 color;
         float3x3 TBN;

         // vertex/digger workflow data
         half4 w0;
         half4 w1;
         half4 w2;
         half4 w3;
         half4 w4;
         half4 w5;
         half4 w6;
         
         // megasplat data
         half4 layer0;
         half4 layer1;
         half3 baryWeights;
         half4 scatter0;
         half4 scatter1;

         // wetness, puddles, streams, lava from vertex or megasplat
         half4 fx;
         // snow min, snow max
         half4 fx2;


      };
      
      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
         half3 pN0;
         half3 pN1;
         half3 pN2;
         half3 pN3;
         half3 axisSign;
         Input IN;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;

         half4 cluster0;
         half4 cluster1;
         half4 cluster2;
         half4 cluster3;

      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
         #if _USESPECULARWORKFLOW
         half3 Specular;
         #endif
         half Alpha;
         
      };


      

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         #if _SURFACENORMALS
            half3 surf0;
            half3 surf1;
            half3 surf2;
            half3 surf3;
         #endif

         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
         

         #if _USEEMISSIVEMETAL || _GLOBALEMIS || _GLOBALSMOOTHAOMETAL || _PERTEXSSS || _PERTEXRIMLIGHT
            half4 emisMetal0;
            half4 emisMetal1;
            half4 emisMetal2;
            half4 emisMetal3;
         #endif

         #if _USESPECULARWORKFLOW
            half3 specular0;
            half3 specular1;
            half3 specular2;
            half3 specular3;
         #endif
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
         #if _SURFACENORMALS
            s.surf0 = half3(0,0,1);
            s.surf1 = half3(0,0,1);
            s.surf2 = half3(0,0,1);
            s.surf3 = half3(0,0,1);
         #endif
      }

       float3 GetGlobalLightDir(Input i)
      {
         float3 lightDir = float3(1,0,0);

         #if _HDRP || PASS_DEFERRED
            lightDir = normalize(_gGlitterLightDir.xyz);
         #elif _URP
            lightDir = GetMainLight().direction;
         #else
            #ifndef USING_DIRECTIONAL_LIGHT
               lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            #else
               lightDir = normalize(_WorldSpaceLightPos0.xyz);
            #endif
         #endif
         return lightDir;
      }

      float3x3 GetTBN(Input i)
      {
         return i.TBN;
      }
      
      float3 GetGlobalLightDirTS(Input i)
      {
         float3 lightDirWS = GetGlobalLightDir(i);
         return mul(GetTBN(i), lightDirWS);
      }
      
      half3 GetGlobalLightColor()
      {
         #if _HDRP || PASS_DEFERRED
            return _gGlitterLightColor;
         #elif _URP
            return (GetMainLight().color);
         #else
            return _LightColor0.rgb;
         #endif
      }

      

      half3 FuzzyShade(half3 color, half3 normal, half coreMult, half edgeMult, half power, float3 viewDir)
      {
         half dt = saturate(dot(viewDir, normal));
         half dark = 1.0 - (coreMult * dt);
         half edge = pow(1-dt, power) * edgeMult;
         return color * (dark + edge);
      }

      half3 ComputeSSS(Input i, float3 V, float3 N, half3 tint, half thickness, half distortion, half scale, half power)
      {
         float3 L = GetGlobalLightDir(i);
         half3 lightColor = GetGlobalLightColor();
         float3 H = normalize(L + N * distortion);
         float VdotH = pow(saturate(dot(V, -H)), power) * scale;
         float3 I =  (VdotH) * thickness;
         return lightColor * I * tint;
      }


      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif
      

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##3 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv3.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #endif

      half2 BlendNormal2(half2 base, half2 blend) { return normalize(half3(base.xy + blend.xy, 1)).xy; } 
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }
      half3 BlendLighterColor(half3 s, half3 d) { return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d; } 
      
      
      #if _SURFACENORMALS  

      #define HALF_EPS 4.8828125e-4    // 2^-11, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)

      

      void ConstructSurfaceGradientTBN(Input i)
      {
         float3x3 tbn = GetTBN(i);
         float3 t = tbn[0];
         float3 b = tbn[1];
         float3 n = tbn[2];

         surfNormal = n;//mul(GetWorldToObjectMatrix(), float4(n, 1)).xyz;
         surfTangent = t;//mul(GetWorldToObjectMatrix(), float4(t, 1)).xyz;
         surfBitangent = b;//cross(surfNormal, surfTangent);
         
         float renormFactor = 1.0 / length(surfNormal);
         surfNormal    *= renormFactor;
         surfTangent   *= renormFactor;
         surfBitangent *= renormFactor;
      }
      
      half3 SurfaceGradientFromTBN(half2 deriv)
      {
          return deriv.x * surfTangent + deriv.y * surfBitangent;
      }

      // Input: vM is tangent space normal in [-1;1].
      // Output: convert vM to a derivative.
      half2 TspaceNormalToDerivative(half3 vM)
      {
         const half scale = 1.0/128.0;
         
         // Ensure vM delivers a positive third component using abs() and
         // constrain vM.z so the range of the derivative is [-128; 128].
         const half3 vMa = abs(vM);
         const half z_ma = max(vMa.z, scale*max(vMa.x, vMa.y));

         return -half2(vM.x, vM.y)/z_ma;
      }

      // Used to produce a surface gradient from the gradient of a volume
      // bump function such as 3D Perlin noise. Equation 2 in [Mik10].
      half3 SurfgradFromVolumeGradient(half3 grad)
      {
         return grad - dot(surfNormal, grad) * surfNormal;
      }

      half3 SurfgradFromTriplanarProjection(half3 pN, half2 xPlaneTN, half2 yPlaneTN, half2 zPlaneTN)
      {
         const half w0 = pN.x;
         const half w1 = pN.y;
         const half w2 = pN.z;
         
         // X-plane tangent normal to gradient derivative
         xPlaneTN = xPlaneTN * 2.0 - 1.0;
         half xPlaneRcpZ = rsqrt(max(1 - dot(xPlaneTN.x, xPlaneTN.x) - dot(xPlaneTN.y, xPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_xplane = xPlaneTN * -xPlaneRcpZ;

         // Y-plane tangent normal to gradient derivative
         yPlaneTN = yPlaneTN * 2.0 - 1.0;
         half yPlaneRcpZ = rsqrt(max(1 - dot(yPlaneTN.x, yPlaneTN.x) - dot(yPlaneTN.y, yPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_yplane = yPlaneTN * -yPlaneRcpZ;

         // Z-plane tangent normal to gradient derivative
         zPlaneTN = zPlaneTN * 2.0 - 1.0;
         half zPlaneRcpZ = rsqrt(max(1 - dot(zPlaneTN.x, zPlaneTN.x) - dot(zPlaneTN.y, zPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_zplane = zPlaneTN * -zPlaneRcpZ;

         // Assume deriv xplane, deriv yplane, and deriv zplane are
         // sampled using (z,y), (x,z), and (x,y), respectively.
         // Positive scales of the lookup coordinate will work
         // as well, but for negative scales the derivative components
         // will need to be negated accordingly.
         float3 grad = float3(w2*d_zplane.x + w1*d_yplane.x,
                              w2*d_zplane.y + w0*d_xplane.y,
                              w0*d_xplane.x + w1*d_yplane.y);

         return SurfgradFromVolumeGradient(grad);
      }

      half3 ConvertNormalToGradient(half3 normal)
      {
         half2 deriv = TspaceNormalToDerivative(normal);

         return SurfaceGradientFromTBN(deriv);
      }

      half3 ConvertNormal2ToGradient(half2 packedNormal)
      {
         half2 tNormal = packedNormal;
         half rcpZ = rsqrt(max(1 - dot(tNormal.x, tNormal.x) - dot(tNormal.y, tNormal.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
         half2 deriv = tNormal * -rcpZ;
         return SurfaceGradientFromTBN(deriv);
      }


      half3 ResolveNormalFromSurfaceGradient(half3 gradient)
      {
         return normalize(surfNormal - gradient);
      }
      

      #endif // _SURFACENORMALS

      void BlendNormalPerTex(inout RawSamples o, half2 noise, float4 fades)
      {
         #if _SURFACENORMALS
            float3 grad = ConvertNormal2ToGradient(noise.xy);
            o.surf0 += grad * fades.x;
            o.surf1 += grad * fades.y;
            #if !_MAX2LAYER
               o.surf2 += grad * fades.z;
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.surf3 += grad * fades.w;
            #endif
         #else
            o.normSAO0.xy = lerp(o.normSAO0.xy, BlendNormal2(o.normSAO0.xy, noise.xy), fades.x);
            o.normSAO1.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #if !_MAX2LAYER
               o.normSAO2.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO2.xy, noise.xy), fades.y);
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.normSAO3.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #endif
         #endif
      }
      
     
      
      half3 BlendNormal3(half3 n1, half3 n2)
      {
         n1 += float3( 0,  0, 1);
         n2 *= float3(-1, -1, 1);
         return n1*dot(n1, n2) / n1.z - n2;
      }
      
      half2 TransformTriplanarNormal(Input IN, float3x3 t2w, half3 axisSign, half3 absVertNormal,
               half3 pN, half2 a0, half2 a1, half2 a2)
      {
         
         a0 = a0 * 2 - 1;
         a1 = a1 * 2 - 1;
         a2 = a2 * 2 - 1;

         a0.x *= axisSign.x;
         a1.x *= axisSign.y;
         a2.x *= axisSign.z;
         
         half3 n0 = half3(a0.xy, 1);
         half3 n1 = half3(a1.xy, 1);
         half3 n2 = half3(a2.xy, 1);

         float3 wn = IN.worldNormal;

         n0 = BlendNormal3(half3(wn.zy, absVertNormal.x), n0);
         n1 = BlendNormal3(half3(wn.xz, absVertNormal.y), n1 * float3(-1, 1, 1)); 
         n2 = BlendNormal3(half3(wn.xy, absVertNormal.z), n2);
  
         n0.z *= axisSign.x;
         n1.z *= axisSign.y;
         n2.z *= -axisSign.z;

         half3 worldNormal = (n0.zyx * pN.x + n1.xzy * pN.y + n2.xyz * pN.z);
         return mul(t2w, worldNormal).xy;

      }
      
      // funcs
      
      inline half MSLuminance(half3 rgb)
      {
         #ifdef UNITY_COLORSPACE_GAMMA
            return dot(rgb, half3(0.22, 0.707, 0.071));
         #else
            return dot(rgb, half3(0.0396819152, 0.458021790, 0.00609653955));
         #endif
      }
      
      
      float2 Hash2D( float2 x )
      {
          float2 k = float2( 0.3183099, 0.3678794 );
          x = x*k + k.yx;
          return -1.0 + 2.0*frac( 16.0 * k*frac( x.x*x.y*(x.x+x.y)) );
      }

      float Noise2D(float2 p )
      {
         float2 i = floor( p );
         float2 f = frac( p );
         
         float2 u = f*f*(3.0-2.0*f);

         return lerp( lerp( dot( Hash2D( i + float2(0.0,0.0) ), f - float2(0.0,0.0) ), 
                           dot( Hash2D( i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                      lerp( dot( Hash2D( i + float2(0.0,1.0) ), f - float2(0.0,1.0) ), 
                           dot( Hash2D( i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
      }
      
      float FBM2D(float2 uv)
      {
         float f = 0.5000*Noise2D( uv ); uv *= 2.01;
         f += 0.2500*Noise2D( uv ); uv *= 1.96;
         f += 0.1250*Noise2D( uv ); 
         return f;
      }
      
      float3 Hash3D( float3 p )
      {
         p = float3( dot(p,float3(127.1,311.7, 74.7)),
                 dot(p,float3(269.5,183.3,246.1)),
                 dot(p,float3(113.5,271.9,124.6)));

         return -1.0 + 2.0*frac(sin(p)*437.5453123);
      }

      float Noise3D( float3 p )
      {
         float3 i = floor( p );
         float3 f = frac( p );
         
         float3 u = f*f*(3.0-2.0*f);

         return lerp( lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,0.0) ), f - float3(0.0,0.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,0.0) ), f - float3(1.0,0.0,0.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,0.0) ), f - float3(0.0,1.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,0.0) ), f - float3(1.0,1.0,0.0) ), u.x), u.y),
                      lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,1.0) ), f - float3(0.0,0.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,1.0) ), f - float3(1.0,0.0,1.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,1.0) ), f - float3(0.0,1.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,1.0) ), f - float3(1.0,1.0,1.0) ), u.x), u.y), u.z );
      }
      
      float FBM3D(float3 uv)
      {
         float f = 0.5000*Noise3D( uv ); uv *= 2.01;
         f += 0.2500*Noise3D( uv ); uv *= 1.96;
         f += 0.1250*Noise3D( uv ); 
         return f;
      }
      
     
      
      float GetSaturation(float3 c)
      {
         float mi = min(min(c.x, c.y), c.z);
         float ma = max(max(c.x, c.y), c.z);
         return (ma - mi)/(ma + 1e-7);
      }

      // Better Color Lerp, does not have darkening issue
      float3 BetterColorLerp(float3 a, float3 b, float x)
      {
         float3 ic = lerp(a, b, x) + float3(1e-6,0.0,0.0);
         float sd = abs(GetSaturation(ic) - lerp(GetSaturation(a), GetSaturation(b), x));
    
         float3 dir = normalize(float3(2.0 * ic.x - ic.y - ic.z, 2.0 * ic.y - ic.x - ic.z, 2.0 * ic.z - ic.y - ic.x));
         float lgt = dot(float3(1.0, 1.0, 1.0), ic);
    
         float ff = dot(dir, normalize(ic));
    
         const float dsp_str = 1.5;
         ic += dsp_str * dir * sd * ff * lgt;
         return saturate(ic);
      }
      
      
      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);
             half4 weights = half4(iWeights.x * max(h0,0.001), iWeights.y * max(h1,0.001), iWeights.z * max(h2,0.001), iWeights.w * max(h3,0.001));
             
             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);

             weights = TotalOne(weights);
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #elif _MAX20TEXTURES
         #define TEXCOUNT 20
      #elif _MAX24TEXTURES
         #define TEXCOUNT 24
      #elif _MAX28TEXTURES
         #define TEXCOUNT 28
      #elif _MAX32TEXTURES
         #define TEXCOUNT 32
      #else
         #define TEXCOUNT 16
      #endif

      #if _DECAL_SPLAT
      
      void DoMergeDecalSplats(half4 iWeights, half4 iIndexes, inout half4 indexes, inout half4 weights)
      {
         for (int i = 0; i < 4; ++i)
         {
            half w = iWeights[i];
            half index = iIndexes[i];
            if (w > weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = index;
            }
            else if (w > weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = index;
            }
            else if (w > weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = index;
            }
            else if (w > weights[3])
            {
               weights[3] = w;
               indexes[3] = index;
            }
         }

      }
      #endif


      void Setup(out half4 weights, float2 uv, out Config config, half4 w0, half4 w1, half4 w2, half4 w3, half4 w4, half4 w5, half4 w6, half4 w7, float3 worldPos, DecalOutput decalOutput)
      {
         config = (Config)0;
         half4 indexes = 0;

         config.uv = uv;

         #if _WORLDUV
         uv = worldPos.xz;
         #endif

         #if _DISABLESPLATMAPS
            float2 scaledUV = uv;
         #else
            float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;
         #endif

         // if only 4 textures, and blending 4 textures, skip this whole thing..
         // this saves about 25% of the ALU of the base shader on low end. However if
         // we rely on sorted texture weights (distance resampling) we have to sort..
         float4 defaultIndexes = float4(0,1,2,3);
         #if _MESHSUBARRAY
            defaultIndexes = _MeshSubArrayIndexes;
         #endif

         #if _MESHSUBARRAY && !_DECAL_SPLAT || (_MAX4TEXTURES && !_MAX3LAYER && !_MAX2LAYER && !_DISTANCERESAMPLE && !_POM && !_DECAL_SPLAT)
            weights = w0;
            config.uv0 = float3(scaledUV, defaultIndexes.x);
            config.uv1 = float3(scaledUV, defaultIndexes.y);
            config.uv2 = float3(scaledUV, defaultIndexes.z);
            config.uv3 = float3(scaledUV, defaultIndexes.w);
            return;
         #endif

         #if _DISABLESPLATMAPS
            weights = float4(1,0,0,0);
            return;
         #else
            half splats[TEXCOUNT];

            splats[0] = w0.x;
            splats[1] = w0.y;
            splats[2] = w0.z;
            splats[3] = w0.w;
            #if !_MAX4TEXTURES
               splats[4] = w1.x;
               splats[5] = w1.y;
               splats[6] = w1.z;
               splats[7] = w1.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES
               splats[8] = w2.x;
               splats[9] = w2.y;
               splats[10] = w2.z;
               splats[11] = w2.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
               splats[12] = w3.x;
               splats[13] = w3.y;
               splats[14] = w3.z;
               splats[15] = w3.w;
            #endif
            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[16] = w4.x;
               splats[17] = w4.y;
               splats[18] = w4.z;
               splats[19] = w4.w;
            #endif
            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[20] = w5.x;
               splats[21] = w5.y;
               splats[22] = w5.z;
               splats[23] = w5.w;
            #endif
            #if _MAX28TEXTURES || _MAX32TEXTURES
               splats[24] = w6.x;
               splats[25] = w6.y;
               splats[26] = w6.z;
               splats[27] = w6.w;
            #endif
            #if _MAX32TEXTURES
               splats[28] = w7.x;
               splats[29] = w7.y;
               splats[30] = w7.z;
               splats[31] = w7.w;
            #endif



            weights[0] = 0;
            weights[1] = 0;
            weights[2] = 0;
            weights[3] = 0;
            indexes[0] = 0;
            indexes[1] = 0;
            indexes[2] = 0;
            indexes[3] = 0;

            int i = 0;
            for (i = 0; i < TEXCOUNT; ++i)
            {
               half w = splats[i];
               if (w >= weights[0])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = weights[0];
                  indexes[1] = indexes[0];
                  weights[0] = w;
                  indexes[0] = i;
               }
               else if (w >= weights[1])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = w;
                  indexes[1] = i;
               }
               else if (w >= weights[2])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = w;
                  indexes[2] = i;
               }
               else if (w >= weights[3])
               {
                  weights[3] = w;
                  indexes[3] = i;
               }
            }

            #if _DECAL_SPLAT
               DoMergeDecalSplats(decalOutput.Weights, decalOutput.Indexes, weights, indexes); 
            #endif


            
            
            // clamp and renormalize
            #if _MAX2LAYER
               weights.zw = 0;
               weights.xy = TotalOne(weights.xy);
            #elif _MAX3LAYER
               weights.w = 0;
               weights.xyz = TotalOne(weights.xyz);
            #elif !_DISABLEHEIGHTBLENDING || _NORMALIZEWEIGHTS // prevents black when painting, which the unity shader does not prevent.
               weights = normalize(weights);
            #endif
            

            config.uv0 = float3(scaledUV, indexes.x);
            config.uv1 = float3(scaledUV, indexes.y);
            config.uv2 = float3(scaledUV, indexes.z);
            config.uv3 = float3(scaledUV, indexes.w);


         #endif //_DISABLESPLATMAPS


      }

      float3 HeightToNormal(float height, float3 worldPos)
      {
         float3 dx = ddx(worldPos);
         float3 dy = ddy(worldPos);
         float3 crossX = cross(float3(0,1,0), dx);
         float3 crossY = cross(float3(0,1,0), dy);
         float3 d = abs(dot(crossY, dx));
         float3 n = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
         n.z *= -1;
         return normalize((d * float3(0,1,0)) - n).xzy;
      }
      
      float ComputeMipLevel(float2 uv, float2 textureSize)
      {
         uv *= textureSize;
         float2  dx_vtc        = ddx(uv);
         float2  dy_vtc        = ddy(uv);
         float delta_max_sqr   = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
         return 0.5 * log2(delta_max_sqr);
      }

      inline half2 UnpackNormal2(half4 packednormal)
      {
          return packednormal.wy * 2 - 1;
         
      }

      half3 TriplanarHBlend(half h0, half h1, half h2, half3 pN, half contrast)
      {
         half3 blend = pN / dot(pN, half3(1,1,1));
         float3 heights = float3(h0, h1, h2) + (blend * 3.0);
         half height_start = max(max(heights.x, heights.y), heights.z) - contrast;
         half3 h = max(heights - height_start.xxx, half3(0,0,0));
         blend = h / dot(h, half3(1,1,1));
         return blend;
      }
      

      void ClearAllButAlbedo(inout MicroSplatLayer o, half3 display)
      {
         o.Albedo = display.rgb;
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

      void ClearAllButAlbedo(inout MicroSplatLayer o, half display)
      {
         o.Albedo = half3(display, display, display);
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

     

      half MicroShadow(float3 lightDir, half3 normal, half ao, half strength)
      {
         half shadow = saturate(abs(dot(normal, lightDir)) + (ao * ao * 2.0) - 1.0);
         return 1 - ((1-shadow) * strength);
      }
      

      void DoDebugOutput(inout MicroSplatLayer l)
      {
         #if _DEBUG_OUTPUT_ALBEDO
            ClearAllButAlbedo(l, l.Albedo);
         #elif _DEBUG_OUTPUT_NORMAL
            // oh unit shader compiler normal stripping, how I hate you so..
            // must multiply by albedo to stop the normal from being white. Why, fuck knows?
            ClearAllButAlbedo(l, float3(l.Normal.xy * 0.5 + 0.5, l.Normal.z * saturate(l.Albedo.z+1)));
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            ClearAllButAlbedo(l, l.Smoothness.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_METAL
            ClearAllButAlbedo(l, l.Metallic.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_AO
            ClearAllButAlbedo(l, l.Occlusion.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_EMISSION
            ClearAllButAlbedo(l, l.Emission * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_HEIGHT
            ClearAllButAlbedo(l, l.Height.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_SPECULAR && _USESPECULARWORKFLOW
            ClearAllButAlbedo(l, l.Specular * saturate(l.Albedo.z+1));
         #elif _DEBUG_BRANCHCOUNT_WEIGHT
            ClearAllButAlbedo(l, _branchWeightCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TRIPLANAR
            ClearAllButAlbedo(l, _branchTriplanarCount / 24 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_CLUSTER
            ClearAllButAlbedo(l, _branchClusterCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_OTHER
            ClearAllButAlbedo(l, _branchOtherCount / 8 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TOTAL
            l.Albedo.r = _branchWeightCount / 12;
            l.Albedo.g = _branchTriplanarCount / 24;
            l.Albedo.b = _branchClusterCount / 12;
            ClearAllButAlbedo(l, (l.Albedo.r + l.Albedo.g + l.Albedo.b + (_branchOtherCount / 8)) / 4); 
         #elif _DEBUG_OUTPUT_MICROSHADOWS
            ClearAllButAlbedo(l,l.Albedo); 
         #elif _DEBUG_SAMPLECOUNT
            float sdisp = (float)_sampleCount / max(_SampleCountDiv, 1);
            half3 sdcolor = float3(sdisp, sdisp > 1 ? 1 : 0, 0);
            ClearAllButAlbedo(l, sdcolor * saturate(l.Albedo.z + 1));
         #elif _DEBUG_PROCLAYERS
            ClearAllButAlbedo(l, (float)_procLayerCount / (float)_PCLayerCount * saturate(l.Albedo.z + 1));
         #endif
      }


      // abstraction around sampler mode
      #if _USELODMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_LOD(tex, sampler##tex, u, l.x)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u, l.x)
      #elif _USEGRADMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_GRAD(tex, sampler##tex, u, l.xy, l.zw)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY_GRAD(tex, ss, u.xy, u.z, l.xy, l.zw)
      #else
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, u.xy, u.z)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u.xy, y.z)
      #endif


      #define MICROSPLAT_SAMPLE_DIFFUSE(u, cl, l) MICROSPLAT_SAMPLE(_Diffuse, u, l)
      #define MICROSPLAT_SAMPLE_EMIS(u, cl, l) MICROSPLAT_SAMPLE(_EmissiveMetal, u, l)
      #define MICROSPLAT_SAMPLE_DIFFUSE_LOD(u, cl, l) UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, u, l)
      

      #if _PACKINGHQ
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) half4(MICROSPLAT_SAMPLE(_NormalSAO, u, l).ga, MICROSPLAT_SAMPLE(_SmoothAO, u, l).ga).brag
      #else
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) MICROSPLAT_SAMPLE(_NormalSAO, u, l)
      #endif

      #if _USESPECULARWORKFLOW
         #define MICROSPLAT_SAMPLE_SPECULAR(u, cl, l) MICROSPLAT_SAMPLE(_Specular, u, l)
      #endif
      
      struct SimpleTriplanarConfig
      {
         float3 pn;
         float2 uv0;
         float2 uv1;
         float2 uv2;
      };
         
      void PrepSimpleTriplanarConfig(inout SimpleTriplanarConfig tc, float3 worldPos, float3 normal, float contrast)
      {
         tc.pn = pow(abs(normal), contrast);
         tc.pn = tc.pn / (tc.pn.x + tc.pn.y + tc.pn.z);
         
         half3 axisSign = sign(normal);

         tc.uv0 = worldPos.zy * axisSign.x;
         tc.uv1 = worldPos.xz * axisSign.y;
         tc.uv2 = worldPos.xy * axisSign.z;
      }
      
      #define SimpleTriplanarSample(tex, tc, scale) (SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv0 * scale) * tc.pn.x + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv1 * scale) * tc.pn.y + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv2 * scale) * tc.pn.z)
      #define SimpleTriplanarSampleLOD(tex, tc, scale, lod) (SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv0 * scale, lod) * tc.pn.x + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv1 * scale, lod) * tc.pn.y + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv2 * scale, lod) * tc.pn.z)
      #define SimpleTriplanarSampleGrad(tex, tc, scale) (SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv0 * scale, ddx(tc.uv0) * scale, ddy(tc.uv0) * scale) * tc.pn.x + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv1 * scale, ddx(tc.uv1) * scale, ddy(tc.uv1) * scale) * tc.pn.y + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv2 * scale, ddx(tc.uv2) * scale, ddy(tc.uv2) * scale) * tc.pn.z)
   
      
      inline half3 MicroSplatDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
      {
          specColor = lerp (half3(0,0,0), albedo, metallic);
          oneMinusReflectivity = (1-metallic);
          return albedo * oneMinusReflectivity;
      }







      Input DescToInput(ShaderData IN)
      {
        Input s = (Input)0;
        s.shaderData = IN;
        s.TBN = IN.TBNMatrix;
        s.worldNormal = IN.worldSpaceNormal;
        s.worldPos = IN.worldSpacePosition;
        s.viewDir = IN.tangentSpaceViewDir;
        s.uv_Control0 = IN.texcoord0.xy;

        s.worldUpVector = float3(0,1,0);
        s.worldHeight = IN.worldSpacePosition.y;
  
        #if _PLANETVECTORS
            float3 rwp = mul(_PQSToLocal, float4(IN.worldSpacePosition, 1));
            s.worldHeight = distance(rwp, float3(0,0,0));
            s.worldUpVector = normalize(rwp);
        #endif

        #if _MICROMESH && _MESHUV2
            s.uv2_Diffuse = IN.texcoord1.xy;
        #endif

        #if _MEGASPLAT
           UnpackMegaSplat(s, IN);
        #endif
   
        #if _MICROVERTEXMESH || _MICRODIGGERMESH
            UnpackVertexWorkflow(s, IN);
        #endif

        #if _PLANETVECTORS
           DoPlanetDataInputCopy(s, IN);
        #endif
        
        return s;
     }
     


      void SampleAlbedo(inout Config config, inout TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
         
            half4 contrasts = _Contrast.xxxx;
            #if _PERTEXTRIPLANARCONTRAST
               SAMPLE_PER_TEX(ptc, 9.5, config, half4(1,0.5,0,0));
               contrasts = half4(ptc0.y, ptc1.y, ptc2.y, ptc3.y);
            #endif


            #if _PERTEXTRIPLANAR
               SAMPLE_PER_TEX(pttri, 9.5, config, half4(0,0,0,0));
            #endif

            {
               // For per-texture triplanar, we modify the view based blending factor of the triplanar
               // such that you get a pure blend of either top down projection, or with the top down projection
               // removed and renormalized. This causes dynamic flow control optimizations to kick in and avoid
               // the extra texture samples while keeping the code simple. Yay..

               // We also only have to do this in the Albedo, because the pN values will be adjusted after the
               // albedo is sampled, causing future samples to use this data. 
              
               #if _PERTEXTRIPLANAR
                  if (pttri0.x > 0.66)
                  {
                     tc.pN0 = half3(0,1,0);
                  }
                  else if (pttri0.x > 0.33)
                  {
                     tc.pN0.y = 0;
                     tc.pN0.xz = TotalOne(tc.pN0.xz);
                  }
               #endif


               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[0], config.cluster0, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[1], config.cluster0, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[2], config.cluster0, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN0;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN0, contrasts.x);
                  tc.pN0 = bf;
               #endif

               s.albedo0 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            MSBRANCH(weights.y)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri1.x > 0.66)
                  {
                     tc.pN1 = half3(0,1,0);
                  }
                  else if (pttri1.x > 0.33)
                  {
                     tc.pN1.y = 0;
                     tc.pN1.xz = TotalOne(tc.pN1.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[0], config.cluster1, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[1], config.cluster1, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  COUNTSAMPLE
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[2], config.cluster1, d2);
               }
               half3 bf = tc.pN1;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN1, contrasts.x);
                  tc.pN1 = bf;
               #endif


               s.albedo1 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri2.x > 0.66)
                  {
                     tc.pN2 = half3(0,1,0);
                  }
                  else if (pttri2.x > 0.33)
                  {
                     tc.pN2.y = 0;
                     tc.pN2.xz = TotalOne(tc.pN2.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[0], config.cluster2, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[1], config.cluster2, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[2], config.cluster2, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN2;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN2, contrasts.x);
                  tc.pN2 = bf;
               #endif
               

               s.albedo2 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {

               #if _PERTEXTRIPLANAR
                  if (pttri3.x > 0.66)
                  {
                     tc.pN3 = half3(0,1,0);
                  }
                  else if (pttri3.x > 0.33)
                  {
                     tc.pN3.y = 0;
                     tc.pN3.xz = TotalOne(tc.pN3.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[0], config.cluster3, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[1], config.cluster3, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[2], config.cluster3, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN3;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN3, contrasts.x);
               tc.pN3 = bf;
               #endif

               s.albedo3 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif

         #else
            s.albedo0 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv0, config.cluster0, mipLevel);
            COUNTSAMPLE

            MSBRANCH(weights.y)
            {
               s.albedo1 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv1, config.cluster1, mipLevel);
               COUNTSAMPLE
            }
            #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.albedo2 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               } 
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.albedo3 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
            #endif
         #endif

         #if _PERTEXHEIGHTOFFSET || _PERTEXHEIGHTCONTRAST
            SAMPLE_PER_TEX(ptHeight, 10.5, config, 1);

            #if _PERTEXHEIGHTOFFSET
               s.albedo0.a = saturate(s.albedo0.a + ptHeight0.b - 1);
               s.albedo1.a = saturate(s.albedo1.a + ptHeight1.b - 1);
               s.albedo2.a = saturate(s.albedo2.a + ptHeight2.b - 1);
               s.albedo3.a = saturate(s.albedo3.a + ptHeight3.b - 1);
            #endif
            #if _PERTEXHEIGHTCONTRAST
               s.albedo0.a = saturate(pow(s.albedo0.a + 0.5, abs(ptHeight0.a)) - 0.5);
               s.albedo1.a = saturate(pow(s.albedo1.a + 0.5, abs(ptHeight1.a)) - 0.5);
               s.albedo2.a = saturate(pow(s.albedo2.a + 0.5, abs(ptHeight2.a)) - 0.5);
               s.albedo3.a = saturate(pow(s.albedo3.a + 0.5, abs(ptHeight3.a)) - 0.5);
            #endif
         #endif
      }
      
      
      
      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif

         #if _NONORMALMAP || _AUTONORMAL
            s.normSAO0 = half4(0,0, 0, 1);
            s.normSAO1 = half4(0,0, 0, 1);
            s.normSAO2 = half4(0,0, 0, 1);
            s.normSAO3 = half4(0,0, 0, 1);
            return;
         #endif

         
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
            
            half3 absVertNormal = abs(tc.IN.worldNormal);
            float3x3 t2w = tc.IN.TBN;
            
            
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[0], config.cluster0, d0).agrb;
                  COUNTSAMPLE
               }            
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[1], config.cluster0, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[2], config.cluster0, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf0 = SurfgradFromTriplanarProjection(tc.pN0, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO0.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN0, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO0.zw = a0.zw * tc.pN0.x + a1.zw * tc.pN0.y + a2.zw * tc.pN0.z;
            }
            MSBRANCH(weights.y)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[0], config.cluster1, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[1], config.cluster1, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[2], config.cluster1, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf1 = SurfgradFromTriplanarProjection(tc.pN1, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO1.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN1, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO1.zw = a0.zw * tc.pN1.x + a1.zw * tc.pN1.y + a2.zw * tc.pN1.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);

               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[0], config.cluster2, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[1], config.cluster2, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[2], config.cluster2, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf2 = SurfgradFromTriplanarProjection(tc.pN2, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO2.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN2, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO2.zw = a0.zw * tc.pN2.x + a1.zw * tc.pN2.y + a2.zw * tc.pN2.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[0], config.cluster3, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[1], config.cluster3, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[2], config.cluster3, d2).agrb;
                  COUNTSAMPLE
               }

               #if _SURFACENORMALS
                  s.surf3 = SurfgradFromTriplanarProjection(tc.pN3, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO3.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN3, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO3.zw = a0.zw * tc.pN3.x + a1.zw * tc.pN3.y + a2.zw * tc.pN3.z;
            }
            #endif

         #else
            s.normSAO0 = MICROSPLAT_SAMPLE_NORMAL(config.uv0, config.cluster0, mipLevel).agrb;
            COUNTSAMPLE
            s.normSAO0.xy = s.normSAO0.xy * 2 - 1;

            #if _SURFACENORMALS
               s.surf0 = ConvertNormal2ToGradient(s.normSAO0.xy);
            #endif

            MSBRANCH(weights.y)
            {
               s.normSAO1 = MICROSPLAT_SAMPLE_NORMAL(config.uv1, config.cluster1, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO1.xy = s.normSAO1.xy * 2 - 1;

               #if _SURFACENORMALS
                  s.surf1 = ConvertNormal2ToGradient(s.normSAO1.xy);
               #endif
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               s.normSAO2 = MICROSPLAT_SAMPLE_NORMAL(config.uv2, config.cluster2, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf2 = ConvertNormal2ToGradient(s.normSAO2.xy);
               #endif
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               s.normSAO3 = MICROSPLAT_SAMPLE_NORMAL(config.uv3, config.cluster3, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf3 = ConvertNormal2ToGradient(s.normSAO3.xy);
               #endif
            }
            #endif
         #endif
      }

      void SampleEmis(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USEEMISSIVEMETAL
            #if _TRIPLANAR
            
               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  s.emisMetal0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }

                  s.emisMetal1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.emisMetal0 = MICROSPLAT_SAMPLE_EMIS(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.emisMetal1 = MICROSPLAT_SAMPLE_EMIS(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
                  MSBRANCH(weights.z)
                  {
                     s.emisMetal2 = MICROSPLAT_SAMPLE_EMIS(config.uv2, config.cluster2, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  MSBRANCH(weights.w)
                  {
                     s.emisMetal3 = MICROSPLAT_SAMPLE_EMIS(config.uv3, config.cluster3, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
            #endif
         #endif
      }
      
      void SampleSpecular(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USESPECULARWORKFLOW
            #if _TRIPLANAR

               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.specular0 = MICROSPLAT_SAMPLE_SPECULAR(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.specular1 = MICROSPLAT_SAMPLE_SPECULAR(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.specular2 = MICROSPLAT_SAMPLE_SPECULAR(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.specular3 = MICROSPLAT_SAMPLE_SPECULAR(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
               #endif
            #endif
         #endif
      }

      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex, DecalOutput decalOutput)
      {
         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         half4 albedo = 0;
         half4 normSAO = half4(0,0,0,1);
         half3 surfGrad = half3(0,0,0);
         half4 emisMetal = 0;
         half3 specular = 0;
         
         float worldHeight = i.worldPos.y;
         float3 upVector = float3(0,1,0);
         
         #if _GLOBALTINT || _GLOBALNORMALS || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _GLOBALSPECULAR
            float globalSlopeFilter = 1;
            #if _GLOBALSLOPEFILTER
               float2 gfilterUV = float2(1 - saturate(dot(worldNormalVertex, upVector) * 0.5 + 0.49), 0.5);
               globalSlopeFilter = SAMPLE_TEXTURE2D(_GlobalSlopeTex, sampler_Diffuse, gfilterUV).a;
            #endif
         #endif

         // declare outside of branchy areas..
         half4 fxLevels = half4(0,0,0,0);
         half burnLevel = 0;
         half wetLevel = 0;
         half3 waterNormalFoam = half3(0, 0, 0);
         half porosity = 0.4;
         float streamFoam = 1.0f;
         half pud = 0;
         half snowCover = 0;
         half SSSThickness = 0;
         half3 SSSTint = half3(1,1,1);
         float traxBuffer = 0;
         float3 traxNormal = 0;
         float2 noiseUV = 0;
         
         

         #if _SPLATFADE
         MSBRANCHOTHER(1 - saturate(camDist - _SplatFade.y))
         {
         #endif

         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE || _SNOWFOOTSTEPS
            traxBuffer = SampleTraxBuffer(i.worldPos, worldNormalVertex, traxNormal);
         #endif
         
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
            #if _MICROMESH
               fxLevels = SampleFXLevels(InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, config.uv), wetLevel, burnLevel, traxBuffer);
            #elif _MICROVERTEXMESH || _MICRODIGGERMESH  || _MEGASPLAT
               fxLevels = ProcessFXLevels(i.fx, traxBuffer);
            #else
               fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel, traxBuffer);
            #endif
         #endif

         #if _DECAL
            fxLevels = max(fxLevels, decalOutput.fxLevels);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         

         MIPFORMAT albedoLOD = INITMIPFORMAT
         MIPFORMAT normalLOD = INITMIPFORMAT
         MIPFORMAT emisLOD = INITMIPFORMAT
         MIPFORMAT specLOD = INITMIPFORMAT
         MIPFORMAT origAlbedoLOD = INITMIPFORMAT;

         #if _TRIPLANAR && !_DISABLESPLATMAPS
            PrepTriplanar(i.shaderData.texcoord0, worldNormalVertex, i.worldPos, config, tc, weights, albedoLOD, normalLOD, emisLOD, origAlbedoLOD);
            tc.IN = i;
         #endif
         
         
         #if !_TRIPLANAR && !_DISABLESPLATMAPS
            #if _USELODMIP
               albedoLOD = ComputeMipLevel(config.uv0.xy, _Diffuse_TexelSize.zw);
               normalLOD = ComputeMipLevel(config.uv0.xy, _NormalSAO_TexelSize.zw);
               #if _USEEMISSIVEMETAL
                  emisLOD = ComputeMipLevel(config.uv0.xy, _EmissiveMetal_TexelSize.zw);
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = ComputeMipLevel(config.uv0.xy, _Specular_TexelSize.zw);;
               #endif
            #elif _USEGRADMIP
               albedoLOD = float4(ddx(config.uv0.xy), ddy(config.uv0.xy));
               normalLOD = albedoLOD;
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
            #endif

            origAlbedoLOD = albedoLOD;
         #endif

         #if _PERTEXCURVEWEIGHT
           SAMPLE_PER_TEX(ptCurveWeight, 19.5, config, half4(0.5,1,1,1));
           weights.x = lerp(smoothstep(0.5 - ptCurveWeight0.r, 0.5 + ptCurveWeight0.r, weights.x), weights.x, ptCurveWeight0.r*2);
           weights.y = lerp(smoothstep(0.5 - ptCurveWeight1.r, 0.5 + ptCurveWeight1.r, weights.y), weights.y, ptCurveWeight1.r*2);
           weights.z = lerp(smoothstep(0.5 - ptCurveWeight2.r, 0.5 + ptCurveWeight2.r, weights.z), weights.z, ptCurveWeight2.r*2);
           weights.w = lerp(smoothstep(0.5 - ptCurveWeight3.r, 0.5 + ptCurveWeight3.r, weights.w), weights.w, ptCurveWeight3.r*2);
           weights = TotalOne(weights);
         #endif
         
         

         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR && !_DISABLESPLATMAPS
            
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif

            // fix for pertex uv scale using gradient sampler and weight blended derivatives
            #if _USEGRADMIP
               albedoLOD = albedoLOD * ptUVScale0.rgrg * weights.x + 
                           albedoLOD * ptUVScale1.rgrg * weights.y + 
                           albedoLOD * ptUVScale2.rgrg * weights.z + 
                           albedoLOD * ptUVScale3.rgrg * weights.w;
               normalLOD = albedoLOD;
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
            #endif
         #endif

         #if _PERTEXUVROTATION && !_TRIPLANAR && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptUVRot, 16.5, config, half4(0,0,0,0));
            config.uv0.xy = RotateUV(config.uv0.xy, ptUVRot0.x);
            config.uv1.xy = RotateUV(config.uv1.xy, ptUVRot1.x);
            #if !_MAX2LAYER
               config.uv2.xy = RotateUV(config.uv2.xy, ptUVRot2.x);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = RotateUV(config.uv3.xy, ptUVRot0.x);
            #endif
         #endif

         
         o.Alpha = 1;

         
         #if _POM && !_DISABLESPLATMAPS
            DoPOM(i, config, tc, albedoLOD, weights, camDist, worldNormalVertex);
         #endif
         

         SampleAlbedo(config, tc, samples, albedoLOD, weights);

         #if _NOISEHEIGHT
            ApplyNoiseHeight(samples, config.uv, config, i.worldPos, worldNormalVertex);
         #endif
         
         #if _STREAMS || (_PARALLAX && !_DISABLESPLATMAPS)
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(i, config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX && !_DISABLESPLATMAPS
            DoParallax(i, earlyHeight, config, tc, samples, weights, camDist);
         #endif


         // Blend results
         #if _PERTEXINTERPCONTRAST && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = clamp(contrast + _Contrast, 0.0001, 1.0); 
            half cnt = contrast.x * weights.x + contrast.y * weights.y + contrast.z * weights.z + contrast.w * weights.w;
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, cnt);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif

         #if _HYBRIDHEIGHTBLEND
            heightWeights = lerp(heightWeights, TotalOne(weights), saturate(camDist/max(1.0, _HybridHeightBlendDistance)));
         #endif

         
         // rescale derivatives after height weighting. Basically, in gradmip mode we blend the mip levels,
         // but this is before height mapping is sampled, so reblending them after alpha will make sure the other
         // channels (normal, etc) are sharper, which likely matters most.. 
         #if _PERTEXUVSCALEOFFSET && !_DISABLESPLATMAPS
            #if _TRIPLANAR
               #if _USEGRADMIP
                  SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));

                  albedoLOD.d0 = origAlbedoLOD.d0 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d0 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d0 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d0 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d1 = origAlbedoLOD.d1 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d1 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d1 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d1 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d2 = origAlbedoLOD.d2 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d2 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d2 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d2 * ptUVScale3.xyxy * heightWeights.w;
               
                  normalLOD.d0 = albedoLOD.d0;
                  normalLOD.d1 = albedoLOD.d1;
                  normalLOD.d2 = albedoLOD.d2;
               
                  #if _USEEMISSIVEMETAL
                     emisLOD.d0 = albedoLOD.d0;
                     emisLOD.d1 = albedoLOD.d1;
                     emisLOD.d2 = albedoLOD.d2;
                  #endif
               #endif // gradmip
            #else // not triplanar
               // fix for pertex uv scale using gradient sampler and weight blended derivatives
               #if _USEGRADMIP
                  albedoLOD = origAlbedoLOD * ptUVScale0.rgrg * heightWeights.x + 
                              origAlbedoLOD * ptUVScale1.rgrg * heightWeights.y + 
                              origAlbedoLOD * ptUVScale2.rgrg * heightWeights.z + 
                              origAlbedoLOD * ptUVScale3.rgrg * heightWeights.w;
                  normalLOD = albedoLOD;
                  #if _USEEMISSIVEMETAL
                     emisLOD = albedoLOD;
                  #endif
                  #if _USESPECULARWORKFLOW
                     specLOD = albedoLOD;
                  #endif
               #endif
            #endif
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples, albedoLOD, heightWeights);
         #endif


         SampleNormal(config, tc, samples, normalLOD, heightWeights);

         #if _USEEMISSIVEMETAL
            SampleEmis(config, tc, samples, emisLOD, heightWeights);
         #endif

         #if _USESPECULARWORKFLOW
            SampleSpecular(config, tc, samples, specLOD, heightWeights);
         #endif

         #if _DISTANCERESAMPLE && !_DISABLESPLATMAPS
            DistanceResample(samples, config, tc, camDist, i.viewDir, fxLevels, albedoLOD, i.worldPos, heightWeights, worldNormalVertex);
         #endif

         #if _STARREACHFORMAT
            samples.normSAO0.w = length(samples.normSAO0.xy);
            samples.normSAO1.w = length(samples.normSAO1.xy);
            samples.normSAO2.w = length(samples.normSAO2.xy);
            samples.normSAO3.w = length(samples.normSAO3.xy);
         #endif

         // PerTexture sampling goes here, passing the samples structure
         
         #if _PERTEXMICROSHADOWS || _PERTEXFUZZYSHADE
            SAMPLE_PER_TEX(ptFuzz, 17.5, config, half4(0, 0, 1, 1));
         #endif

         #if _PERTEXMICROSHADOWS
            #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP)
            {
               half3 lightDir = GetGlobalLightDirTS(i);
               half4 microShadows = half4(1,1,1,1);
               microShadows.x = MicroShadow(lightDir, half3(samples.normSAO0.xy, 1), samples.normSAO0.a, ptFuzz0.a);
               microShadows.y = MicroShadow(lightDir, half3(samples.normSAO1.xy, 1), samples.normSAO1.a, ptFuzz1.a);
               microShadows.z = MicroShadow(lightDir, half3(samples.normSAO2.xy, 1), samples.normSAO2.a, ptFuzz2.a);
               microShadows.w = MicroShadow(lightDir, half3(samples.normSAO3.xy, 1), samples.normSAO3.a, ptFuzz3.a);
               samples.normSAO0.a *= microShadows.x;
               samples.normSAO1.a *= microShadows.y;
               #if !_MAX2LAYER
                  samples.normSAO2.a *= microShadows.z;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.normSAO3.a *= microShadows.w;
               #endif

               
               #if _DEBUG_OUTPUT_MICROSHADOWS
               o.Albedo = BlendWeights(microShadows.x, microShadows.y, microShadows.z, microShadows.a, heightWeights);
               return o;
               #endif

               

               
            }
            #endif

         #endif // _PERTEXMICROSHADOWS


         #if _PERTEXFUZZYSHADE
            samples.albedo0.rgb = FuzzyShade(samples.albedo0.rgb, half3(samples.normSAO0.rg, 1), ptFuzz0.r, ptFuzz0.g, ptFuzz0.b, i.viewDir);
            samples.albedo1.rgb = FuzzyShade(samples.albedo1.rgb, half3(samples.normSAO1.rg, 1), ptFuzz1.r, ptFuzz1.g, ptFuzz1.b, i.viewDir);
            #if !_MAX2LAYER
               samples.albedo2.rgb = FuzzyShade(samples.albedo2.rgb, half3(samples.normSAO2.rg, 1), ptFuzz2.r, ptFuzz2.g, ptFuzz2.b, i.viewDir);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = FuzzyShade(samples.albedo3.rgb, half3(samples.normSAO3.rg, 1), ptFuzz3.r, ptFuzz3.g, ptFuzz3.b, i.viewDir);
            #endif
         #endif

         #if _PERTEXSATURATION && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptSaturattion, 9.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = lerp(MSLuminance(samples.albedo0.rgb), samples.albedo0.rgb, ptSaturattion0.a);
            samples.albedo1.rgb = lerp(MSLuminance(samples.albedo1.rgb), samples.albedo1.rgb, ptSaturattion1.a);
            #if !_MAX2LAYER
               samples.albedo2.rgb = lerp(MSLuminance(samples.albedo2.rgb), samples.albedo2.rgb, ptSaturattion2.a);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = lerp(MSLuminance(samples.albedo3.rgb), samples.albedo3.rgb, ptSaturattion3.a);
            #endif
         
         #endif
         
         #if _PERTEXTINT && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif
         
         #if _PCHEIGHTGRADIENT || _PCHEIGHTHSV || _PCSLOPEGRADIENT || _PCSLOPEHSV
            ProceduralGradients(i, samples, config, worldHeight, worldNormalVertex);
         #endif

         
         

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXCOLORINTENSITY
            SAMPLE_PER_TEX(ptCI, 23.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = saturate(samples.albedo0.rgb * (1 + ptCI0.rrr));
            samples.albedo1.rgb = saturate(samples.albedo1.rgb * (1 + ptCI1.rrr));
            #if !_MAX2LAYER
               samples.albedo2.rgb = saturate(samples.albedo2.rgb * (1 + ptCI2.rrr));
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = saturate(samples.albedo3.rgb * (1 + ptCI3.rrr));
            #endif
         #endif

         #if (_PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = saturate(((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5);
               samples.albedo1.rgb = saturate(((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5);
               #if !_MAX2LAYER
                 samples.albedo2.rgb = saturate(((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5);
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb = saturate(samples.albedo0.rgb + ptBC0.rrr);
               samples.albedo1.rgb = saturate(samples.albedo1.rgb + ptBC1.rrr);
               #if !_MAX2LAYER
                  samples.albedo2.rgb = saturate(samples.albedo2.rgb + ptBC2.rrr);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(samples.albedo3.rgb + ptBC3.rrr);
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if (_PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR && !_DISABLESPLATMAPS
            #if _SURFACENORMALS
               samples.surf0 *= perTexMatSettings0.r;
               samples.surf1 *= perTexMatSettings1.r;
               samples.surf2 *= perTexMatSettings2.r;
               samples.surf3 *= perTexMatSettings3.r;
            #else
               samples.normSAO0.xy *= perTexMatSettings0.r;
               samples.normSAO1.xy *= perTexMatSettings1.r;
               samples.normSAO2.xy *= perTexMatSettings2.r;
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR && !_DISABLESPLATMAPS
            samples.normSAO0.a = pow(abs(samples.normSAO0.a), perTexMatSettings0.b);
            samples.normSAO1.a = pow(abs(samples.normSAO1.a), perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(abs(samples.normSAO2.a), perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(abs(samples.normSAO3.a), perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR && !_DISABLESPLATMAPS
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         
         #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP) 
          #if _PERTEXSSS
          {
            SAMPLE_PER_TEX(ptSSS, 18.5, config, half4(1, 1, 1, 1)); // tint, thickness
            half4 vals = ptSSS0 * heightWeights.x + ptSSS1 * heightWeights.y + ptSSS2 * heightWeights.z + ptSSS3 * heightWeights.w;
            SSSThickness = vals.a;
            SSSTint = vals.rgb;
          }
          #endif
         #endif

         #if _PERTEXRIMLIGHT
         {
            SAMPLE_PER_TEX(ptRimA, 26.5, config, half4(1, 1, 1, 1));
            SAMPLE_PER_TEX(ptRimB, 27.5, config, half4(1, 1, 1, 0));
            samples.emisMetal0.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO0.xy, 1))), max(0.0001, ptRimA0.g)) * ptRimB0.rgb * ptRimB0.a;
            samples.emisMetal1.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO1.xy, 1))), max(0.0001, ptRimA1.g)) * ptRimB1.rgb * ptRimB1.a;
            samples.emisMetal2.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO2.xy, 1))), max(0.0001, ptRimA2.g)) * ptRimB2.rgb * ptRimB2.a;
            samples.emisMetal3.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO3.xy, 1))), max(0.0001, ptRimA3.g)) * ptRimB3.rgb * ptRimB3.a;
         }
         #endif



         #if (((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)) && !_DISABLESPLATMAPS
            ApplyDetailDistanceNoisePerTex(samples, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         
         #if _GLOBALNOISEUV
            // noise defaults so that a value of 1, 1 is 4 pixels in size and moves the uvs by 1 pixel max.
            #if _CUSTOMSPLATTEXTURES
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #else
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif
         #endif

         
         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE
            ApplyTrax(samples, config, i.worldPos, traxBuffer, traxNormal);
         #endif

         #if (_ANTITILEARRAYDETAIL || _ANTITILEARRAYDISTANCE || _ANTITILEARRAYNORMAL) && !_DISABLESPLATMAPS
            ApplyAntiTilePerTex(samples, config, camDist, i.worldPos, worldNormalVertex, heightWeights);
         #endif

         #if _GEOMAP && !_DISABLESPLATMAPS
         GeoTexturePerTex(samples, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif
         
         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH && !_DISABLESPLATMAPS
         GlobalTintTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH && !_DISABLESPLATMAPS
         GlobalNormalTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && _PERTEXGLOBALSAOMSTRENGTH && !_DISABLESPLATMAPS
         GlobalSAOMTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALEMIS && _PERTEXGLOBALEMISSTRENGTH && !_DISABLESPLATMAPS
         GlobalEmisTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && _PERTEXGLOBALSPECULARSTRENGTH && !_DISABLESPLATMAPS && _USESPECULARWORKFLOW
         GlobalSpecularTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _PERTEXMETALLIC && !_DISABLESPLATMAPS
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         #if _GLITTER && !_DISABLESPLATMAPS
            DoGlitter(i, samples, config, camDist, worldNormalVertex, i.worldPos);
         #endif
         
         // Blend em..
         #if _DISABLESPLATMAPS
            // If we don't sample from the _Diffuse, then the shader compiler will strip the sampler on
            // some platforms, which will cause everything to break. So we sample from the lowest mip
            // and saturate to 1 to keep the cost minimal. Annoying, but the compiler removes the texture
            // and sampler, even though the sampler is still used.
            albedo = saturate(UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, float3(0,0,0), 12) + 1);
            albedo.a = 0.5; // make height something we can blend with for the combined mesh mode, since it still height blends.
            normSAO = half4(0,0,0,1);
         #else
            albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
            normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);

            #if _SURFACENORMALS
               surfGrad = BlendWeights(samples.surf0, samples.surf1, samples.surf2, samples.surf3, heightWeights);
            #endif

            #if (_USEEMISSIVEMETAL || _PERTEXRIMLIGHT) && !_DISABLESPLATMAPS
               emisMetal = BlendWeights(samples.emisMetal0, samples.emisMetal1, samples.emisMetal2, samples.emisMetal3, heightWeights);
            #endif

            #if _USESPECULARWORKFLOW && !_DISABLESPLATMAPS
               specular = BlendWeights(samples.specular0, samples.specular1, samples.specular2, samples.specular3, heightWeights);
            #endif

            #if _PERTEXOUTLINECOLOR
               SAMPLE_PER_TEX(ptOutlineColor, 28.5, config, half4(0.5, 0.5, 0.5, 1));
               half4 outlineColor = BlendWeights(ptOutlineColor0, ptOutlineColor1, ptOutlineColor2, ptOutlineColor3, heightWeights);
               half4 tstr = saturate(abs(heightWeights - 0.5) * 2);
               half transitionBlend = min(min(min(tstr.x, tstr.y), tstr.z), tstr.w);
               albedo.rgb = lerp(albedo.rgb * outlineColor.rgb * 2, albedo.rgb, outlineColor.a * transitionBlend);
            #endif
         #endif



         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
            o.Alpha = 1.0;
            if (config.uv0.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.x;
            else if (config.uv1.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.y;
            else if (config.uv2.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.z;
            else if (config.uv3.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.w;
         #endif



         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
            ApplyDetailDistanceNoise(albedo.rgb, normSAO, surfGrad, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         #if _SPLATFADE
         }
         #endif

         #if _SPLATFADE
            
            float2 sfDX = ddx(config.uv * _UVScale);
            float2 sfDY = ddy(config.uv * _UVScale);

            MSBRANCHOTHER(camDist - _SplatFade.x)
            {
               float falloff = saturate(InverseLerp(_SplatFade.x, _SplatFade.y, camDist));
               half4 sfalb = SAMPLE_TEXTURE2D_ARRAY_GRAD(_Diffuse, sampler_Diffuse, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY);
               COUNTSAMPLE
               albedo.rgb = lerp(albedo.rgb, sfalb.rgb, falloff);

               #if !_NONORMALMAP && !_AUTONORMAL
                  half4 sfnormSAO = SAMPLE_TEXTURE2D_ARRAY_GRAD(_NormalSAO, sampler_NormalSAO, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY).agrb;
                  COUNTSAMPLE
                  sfnormSAO.xy = sfnormSAO.xy * 2 - 1;

                  normSAO = lerp(normSAO, sfnormSAO, falloff);
  
                  #if _SURFACENORMALS
                     surfGrad = lerp(surfGrad, ConvertNormal2ToGradient(sfnormSAO.xy), falloff);
                  #endif
               #endif
              
            }
         #endif

         #if _AUTONORMAL
            float3 autoNormal = HeightToNormal(albedo.a * _AutoNormalHeightScale, i.worldPos);
            normSAO.xy = autoNormal;
            normSAO.z = 0;
            normSAO.w = (autoNormal.z * autoNormal.z);
         #endif
 


         #if _MESHCOMBINED
            SampleMeshCombined(albedo, normSAO, surfGrad, emisMetal, specular, o.Alpha, SSSThickness, SSSTint, config, heightWeights);
         #endif

         #if _ISOBJECTSHADER
            SampleObjectShader(i, albedo, normSAO, surfGrad, emisMetal, specular, config);
         #endif

         #if _GEOMAP
            GeoTexture(albedo.rgb, normSAO, surfGrad, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif

         
         #if _SCATTER
            ApplyScatter(
               #if _MEGASPLAT
               config, 
               #endif
               i, albedo, normSAO, surfGrad, config.uv, camDist);
         #endif

         #if _DECAL
            DoDecalBlend(decalOutput, albedo, normSAO, surfGrad, emisMetal, i.uv_Control0);
         #endif
         

         #if _GLOBALTINT && !_PERTEXGLOBALTINTSTRENGTH
            GlobalTintTexture(albedo.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _VSGRASSMAP
            VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMALSTRENGTH
            GlobalNormalTexture(normSAO, surfGrad, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && !_PERTEXGLOBALSAOMSTRENGTH
            GlobalSAOMTexture(normSAO, emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALEMIS && !_PERTEXGLOBALEMISSTRENGTH
            GlobalEmisTexture(emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && !_PERTEXGLOBALSPECULARSTRENGTH && _USESPECULARWORKFLOW
            GlobalSpecularTexture(specular.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         
         
         o.Albedo = albedo.rgb;
         o.Height = albedo.a;

         #if _NONORMALMAP
            o.Normal = half3(0,0,1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #elif _SURFACENORMALS
            o.Normal = ResolveNormalFromSurfaceGradient(surfGrad);
            o.Normal = mul(GetTBN(i), o.Normal);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #else
            o.Normal = half3(normSAO.xy, 1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;       
         #endif


         

         #if _USEEMISSIVEMETAL || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _PERTEXRIMLIGHT
           #if _USEEMISSIVEMETAL
	           emisMetal.rgb *= _EmissiveMult;
	        #endif
           
           o.Emission += emisMetal.rgb;
           o.Metallic = emisMetal.a;
	        
         #endif

         #if _USESPECULARWORKFLOW
            o.Specular = specular;
         #endif

         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(i, o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         
         #if _SNOW
         snowCover = DoSnow(i, o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, 
            config, weights, SSSTint, SSSThickness, traxBuffer, traxNormal);
         #endif

         #if _PERTEXSSS || _MESHCOMBINEDUSESSS || (_SNOW && _SNOWSSS)
         {
            half3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

            o.Emission += ComputeSSS(i, worldView, WorldNormalVector(i, o.Normal),
               SSSTint, SSSThickness, _SSSDistance, _SSSScale, _SSSPower);
         }
         #endif
         
         #if _SNOWGLITTER
            DoSnowGlitter(i, config, o, camDist, worldNormalVertex, snowCover);
         #endif

         #if _WINDPARTICULATE || _SNOWPARTICULATE
            DoWindParticulate(i, o, config, weights, camDist, worldNormalVertex, snowCover);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         #if _SPECULARFADE
         {
            float specFade = saturate((i.worldPos.y - _SpecularFades.x) / max(_SpecularFades.y - _SpecularFades.x, 0.0001));
            o.Metallic *= specFade;
            o.Smoothness *= specFade;
         }
         #endif

         #if _VSSHADOWMAP
         VSShadowTexture(o, i, config, camDist);
         #endif
         
         #if _TOONWIREFRAME
         ToonWireframe(config.uv, o.Albedo, camDist);
         #endif

        #if _SEETHROUGHSHADER
            SeethroughShader(o.Albedo, o.Emission, o.Alpha, i.worldPos, o.Normal, i.worldNormal);                   
        #endif


         #if _DEBUG_TRAXBUFFER
            ClearAllButAlbedo(o, half3(traxBuffer, 0, 0) * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMALVERTEX
            ClearAllButAlbedo(o, worldNormalVertex * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMAL
            ClearAllButAlbedo(o,  WorldNormalVector(i, o.Normal) * saturate(o.Albedo.z+1));
         #endif

         #if _DEBUG_MEGABARY && _MEGASPLAT
            o.Albedo = i.baryWeights.xyz;
         #endif


         return o;
      }
      
      void SampleSplats(float2 controlUV, inout half4 w0, inout half4 w1, inout half4 w2, inout half4 w3, inout half4 w4, inout half4 w5, inout half4 w6, inout half4 w7)
      {
         #if _CUSTOMSPLATTEXTURES
            #if !_MICROMESH
            controlUV = (controlUV * (_CustomControl0_TexelSize.zw - 1.0f) + 0.5f) * _CustomControl0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_CustomControl0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_CustomControl1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_CustomControl2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_CustomControl3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_CustomControl4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_CustomControl5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_CustomControl6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_CustomControl7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #else
            #if !_MICROMESH
            controlUV = (controlUV * (_Control0_TexelSize.zw - 1.0f) + 0.5f) * _Control0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_Control0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_Control1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_Control2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_Control3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_Control4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_Control5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_Control6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_Control7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #endif
      }   


      

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
         #if _MEGANOUV
            i.uv_Control0 = i.worldPos.xz;
         #endif
         
         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);
          
         #if _FORCELOCALSPACE
            worldNormalVertex = mul((float3x3)GetWorldToObjectMatrix(), worldNormalVertex).xyz;
            i.worldPos = i.worldPos -  mul(GetObjectToWorldMatrix(), float4(0,0,0,1)).xyz;
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ORIGINSHIFT
             i.worldPos = i.worldPos + mul(_GlobalOriginMTX, float4(0,0,0,1)).xyz;
             i.worldHeight = i.worldPos.y;
         #endif

         #if _DEBUG_USE_TOPOLOGY
            i.worldPos = SAMPLE_TEXTURE2D(_DebugWorldPos, sampler_Diffuse, i.uv_Control0);
            worldNormalVertex = SAMPLE_TEXTURE2D(_DebugWorldNormal, sampler_Diffuse, i.uv_Control0);
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ALPHABELOWHEIGHT && !_TBDISABLEALPHAHOLES
            ClipWaterLevel(i.worldPos);
         #endif

         #if !_TBDISABLEALPHAHOLES && defined(_ALPHATEST_ON)
            // UNITY 2019.3 holes
            ClipHoles(i.uv_Control0);
         #endif


         float2 origUV = i.uv_Control0;

         #if _MICROMESH && _MESHUV2
         float2 controlUV = i.uv2_Diffuse;
         #else
         float2 controlUV = i.uv_Control0;
         #endif


         #if _MICROMESH
            controlUV = InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, controlUV);
         #endif

         half4 weights = half4(1,0,0,0);

         Config config = (Config)0;
         UNITY_INITIALIZE_OUTPUT(Config,config);
         config.uv = origUV;

         DecalOutput decalOutput = (DecalOutput)0;
         #if _DECAL
            decalOutput = DoDecals(i.uv_Control0, i.worldPos, camDist, worldNormalVertex);
         #endif

         #if _SURFACENORMALS
         // Initialize the surface gradient basis vectors
         ConstructSurfaceGradientTBN(i);
         #endif
        


         #if _SPLATFADE
         MSBRANCHOTHER(_SplatFade.y - camDist)
         #endif // _SPLATFADE
         {
            #if !_DISABLESPLATMAPS

               // Sample the splat data, from textures or vertices, and setup the config..
               #if _MICRODIGGERMESH
                  DiggerSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLAT
                  MegaSplatVertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLATTEXTURE
                   MegaSplatTextureSetup(controlUV, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MICROVERTEXMESH
                  VertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif !_PROCEDURALTEXTURE || _PROCEDURALBLENDSPLATS
                  half4 w0 = 0; half4 w1 = 0; half4 w2 = 0; half4 w3 = 0; half4 w4 = 0; half4 w5 = 0; half4 w6 = 0; half4 w7 = 0;
                  SampleSplats(controlUV, w0, w1, w2, w3, w4, w5, w6, w7);
                  Setup(weights, origUV, config, w0, w1, w2, w3, w4, w5, w6, w7, i.worldPos, decalOutput);
               #endif

               #if _PROCEDURALTEXTURE
                  float3 procNormal = worldNormalVertex;
                  float3 worldPos = i.worldPos;
                  ProceduralSetup(i, worldPos, i.worldHeight, procNormal, i.worldUpVector, weights, origUV, config, ddx(origUV), ddy(origUV), ddx(worldPos), ddy(worldPos), decalOutput);
               #endif
            #else // _DISABLESPLATMAPS
                Setup(weights, origUV, config, half4(1,0,0,0), 0, 0, 0, 0, 0, 0, 0, i.worldPos, decalOutput);
            #endif

            #if _SLOPETEXTURE
               SlopeTexture(config, weights, worldNormalVertex);
            #endif
         } // _SPLATFADE else case


         #if _TOONFLATTEXTURE
            float2 quv = floor(origUV * _ToonTerrainSize);
            float2 fuv = frac(origUV * _ToonTerrainSize);
            #if !_TOONFLATTEXTUREQUAD
               quv = Hash2D((fuv.x > fuv.y) ? quv : quv * 0.333);
            #endif
            float2 uvq = quv / _ToonTerrainSize;
            config.uv0.xy = uvq;
            config.uv1.xy = uvq;
            config.uv2.xy = uvq;
            config.uv3.xy = uvq;
         #endif
         
         #if (_TEXTURECLUSTER2 || _TEXTURECLUSTER3) && !_DISABLESPLATMAPS
            PrepClusters(origUV, config, i.worldPos, worldNormalVertex);
         #endif

         #if (_ALPHAHOLE || _ALPHAHOLETEXTURE) && !_DISABLESPLATMAPS && !_TBDISABLEALPHAHOLES
         ClipAlphaHole(config, weights);
         #endif


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex, decalOutput);

         // On windows, sometimes the shared samplers gets stripped, so we have to do this crap.
         // We sample from the lowest mip, so it shouldn't cost much, but still, I hate this, wtf..
         float stripVal = saturate(SAMPLE_TEXTURE2D_LOD(_Diffuse, sampler_Diffuse, config.uv0, 11).r + 2);
         stripVal *= saturate(SAMPLE_TEXTURE2D_LOD(_NormalSAO, sampler_NormalSAO, config.uv0, 11).r + 2);
         l.Albedo *= stripVal;
         l.Normal *= stripVal;

         #if _PROCEDURALTEXTURE
            ProceduralTextureDebugOutput(l, weights, config);
         #endif


         return l;

      }



   





float4 ConstructTerrainTangent(float3 normal, float3 positiveZ)
{
    // Consider a flat terrain. It should have tangent be (1, 0, 0) and bitangent be (0, 0, 1) as the UV of the terrain grid mesh is a scale of the world XZ position.
    // In CreateTangentToWorld function (in SpaceTransform.hlsl), it is cross(normal, tangent) * sgn for the bitangent vector.
    // It is not true in a left-handed coordinate system for the terrain bitangent, if we provide 1 as the tangent.w. It would produce (0, 0, -1) instead of (0, 0, 1).
    // Also terrain's tangent calculation was wrong in a left handed system because cross((0,0,1), terrainNormalOS) points to the wrong direction as negative X.
    // Therefore all the 4 xyzw components of the tangent needs to be flipped to correct the tangent frame.
    // (See TerrainLitData.hlsl - GetSurfaceAndBuiltinData)
    float3 tangent = cross(normal, positiveZ);
    return float4(tangent, -1);
}



void TerrainInstancing(inout float4 vertex, inout float3 normal, inout float2 uv)
{
#if _MICROTERRAIN && defined(UNITY_INSTANCING_ENABLED) && !_TERRAINBLENDABLESHADER
   
    float2 patchVertex = vertex.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;

    float2 sampleUV = (uv / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

    float height = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, shared_linear_clamp_sampler, sampleUV, 0));
   
    vertex.xz = sampleCoords * _TerrainHeightmapScale.xz;
    vertex.y = height * _TerrainHeightmapScale.y;

    
    normal = float3(0, 1, 0);

#endif
}


void ApplyMeshModification(inout VertexData input)
{
   #if _MICROTERRAIN && !_TERRAINBLENDABLESHADER
      float2 uv = input.texcoord0.xy;
      TerrainInstancing(input.vertex, input.normal, uv);
      input.texcoord0.xy = uv;
   #endif
   #if _PERPIXNORMAL && !_TERRAINBLENDABLESHADER
      input.normal = float3(0,1,0);
   #endif

}

// called by the template, so we can remove tangent from VertexData
void ApplyTerrainTangent(inout VertexToPixel input)
{
   #if (_MICROTERRAIN || _PERPIXNORMAL) && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif

   // digger meshes ain't got no tangent either..
   #if _MICRODIGGERMESH && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif
}


void ModifyVertex(inout VertexData v, inout ExtraV2F d)
{
   ApplyMeshModification(v);

   #if _MICROVERTEXMESH || _MICRODIGGERMESH
      EncodeVertexWorkflow(v, d);
   #elif _MEGASPLAT
      EncodeMegaSplatVertex(v, d);
   #endif

}


void ModifyTessellatedVertex(inout VertexData v, inout ExtraV2F d)
{
   #if _TESSDISTANCE
      v.vertex.xyz += OffsetVertex(v, d);
   #endif
}

float3 GetTessFactors ()
{
   #if _TESSDISTANCE
      return float3(_TessData2.x, _TessData2.y, _TessData1.x);
   #endif
   return 0;
}


        


    
    void SurfaceFunction(inout Surface o, inout ShaderData d)
    {
       
        float3 worldNormalVertex = d.worldSpaceNormal;

        #if (defined(UNITY_INSTANCING_ENABLED) && _MICROTERRAIN && !_TERRAINBLENDABLESHADER)
            float2 sampleCoords = (d.texcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            float3 geomBitangent = normalize(cross(geomNormal, geomTangent)) * -1;
            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

         #elif _PERPIXNORMAL &&  (_MICROTERRAIN || _MICROMESHTERRAIN) && !_TERRAINBLENDABLESHADER
            float2 sampleCoords = (d.texcoord0.xy * _PerPixelNormal_TexelSize.zw + 0.5f) * _PerPixelNormal_TexelSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_PerPixelNormal, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            
            float3 geomBitangent = normalize(cross(geomTangent, geomNormal)) * -1;
            #if _MICROMESHTERRAIN
                geomBitangent *= -1;
            #endif

            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

        #endif

        #if _TOONPOLYEDGE
           FlatShade(d);
        #endif

         Input i = DescToInput(d);

         
         
         #if _SRPTERRAINBLEND
            MicroSplatLayer l = BlendWithTerrain(d);

               #if _DEBUG_WORLDNORMAL
                  ClearAllButAlbedo(l, normalize(TangentToWorldSpace(d, l.Normal)) * saturate(l.Albedo.z+1));
               #endif
         #else
            MicroSplatLayer l = SurfImpl(i, worldNormalVertex);
         #endif

        DoDebugOutput(l);




        o.Albedo = l.Albedo;
        o.Normal = l.Normal;
        o.Smoothness = l.Smoothness;
        o.Occlusion = l.Occlusion;
        o.Metallic = l.Metallic;
        o.Emission = l.Emission;
        #if _USESPECULARWORKFLOW
        o.Specular = l.Specular;
        #endif
        o.Height = l.Height;
        o.Alpha = l.Alpha;


    }



        



            // SHADERDESC

         ShaderData CreateShaderData(VertexToPixel i)
         {
            ShaderData d = (ShaderData)0;
            d.worldSpacePosition = i.worldPos;

            d.worldSpaceNormal = i.worldNormal;
            d.worldSpaceTangent = i.worldTangent.xyz;
            float3 bitangent = cross(i.worldTangent.xyz, i.worldNormal) * i.worldTangent.w * -1;
            

            d.TBNMatrix = float3x3(d.worldSpaceTangent, bitangent, d.worldSpaceNormal);
            d.worldSpaceViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);
             d.texcoord0 = i.texcoord0;
            #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                d.texcoord1 = i.texcoord1;
               // d.texcoord2 = i.texcoord2;
            #endif
            // d.texcoord3 = i.texcoord3;
            // d.vertexColor = i.vertexColor;

            // these rarely get used, so we back transform them. Usually will be stripped.
            #if _HDRP
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(GetCameraRelativePositionWS(i.worldPos), 1));
            #else
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(i.worldPos, 1));
            #endif
            // d.localSpaceNormal = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldNormal));
            // d.localSpaceTangent = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldTangent.xyz));

            // d.screenPos = i.screenPos;
            // d.screenUV = i.screenPos.xy / i.screenPos.w;

            // d.extraV2F0 = i.extraV2F0;
            // d.extraV2F1 = i.extraV2F1;
            // d.extraV2F2 = i.extraV2F2;
            // d.extraV2F3 = i.extraV2F3;
            // d.extraV2F4 = i.extraV2F4;
            // d.extraV2F5 = i.extraV2F5;
            // d.extraV2F6 = i.extraV2F6;
            // d.extraV2F7 = i.extraV2F7;

            return d;
         }
         // CHAINS

            void ChainModifyVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               ModifyVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }

            void ChainModifyTessellatedVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               // d.extraV2F0 = v2p.extraV2F0;
               // d.extraV2F1 = v2p.extraV2F1;
               // d.extraV2F2 = v2p.extraV2F2;
               // d.extraV2F3 = v2p.extraV2F3;
               // d.extraV2F4 = v2p.extraV2F4;
               // d.extraV2F5 = v2p.extraV2F5;
               // d.extraV2F6 = v2p.extraV2F6;
               // d.extraV2F7 = v2p.extraV2F7;

               ModifyTessellatedVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }


            void ChainFinalColorForward(inout Surface l, inout ShaderData d, inout half4 color)
            {
               
            }

            void ChainFinalGBufferStandard(inout Surface s, inout ShaderData d, inout half4 GBuffer0, inout half4 GBuffer1, inout half4 GBuffer2, inout half4 outEmission, inout half4 outShadowMask)
            {
               
            }


            
         #if _PASSSHADOW
            float3 _LightDirection;
            float3 _LightPosition;
         #endif

         // vertex shader
         VertexToPixel Vert (VertexData v)
         {
           
           VertexToPixel o = (VertexToPixel)0;

           UNITY_SETUP_INSTANCE_ID(v);
           UNITY_TRANSFER_INSTANCE_ID(v, o);
           UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


#if !_TESSELLATION_ON
           ChainModifyVertex(v, o);
#endif

            o.texcoord0 = v.texcoord0;

           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               o.texcoord1 = v.texcoord1;
              // o.texcoord2 = v.texcoord2;
           #endif

           // o.texcoord3 = v.texcoord3;
           // o.vertexColor = v.vertexColor;
           
           VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
           o.worldPos = TransformObjectToWorld(v.vertex.xyz);
           o.worldNormal = TransformObjectToWorldNormal(v.normal);

           
           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float2 uv1 = v.texcoord1.xy;
               float2 uv2 = v.texcoord2.xy;
               o.worldTangent = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);
           #else
               float2 uv1 = v.texcoord0.xy;
               float2 uv2 = uv1;
           #endif

           // MS Only
           ApplyTerrainTangent(o);
           

          #if _PASSSHADOW
              #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                 float3 lightDirectionWS = normalize(_LightPosition - o.worldPos);
              #else
                 float3 lightDirectionWS = _LightDirection;
              #endif
              // Define shadow pass specific clip position for Universal
              o.pos = TransformWorldToHClip(ApplyShadowBias(o.worldPos, o.worldNormal, lightDirectionWS));
              #if UNITY_REVERSED_Z
                  o.pos.z = min(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #else
                  o.pos.z = max(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #endif
          #elif _PASSMETA
              #if _MICROTERRAIN
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), v.texcoord0.xy, v.texcoord0.xy, unity_LightmapST, unity_DynamicLightmapST);
              #else
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), uv1, uv2, unity_LightmapST, unity_DynamicLightmapST);
              #endif
          #else
              o.pos = TransformWorldToHClip(o.worldPos);
          #endif



          // o.screenPos = ComputeScreenPos(o.pos, _ProjectionParams.x);
          

          #if _PASSFORWARD || _PASSGBUFFER
              #if _MICROTERRAIN
                 OUTPUT_LIGHTMAP_UV(v.texcoord0.xy, unity_LightmapST, o.lightmapUV);
              #else
                 OUTPUT_LIGHTMAP_UV(uv1, unity_LightmapST, o.lightmapUV);
              #endif
              OUTPUT_SH(o.worldNormal, o.sh);
              #if defined(DYNAMICLIGHTMAP_ON)
                   o.dynamicLightmapUV.xy = uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
              #endif
          #endif

          #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
              half fogFactor = 0;
              #if defined(_FOG_FRAGMENT)
                fogFactor = ComputeFogFactor(o.pos.z);
              #endif
              #if _BAKEDLIT
                  half3 vertexLight = VertexLighting(o.worldPos, o.worldNormal);
                  o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
              #else
                  o.fogFactorAndVertexLight = half4(fogFactor, 0,0,0);
              #endif
          #endif

          #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             o.shadowCoord = GetShadowCoord(vertexInput);
          #endif

           return o;
         }


            

            // fragment shader
            half4 Frag (VertexToPixel IN
            #ifdef _DEPTHOFFSET_ON
              , out float outputDepth : SV_Depth
            #endif
            ) : SV_Target
            {
               UNITY_SETUP_INSTANCE_ID(IN);

               ShaderData d = CreateShaderData(IN);
               Surface l = (Surface)0;

               #ifdef _DEPTHOFFSET_ON
                  l.outputDepth = outputDepth;
               #endif

               l.Albedo = half3(0.5, 0.5, 0.5);
               l.Normal = float3(0,0,1);
               l.Occlusion = 1;
               l.Alpha = 1;

               SurfaceFunction(l, d);

               #ifdef _DEPTHOFFSET_ON
                  outputDepth = l.outputDepth;
               #endif

             return 0;

            }

         ENDHLSL

      }


      
        Pass
        {
            Name "DepthOnly"
            Tags 
            { 
                "LightMode" = "DepthOnly"
            }
           
            // Render State
            Blend One Zero, One Zero
            Cull Back
            ZTest LEqual
            ZWrite On
            ColorMask 0
            
            

            HLSLPROGRAM

               #pragma vertex Vert
   #pragma fragment Frag


            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
            #define _PASSDEPTH 1
            #define _PASSDEPTHNORMALS 1

            #pragma target 3.5
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_local _ _ALPHATEST_ON

            
      #define _MICROSPLAT 1
      #define _MICROTERRAIN 1
      #define _USEGRADMIP 1
      #define _MAX4TEXTURES 1
      #define _BRANCHSAMPLES 1
      #define _BRANCHSAMPLESAGR 1
      #define _MSRENDERLOOP_UNITYURP2022 1

#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


   #define _URP 1


            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"


                  #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, float3x3(d.worldSpaceTangent, cross(d.worldSpaceTangent, d.worldSpaceNormal), d.worldSpaceNormal))

      
      #define UnityObjectToWorldNormal(normal) mul(GetObjectToWorldMatrix(), normal)

      #define _WorldSpaceLightPos0 _MainLightPosition
      
      #define UNITY_DECLARE_TEX2D(name) TEXTURE2D(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2D_NOSAMPLER(name) TEXTURE2D(name);
      #define UNITY_DECLARE_TEX2DARRAY(name) TEXTURE2D_ARRAY(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(name) TEXTURE2D_ARRAY(name);

      #define UNITY_SAMPLE_TEX2DARRAY(tex,coord)            SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, coord.xy, coord.z)
      #define UNITY_SAMPLE_TEX2DARRAY_LOD(tex,coord,lod)    SAMPLE_TEXTURE2D_ARRAY_LOD(tex, sampler##tex, coord.xy, coord.z, lod)
      #define UNITY_SAMPLE_TEX2D(tex, coord)                SAMPLE_TEXTURE2D(tex, sampler##tex, coord)
      #define UNITY_SAMPLE_TEX2D_SAMPLER(tex, samp, coord)  SAMPLE_TEXTURE2D(tex, sampler##samp, coord)

      #if defined(UNITY_COMPILER_HLSL)
         #define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
      #else
         #define UNITY_INITIALIZE_OUTPUT(type,name)
      #endif

      #define sampler2D_float sampler2D
      #define sampler2D_half sampler2D

      

      // data across stages, stripped like the above.
      struct VertexToPixel
      {
         float4 pos : SV_POSITION;
         float3 worldPos : TEXCOORD0;
         float3 worldNormal : TEXCOORD1;
         float4 worldTangent : TEXCOORD2;
          float4 texcoord0 : TEXCCOORD3;
         #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
             float4 texcoord1 : TEXCCOORD4;
            // float4 texcoord2 : TEXCCOORD5;
         #endif
         // float4 texcoord3 : TEXCCOORD6;
         // float4 screenPos : TEXCOORD7;
         // float4 vertexColor : COLOR;

         // float4 extraV2F0 : TEXCOORD13;
         // float4 extraV2F1 : TEXCOORD14;
         // float4 extraV2F2 : TEXCOORD15;
         // float4 extraV2F3 : TEXCOORD16;
         // float4 extraV2F4 : TEXCOORD17;
         // float4 extraV2F5 : TEXCOORD18;
         // float4 extraV2F6 : TEXCOORD19;
         // float4 extraV2F7 : TEXCOORD20;

         
         #if defined(LIGHTMAP_ON)
            float2 lightmapUV : TEXCOORD8;
         #endif
         #if defined(DYNAMICLIGHTMAP_ON)
            float2 dynamicLightmapUV : TEXCOORD9;
         #endif
         #if !defined(LIGHTMAP_ON)
            float3 sh : TEXCOORD10;
         #endif

         float4 fogFactorAndVertexLight : TEXCOORD11;
         float4 shadowCoord : TEXCOORD12;

         #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
         #endif
         #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
         #endif
         #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
         #endif
         #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
         #endif
      };

         
            // TEMPLATE_SHARED
            
            // data describing the user output of a pixel
            struct Surface
            {
               half3 Albedo;
               half Height;
               half3 Normal;
               half Smoothness;
               half3 Emission;
               half Metallic;
               half3 Specular;
               half Occlusion;
               half Alpha;
               // HDRP Only
               half SpecularOcclusion;
               half SubsurfaceMask;
               half Thickness;
               half CoatMask;
               half Anisotropy;
               half IridescenceMask;
               half IridescenceThickness;
            };

            // data the user might need, this will grow to be big. But easy to strip
            struct ShaderData
            {
               float3 localSpacePosition;
               float3 localSpaceNormal;
               float3 localSpaceTangent;
        
               float3 worldSpacePosition;
               float3 worldSpaceNormal;
               float3 worldSpaceTangent;

               float3 worldSpaceViewDir;
               float3 tangentSpaceViewDir;

               float4 texcoord0;
               float4 texcoord1;
               float4 texcoord2;
               float4 texcoord3;

               float2 screenUV;
               float4 screenPos;

               float4 vertexColor;

               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;

               float3x3 TBNMatrix;
            };

            struct VertexData
            {
               #if SHADER_TARGET > 30 && _PLANETCOMPUTE
 //              // uint vertexID : SV_VertexID;
               #endif
               float4 vertex : POSITION;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                  float4 tangent : TANGENT;
                  float4 texcoord1 : TEXCOORD1;
                  float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD4; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity    : TEXCOORD5; // Add Precomputed Velocity (Alembic computes velocities on runtime side).
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessVertex 
            {
               float4 vertex : INTERNALTESSPOS;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float4 tangent : TANGENT;
               float4 texcoord1 : TEXCOORD1;
               float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               // float4 extraV2F0 : TEXCOORD4;
               // float4 extraV2F1 : TEXCOORD5;
               // float4 extraV2F2 : TEXCOORD6;
               // float4 extraV2F3 : TEXCOORD7;
               // float4 extraV2F4 : TEXCOORD8;
               // float4 extraV2F5 : TEXCOORD9;
               // float4 extraV2F6 : TEXCOORD10;
               // float4 extraV2F7 : TEXCOORD11;

               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD12; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity : TEXCOORD13;
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
               UNITY_VERTEX_OUTPUT_STEREO
            };

            struct ExtraV2F
            {
               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;
            };


            float3 WorldToTangentSpace(ShaderData d, float3 normal)
            {
               return mul(d.TBNMatrix, normal);
            }

            float3 TangentToWorldSpace(ShaderData d, float3 normal)
            {
               return mul(normal, d.TBNMatrix);
            }

            // in this case, make standard more like SRPs, because we can't fix
            // GetWorldToObjectMatrix() in HDRP, since it already does macro-fu there

            #if _STANDARD
               float3 TransformWorldToObject(float3 p) { return mul(GetWorldToObjectMatrix(), float4(p, 1)); };
               float3 TransformObjectToWorld(float3 p) { return mul(GetObjectToWorldMatrix(), float4(p, 1)); };
               float4 TransformWorldToObject(float4 p) { return mul(GetWorldToObjectMatrix(), p); };
               float4 TransformObjectToWorld(float4 p) { return mul(GetObjectToWorldMatrix(), p); };
               float4x4 GetWorldToObjectMatrix() { return GetWorldToObjectMatrix(); }
               float4x4 GetObjectToWorldMatrix() { return GetObjectToWorldMatrix(); }
            #endif

            float3 GetCameraWorldPosition()
            {
               #if _HDRP
                  return GetCameraRelativePositionWS(_WorldSpaceCameraPos);
               #else
                  return _WorldSpaceCameraPos;
               #endif
            }

            
             #if _HDRP

               half3 UnpackNormalmapRGorAG(half4 packednormal)
               {
                     // This do the trick
                  packednormal.x *= packednormal.w;

                  half3 normal;
                  normal.xy = packednormal.xy * 2 - 1;
                  normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                  return normal;
               }
               half3 UnpackNormal(half4 packednormal)
               {
                  #if defined(UNITY_NO_DXT5nm)
                     return packednormal.xyz * 2 - 1;
                  #else
                     return UnpackNormalmapRGorAG(packednormal);
                  #endif
               }
               #endif
               #if _HDRP || _URP

               half3 UnpackScaleNormal(half4 packednormal, half scale)
               {
                 #ifndef UNITY_NO_DXT5nm
                   // Unpack normal as DXT5nm (1, y, 1, x) or BC5 (x, y, 0, 1)
                   // Note neutral texture like "bump" is (0, 0, 1, 1) to work with both plain RGB normal and DXT5nm/BC5
                   packednormal.x *= packednormal.w;
                 #endif
                   half3 normal;
                   normal.xy = (packednormal.xy * 2 - 1) * scale;
                   normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                   return normal;
               }  

             #endif


            void GetSun(out float3 lightDir, out float3 color)
            {
               lightDir = float3(0.5, 0.5, 0);
               color = 1;
               #if _HDRP
                  if (_DirectionalLightCount > 0)
                  {
                     DirectionalLightData light = _DirectionalLightDatas[0];
                     lightDir = -light.forward.xyz;
                     color = light.color;
                  }
               #elif _STANDARD
                  lightDir = normalize(_WorldSpaceLightPos0.xyz);
                  color = _LightColor0.rgb;
               #elif _URP
                  Light light = GetMainLight();
                  lightDir = light.direction;
                  color = light.color;
               #endif
            }

     



            
            CBUFFER_START(UnityPerMaterial)

               

      #if _MESHSUBARRAY
         half4 _MeshSubArrayIndexes;
      #endif

      float4 _Diffuse_TexelSize;
      float4 _NormalSAO_TexelSize;

      #if _HYBRIDHEIGHTBLEND
         float _HybridHeightBlendDistance;
      #endif

      #if _PACKINGHQ
         float4 _SmoothAO_TexelSize;
      #endif

      #ifdef _ALPHATEST_ON
      float4 _TerrainHolesTexture_TexelSize;
      #endif

      #if _USESPECULARWORKFLOW
         float4 _Specular_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         float4 _EmissiveMetal_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         half _EmissiveMult;
      #endif

      #if _AUTONORMAL
         half _AutoNormalHeightScale;
      #endif

      float4 _UVScale; // scale and offset

      half _Contrast;
      
      

       #if _VSSHADOWMAP
         float4 gVSSunDirection;
      #endif

      #if _FORCELOCALSPACE && _PLANETVECTORS
         float4x4 _PQSToLocal;
      #endif

      #if _ORIGINSHIFT
         float4x4 _GlobalOriginMTX;
      #endif

      float4 _Control0_TexelSize;
      #if _CUSTOMSPLATTEXTURES
         float4 _CustomControl0_TexelSize;
      #endif
      float4 _PerPixelNormal_TexelSize;

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         float2 _NoiseUVParams;
      #endif

      float4 _PerTexProps_TexelSize;

      #if _SURFACENORMALS  
         float3 surfTangent;
         float3 surfBitangent;
         float3 surfNormal;
      #endif



            CBUFFER_END

            

            

                  

      #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, data.TBN)
      

      



   
// In Unity 2020.3LTS, Unity will spew tons of errors about missing this sampler in
// URP, even though it shouldn't be required.
TEXTURE2D(_MainTex);

      // globals, outside of CBuffer, but used by more than one module
      float3 _gGlitterLightDir;
      float3 _gGlitterLightWorldPos;
      half3 _gGlitterLightColor;

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
         float4    _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
         float4    _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
         float4    _TerrainNormalmapTexture_TexelSize;
      #endif

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
          TEXTURE2D(_TerrainHeightmapTexture);
          TEXTURE2D(_TerrainNormalmapTexture);
      #endif

      UNITY_INSTANCING_BUFFER_START(Terrain)
          UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
      UNITY_INSTANCING_BUFFER_END(Terrain)          


      

      // dynamic branching helpers, for regular and aggressive branching
      // debug mode shows how many samples using branching will save us. 
      //
      // These macros are always used instead of the UNITY_BRANCH macro
      // to maintain debug displays and allow branching to be disabled
      // on as granular level as we want. 
      
      #if _BRANCHSAMPLES
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++; if (w > 0)
         #else
            #define MSBRANCH(w) UNITY_BRANCH if (w > 0)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++;
         #else
            #define MSBRANCH(w) 
         #endif
      #endif
      
      #if _BRANCHSAMPLESAGR
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER ||_DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++; if (w > 0.001)
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++; if (w > 0.001)
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++; if (w > 0.001)
         #else
            #define MSBRANCHTRIPLANAR(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHCLUSTER(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHOTHER(w) UNITY_BRANCH if (w > 0.001)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++;
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++;
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++;
         #else
            #define MSBRANCHTRIPLANAR(w)
            #define MSBRANCHCLUSTER(w)
            #define MSBRANCHOTHER(w)
         #endif
      #endif

      #if _DEBUG_SAMPLECOUNT
         int _sampleCount;
         #define COUNTSAMPLE { _sampleCount++; }
      #else
         #define COUNTSAMPLE
      #endif

      #if _DEBUG_PROCLAYERS
         int _procLayerCount;
         #define COUNTPROCLAYER { _procLayerCount++; }
      #else
         #define COUNTPROCLAYER
      #endif


      #if _DEBUG_USE_TOPOLOGY
         TEXTURE2D(_DebugWorldPos);
         TEXTURE2D(_DebugWorldNormal);
      #endif
      

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         TEXTURE2D(_NoiseUV);
      #endif

      #if _PACKINGHQ
         UNITY_DECLARE_TEX2DARRAY(_SmoothAO);
      #endif

      #if _USESPECULARWORKFLOW
         UNITY_DECLARE_TEX2DARRAY(_Specular);
      #endif

      #if _USEEMISSIVEMETAL
         UNITY_DECLARE_TEX2DARRAY(_EmissiveMetal);
      #endif

      TEXTURE2D(_PerPixelNormal);

      SamplerState shared_linear_clamp_sampler;
      SamplerState shared_point_clamp_sampler;
      
      TEXTURE2D(_Control0);
      #if _CUSTOMSPLATTEXTURES
         TEXTURE2D(_CustomControl0);
         #if !_MAX4TEXTURES
         TEXTURE2D(_CustomControl1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_CustomControl2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_CustomControl3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_CustomControl7);
         #endif
      #else
         #if !_MAX4TEXTURES
         TEXTURE2D(_Control1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_Control2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_Control3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_Control7);
         #endif
      #endif

      TEXTURE2D_FLOAT(_PerTexProps);
   
      struct DecalLayer
      {
         float3 uv;
         float2 dx;
         float2 dy;
         int decalIndex;
         bool dynamic; 
      };

      struct DecalOutput
      {
         DecalLayer l0;
         DecalLayer l1;
         DecalLayer l2;
         DecalLayer l3;
         
         half4 Weights;
         half4 Indexes;
         half4 fxLevels;
         
      };
      

      struct TriGradMipFormat
      {
         float4 d0;
         float4 d1;
         float4 d2;
      };

      float InverseLerp(float x, float y, float v) { return (v-x)/max(y-x, 0.001); }
      float2 InverseLerp(float2 x, float2 y, float2 v) { return (v-x)/max(y-x, float2(0.001, 0.001)); }
      float3 InverseLerp(float3 x, float3 y, float3 v) { return (v-x)/max(y-x, float3(0.001, 0.001, 0.001)); }
      float4 InverseLerp(float4 x, float4 y, float4 v) { return (v-x)/max(y-x, float4(0.001, 0.001, 0.001, 0.001)); }
      

      // 2019.3 holes
      #ifdef _ALPHATEST_ON
          TEXTURE2D(_TerrainHolesTexture);
          
          void ClipHoles(float2 uv)
          {
              float hole = SAMPLE_TEXTURE2D(_TerrainHolesTexture, shared_linear_clamp_sampler, uv).r;
              COUNTSAMPLE
              clip(hole < 0.5f ? -1 : 1);
          }
      #endif

      
      #if _TRIPLANAR
         #if _USEGRADMIP
            #define MIPFORMAT TriGradMipFormat
            #define INITMIPFORMAT (TriGradMipFormat)0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float3
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float3
         #endif
      #else
         #if _USEGRADMIP
            #define MIPFORMAT float4
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float
         #endif
      #endif

      float2 TotalOne(float2 v) { return v * (1.0 / max(v.x + v.y, 0.001)); }
      float3 TotalOne(float3 v) { return v * (1.0 / max(v.x + v.y + v.z, 0.001)); }
      float4 TotalOne(float4 v) { return v * (1.0 / max(v.x + v.y + v.z + v.w, 0.001)); }

      float2 RotateUV(float2 uv, float amt)
      {
         uv -=0.5;
         float s = sin ( amt);
         float c = cos ( amt );
         float2x2 mtx = float2x2( c, -s, s, c);
         mtx *= 0.5;
         mtx += 0.5;
         mtx = mtx * 2-1;
         uv = mul ( uv, mtx );
         uv += 0.5;
         return uv;
      }

      float4 DecodeToFloat4(float v)
      {
         uint vi = (uint)(v * (256.0f * 256.0f * 256.0f * 256.0f));
         int ex = (int)(vi / (256 * 256 * 256) % 256);
         int ey = (int)((vi / (256 * 256)) % 256);
         int ez = (int)((vi / (256)) % 256);
         int ew = (int)(vi % 256);
         float4 e = float4(ex / 255.0, ey / 255.0, ez / 255.0, ew / 255.0);
         return e;
      }

      

      struct Input 
      {
         ShaderData shaderData;
         float2 uv_Control0;
         float2 uv2_Diffuse;

         float worldHeight;
         float3 worldUpVector;

         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         float4 color;
         float3x3 TBN;

         // vertex/digger workflow data
         half4 w0;
         half4 w1;
         half4 w2;
         half4 w3;
         half4 w4;
         half4 w5;
         half4 w6;
         
         // megasplat data
         half4 layer0;
         half4 layer1;
         half3 baryWeights;
         half4 scatter0;
         half4 scatter1;

         // wetness, puddles, streams, lava from vertex or megasplat
         half4 fx;
         // snow min, snow max
         half4 fx2;


      };
      
      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
         half3 pN0;
         half3 pN1;
         half3 pN2;
         half3 pN3;
         half3 axisSign;
         Input IN;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;

         half4 cluster0;
         half4 cluster1;
         half4 cluster2;
         half4 cluster3;

      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
         #if _USESPECULARWORKFLOW
         half3 Specular;
         #endif
         half Alpha;
         
      };


      

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         #if _SURFACENORMALS
            half3 surf0;
            half3 surf1;
            half3 surf2;
            half3 surf3;
         #endif

         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
         

         #if _USEEMISSIVEMETAL || _GLOBALEMIS || _GLOBALSMOOTHAOMETAL || _PERTEXSSS || _PERTEXRIMLIGHT
            half4 emisMetal0;
            half4 emisMetal1;
            half4 emisMetal2;
            half4 emisMetal3;
         #endif

         #if _USESPECULARWORKFLOW
            half3 specular0;
            half3 specular1;
            half3 specular2;
            half3 specular3;
         #endif
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
         #if _SURFACENORMALS
            s.surf0 = half3(0,0,1);
            s.surf1 = half3(0,0,1);
            s.surf2 = half3(0,0,1);
            s.surf3 = half3(0,0,1);
         #endif
      }

       float3 GetGlobalLightDir(Input i)
      {
         float3 lightDir = float3(1,0,0);

         #if _HDRP || PASS_DEFERRED
            lightDir = normalize(_gGlitterLightDir.xyz);
         #elif _URP
            lightDir = GetMainLight().direction;
         #else
            #ifndef USING_DIRECTIONAL_LIGHT
               lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            #else
               lightDir = normalize(_WorldSpaceLightPos0.xyz);
            #endif
         #endif
         return lightDir;
      }

      float3x3 GetTBN(Input i)
      {
         return i.TBN;
      }
      
      float3 GetGlobalLightDirTS(Input i)
      {
         float3 lightDirWS = GetGlobalLightDir(i);
         return mul(GetTBN(i), lightDirWS);
      }
      
      half3 GetGlobalLightColor()
      {
         #if _HDRP || PASS_DEFERRED
            return _gGlitterLightColor;
         #elif _URP
            return (GetMainLight().color);
         #else
            return _LightColor0.rgb;
         #endif
      }

      

      half3 FuzzyShade(half3 color, half3 normal, half coreMult, half edgeMult, half power, float3 viewDir)
      {
         half dt = saturate(dot(viewDir, normal));
         half dark = 1.0 - (coreMult * dt);
         half edge = pow(1-dt, power) * edgeMult;
         return color * (dark + edge);
      }

      half3 ComputeSSS(Input i, float3 V, float3 N, half3 tint, half thickness, half distortion, half scale, half power)
      {
         float3 L = GetGlobalLightDir(i);
         half3 lightColor = GetGlobalLightColor();
         float3 H = normalize(L + N * distortion);
         float VdotH = pow(saturate(dot(V, -H)), power) * scale;
         float3 I =  (VdotH) * thickness;
         return lightColor * I * tint;
      }


      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif
      

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##3 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv3.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #endif

      half2 BlendNormal2(half2 base, half2 blend) { return normalize(half3(base.xy + blend.xy, 1)).xy; } 
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }
      half3 BlendLighterColor(half3 s, half3 d) { return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d; } 
      
      
      #if _SURFACENORMALS  

      #define HALF_EPS 4.8828125e-4    // 2^-11, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)

      

      void ConstructSurfaceGradientTBN(Input i)
      {
         float3x3 tbn = GetTBN(i);
         float3 t = tbn[0];
         float3 b = tbn[1];
         float3 n = tbn[2];

         surfNormal = n;//mul(GetWorldToObjectMatrix(), float4(n, 1)).xyz;
         surfTangent = t;//mul(GetWorldToObjectMatrix(), float4(t, 1)).xyz;
         surfBitangent = b;//cross(surfNormal, surfTangent);
         
         float renormFactor = 1.0 / length(surfNormal);
         surfNormal    *= renormFactor;
         surfTangent   *= renormFactor;
         surfBitangent *= renormFactor;
      }
      
      half3 SurfaceGradientFromTBN(half2 deriv)
      {
          return deriv.x * surfTangent + deriv.y * surfBitangent;
      }

      // Input: vM is tangent space normal in [-1;1].
      // Output: convert vM to a derivative.
      half2 TspaceNormalToDerivative(half3 vM)
      {
         const half scale = 1.0/128.0;
         
         // Ensure vM delivers a positive third component using abs() and
         // constrain vM.z so the range of the derivative is [-128; 128].
         const half3 vMa = abs(vM);
         const half z_ma = max(vMa.z, scale*max(vMa.x, vMa.y));

         return -half2(vM.x, vM.y)/z_ma;
      }

      // Used to produce a surface gradient from the gradient of a volume
      // bump function such as 3D Perlin noise. Equation 2 in [Mik10].
      half3 SurfgradFromVolumeGradient(half3 grad)
      {
         return grad - dot(surfNormal, grad) * surfNormal;
      }

      half3 SurfgradFromTriplanarProjection(half3 pN, half2 xPlaneTN, half2 yPlaneTN, half2 zPlaneTN)
      {
         const half w0 = pN.x;
         const half w1 = pN.y;
         const half w2 = pN.z;
         
         // X-plane tangent normal to gradient derivative
         xPlaneTN = xPlaneTN * 2.0 - 1.0;
         half xPlaneRcpZ = rsqrt(max(1 - dot(xPlaneTN.x, xPlaneTN.x) - dot(xPlaneTN.y, xPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_xplane = xPlaneTN * -xPlaneRcpZ;

         // Y-plane tangent normal to gradient derivative
         yPlaneTN = yPlaneTN * 2.0 - 1.0;
         half yPlaneRcpZ = rsqrt(max(1 - dot(yPlaneTN.x, yPlaneTN.x) - dot(yPlaneTN.y, yPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_yplane = yPlaneTN * -yPlaneRcpZ;

         // Z-plane tangent normal to gradient derivative
         zPlaneTN = zPlaneTN * 2.0 - 1.0;
         half zPlaneRcpZ = rsqrt(max(1 - dot(zPlaneTN.x, zPlaneTN.x) - dot(zPlaneTN.y, zPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_zplane = zPlaneTN * -zPlaneRcpZ;

         // Assume deriv xplane, deriv yplane, and deriv zplane are
         // sampled using (z,y), (x,z), and (x,y), respectively.
         // Positive scales of the lookup coordinate will work
         // as well, but for negative scales the derivative components
         // will need to be negated accordingly.
         float3 grad = float3(w2*d_zplane.x + w1*d_yplane.x,
                              w2*d_zplane.y + w0*d_xplane.y,
                              w0*d_xplane.x + w1*d_yplane.y);

         return SurfgradFromVolumeGradient(grad);
      }

      half3 ConvertNormalToGradient(half3 normal)
      {
         half2 deriv = TspaceNormalToDerivative(normal);

         return SurfaceGradientFromTBN(deriv);
      }

      half3 ConvertNormal2ToGradient(half2 packedNormal)
      {
         half2 tNormal = packedNormal;
         half rcpZ = rsqrt(max(1 - dot(tNormal.x, tNormal.x) - dot(tNormal.y, tNormal.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
         half2 deriv = tNormal * -rcpZ;
         return SurfaceGradientFromTBN(deriv);
      }


      half3 ResolveNormalFromSurfaceGradient(half3 gradient)
      {
         return normalize(surfNormal - gradient);
      }
      

      #endif // _SURFACENORMALS

      void BlendNormalPerTex(inout RawSamples o, half2 noise, float4 fades)
      {
         #if _SURFACENORMALS
            float3 grad = ConvertNormal2ToGradient(noise.xy);
            o.surf0 += grad * fades.x;
            o.surf1 += grad * fades.y;
            #if !_MAX2LAYER
               o.surf2 += grad * fades.z;
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.surf3 += grad * fades.w;
            #endif
         #else
            o.normSAO0.xy = lerp(o.normSAO0.xy, BlendNormal2(o.normSAO0.xy, noise.xy), fades.x);
            o.normSAO1.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #if !_MAX2LAYER
               o.normSAO2.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO2.xy, noise.xy), fades.y);
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.normSAO3.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #endif
         #endif
      }
      
     
      
      half3 BlendNormal3(half3 n1, half3 n2)
      {
         n1 += float3( 0,  0, 1);
         n2 *= float3(-1, -1, 1);
         return n1*dot(n1, n2) / n1.z - n2;
      }
      
      half2 TransformTriplanarNormal(Input IN, float3x3 t2w, half3 axisSign, half3 absVertNormal,
               half3 pN, half2 a0, half2 a1, half2 a2)
      {
         
         a0 = a0 * 2 - 1;
         a1 = a1 * 2 - 1;
         a2 = a2 * 2 - 1;

         a0.x *= axisSign.x;
         a1.x *= axisSign.y;
         a2.x *= axisSign.z;
         
         half3 n0 = half3(a0.xy, 1);
         half3 n1 = half3(a1.xy, 1);
         half3 n2 = half3(a2.xy, 1);

         float3 wn = IN.worldNormal;

         n0 = BlendNormal3(half3(wn.zy, absVertNormal.x), n0);
         n1 = BlendNormal3(half3(wn.xz, absVertNormal.y), n1 * float3(-1, 1, 1)); 
         n2 = BlendNormal3(half3(wn.xy, absVertNormal.z), n2);
  
         n0.z *= axisSign.x;
         n1.z *= axisSign.y;
         n2.z *= -axisSign.z;

         half3 worldNormal = (n0.zyx * pN.x + n1.xzy * pN.y + n2.xyz * pN.z);
         return mul(t2w, worldNormal).xy;

      }
      
      // funcs
      
      inline half MSLuminance(half3 rgb)
      {
         #ifdef UNITY_COLORSPACE_GAMMA
            return dot(rgb, half3(0.22, 0.707, 0.071));
         #else
            return dot(rgb, half3(0.0396819152, 0.458021790, 0.00609653955));
         #endif
      }
      
      
      float2 Hash2D( float2 x )
      {
          float2 k = float2( 0.3183099, 0.3678794 );
          x = x*k + k.yx;
          return -1.0 + 2.0*frac( 16.0 * k*frac( x.x*x.y*(x.x+x.y)) );
      }

      float Noise2D(float2 p )
      {
         float2 i = floor( p );
         float2 f = frac( p );
         
         float2 u = f*f*(3.0-2.0*f);

         return lerp( lerp( dot( Hash2D( i + float2(0.0,0.0) ), f - float2(0.0,0.0) ), 
                           dot( Hash2D( i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                      lerp( dot( Hash2D( i + float2(0.0,1.0) ), f - float2(0.0,1.0) ), 
                           dot( Hash2D( i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
      }
      
      float FBM2D(float2 uv)
      {
         float f = 0.5000*Noise2D( uv ); uv *= 2.01;
         f += 0.2500*Noise2D( uv ); uv *= 1.96;
         f += 0.1250*Noise2D( uv ); 
         return f;
      }
      
      float3 Hash3D( float3 p )
      {
         p = float3( dot(p,float3(127.1,311.7, 74.7)),
                 dot(p,float3(269.5,183.3,246.1)),
                 dot(p,float3(113.5,271.9,124.6)));

         return -1.0 + 2.0*frac(sin(p)*437.5453123);
      }

      float Noise3D( float3 p )
      {
         float3 i = floor( p );
         float3 f = frac( p );
         
         float3 u = f*f*(3.0-2.0*f);

         return lerp( lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,0.0) ), f - float3(0.0,0.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,0.0) ), f - float3(1.0,0.0,0.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,0.0) ), f - float3(0.0,1.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,0.0) ), f - float3(1.0,1.0,0.0) ), u.x), u.y),
                      lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,1.0) ), f - float3(0.0,0.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,1.0) ), f - float3(1.0,0.0,1.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,1.0) ), f - float3(0.0,1.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,1.0) ), f - float3(1.0,1.0,1.0) ), u.x), u.y), u.z );
      }
      
      float FBM3D(float3 uv)
      {
         float f = 0.5000*Noise3D( uv ); uv *= 2.01;
         f += 0.2500*Noise3D( uv ); uv *= 1.96;
         f += 0.1250*Noise3D( uv ); 
         return f;
      }
      
     
      
      float GetSaturation(float3 c)
      {
         float mi = min(min(c.x, c.y), c.z);
         float ma = max(max(c.x, c.y), c.z);
         return (ma - mi)/(ma + 1e-7);
      }

      // Better Color Lerp, does not have darkening issue
      float3 BetterColorLerp(float3 a, float3 b, float x)
      {
         float3 ic = lerp(a, b, x) + float3(1e-6,0.0,0.0);
         float sd = abs(GetSaturation(ic) - lerp(GetSaturation(a), GetSaturation(b), x));
    
         float3 dir = normalize(float3(2.0 * ic.x - ic.y - ic.z, 2.0 * ic.y - ic.x - ic.z, 2.0 * ic.z - ic.y - ic.x));
         float lgt = dot(float3(1.0, 1.0, 1.0), ic);
    
         float ff = dot(dir, normalize(ic));
    
         const float dsp_str = 1.5;
         ic += dsp_str * dir * sd * ff * lgt;
         return saturate(ic);
      }
      
      
      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);
             half4 weights = half4(iWeights.x * max(h0,0.001), iWeights.y * max(h1,0.001), iWeights.z * max(h2,0.001), iWeights.w * max(h3,0.001));
             
             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);

             weights = TotalOne(weights);
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #elif _MAX20TEXTURES
         #define TEXCOUNT 20
      #elif _MAX24TEXTURES
         #define TEXCOUNT 24
      #elif _MAX28TEXTURES
         #define TEXCOUNT 28
      #elif _MAX32TEXTURES
         #define TEXCOUNT 32
      #else
         #define TEXCOUNT 16
      #endif

      #if _DECAL_SPLAT
      
      void DoMergeDecalSplats(half4 iWeights, half4 iIndexes, inout half4 indexes, inout half4 weights)
      {
         for (int i = 0; i < 4; ++i)
         {
            half w = iWeights[i];
            half index = iIndexes[i];
            if (w > weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = index;
            }
            else if (w > weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = index;
            }
            else if (w > weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = index;
            }
            else if (w > weights[3])
            {
               weights[3] = w;
               indexes[3] = index;
            }
         }

      }
      #endif


      void Setup(out half4 weights, float2 uv, out Config config, half4 w0, half4 w1, half4 w2, half4 w3, half4 w4, half4 w5, half4 w6, half4 w7, float3 worldPos, DecalOutput decalOutput)
      {
         config = (Config)0;
         half4 indexes = 0;

         config.uv = uv;

         #if _WORLDUV
         uv = worldPos.xz;
         #endif

         #if _DISABLESPLATMAPS
            float2 scaledUV = uv;
         #else
            float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;
         #endif

         // if only 4 textures, and blending 4 textures, skip this whole thing..
         // this saves about 25% of the ALU of the base shader on low end. However if
         // we rely on sorted texture weights (distance resampling) we have to sort..
         float4 defaultIndexes = float4(0,1,2,3);
         #if _MESHSUBARRAY
            defaultIndexes = _MeshSubArrayIndexes;
         #endif

         #if _MESHSUBARRAY && !_DECAL_SPLAT || (_MAX4TEXTURES && !_MAX3LAYER && !_MAX2LAYER && !_DISTANCERESAMPLE && !_POM && !_DECAL_SPLAT)
            weights = w0;
            config.uv0 = float3(scaledUV, defaultIndexes.x);
            config.uv1 = float3(scaledUV, defaultIndexes.y);
            config.uv2 = float3(scaledUV, defaultIndexes.z);
            config.uv3 = float3(scaledUV, defaultIndexes.w);
            return;
         #endif

         #if _DISABLESPLATMAPS
            weights = float4(1,0,0,0);
            return;
         #else
            half splats[TEXCOUNT];

            splats[0] = w0.x;
            splats[1] = w0.y;
            splats[2] = w0.z;
            splats[3] = w0.w;
            #if !_MAX4TEXTURES
               splats[4] = w1.x;
               splats[5] = w1.y;
               splats[6] = w1.z;
               splats[7] = w1.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES
               splats[8] = w2.x;
               splats[9] = w2.y;
               splats[10] = w2.z;
               splats[11] = w2.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
               splats[12] = w3.x;
               splats[13] = w3.y;
               splats[14] = w3.z;
               splats[15] = w3.w;
            #endif
            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[16] = w4.x;
               splats[17] = w4.y;
               splats[18] = w4.z;
               splats[19] = w4.w;
            #endif
            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[20] = w5.x;
               splats[21] = w5.y;
               splats[22] = w5.z;
               splats[23] = w5.w;
            #endif
            #if _MAX28TEXTURES || _MAX32TEXTURES
               splats[24] = w6.x;
               splats[25] = w6.y;
               splats[26] = w6.z;
               splats[27] = w6.w;
            #endif
            #if _MAX32TEXTURES
               splats[28] = w7.x;
               splats[29] = w7.y;
               splats[30] = w7.z;
               splats[31] = w7.w;
            #endif



            weights[0] = 0;
            weights[1] = 0;
            weights[2] = 0;
            weights[3] = 0;
            indexes[0] = 0;
            indexes[1] = 0;
            indexes[2] = 0;
            indexes[3] = 0;

            int i = 0;
            for (i = 0; i < TEXCOUNT; ++i)
            {
               half w = splats[i];
               if (w >= weights[0])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = weights[0];
                  indexes[1] = indexes[0];
                  weights[0] = w;
                  indexes[0] = i;
               }
               else if (w >= weights[1])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = w;
                  indexes[1] = i;
               }
               else if (w >= weights[2])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = w;
                  indexes[2] = i;
               }
               else if (w >= weights[3])
               {
                  weights[3] = w;
                  indexes[3] = i;
               }
            }

            #if _DECAL_SPLAT
               DoMergeDecalSplats(decalOutput.Weights, decalOutput.Indexes, weights, indexes); 
            #endif


            
            
            // clamp and renormalize
            #if _MAX2LAYER
               weights.zw = 0;
               weights.xy = TotalOne(weights.xy);
            #elif _MAX3LAYER
               weights.w = 0;
               weights.xyz = TotalOne(weights.xyz);
            #elif !_DISABLEHEIGHTBLENDING || _NORMALIZEWEIGHTS // prevents black when painting, which the unity shader does not prevent.
               weights = normalize(weights);
            #endif
            

            config.uv0 = float3(scaledUV, indexes.x);
            config.uv1 = float3(scaledUV, indexes.y);
            config.uv2 = float3(scaledUV, indexes.z);
            config.uv3 = float3(scaledUV, indexes.w);


         #endif //_DISABLESPLATMAPS


      }

      float3 HeightToNormal(float height, float3 worldPos)
      {
         float3 dx = ddx(worldPos);
         float3 dy = ddy(worldPos);
         float3 crossX = cross(float3(0,1,0), dx);
         float3 crossY = cross(float3(0,1,0), dy);
         float3 d = abs(dot(crossY, dx));
         float3 n = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
         n.z *= -1;
         return normalize((d * float3(0,1,0)) - n).xzy;
      }
      
      float ComputeMipLevel(float2 uv, float2 textureSize)
      {
         uv *= textureSize;
         float2  dx_vtc        = ddx(uv);
         float2  dy_vtc        = ddy(uv);
         float delta_max_sqr   = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
         return 0.5 * log2(delta_max_sqr);
      }

      inline half2 UnpackNormal2(half4 packednormal)
      {
          return packednormal.wy * 2 - 1;
         
      }

      half3 TriplanarHBlend(half h0, half h1, half h2, half3 pN, half contrast)
      {
         half3 blend = pN / dot(pN, half3(1,1,1));
         float3 heights = float3(h0, h1, h2) + (blend * 3.0);
         half height_start = max(max(heights.x, heights.y), heights.z) - contrast;
         half3 h = max(heights - height_start.xxx, half3(0,0,0));
         blend = h / dot(h, half3(1,1,1));
         return blend;
      }
      

      void ClearAllButAlbedo(inout MicroSplatLayer o, half3 display)
      {
         o.Albedo = display.rgb;
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

      void ClearAllButAlbedo(inout MicroSplatLayer o, half display)
      {
         o.Albedo = half3(display, display, display);
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

     

      half MicroShadow(float3 lightDir, half3 normal, half ao, half strength)
      {
         half shadow = saturate(abs(dot(normal, lightDir)) + (ao * ao * 2.0) - 1.0);
         return 1 - ((1-shadow) * strength);
      }
      

      void DoDebugOutput(inout MicroSplatLayer l)
      {
         #if _DEBUG_OUTPUT_ALBEDO
            ClearAllButAlbedo(l, l.Albedo);
         #elif _DEBUG_OUTPUT_NORMAL
            // oh unit shader compiler normal stripping, how I hate you so..
            // must multiply by albedo to stop the normal from being white. Why, fuck knows?
            ClearAllButAlbedo(l, float3(l.Normal.xy * 0.5 + 0.5, l.Normal.z * saturate(l.Albedo.z+1)));
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            ClearAllButAlbedo(l, l.Smoothness.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_METAL
            ClearAllButAlbedo(l, l.Metallic.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_AO
            ClearAllButAlbedo(l, l.Occlusion.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_EMISSION
            ClearAllButAlbedo(l, l.Emission * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_HEIGHT
            ClearAllButAlbedo(l, l.Height.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_SPECULAR && _USESPECULARWORKFLOW
            ClearAllButAlbedo(l, l.Specular * saturate(l.Albedo.z+1));
         #elif _DEBUG_BRANCHCOUNT_WEIGHT
            ClearAllButAlbedo(l, _branchWeightCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TRIPLANAR
            ClearAllButAlbedo(l, _branchTriplanarCount / 24 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_CLUSTER
            ClearAllButAlbedo(l, _branchClusterCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_OTHER
            ClearAllButAlbedo(l, _branchOtherCount / 8 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TOTAL
            l.Albedo.r = _branchWeightCount / 12;
            l.Albedo.g = _branchTriplanarCount / 24;
            l.Albedo.b = _branchClusterCount / 12;
            ClearAllButAlbedo(l, (l.Albedo.r + l.Albedo.g + l.Albedo.b + (_branchOtherCount / 8)) / 4); 
         #elif _DEBUG_OUTPUT_MICROSHADOWS
            ClearAllButAlbedo(l,l.Albedo); 
         #elif _DEBUG_SAMPLECOUNT
            float sdisp = (float)_sampleCount / max(_SampleCountDiv, 1);
            half3 sdcolor = float3(sdisp, sdisp > 1 ? 1 : 0, 0);
            ClearAllButAlbedo(l, sdcolor * saturate(l.Albedo.z + 1));
         #elif _DEBUG_PROCLAYERS
            ClearAllButAlbedo(l, (float)_procLayerCount / (float)_PCLayerCount * saturate(l.Albedo.z + 1));
         #endif
      }


      // abstraction around sampler mode
      #if _USELODMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_LOD(tex, sampler##tex, u, l.x)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u, l.x)
      #elif _USEGRADMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_GRAD(tex, sampler##tex, u, l.xy, l.zw)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY_GRAD(tex, ss, u.xy, u.z, l.xy, l.zw)
      #else
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, u.xy, u.z)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u.xy, y.z)
      #endif


      #define MICROSPLAT_SAMPLE_DIFFUSE(u, cl, l) MICROSPLAT_SAMPLE(_Diffuse, u, l)
      #define MICROSPLAT_SAMPLE_EMIS(u, cl, l) MICROSPLAT_SAMPLE(_EmissiveMetal, u, l)
      #define MICROSPLAT_SAMPLE_DIFFUSE_LOD(u, cl, l) UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, u, l)
      

      #if _PACKINGHQ
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) half4(MICROSPLAT_SAMPLE(_NormalSAO, u, l).ga, MICROSPLAT_SAMPLE(_SmoothAO, u, l).ga).brag
      #else
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) MICROSPLAT_SAMPLE(_NormalSAO, u, l)
      #endif

      #if _USESPECULARWORKFLOW
         #define MICROSPLAT_SAMPLE_SPECULAR(u, cl, l) MICROSPLAT_SAMPLE(_Specular, u, l)
      #endif
      
      struct SimpleTriplanarConfig
      {
         float3 pn;
         float2 uv0;
         float2 uv1;
         float2 uv2;
      };
         
      void PrepSimpleTriplanarConfig(inout SimpleTriplanarConfig tc, float3 worldPos, float3 normal, float contrast)
      {
         tc.pn = pow(abs(normal), contrast);
         tc.pn = tc.pn / (tc.pn.x + tc.pn.y + tc.pn.z);
         
         half3 axisSign = sign(normal);

         tc.uv0 = worldPos.zy * axisSign.x;
         tc.uv1 = worldPos.xz * axisSign.y;
         tc.uv2 = worldPos.xy * axisSign.z;
      }
      
      #define SimpleTriplanarSample(tex, tc, scale) (SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv0 * scale) * tc.pn.x + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv1 * scale) * tc.pn.y + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv2 * scale) * tc.pn.z)
      #define SimpleTriplanarSampleLOD(tex, tc, scale, lod) (SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv0 * scale, lod) * tc.pn.x + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv1 * scale, lod) * tc.pn.y + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv2 * scale, lod) * tc.pn.z)
      #define SimpleTriplanarSampleGrad(tex, tc, scale) (SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv0 * scale, ddx(tc.uv0) * scale, ddy(tc.uv0) * scale) * tc.pn.x + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv1 * scale, ddx(tc.uv1) * scale, ddy(tc.uv1) * scale) * tc.pn.y + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv2 * scale, ddx(tc.uv2) * scale, ddy(tc.uv2) * scale) * tc.pn.z)
   
      
      inline half3 MicroSplatDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
      {
          specColor = lerp (half3(0,0,0), albedo, metallic);
          oneMinusReflectivity = (1-metallic);
          return albedo * oneMinusReflectivity;
      }







      Input DescToInput(ShaderData IN)
      {
        Input s = (Input)0;
        s.shaderData = IN;
        s.TBN = IN.TBNMatrix;
        s.worldNormal = IN.worldSpaceNormal;
        s.worldPos = IN.worldSpacePosition;
        s.viewDir = IN.tangentSpaceViewDir;
        s.uv_Control0 = IN.texcoord0.xy;

        s.worldUpVector = float3(0,1,0);
        s.worldHeight = IN.worldSpacePosition.y;
  
        #if _PLANETVECTORS
            float3 rwp = mul(_PQSToLocal, float4(IN.worldSpacePosition, 1));
            s.worldHeight = distance(rwp, float3(0,0,0));
            s.worldUpVector = normalize(rwp);
        #endif

        #if _MICROMESH && _MESHUV2
            s.uv2_Diffuse = IN.texcoord1.xy;
        #endif

        #if _MEGASPLAT
           UnpackMegaSplat(s, IN);
        #endif
   
        #if _MICROVERTEXMESH || _MICRODIGGERMESH
            UnpackVertexWorkflow(s, IN);
        #endif

        #if _PLANETVECTORS
           DoPlanetDataInputCopy(s, IN);
        #endif
        
        return s;
     }
     


      void SampleAlbedo(inout Config config, inout TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
         
            half4 contrasts = _Contrast.xxxx;
            #if _PERTEXTRIPLANARCONTRAST
               SAMPLE_PER_TEX(ptc, 9.5, config, half4(1,0.5,0,0));
               contrasts = half4(ptc0.y, ptc1.y, ptc2.y, ptc3.y);
            #endif


            #if _PERTEXTRIPLANAR
               SAMPLE_PER_TEX(pttri, 9.5, config, half4(0,0,0,0));
            #endif

            {
               // For per-texture triplanar, we modify the view based blending factor of the triplanar
               // such that you get a pure blend of either top down projection, or with the top down projection
               // removed and renormalized. This causes dynamic flow control optimizations to kick in and avoid
               // the extra texture samples while keeping the code simple. Yay..

               // We also only have to do this in the Albedo, because the pN values will be adjusted after the
               // albedo is sampled, causing future samples to use this data. 
              
               #if _PERTEXTRIPLANAR
                  if (pttri0.x > 0.66)
                  {
                     tc.pN0 = half3(0,1,0);
                  }
                  else if (pttri0.x > 0.33)
                  {
                     tc.pN0.y = 0;
                     tc.pN0.xz = TotalOne(tc.pN0.xz);
                  }
               #endif


               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[0], config.cluster0, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[1], config.cluster0, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[2], config.cluster0, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN0;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN0, contrasts.x);
                  tc.pN0 = bf;
               #endif

               s.albedo0 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            MSBRANCH(weights.y)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri1.x > 0.66)
                  {
                     tc.pN1 = half3(0,1,0);
                  }
                  else if (pttri1.x > 0.33)
                  {
                     tc.pN1.y = 0;
                     tc.pN1.xz = TotalOne(tc.pN1.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[0], config.cluster1, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[1], config.cluster1, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  COUNTSAMPLE
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[2], config.cluster1, d2);
               }
               half3 bf = tc.pN1;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN1, contrasts.x);
                  tc.pN1 = bf;
               #endif


               s.albedo1 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri2.x > 0.66)
                  {
                     tc.pN2 = half3(0,1,0);
                  }
                  else if (pttri2.x > 0.33)
                  {
                     tc.pN2.y = 0;
                     tc.pN2.xz = TotalOne(tc.pN2.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[0], config.cluster2, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[1], config.cluster2, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[2], config.cluster2, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN2;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN2, contrasts.x);
                  tc.pN2 = bf;
               #endif
               

               s.albedo2 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {

               #if _PERTEXTRIPLANAR
                  if (pttri3.x > 0.66)
                  {
                     tc.pN3 = half3(0,1,0);
                  }
                  else if (pttri3.x > 0.33)
                  {
                     tc.pN3.y = 0;
                     tc.pN3.xz = TotalOne(tc.pN3.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[0], config.cluster3, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[1], config.cluster3, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[2], config.cluster3, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN3;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN3, contrasts.x);
               tc.pN3 = bf;
               #endif

               s.albedo3 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif

         #else
            s.albedo0 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv0, config.cluster0, mipLevel);
            COUNTSAMPLE

            MSBRANCH(weights.y)
            {
               s.albedo1 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv1, config.cluster1, mipLevel);
               COUNTSAMPLE
            }
            #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.albedo2 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               } 
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.albedo3 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
            #endif
         #endif

         #if _PERTEXHEIGHTOFFSET || _PERTEXHEIGHTCONTRAST
            SAMPLE_PER_TEX(ptHeight, 10.5, config, 1);

            #if _PERTEXHEIGHTOFFSET
               s.albedo0.a = saturate(s.albedo0.a + ptHeight0.b - 1);
               s.albedo1.a = saturate(s.albedo1.a + ptHeight1.b - 1);
               s.albedo2.a = saturate(s.albedo2.a + ptHeight2.b - 1);
               s.albedo3.a = saturate(s.albedo3.a + ptHeight3.b - 1);
            #endif
            #if _PERTEXHEIGHTCONTRAST
               s.albedo0.a = saturate(pow(s.albedo0.a + 0.5, abs(ptHeight0.a)) - 0.5);
               s.albedo1.a = saturate(pow(s.albedo1.a + 0.5, abs(ptHeight1.a)) - 0.5);
               s.albedo2.a = saturate(pow(s.albedo2.a + 0.5, abs(ptHeight2.a)) - 0.5);
               s.albedo3.a = saturate(pow(s.albedo3.a + 0.5, abs(ptHeight3.a)) - 0.5);
            #endif
         #endif
      }
      
      
      
      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif

         #if _NONORMALMAP || _AUTONORMAL
            s.normSAO0 = half4(0,0, 0, 1);
            s.normSAO1 = half4(0,0, 0, 1);
            s.normSAO2 = half4(0,0, 0, 1);
            s.normSAO3 = half4(0,0, 0, 1);
            return;
         #endif

         
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
            
            half3 absVertNormal = abs(tc.IN.worldNormal);
            float3x3 t2w = tc.IN.TBN;
            
            
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[0], config.cluster0, d0).agrb;
                  COUNTSAMPLE
               }            
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[1], config.cluster0, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[2], config.cluster0, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf0 = SurfgradFromTriplanarProjection(tc.pN0, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO0.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN0, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO0.zw = a0.zw * tc.pN0.x + a1.zw * tc.pN0.y + a2.zw * tc.pN0.z;
            }
            MSBRANCH(weights.y)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[0], config.cluster1, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[1], config.cluster1, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[2], config.cluster1, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf1 = SurfgradFromTriplanarProjection(tc.pN1, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO1.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN1, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO1.zw = a0.zw * tc.pN1.x + a1.zw * tc.pN1.y + a2.zw * tc.pN1.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);

               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[0], config.cluster2, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[1], config.cluster2, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[2], config.cluster2, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf2 = SurfgradFromTriplanarProjection(tc.pN2, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO2.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN2, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO2.zw = a0.zw * tc.pN2.x + a1.zw * tc.pN2.y + a2.zw * tc.pN2.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[0], config.cluster3, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[1], config.cluster3, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[2], config.cluster3, d2).agrb;
                  COUNTSAMPLE
               }

               #if _SURFACENORMALS
                  s.surf3 = SurfgradFromTriplanarProjection(tc.pN3, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO3.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN3, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO3.zw = a0.zw * tc.pN3.x + a1.zw * tc.pN3.y + a2.zw * tc.pN3.z;
            }
            #endif

         #else
            s.normSAO0 = MICROSPLAT_SAMPLE_NORMAL(config.uv0, config.cluster0, mipLevel).agrb;
            COUNTSAMPLE
            s.normSAO0.xy = s.normSAO0.xy * 2 - 1;

            #if _SURFACENORMALS
               s.surf0 = ConvertNormal2ToGradient(s.normSAO0.xy);
            #endif

            MSBRANCH(weights.y)
            {
               s.normSAO1 = MICROSPLAT_SAMPLE_NORMAL(config.uv1, config.cluster1, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO1.xy = s.normSAO1.xy * 2 - 1;

               #if _SURFACENORMALS
                  s.surf1 = ConvertNormal2ToGradient(s.normSAO1.xy);
               #endif
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               s.normSAO2 = MICROSPLAT_SAMPLE_NORMAL(config.uv2, config.cluster2, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf2 = ConvertNormal2ToGradient(s.normSAO2.xy);
               #endif
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               s.normSAO3 = MICROSPLAT_SAMPLE_NORMAL(config.uv3, config.cluster3, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf3 = ConvertNormal2ToGradient(s.normSAO3.xy);
               #endif
            }
            #endif
         #endif
      }

      void SampleEmis(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USEEMISSIVEMETAL
            #if _TRIPLANAR
            
               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  s.emisMetal0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }

                  s.emisMetal1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.emisMetal0 = MICROSPLAT_SAMPLE_EMIS(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.emisMetal1 = MICROSPLAT_SAMPLE_EMIS(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
                  MSBRANCH(weights.z)
                  {
                     s.emisMetal2 = MICROSPLAT_SAMPLE_EMIS(config.uv2, config.cluster2, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  MSBRANCH(weights.w)
                  {
                     s.emisMetal3 = MICROSPLAT_SAMPLE_EMIS(config.uv3, config.cluster3, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
            #endif
         #endif
      }
      
      void SampleSpecular(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USESPECULARWORKFLOW
            #if _TRIPLANAR

               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.specular0 = MICROSPLAT_SAMPLE_SPECULAR(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.specular1 = MICROSPLAT_SAMPLE_SPECULAR(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.specular2 = MICROSPLAT_SAMPLE_SPECULAR(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.specular3 = MICROSPLAT_SAMPLE_SPECULAR(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
               #endif
            #endif
         #endif
      }

      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex, DecalOutput decalOutput)
      {
         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         half4 albedo = 0;
         half4 normSAO = half4(0,0,0,1);
         half3 surfGrad = half3(0,0,0);
         half4 emisMetal = 0;
         half3 specular = 0;
         
         float worldHeight = i.worldPos.y;
         float3 upVector = float3(0,1,0);
         
         #if _GLOBALTINT || _GLOBALNORMALS || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _GLOBALSPECULAR
            float globalSlopeFilter = 1;
            #if _GLOBALSLOPEFILTER
               float2 gfilterUV = float2(1 - saturate(dot(worldNormalVertex, upVector) * 0.5 + 0.49), 0.5);
               globalSlopeFilter = SAMPLE_TEXTURE2D(_GlobalSlopeTex, sampler_Diffuse, gfilterUV).a;
            #endif
         #endif

         // declare outside of branchy areas..
         half4 fxLevels = half4(0,0,0,0);
         half burnLevel = 0;
         half wetLevel = 0;
         half3 waterNormalFoam = half3(0, 0, 0);
         half porosity = 0.4;
         float streamFoam = 1.0f;
         half pud = 0;
         half snowCover = 0;
         half SSSThickness = 0;
         half3 SSSTint = half3(1,1,1);
         float traxBuffer = 0;
         float3 traxNormal = 0;
         float2 noiseUV = 0;
         
         

         #if _SPLATFADE
         MSBRANCHOTHER(1 - saturate(camDist - _SplatFade.y))
         {
         #endif

         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE || _SNOWFOOTSTEPS
            traxBuffer = SampleTraxBuffer(i.worldPos, worldNormalVertex, traxNormal);
         #endif
         
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
            #if _MICROMESH
               fxLevels = SampleFXLevels(InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, config.uv), wetLevel, burnLevel, traxBuffer);
            #elif _MICROVERTEXMESH || _MICRODIGGERMESH  || _MEGASPLAT
               fxLevels = ProcessFXLevels(i.fx, traxBuffer);
            #else
               fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel, traxBuffer);
            #endif
         #endif

         #if _DECAL
            fxLevels = max(fxLevels, decalOutput.fxLevels);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         

         MIPFORMAT albedoLOD = INITMIPFORMAT
         MIPFORMAT normalLOD = INITMIPFORMAT
         MIPFORMAT emisLOD = INITMIPFORMAT
         MIPFORMAT specLOD = INITMIPFORMAT
         MIPFORMAT origAlbedoLOD = INITMIPFORMAT;

         #if _TRIPLANAR && !_DISABLESPLATMAPS
            PrepTriplanar(i.shaderData.texcoord0, worldNormalVertex, i.worldPos, config, tc, weights, albedoLOD, normalLOD, emisLOD, origAlbedoLOD);
            tc.IN = i;
         #endif
         
         
         #if !_TRIPLANAR && !_DISABLESPLATMAPS
            #if _USELODMIP
               albedoLOD = ComputeMipLevel(config.uv0.xy, _Diffuse_TexelSize.zw);
               normalLOD = ComputeMipLevel(config.uv0.xy, _NormalSAO_TexelSize.zw);
               #if _USEEMISSIVEMETAL
                  emisLOD = ComputeMipLevel(config.uv0.xy, _EmissiveMetal_TexelSize.zw);
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = ComputeMipLevel(config.uv0.xy, _Specular_TexelSize.zw);;
               #endif
            #elif _USEGRADMIP
               albedoLOD = float4(ddx(config.uv0.xy), ddy(config.uv0.xy));
               normalLOD = albedoLOD;
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
            #endif

            origAlbedoLOD = albedoLOD;
         #endif

         #if _PERTEXCURVEWEIGHT
           SAMPLE_PER_TEX(ptCurveWeight, 19.5, config, half4(0.5,1,1,1));
           weights.x = lerp(smoothstep(0.5 - ptCurveWeight0.r, 0.5 + ptCurveWeight0.r, weights.x), weights.x, ptCurveWeight0.r*2);
           weights.y = lerp(smoothstep(0.5 - ptCurveWeight1.r, 0.5 + ptCurveWeight1.r, weights.y), weights.y, ptCurveWeight1.r*2);
           weights.z = lerp(smoothstep(0.5 - ptCurveWeight2.r, 0.5 + ptCurveWeight2.r, weights.z), weights.z, ptCurveWeight2.r*2);
           weights.w = lerp(smoothstep(0.5 - ptCurveWeight3.r, 0.5 + ptCurveWeight3.r, weights.w), weights.w, ptCurveWeight3.r*2);
           weights = TotalOne(weights);
         #endif
         
         

         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR && !_DISABLESPLATMAPS
            
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif

            // fix for pertex uv scale using gradient sampler and weight blended derivatives
            #if _USEGRADMIP
               albedoLOD = albedoLOD * ptUVScale0.rgrg * weights.x + 
                           albedoLOD * ptUVScale1.rgrg * weights.y + 
                           albedoLOD * ptUVScale2.rgrg * weights.z + 
                           albedoLOD * ptUVScale3.rgrg * weights.w;
               normalLOD = albedoLOD;
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
            #endif
         #endif

         #if _PERTEXUVROTATION && !_TRIPLANAR && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptUVRot, 16.5, config, half4(0,0,0,0));
            config.uv0.xy = RotateUV(config.uv0.xy, ptUVRot0.x);
            config.uv1.xy = RotateUV(config.uv1.xy, ptUVRot1.x);
            #if !_MAX2LAYER
               config.uv2.xy = RotateUV(config.uv2.xy, ptUVRot2.x);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = RotateUV(config.uv3.xy, ptUVRot0.x);
            #endif
         #endif

         
         o.Alpha = 1;

         
         #if _POM && !_DISABLESPLATMAPS
            DoPOM(i, config, tc, albedoLOD, weights, camDist, worldNormalVertex);
         #endif
         

         SampleAlbedo(config, tc, samples, albedoLOD, weights);

         #if _NOISEHEIGHT
            ApplyNoiseHeight(samples, config.uv, config, i.worldPos, worldNormalVertex);
         #endif
         
         #if _STREAMS || (_PARALLAX && !_DISABLESPLATMAPS)
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(i, config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX && !_DISABLESPLATMAPS
            DoParallax(i, earlyHeight, config, tc, samples, weights, camDist);
         #endif


         // Blend results
         #if _PERTEXINTERPCONTRAST && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = clamp(contrast + _Contrast, 0.0001, 1.0); 
            half cnt = contrast.x * weights.x + contrast.y * weights.y + contrast.z * weights.z + contrast.w * weights.w;
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, cnt);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif

         #if _HYBRIDHEIGHTBLEND
            heightWeights = lerp(heightWeights, TotalOne(weights), saturate(camDist/max(1.0, _HybridHeightBlendDistance)));
         #endif

         
         // rescale derivatives after height weighting. Basically, in gradmip mode we blend the mip levels,
         // but this is before height mapping is sampled, so reblending them after alpha will make sure the other
         // channels (normal, etc) are sharper, which likely matters most.. 
         #if _PERTEXUVSCALEOFFSET && !_DISABLESPLATMAPS
            #if _TRIPLANAR
               #if _USEGRADMIP
                  SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));

                  albedoLOD.d0 = origAlbedoLOD.d0 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d0 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d0 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d0 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d1 = origAlbedoLOD.d1 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d1 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d1 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d1 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d2 = origAlbedoLOD.d2 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d2 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d2 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d2 * ptUVScale3.xyxy * heightWeights.w;
               
                  normalLOD.d0 = albedoLOD.d0;
                  normalLOD.d1 = albedoLOD.d1;
                  normalLOD.d2 = albedoLOD.d2;
               
                  #if _USEEMISSIVEMETAL
                     emisLOD.d0 = albedoLOD.d0;
                     emisLOD.d1 = albedoLOD.d1;
                     emisLOD.d2 = albedoLOD.d2;
                  #endif
               #endif // gradmip
            #else // not triplanar
               // fix for pertex uv scale using gradient sampler and weight blended derivatives
               #if _USEGRADMIP
                  albedoLOD = origAlbedoLOD * ptUVScale0.rgrg * heightWeights.x + 
                              origAlbedoLOD * ptUVScale1.rgrg * heightWeights.y + 
                              origAlbedoLOD * ptUVScale2.rgrg * heightWeights.z + 
                              origAlbedoLOD * ptUVScale3.rgrg * heightWeights.w;
                  normalLOD = albedoLOD;
                  #if _USEEMISSIVEMETAL
                     emisLOD = albedoLOD;
                  #endif
                  #if _USESPECULARWORKFLOW
                     specLOD = albedoLOD;
                  #endif
               #endif
            #endif
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples, albedoLOD, heightWeights);
         #endif


         SampleNormal(config, tc, samples, normalLOD, heightWeights);

         #if _USEEMISSIVEMETAL
            SampleEmis(config, tc, samples, emisLOD, heightWeights);
         #endif

         #if _USESPECULARWORKFLOW
            SampleSpecular(config, tc, samples, specLOD, heightWeights);
         #endif

         #if _DISTANCERESAMPLE && !_DISABLESPLATMAPS
            DistanceResample(samples, config, tc, camDist, i.viewDir, fxLevels, albedoLOD, i.worldPos, heightWeights, worldNormalVertex);
         #endif

         #if _STARREACHFORMAT
            samples.normSAO0.w = length(samples.normSAO0.xy);
            samples.normSAO1.w = length(samples.normSAO1.xy);
            samples.normSAO2.w = length(samples.normSAO2.xy);
            samples.normSAO3.w = length(samples.normSAO3.xy);
         #endif

         // PerTexture sampling goes here, passing the samples structure
         
         #if _PERTEXMICROSHADOWS || _PERTEXFUZZYSHADE
            SAMPLE_PER_TEX(ptFuzz, 17.5, config, half4(0, 0, 1, 1));
         #endif

         #if _PERTEXMICROSHADOWS
            #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP)
            {
               half3 lightDir = GetGlobalLightDirTS(i);
               half4 microShadows = half4(1,1,1,1);
               microShadows.x = MicroShadow(lightDir, half3(samples.normSAO0.xy, 1), samples.normSAO0.a, ptFuzz0.a);
               microShadows.y = MicroShadow(lightDir, half3(samples.normSAO1.xy, 1), samples.normSAO1.a, ptFuzz1.a);
               microShadows.z = MicroShadow(lightDir, half3(samples.normSAO2.xy, 1), samples.normSAO2.a, ptFuzz2.a);
               microShadows.w = MicroShadow(lightDir, half3(samples.normSAO3.xy, 1), samples.normSAO3.a, ptFuzz3.a);
               samples.normSAO0.a *= microShadows.x;
               samples.normSAO1.a *= microShadows.y;
               #if !_MAX2LAYER
                  samples.normSAO2.a *= microShadows.z;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.normSAO3.a *= microShadows.w;
               #endif

               
               #if _DEBUG_OUTPUT_MICROSHADOWS
               o.Albedo = BlendWeights(microShadows.x, microShadows.y, microShadows.z, microShadows.a, heightWeights);
               return o;
               #endif

               

               
            }
            #endif

         #endif // _PERTEXMICROSHADOWS


         #if _PERTEXFUZZYSHADE
            samples.albedo0.rgb = FuzzyShade(samples.albedo0.rgb, half3(samples.normSAO0.rg, 1), ptFuzz0.r, ptFuzz0.g, ptFuzz0.b, i.viewDir);
            samples.albedo1.rgb = FuzzyShade(samples.albedo1.rgb, half3(samples.normSAO1.rg, 1), ptFuzz1.r, ptFuzz1.g, ptFuzz1.b, i.viewDir);
            #if !_MAX2LAYER
               samples.albedo2.rgb = FuzzyShade(samples.albedo2.rgb, half3(samples.normSAO2.rg, 1), ptFuzz2.r, ptFuzz2.g, ptFuzz2.b, i.viewDir);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = FuzzyShade(samples.albedo3.rgb, half3(samples.normSAO3.rg, 1), ptFuzz3.r, ptFuzz3.g, ptFuzz3.b, i.viewDir);
            #endif
         #endif

         #if _PERTEXSATURATION && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptSaturattion, 9.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = lerp(MSLuminance(samples.albedo0.rgb), samples.albedo0.rgb, ptSaturattion0.a);
            samples.albedo1.rgb = lerp(MSLuminance(samples.albedo1.rgb), samples.albedo1.rgb, ptSaturattion1.a);
            #if !_MAX2LAYER
               samples.albedo2.rgb = lerp(MSLuminance(samples.albedo2.rgb), samples.albedo2.rgb, ptSaturattion2.a);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = lerp(MSLuminance(samples.albedo3.rgb), samples.albedo3.rgb, ptSaturattion3.a);
            #endif
         
         #endif
         
         #if _PERTEXTINT && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif
         
         #if _PCHEIGHTGRADIENT || _PCHEIGHTHSV || _PCSLOPEGRADIENT || _PCSLOPEHSV
            ProceduralGradients(i, samples, config, worldHeight, worldNormalVertex);
         #endif

         
         

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXCOLORINTENSITY
            SAMPLE_PER_TEX(ptCI, 23.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = saturate(samples.albedo0.rgb * (1 + ptCI0.rrr));
            samples.albedo1.rgb = saturate(samples.albedo1.rgb * (1 + ptCI1.rrr));
            #if !_MAX2LAYER
               samples.albedo2.rgb = saturate(samples.albedo2.rgb * (1 + ptCI2.rrr));
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = saturate(samples.albedo3.rgb * (1 + ptCI3.rrr));
            #endif
         #endif

         #if (_PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = saturate(((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5);
               samples.albedo1.rgb = saturate(((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5);
               #if !_MAX2LAYER
                 samples.albedo2.rgb = saturate(((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5);
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb = saturate(samples.albedo0.rgb + ptBC0.rrr);
               samples.albedo1.rgb = saturate(samples.albedo1.rgb + ptBC1.rrr);
               #if !_MAX2LAYER
                  samples.albedo2.rgb = saturate(samples.albedo2.rgb + ptBC2.rrr);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(samples.albedo3.rgb + ptBC3.rrr);
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if (_PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR && !_DISABLESPLATMAPS
            #if _SURFACENORMALS
               samples.surf0 *= perTexMatSettings0.r;
               samples.surf1 *= perTexMatSettings1.r;
               samples.surf2 *= perTexMatSettings2.r;
               samples.surf3 *= perTexMatSettings3.r;
            #else
               samples.normSAO0.xy *= perTexMatSettings0.r;
               samples.normSAO1.xy *= perTexMatSettings1.r;
               samples.normSAO2.xy *= perTexMatSettings2.r;
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR && !_DISABLESPLATMAPS
            samples.normSAO0.a = pow(abs(samples.normSAO0.a), perTexMatSettings0.b);
            samples.normSAO1.a = pow(abs(samples.normSAO1.a), perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(abs(samples.normSAO2.a), perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(abs(samples.normSAO3.a), perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR && !_DISABLESPLATMAPS
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         
         #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP) 
          #if _PERTEXSSS
          {
            SAMPLE_PER_TEX(ptSSS, 18.5, config, half4(1, 1, 1, 1)); // tint, thickness
            half4 vals = ptSSS0 * heightWeights.x + ptSSS1 * heightWeights.y + ptSSS2 * heightWeights.z + ptSSS3 * heightWeights.w;
            SSSThickness = vals.a;
            SSSTint = vals.rgb;
          }
          #endif
         #endif

         #if _PERTEXRIMLIGHT
         {
            SAMPLE_PER_TEX(ptRimA, 26.5, config, half4(1, 1, 1, 1));
            SAMPLE_PER_TEX(ptRimB, 27.5, config, half4(1, 1, 1, 0));
            samples.emisMetal0.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO0.xy, 1))), max(0.0001, ptRimA0.g)) * ptRimB0.rgb * ptRimB0.a;
            samples.emisMetal1.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO1.xy, 1))), max(0.0001, ptRimA1.g)) * ptRimB1.rgb * ptRimB1.a;
            samples.emisMetal2.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO2.xy, 1))), max(0.0001, ptRimA2.g)) * ptRimB2.rgb * ptRimB2.a;
            samples.emisMetal3.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO3.xy, 1))), max(0.0001, ptRimA3.g)) * ptRimB3.rgb * ptRimB3.a;
         }
         #endif



         #if (((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)) && !_DISABLESPLATMAPS
            ApplyDetailDistanceNoisePerTex(samples, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         
         #if _GLOBALNOISEUV
            // noise defaults so that a value of 1, 1 is 4 pixels in size and moves the uvs by 1 pixel max.
            #if _CUSTOMSPLATTEXTURES
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #else
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif
         #endif

         
         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE
            ApplyTrax(samples, config, i.worldPos, traxBuffer, traxNormal);
         #endif

         #if (_ANTITILEARRAYDETAIL || _ANTITILEARRAYDISTANCE || _ANTITILEARRAYNORMAL) && !_DISABLESPLATMAPS
            ApplyAntiTilePerTex(samples, config, camDist, i.worldPos, worldNormalVertex, heightWeights);
         #endif

         #if _GEOMAP && !_DISABLESPLATMAPS
         GeoTexturePerTex(samples, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif
         
         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH && !_DISABLESPLATMAPS
         GlobalTintTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH && !_DISABLESPLATMAPS
         GlobalNormalTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && _PERTEXGLOBALSAOMSTRENGTH && !_DISABLESPLATMAPS
         GlobalSAOMTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALEMIS && _PERTEXGLOBALEMISSTRENGTH && !_DISABLESPLATMAPS
         GlobalEmisTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && _PERTEXGLOBALSPECULARSTRENGTH && !_DISABLESPLATMAPS && _USESPECULARWORKFLOW
         GlobalSpecularTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _PERTEXMETALLIC && !_DISABLESPLATMAPS
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         #if _GLITTER && !_DISABLESPLATMAPS
            DoGlitter(i, samples, config, camDist, worldNormalVertex, i.worldPos);
         #endif
         
         // Blend em..
         #if _DISABLESPLATMAPS
            // If we don't sample from the _Diffuse, then the shader compiler will strip the sampler on
            // some platforms, which will cause everything to break. So we sample from the lowest mip
            // and saturate to 1 to keep the cost minimal. Annoying, but the compiler removes the texture
            // and sampler, even though the sampler is still used.
            albedo = saturate(UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, float3(0,0,0), 12) + 1);
            albedo.a = 0.5; // make height something we can blend with for the combined mesh mode, since it still height blends.
            normSAO = half4(0,0,0,1);
         #else
            albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
            normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);

            #if _SURFACENORMALS
               surfGrad = BlendWeights(samples.surf0, samples.surf1, samples.surf2, samples.surf3, heightWeights);
            #endif

            #if (_USEEMISSIVEMETAL || _PERTEXRIMLIGHT) && !_DISABLESPLATMAPS
               emisMetal = BlendWeights(samples.emisMetal0, samples.emisMetal1, samples.emisMetal2, samples.emisMetal3, heightWeights);
            #endif

            #if _USESPECULARWORKFLOW && !_DISABLESPLATMAPS
               specular = BlendWeights(samples.specular0, samples.specular1, samples.specular2, samples.specular3, heightWeights);
            #endif

            #if _PERTEXOUTLINECOLOR
               SAMPLE_PER_TEX(ptOutlineColor, 28.5, config, half4(0.5, 0.5, 0.5, 1));
               half4 outlineColor = BlendWeights(ptOutlineColor0, ptOutlineColor1, ptOutlineColor2, ptOutlineColor3, heightWeights);
               half4 tstr = saturate(abs(heightWeights - 0.5) * 2);
               half transitionBlend = min(min(min(tstr.x, tstr.y), tstr.z), tstr.w);
               albedo.rgb = lerp(albedo.rgb * outlineColor.rgb * 2, albedo.rgb, outlineColor.a * transitionBlend);
            #endif
         #endif



         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
            o.Alpha = 1.0;
            if (config.uv0.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.x;
            else if (config.uv1.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.y;
            else if (config.uv2.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.z;
            else if (config.uv3.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.w;
         #endif



         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
            ApplyDetailDistanceNoise(albedo.rgb, normSAO, surfGrad, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         #if _SPLATFADE
         }
         #endif

         #if _SPLATFADE
            
            float2 sfDX = ddx(config.uv * _UVScale);
            float2 sfDY = ddy(config.uv * _UVScale);

            MSBRANCHOTHER(camDist - _SplatFade.x)
            {
               float falloff = saturate(InverseLerp(_SplatFade.x, _SplatFade.y, camDist));
               half4 sfalb = SAMPLE_TEXTURE2D_ARRAY_GRAD(_Diffuse, sampler_Diffuse, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY);
               COUNTSAMPLE
               albedo.rgb = lerp(albedo.rgb, sfalb.rgb, falloff);

               #if !_NONORMALMAP && !_AUTONORMAL
                  half4 sfnormSAO = SAMPLE_TEXTURE2D_ARRAY_GRAD(_NormalSAO, sampler_NormalSAO, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY).agrb;
                  COUNTSAMPLE
                  sfnormSAO.xy = sfnormSAO.xy * 2 - 1;

                  normSAO = lerp(normSAO, sfnormSAO, falloff);
  
                  #if _SURFACENORMALS
                     surfGrad = lerp(surfGrad, ConvertNormal2ToGradient(sfnormSAO.xy), falloff);
                  #endif
               #endif
              
            }
         #endif

         #if _AUTONORMAL
            float3 autoNormal = HeightToNormal(albedo.a * _AutoNormalHeightScale, i.worldPos);
            normSAO.xy = autoNormal;
            normSAO.z = 0;
            normSAO.w = (autoNormal.z * autoNormal.z);
         #endif
 


         #if _MESHCOMBINED
            SampleMeshCombined(albedo, normSAO, surfGrad, emisMetal, specular, o.Alpha, SSSThickness, SSSTint, config, heightWeights);
         #endif

         #if _ISOBJECTSHADER
            SampleObjectShader(i, albedo, normSAO, surfGrad, emisMetal, specular, config);
         #endif

         #if _GEOMAP
            GeoTexture(albedo.rgb, normSAO, surfGrad, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif

         
         #if _SCATTER
            ApplyScatter(
               #if _MEGASPLAT
               config, 
               #endif
               i, albedo, normSAO, surfGrad, config.uv, camDist);
         #endif

         #if _DECAL
            DoDecalBlend(decalOutput, albedo, normSAO, surfGrad, emisMetal, i.uv_Control0);
         #endif
         

         #if _GLOBALTINT && !_PERTEXGLOBALTINTSTRENGTH
            GlobalTintTexture(albedo.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _VSGRASSMAP
            VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMALSTRENGTH
            GlobalNormalTexture(normSAO, surfGrad, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && !_PERTEXGLOBALSAOMSTRENGTH
            GlobalSAOMTexture(normSAO, emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALEMIS && !_PERTEXGLOBALEMISSTRENGTH
            GlobalEmisTexture(emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && !_PERTEXGLOBALSPECULARSTRENGTH && _USESPECULARWORKFLOW
            GlobalSpecularTexture(specular.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         
         
         o.Albedo = albedo.rgb;
         o.Height = albedo.a;

         #if _NONORMALMAP
            o.Normal = half3(0,0,1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #elif _SURFACENORMALS
            o.Normal = ResolveNormalFromSurfaceGradient(surfGrad);
            o.Normal = mul(GetTBN(i), o.Normal);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #else
            o.Normal = half3(normSAO.xy, 1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;       
         #endif


         

         #if _USEEMISSIVEMETAL || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _PERTEXRIMLIGHT
           #if _USEEMISSIVEMETAL
	           emisMetal.rgb *= _EmissiveMult;
	        #endif
           
           o.Emission += emisMetal.rgb;
           o.Metallic = emisMetal.a;
	        
         #endif

         #if _USESPECULARWORKFLOW
            o.Specular = specular;
         #endif

         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(i, o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         
         #if _SNOW
         snowCover = DoSnow(i, o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, 
            config, weights, SSSTint, SSSThickness, traxBuffer, traxNormal);
         #endif

         #if _PERTEXSSS || _MESHCOMBINEDUSESSS || (_SNOW && _SNOWSSS)
         {
            half3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

            o.Emission += ComputeSSS(i, worldView, WorldNormalVector(i, o.Normal),
               SSSTint, SSSThickness, _SSSDistance, _SSSScale, _SSSPower);
         }
         #endif
         
         #if _SNOWGLITTER
            DoSnowGlitter(i, config, o, camDist, worldNormalVertex, snowCover);
         #endif

         #if _WINDPARTICULATE || _SNOWPARTICULATE
            DoWindParticulate(i, o, config, weights, camDist, worldNormalVertex, snowCover);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         #if _SPECULARFADE
         {
            float specFade = saturate((i.worldPos.y - _SpecularFades.x) / max(_SpecularFades.y - _SpecularFades.x, 0.0001));
            o.Metallic *= specFade;
            o.Smoothness *= specFade;
         }
         #endif

         #if _VSSHADOWMAP
         VSShadowTexture(o, i, config, camDist);
         #endif
         
         #if _TOONWIREFRAME
         ToonWireframe(config.uv, o.Albedo, camDist);
         #endif

        #if _SEETHROUGHSHADER
            SeethroughShader(o.Albedo, o.Emission, o.Alpha, i.worldPos, o.Normal, i.worldNormal);                   
        #endif


         #if _DEBUG_TRAXBUFFER
            ClearAllButAlbedo(o, half3(traxBuffer, 0, 0) * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMALVERTEX
            ClearAllButAlbedo(o, worldNormalVertex * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMAL
            ClearAllButAlbedo(o,  WorldNormalVector(i, o.Normal) * saturate(o.Albedo.z+1));
         #endif

         #if _DEBUG_MEGABARY && _MEGASPLAT
            o.Albedo = i.baryWeights.xyz;
         #endif


         return o;
      }
      
      void SampleSplats(float2 controlUV, inout half4 w0, inout half4 w1, inout half4 w2, inout half4 w3, inout half4 w4, inout half4 w5, inout half4 w6, inout half4 w7)
      {
         #if _CUSTOMSPLATTEXTURES
            #if !_MICROMESH
            controlUV = (controlUV * (_CustomControl0_TexelSize.zw - 1.0f) + 0.5f) * _CustomControl0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_CustomControl0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_CustomControl1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_CustomControl2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_CustomControl3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_CustomControl4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_CustomControl5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_CustomControl6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_CustomControl7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #else
            #if !_MICROMESH
            controlUV = (controlUV * (_Control0_TexelSize.zw - 1.0f) + 0.5f) * _Control0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_Control0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_Control1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_Control2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_Control3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_Control4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_Control5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_Control6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_Control7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #endif
      }   


      

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
         #if _MEGANOUV
            i.uv_Control0 = i.worldPos.xz;
         #endif
         
         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);
          
         #if _FORCELOCALSPACE
            worldNormalVertex = mul((float3x3)GetWorldToObjectMatrix(), worldNormalVertex).xyz;
            i.worldPos = i.worldPos -  mul(GetObjectToWorldMatrix(), float4(0,0,0,1)).xyz;
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ORIGINSHIFT
             i.worldPos = i.worldPos + mul(_GlobalOriginMTX, float4(0,0,0,1)).xyz;
             i.worldHeight = i.worldPos.y;
         #endif

         #if _DEBUG_USE_TOPOLOGY
            i.worldPos = SAMPLE_TEXTURE2D(_DebugWorldPos, sampler_Diffuse, i.uv_Control0);
            worldNormalVertex = SAMPLE_TEXTURE2D(_DebugWorldNormal, sampler_Diffuse, i.uv_Control0);
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ALPHABELOWHEIGHT && !_TBDISABLEALPHAHOLES
            ClipWaterLevel(i.worldPos);
         #endif

         #if !_TBDISABLEALPHAHOLES && defined(_ALPHATEST_ON)
            // UNITY 2019.3 holes
            ClipHoles(i.uv_Control0);
         #endif


         float2 origUV = i.uv_Control0;

         #if _MICROMESH && _MESHUV2
         float2 controlUV = i.uv2_Diffuse;
         #else
         float2 controlUV = i.uv_Control0;
         #endif


         #if _MICROMESH
            controlUV = InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, controlUV);
         #endif

         half4 weights = half4(1,0,0,0);

         Config config = (Config)0;
         UNITY_INITIALIZE_OUTPUT(Config,config);
         config.uv = origUV;

         DecalOutput decalOutput = (DecalOutput)0;
         #if _DECAL
            decalOutput = DoDecals(i.uv_Control0, i.worldPos, camDist, worldNormalVertex);
         #endif

         #if _SURFACENORMALS
         // Initialize the surface gradient basis vectors
         ConstructSurfaceGradientTBN(i);
         #endif
        


         #if _SPLATFADE
         MSBRANCHOTHER(_SplatFade.y - camDist)
         #endif // _SPLATFADE
         {
            #if !_DISABLESPLATMAPS

               // Sample the splat data, from textures or vertices, and setup the config..
               #if _MICRODIGGERMESH
                  DiggerSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLAT
                  MegaSplatVertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLATTEXTURE
                   MegaSplatTextureSetup(controlUV, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MICROVERTEXMESH
                  VertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif !_PROCEDURALTEXTURE || _PROCEDURALBLENDSPLATS
                  half4 w0 = 0; half4 w1 = 0; half4 w2 = 0; half4 w3 = 0; half4 w4 = 0; half4 w5 = 0; half4 w6 = 0; half4 w7 = 0;
                  SampleSplats(controlUV, w0, w1, w2, w3, w4, w5, w6, w7);
                  Setup(weights, origUV, config, w0, w1, w2, w3, w4, w5, w6, w7, i.worldPos, decalOutput);
               #endif

               #if _PROCEDURALTEXTURE
                  float3 procNormal = worldNormalVertex;
                  float3 worldPos = i.worldPos;
                  ProceduralSetup(i, worldPos, i.worldHeight, procNormal, i.worldUpVector, weights, origUV, config, ddx(origUV), ddy(origUV), ddx(worldPos), ddy(worldPos), decalOutput);
               #endif
            #else // _DISABLESPLATMAPS
                Setup(weights, origUV, config, half4(1,0,0,0), 0, 0, 0, 0, 0, 0, 0, i.worldPos, decalOutput);
            #endif

            #if _SLOPETEXTURE
               SlopeTexture(config, weights, worldNormalVertex);
            #endif
         } // _SPLATFADE else case


         #if _TOONFLATTEXTURE
            float2 quv = floor(origUV * _ToonTerrainSize);
            float2 fuv = frac(origUV * _ToonTerrainSize);
            #if !_TOONFLATTEXTUREQUAD
               quv = Hash2D((fuv.x > fuv.y) ? quv : quv * 0.333);
            #endif
            float2 uvq = quv / _ToonTerrainSize;
            config.uv0.xy = uvq;
            config.uv1.xy = uvq;
            config.uv2.xy = uvq;
            config.uv3.xy = uvq;
         #endif
         
         #if (_TEXTURECLUSTER2 || _TEXTURECLUSTER3) && !_DISABLESPLATMAPS
            PrepClusters(origUV, config, i.worldPos, worldNormalVertex);
         #endif

         #if (_ALPHAHOLE || _ALPHAHOLETEXTURE) && !_DISABLESPLATMAPS && !_TBDISABLEALPHAHOLES
         ClipAlphaHole(config, weights);
         #endif


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex, decalOutput);

         // On windows, sometimes the shared samplers gets stripped, so we have to do this crap.
         // We sample from the lowest mip, so it shouldn't cost much, but still, I hate this, wtf..
         float stripVal = saturate(SAMPLE_TEXTURE2D_LOD(_Diffuse, sampler_Diffuse, config.uv0, 11).r + 2);
         stripVal *= saturate(SAMPLE_TEXTURE2D_LOD(_NormalSAO, sampler_NormalSAO, config.uv0, 11).r + 2);
         l.Albedo *= stripVal;
         l.Normal *= stripVal;

         #if _PROCEDURALTEXTURE
            ProceduralTextureDebugOutput(l, weights, config);
         #endif


         return l;

      }



   





float4 ConstructTerrainTangent(float3 normal, float3 positiveZ)
{
    // Consider a flat terrain. It should have tangent be (1, 0, 0) and bitangent be (0, 0, 1) as the UV of the terrain grid mesh is a scale of the world XZ position.
    // In CreateTangentToWorld function (in SpaceTransform.hlsl), it is cross(normal, tangent) * sgn for the bitangent vector.
    // It is not true in a left-handed coordinate system for the terrain bitangent, if we provide 1 as the tangent.w. It would produce (0, 0, -1) instead of (0, 0, 1).
    // Also terrain's tangent calculation was wrong in a left handed system because cross((0,0,1), terrainNormalOS) points to the wrong direction as negative X.
    // Therefore all the 4 xyzw components of the tangent needs to be flipped to correct the tangent frame.
    // (See TerrainLitData.hlsl - GetSurfaceAndBuiltinData)
    float3 tangent = cross(normal, positiveZ);
    return float4(tangent, -1);
}



void TerrainInstancing(inout float4 vertex, inout float3 normal, inout float2 uv)
{
#if _MICROTERRAIN && defined(UNITY_INSTANCING_ENABLED) && !_TERRAINBLENDABLESHADER
   
    float2 patchVertex = vertex.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;

    float2 sampleUV = (uv / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

    float height = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, shared_linear_clamp_sampler, sampleUV, 0));
   
    vertex.xz = sampleCoords * _TerrainHeightmapScale.xz;
    vertex.y = height * _TerrainHeightmapScale.y;

    
    normal = float3(0, 1, 0);

#endif
}


void ApplyMeshModification(inout VertexData input)
{
   #if _MICROTERRAIN && !_TERRAINBLENDABLESHADER
      float2 uv = input.texcoord0.xy;
      TerrainInstancing(input.vertex, input.normal, uv);
      input.texcoord0.xy = uv;
   #endif
   #if _PERPIXNORMAL && !_TERRAINBLENDABLESHADER
      input.normal = float3(0,1,0);
   #endif

}

// called by the template, so we can remove tangent from VertexData
void ApplyTerrainTangent(inout VertexToPixel input)
{
   #if (_MICROTERRAIN || _PERPIXNORMAL) && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif

   // digger meshes ain't got no tangent either..
   #if _MICRODIGGERMESH && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif
}


void ModifyVertex(inout VertexData v, inout ExtraV2F d)
{
   ApplyMeshModification(v);

   #if _MICROVERTEXMESH || _MICRODIGGERMESH
      EncodeVertexWorkflow(v, d);
   #elif _MEGASPLAT
      EncodeMegaSplatVertex(v, d);
   #endif

}


void ModifyTessellatedVertex(inout VertexData v, inout ExtraV2F d)
{
   #if _TESSDISTANCE
      v.vertex.xyz += OffsetVertex(v, d);
   #endif
}

float3 GetTessFactors ()
{
   #if _TESSDISTANCE
      return float3(_TessData2.x, _TessData2.y, _TessData1.x);
   #endif
   return 0;
}


        


    
    void SurfaceFunction(inout Surface o, inout ShaderData d)
    {
       
        float3 worldNormalVertex = d.worldSpaceNormal;

        #if (defined(UNITY_INSTANCING_ENABLED) && _MICROTERRAIN && !_TERRAINBLENDABLESHADER)
            float2 sampleCoords = (d.texcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            float3 geomBitangent = normalize(cross(geomNormal, geomTangent)) * -1;
            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

         #elif _PERPIXNORMAL &&  (_MICROTERRAIN || _MICROMESHTERRAIN) && !_TERRAINBLENDABLESHADER
            float2 sampleCoords = (d.texcoord0.xy * _PerPixelNormal_TexelSize.zw + 0.5f) * _PerPixelNormal_TexelSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_PerPixelNormal, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            
            float3 geomBitangent = normalize(cross(geomTangent, geomNormal)) * -1;
            #if _MICROMESHTERRAIN
                geomBitangent *= -1;
            #endif

            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

        #endif

        #if _TOONPOLYEDGE
           FlatShade(d);
        #endif

         Input i = DescToInput(d);

         
         
         #if _SRPTERRAINBLEND
            MicroSplatLayer l = BlendWithTerrain(d);

               #if _DEBUG_WORLDNORMAL
                  ClearAllButAlbedo(l, normalize(TangentToWorldSpace(d, l.Normal)) * saturate(l.Albedo.z+1));
               #endif
         #else
            MicroSplatLayer l = SurfImpl(i, worldNormalVertex);
         #endif

        DoDebugOutput(l);




        o.Albedo = l.Albedo;
        o.Normal = l.Normal;
        o.Smoothness = l.Smoothness;
        o.Occlusion = l.Occlusion;
        o.Metallic = l.Metallic;
        o.Emission = l.Emission;
        #if _USESPECULARWORKFLOW
        o.Specular = l.Specular;
        #endif
        o.Height = l.Height;
        o.Alpha = l.Alpha;


    }



        



            // SHADERDESC

         ShaderData CreateShaderData(VertexToPixel i)
         {
            ShaderData d = (ShaderData)0;
            d.worldSpacePosition = i.worldPos;

            d.worldSpaceNormal = i.worldNormal;
            d.worldSpaceTangent = i.worldTangent.xyz;
            float3 bitangent = cross(i.worldTangent.xyz, i.worldNormal) * i.worldTangent.w * -1;
            

            d.TBNMatrix = float3x3(d.worldSpaceTangent, bitangent, d.worldSpaceNormal);
            d.worldSpaceViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);
             d.texcoord0 = i.texcoord0;
            #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                d.texcoord1 = i.texcoord1;
               // d.texcoord2 = i.texcoord2;
            #endif
            // d.texcoord3 = i.texcoord3;
            // d.vertexColor = i.vertexColor;

            // these rarely get used, so we back transform them. Usually will be stripped.
            #if _HDRP
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(GetCameraRelativePositionWS(i.worldPos), 1));
            #else
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(i.worldPos, 1));
            #endif
            // d.localSpaceNormal = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldNormal));
            // d.localSpaceTangent = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldTangent.xyz));

            // d.screenPos = i.screenPos;
            // d.screenUV = i.screenPos.xy / i.screenPos.w;

            // d.extraV2F0 = i.extraV2F0;
            // d.extraV2F1 = i.extraV2F1;
            // d.extraV2F2 = i.extraV2F2;
            // d.extraV2F3 = i.extraV2F3;
            // d.extraV2F4 = i.extraV2F4;
            // d.extraV2F5 = i.extraV2F5;
            // d.extraV2F6 = i.extraV2F6;
            // d.extraV2F7 = i.extraV2F7;

            return d;
         }
         // CHAINS

            void ChainModifyVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               ModifyVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }

            void ChainModifyTessellatedVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               // d.extraV2F0 = v2p.extraV2F0;
               // d.extraV2F1 = v2p.extraV2F1;
               // d.extraV2F2 = v2p.extraV2F2;
               // d.extraV2F3 = v2p.extraV2F3;
               // d.extraV2F4 = v2p.extraV2F4;
               // d.extraV2F5 = v2p.extraV2F5;
               // d.extraV2F6 = v2p.extraV2F6;
               // d.extraV2F7 = v2p.extraV2F7;

               ModifyTessellatedVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }


            void ChainFinalColorForward(inout Surface l, inout ShaderData d, inout half4 color)
            {
               
            }

            void ChainFinalGBufferStandard(inout Surface s, inout ShaderData d, inout half4 GBuffer0, inout half4 GBuffer1, inout half4 GBuffer2, inout half4 outEmission, inout half4 outShadowMask)
            {
               
            }


            
         #if _PASSSHADOW
            float3 _LightDirection;
            float3 _LightPosition;
         #endif

         // vertex shader
         VertexToPixel Vert (VertexData v)
         {
           
           VertexToPixel o = (VertexToPixel)0;

           UNITY_SETUP_INSTANCE_ID(v);
           UNITY_TRANSFER_INSTANCE_ID(v, o);
           UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


#if !_TESSELLATION_ON
           ChainModifyVertex(v, o);
#endif

            o.texcoord0 = v.texcoord0;

           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               o.texcoord1 = v.texcoord1;
              // o.texcoord2 = v.texcoord2;
           #endif

           // o.texcoord3 = v.texcoord3;
           // o.vertexColor = v.vertexColor;
           
           VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
           o.worldPos = TransformObjectToWorld(v.vertex.xyz);
           o.worldNormal = TransformObjectToWorldNormal(v.normal);

           
           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float2 uv1 = v.texcoord1.xy;
               float2 uv2 = v.texcoord2.xy;
               o.worldTangent = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);
           #else
               float2 uv1 = v.texcoord0.xy;
               float2 uv2 = uv1;
           #endif

           // MS Only
           ApplyTerrainTangent(o);
           

          #if _PASSSHADOW
              #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                 float3 lightDirectionWS = normalize(_LightPosition - o.worldPos);
              #else
                 float3 lightDirectionWS = _LightDirection;
              #endif
              // Define shadow pass specific clip position for Universal
              o.pos = TransformWorldToHClip(ApplyShadowBias(o.worldPos, o.worldNormal, lightDirectionWS));
              #if UNITY_REVERSED_Z
                  o.pos.z = min(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #else
                  o.pos.z = max(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #endif
          #elif _PASSMETA
              #if _MICROTERRAIN
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), v.texcoord0.xy, v.texcoord0.xy, unity_LightmapST, unity_DynamicLightmapST);
              #else
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), uv1, uv2, unity_LightmapST, unity_DynamicLightmapST);
              #endif
          #else
              o.pos = TransformWorldToHClip(o.worldPos);
          #endif



          // o.screenPos = ComputeScreenPos(o.pos, _ProjectionParams.x);
          

          #if _PASSFORWARD || _PASSGBUFFER
              #if _MICROTERRAIN
                 OUTPUT_LIGHTMAP_UV(v.texcoord0.xy, unity_LightmapST, o.lightmapUV);
              #else
                 OUTPUT_LIGHTMAP_UV(uv1, unity_LightmapST, o.lightmapUV);
              #endif
              OUTPUT_SH(o.worldNormal, o.sh);
              #if defined(DYNAMICLIGHTMAP_ON)
                   o.dynamicLightmapUV.xy = uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
              #endif
          #endif

          #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
              half fogFactor = 0;
              #if defined(_FOG_FRAGMENT)
                fogFactor = ComputeFogFactor(o.pos.z);
              #endif
              #if _BAKEDLIT
                  half3 vertexLight = VertexLighting(o.worldPos, o.worldNormal);
                  o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
              #else
                  o.fogFactorAndVertexLight = half4(fogFactor, 0,0,0);
              #endif
          #endif

          #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             o.shadowCoord = GetShadowCoord(vertexInput);
          #endif

           return o;
         }


            

            // fragment shader
            half4 Frag (VertexToPixel IN
            #ifdef _DEPTHOFFSET_ON
              , out float outputDepth : SV_Depth
            #endif
            ) : SV_Target
            {
               UNITY_SETUP_INSTANCE_ID(IN);
               UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

               ShaderData d = CreateShaderData(IN);
               Surface l = (Surface)0;

               #ifdef _DEPTHOFFSET_ON
                  l.outputDepth = outputDepth;
               #endif

               l.Albedo = half3(0.5, 0.5, 0.5);
               l.Normal = float3(0,0,1);
               l.Occlusion = 1;
               l.Alpha = 1;

               SurfaceFunction(l, d);

               #ifdef _DEPTHOFFSET_ON
                  outputDepth = l.outputDepth;
               #endif

               return 0;

            }

         ENDHLSL

      }


      
        Pass
        {
            Name "Meta"
            Tags 
            { 
                "LightMode" = "Meta"
            }

             // Render State
            Cull Off

            

            HLSLPROGRAM

               #pragma vertex Vert
   #pragma fragment Frag

            #pragma target 3.5

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_local _ _ALPHATEST_ON
        
            #define SHADERPASS SHADERPASS_META
            #define _PASSMETA 1

            
      #define _MICROSPLAT 1
      #define _MICROTERRAIN 1
      #define _USEGRADMIP 1
      #define _MAX4TEXTURES 1
      #define _BRANCHSAMPLES 1
      #define _BRANCHSAMPLESAGR 1
      #define _MSRENDERLOOP_UNITYURP2022 1

#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


   #define _URP 1




            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        

                  #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, float3x3(d.worldSpaceTangent, cross(d.worldSpaceTangent, d.worldSpaceNormal), d.worldSpaceNormal))

      
      #define UnityObjectToWorldNormal(normal) mul(GetObjectToWorldMatrix(), normal)

      #define _WorldSpaceLightPos0 _MainLightPosition
      
      #define UNITY_DECLARE_TEX2D(name) TEXTURE2D(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2D_NOSAMPLER(name) TEXTURE2D(name);
      #define UNITY_DECLARE_TEX2DARRAY(name) TEXTURE2D_ARRAY(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(name) TEXTURE2D_ARRAY(name);

      #define UNITY_SAMPLE_TEX2DARRAY(tex,coord)            SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, coord.xy, coord.z)
      #define UNITY_SAMPLE_TEX2DARRAY_LOD(tex,coord,lod)    SAMPLE_TEXTURE2D_ARRAY_LOD(tex, sampler##tex, coord.xy, coord.z, lod)
      #define UNITY_SAMPLE_TEX2D(tex, coord)                SAMPLE_TEXTURE2D(tex, sampler##tex, coord)
      #define UNITY_SAMPLE_TEX2D_SAMPLER(tex, samp, coord)  SAMPLE_TEXTURE2D(tex, sampler##samp, coord)

      #if defined(UNITY_COMPILER_HLSL)
         #define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
      #else
         #define UNITY_INITIALIZE_OUTPUT(type,name)
      #endif

      #define sampler2D_float sampler2D
      #define sampler2D_half sampler2D

      

      // data across stages, stripped like the above.
      struct VertexToPixel
      {
         float4 pos : SV_POSITION;
         float3 worldPos : TEXCOORD0;
         float3 worldNormal : TEXCOORD1;
         float4 worldTangent : TEXCOORD2;
          float4 texcoord0 : TEXCCOORD3;
         #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
             float4 texcoord1 : TEXCCOORD4;
            // float4 texcoord2 : TEXCCOORD5;
         #endif
         // float4 texcoord3 : TEXCCOORD6;
         // float4 screenPos : TEXCOORD7;
         // float4 vertexColor : COLOR;

         // float4 extraV2F0 : TEXCOORD13;
         // float4 extraV2F1 : TEXCOORD14;
         // float4 extraV2F2 : TEXCOORD15;
         // float4 extraV2F3 : TEXCOORD16;
         // float4 extraV2F4 : TEXCOORD17;
         // float4 extraV2F5 : TEXCOORD18;
         // float4 extraV2F6 : TEXCOORD19;
         // float4 extraV2F7 : TEXCOORD20;

         
         #if defined(LIGHTMAP_ON)
            float2 lightmapUV : TEXCOORD8;
         #endif
         #if defined(DYNAMICLIGHTMAP_ON)
            float2 dynamicLightmapUV : TEXCOORD9;
         #endif
         #if !defined(LIGHTMAP_ON)
            float3 sh : TEXCOORD10;
         #endif

         float4 fogFactorAndVertexLight : TEXCOORD11;
         float4 shadowCoord : TEXCOORD12;

         #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
         #endif
         #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
         #endif
         #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
         #endif
         #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
         #endif
      };


            // TEMPLATE_SHARED
            
            // data describing the user output of a pixel
            struct Surface
            {
               half3 Albedo;
               half Height;
               half3 Normal;
               half Smoothness;
               half3 Emission;
               half Metallic;
               half3 Specular;
               half Occlusion;
               half Alpha;
               // HDRP Only
               half SpecularOcclusion;
               half SubsurfaceMask;
               half Thickness;
               half CoatMask;
               half Anisotropy;
               half IridescenceMask;
               half IridescenceThickness;
            };

            // data the user might need, this will grow to be big. But easy to strip
            struct ShaderData
            {
               float3 localSpacePosition;
               float3 localSpaceNormal;
               float3 localSpaceTangent;
        
               float3 worldSpacePosition;
               float3 worldSpaceNormal;
               float3 worldSpaceTangent;

               float3 worldSpaceViewDir;
               float3 tangentSpaceViewDir;

               float4 texcoord0;
               float4 texcoord1;
               float4 texcoord2;
               float4 texcoord3;

               float2 screenUV;
               float4 screenPos;

               float4 vertexColor;

               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;

               float3x3 TBNMatrix;
            };

            struct VertexData
            {
               #if SHADER_TARGET > 30 && _PLANETCOMPUTE
 //              // uint vertexID : SV_VertexID;
               #endif
               float4 vertex : POSITION;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                  float4 tangent : TANGENT;
                  float4 texcoord1 : TEXCOORD1;
                  float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD4; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity    : TEXCOORD5; // Add Precomputed Velocity (Alembic computes velocities on runtime side).
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessVertex 
            {
               float4 vertex : INTERNALTESSPOS;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float4 tangent : TANGENT;
               float4 texcoord1 : TEXCOORD1;
               float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               // float4 extraV2F0 : TEXCOORD4;
               // float4 extraV2F1 : TEXCOORD5;
               // float4 extraV2F2 : TEXCOORD6;
               // float4 extraV2F3 : TEXCOORD7;
               // float4 extraV2F4 : TEXCOORD8;
               // float4 extraV2F5 : TEXCOORD9;
               // float4 extraV2F6 : TEXCOORD10;
               // float4 extraV2F7 : TEXCOORD11;

               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD12; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity : TEXCOORD13;
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
               UNITY_VERTEX_OUTPUT_STEREO
            };

            struct ExtraV2F
            {
               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;
            };


            float3 WorldToTangentSpace(ShaderData d, float3 normal)
            {
               return mul(d.TBNMatrix, normal);
            }

            float3 TangentToWorldSpace(ShaderData d, float3 normal)
            {
               return mul(normal, d.TBNMatrix);
            }

            // in this case, make standard more like SRPs, because we can't fix
            // GetWorldToObjectMatrix() in HDRP, since it already does macro-fu there

            #if _STANDARD
               float3 TransformWorldToObject(float3 p) { return mul(GetWorldToObjectMatrix(), float4(p, 1)); };
               float3 TransformObjectToWorld(float3 p) { return mul(GetObjectToWorldMatrix(), float4(p, 1)); };
               float4 TransformWorldToObject(float4 p) { return mul(GetWorldToObjectMatrix(), p); };
               float4 TransformObjectToWorld(float4 p) { return mul(GetObjectToWorldMatrix(), p); };
               float4x4 GetWorldToObjectMatrix() { return GetWorldToObjectMatrix(); }
               float4x4 GetObjectToWorldMatrix() { return GetObjectToWorldMatrix(); }
            #endif

            float3 GetCameraWorldPosition()
            {
               #if _HDRP
                  return GetCameraRelativePositionWS(_WorldSpaceCameraPos);
               #else
                  return _WorldSpaceCameraPos;
               #endif
            }

            
             #if _HDRP

               half3 UnpackNormalmapRGorAG(half4 packednormal)
               {
                     // This do the trick
                  packednormal.x *= packednormal.w;

                  half3 normal;
                  normal.xy = packednormal.xy * 2 - 1;
                  normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                  return normal;
               }
               half3 UnpackNormal(half4 packednormal)
               {
                  #if defined(UNITY_NO_DXT5nm)
                     return packednormal.xyz * 2 - 1;
                  #else
                     return UnpackNormalmapRGorAG(packednormal);
                  #endif
               }
               #endif
               #if _HDRP || _URP

               half3 UnpackScaleNormal(half4 packednormal, half scale)
               {
                 #ifndef UNITY_NO_DXT5nm
                   // Unpack normal as DXT5nm (1, y, 1, x) or BC5 (x, y, 0, 1)
                   // Note neutral texture like "bump" is (0, 0, 1, 1) to work with both plain RGB normal and DXT5nm/BC5
                   packednormal.x *= packednormal.w;
                 #endif
                   half3 normal;
                   normal.xy = (packednormal.xy * 2 - 1) * scale;
                   normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                   return normal;
               }  

             #endif


            void GetSun(out float3 lightDir, out float3 color)
            {
               lightDir = float3(0.5, 0.5, 0);
               color = 1;
               #if _HDRP
                  if (_DirectionalLightCount > 0)
                  {
                     DirectionalLightData light = _DirectionalLightDatas[0];
                     lightDir = -light.forward.xyz;
                     color = light.color;
                  }
               #elif _STANDARD
                  lightDir = normalize(_WorldSpaceLightPos0.xyz);
                  color = _LightColor0.rgb;
               #elif _URP
                  Light light = GetMainLight();
                  lightDir = light.direction;
                  color = light.color;
               #endif
            }

     



            
            CBUFFER_START(UnityPerMaterial)

               

      #if _MESHSUBARRAY
         half4 _MeshSubArrayIndexes;
      #endif

      float4 _Diffuse_TexelSize;
      float4 _NormalSAO_TexelSize;

      #if _HYBRIDHEIGHTBLEND
         float _HybridHeightBlendDistance;
      #endif

      #if _PACKINGHQ
         float4 _SmoothAO_TexelSize;
      #endif

      #ifdef _ALPHATEST_ON
      float4 _TerrainHolesTexture_TexelSize;
      #endif

      #if _USESPECULARWORKFLOW
         float4 _Specular_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         float4 _EmissiveMetal_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         half _EmissiveMult;
      #endif

      #if _AUTONORMAL
         half _AutoNormalHeightScale;
      #endif

      float4 _UVScale; // scale and offset

      half _Contrast;
      
      

       #if _VSSHADOWMAP
         float4 gVSSunDirection;
      #endif

      #if _FORCELOCALSPACE && _PLANETVECTORS
         float4x4 _PQSToLocal;
      #endif

      #if _ORIGINSHIFT
         float4x4 _GlobalOriginMTX;
      #endif

      float4 _Control0_TexelSize;
      #if _CUSTOMSPLATTEXTURES
         float4 _CustomControl0_TexelSize;
      #endif
      float4 _PerPixelNormal_TexelSize;

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         float2 _NoiseUVParams;
      #endif

      float4 _PerTexProps_TexelSize;

      #if _SURFACENORMALS  
         float3 surfTangent;
         float3 surfBitangent;
         float3 surfNormal;
      #endif



            CBUFFER_END

            

            

                  

      #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, data.TBN)
      

      



   
// In Unity 2020.3LTS, Unity will spew tons of errors about missing this sampler in
// URP, even though it shouldn't be required.
TEXTURE2D(_MainTex);

      // globals, outside of CBuffer, but used by more than one module
      float3 _gGlitterLightDir;
      float3 _gGlitterLightWorldPos;
      half3 _gGlitterLightColor;

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
         float4    _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
         float4    _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
         float4    _TerrainNormalmapTexture_TexelSize;
      #endif

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
          TEXTURE2D(_TerrainHeightmapTexture);
          TEXTURE2D(_TerrainNormalmapTexture);
      #endif

      UNITY_INSTANCING_BUFFER_START(Terrain)
          UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
      UNITY_INSTANCING_BUFFER_END(Terrain)          


      

      // dynamic branching helpers, for regular and aggressive branching
      // debug mode shows how many samples using branching will save us. 
      //
      // These macros are always used instead of the UNITY_BRANCH macro
      // to maintain debug displays and allow branching to be disabled
      // on as granular level as we want. 
      
      #if _BRANCHSAMPLES
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++; if (w > 0)
         #else
            #define MSBRANCH(w) UNITY_BRANCH if (w > 0)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++;
         #else
            #define MSBRANCH(w) 
         #endif
      #endif
      
      #if _BRANCHSAMPLESAGR
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER ||_DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++; if (w > 0.001)
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++; if (w > 0.001)
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++; if (w > 0.001)
         #else
            #define MSBRANCHTRIPLANAR(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHCLUSTER(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHOTHER(w) UNITY_BRANCH if (w > 0.001)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++;
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++;
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++;
         #else
            #define MSBRANCHTRIPLANAR(w)
            #define MSBRANCHCLUSTER(w)
            #define MSBRANCHOTHER(w)
         #endif
      #endif

      #if _DEBUG_SAMPLECOUNT
         int _sampleCount;
         #define COUNTSAMPLE { _sampleCount++; }
      #else
         #define COUNTSAMPLE
      #endif

      #if _DEBUG_PROCLAYERS
         int _procLayerCount;
         #define COUNTPROCLAYER { _procLayerCount++; }
      #else
         #define COUNTPROCLAYER
      #endif


      #if _DEBUG_USE_TOPOLOGY
         TEXTURE2D(_DebugWorldPos);
         TEXTURE2D(_DebugWorldNormal);
      #endif
      

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         TEXTURE2D(_NoiseUV);
      #endif

      #if _PACKINGHQ
         UNITY_DECLARE_TEX2DARRAY(_SmoothAO);
      #endif

      #if _USESPECULARWORKFLOW
         UNITY_DECLARE_TEX2DARRAY(_Specular);
      #endif

      #if _USEEMISSIVEMETAL
         UNITY_DECLARE_TEX2DARRAY(_EmissiveMetal);
      #endif

      TEXTURE2D(_PerPixelNormal);

      SamplerState shared_linear_clamp_sampler;
      SamplerState shared_point_clamp_sampler;
      
      TEXTURE2D(_Control0);
      #if _CUSTOMSPLATTEXTURES
         TEXTURE2D(_CustomControl0);
         #if !_MAX4TEXTURES
         TEXTURE2D(_CustomControl1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_CustomControl2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_CustomControl3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_CustomControl7);
         #endif
      #else
         #if !_MAX4TEXTURES
         TEXTURE2D(_Control1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_Control2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_Control3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_Control7);
         #endif
      #endif

      TEXTURE2D_FLOAT(_PerTexProps);
   
      struct DecalLayer
      {
         float3 uv;
         float2 dx;
         float2 dy;
         int decalIndex;
         bool dynamic; 
      };

      struct DecalOutput
      {
         DecalLayer l0;
         DecalLayer l1;
         DecalLayer l2;
         DecalLayer l3;
         
         half4 Weights;
         half4 Indexes;
         half4 fxLevels;
         
      };
      

      struct TriGradMipFormat
      {
         float4 d0;
         float4 d1;
         float4 d2;
      };

      float InverseLerp(float x, float y, float v) { return (v-x)/max(y-x, 0.001); }
      float2 InverseLerp(float2 x, float2 y, float2 v) { return (v-x)/max(y-x, float2(0.001, 0.001)); }
      float3 InverseLerp(float3 x, float3 y, float3 v) { return (v-x)/max(y-x, float3(0.001, 0.001, 0.001)); }
      float4 InverseLerp(float4 x, float4 y, float4 v) { return (v-x)/max(y-x, float4(0.001, 0.001, 0.001, 0.001)); }
      

      // 2019.3 holes
      #ifdef _ALPHATEST_ON
          TEXTURE2D(_TerrainHolesTexture);
          
          void ClipHoles(float2 uv)
          {
              float hole = SAMPLE_TEXTURE2D(_TerrainHolesTexture, shared_linear_clamp_sampler, uv).r;
              COUNTSAMPLE
              clip(hole < 0.5f ? -1 : 1);
          }
      #endif

      
      #if _TRIPLANAR
         #if _USEGRADMIP
            #define MIPFORMAT TriGradMipFormat
            #define INITMIPFORMAT (TriGradMipFormat)0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float3
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float3
         #endif
      #else
         #if _USEGRADMIP
            #define MIPFORMAT float4
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float
         #endif
      #endif

      float2 TotalOne(float2 v) { return v * (1.0 / max(v.x + v.y, 0.001)); }
      float3 TotalOne(float3 v) { return v * (1.0 / max(v.x + v.y + v.z, 0.001)); }
      float4 TotalOne(float4 v) { return v * (1.0 / max(v.x + v.y + v.z + v.w, 0.001)); }

      float2 RotateUV(float2 uv, float amt)
      {
         uv -=0.5;
         float s = sin ( amt);
         float c = cos ( amt );
         float2x2 mtx = float2x2( c, -s, s, c);
         mtx *= 0.5;
         mtx += 0.5;
         mtx = mtx * 2-1;
         uv = mul ( uv, mtx );
         uv += 0.5;
         return uv;
      }

      float4 DecodeToFloat4(float v)
      {
         uint vi = (uint)(v * (256.0f * 256.0f * 256.0f * 256.0f));
         int ex = (int)(vi / (256 * 256 * 256) % 256);
         int ey = (int)((vi / (256 * 256)) % 256);
         int ez = (int)((vi / (256)) % 256);
         int ew = (int)(vi % 256);
         float4 e = float4(ex / 255.0, ey / 255.0, ez / 255.0, ew / 255.0);
         return e;
      }

      

      struct Input 
      {
         ShaderData shaderData;
         float2 uv_Control0;
         float2 uv2_Diffuse;

         float worldHeight;
         float3 worldUpVector;

         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         float4 color;
         float3x3 TBN;

         // vertex/digger workflow data
         half4 w0;
         half4 w1;
         half4 w2;
         half4 w3;
         half4 w4;
         half4 w5;
         half4 w6;
         
         // megasplat data
         half4 layer0;
         half4 layer1;
         half3 baryWeights;
         half4 scatter0;
         half4 scatter1;

         // wetness, puddles, streams, lava from vertex or megasplat
         half4 fx;
         // snow min, snow max
         half4 fx2;


      };
      
      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
         half3 pN0;
         half3 pN1;
         half3 pN2;
         half3 pN3;
         half3 axisSign;
         Input IN;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;

         half4 cluster0;
         half4 cluster1;
         half4 cluster2;
         half4 cluster3;

      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
         #if _USESPECULARWORKFLOW
         half3 Specular;
         #endif
         half Alpha;
         
      };


      

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         #if _SURFACENORMALS
            half3 surf0;
            half3 surf1;
            half3 surf2;
            half3 surf3;
         #endif

         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
         

         #if _USEEMISSIVEMETAL || _GLOBALEMIS || _GLOBALSMOOTHAOMETAL || _PERTEXSSS || _PERTEXRIMLIGHT
            half4 emisMetal0;
            half4 emisMetal1;
            half4 emisMetal2;
            half4 emisMetal3;
         #endif

         #if _USESPECULARWORKFLOW
            half3 specular0;
            half3 specular1;
            half3 specular2;
            half3 specular3;
         #endif
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
         #if _SURFACENORMALS
            s.surf0 = half3(0,0,1);
            s.surf1 = half3(0,0,1);
            s.surf2 = half3(0,0,1);
            s.surf3 = half3(0,0,1);
         #endif
      }

       float3 GetGlobalLightDir(Input i)
      {
         float3 lightDir = float3(1,0,0);

         #if _HDRP || PASS_DEFERRED
            lightDir = normalize(_gGlitterLightDir.xyz);
         #elif _URP
            lightDir = GetMainLight().direction;
         #else
            #ifndef USING_DIRECTIONAL_LIGHT
               lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            #else
               lightDir = normalize(_WorldSpaceLightPos0.xyz);
            #endif
         #endif
         return lightDir;
      }

      float3x3 GetTBN(Input i)
      {
         return i.TBN;
      }
      
      float3 GetGlobalLightDirTS(Input i)
      {
         float3 lightDirWS = GetGlobalLightDir(i);
         return mul(GetTBN(i), lightDirWS);
      }
      
      half3 GetGlobalLightColor()
      {
         #if _HDRP || PASS_DEFERRED
            return _gGlitterLightColor;
         #elif _URP
            return (GetMainLight().color);
         #else
            return _LightColor0.rgb;
         #endif
      }

      

      half3 FuzzyShade(half3 color, half3 normal, half coreMult, half edgeMult, half power, float3 viewDir)
      {
         half dt = saturate(dot(viewDir, normal));
         half dark = 1.0 - (coreMult * dt);
         half edge = pow(1-dt, power) * edgeMult;
         return color * (dark + edge);
      }

      half3 ComputeSSS(Input i, float3 V, float3 N, half3 tint, half thickness, half distortion, half scale, half power)
      {
         float3 L = GetGlobalLightDir(i);
         half3 lightColor = GetGlobalLightColor();
         float3 H = normalize(L + N * distortion);
         float VdotH = pow(saturate(dot(V, -H)), power) * scale;
         float3 I =  (VdotH) * thickness;
         return lightColor * I * tint;
      }


      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif
      

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##3 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv3.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #endif

      half2 BlendNormal2(half2 base, half2 blend) { return normalize(half3(base.xy + blend.xy, 1)).xy; } 
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }
      half3 BlendLighterColor(half3 s, half3 d) { return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d; } 
      
      
      #if _SURFACENORMALS  

      #define HALF_EPS 4.8828125e-4    // 2^-11, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)

      

      void ConstructSurfaceGradientTBN(Input i)
      {
         float3x3 tbn = GetTBN(i);
         float3 t = tbn[0];
         float3 b = tbn[1];
         float3 n = tbn[2];

         surfNormal = n;//mul(GetWorldToObjectMatrix(), float4(n, 1)).xyz;
         surfTangent = t;//mul(GetWorldToObjectMatrix(), float4(t, 1)).xyz;
         surfBitangent = b;//cross(surfNormal, surfTangent);
         
         float renormFactor = 1.0 / length(surfNormal);
         surfNormal    *= renormFactor;
         surfTangent   *= renormFactor;
         surfBitangent *= renormFactor;
      }
      
      half3 SurfaceGradientFromTBN(half2 deriv)
      {
          return deriv.x * surfTangent + deriv.y * surfBitangent;
      }

      // Input: vM is tangent space normal in [-1;1].
      // Output: convert vM to a derivative.
      half2 TspaceNormalToDerivative(half3 vM)
      {
         const half scale = 1.0/128.0;
         
         // Ensure vM delivers a positive third component using abs() and
         // constrain vM.z so the range of the derivative is [-128; 128].
         const half3 vMa = abs(vM);
         const half z_ma = max(vMa.z, scale*max(vMa.x, vMa.y));

         return -half2(vM.x, vM.y)/z_ma;
      }

      // Used to produce a surface gradient from the gradient of a volume
      // bump function such as 3D Perlin noise. Equation 2 in [Mik10].
      half3 SurfgradFromVolumeGradient(half3 grad)
      {
         return grad - dot(surfNormal, grad) * surfNormal;
      }

      half3 SurfgradFromTriplanarProjection(half3 pN, half2 xPlaneTN, half2 yPlaneTN, half2 zPlaneTN)
      {
         const half w0 = pN.x;
         const half w1 = pN.y;
         const half w2 = pN.z;
         
         // X-plane tangent normal to gradient derivative
         xPlaneTN = xPlaneTN * 2.0 - 1.0;
         half xPlaneRcpZ = rsqrt(max(1 - dot(xPlaneTN.x, xPlaneTN.x) - dot(xPlaneTN.y, xPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_xplane = xPlaneTN * -xPlaneRcpZ;

         // Y-plane tangent normal to gradient derivative
         yPlaneTN = yPlaneTN * 2.0 - 1.0;
         half yPlaneRcpZ = rsqrt(max(1 - dot(yPlaneTN.x, yPlaneTN.x) - dot(yPlaneTN.y, yPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_yplane = yPlaneTN * -yPlaneRcpZ;

         // Z-plane tangent normal to gradient derivative
         zPlaneTN = zPlaneTN * 2.0 - 1.0;
         half zPlaneRcpZ = rsqrt(max(1 - dot(zPlaneTN.x, zPlaneTN.x) - dot(zPlaneTN.y, zPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_zplane = zPlaneTN * -zPlaneRcpZ;

         // Assume deriv xplane, deriv yplane, and deriv zplane are
         // sampled using (z,y), (x,z), and (x,y), respectively.
         // Positive scales of the lookup coordinate will work
         // as well, but for negative scales the derivative components
         // will need to be negated accordingly.
         float3 grad = float3(w2*d_zplane.x + w1*d_yplane.x,
                              w2*d_zplane.y + w0*d_xplane.y,
                              w0*d_xplane.x + w1*d_yplane.y);

         return SurfgradFromVolumeGradient(grad);
      }

      half3 ConvertNormalToGradient(half3 normal)
      {
         half2 deriv = TspaceNormalToDerivative(normal);

         return SurfaceGradientFromTBN(deriv);
      }

      half3 ConvertNormal2ToGradient(half2 packedNormal)
      {
         half2 tNormal = packedNormal;
         half rcpZ = rsqrt(max(1 - dot(tNormal.x, tNormal.x) - dot(tNormal.y, tNormal.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
         half2 deriv = tNormal * -rcpZ;
         return SurfaceGradientFromTBN(deriv);
      }


      half3 ResolveNormalFromSurfaceGradient(half3 gradient)
      {
         return normalize(surfNormal - gradient);
      }
      

      #endif // _SURFACENORMALS

      void BlendNormalPerTex(inout RawSamples o, half2 noise, float4 fades)
      {
         #if _SURFACENORMALS
            float3 grad = ConvertNormal2ToGradient(noise.xy);
            o.surf0 += grad * fades.x;
            o.surf1 += grad * fades.y;
            #if !_MAX2LAYER
               o.surf2 += grad * fades.z;
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.surf3 += grad * fades.w;
            #endif
         #else
            o.normSAO0.xy = lerp(o.normSAO0.xy, BlendNormal2(o.normSAO0.xy, noise.xy), fades.x);
            o.normSAO1.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #if !_MAX2LAYER
               o.normSAO2.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO2.xy, noise.xy), fades.y);
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.normSAO3.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #endif
         #endif
      }
      
     
      
      half3 BlendNormal3(half3 n1, half3 n2)
      {
         n1 += float3( 0,  0, 1);
         n2 *= float3(-1, -1, 1);
         return n1*dot(n1, n2) / n1.z - n2;
      }
      
      half2 TransformTriplanarNormal(Input IN, float3x3 t2w, half3 axisSign, half3 absVertNormal,
               half3 pN, half2 a0, half2 a1, half2 a2)
      {
         
         a0 = a0 * 2 - 1;
         a1 = a1 * 2 - 1;
         a2 = a2 * 2 - 1;

         a0.x *= axisSign.x;
         a1.x *= axisSign.y;
         a2.x *= axisSign.z;
         
         half3 n0 = half3(a0.xy, 1);
         half3 n1 = half3(a1.xy, 1);
         half3 n2 = half3(a2.xy, 1);

         float3 wn = IN.worldNormal;

         n0 = BlendNormal3(half3(wn.zy, absVertNormal.x), n0);
         n1 = BlendNormal3(half3(wn.xz, absVertNormal.y), n1 * float3(-1, 1, 1)); 
         n2 = BlendNormal3(half3(wn.xy, absVertNormal.z), n2);
  
         n0.z *= axisSign.x;
         n1.z *= axisSign.y;
         n2.z *= -axisSign.z;

         half3 worldNormal = (n0.zyx * pN.x + n1.xzy * pN.y + n2.xyz * pN.z);
         return mul(t2w, worldNormal).xy;

      }
      
      // funcs
      
      inline half MSLuminance(half3 rgb)
      {
         #ifdef UNITY_COLORSPACE_GAMMA
            return dot(rgb, half3(0.22, 0.707, 0.071));
         #else
            return dot(rgb, half3(0.0396819152, 0.458021790, 0.00609653955));
         #endif
      }
      
      
      float2 Hash2D( float2 x )
      {
          float2 k = float2( 0.3183099, 0.3678794 );
          x = x*k + k.yx;
          return -1.0 + 2.0*frac( 16.0 * k*frac( x.x*x.y*(x.x+x.y)) );
      }

      float Noise2D(float2 p )
      {
         float2 i = floor( p );
         float2 f = frac( p );
         
         float2 u = f*f*(3.0-2.0*f);

         return lerp( lerp( dot( Hash2D( i + float2(0.0,0.0) ), f - float2(0.0,0.0) ), 
                           dot( Hash2D( i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                      lerp( dot( Hash2D( i + float2(0.0,1.0) ), f - float2(0.0,1.0) ), 
                           dot( Hash2D( i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
      }
      
      float FBM2D(float2 uv)
      {
         float f = 0.5000*Noise2D( uv ); uv *= 2.01;
         f += 0.2500*Noise2D( uv ); uv *= 1.96;
         f += 0.1250*Noise2D( uv ); 
         return f;
      }
      
      float3 Hash3D( float3 p )
      {
         p = float3( dot(p,float3(127.1,311.7, 74.7)),
                 dot(p,float3(269.5,183.3,246.1)),
                 dot(p,float3(113.5,271.9,124.6)));

         return -1.0 + 2.0*frac(sin(p)*437.5453123);
      }

      float Noise3D( float3 p )
      {
         float3 i = floor( p );
         float3 f = frac( p );
         
         float3 u = f*f*(3.0-2.0*f);

         return lerp( lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,0.0) ), f - float3(0.0,0.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,0.0) ), f - float3(1.0,0.0,0.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,0.0) ), f - float3(0.0,1.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,0.0) ), f - float3(1.0,1.0,0.0) ), u.x), u.y),
                      lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,1.0) ), f - float3(0.0,0.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,1.0) ), f - float3(1.0,0.0,1.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,1.0) ), f - float3(0.0,1.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,1.0) ), f - float3(1.0,1.0,1.0) ), u.x), u.y), u.z );
      }
      
      float FBM3D(float3 uv)
      {
         float f = 0.5000*Noise3D( uv ); uv *= 2.01;
         f += 0.2500*Noise3D( uv ); uv *= 1.96;
         f += 0.1250*Noise3D( uv ); 
         return f;
      }
      
     
      
      float GetSaturation(float3 c)
      {
         float mi = min(min(c.x, c.y), c.z);
         float ma = max(max(c.x, c.y), c.z);
         return (ma - mi)/(ma + 1e-7);
      }

      // Better Color Lerp, does not have darkening issue
      float3 BetterColorLerp(float3 a, float3 b, float x)
      {
         float3 ic = lerp(a, b, x) + float3(1e-6,0.0,0.0);
         float sd = abs(GetSaturation(ic) - lerp(GetSaturation(a), GetSaturation(b), x));
    
         float3 dir = normalize(float3(2.0 * ic.x - ic.y - ic.z, 2.0 * ic.y - ic.x - ic.z, 2.0 * ic.z - ic.y - ic.x));
         float lgt = dot(float3(1.0, 1.0, 1.0), ic);
    
         float ff = dot(dir, normalize(ic));
    
         const float dsp_str = 1.5;
         ic += dsp_str * dir * sd * ff * lgt;
         return saturate(ic);
      }
      
      
      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);
             half4 weights = half4(iWeights.x * max(h0,0.001), iWeights.y * max(h1,0.001), iWeights.z * max(h2,0.001), iWeights.w * max(h3,0.001));
             
             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);

             weights = TotalOne(weights);
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #elif _MAX20TEXTURES
         #define TEXCOUNT 20
      #elif _MAX24TEXTURES
         #define TEXCOUNT 24
      #elif _MAX28TEXTURES
         #define TEXCOUNT 28
      #elif _MAX32TEXTURES
         #define TEXCOUNT 32
      #else
         #define TEXCOUNT 16
      #endif

      #if _DECAL_SPLAT
      
      void DoMergeDecalSplats(half4 iWeights, half4 iIndexes, inout half4 indexes, inout half4 weights)
      {
         for (int i = 0; i < 4; ++i)
         {
            half w = iWeights[i];
            half index = iIndexes[i];
            if (w > weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = index;
            }
            else if (w > weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = index;
            }
            else if (w > weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = index;
            }
            else if (w > weights[3])
            {
               weights[3] = w;
               indexes[3] = index;
            }
         }

      }
      #endif


      void Setup(out half4 weights, float2 uv, out Config config, half4 w0, half4 w1, half4 w2, half4 w3, half4 w4, half4 w5, half4 w6, half4 w7, float3 worldPos, DecalOutput decalOutput)
      {
         config = (Config)0;
         half4 indexes = 0;

         config.uv = uv;

         #if _WORLDUV
         uv = worldPos.xz;
         #endif

         #if _DISABLESPLATMAPS
            float2 scaledUV = uv;
         #else
            float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;
         #endif

         // if only 4 textures, and blending 4 textures, skip this whole thing..
         // this saves about 25% of the ALU of the base shader on low end. However if
         // we rely on sorted texture weights (distance resampling) we have to sort..
         float4 defaultIndexes = float4(0,1,2,3);
         #if _MESHSUBARRAY
            defaultIndexes = _MeshSubArrayIndexes;
         #endif

         #if _MESHSUBARRAY && !_DECAL_SPLAT || (_MAX4TEXTURES && !_MAX3LAYER && !_MAX2LAYER && !_DISTANCERESAMPLE && !_POM && !_DECAL_SPLAT)
            weights = w0;
            config.uv0 = float3(scaledUV, defaultIndexes.x);
            config.uv1 = float3(scaledUV, defaultIndexes.y);
            config.uv2 = float3(scaledUV, defaultIndexes.z);
            config.uv3 = float3(scaledUV, defaultIndexes.w);
            return;
         #endif

         #if _DISABLESPLATMAPS
            weights = float4(1,0,0,0);
            return;
         #else
            half splats[TEXCOUNT];

            splats[0] = w0.x;
            splats[1] = w0.y;
            splats[2] = w0.z;
            splats[3] = w0.w;
            #if !_MAX4TEXTURES
               splats[4] = w1.x;
               splats[5] = w1.y;
               splats[6] = w1.z;
               splats[7] = w1.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES
               splats[8] = w2.x;
               splats[9] = w2.y;
               splats[10] = w2.z;
               splats[11] = w2.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
               splats[12] = w3.x;
               splats[13] = w3.y;
               splats[14] = w3.z;
               splats[15] = w3.w;
            #endif
            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[16] = w4.x;
               splats[17] = w4.y;
               splats[18] = w4.z;
               splats[19] = w4.w;
            #endif
            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[20] = w5.x;
               splats[21] = w5.y;
               splats[22] = w5.z;
               splats[23] = w5.w;
            #endif
            #if _MAX28TEXTURES || _MAX32TEXTURES
               splats[24] = w6.x;
               splats[25] = w6.y;
               splats[26] = w6.z;
               splats[27] = w6.w;
            #endif
            #if _MAX32TEXTURES
               splats[28] = w7.x;
               splats[29] = w7.y;
               splats[30] = w7.z;
               splats[31] = w7.w;
            #endif



            weights[0] = 0;
            weights[1] = 0;
            weights[2] = 0;
            weights[3] = 0;
            indexes[0] = 0;
            indexes[1] = 0;
            indexes[2] = 0;
            indexes[3] = 0;

            int i = 0;
            for (i = 0; i < TEXCOUNT; ++i)
            {
               half w = splats[i];
               if (w >= weights[0])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = weights[0];
                  indexes[1] = indexes[0];
                  weights[0] = w;
                  indexes[0] = i;
               }
               else if (w >= weights[1])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = w;
                  indexes[1] = i;
               }
               else if (w >= weights[2])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = w;
                  indexes[2] = i;
               }
               else if (w >= weights[3])
               {
                  weights[3] = w;
                  indexes[3] = i;
               }
            }

            #if _DECAL_SPLAT
               DoMergeDecalSplats(decalOutput.Weights, decalOutput.Indexes, weights, indexes); 
            #endif


            
            
            // clamp and renormalize
            #if _MAX2LAYER
               weights.zw = 0;
               weights.xy = TotalOne(weights.xy);
            #elif _MAX3LAYER
               weights.w = 0;
               weights.xyz = TotalOne(weights.xyz);
            #elif !_DISABLEHEIGHTBLENDING || _NORMALIZEWEIGHTS // prevents black when painting, which the unity shader does not prevent.
               weights = normalize(weights);
            #endif
            

            config.uv0 = float3(scaledUV, indexes.x);
            config.uv1 = float3(scaledUV, indexes.y);
            config.uv2 = float3(scaledUV, indexes.z);
            config.uv3 = float3(scaledUV, indexes.w);


         #endif //_DISABLESPLATMAPS


      }

      float3 HeightToNormal(float height, float3 worldPos)
      {
         float3 dx = ddx(worldPos);
         float3 dy = ddy(worldPos);
         float3 crossX = cross(float3(0,1,0), dx);
         float3 crossY = cross(float3(0,1,0), dy);
         float3 d = abs(dot(crossY, dx));
         float3 n = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
         n.z *= -1;
         return normalize((d * float3(0,1,0)) - n).xzy;
      }
      
      float ComputeMipLevel(float2 uv, float2 textureSize)
      {
         uv *= textureSize;
         float2  dx_vtc        = ddx(uv);
         float2  dy_vtc        = ddy(uv);
         float delta_max_sqr   = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
         return 0.5 * log2(delta_max_sqr);
      }

      inline half2 UnpackNormal2(half4 packednormal)
      {
          return packednormal.wy * 2 - 1;
         
      }

      half3 TriplanarHBlend(half h0, half h1, half h2, half3 pN, half contrast)
      {
         half3 blend = pN / dot(pN, half3(1,1,1));
         float3 heights = float3(h0, h1, h2) + (blend * 3.0);
         half height_start = max(max(heights.x, heights.y), heights.z) - contrast;
         half3 h = max(heights - height_start.xxx, half3(0,0,0));
         blend = h / dot(h, half3(1,1,1));
         return blend;
      }
      

      void ClearAllButAlbedo(inout MicroSplatLayer o, half3 display)
      {
         o.Albedo = display.rgb;
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

      void ClearAllButAlbedo(inout MicroSplatLayer o, half display)
      {
         o.Albedo = half3(display, display, display);
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

     

      half MicroShadow(float3 lightDir, half3 normal, half ao, half strength)
      {
         half shadow = saturate(abs(dot(normal, lightDir)) + (ao * ao * 2.0) - 1.0);
         return 1 - ((1-shadow) * strength);
      }
      

      void DoDebugOutput(inout MicroSplatLayer l)
      {
         #if _DEBUG_OUTPUT_ALBEDO
            ClearAllButAlbedo(l, l.Albedo);
         #elif _DEBUG_OUTPUT_NORMAL
            // oh unit shader compiler normal stripping, how I hate you so..
            // must multiply by albedo to stop the normal from being white. Why, fuck knows?
            ClearAllButAlbedo(l, float3(l.Normal.xy * 0.5 + 0.5, l.Normal.z * saturate(l.Albedo.z+1)));
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            ClearAllButAlbedo(l, l.Smoothness.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_METAL
            ClearAllButAlbedo(l, l.Metallic.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_AO
            ClearAllButAlbedo(l, l.Occlusion.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_EMISSION
            ClearAllButAlbedo(l, l.Emission * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_HEIGHT
            ClearAllButAlbedo(l, l.Height.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_SPECULAR && _USESPECULARWORKFLOW
            ClearAllButAlbedo(l, l.Specular * saturate(l.Albedo.z+1));
         #elif _DEBUG_BRANCHCOUNT_WEIGHT
            ClearAllButAlbedo(l, _branchWeightCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TRIPLANAR
            ClearAllButAlbedo(l, _branchTriplanarCount / 24 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_CLUSTER
            ClearAllButAlbedo(l, _branchClusterCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_OTHER
            ClearAllButAlbedo(l, _branchOtherCount / 8 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TOTAL
            l.Albedo.r = _branchWeightCount / 12;
            l.Albedo.g = _branchTriplanarCount / 24;
            l.Albedo.b = _branchClusterCount / 12;
            ClearAllButAlbedo(l, (l.Albedo.r + l.Albedo.g + l.Albedo.b + (_branchOtherCount / 8)) / 4); 
         #elif _DEBUG_OUTPUT_MICROSHADOWS
            ClearAllButAlbedo(l,l.Albedo); 
         #elif _DEBUG_SAMPLECOUNT
            float sdisp = (float)_sampleCount / max(_SampleCountDiv, 1);
            half3 sdcolor = float3(sdisp, sdisp > 1 ? 1 : 0, 0);
            ClearAllButAlbedo(l, sdcolor * saturate(l.Albedo.z + 1));
         #elif _DEBUG_PROCLAYERS
            ClearAllButAlbedo(l, (float)_procLayerCount / (float)_PCLayerCount * saturate(l.Albedo.z + 1));
         #endif
      }


      // abstraction around sampler mode
      #if _USELODMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_LOD(tex, sampler##tex, u, l.x)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u, l.x)
      #elif _USEGRADMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_GRAD(tex, sampler##tex, u, l.xy, l.zw)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY_GRAD(tex, ss, u.xy, u.z, l.xy, l.zw)
      #else
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, u.xy, u.z)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u.xy, y.z)
      #endif


      #define MICROSPLAT_SAMPLE_DIFFUSE(u, cl, l) MICROSPLAT_SAMPLE(_Diffuse, u, l)
      #define MICROSPLAT_SAMPLE_EMIS(u, cl, l) MICROSPLAT_SAMPLE(_EmissiveMetal, u, l)
      #define MICROSPLAT_SAMPLE_DIFFUSE_LOD(u, cl, l) UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, u, l)
      

      #if _PACKINGHQ
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) half4(MICROSPLAT_SAMPLE(_NormalSAO, u, l).ga, MICROSPLAT_SAMPLE(_SmoothAO, u, l).ga).brag
      #else
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) MICROSPLAT_SAMPLE(_NormalSAO, u, l)
      #endif

      #if _USESPECULARWORKFLOW
         #define MICROSPLAT_SAMPLE_SPECULAR(u, cl, l) MICROSPLAT_SAMPLE(_Specular, u, l)
      #endif
      
      struct SimpleTriplanarConfig
      {
         float3 pn;
         float2 uv0;
         float2 uv1;
         float2 uv2;
      };
         
      void PrepSimpleTriplanarConfig(inout SimpleTriplanarConfig tc, float3 worldPos, float3 normal, float contrast)
      {
         tc.pn = pow(abs(normal), contrast);
         tc.pn = tc.pn / (tc.pn.x + tc.pn.y + tc.pn.z);
         
         half3 axisSign = sign(normal);

         tc.uv0 = worldPos.zy * axisSign.x;
         tc.uv1 = worldPos.xz * axisSign.y;
         tc.uv2 = worldPos.xy * axisSign.z;
      }
      
      #define SimpleTriplanarSample(tex, tc, scale) (SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv0 * scale) * tc.pn.x + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv1 * scale) * tc.pn.y + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv2 * scale) * tc.pn.z)
      #define SimpleTriplanarSampleLOD(tex, tc, scale, lod) (SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv0 * scale, lod) * tc.pn.x + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv1 * scale, lod) * tc.pn.y + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv2 * scale, lod) * tc.pn.z)
      #define SimpleTriplanarSampleGrad(tex, tc, scale) (SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv0 * scale, ddx(tc.uv0) * scale, ddy(tc.uv0) * scale) * tc.pn.x + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv1 * scale, ddx(tc.uv1) * scale, ddy(tc.uv1) * scale) * tc.pn.y + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv2 * scale, ddx(tc.uv2) * scale, ddy(tc.uv2) * scale) * tc.pn.z)
   
      
      inline half3 MicroSplatDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
      {
          specColor = lerp (half3(0,0,0), albedo, metallic);
          oneMinusReflectivity = (1-metallic);
          return albedo * oneMinusReflectivity;
      }







      Input DescToInput(ShaderData IN)
      {
        Input s = (Input)0;
        s.shaderData = IN;
        s.TBN = IN.TBNMatrix;
        s.worldNormal = IN.worldSpaceNormal;
        s.worldPos = IN.worldSpacePosition;
        s.viewDir = IN.tangentSpaceViewDir;
        s.uv_Control0 = IN.texcoord0.xy;

        s.worldUpVector = float3(0,1,0);
        s.worldHeight = IN.worldSpacePosition.y;
  
        #if _PLANETVECTORS
            float3 rwp = mul(_PQSToLocal, float4(IN.worldSpacePosition, 1));
            s.worldHeight = distance(rwp, float3(0,0,0));
            s.worldUpVector = normalize(rwp);
        #endif

        #if _MICROMESH && _MESHUV2
            s.uv2_Diffuse = IN.texcoord1.xy;
        #endif

        #if _MEGASPLAT
           UnpackMegaSplat(s, IN);
        #endif
   
        #if _MICROVERTEXMESH || _MICRODIGGERMESH
            UnpackVertexWorkflow(s, IN);
        #endif

        #if _PLANETVECTORS
           DoPlanetDataInputCopy(s, IN);
        #endif
        
        return s;
     }
     


      void SampleAlbedo(inout Config config, inout TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
         
            half4 contrasts = _Contrast.xxxx;
            #if _PERTEXTRIPLANARCONTRAST
               SAMPLE_PER_TEX(ptc, 9.5, config, half4(1,0.5,0,0));
               contrasts = half4(ptc0.y, ptc1.y, ptc2.y, ptc3.y);
            #endif


            #if _PERTEXTRIPLANAR
               SAMPLE_PER_TEX(pttri, 9.5, config, half4(0,0,0,0));
            #endif

            {
               // For per-texture triplanar, we modify the view based blending factor of the triplanar
               // such that you get a pure blend of either top down projection, or with the top down projection
               // removed and renormalized. This causes dynamic flow control optimizations to kick in and avoid
               // the extra texture samples while keeping the code simple. Yay..

               // We also only have to do this in the Albedo, because the pN values will be adjusted after the
               // albedo is sampled, causing future samples to use this data. 
              
               #if _PERTEXTRIPLANAR
                  if (pttri0.x > 0.66)
                  {
                     tc.pN0 = half3(0,1,0);
                  }
                  else if (pttri0.x > 0.33)
                  {
                     tc.pN0.y = 0;
                     tc.pN0.xz = TotalOne(tc.pN0.xz);
                  }
               #endif


               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[0], config.cluster0, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[1], config.cluster0, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[2], config.cluster0, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN0;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN0, contrasts.x);
                  tc.pN0 = bf;
               #endif

               s.albedo0 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            MSBRANCH(weights.y)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri1.x > 0.66)
                  {
                     tc.pN1 = half3(0,1,0);
                  }
                  else if (pttri1.x > 0.33)
                  {
                     tc.pN1.y = 0;
                     tc.pN1.xz = TotalOne(tc.pN1.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[0], config.cluster1, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[1], config.cluster1, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  COUNTSAMPLE
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[2], config.cluster1, d2);
               }
               half3 bf = tc.pN1;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN1, contrasts.x);
                  tc.pN1 = bf;
               #endif


               s.albedo1 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri2.x > 0.66)
                  {
                     tc.pN2 = half3(0,1,0);
                  }
                  else if (pttri2.x > 0.33)
                  {
                     tc.pN2.y = 0;
                     tc.pN2.xz = TotalOne(tc.pN2.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[0], config.cluster2, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[1], config.cluster2, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[2], config.cluster2, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN2;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN2, contrasts.x);
                  tc.pN2 = bf;
               #endif
               

               s.albedo2 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {

               #if _PERTEXTRIPLANAR
                  if (pttri3.x > 0.66)
                  {
                     tc.pN3 = half3(0,1,0);
                  }
                  else if (pttri3.x > 0.33)
                  {
                     tc.pN3.y = 0;
                     tc.pN3.xz = TotalOne(tc.pN3.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[0], config.cluster3, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[1], config.cluster3, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[2], config.cluster3, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN3;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN3, contrasts.x);
               tc.pN3 = bf;
               #endif

               s.albedo3 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif

         #else
            s.albedo0 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv0, config.cluster0, mipLevel);
            COUNTSAMPLE

            MSBRANCH(weights.y)
            {
               s.albedo1 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv1, config.cluster1, mipLevel);
               COUNTSAMPLE
            }
            #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.albedo2 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               } 
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.albedo3 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
            #endif
         #endif

         #if _PERTEXHEIGHTOFFSET || _PERTEXHEIGHTCONTRAST
            SAMPLE_PER_TEX(ptHeight, 10.5, config, 1);

            #if _PERTEXHEIGHTOFFSET
               s.albedo0.a = saturate(s.albedo0.a + ptHeight0.b - 1);
               s.albedo1.a = saturate(s.albedo1.a + ptHeight1.b - 1);
               s.albedo2.a = saturate(s.albedo2.a + ptHeight2.b - 1);
               s.albedo3.a = saturate(s.albedo3.a + ptHeight3.b - 1);
            #endif
            #if _PERTEXHEIGHTCONTRAST
               s.albedo0.a = saturate(pow(s.albedo0.a + 0.5, abs(ptHeight0.a)) - 0.5);
               s.albedo1.a = saturate(pow(s.albedo1.a + 0.5, abs(ptHeight1.a)) - 0.5);
               s.albedo2.a = saturate(pow(s.albedo2.a + 0.5, abs(ptHeight2.a)) - 0.5);
               s.albedo3.a = saturate(pow(s.albedo3.a + 0.5, abs(ptHeight3.a)) - 0.5);
            #endif
         #endif
      }
      
      
      
      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif

         #if _NONORMALMAP || _AUTONORMAL
            s.normSAO0 = half4(0,0, 0, 1);
            s.normSAO1 = half4(0,0, 0, 1);
            s.normSAO2 = half4(0,0, 0, 1);
            s.normSAO3 = half4(0,0, 0, 1);
            return;
         #endif

         
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
            
            half3 absVertNormal = abs(tc.IN.worldNormal);
            float3x3 t2w = tc.IN.TBN;
            
            
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[0], config.cluster0, d0).agrb;
                  COUNTSAMPLE
               }            
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[1], config.cluster0, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[2], config.cluster0, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf0 = SurfgradFromTriplanarProjection(tc.pN0, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO0.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN0, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO0.zw = a0.zw * tc.pN0.x + a1.zw * tc.pN0.y + a2.zw * tc.pN0.z;
            }
            MSBRANCH(weights.y)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[0], config.cluster1, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[1], config.cluster1, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[2], config.cluster1, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf1 = SurfgradFromTriplanarProjection(tc.pN1, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO1.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN1, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO1.zw = a0.zw * tc.pN1.x + a1.zw * tc.pN1.y + a2.zw * tc.pN1.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);

               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[0], config.cluster2, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[1], config.cluster2, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[2], config.cluster2, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf2 = SurfgradFromTriplanarProjection(tc.pN2, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO2.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN2, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO2.zw = a0.zw * tc.pN2.x + a1.zw * tc.pN2.y + a2.zw * tc.pN2.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[0], config.cluster3, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[1], config.cluster3, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[2], config.cluster3, d2).agrb;
                  COUNTSAMPLE
               }

               #if _SURFACENORMALS
                  s.surf3 = SurfgradFromTriplanarProjection(tc.pN3, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO3.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN3, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO3.zw = a0.zw * tc.pN3.x + a1.zw * tc.pN3.y + a2.zw * tc.pN3.z;
            }
            #endif

         #else
            s.normSAO0 = MICROSPLAT_SAMPLE_NORMAL(config.uv0, config.cluster0, mipLevel).agrb;
            COUNTSAMPLE
            s.normSAO0.xy = s.normSAO0.xy * 2 - 1;

            #if _SURFACENORMALS
               s.surf0 = ConvertNormal2ToGradient(s.normSAO0.xy);
            #endif

            MSBRANCH(weights.y)
            {
               s.normSAO1 = MICROSPLAT_SAMPLE_NORMAL(config.uv1, config.cluster1, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO1.xy = s.normSAO1.xy * 2 - 1;

               #if _SURFACENORMALS
                  s.surf1 = ConvertNormal2ToGradient(s.normSAO1.xy);
               #endif
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               s.normSAO2 = MICROSPLAT_SAMPLE_NORMAL(config.uv2, config.cluster2, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf2 = ConvertNormal2ToGradient(s.normSAO2.xy);
               #endif
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               s.normSAO3 = MICROSPLAT_SAMPLE_NORMAL(config.uv3, config.cluster3, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf3 = ConvertNormal2ToGradient(s.normSAO3.xy);
               #endif
            }
            #endif
         #endif
      }

      void SampleEmis(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USEEMISSIVEMETAL
            #if _TRIPLANAR
            
               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  s.emisMetal0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }

                  s.emisMetal1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.emisMetal0 = MICROSPLAT_SAMPLE_EMIS(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.emisMetal1 = MICROSPLAT_SAMPLE_EMIS(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
                  MSBRANCH(weights.z)
                  {
                     s.emisMetal2 = MICROSPLAT_SAMPLE_EMIS(config.uv2, config.cluster2, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  MSBRANCH(weights.w)
                  {
                     s.emisMetal3 = MICROSPLAT_SAMPLE_EMIS(config.uv3, config.cluster3, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
            #endif
         #endif
      }
      
      void SampleSpecular(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USESPECULARWORKFLOW
            #if _TRIPLANAR

               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.specular0 = MICROSPLAT_SAMPLE_SPECULAR(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.specular1 = MICROSPLAT_SAMPLE_SPECULAR(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.specular2 = MICROSPLAT_SAMPLE_SPECULAR(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.specular3 = MICROSPLAT_SAMPLE_SPECULAR(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
               #endif
            #endif
         #endif
      }

      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex, DecalOutput decalOutput)
      {
         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         half4 albedo = 0;
         half4 normSAO = half4(0,0,0,1);
         half3 surfGrad = half3(0,0,0);
         half4 emisMetal = 0;
         half3 specular = 0;
         
         float worldHeight = i.worldPos.y;
         float3 upVector = float3(0,1,0);
         
         #if _GLOBALTINT || _GLOBALNORMALS || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _GLOBALSPECULAR
            float globalSlopeFilter = 1;
            #if _GLOBALSLOPEFILTER
               float2 gfilterUV = float2(1 - saturate(dot(worldNormalVertex, upVector) * 0.5 + 0.49), 0.5);
               globalSlopeFilter = SAMPLE_TEXTURE2D(_GlobalSlopeTex, sampler_Diffuse, gfilterUV).a;
            #endif
         #endif

         // declare outside of branchy areas..
         half4 fxLevels = half4(0,0,0,0);
         half burnLevel = 0;
         half wetLevel = 0;
         half3 waterNormalFoam = half3(0, 0, 0);
         half porosity = 0.4;
         float streamFoam = 1.0f;
         half pud = 0;
         half snowCover = 0;
         half SSSThickness = 0;
         half3 SSSTint = half3(1,1,1);
         float traxBuffer = 0;
         float3 traxNormal = 0;
         float2 noiseUV = 0;
         
         

         #if _SPLATFADE
         MSBRANCHOTHER(1 - saturate(camDist - _SplatFade.y))
         {
         #endif

         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE || _SNOWFOOTSTEPS
            traxBuffer = SampleTraxBuffer(i.worldPos, worldNormalVertex, traxNormal);
         #endif
         
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
            #if _MICROMESH
               fxLevels = SampleFXLevels(InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, config.uv), wetLevel, burnLevel, traxBuffer);
            #elif _MICROVERTEXMESH || _MICRODIGGERMESH  || _MEGASPLAT
               fxLevels = ProcessFXLevels(i.fx, traxBuffer);
            #else
               fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel, traxBuffer);
            #endif
         #endif

         #if _DECAL
            fxLevels = max(fxLevels, decalOutput.fxLevels);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         

         MIPFORMAT albedoLOD = INITMIPFORMAT
         MIPFORMAT normalLOD = INITMIPFORMAT
         MIPFORMAT emisLOD = INITMIPFORMAT
         MIPFORMAT specLOD = INITMIPFORMAT
         MIPFORMAT origAlbedoLOD = INITMIPFORMAT;

         #if _TRIPLANAR && !_DISABLESPLATMAPS
            PrepTriplanar(i.shaderData.texcoord0, worldNormalVertex, i.worldPos, config, tc, weights, albedoLOD, normalLOD, emisLOD, origAlbedoLOD);
            tc.IN = i;
         #endif
         
         
         #if !_TRIPLANAR && !_DISABLESPLATMAPS
            #if _USELODMIP
               albedoLOD = ComputeMipLevel(config.uv0.xy, _Diffuse_TexelSize.zw);
               normalLOD = ComputeMipLevel(config.uv0.xy, _NormalSAO_TexelSize.zw);
               #if _USEEMISSIVEMETAL
                  emisLOD = ComputeMipLevel(config.uv0.xy, _EmissiveMetal_TexelSize.zw);
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = ComputeMipLevel(config.uv0.xy, _Specular_TexelSize.zw);;
               #endif
            #elif _USEGRADMIP
               albedoLOD = float4(ddx(config.uv0.xy), ddy(config.uv0.xy));
               normalLOD = albedoLOD;
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
            #endif

            origAlbedoLOD = albedoLOD;
         #endif

         #if _PERTEXCURVEWEIGHT
           SAMPLE_PER_TEX(ptCurveWeight, 19.5, config, half4(0.5,1,1,1));
           weights.x = lerp(smoothstep(0.5 - ptCurveWeight0.r, 0.5 + ptCurveWeight0.r, weights.x), weights.x, ptCurveWeight0.r*2);
           weights.y = lerp(smoothstep(0.5 - ptCurveWeight1.r, 0.5 + ptCurveWeight1.r, weights.y), weights.y, ptCurveWeight1.r*2);
           weights.z = lerp(smoothstep(0.5 - ptCurveWeight2.r, 0.5 + ptCurveWeight2.r, weights.z), weights.z, ptCurveWeight2.r*2);
           weights.w = lerp(smoothstep(0.5 - ptCurveWeight3.r, 0.5 + ptCurveWeight3.r, weights.w), weights.w, ptCurveWeight3.r*2);
           weights = TotalOne(weights);
         #endif
         
         

         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR && !_DISABLESPLATMAPS
            
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif

            // fix for pertex uv scale using gradient sampler and weight blended derivatives
            #if _USEGRADMIP
               albedoLOD = albedoLOD * ptUVScale0.rgrg * weights.x + 
                           albedoLOD * ptUVScale1.rgrg * weights.y + 
                           albedoLOD * ptUVScale2.rgrg * weights.z + 
                           albedoLOD * ptUVScale3.rgrg * weights.w;
               normalLOD = albedoLOD;
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
            #endif
         #endif

         #if _PERTEXUVROTATION && !_TRIPLANAR && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptUVRot, 16.5, config, half4(0,0,0,0));
            config.uv0.xy = RotateUV(config.uv0.xy, ptUVRot0.x);
            config.uv1.xy = RotateUV(config.uv1.xy, ptUVRot1.x);
            #if !_MAX2LAYER
               config.uv2.xy = RotateUV(config.uv2.xy, ptUVRot2.x);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = RotateUV(config.uv3.xy, ptUVRot0.x);
            #endif
         #endif

         
         o.Alpha = 1;

         
         #if _POM && !_DISABLESPLATMAPS
            DoPOM(i, config, tc, albedoLOD, weights, camDist, worldNormalVertex);
         #endif
         

         SampleAlbedo(config, tc, samples, albedoLOD, weights);

         #if _NOISEHEIGHT
            ApplyNoiseHeight(samples, config.uv, config, i.worldPos, worldNormalVertex);
         #endif
         
         #if _STREAMS || (_PARALLAX && !_DISABLESPLATMAPS)
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(i, config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX && !_DISABLESPLATMAPS
            DoParallax(i, earlyHeight, config, tc, samples, weights, camDist);
         #endif


         // Blend results
         #if _PERTEXINTERPCONTRAST && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = clamp(contrast + _Contrast, 0.0001, 1.0); 
            half cnt = contrast.x * weights.x + contrast.y * weights.y + contrast.z * weights.z + contrast.w * weights.w;
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, cnt);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif

         #if _HYBRIDHEIGHTBLEND
            heightWeights = lerp(heightWeights, TotalOne(weights), saturate(camDist/max(1.0, _HybridHeightBlendDistance)));
         #endif

         
         // rescale derivatives after height weighting. Basically, in gradmip mode we blend the mip levels,
         // but this is before height mapping is sampled, so reblending them after alpha will make sure the other
         // channels (normal, etc) are sharper, which likely matters most.. 
         #if _PERTEXUVSCALEOFFSET && !_DISABLESPLATMAPS
            #if _TRIPLANAR
               #if _USEGRADMIP
                  SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));

                  albedoLOD.d0 = origAlbedoLOD.d0 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d0 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d0 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d0 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d1 = origAlbedoLOD.d1 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d1 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d1 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d1 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d2 = origAlbedoLOD.d2 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d2 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d2 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d2 * ptUVScale3.xyxy * heightWeights.w;
               
                  normalLOD.d0 = albedoLOD.d0;
                  normalLOD.d1 = albedoLOD.d1;
                  normalLOD.d2 = albedoLOD.d2;
               
                  #if _USEEMISSIVEMETAL
                     emisLOD.d0 = albedoLOD.d0;
                     emisLOD.d1 = albedoLOD.d1;
                     emisLOD.d2 = albedoLOD.d2;
                  #endif
               #endif // gradmip
            #else // not triplanar
               // fix for pertex uv scale using gradient sampler and weight blended derivatives
               #if _USEGRADMIP
                  albedoLOD = origAlbedoLOD * ptUVScale0.rgrg * heightWeights.x + 
                              origAlbedoLOD * ptUVScale1.rgrg * heightWeights.y + 
                              origAlbedoLOD * ptUVScale2.rgrg * heightWeights.z + 
                              origAlbedoLOD * ptUVScale3.rgrg * heightWeights.w;
                  normalLOD = albedoLOD;
                  #if _USEEMISSIVEMETAL
                     emisLOD = albedoLOD;
                  #endif
                  #if _USESPECULARWORKFLOW
                     specLOD = albedoLOD;
                  #endif
               #endif
            #endif
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples, albedoLOD, heightWeights);
         #endif


         SampleNormal(config, tc, samples, normalLOD, heightWeights);

         #if _USEEMISSIVEMETAL
            SampleEmis(config, tc, samples, emisLOD, heightWeights);
         #endif

         #if _USESPECULARWORKFLOW
            SampleSpecular(config, tc, samples, specLOD, heightWeights);
         #endif

         #if _DISTANCERESAMPLE && !_DISABLESPLATMAPS
            DistanceResample(samples, config, tc, camDist, i.viewDir, fxLevels, albedoLOD, i.worldPos, heightWeights, worldNormalVertex);
         #endif

         #if _STARREACHFORMAT
            samples.normSAO0.w = length(samples.normSAO0.xy);
            samples.normSAO1.w = length(samples.normSAO1.xy);
            samples.normSAO2.w = length(samples.normSAO2.xy);
            samples.normSAO3.w = length(samples.normSAO3.xy);
         #endif

         // PerTexture sampling goes here, passing the samples structure
         
         #if _PERTEXMICROSHADOWS || _PERTEXFUZZYSHADE
            SAMPLE_PER_TEX(ptFuzz, 17.5, config, half4(0, 0, 1, 1));
         #endif

         #if _PERTEXMICROSHADOWS
            #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP)
            {
               half3 lightDir = GetGlobalLightDirTS(i);
               half4 microShadows = half4(1,1,1,1);
               microShadows.x = MicroShadow(lightDir, half3(samples.normSAO0.xy, 1), samples.normSAO0.a, ptFuzz0.a);
               microShadows.y = MicroShadow(lightDir, half3(samples.normSAO1.xy, 1), samples.normSAO1.a, ptFuzz1.a);
               microShadows.z = MicroShadow(lightDir, half3(samples.normSAO2.xy, 1), samples.normSAO2.a, ptFuzz2.a);
               microShadows.w = MicroShadow(lightDir, half3(samples.normSAO3.xy, 1), samples.normSAO3.a, ptFuzz3.a);
               samples.normSAO0.a *= microShadows.x;
               samples.normSAO1.a *= microShadows.y;
               #if !_MAX2LAYER
                  samples.normSAO2.a *= microShadows.z;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.normSAO3.a *= microShadows.w;
               #endif

               
               #if _DEBUG_OUTPUT_MICROSHADOWS
               o.Albedo = BlendWeights(microShadows.x, microShadows.y, microShadows.z, microShadows.a, heightWeights);
               return o;
               #endif

               

               
            }
            #endif

         #endif // _PERTEXMICROSHADOWS


         #if _PERTEXFUZZYSHADE
            samples.albedo0.rgb = FuzzyShade(samples.albedo0.rgb, half3(samples.normSAO0.rg, 1), ptFuzz0.r, ptFuzz0.g, ptFuzz0.b, i.viewDir);
            samples.albedo1.rgb = FuzzyShade(samples.albedo1.rgb, half3(samples.normSAO1.rg, 1), ptFuzz1.r, ptFuzz1.g, ptFuzz1.b, i.viewDir);
            #if !_MAX2LAYER
               samples.albedo2.rgb = FuzzyShade(samples.albedo2.rgb, half3(samples.normSAO2.rg, 1), ptFuzz2.r, ptFuzz2.g, ptFuzz2.b, i.viewDir);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = FuzzyShade(samples.albedo3.rgb, half3(samples.normSAO3.rg, 1), ptFuzz3.r, ptFuzz3.g, ptFuzz3.b, i.viewDir);
            #endif
         #endif

         #if _PERTEXSATURATION && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptSaturattion, 9.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = lerp(MSLuminance(samples.albedo0.rgb), samples.albedo0.rgb, ptSaturattion0.a);
            samples.albedo1.rgb = lerp(MSLuminance(samples.albedo1.rgb), samples.albedo1.rgb, ptSaturattion1.a);
            #if !_MAX2LAYER
               samples.albedo2.rgb = lerp(MSLuminance(samples.albedo2.rgb), samples.albedo2.rgb, ptSaturattion2.a);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = lerp(MSLuminance(samples.albedo3.rgb), samples.albedo3.rgb, ptSaturattion3.a);
            #endif
         
         #endif
         
         #if _PERTEXTINT && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif
         
         #if _PCHEIGHTGRADIENT || _PCHEIGHTHSV || _PCSLOPEGRADIENT || _PCSLOPEHSV
            ProceduralGradients(i, samples, config, worldHeight, worldNormalVertex);
         #endif

         
         

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXCOLORINTENSITY
            SAMPLE_PER_TEX(ptCI, 23.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = saturate(samples.albedo0.rgb * (1 + ptCI0.rrr));
            samples.albedo1.rgb = saturate(samples.albedo1.rgb * (1 + ptCI1.rrr));
            #if !_MAX2LAYER
               samples.albedo2.rgb = saturate(samples.albedo2.rgb * (1 + ptCI2.rrr));
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = saturate(samples.albedo3.rgb * (1 + ptCI3.rrr));
            #endif
         #endif

         #if (_PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = saturate(((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5);
               samples.albedo1.rgb = saturate(((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5);
               #if !_MAX2LAYER
                 samples.albedo2.rgb = saturate(((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5);
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb = saturate(samples.albedo0.rgb + ptBC0.rrr);
               samples.albedo1.rgb = saturate(samples.albedo1.rgb + ptBC1.rrr);
               #if !_MAX2LAYER
                  samples.albedo2.rgb = saturate(samples.albedo2.rgb + ptBC2.rrr);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(samples.albedo3.rgb + ptBC3.rrr);
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if (_PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR && !_DISABLESPLATMAPS
            #if _SURFACENORMALS
               samples.surf0 *= perTexMatSettings0.r;
               samples.surf1 *= perTexMatSettings1.r;
               samples.surf2 *= perTexMatSettings2.r;
               samples.surf3 *= perTexMatSettings3.r;
            #else
               samples.normSAO0.xy *= perTexMatSettings0.r;
               samples.normSAO1.xy *= perTexMatSettings1.r;
               samples.normSAO2.xy *= perTexMatSettings2.r;
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR && !_DISABLESPLATMAPS
            samples.normSAO0.a = pow(abs(samples.normSAO0.a), perTexMatSettings0.b);
            samples.normSAO1.a = pow(abs(samples.normSAO1.a), perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(abs(samples.normSAO2.a), perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(abs(samples.normSAO3.a), perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR && !_DISABLESPLATMAPS
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         
         #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP) 
          #if _PERTEXSSS
          {
            SAMPLE_PER_TEX(ptSSS, 18.5, config, half4(1, 1, 1, 1)); // tint, thickness
            half4 vals = ptSSS0 * heightWeights.x + ptSSS1 * heightWeights.y + ptSSS2 * heightWeights.z + ptSSS3 * heightWeights.w;
            SSSThickness = vals.a;
            SSSTint = vals.rgb;
          }
          #endif
         #endif

         #if _PERTEXRIMLIGHT
         {
            SAMPLE_PER_TEX(ptRimA, 26.5, config, half4(1, 1, 1, 1));
            SAMPLE_PER_TEX(ptRimB, 27.5, config, half4(1, 1, 1, 0));
            samples.emisMetal0.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO0.xy, 1))), max(0.0001, ptRimA0.g)) * ptRimB0.rgb * ptRimB0.a;
            samples.emisMetal1.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO1.xy, 1))), max(0.0001, ptRimA1.g)) * ptRimB1.rgb * ptRimB1.a;
            samples.emisMetal2.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO2.xy, 1))), max(0.0001, ptRimA2.g)) * ptRimB2.rgb * ptRimB2.a;
            samples.emisMetal3.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO3.xy, 1))), max(0.0001, ptRimA3.g)) * ptRimB3.rgb * ptRimB3.a;
         }
         #endif



         #if (((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)) && !_DISABLESPLATMAPS
            ApplyDetailDistanceNoisePerTex(samples, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         
         #if _GLOBALNOISEUV
            // noise defaults so that a value of 1, 1 is 4 pixels in size and moves the uvs by 1 pixel max.
            #if _CUSTOMSPLATTEXTURES
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #else
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif
         #endif

         
         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE
            ApplyTrax(samples, config, i.worldPos, traxBuffer, traxNormal);
         #endif

         #if (_ANTITILEARRAYDETAIL || _ANTITILEARRAYDISTANCE || _ANTITILEARRAYNORMAL) && !_DISABLESPLATMAPS
            ApplyAntiTilePerTex(samples, config, camDist, i.worldPos, worldNormalVertex, heightWeights);
         #endif

         #if _GEOMAP && !_DISABLESPLATMAPS
         GeoTexturePerTex(samples, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif
         
         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH && !_DISABLESPLATMAPS
         GlobalTintTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH && !_DISABLESPLATMAPS
         GlobalNormalTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && _PERTEXGLOBALSAOMSTRENGTH && !_DISABLESPLATMAPS
         GlobalSAOMTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALEMIS && _PERTEXGLOBALEMISSTRENGTH && !_DISABLESPLATMAPS
         GlobalEmisTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && _PERTEXGLOBALSPECULARSTRENGTH && !_DISABLESPLATMAPS && _USESPECULARWORKFLOW
         GlobalSpecularTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _PERTEXMETALLIC && !_DISABLESPLATMAPS
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         #if _GLITTER && !_DISABLESPLATMAPS
            DoGlitter(i, samples, config, camDist, worldNormalVertex, i.worldPos);
         #endif
         
         // Blend em..
         #if _DISABLESPLATMAPS
            // If we don't sample from the _Diffuse, then the shader compiler will strip the sampler on
            // some platforms, which will cause everything to break. So we sample from the lowest mip
            // and saturate to 1 to keep the cost minimal. Annoying, but the compiler removes the texture
            // and sampler, even though the sampler is still used.
            albedo = saturate(UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, float3(0,0,0), 12) + 1);
            albedo.a = 0.5; // make height something we can blend with for the combined mesh mode, since it still height blends.
            normSAO = half4(0,0,0,1);
         #else
            albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
            normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);

            #if _SURFACENORMALS
               surfGrad = BlendWeights(samples.surf0, samples.surf1, samples.surf2, samples.surf3, heightWeights);
            #endif

            #if (_USEEMISSIVEMETAL || _PERTEXRIMLIGHT) && !_DISABLESPLATMAPS
               emisMetal = BlendWeights(samples.emisMetal0, samples.emisMetal1, samples.emisMetal2, samples.emisMetal3, heightWeights);
            #endif

            #if _USESPECULARWORKFLOW && !_DISABLESPLATMAPS
               specular = BlendWeights(samples.specular0, samples.specular1, samples.specular2, samples.specular3, heightWeights);
            #endif

            #if _PERTEXOUTLINECOLOR
               SAMPLE_PER_TEX(ptOutlineColor, 28.5, config, half4(0.5, 0.5, 0.5, 1));
               half4 outlineColor = BlendWeights(ptOutlineColor0, ptOutlineColor1, ptOutlineColor2, ptOutlineColor3, heightWeights);
               half4 tstr = saturate(abs(heightWeights - 0.5) * 2);
               half transitionBlend = min(min(min(tstr.x, tstr.y), tstr.z), tstr.w);
               albedo.rgb = lerp(albedo.rgb * outlineColor.rgb * 2, albedo.rgb, outlineColor.a * transitionBlend);
            #endif
         #endif



         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
            o.Alpha = 1.0;
            if (config.uv0.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.x;
            else if (config.uv1.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.y;
            else if (config.uv2.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.z;
            else if (config.uv3.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.w;
         #endif



         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
            ApplyDetailDistanceNoise(albedo.rgb, normSAO, surfGrad, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         #if _SPLATFADE
         }
         #endif

         #if _SPLATFADE
            
            float2 sfDX = ddx(config.uv * _UVScale);
            float2 sfDY = ddy(config.uv * _UVScale);

            MSBRANCHOTHER(camDist - _SplatFade.x)
            {
               float falloff = saturate(InverseLerp(_SplatFade.x, _SplatFade.y, camDist));
               half4 sfalb = SAMPLE_TEXTURE2D_ARRAY_GRAD(_Diffuse, sampler_Diffuse, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY);
               COUNTSAMPLE
               albedo.rgb = lerp(albedo.rgb, sfalb.rgb, falloff);

               #if !_NONORMALMAP && !_AUTONORMAL
                  half4 sfnormSAO = SAMPLE_TEXTURE2D_ARRAY_GRAD(_NormalSAO, sampler_NormalSAO, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY).agrb;
                  COUNTSAMPLE
                  sfnormSAO.xy = sfnormSAO.xy * 2 - 1;

                  normSAO = lerp(normSAO, sfnormSAO, falloff);
  
                  #if _SURFACENORMALS
                     surfGrad = lerp(surfGrad, ConvertNormal2ToGradient(sfnormSAO.xy), falloff);
                  #endif
               #endif
              
            }
         #endif

         #if _AUTONORMAL
            float3 autoNormal = HeightToNormal(albedo.a * _AutoNormalHeightScale, i.worldPos);
            normSAO.xy = autoNormal;
            normSAO.z = 0;
            normSAO.w = (autoNormal.z * autoNormal.z);
         #endif
 


         #if _MESHCOMBINED
            SampleMeshCombined(albedo, normSAO, surfGrad, emisMetal, specular, o.Alpha, SSSThickness, SSSTint, config, heightWeights);
         #endif

         #if _ISOBJECTSHADER
            SampleObjectShader(i, albedo, normSAO, surfGrad, emisMetal, specular, config);
         #endif

         #if _GEOMAP
            GeoTexture(albedo.rgb, normSAO, surfGrad, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif

         
         #if _SCATTER
            ApplyScatter(
               #if _MEGASPLAT
               config, 
               #endif
               i, albedo, normSAO, surfGrad, config.uv, camDist);
         #endif

         #if _DECAL
            DoDecalBlend(decalOutput, albedo, normSAO, surfGrad, emisMetal, i.uv_Control0);
         #endif
         

         #if _GLOBALTINT && !_PERTEXGLOBALTINTSTRENGTH
            GlobalTintTexture(albedo.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _VSGRASSMAP
            VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMALSTRENGTH
            GlobalNormalTexture(normSAO, surfGrad, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && !_PERTEXGLOBALSAOMSTRENGTH
            GlobalSAOMTexture(normSAO, emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALEMIS && !_PERTEXGLOBALEMISSTRENGTH
            GlobalEmisTexture(emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && !_PERTEXGLOBALSPECULARSTRENGTH && _USESPECULARWORKFLOW
            GlobalSpecularTexture(specular.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         
         
         o.Albedo = albedo.rgb;
         o.Height = albedo.a;

         #if _NONORMALMAP
            o.Normal = half3(0,0,1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #elif _SURFACENORMALS
            o.Normal = ResolveNormalFromSurfaceGradient(surfGrad);
            o.Normal = mul(GetTBN(i), o.Normal);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #else
            o.Normal = half3(normSAO.xy, 1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;       
         #endif


         

         #if _USEEMISSIVEMETAL || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _PERTEXRIMLIGHT
           #if _USEEMISSIVEMETAL
	           emisMetal.rgb *= _EmissiveMult;
	        #endif
           
           o.Emission += emisMetal.rgb;
           o.Metallic = emisMetal.a;
	        
         #endif

         #if _USESPECULARWORKFLOW
            o.Specular = specular;
         #endif

         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(i, o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         
         #if _SNOW
         snowCover = DoSnow(i, o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, 
            config, weights, SSSTint, SSSThickness, traxBuffer, traxNormal);
         #endif

         #if _PERTEXSSS || _MESHCOMBINEDUSESSS || (_SNOW && _SNOWSSS)
         {
            half3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

            o.Emission += ComputeSSS(i, worldView, WorldNormalVector(i, o.Normal),
               SSSTint, SSSThickness, _SSSDistance, _SSSScale, _SSSPower);
         }
         #endif
         
         #if _SNOWGLITTER
            DoSnowGlitter(i, config, o, camDist, worldNormalVertex, snowCover);
         #endif

         #if _WINDPARTICULATE || _SNOWPARTICULATE
            DoWindParticulate(i, o, config, weights, camDist, worldNormalVertex, snowCover);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         #if _SPECULARFADE
         {
            float specFade = saturate((i.worldPos.y - _SpecularFades.x) / max(_SpecularFades.y - _SpecularFades.x, 0.0001));
            o.Metallic *= specFade;
            o.Smoothness *= specFade;
         }
         #endif

         #if _VSSHADOWMAP
         VSShadowTexture(o, i, config, camDist);
         #endif
         
         #if _TOONWIREFRAME
         ToonWireframe(config.uv, o.Albedo, camDist);
         #endif

        #if _SEETHROUGHSHADER
            SeethroughShader(o.Albedo, o.Emission, o.Alpha, i.worldPos, o.Normal, i.worldNormal);                   
        #endif


         #if _DEBUG_TRAXBUFFER
            ClearAllButAlbedo(o, half3(traxBuffer, 0, 0) * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMALVERTEX
            ClearAllButAlbedo(o, worldNormalVertex * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMAL
            ClearAllButAlbedo(o,  WorldNormalVector(i, o.Normal) * saturate(o.Albedo.z+1));
         #endif

         #if _DEBUG_MEGABARY && _MEGASPLAT
            o.Albedo = i.baryWeights.xyz;
         #endif


         return o;
      }
      
      void SampleSplats(float2 controlUV, inout half4 w0, inout half4 w1, inout half4 w2, inout half4 w3, inout half4 w4, inout half4 w5, inout half4 w6, inout half4 w7)
      {
         #if _CUSTOMSPLATTEXTURES
            #if !_MICROMESH
            controlUV = (controlUV * (_CustomControl0_TexelSize.zw - 1.0f) + 0.5f) * _CustomControl0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_CustomControl0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_CustomControl1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_CustomControl2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_CustomControl3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_CustomControl4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_CustomControl5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_CustomControl6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_CustomControl7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #else
            #if !_MICROMESH
            controlUV = (controlUV * (_Control0_TexelSize.zw - 1.0f) + 0.5f) * _Control0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_Control0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_Control1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_Control2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_Control3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_Control4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_Control5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_Control6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_Control7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #endif
      }   


      

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
         #if _MEGANOUV
            i.uv_Control0 = i.worldPos.xz;
         #endif
         
         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);
          
         #if _FORCELOCALSPACE
            worldNormalVertex = mul((float3x3)GetWorldToObjectMatrix(), worldNormalVertex).xyz;
            i.worldPos = i.worldPos -  mul(GetObjectToWorldMatrix(), float4(0,0,0,1)).xyz;
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ORIGINSHIFT
             i.worldPos = i.worldPos + mul(_GlobalOriginMTX, float4(0,0,0,1)).xyz;
             i.worldHeight = i.worldPos.y;
         #endif

         #if _DEBUG_USE_TOPOLOGY
            i.worldPos = SAMPLE_TEXTURE2D(_DebugWorldPos, sampler_Diffuse, i.uv_Control0);
            worldNormalVertex = SAMPLE_TEXTURE2D(_DebugWorldNormal, sampler_Diffuse, i.uv_Control0);
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ALPHABELOWHEIGHT && !_TBDISABLEALPHAHOLES
            ClipWaterLevel(i.worldPos);
         #endif

         #if !_TBDISABLEALPHAHOLES && defined(_ALPHATEST_ON)
            // UNITY 2019.3 holes
            ClipHoles(i.uv_Control0);
         #endif


         float2 origUV = i.uv_Control0;

         #if _MICROMESH && _MESHUV2
         float2 controlUV = i.uv2_Diffuse;
         #else
         float2 controlUV = i.uv_Control0;
         #endif


         #if _MICROMESH
            controlUV = InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, controlUV);
         #endif

         half4 weights = half4(1,0,0,0);

         Config config = (Config)0;
         UNITY_INITIALIZE_OUTPUT(Config,config);
         config.uv = origUV;

         DecalOutput decalOutput = (DecalOutput)0;
         #if _DECAL
            decalOutput = DoDecals(i.uv_Control0, i.worldPos, camDist, worldNormalVertex);
         #endif

         #if _SURFACENORMALS
         // Initialize the surface gradient basis vectors
         ConstructSurfaceGradientTBN(i);
         #endif
        


         #if _SPLATFADE
         MSBRANCHOTHER(_SplatFade.y - camDist)
         #endif // _SPLATFADE
         {
            #if !_DISABLESPLATMAPS

               // Sample the splat data, from textures or vertices, and setup the config..
               #if _MICRODIGGERMESH
                  DiggerSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLAT
                  MegaSplatVertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLATTEXTURE
                   MegaSplatTextureSetup(controlUV, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MICROVERTEXMESH
                  VertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif !_PROCEDURALTEXTURE || _PROCEDURALBLENDSPLATS
                  half4 w0 = 0; half4 w1 = 0; half4 w2 = 0; half4 w3 = 0; half4 w4 = 0; half4 w5 = 0; half4 w6 = 0; half4 w7 = 0;
                  SampleSplats(controlUV, w0, w1, w2, w3, w4, w5, w6, w7);
                  Setup(weights, origUV, config, w0, w1, w2, w3, w4, w5, w6, w7, i.worldPos, decalOutput);
               #endif

               #if _PROCEDURALTEXTURE
                  float3 procNormal = worldNormalVertex;
                  float3 worldPos = i.worldPos;
                  ProceduralSetup(i, worldPos, i.worldHeight, procNormal, i.worldUpVector, weights, origUV, config, ddx(origUV), ddy(origUV), ddx(worldPos), ddy(worldPos), decalOutput);
               #endif
            #else // _DISABLESPLATMAPS
                Setup(weights, origUV, config, half4(1,0,0,0), 0, 0, 0, 0, 0, 0, 0, i.worldPos, decalOutput);
            #endif

            #if _SLOPETEXTURE
               SlopeTexture(config, weights, worldNormalVertex);
            #endif
         } // _SPLATFADE else case


         #if _TOONFLATTEXTURE
            float2 quv = floor(origUV * _ToonTerrainSize);
            float2 fuv = frac(origUV * _ToonTerrainSize);
            #if !_TOONFLATTEXTUREQUAD
               quv = Hash2D((fuv.x > fuv.y) ? quv : quv * 0.333);
            #endif
            float2 uvq = quv / _ToonTerrainSize;
            config.uv0.xy = uvq;
            config.uv1.xy = uvq;
            config.uv2.xy = uvq;
            config.uv3.xy = uvq;
         #endif
         
         #if (_TEXTURECLUSTER2 || _TEXTURECLUSTER3) && !_DISABLESPLATMAPS
            PrepClusters(origUV, config, i.worldPos, worldNormalVertex);
         #endif

         #if (_ALPHAHOLE || _ALPHAHOLETEXTURE) && !_DISABLESPLATMAPS && !_TBDISABLEALPHAHOLES
         ClipAlphaHole(config, weights);
         #endif


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex, decalOutput);

         // On windows, sometimes the shared samplers gets stripped, so we have to do this crap.
         // We sample from the lowest mip, so it shouldn't cost much, but still, I hate this, wtf..
         float stripVal = saturate(SAMPLE_TEXTURE2D_LOD(_Diffuse, sampler_Diffuse, config.uv0, 11).r + 2);
         stripVal *= saturate(SAMPLE_TEXTURE2D_LOD(_NormalSAO, sampler_NormalSAO, config.uv0, 11).r + 2);
         l.Albedo *= stripVal;
         l.Normal *= stripVal;

         #if _PROCEDURALTEXTURE
            ProceduralTextureDebugOutput(l, weights, config);
         #endif


         return l;

      }



   





float4 ConstructTerrainTangent(float3 normal, float3 positiveZ)
{
    // Consider a flat terrain. It should have tangent be (1, 0, 0) and bitangent be (0, 0, 1) as the UV of the terrain grid mesh is a scale of the world XZ position.
    // In CreateTangentToWorld function (in SpaceTransform.hlsl), it is cross(normal, tangent) * sgn for the bitangent vector.
    // It is not true in a left-handed coordinate system for the terrain bitangent, if we provide 1 as the tangent.w. It would produce (0, 0, -1) instead of (0, 0, 1).
    // Also terrain's tangent calculation was wrong in a left handed system because cross((0,0,1), terrainNormalOS) points to the wrong direction as negative X.
    // Therefore all the 4 xyzw components of the tangent needs to be flipped to correct the tangent frame.
    // (See TerrainLitData.hlsl - GetSurfaceAndBuiltinData)
    float3 tangent = cross(normal, positiveZ);
    return float4(tangent, -1);
}



void TerrainInstancing(inout float4 vertex, inout float3 normal, inout float2 uv)
{
#if _MICROTERRAIN && defined(UNITY_INSTANCING_ENABLED) && !_TERRAINBLENDABLESHADER
   
    float2 patchVertex = vertex.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;

    float2 sampleUV = (uv / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

    float height = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, shared_linear_clamp_sampler, sampleUV, 0));
   
    vertex.xz = sampleCoords * _TerrainHeightmapScale.xz;
    vertex.y = height * _TerrainHeightmapScale.y;

    
    normal = float3(0, 1, 0);

#endif
}


void ApplyMeshModification(inout VertexData input)
{
   #if _MICROTERRAIN && !_TERRAINBLENDABLESHADER
      float2 uv = input.texcoord0.xy;
      TerrainInstancing(input.vertex, input.normal, uv);
      input.texcoord0.xy = uv;
   #endif
   #if _PERPIXNORMAL && !_TERRAINBLENDABLESHADER
      input.normal = float3(0,1,0);
   #endif

}

// called by the template, so we can remove tangent from VertexData
void ApplyTerrainTangent(inout VertexToPixel input)
{
   #if (_MICROTERRAIN || _PERPIXNORMAL) && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif

   // digger meshes ain't got no tangent either..
   #if _MICRODIGGERMESH && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif
}


void ModifyVertex(inout VertexData v, inout ExtraV2F d)
{
   ApplyMeshModification(v);

   #if _MICROVERTEXMESH || _MICRODIGGERMESH
      EncodeVertexWorkflow(v, d);
   #elif _MEGASPLAT
      EncodeMegaSplatVertex(v, d);
   #endif

}


void ModifyTessellatedVertex(inout VertexData v, inout ExtraV2F d)
{
   #if _TESSDISTANCE
      v.vertex.xyz += OffsetVertex(v, d);
   #endif
}

float3 GetTessFactors ()
{
   #if _TESSDISTANCE
      return float3(_TessData2.x, _TessData2.y, _TessData1.x);
   #endif
   return 0;
}


        


    
    void SurfaceFunction(inout Surface o, inout ShaderData d)
    {
       
        float3 worldNormalVertex = d.worldSpaceNormal;

        #if (defined(UNITY_INSTANCING_ENABLED) && _MICROTERRAIN && !_TERRAINBLENDABLESHADER)
            float2 sampleCoords = (d.texcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            float3 geomBitangent = normalize(cross(geomNormal, geomTangent)) * -1;
            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

         #elif _PERPIXNORMAL &&  (_MICROTERRAIN || _MICROMESHTERRAIN) && !_TERRAINBLENDABLESHADER
            float2 sampleCoords = (d.texcoord0.xy * _PerPixelNormal_TexelSize.zw + 0.5f) * _PerPixelNormal_TexelSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_PerPixelNormal, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            
            float3 geomBitangent = normalize(cross(geomTangent, geomNormal)) * -1;
            #if _MICROMESHTERRAIN
                geomBitangent *= -1;
            #endif

            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

        #endif

        #if _TOONPOLYEDGE
           FlatShade(d);
        #endif

         Input i = DescToInput(d);

         
         
         #if _SRPTERRAINBLEND
            MicroSplatLayer l = BlendWithTerrain(d);

               #if _DEBUG_WORLDNORMAL
                  ClearAllButAlbedo(l, normalize(TangentToWorldSpace(d, l.Normal)) * saturate(l.Albedo.z+1));
               #endif
         #else
            MicroSplatLayer l = SurfImpl(i, worldNormalVertex);
         #endif

        DoDebugOutput(l);




        o.Albedo = l.Albedo;
        o.Normal = l.Normal;
        o.Smoothness = l.Smoothness;
        o.Occlusion = l.Occlusion;
        o.Metallic = l.Metallic;
        o.Emission = l.Emission;
        #if _USESPECULARWORKFLOW
        o.Specular = l.Specular;
        #endif
        o.Height = l.Height;
        o.Alpha = l.Alpha;


    }



        



            // SHADERDESC

         ShaderData CreateShaderData(VertexToPixel i)
         {
            ShaderData d = (ShaderData)0;
            d.worldSpacePosition = i.worldPos;

            d.worldSpaceNormal = i.worldNormal;
            d.worldSpaceTangent = i.worldTangent.xyz;
            float3 bitangent = cross(i.worldTangent.xyz, i.worldNormal) * i.worldTangent.w * -1;
            

            d.TBNMatrix = float3x3(d.worldSpaceTangent, bitangent, d.worldSpaceNormal);
            d.worldSpaceViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);
             d.texcoord0 = i.texcoord0;
            #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                d.texcoord1 = i.texcoord1;
               // d.texcoord2 = i.texcoord2;
            #endif
            // d.texcoord3 = i.texcoord3;
            // d.vertexColor = i.vertexColor;

            // these rarely get used, so we back transform them. Usually will be stripped.
            #if _HDRP
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(GetCameraRelativePositionWS(i.worldPos), 1));
            #else
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(i.worldPos, 1));
            #endif
            // d.localSpaceNormal = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldNormal));
            // d.localSpaceTangent = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldTangent.xyz));

            // d.screenPos = i.screenPos;
            // d.screenUV = i.screenPos.xy / i.screenPos.w;

            // d.extraV2F0 = i.extraV2F0;
            // d.extraV2F1 = i.extraV2F1;
            // d.extraV2F2 = i.extraV2F2;
            // d.extraV2F3 = i.extraV2F3;
            // d.extraV2F4 = i.extraV2F4;
            // d.extraV2F5 = i.extraV2F5;
            // d.extraV2F6 = i.extraV2F6;
            // d.extraV2F7 = i.extraV2F7;

            return d;
         }
         // CHAINS

            void ChainModifyVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               ModifyVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }

            void ChainModifyTessellatedVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               // d.extraV2F0 = v2p.extraV2F0;
               // d.extraV2F1 = v2p.extraV2F1;
               // d.extraV2F2 = v2p.extraV2F2;
               // d.extraV2F3 = v2p.extraV2F3;
               // d.extraV2F4 = v2p.extraV2F4;
               // d.extraV2F5 = v2p.extraV2F5;
               // d.extraV2F6 = v2p.extraV2F6;
               // d.extraV2F7 = v2p.extraV2F7;

               ModifyTessellatedVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }


            void ChainFinalColorForward(inout Surface l, inout ShaderData d, inout half4 color)
            {
               
            }

            void ChainFinalGBufferStandard(inout Surface s, inout ShaderData d, inout half4 GBuffer0, inout half4 GBuffer1, inout half4 GBuffer2, inout half4 outEmission, inout half4 outShadowMask)
            {
               
            }


            
         #if _PASSSHADOW
            float3 _LightDirection;
            float3 _LightPosition;
         #endif

         // vertex shader
         VertexToPixel Vert (VertexData v)
         {
           
           VertexToPixel o = (VertexToPixel)0;

           UNITY_SETUP_INSTANCE_ID(v);
           UNITY_TRANSFER_INSTANCE_ID(v, o);
           UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


#if !_TESSELLATION_ON
           ChainModifyVertex(v, o);
#endif

            o.texcoord0 = v.texcoord0;

           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               o.texcoord1 = v.texcoord1;
              // o.texcoord2 = v.texcoord2;
           #endif

           // o.texcoord3 = v.texcoord3;
           // o.vertexColor = v.vertexColor;
           
           VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
           o.worldPos = TransformObjectToWorld(v.vertex.xyz);
           o.worldNormal = TransformObjectToWorldNormal(v.normal);

           
           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float2 uv1 = v.texcoord1.xy;
               float2 uv2 = v.texcoord2.xy;
               o.worldTangent = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);
           #else
               float2 uv1 = v.texcoord0.xy;
               float2 uv2 = uv1;
           #endif

           // MS Only
           ApplyTerrainTangent(o);
           

          #if _PASSSHADOW
              #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                 float3 lightDirectionWS = normalize(_LightPosition - o.worldPos);
              #else
                 float3 lightDirectionWS = _LightDirection;
              #endif
              // Define shadow pass specific clip position for Universal
              o.pos = TransformWorldToHClip(ApplyShadowBias(o.worldPos, o.worldNormal, lightDirectionWS));
              #if UNITY_REVERSED_Z
                  o.pos.z = min(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #else
                  o.pos.z = max(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #endif
          #elif _PASSMETA
              #if _MICROTERRAIN
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), v.texcoord0.xy, v.texcoord0.xy, unity_LightmapST, unity_DynamicLightmapST);
              #else
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), uv1, uv2, unity_LightmapST, unity_DynamicLightmapST);
              #endif
          #else
              o.pos = TransformWorldToHClip(o.worldPos);
          #endif



          // o.screenPos = ComputeScreenPos(o.pos, _ProjectionParams.x);
          

          #if _PASSFORWARD || _PASSGBUFFER
              #if _MICROTERRAIN
                 OUTPUT_LIGHTMAP_UV(v.texcoord0.xy, unity_LightmapST, o.lightmapUV);
              #else
                 OUTPUT_LIGHTMAP_UV(uv1, unity_LightmapST, o.lightmapUV);
              #endif
              OUTPUT_SH(o.worldNormal, o.sh);
              #if defined(DYNAMICLIGHTMAP_ON)
                   o.dynamicLightmapUV.xy = uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
              #endif
          #endif

          #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
              half fogFactor = 0;
              #if defined(_FOG_FRAGMENT)
                fogFactor = ComputeFogFactor(o.pos.z);
              #endif
              #if _BAKEDLIT
                  half3 vertexLight = VertexLighting(o.worldPos, o.worldNormal);
                  o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
              #else
                  o.fogFactorAndVertexLight = half4(fogFactor, 0,0,0);
              #endif
          #endif

          #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             o.shadowCoord = GetShadowCoord(vertexInput);
          #endif

           return o;
         }


            

            // fragment shader
            half4 Frag (VertexToPixel IN) : SV_Target
            {
               UNITY_SETUP_INSTANCE_ID(IN);

               ShaderData d = CreateShaderData(IN);

               Surface l = (Surface)0;

               l.Albedo = half3(0.5, 0.5, 0.5);
               l.Normal = float3(0,0,1);
               l.Occlusion = 1;
               l.Alpha = 1;

               SurfaceFunction(l, d);

               MetaInput metaInput = (MetaInput)0;
               metaInput.Albedo = l.Albedo;
               metaInput.Emission = l.Emission;

               return MetaFragment(metaInput);

            }

         ENDHLSL

      }


      
        Pass
        {
            Name "DepthNormals"
            Tags
            {
               "LightMode" = "DepthNormals"
            }
    
            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            

            HLSLPROGRAM

               #pragma vertex Vert
   #pragma fragment Frag

            #pragma target 3.5

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_local _ _ALPHATEST_ON

        
            #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
            #define _PASSDEPTH 1
            #define _PASSDEPTHNORMALS 1


            
      #define _MICROSPLAT 1
      #define _MICROTERRAIN 1
      #define _USEGRADMIP 1
      #define _MAX4TEXTURES 1
      #define _BRANCHSAMPLES 1
      #define _BRANCHSAMPLESAGR 1
      #define _MSRENDERLOOP_UNITYURP2022 1

#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


   #define _URP 1



            // this has to be here or specular color will be ignored. Not in SG code
            #if _SIMPLELIT
               #define _SPECULAR_COLOR
            #endif


            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"


        

               #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, float3x3(d.worldSpaceTangent, cross(d.worldSpaceTangent, d.worldSpaceNormal), d.worldSpaceNormal))

      
      #define UnityObjectToWorldNormal(normal) mul(GetObjectToWorldMatrix(), normal)

      #define _WorldSpaceLightPos0 _MainLightPosition
      
      #define UNITY_DECLARE_TEX2D(name) TEXTURE2D(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2D_NOSAMPLER(name) TEXTURE2D(name);
      #define UNITY_DECLARE_TEX2DARRAY(name) TEXTURE2D_ARRAY(name); SAMPLER(sampler##name);
      #define UNITY_DECLARE_TEX2DARRAY_NOSAMPLER(name) TEXTURE2D_ARRAY(name);

      #define UNITY_SAMPLE_TEX2DARRAY(tex,coord)            SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, coord.xy, coord.z)
      #define UNITY_SAMPLE_TEX2DARRAY_LOD(tex,coord,lod)    SAMPLE_TEXTURE2D_ARRAY_LOD(tex, sampler##tex, coord.xy, coord.z, lod)
      #define UNITY_SAMPLE_TEX2D(tex, coord)                SAMPLE_TEXTURE2D(tex, sampler##tex, coord)
      #define UNITY_SAMPLE_TEX2D_SAMPLER(tex, samp, coord)  SAMPLE_TEXTURE2D(tex, sampler##samp, coord)

      #if defined(UNITY_COMPILER_HLSL)
         #define UNITY_INITIALIZE_OUTPUT(type,name) name = (type)0;
      #else
         #define UNITY_INITIALIZE_OUTPUT(type,name)
      #endif

      #define sampler2D_float sampler2D
      #define sampler2D_half sampler2D

      

      // data across stages, stripped like the above.
      struct VertexToPixel
      {
         float4 pos : SV_POSITION;
         float3 worldPos : TEXCOORD0;
         float3 worldNormal : TEXCOORD1;
         float4 worldTangent : TEXCOORD2;
          float4 texcoord0 : TEXCCOORD3;
         #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
             float4 texcoord1 : TEXCCOORD4;
            // float4 texcoord2 : TEXCCOORD5;
         #endif
         // float4 texcoord3 : TEXCCOORD6;
         // float4 screenPos : TEXCOORD7;
         // float4 vertexColor : COLOR;

         // float4 extraV2F0 : TEXCOORD13;
         // float4 extraV2F1 : TEXCOORD14;
         // float4 extraV2F2 : TEXCOORD15;
         // float4 extraV2F3 : TEXCOORD16;
         // float4 extraV2F4 : TEXCOORD17;
         // float4 extraV2F5 : TEXCOORD18;
         // float4 extraV2F6 : TEXCOORD19;
         // float4 extraV2F7 : TEXCOORD20;

         
         #if defined(LIGHTMAP_ON)
            float2 lightmapUV : TEXCOORD8;
         #endif
         #if defined(DYNAMICLIGHTMAP_ON)
            float2 dynamicLightmapUV : TEXCOORD9;
         #endif
         #if !defined(LIGHTMAP_ON)
            float3 sh : TEXCOORD10;
         #endif

         float4 fogFactorAndVertexLight : TEXCOORD11;
         float4 shadowCoord : TEXCOORD12;

         #if UNITY_ANY_INSTANCING_ENABLED
         uint instanceID : CUSTOM_INSTANCE_ID;
         #endif
         #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
         uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
         #endif
         #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
         uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
         #endif
         #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
         FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
         #endif
      };


         // TEMPLATE_SHARED
            
            // data describing the user output of a pixel
            struct Surface
            {
               half3 Albedo;
               half Height;
               half3 Normal;
               half Smoothness;
               half3 Emission;
               half Metallic;
               half3 Specular;
               half Occlusion;
               half Alpha;
               // HDRP Only
               half SpecularOcclusion;
               half SubsurfaceMask;
               half Thickness;
               half CoatMask;
               half Anisotropy;
               half IridescenceMask;
               half IridescenceThickness;
            };

            // data the user might need, this will grow to be big. But easy to strip
            struct ShaderData
            {
               float3 localSpacePosition;
               float3 localSpaceNormal;
               float3 localSpaceTangent;
        
               float3 worldSpacePosition;
               float3 worldSpaceNormal;
               float3 worldSpaceTangent;

               float3 worldSpaceViewDir;
               float3 tangentSpaceViewDir;

               float4 texcoord0;
               float4 texcoord1;
               float4 texcoord2;
               float4 texcoord3;

               float2 screenUV;
               float4 screenPos;

               float4 vertexColor;

               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;

               float3x3 TBNMatrix;
            };

            struct VertexData
            {
               #if SHADER_TARGET > 30 && _PLANETCOMPUTE
 //              // uint vertexID : SV_VertexID;
               #endif
               float4 vertex : POSITION;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                  float4 tangent : TANGENT;
                  float4 texcoord1 : TEXCOORD1;
                  float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD4; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity    : TEXCOORD5; // Add Precomputed Velocity (Alembic computes velocities on runtime side).
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct TessVertex 
            {
               float4 vertex : INTERNALTESSPOS;
               float3 normal : NORMAL;
               float4 texcoord0 : TEXCOORD0;
               #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float4 tangent : TANGENT;
               float4 texcoord1 : TEXCOORD1;
               float4 texcoord2 : TEXCOORD2;
               #endif
               // float4 texcoord3 : TEXCOORD3;
               // float4 vertexColor : COLOR;

               
               // float4 extraV2F0 : TEXCOORD4;
               // float4 extraV2F1 : TEXCOORD5;
               // float4 extraV2F2 : TEXCOORD6;
               // float4 extraV2F3 : TEXCOORD7;
               // float4 extraV2F4 : TEXCOORD8;
               // float4 extraV2F5 : TEXCOORD9;
               // float4 extraV2F6 : TEXCOORD10;
               // float4 extraV2F7 : TEXCOORD11;

               #if _HDRP && (_PASSMOTIONVECTOR || (_PASSFORWARD && defined(_WRITE_TRANSPARENT_MOTION_VECTOR)))
                  float3 previousPositionOS : TEXCOORD12; // Contain previous transform position (in case of skinning for example)
                  #if defined (_ADD_PRECOMPUTED_VELOCITY)
                     float3 precomputedVelocity : TEXCOORD13;
                  #endif
               #endif

               UNITY_VERTEX_INPUT_INSTANCE_ID
               UNITY_VERTEX_OUTPUT_STEREO
            };

            struct ExtraV2F
            {
               float4 extraV2F0;
               float4 extraV2F1;
               float4 extraV2F2;
               float4 extraV2F3;
               float4 extraV2F4;
               float4 extraV2F5;
               float4 extraV2F6;
               float4 extraV2F7;
            };


            float3 WorldToTangentSpace(ShaderData d, float3 normal)
            {
               return mul(d.TBNMatrix, normal);
            }

            float3 TangentToWorldSpace(ShaderData d, float3 normal)
            {
               return mul(normal, d.TBNMatrix);
            }

            // in this case, make standard more like SRPs, because we can't fix
            // GetWorldToObjectMatrix() in HDRP, since it already does macro-fu there

            #if _STANDARD
               float3 TransformWorldToObject(float3 p) { return mul(GetWorldToObjectMatrix(), float4(p, 1)); };
               float3 TransformObjectToWorld(float3 p) { return mul(GetObjectToWorldMatrix(), float4(p, 1)); };
               float4 TransformWorldToObject(float4 p) { return mul(GetWorldToObjectMatrix(), p); };
               float4 TransformObjectToWorld(float4 p) { return mul(GetObjectToWorldMatrix(), p); };
               float4x4 GetWorldToObjectMatrix() { return GetWorldToObjectMatrix(); }
               float4x4 GetObjectToWorldMatrix() { return GetObjectToWorldMatrix(); }
            #endif

            float3 GetCameraWorldPosition()
            {
               #if _HDRP
                  return GetCameraRelativePositionWS(_WorldSpaceCameraPos);
               #else
                  return _WorldSpaceCameraPos;
               #endif
            }

            
             #if _HDRP

               half3 UnpackNormalmapRGorAG(half4 packednormal)
               {
                     // This do the trick
                  packednormal.x *= packednormal.w;

                  half3 normal;
                  normal.xy = packednormal.xy * 2 - 1;
                  normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                  return normal;
               }
               half3 UnpackNormal(half4 packednormal)
               {
                  #if defined(UNITY_NO_DXT5nm)
                     return packednormal.xyz * 2 - 1;
                  #else
                     return UnpackNormalmapRGorAG(packednormal);
                  #endif
               }
               #endif
               #if _HDRP || _URP

               half3 UnpackScaleNormal(half4 packednormal, half scale)
               {
                 #ifndef UNITY_NO_DXT5nm
                   // Unpack normal as DXT5nm (1, y, 1, x) or BC5 (x, y, 0, 1)
                   // Note neutral texture like "bump" is (0, 0, 1, 1) to work with both plain RGB normal and DXT5nm/BC5
                   packednormal.x *= packednormal.w;
                 #endif
                   half3 normal;
                   normal.xy = (packednormal.xy * 2 - 1) * scale;
                   normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                   return normal;
               }  

             #endif


            void GetSun(out float3 lightDir, out float3 color)
            {
               lightDir = float3(0.5, 0.5, 0);
               color = 1;
               #if _HDRP
                  if (_DirectionalLightCount > 0)
                  {
                     DirectionalLightData light = _DirectionalLightDatas[0];
                     lightDir = -light.forward.xyz;
                     color = light.color;
                  }
               #elif _STANDARD
                  lightDir = normalize(_WorldSpaceLightPos0.xyz);
                  color = _LightColor0.rgb;
               #elif _URP
                  Light light = GetMainLight();
                  lightDir = light.direction;
                  color = light.color;
               #endif
            }

     



            
         CBUFFER_START(UnityPerMaterial)

            

      #if _MESHSUBARRAY
         half4 _MeshSubArrayIndexes;
      #endif

      float4 _Diffuse_TexelSize;
      float4 _NormalSAO_TexelSize;

      #if _HYBRIDHEIGHTBLEND
         float _HybridHeightBlendDistance;
      #endif

      #if _PACKINGHQ
         float4 _SmoothAO_TexelSize;
      #endif

      #ifdef _ALPHATEST_ON
      float4 _TerrainHolesTexture_TexelSize;
      #endif

      #if _USESPECULARWORKFLOW
         float4 _Specular_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         float4 _EmissiveMetal_TexelSize;
      #endif

      #if _USEEMISSIVEMETAL
         half _EmissiveMult;
      #endif

      #if _AUTONORMAL
         half _AutoNormalHeightScale;
      #endif

      float4 _UVScale; // scale and offset

      half _Contrast;
      
      

       #if _VSSHADOWMAP
         float4 gVSSunDirection;
      #endif

      #if _FORCELOCALSPACE && _PLANETVECTORS
         float4x4 _PQSToLocal;
      #endif

      #if _ORIGINSHIFT
         float4x4 _GlobalOriginMTX;
      #endif

      float4 _Control0_TexelSize;
      #if _CUSTOMSPLATTEXTURES
         float4 _CustomControl0_TexelSize;
      #endif
      float4 _PerPixelNormal_TexelSize;

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         float2 _NoiseUVParams;
      #endif

      float4 _PerTexProps_TexelSize;

      #if _SURFACENORMALS  
         float3 surfTangent;
         float3 surfBitangent;
         float3 surfNormal;
      #endif



         CBUFFER_END

         

         

               

      #undef WorldNormalVector
      #define WorldNormalVector(data, normal) mul(normal, data.TBN)
      

      



   
// In Unity 2020.3LTS, Unity will spew tons of errors about missing this sampler in
// URP, even though it shouldn't be required.
TEXTURE2D(_MainTex);

      // globals, outside of CBuffer, but used by more than one module
      float3 _gGlitterLightDir;
      float3 _gGlitterLightWorldPos;
      half3 _gGlitterLightColor;

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
         float4    _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
         float4    _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
         float4    _TerrainNormalmapTexture_TexelSize;
      #endif

      #if (_MICROTERRAIN || _MICROMESHTERRAIN)
          TEXTURE2D(_TerrainHeightmapTexture);
          TEXTURE2D(_TerrainNormalmapTexture);
      #endif

      UNITY_INSTANCING_BUFFER_START(Terrain)
          UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
      UNITY_INSTANCING_BUFFER_END(Terrain)          


      

      // dynamic branching helpers, for regular and aggressive branching
      // debug mode shows how many samples using branching will save us. 
      //
      // These macros are always used instead of the UNITY_BRANCH macro
      // to maintain debug displays and allow branching to be disabled
      // on as granular level as we want. 
      
      #if _BRANCHSAMPLES
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++; if (w > 0)
         #else
            #define MSBRANCH(w) UNITY_BRANCH if (w > 0)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_WEIGHT || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchWeightCount;
            #define MSBRANCH(w) if (w > 0) _branchWeightCount++;
         #else
            #define MSBRANCH(w) 
         #endif
      #endif
      
      #if _BRANCHSAMPLESAGR
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER ||_DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++; if (w > 0.001)
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++; if (w > 0.001)
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++; if (w > 0.001)
         #else
            #define MSBRANCHTRIPLANAR(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHCLUSTER(w) UNITY_BRANCH if (w > 0.001)
            #define MSBRANCHOTHER(w) UNITY_BRANCH if (w > 0.001)
         #endif
      #else
         #if _DEBUG_BRANCHCOUNT_TRIPLANAR || _DEBUG_BRANCHCOUNT_CLUSTER || _DEBUG_BRANCHCOUNT_OTHER || _DEBUG_BRANCHCOUNT_TOTAL
            float _branchTriplanarCount;
            float _branchClusterCount;
            float _branchOtherCount;
            #define MSBRANCHTRIPLANAR(w) if (w > 0.001) _branchTriplanarCount++;
            #define MSBRANCHCLUSTER(w) if (w > 0.001) _branchClusterCount++;
            #define MSBRANCHOTHER(w) if (w > 0.001) _branchOtherCount++;
         #else
            #define MSBRANCHTRIPLANAR(w)
            #define MSBRANCHCLUSTER(w)
            #define MSBRANCHOTHER(w)
         #endif
      #endif

      #if _DEBUG_SAMPLECOUNT
         int _sampleCount;
         #define COUNTSAMPLE { _sampleCount++; }
      #else
         #define COUNTSAMPLE
      #endif

      #if _DEBUG_PROCLAYERS
         int _procLayerCount;
         #define COUNTPROCLAYER { _procLayerCount++; }
      #else
         #define COUNTPROCLAYER
      #endif


      #if _DEBUG_USE_TOPOLOGY
         TEXTURE2D(_DebugWorldPos);
         TEXTURE2D(_DebugWorldNormal);
      #endif
      

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);

      #if _CONTROLNOISEUV || _GLOBALNOISEUV
         TEXTURE2D(_NoiseUV);
      #endif

      #if _PACKINGHQ
         UNITY_DECLARE_TEX2DARRAY(_SmoothAO);
      #endif

      #if _USESPECULARWORKFLOW
         UNITY_DECLARE_TEX2DARRAY(_Specular);
      #endif

      #if _USEEMISSIVEMETAL
         UNITY_DECLARE_TEX2DARRAY(_EmissiveMetal);
      #endif

      TEXTURE2D(_PerPixelNormal);

      SamplerState shared_linear_clamp_sampler;
      SamplerState shared_point_clamp_sampler;
      
      TEXTURE2D(_Control0);
      #if _CUSTOMSPLATTEXTURES
         TEXTURE2D(_CustomControl0);
         #if !_MAX4TEXTURES
         TEXTURE2D(_CustomControl1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_CustomControl2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_CustomControl3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_CustomControl6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_CustomControl7);
         #endif
      #else
         #if !_MAX4TEXTURES
         TEXTURE2D(_Control1);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         TEXTURE2D(_Control2);
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         TEXTURE2D(_Control3);
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control4);
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control5);
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
         TEXTURE2D(_Control6);
         #endif
         #if _MAX32TEXTURES
         TEXTURE2D(_Control7);
         #endif
      #endif

      TEXTURE2D_FLOAT(_PerTexProps);
   
      struct DecalLayer
      {
         float3 uv;
         float2 dx;
         float2 dy;
         int decalIndex;
         bool dynamic; 
      };

      struct DecalOutput
      {
         DecalLayer l0;
         DecalLayer l1;
         DecalLayer l2;
         DecalLayer l3;
         
         half4 Weights;
         half4 Indexes;
         half4 fxLevels;
         
      };
      

      struct TriGradMipFormat
      {
         float4 d0;
         float4 d1;
         float4 d2;
      };

      float InverseLerp(float x, float y, float v) { return (v-x)/max(y-x, 0.001); }
      float2 InverseLerp(float2 x, float2 y, float2 v) { return (v-x)/max(y-x, float2(0.001, 0.001)); }
      float3 InverseLerp(float3 x, float3 y, float3 v) { return (v-x)/max(y-x, float3(0.001, 0.001, 0.001)); }
      float4 InverseLerp(float4 x, float4 y, float4 v) { return (v-x)/max(y-x, float4(0.001, 0.001, 0.001, 0.001)); }
      

      // 2019.3 holes
      #ifdef _ALPHATEST_ON
          TEXTURE2D(_TerrainHolesTexture);
          
          void ClipHoles(float2 uv)
          {
              float hole = SAMPLE_TEXTURE2D(_TerrainHolesTexture, shared_linear_clamp_sampler, uv).r;
              COUNTSAMPLE
              clip(hole < 0.5f ? -1 : 1);
          }
      #endif

      
      #if _TRIPLANAR
         #if _USEGRADMIP
            #define MIPFORMAT TriGradMipFormat
            #define INITMIPFORMAT (TriGradMipFormat)0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float3
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float3
         #endif
      #else
         #if _USEGRADMIP
            #define MIPFORMAT float4
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float4
         #else
            #define MIPFORMAT float
            #define INITMIPFORMAT 0;
            #define MIPFROMATRAW float
         #endif
      #endif

      float2 TotalOne(float2 v) { return v * (1.0 / max(v.x + v.y, 0.001)); }
      float3 TotalOne(float3 v) { return v * (1.0 / max(v.x + v.y + v.z, 0.001)); }
      float4 TotalOne(float4 v) { return v * (1.0 / max(v.x + v.y + v.z + v.w, 0.001)); }

      float2 RotateUV(float2 uv, float amt)
      {
         uv -=0.5;
         float s = sin ( amt);
         float c = cos ( amt );
         float2x2 mtx = float2x2( c, -s, s, c);
         mtx *= 0.5;
         mtx += 0.5;
         mtx = mtx * 2-1;
         uv = mul ( uv, mtx );
         uv += 0.5;
         return uv;
      }

      float4 DecodeToFloat4(float v)
      {
         uint vi = (uint)(v * (256.0f * 256.0f * 256.0f * 256.0f));
         int ex = (int)(vi / (256 * 256 * 256) % 256);
         int ey = (int)((vi / (256 * 256)) % 256);
         int ez = (int)((vi / (256)) % 256);
         int ew = (int)(vi % 256);
         float4 e = float4(ex / 255.0, ey / 255.0, ez / 255.0, ew / 255.0);
         return e;
      }

      

      struct Input 
      {
         ShaderData shaderData;
         float2 uv_Control0;
         float2 uv2_Diffuse;

         float worldHeight;
         float3 worldUpVector;

         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         float4 color;
         float3x3 TBN;

         // vertex/digger workflow data
         half4 w0;
         half4 w1;
         half4 w2;
         half4 w3;
         half4 w4;
         half4 w5;
         half4 w6;
         
         // megasplat data
         half4 layer0;
         half4 layer1;
         half3 baryWeights;
         half4 scatter0;
         half4 scatter1;

         // wetness, puddles, streams, lava from vertex or megasplat
         half4 fx;
         // snow min, snow max
         half4 fx2;


      };
      
      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
         half3 pN0;
         half3 pN1;
         half3 pN2;
         half3 pN3;
         half3 axisSign;
         Input IN;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;

         half4 cluster0;
         half4 cluster1;
         half4 cluster2;
         half4 cluster3;

      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
         #if _USESPECULARWORKFLOW
         half3 Specular;
         #endif
         half Alpha;
         
      };


      

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         #if _SURFACENORMALS
            half3 surf0;
            half3 surf1;
            half3 surf2;
            half3 surf3;
         #endif

         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
         

         #if _USEEMISSIVEMETAL || _GLOBALEMIS || _GLOBALSMOOTHAOMETAL || _PERTEXSSS || _PERTEXRIMLIGHT
            half4 emisMetal0;
            half4 emisMetal1;
            half4 emisMetal2;
            half4 emisMetal3;
         #endif

         #if _USESPECULARWORKFLOW
            half3 specular0;
            half3 specular1;
            half3 specular2;
            half3 specular3;
         #endif
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
         #if _SURFACENORMALS
            s.surf0 = half3(0,0,1);
            s.surf1 = half3(0,0,1);
            s.surf2 = half3(0,0,1);
            s.surf3 = half3(0,0,1);
         #endif
      }

       float3 GetGlobalLightDir(Input i)
      {
         float3 lightDir = float3(1,0,0);

         #if _HDRP || PASS_DEFERRED
            lightDir = normalize(_gGlitterLightDir.xyz);
         #elif _URP
            lightDir = GetMainLight().direction;
         #else
            #ifndef USING_DIRECTIONAL_LIGHT
               lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            #else
               lightDir = normalize(_WorldSpaceLightPos0.xyz);
            #endif
         #endif
         return lightDir;
      }

      float3x3 GetTBN(Input i)
      {
         return i.TBN;
      }
      
      float3 GetGlobalLightDirTS(Input i)
      {
         float3 lightDirWS = GetGlobalLightDir(i);
         return mul(GetTBN(i), lightDirWS);
      }
      
      half3 GetGlobalLightColor()
      {
         #if _HDRP || PASS_DEFERRED
            return _gGlitterLightColor;
         #elif _URP
            return (GetMainLight().color);
         #else
            return _LightColor0.rgb;
         #endif
      }

      

      half3 FuzzyShade(half3 color, half3 normal, half coreMult, half edgeMult, half power, float3 viewDir)
      {
         half dt = saturate(dot(viewDir, normal));
         half dark = 1.0 - (coreMult * dt);
         half edge = pow(1-dt, power) * edgeMult;
         return color * (dark + edge);
      }

      half3 ComputeSSS(Input i, float3 V, float3 N, half3 tint, half thickness, half distortion, half scale, half power)
      {
         float3 L = GetGlobalLightDir(i);
         half3 lightColor = GetGlobalLightColor();
         float3 H = normalize(L + N * distortion);
         float VdotH = pow(saturate(dot(V, -H)), power) * scale;
         float3 I =  (VdotH) * thickness;
         return lightColor * I * tint;
      }


      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif
      

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv0.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##1 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv1.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##2 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv2.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \
            half4 varName##3 = SAMPLE_TEXTURE2D_LOD(_PerTexProps, shared_point_clamp_sampler, float2(config.uv3.z*_PerTexProps_TexelSize.x, pixel*_PerTexProps_TexelSize.y), 0); \

      #endif

      half2 BlendNormal2(half2 base, half2 blend) { return normalize(half3(base.xy + blend.xy, 1)).xy; } 
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }
      half3 BlendLighterColor(half3 s, half3 d) { return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d; } 
      
      
      #if _SURFACENORMALS  

      #define HALF_EPS 4.8828125e-4    // 2^-11, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)

      

      void ConstructSurfaceGradientTBN(Input i)
      {
         float3x3 tbn = GetTBN(i);
         float3 t = tbn[0];
         float3 b = tbn[1];
         float3 n = tbn[2];

         surfNormal = n;//mul(GetWorldToObjectMatrix(), float4(n, 1)).xyz;
         surfTangent = t;//mul(GetWorldToObjectMatrix(), float4(t, 1)).xyz;
         surfBitangent = b;//cross(surfNormal, surfTangent);
         
         float renormFactor = 1.0 / length(surfNormal);
         surfNormal    *= renormFactor;
         surfTangent   *= renormFactor;
         surfBitangent *= renormFactor;
      }
      
      half3 SurfaceGradientFromTBN(half2 deriv)
      {
          return deriv.x * surfTangent + deriv.y * surfBitangent;
      }

      // Input: vM is tangent space normal in [-1;1].
      // Output: convert vM to a derivative.
      half2 TspaceNormalToDerivative(half3 vM)
      {
         const half scale = 1.0/128.0;
         
         // Ensure vM delivers a positive third component using abs() and
         // constrain vM.z so the range of the derivative is [-128; 128].
         const half3 vMa = abs(vM);
         const half z_ma = max(vMa.z, scale*max(vMa.x, vMa.y));

         return -half2(vM.x, vM.y)/z_ma;
      }

      // Used to produce a surface gradient from the gradient of a volume
      // bump function such as 3D Perlin noise. Equation 2 in [Mik10].
      half3 SurfgradFromVolumeGradient(half3 grad)
      {
         return grad - dot(surfNormal, grad) * surfNormal;
      }

      half3 SurfgradFromTriplanarProjection(half3 pN, half2 xPlaneTN, half2 yPlaneTN, half2 zPlaneTN)
      {
         const half w0 = pN.x;
         const half w1 = pN.y;
         const half w2 = pN.z;
         
         // X-plane tangent normal to gradient derivative
         xPlaneTN = xPlaneTN * 2.0 - 1.0;
         half xPlaneRcpZ = rsqrt(max(1 - dot(xPlaneTN.x, xPlaneTN.x) - dot(xPlaneTN.y, xPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_xplane = xPlaneTN * -xPlaneRcpZ;

         // Y-plane tangent normal to gradient derivative
         yPlaneTN = yPlaneTN * 2.0 - 1.0;
         half yPlaneRcpZ = rsqrt(max(1 - dot(yPlaneTN.x, yPlaneTN.x) - dot(yPlaneTN.y, yPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_yplane = yPlaneTN * -yPlaneRcpZ;

         // Z-plane tangent normal to gradient derivative
         zPlaneTN = zPlaneTN * 2.0 - 1.0;
         half zPlaneRcpZ = rsqrt(max(1 - dot(zPlaneTN.x, zPlaneTN.x) - dot(zPlaneTN.y, zPlaneTN.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
      
         half2 d_zplane = zPlaneTN * -zPlaneRcpZ;

         // Assume deriv xplane, deriv yplane, and deriv zplane are
         // sampled using (z,y), (x,z), and (x,y), respectively.
         // Positive scales of the lookup coordinate will work
         // as well, but for negative scales the derivative components
         // will need to be negated accordingly.
         float3 grad = float3(w2*d_zplane.x + w1*d_yplane.x,
                              w2*d_zplane.y + w0*d_xplane.y,
                              w0*d_xplane.x + w1*d_yplane.y);

         return SurfgradFromVolumeGradient(grad);
      }

      half3 ConvertNormalToGradient(half3 normal)
      {
         half2 deriv = TspaceNormalToDerivative(normal);

         return SurfaceGradientFromTBN(deriv);
      }

      half3 ConvertNormal2ToGradient(half2 packedNormal)
      {
         half2 tNormal = packedNormal;
         half rcpZ = rsqrt(max(1 - dot(tNormal.x, tNormal.x) - dot(tNormal.y, tNormal.y), dot(HALF_EPS, HALF_EPS))); // Clamp to avoid INF
         half2 deriv = tNormal * -rcpZ;
         return SurfaceGradientFromTBN(deriv);
      }


      half3 ResolveNormalFromSurfaceGradient(half3 gradient)
      {
         return normalize(surfNormal - gradient);
      }
      

      #endif // _SURFACENORMALS

      void BlendNormalPerTex(inout RawSamples o, half2 noise, float4 fades)
      {
         #if _SURFACENORMALS
            float3 grad = ConvertNormal2ToGradient(noise.xy);
            o.surf0 += grad * fades.x;
            o.surf1 += grad * fades.y;
            #if !_MAX2LAYER
               o.surf2 += grad * fades.z;
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.surf3 += grad * fades.w;
            #endif
         #else
            o.normSAO0.xy = lerp(o.normSAO0.xy, BlendNormal2(o.normSAO0.xy, noise.xy), fades.x);
            o.normSAO1.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #if !_MAX2LAYER
               o.normSAO2.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO2.xy, noise.xy), fades.y);
            #endif
            #if !_MAX2LAYER && !_MAX3LAYER
               o.normSAO3.xy = lerp(o.normSAO1.xy, BlendNormal2(o.normSAO1.xy, noise.xy), fades.y);
            #endif
         #endif
      }
      
     
      
      half3 BlendNormal3(half3 n1, half3 n2)
      {
         n1 += float3( 0,  0, 1);
         n2 *= float3(-1, -1, 1);
         return n1*dot(n1, n2) / n1.z - n2;
      }
      
      half2 TransformTriplanarNormal(Input IN, float3x3 t2w, half3 axisSign, half3 absVertNormal,
               half3 pN, half2 a0, half2 a1, half2 a2)
      {
         
         a0 = a0 * 2 - 1;
         a1 = a1 * 2 - 1;
         a2 = a2 * 2 - 1;

         a0.x *= axisSign.x;
         a1.x *= axisSign.y;
         a2.x *= axisSign.z;
         
         half3 n0 = half3(a0.xy, 1);
         half3 n1 = half3(a1.xy, 1);
         half3 n2 = half3(a2.xy, 1);

         float3 wn = IN.worldNormal;

         n0 = BlendNormal3(half3(wn.zy, absVertNormal.x), n0);
         n1 = BlendNormal3(half3(wn.xz, absVertNormal.y), n1 * float3(-1, 1, 1)); 
         n2 = BlendNormal3(half3(wn.xy, absVertNormal.z), n2);
  
         n0.z *= axisSign.x;
         n1.z *= axisSign.y;
         n2.z *= -axisSign.z;

         half3 worldNormal = (n0.zyx * pN.x + n1.xzy * pN.y + n2.xyz * pN.z);
         return mul(t2w, worldNormal).xy;

      }
      
      // funcs
      
      inline half MSLuminance(half3 rgb)
      {
         #ifdef UNITY_COLORSPACE_GAMMA
            return dot(rgb, half3(0.22, 0.707, 0.071));
         #else
            return dot(rgb, half3(0.0396819152, 0.458021790, 0.00609653955));
         #endif
      }
      
      
      float2 Hash2D( float2 x )
      {
          float2 k = float2( 0.3183099, 0.3678794 );
          x = x*k + k.yx;
          return -1.0 + 2.0*frac( 16.0 * k*frac( x.x*x.y*(x.x+x.y)) );
      }

      float Noise2D(float2 p )
      {
         float2 i = floor( p );
         float2 f = frac( p );
         
         float2 u = f*f*(3.0-2.0*f);

         return lerp( lerp( dot( Hash2D( i + float2(0.0,0.0) ), f - float2(0.0,0.0) ), 
                           dot( Hash2D( i + float2(1.0,0.0) ), f - float2(1.0,0.0) ), u.x),
                      lerp( dot( Hash2D( i + float2(0.0,1.0) ), f - float2(0.0,1.0) ), 
                           dot( Hash2D( i + float2(1.0,1.0) ), f - float2(1.0,1.0) ), u.x), u.y);
      }
      
      float FBM2D(float2 uv)
      {
         float f = 0.5000*Noise2D( uv ); uv *= 2.01;
         f += 0.2500*Noise2D( uv ); uv *= 1.96;
         f += 0.1250*Noise2D( uv ); 
         return f;
      }
      
      float3 Hash3D( float3 p )
      {
         p = float3( dot(p,float3(127.1,311.7, 74.7)),
                 dot(p,float3(269.5,183.3,246.1)),
                 dot(p,float3(113.5,271.9,124.6)));

         return -1.0 + 2.0*frac(sin(p)*437.5453123);
      }

      float Noise3D( float3 p )
      {
         float3 i = floor( p );
         float3 f = frac( p );
         
         float3 u = f*f*(3.0-2.0*f);

         return lerp( lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,0.0) ), f - float3(0.0,0.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,0.0) ), f - float3(1.0,0.0,0.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,0.0) ), f - float3(0.0,1.0,0.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,0.0) ), f - float3(1.0,1.0,0.0) ), u.x), u.y),
                      lerp( lerp( dot( Hash3D( i + float3(0.0,0.0,1.0) ), f - float3(0.0,0.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,0.0,1.0) ), f - float3(1.0,0.0,1.0) ), u.x),
                           lerp( dot( Hash3D( i + float3(0.0,1.0,1.0) ), f - float3(0.0,1.0,1.0) ), 
                                dot( Hash3D( i + float3(1.0,1.0,1.0) ), f - float3(1.0,1.0,1.0) ), u.x), u.y), u.z );
      }
      
      float FBM3D(float3 uv)
      {
         float f = 0.5000*Noise3D( uv ); uv *= 2.01;
         f += 0.2500*Noise3D( uv ); uv *= 1.96;
         f += 0.1250*Noise3D( uv ); 
         return f;
      }
      
     
      
      float GetSaturation(float3 c)
      {
         float mi = min(min(c.x, c.y), c.z);
         float ma = max(max(c.x, c.y), c.z);
         return (ma - mi)/(ma + 1e-7);
      }

      // Better Color Lerp, does not have darkening issue
      float3 BetterColorLerp(float3 a, float3 b, float x)
      {
         float3 ic = lerp(a, b, x) + float3(1e-6,0.0,0.0);
         float sd = abs(GetSaturation(ic) - lerp(GetSaturation(a), GetSaturation(b), x));
    
         float3 dir = normalize(float3(2.0 * ic.x - ic.y - ic.z, 2.0 * ic.y - ic.x - ic.z, 2.0 * ic.z - ic.y - ic.x));
         float lgt = dot(float3(1.0, 1.0, 1.0), ic);
    
         float ff = dot(dir, normalize(ic));
    
         const float dsp_str = 1.5;
         ic += dsp_str * dir * sd * ff * lgt;
         return saturate(ic);
      }
      
      
      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);
             half4 weights = half4(iWeights.x * max(h0,0.001), iWeights.y * max(h1,0.001), iWeights.z * max(h2,0.001), iWeights.w * max(h3,0.001));
             
             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);

             weights = TotalOne(weights);
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #elif _MAX20TEXTURES
         #define TEXCOUNT 20
      #elif _MAX24TEXTURES
         #define TEXCOUNT 24
      #elif _MAX28TEXTURES
         #define TEXCOUNT 28
      #elif _MAX32TEXTURES
         #define TEXCOUNT 32
      #else
         #define TEXCOUNT 16
      #endif

      #if _DECAL_SPLAT
      
      void DoMergeDecalSplats(half4 iWeights, half4 iIndexes, inout half4 indexes, inout half4 weights)
      {
         for (int i = 0; i < 4; ++i)
         {
            half w = iWeights[i];
            half index = iIndexes[i];
            if (w > weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = index;
            }
            else if (w > weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = index;
            }
            else if (w > weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = index;
            }
            else if (w > weights[3])
            {
               weights[3] = w;
               indexes[3] = index;
            }
         }

      }
      #endif


      void Setup(out half4 weights, float2 uv, out Config config, half4 w0, half4 w1, half4 w2, half4 w3, half4 w4, half4 w5, half4 w6, half4 w7, float3 worldPos, DecalOutput decalOutput)
      {
         config = (Config)0;
         half4 indexes = 0;

         config.uv = uv;

         #if _WORLDUV
         uv = worldPos.xz;
         #endif

         #if _DISABLESPLATMAPS
            float2 scaledUV = uv;
         #else
            float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;
         #endif

         // if only 4 textures, and blending 4 textures, skip this whole thing..
         // this saves about 25% of the ALU of the base shader on low end. However if
         // we rely on sorted texture weights (distance resampling) we have to sort..
         float4 defaultIndexes = float4(0,1,2,3);
         #if _MESHSUBARRAY
            defaultIndexes = _MeshSubArrayIndexes;
         #endif

         #if _MESHSUBARRAY && !_DECAL_SPLAT || (_MAX4TEXTURES && !_MAX3LAYER && !_MAX2LAYER && !_DISTANCERESAMPLE && !_POM && !_DECAL_SPLAT)
            weights = w0;
            config.uv0 = float3(scaledUV, defaultIndexes.x);
            config.uv1 = float3(scaledUV, defaultIndexes.y);
            config.uv2 = float3(scaledUV, defaultIndexes.z);
            config.uv3 = float3(scaledUV, defaultIndexes.w);
            return;
         #endif

         #if _DISABLESPLATMAPS
            weights = float4(1,0,0,0);
            return;
         #else
            half splats[TEXCOUNT];

            splats[0] = w0.x;
            splats[1] = w0.y;
            splats[2] = w0.z;
            splats[3] = w0.w;
            #if !_MAX4TEXTURES
               splats[4] = w1.x;
               splats[5] = w1.y;
               splats[6] = w1.z;
               splats[7] = w1.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES
               splats[8] = w2.x;
               splats[9] = w2.y;
               splats[10] = w2.z;
               splats[11] = w2.w;
            #endif
            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
               splats[12] = w3.x;
               splats[13] = w3.y;
               splats[14] = w3.z;
               splats[15] = w3.w;
            #endif
            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[16] = w4.x;
               splats[17] = w4.y;
               splats[18] = w4.z;
               splats[19] = w4.w;
            #endif
            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
               splats[20] = w5.x;
               splats[21] = w5.y;
               splats[22] = w5.z;
               splats[23] = w5.w;
            #endif
            #if _MAX28TEXTURES || _MAX32TEXTURES
               splats[24] = w6.x;
               splats[25] = w6.y;
               splats[26] = w6.z;
               splats[27] = w6.w;
            #endif
            #if _MAX32TEXTURES
               splats[28] = w7.x;
               splats[29] = w7.y;
               splats[30] = w7.z;
               splats[31] = w7.w;
            #endif



            weights[0] = 0;
            weights[1] = 0;
            weights[2] = 0;
            weights[3] = 0;
            indexes[0] = 0;
            indexes[1] = 0;
            indexes[2] = 0;
            indexes[3] = 0;

            int i = 0;
            for (i = 0; i < TEXCOUNT; ++i)
            {
               half w = splats[i];
               if (w >= weights[0])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = weights[0];
                  indexes[1] = indexes[0];
                  weights[0] = w;
                  indexes[0] = i;
               }
               else if (w >= weights[1])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = weights[1];
                  indexes[2] = indexes[1];
                  weights[1] = w;
                  indexes[1] = i;
               }
               else if (w >= weights[2])
               {
                  weights[3] = weights[2];
                  indexes[3] = indexes[2];
                  weights[2] = w;
                  indexes[2] = i;
               }
               else if (w >= weights[3])
               {
                  weights[3] = w;
                  indexes[3] = i;
               }
            }

            #if _DECAL_SPLAT
               DoMergeDecalSplats(decalOutput.Weights, decalOutput.Indexes, weights, indexes); 
            #endif


            
            
            // clamp and renormalize
            #if _MAX2LAYER
               weights.zw = 0;
               weights.xy = TotalOne(weights.xy);
            #elif _MAX3LAYER
               weights.w = 0;
               weights.xyz = TotalOne(weights.xyz);
            #elif !_DISABLEHEIGHTBLENDING || _NORMALIZEWEIGHTS // prevents black when painting, which the unity shader does not prevent.
               weights = normalize(weights);
            #endif
            

            config.uv0 = float3(scaledUV, indexes.x);
            config.uv1 = float3(scaledUV, indexes.y);
            config.uv2 = float3(scaledUV, indexes.z);
            config.uv3 = float3(scaledUV, indexes.w);


         #endif //_DISABLESPLATMAPS


      }

      float3 HeightToNormal(float height, float3 worldPos)
      {
         float3 dx = ddx(worldPos);
         float3 dy = ddy(worldPos);
         float3 crossX = cross(float3(0,1,0), dx);
         float3 crossY = cross(float3(0,1,0), dy);
         float3 d = abs(dot(crossY, dx));
         float3 n = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
         n.z *= -1;
         return normalize((d * float3(0,1,0)) - n).xzy;
      }
      
      float ComputeMipLevel(float2 uv, float2 textureSize)
      {
         uv *= textureSize;
         float2  dx_vtc        = ddx(uv);
         float2  dy_vtc        = ddy(uv);
         float delta_max_sqr   = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
         return 0.5 * log2(delta_max_sqr);
      }

      inline half2 UnpackNormal2(half4 packednormal)
      {
          return packednormal.wy * 2 - 1;
         
      }

      half3 TriplanarHBlend(half h0, half h1, half h2, half3 pN, half contrast)
      {
         half3 blend = pN / dot(pN, half3(1,1,1));
         float3 heights = float3(h0, h1, h2) + (blend * 3.0);
         half height_start = max(max(heights.x, heights.y), heights.z) - contrast;
         half3 h = max(heights - height_start.xxx, half3(0,0,0));
         blend = h / dot(h, half3(1,1,1));
         return blend;
      }
      

      void ClearAllButAlbedo(inout MicroSplatLayer o, half3 display)
      {
         o.Albedo = display.rgb;
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

      void ClearAllButAlbedo(inout MicroSplatLayer o, half display)
      {
         o.Albedo = half3(display, display, display);
         o.Normal = half3(0, 0, 1);
         o.Smoothness = 0;
         o.Occlusion = 1;
         o.Emission = 0;
         o.Metallic = 0;
         o.Height = 0;
         #if _USESPECULARWORKFLOW
         o.Specular = 0;
         #endif

      }

     

      half MicroShadow(float3 lightDir, half3 normal, half ao, half strength)
      {
         half shadow = saturate(abs(dot(normal, lightDir)) + (ao * ao * 2.0) - 1.0);
         return 1 - ((1-shadow) * strength);
      }
      

      void DoDebugOutput(inout MicroSplatLayer l)
      {
         #if _DEBUG_OUTPUT_ALBEDO
            ClearAllButAlbedo(l, l.Albedo);
         #elif _DEBUG_OUTPUT_NORMAL
            // oh unit shader compiler normal stripping, how I hate you so..
            // must multiply by albedo to stop the normal from being white. Why, fuck knows?
            ClearAllButAlbedo(l, float3(l.Normal.xy * 0.5 + 0.5, l.Normal.z * saturate(l.Albedo.z+1)));
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            ClearAllButAlbedo(l, l.Smoothness.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_METAL
            ClearAllButAlbedo(l, l.Metallic.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_AO
            ClearAllButAlbedo(l, l.Occlusion.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_EMISSION
            ClearAllButAlbedo(l, l.Emission * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_HEIGHT
            ClearAllButAlbedo(l, l.Height.xxx * saturate(l.Albedo.z+1));
         #elif _DEBUG_OUTPUT_SPECULAR && _USESPECULARWORKFLOW
            ClearAllButAlbedo(l, l.Specular * saturate(l.Albedo.z+1));
         #elif _DEBUG_BRANCHCOUNT_WEIGHT
            ClearAllButAlbedo(l, _branchWeightCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TRIPLANAR
            ClearAllButAlbedo(l, _branchTriplanarCount / 24 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_CLUSTER
            ClearAllButAlbedo(l, _branchClusterCount / 12 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_OTHER
            ClearAllButAlbedo(l, _branchOtherCount / 8 * saturate(l.Albedo.z + 1));
         #elif _DEBUG_BRANCHCOUNT_TOTAL
            l.Albedo.r = _branchWeightCount / 12;
            l.Albedo.g = _branchTriplanarCount / 24;
            l.Albedo.b = _branchClusterCount / 12;
            ClearAllButAlbedo(l, (l.Albedo.r + l.Albedo.g + l.Albedo.b + (_branchOtherCount / 8)) / 4); 
         #elif _DEBUG_OUTPUT_MICROSHADOWS
            ClearAllButAlbedo(l,l.Albedo); 
         #elif _DEBUG_SAMPLECOUNT
            float sdisp = (float)_sampleCount / max(_SampleCountDiv, 1);
            half3 sdcolor = float3(sdisp, sdisp > 1 ? 1 : 0, 0);
            ClearAllButAlbedo(l, sdcolor * saturate(l.Albedo.z + 1));
         #elif _DEBUG_PROCLAYERS
            ClearAllButAlbedo(l, (float)_procLayerCount / (float)_PCLayerCount * saturate(l.Albedo.z + 1));
         #endif
      }


      // abstraction around sampler mode
      #if _USELODMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_LOD(tex, sampler##tex, u, l.x)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u, l.x)
      #elif _USEGRADMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_GRAD(tex, sampler##tex, u, l.xy, l.zw)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY_GRAD(tex, ss, u.xy, u.z, l.xy, l.zw)
      #else
         #define MICROSPLAT_SAMPLE(tex, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, sampler##tex, u.xy, u.z)
         #define MICROSPLAT_SAMPLE_SAMPLER(tex, ss, u, l) SAMPLE_TEXTURE2D_ARRAY(tex, ss, u.xy, y.z)
      #endif


      #define MICROSPLAT_SAMPLE_DIFFUSE(u, cl, l) MICROSPLAT_SAMPLE(_Diffuse, u, l)
      #define MICROSPLAT_SAMPLE_EMIS(u, cl, l) MICROSPLAT_SAMPLE(_EmissiveMetal, u, l)
      #define MICROSPLAT_SAMPLE_DIFFUSE_LOD(u, cl, l) UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, u, l)
      

      #if _PACKINGHQ
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) half4(MICROSPLAT_SAMPLE(_NormalSAO, u, l).ga, MICROSPLAT_SAMPLE(_SmoothAO, u, l).ga).brag
      #else
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) MICROSPLAT_SAMPLE(_NormalSAO, u, l)
      #endif

      #if _USESPECULARWORKFLOW
         #define MICROSPLAT_SAMPLE_SPECULAR(u, cl, l) MICROSPLAT_SAMPLE(_Specular, u, l)
      #endif
      
      struct SimpleTriplanarConfig
      {
         float3 pn;
         float2 uv0;
         float2 uv1;
         float2 uv2;
      };
         
      void PrepSimpleTriplanarConfig(inout SimpleTriplanarConfig tc, float3 worldPos, float3 normal, float contrast)
      {
         tc.pn = pow(abs(normal), contrast);
         tc.pn = tc.pn / (tc.pn.x + tc.pn.y + tc.pn.z);
         
         half3 axisSign = sign(normal);

         tc.uv0 = worldPos.zy * axisSign.x;
         tc.uv1 = worldPos.xz * axisSign.y;
         tc.uv2 = worldPos.xy * axisSign.z;
      }
      
      #define SimpleTriplanarSample(tex, tc, scale) (SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv0 * scale) * tc.pn.x + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv1 * scale) * tc.pn.y + SAMPLE_TEXTURE2D(tex, sampler_Diffuse, tc.uv2 * scale) * tc.pn.z)
      #define SimpleTriplanarSampleLOD(tex, tc, scale, lod) (SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv0 * scale, lod) * tc.pn.x + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv1 * scale, lod) * tc.pn.y + SAMPLE_TEXTURE2D_LOD(tex, sampler_Diffuse, tc.uv2 * scale, lod) * tc.pn.z)
      #define SimpleTriplanarSampleGrad(tex, tc, scale) (SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv0 * scale, ddx(tc.uv0) * scale, ddy(tc.uv0) * scale) * tc.pn.x + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv1 * scale, ddx(tc.uv1) * scale, ddy(tc.uv1) * scale) * tc.pn.y + SAMPLE_TEXTURE2D_GRAD(tex, sampler_Diffuse, tc.uv2 * scale, ddx(tc.uv2) * scale, ddy(tc.uv2) * scale) * tc.pn.z)
   
      
      inline half3 MicroSplatDiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
      {
          specColor = lerp (half3(0,0,0), albedo, metallic);
          oneMinusReflectivity = (1-metallic);
          return albedo * oneMinusReflectivity;
      }







      Input DescToInput(ShaderData IN)
      {
        Input s = (Input)0;
        s.shaderData = IN;
        s.TBN = IN.TBNMatrix;
        s.worldNormal = IN.worldSpaceNormal;
        s.worldPos = IN.worldSpacePosition;
        s.viewDir = IN.tangentSpaceViewDir;
        s.uv_Control0 = IN.texcoord0.xy;

        s.worldUpVector = float3(0,1,0);
        s.worldHeight = IN.worldSpacePosition.y;
  
        #if _PLANETVECTORS
            float3 rwp = mul(_PQSToLocal, float4(IN.worldSpacePosition, 1));
            s.worldHeight = distance(rwp, float3(0,0,0));
            s.worldUpVector = normalize(rwp);
        #endif

        #if _MICROMESH && _MESHUV2
            s.uv2_Diffuse = IN.texcoord1.xy;
        #endif

        #if _MEGASPLAT
           UnpackMegaSplat(s, IN);
        #endif
   
        #if _MICROVERTEXMESH || _MICRODIGGERMESH
            UnpackVertexWorkflow(s, IN);
        #endif

        #if _PLANETVECTORS
           DoPlanetDataInputCopy(s, IN);
        #endif
        
        return s;
     }
     


      void SampleAlbedo(inout Config config, inout TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
         
            half4 contrasts = _Contrast.xxxx;
            #if _PERTEXTRIPLANARCONTRAST
               SAMPLE_PER_TEX(ptc, 9.5, config, half4(1,0.5,0,0));
               contrasts = half4(ptc0.y, ptc1.y, ptc2.y, ptc3.y);
            #endif


            #if _PERTEXTRIPLANAR
               SAMPLE_PER_TEX(pttri, 9.5, config, half4(0,0,0,0));
            #endif

            {
               // For per-texture triplanar, we modify the view based blending factor of the triplanar
               // such that you get a pure blend of either top down projection, or with the top down projection
               // removed and renormalized. This causes dynamic flow control optimizations to kick in and avoid
               // the extra texture samples while keeping the code simple. Yay..

               // We also only have to do this in the Albedo, because the pN values will be adjusted after the
               // albedo is sampled, causing future samples to use this data. 
              
               #if _PERTEXTRIPLANAR
                  if (pttri0.x > 0.66)
                  {
                     tc.pN0 = half3(0,1,0);
                  }
                  else if (pttri0.x > 0.33)
                  {
                     tc.pN0.y = 0;
                     tc.pN0.xz = TotalOne(tc.pN0.xz);
                  }
               #endif


               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[0], config.cluster0, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[1], config.cluster0, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[2], config.cluster0, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN0;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN0, contrasts.x);
                  tc.pN0 = bf;
               #endif

               s.albedo0 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            MSBRANCH(weights.y)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri1.x > 0.66)
                  {
                     tc.pN1 = half3(0,1,0);
                  }
                  else if (pttri1.x > 0.33)
                  {
                     tc.pN1.y = 0;
                     tc.pN1.xz = TotalOne(tc.pN1.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[0], config.cluster1, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[1], config.cluster1, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  COUNTSAMPLE
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[2], config.cluster1, d2);
               }
               half3 bf = tc.pN1;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN1, contrasts.x);
                  tc.pN1 = bf;
               #endif


               s.albedo1 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               #if _PERTEXTRIPLANAR
                  if (pttri2.x > 0.66)
                  {
                     tc.pN2 = half3(0,1,0);
                  }
                  else if (pttri2.x > 0.33)
                  {
                     tc.pN2.y = 0;
                     tc.pN2.xz = TotalOne(tc.pN2.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[0], config.cluster2, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[1], config.cluster2, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[2], config.cluster2, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN2;
               #if _TRIPLANARHEIGHTBLEND
                  bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN2, contrasts.x);
                  tc.pN2 = bf;
               #endif
               

               s.albedo2 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {

               #if _PERTEXTRIPLANAR
                  if (pttri3.x > 0.66)
                  {
                     tc.pN3 = half3(0,1,0);
                  }
                  else if (pttri3.x > 0.33)
                  {
                     tc.pN3.y = 0;
                     tc.pN3.xz = TotalOne(tc.pN3.xz);
                  }
               #endif

               half4 a0 = half4(0,0,0,0);
               half4 a1 = half4(0,0,0,0);
               half4 a2 = half4(0,0,0,0);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[0], config.cluster3, d0);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[1], config.cluster3, d1);
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[2], config.cluster3, d2);
                  COUNTSAMPLE
               }

               half3 bf = tc.pN3;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN3, contrasts.x);
               tc.pN3 = bf;
               #endif

               s.albedo3 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif

         #else
            s.albedo0 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv0, config.cluster0, mipLevel);
            COUNTSAMPLE

            MSBRANCH(weights.y)
            {
               s.albedo1 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv1, config.cluster1, mipLevel);
               COUNTSAMPLE
            }
            #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.albedo2 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               } 
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.albedo3 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
            #endif
         #endif

         #if _PERTEXHEIGHTOFFSET || _PERTEXHEIGHTCONTRAST
            SAMPLE_PER_TEX(ptHeight, 10.5, config, 1);

            #if _PERTEXHEIGHTOFFSET
               s.albedo0.a = saturate(s.albedo0.a + ptHeight0.b - 1);
               s.albedo1.a = saturate(s.albedo1.a + ptHeight1.b - 1);
               s.albedo2.a = saturate(s.albedo2.a + ptHeight2.b - 1);
               s.albedo3.a = saturate(s.albedo3.a + ptHeight3.b - 1);
            #endif
            #if _PERTEXHEIGHTCONTRAST
               s.albedo0.a = saturate(pow(s.albedo0.a + 0.5, abs(ptHeight0.a)) - 0.5);
               s.albedo1.a = saturate(pow(s.albedo1.a + 0.5, abs(ptHeight1.a)) - 0.5);
               s.albedo2.a = saturate(pow(s.albedo2.a + 0.5, abs(ptHeight2.a)) - 0.5);
               s.albedo3.a = saturate(pow(s.albedo3.a + 0.5, abs(ptHeight3.a)) - 0.5);
            #endif
         #endif
      }
      
      
      
      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
         return;
         #endif

         #if _NONORMALMAP || _AUTONORMAL
            s.normSAO0 = half4(0,0, 0, 1);
            s.normSAO1 = half4(0,0, 0, 1);
            s.normSAO2 = half4(0,0, 0, 1);
            s.normSAO3 = half4(0,0, 0, 1);
            return;
         #endif

         
         #if _TRIPLANAR
            #if _USEGRADMIP
               float4 d0 = mipLevel.d0;
               float4 d1 = mipLevel.d1;
               float4 d2 = mipLevel.d2;
            #elif _USELODMIP
               float d0 = mipLevel.x;
               float d1 = mipLevel.y;
               float d2 = mipLevel.z;
            #else
               MIPFORMAT d0 = mipLevel;
               MIPFORMAT d1 = mipLevel;
               MIPFORMAT d2 = mipLevel;
            #endif
            
            half3 absVertNormal = abs(tc.IN.worldNormal);
            float3x3 t2w = tc.IN.TBN;
            
            
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN0.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[0], config.cluster0, d0).agrb;
                  COUNTSAMPLE
               }            
               MSBRANCHTRIPLANAR(tc.pN0.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[1], config.cluster0, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN0.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[2], config.cluster0, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf0 = SurfgradFromTriplanarProjection(tc.pN0, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO0.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN0, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO0.zw = a0.zw * tc.pN0.x + a1.zw * tc.pN0.y + a2.zw * tc.pN0.z;
            }
            MSBRANCH(weights.y)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN1.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[0], config.cluster1, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[1], config.cluster1, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN1.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[2], config.cluster1, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf1 = SurfgradFromTriplanarProjection(tc.pN1, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO1.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN1, a0.xy, a1.xy, a2.xy);
               #endif
               
               s.normSAO1.zw = a0.zw * tc.pN1.x + a1.zw * tc.pN1.y + a2.zw * tc.pN1.z;
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);

               MSBRANCHTRIPLANAR(tc.pN2.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[0], config.cluster2, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[1], config.cluster2, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN2.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[2], config.cluster2, d2).agrb;
                  COUNTSAMPLE
               }
               
               #if _SURFACENORMALS
                  s.surf2 = SurfgradFromTriplanarProjection(tc.pN2, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO2.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN2, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO2.zw = a0.zw * tc.pN2.x + a1.zw * tc.pN2.y + a2.zw * tc.pN2.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               half4 a0 = half4(0.5, 0.5, 0, 1);
               half4 a1 = half4(0.5, 0.5, 0, 1);
               half4 a2 = half4(0.5, 0.5, 0, 1);
               MSBRANCHTRIPLANAR(tc.pN3.x)
               {
                  a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[0], config.cluster3, d0).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.y)
               {
                  a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[1], config.cluster3, d1).agrb;
                  COUNTSAMPLE
               }
               MSBRANCHTRIPLANAR(tc.pN3.z)
               {
                  a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[2], config.cluster3, d2).agrb;
                  COUNTSAMPLE
               }

               #if _SURFACENORMALS
                  s.surf3 = SurfgradFromTriplanarProjection(tc.pN3, a0.xy, a1.xy, a2.xy);
               #else
                  s.normSAO3.xy = TransformTriplanarNormal(tc.IN, t2w, tc.axisSign, absVertNormal, tc.pN3, a0.xy, a1.xy, a2.xy);
               #endif

               s.normSAO3.zw = a0.zw * tc.pN3.x + a1.zw * tc.pN3.y + a2.zw * tc.pN3.z;
            }
            #endif

         #else
            s.normSAO0 = MICROSPLAT_SAMPLE_NORMAL(config.uv0, config.cluster0, mipLevel).agrb;
            COUNTSAMPLE
            s.normSAO0.xy = s.normSAO0.xy * 2 - 1;

            #if _SURFACENORMALS
               s.surf0 = ConvertNormal2ToGradient(s.normSAO0.xy);
            #endif

            MSBRANCH(weights.y)
            {
               s.normSAO1 = MICROSPLAT_SAMPLE_NORMAL(config.uv1, config.cluster1, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO1.xy = s.normSAO1.xy * 2 - 1;

               #if _SURFACENORMALS
                  s.surf1 = ConvertNormal2ToGradient(s.normSAO1.xy);
               #endif
            }
            #if !_MAX2LAYER
            MSBRANCH(weights.z)
            {
               s.normSAO2 = MICROSPLAT_SAMPLE_NORMAL(config.uv2, config.cluster2, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf2 = ConvertNormal2ToGradient(s.normSAO2.xy);
               #endif
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            MSBRANCH(weights.w)
            {
               s.normSAO3 = MICROSPLAT_SAMPLE_NORMAL(config.uv3, config.cluster3, mipLevel).agrb;
               COUNTSAMPLE
               s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
               #if _SURFACENORMALS
                  s.surf3 = ConvertNormal2ToGradient(s.normSAO3.xy);
               #endif
            }
            #endif
         #endif
      }

      void SampleEmis(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USEEMISSIVEMETAL
            #if _TRIPLANAR
            
               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  s.emisMetal0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }

                  s.emisMetal1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.emisMetal3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.emisMetal0 = MICROSPLAT_SAMPLE_EMIS(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.emisMetal1 = MICROSPLAT_SAMPLE_EMIS(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
                  MSBRANCH(weights.z)
                  {
                     s.emisMetal2 = MICROSPLAT_SAMPLE_EMIS(config.uv2, config.cluster2, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  MSBRANCH(weights.w)
                  {
                     s.emisMetal3 = MICROSPLAT_SAMPLE_EMIS(config.uv3, config.cluster3, mipLevel);
                     COUNTSAMPLE
                  }
               #endif
            #endif
         #endif
      }
      
      void SampleSpecular(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel, half4 weights)
      {
         #if _DISABLESPLATMAPS
            return;
         #endif
         #if _USESPECULARWORKFLOW
            #if _TRIPLANAR

               #if _USEGRADMIP
                  float4 d0 = mipLevel.d0;
                  float4 d1 = mipLevel.d1;
                  float4 d2 = mipLevel.d2;
               #elif _USELODMIP
                  float d0 = mipLevel.x;
                  float d1 = mipLevel.y;
                  float d2 = mipLevel.z;
               #else
                  MIPFORMAT d0 = mipLevel;
                  MIPFORMAT d1 = mipLevel;
                  MIPFORMAT d2 = mipLevel;
               #endif
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN0.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[0], config.cluster0, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[1], config.cluster0, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN0.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv0[2], config.cluster0, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               MSBRANCH(weights.y)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN1.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[0], config.cluster1, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[1], config.cluster1, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN1.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv1[2], config.cluster1, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN2.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[0], config.cluster2, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[1], config.cluster2, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN2.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv2[2], config.cluster2, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  half4 a0 = half4(0, 0, 0, 0);
                  half4 a1 = half4(0, 0, 0, 0);
                  half4 a2 = half4(0, 0, 0, 0);
                  MSBRANCHTRIPLANAR(tc.pN3.x)
                  {
                     a0 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[0], config.cluster3, d0);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.y)
                  {
                     a1 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[1], config.cluster3, d1);
                     COUNTSAMPLE
                  }
                  MSBRANCHTRIPLANAR(tc.pN3.z)
                  {
                     a2 = MICROSPLAT_SAMPLE_SPECULAR(tc.uv3[2], config.cluster3, d2);
                     COUNTSAMPLE
                  }
                  
                  s.specular3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.specular0 = MICROSPLAT_SAMPLE_SPECULAR(config.uv0, config.cluster0, mipLevel);
               COUNTSAMPLE

               MSBRANCH(weights.y)
               {
                  s.specular1 = MICROSPLAT_SAMPLE_SPECULAR(config.uv1, config.cluster1, mipLevel);
                  COUNTSAMPLE
               }
               #if !_MAX2LAYER
               MSBRANCH(weights.z)
               {
                  s.specular2 = MICROSPLAT_SAMPLE_SPECULAR(config.uv2, config.cluster2, mipLevel);
                  COUNTSAMPLE
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               MSBRANCH(weights.w)
               {
                  s.specular3 = MICROSPLAT_SAMPLE_SPECULAR(config.uv3, config.cluster3, mipLevel);
                  COUNTSAMPLE
               }
               #endif
            #endif
         #endif
      }

      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex, DecalOutput decalOutput)
      {
         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         half4 albedo = 0;
         half4 normSAO = half4(0,0,0,1);
         half3 surfGrad = half3(0,0,0);
         half4 emisMetal = 0;
         half3 specular = 0;
         
         float worldHeight = i.worldPos.y;
         float3 upVector = float3(0,1,0);
         
         #if _GLOBALTINT || _GLOBALNORMALS || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _GLOBALSPECULAR
            float globalSlopeFilter = 1;
            #if _GLOBALSLOPEFILTER
               float2 gfilterUV = float2(1 - saturate(dot(worldNormalVertex, upVector) * 0.5 + 0.49), 0.5);
               globalSlopeFilter = SAMPLE_TEXTURE2D(_GlobalSlopeTex, sampler_Diffuse, gfilterUV).a;
            #endif
         #endif

         // declare outside of branchy areas..
         half4 fxLevels = half4(0,0,0,0);
         half burnLevel = 0;
         half wetLevel = 0;
         half3 waterNormalFoam = half3(0, 0, 0);
         half porosity = 0.4;
         float streamFoam = 1.0f;
         half pud = 0;
         half snowCover = 0;
         half SSSThickness = 0;
         half3 SSSTint = half3(1,1,1);
         float traxBuffer = 0;
         float3 traxNormal = 0;
         float2 noiseUV = 0;
         
         

         #if _SPLATFADE
         MSBRANCHOTHER(1 - saturate(camDist - _SplatFade.y))
         {
         #endif

         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE || _SNOWFOOTSTEPS
            traxBuffer = SampleTraxBuffer(i.worldPos, worldNormalVertex, traxNormal);
         #endif
         
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
            #if _MICROMESH
               fxLevels = SampleFXLevels(InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, config.uv), wetLevel, burnLevel, traxBuffer);
            #elif _MICROVERTEXMESH || _MICRODIGGERMESH  || _MEGASPLAT
               fxLevels = ProcessFXLevels(i.fx, traxBuffer);
            #else
               fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel, traxBuffer);
            #endif
         #endif

         #if _DECAL
            fxLevels = max(fxLevels, decalOutput.fxLevels);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         

         MIPFORMAT albedoLOD = INITMIPFORMAT
         MIPFORMAT normalLOD = INITMIPFORMAT
         MIPFORMAT emisLOD = INITMIPFORMAT
         MIPFORMAT specLOD = INITMIPFORMAT
         MIPFORMAT origAlbedoLOD = INITMIPFORMAT;

         #if _TRIPLANAR && !_DISABLESPLATMAPS
            PrepTriplanar(i.shaderData.texcoord0, worldNormalVertex, i.worldPos, config, tc, weights, albedoLOD, normalLOD, emisLOD, origAlbedoLOD);
            tc.IN = i;
         #endif
         
         
         #if !_TRIPLANAR && !_DISABLESPLATMAPS
            #if _USELODMIP
               albedoLOD = ComputeMipLevel(config.uv0.xy, _Diffuse_TexelSize.zw);
               normalLOD = ComputeMipLevel(config.uv0.xy, _NormalSAO_TexelSize.zw);
               #if _USEEMISSIVEMETAL
                  emisLOD = ComputeMipLevel(config.uv0.xy, _EmissiveMetal_TexelSize.zw);
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = ComputeMipLevel(config.uv0.xy, _Specular_TexelSize.zw);;
               #endif
            #elif _USEGRADMIP
               albedoLOD = float4(ddx(config.uv0.xy), ddy(config.uv0.xy));
               normalLOD = albedoLOD;
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
            #endif

            origAlbedoLOD = albedoLOD;
         #endif

         #if _PERTEXCURVEWEIGHT
           SAMPLE_PER_TEX(ptCurveWeight, 19.5, config, half4(0.5,1,1,1));
           weights.x = lerp(smoothstep(0.5 - ptCurveWeight0.r, 0.5 + ptCurveWeight0.r, weights.x), weights.x, ptCurveWeight0.r*2);
           weights.y = lerp(smoothstep(0.5 - ptCurveWeight1.r, 0.5 + ptCurveWeight1.r, weights.y), weights.y, ptCurveWeight1.r*2);
           weights.z = lerp(smoothstep(0.5 - ptCurveWeight2.r, 0.5 + ptCurveWeight2.r, weights.z), weights.z, ptCurveWeight2.r*2);
           weights.w = lerp(smoothstep(0.5 - ptCurveWeight3.r, 0.5 + ptCurveWeight3.r, weights.w), weights.w, ptCurveWeight3.r*2);
           weights = TotalOne(weights);
         #endif
         
         

         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR && !_DISABLESPLATMAPS
            
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif

            // fix for pertex uv scale using gradient sampler and weight blended derivatives
            #if _USEGRADMIP
               albedoLOD = albedoLOD * ptUVScale0.rgrg * weights.x + 
                           albedoLOD * ptUVScale1.rgrg * weights.y + 
                           albedoLOD * ptUVScale2.rgrg * weights.z + 
                           albedoLOD * ptUVScale3.rgrg * weights.w;
               normalLOD = albedoLOD;
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
               #if _USESPECULARWORKFLOW
                  specLOD = albedoLOD;
               #endif
            #endif
         #endif

         #if _PERTEXUVROTATION && !_TRIPLANAR && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptUVRot, 16.5, config, half4(0,0,0,0));
            config.uv0.xy = RotateUV(config.uv0.xy, ptUVRot0.x);
            config.uv1.xy = RotateUV(config.uv1.xy, ptUVRot1.x);
            #if !_MAX2LAYER
               config.uv2.xy = RotateUV(config.uv2.xy, ptUVRot2.x);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = RotateUV(config.uv3.xy, ptUVRot0.x);
            #endif
         #endif

         
         o.Alpha = 1;

         
         #if _POM && !_DISABLESPLATMAPS
            DoPOM(i, config, tc, albedoLOD, weights, camDist, worldNormalVertex);
         #endif
         

         SampleAlbedo(config, tc, samples, albedoLOD, weights);

         #if _NOISEHEIGHT
            ApplyNoiseHeight(samples, config.uv, config, i.worldPos, worldNormalVertex);
         #endif
         
         #if _STREAMS || (_PARALLAX && !_DISABLESPLATMAPS)
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(i, config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX && !_DISABLESPLATMAPS
            DoParallax(i, earlyHeight, config, tc, samples, weights, camDist);
         #endif


         // Blend results
         #if _PERTEXINTERPCONTRAST && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = clamp(contrast + _Contrast, 0.0001, 1.0); 
            half cnt = contrast.x * weights.x + contrast.y * weights.y + contrast.z * weights.z + contrast.w * weights.w;
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, cnt);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif

         #if _HYBRIDHEIGHTBLEND
            heightWeights = lerp(heightWeights, TotalOne(weights), saturate(camDist/max(1.0, _HybridHeightBlendDistance)));
         #endif

         
         // rescale derivatives after height weighting. Basically, in gradmip mode we blend the mip levels,
         // but this is before height mapping is sampled, so reblending them after alpha will make sure the other
         // channels (normal, etc) are sharper, which likely matters most.. 
         #if _PERTEXUVSCALEOFFSET && !_DISABLESPLATMAPS
            #if _TRIPLANAR
               #if _USEGRADMIP
                  SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));

                  albedoLOD.d0 = origAlbedoLOD.d0 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d0 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d0 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d0 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d1 = origAlbedoLOD.d1 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d1 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d1 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d1 * ptUVScale3.xyxy * heightWeights.w;
               
                  albedoLOD.d2 = origAlbedoLOD.d2 * ptUVScale0.xyxy * heightWeights.x + 
                     origAlbedoLOD.d2 * ptUVScale1.xyxy * heightWeights.y + 
                     origAlbedoLOD.d2 * ptUVScale2.xyxy * heightWeights.z + 
                     origAlbedoLOD.d2 * ptUVScale3.xyxy * heightWeights.w;
               
                  normalLOD.d0 = albedoLOD.d0;
                  normalLOD.d1 = albedoLOD.d1;
                  normalLOD.d2 = albedoLOD.d2;
               
                  #if _USEEMISSIVEMETAL
                     emisLOD.d0 = albedoLOD.d0;
                     emisLOD.d1 = albedoLOD.d1;
                     emisLOD.d2 = albedoLOD.d2;
                  #endif
               #endif // gradmip
            #else // not triplanar
               // fix for pertex uv scale using gradient sampler and weight blended derivatives
               #if _USEGRADMIP
                  albedoLOD = origAlbedoLOD * ptUVScale0.rgrg * heightWeights.x + 
                              origAlbedoLOD * ptUVScale1.rgrg * heightWeights.y + 
                              origAlbedoLOD * ptUVScale2.rgrg * heightWeights.z + 
                              origAlbedoLOD * ptUVScale3.rgrg * heightWeights.w;
                  normalLOD = albedoLOD;
                  #if _USEEMISSIVEMETAL
                     emisLOD = albedoLOD;
                  #endif
                  #if _USESPECULARWORKFLOW
                     specLOD = albedoLOD;
                  #endif
               #endif
            #endif
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples, albedoLOD, heightWeights);
         #endif


         SampleNormal(config, tc, samples, normalLOD, heightWeights);

         #if _USEEMISSIVEMETAL
            SampleEmis(config, tc, samples, emisLOD, heightWeights);
         #endif

         #if _USESPECULARWORKFLOW
            SampleSpecular(config, tc, samples, specLOD, heightWeights);
         #endif

         #if _DISTANCERESAMPLE && !_DISABLESPLATMAPS
            DistanceResample(samples, config, tc, camDist, i.viewDir, fxLevels, albedoLOD, i.worldPos, heightWeights, worldNormalVertex);
         #endif

         #if _STARREACHFORMAT
            samples.normSAO0.w = length(samples.normSAO0.xy);
            samples.normSAO1.w = length(samples.normSAO1.xy);
            samples.normSAO2.w = length(samples.normSAO2.xy);
            samples.normSAO3.w = length(samples.normSAO3.xy);
         #endif

         // PerTexture sampling goes here, passing the samples structure
         
         #if _PERTEXMICROSHADOWS || _PERTEXFUZZYSHADE
            SAMPLE_PER_TEX(ptFuzz, 17.5, config, half4(0, 0, 1, 1));
         #endif

         #if _PERTEXMICROSHADOWS
            #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP)
            {
               half3 lightDir = GetGlobalLightDirTS(i);
               half4 microShadows = half4(1,1,1,1);
               microShadows.x = MicroShadow(lightDir, half3(samples.normSAO0.xy, 1), samples.normSAO0.a, ptFuzz0.a);
               microShadows.y = MicroShadow(lightDir, half3(samples.normSAO1.xy, 1), samples.normSAO1.a, ptFuzz1.a);
               microShadows.z = MicroShadow(lightDir, half3(samples.normSAO2.xy, 1), samples.normSAO2.a, ptFuzz2.a);
               microShadows.w = MicroShadow(lightDir, half3(samples.normSAO3.xy, 1), samples.normSAO3.a, ptFuzz3.a);
               samples.normSAO0.a *= microShadows.x;
               samples.normSAO1.a *= microShadows.y;
               #if !_MAX2LAYER
                  samples.normSAO2.a *= microShadows.z;
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.normSAO3.a *= microShadows.w;
               #endif

               
               #if _DEBUG_OUTPUT_MICROSHADOWS
               o.Albedo = BlendWeights(microShadows.x, microShadows.y, microShadows.z, microShadows.a, heightWeights);
               return o;
               #endif

               

               
            }
            #endif

         #endif // _PERTEXMICROSHADOWS


         #if _PERTEXFUZZYSHADE
            samples.albedo0.rgb = FuzzyShade(samples.albedo0.rgb, half3(samples.normSAO0.rg, 1), ptFuzz0.r, ptFuzz0.g, ptFuzz0.b, i.viewDir);
            samples.albedo1.rgb = FuzzyShade(samples.albedo1.rgb, half3(samples.normSAO1.rg, 1), ptFuzz1.r, ptFuzz1.g, ptFuzz1.b, i.viewDir);
            #if !_MAX2LAYER
               samples.albedo2.rgb = FuzzyShade(samples.albedo2.rgb, half3(samples.normSAO2.rg, 1), ptFuzz2.r, ptFuzz2.g, ptFuzz2.b, i.viewDir);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = FuzzyShade(samples.albedo3.rgb, half3(samples.normSAO3.rg, 1), ptFuzz3.r, ptFuzz3.g, ptFuzz3.b, i.viewDir);
            #endif
         #endif

         #if _PERTEXSATURATION && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptSaturattion, 9.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = lerp(MSLuminance(samples.albedo0.rgb), samples.albedo0.rgb, ptSaturattion0.a);
            samples.albedo1.rgb = lerp(MSLuminance(samples.albedo1.rgb), samples.albedo1.rgb, ptSaturattion1.a);
            #if !_MAX2LAYER
               samples.albedo2.rgb = lerp(MSLuminance(samples.albedo2.rgb), samples.albedo2.rgb, ptSaturattion2.a);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = lerp(MSLuminance(samples.albedo3.rgb), samples.albedo3.rgb, ptSaturattion3.a);
            #endif
         
         #endif
         
         #if _PERTEXTINT && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif
         
         #if _PCHEIGHTGRADIENT || _PCHEIGHTHSV || _PCSLOPEGRADIENT || _PCSLOPEHSV
            ProceduralGradients(i, samples, config, worldHeight, worldNormalVertex);
         #endif

         
         

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXCOLORINTENSITY
            SAMPLE_PER_TEX(ptCI, 23.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb = saturate(samples.albedo0.rgb * (1 + ptCI0.rrr));
            samples.albedo1.rgb = saturate(samples.albedo1.rgb * (1 + ptCI1.rrr));
            #if !_MAX2LAYER
               samples.albedo2.rgb = saturate(samples.albedo2.rgb * (1 + ptCI2.rrr));
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb = saturate(samples.albedo3.rgb * (1 + ptCI3.rrr));
            #endif
         #endif

         #if (_PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = saturate(((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5);
               samples.albedo1.rgb = saturate(((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5);
               #if !_MAX2LAYER
                 samples.albedo2.rgb = saturate(((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5);
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb = saturate(samples.albedo0.rgb + ptBC0.rrr);
               samples.albedo1.rgb = saturate(samples.albedo1.rgb + ptBC1.rrr);
               #if !_MAX2LAYER
                  samples.albedo2.rgb = saturate(samples.albedo2.rgb + ptBC2.rrr);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(samples.albedo3.rgb + ptBC3.rrr);
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if (_PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC) && !_DISABLESPLATMAPS
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR && !_DISABLESPLATMAPS
            #if _SURFACENORMALS
               samples.surf0 *= perTexMatSettings0.r;
               samples.surf1 *= perTexMatSettings1.r;
               samples.surf2 *= perTexMatSettings2.r;
               samples.surf3 *= perTexMatSettings3.r;
            #else
               samples.normSAO0.xy *= perTexMatSettings0.r;
               samples.normSAO1.xy *= perTexMatSettings1.r;
               samples.normSAO2.xy *= perTexMatSettings2.r;
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR && !_DISABLESPLATMAPS
            samples.normSAO0.a = pow(abs(samples.normSAO0.a), perTexMatSettings0.b);
            samples.normSAO1.a = pow(abs(samples.normSAO1.a), perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(abs(samples.normSAO2.a), perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(abs(samples.normSAO3.a), perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR && !_DISABLESPLATMAPS
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         
         #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_DEFERRED) || (defined(_URP) && defined(_PASSFORWARD) || _HDRP) 
          #if _PERTEXSSS
          {
            SAMPLE_PER_TEX(ptSSS, 18.5, config, half4(1, 1, 1, 1)); // tint, thickness
            half4 vals = ptSSS0 * heightWeights.x + ptSSS1 * heightWeights.y + ptSSS2 * heightWeights.z + ptSSS3 * heightWeights.w;
            SSSThickness = vals.a;
            SSSTint = vals.rgb;
          }
          #endif
         #endif

         #if _PERTEXRIMLIGHT
         {
            SAMPLE_PER_TEX(ptRimA, 26.5, config, half4(1, 1, 1, 1));
            SAMPLE_PER_TEX(ptRimB, 27.5, config, half4(1, 1, 1, 0));
            samples.emisMetal0.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO0.xy, 1))), max(0.0001, ptRimA0.g)) * ptRimB0.rgb * ptRimB0.a;
            samples.emisMetal1.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO1.xy, 1))), max(0.0001, ptRimA1.g)) * ptRimB1.rgb * ptRimB1.a;
            samples.emisMetal2.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO2.xy, 1))), max(0.0001, ptRimA2.g)) * ptRimB2.rgb * ptRimB2.a;
            samples.emisMetal3.rgb += pow(1.0 - saturate(dot(i.viewDir, float3(samples.normSAO3.xy, 1))), max(0.0001, ptRimA3.g)) * ptRimB3.rgb * ptRimB3.a;
         }
         #endif



         #if (((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)) && !_DISABLESPLATMAPS
            ApplyDetailDistanceNoisePerTex(samples, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         
         #if _GLOBALNOISEUV
            // noise defaults so that a value of 1, 1 is 4 pixels in size and moves the uvs by 1 pixel max.
            #if _CUSTOMSPLATTEXTURES
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #else
               noiseUV = (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, config.uv * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif
         #endif

         
         #if _TRAXSINGLE || _TRAXARRAY || _TRAXNOTEXTURE
            ApplyTrax(samples, config, i.worldPos, traxBuffer, traxNormal);
         #endif

         #if (_ANTITILEARRAYDETAIL || _ANTITILEARRAYDISTANCE || _ANTITILEARRAYNORMAL) && !_DISABLESPLATMAPS
            ApplyAntiTilePerTex(samples, config, camDist, i.worldPos, worldNormalVertex, heightWeights);
         #endif

         #if _GEOMAP && !_DISABLESPLATMAPS
         GeoTexturePerTex(samples, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif
         
         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH && !_DISABLESPLATMAPS
         GlobalTintTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH && !_DISABLESPLATMAPS
         GlobalNormalTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && _PERTEXGLOBALSAOMSTRENGTH && !_DISABLESPLATMAPS
         GlobalSAOMTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALEMIS && _PERTEXGLOBALEMISSTRENGTH && !_DISABLESPLATMAPS
         GlobalEmisTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && _PERTEXGLOBALSPECULARSTRENGTH && !_DISABLESPLATMAPS && _USESPECULARWORKFLOW
         GlobalSpecularTexturePerTex(samples, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _PERTEXMETALLIC && !_DISABLESPLATMAPS
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         #if _GLITTER && !_DISABLESPLATMAPS
            DoGlitter(i, samples, config, camDist, worldNormalVertex, i.worldPos);
         #endif
         
         // Blend em..
         #if _DISABLESPLATMAPS
            // If we don't sample from the _Diffuse, then the shader compiler will strip the sampler on
            // some platforms, which will cause everything to break. So we sample from the lowest mip
            // and saturate to 1 to keep the cost minimal. Annoying, but the compiler removes the texture
            // and sampler, even though the sampler is still used.
            albedo = saturate(UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, float3(0,0,0), 12) + 1);
            albedo.a = 0.5; // make height something we can blend with for the combined mesh mode, since it still height blends.
            normSAO = half4(0,0,0,1);
         #else
            albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
            normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);

            #if _SURFACENORMALS
               surfGrad = BlendWeights(samples.surf0, samples.surf1, samples.surf2, samples.surf3, heightWeights);
            #endif

            #if (_USEEMISSIVEMETAL || _PERTEXRIMLIGHT) && !_DISABLESPLATMAPS
               emisMetal = BlendWeights(samples.emisMetal0, samples.emisMetal1, samples.emisMetal2, samples.emisMetal3, heightWeights);
            #endif

            #if _USESPECULARWORKFLOW && !_DISABLESPLATMAPS
               specular = BlendWeights(samples.specular0, samples.specular1, samples.specular2, samples.specular3, heightWeights);
            #endif

            #if _PERTEXOUTLINECOLOR
               SAMPLE_PER_TEX(ptOutlineColor, 28.5, config, half4(0.5, 0.5, 0.5, 1));
               half4 outlineColor = BlendWeights(ptOutlineColor0, ptOutlineColor1, ptOutlineColor2, ptOutlineColor3, heightWeights);
               half4 tstr = saturate(abs(heightWeights - 0.5) * 2);
               half transitionBlend = min(min(min(tstr.x, tstr.y), tstr.z), tstr.w);
               albedo.rgb = lerp(albedo.rgb * outlineColor.rgb * 2, albedo.rgb, outlineColor.a * transitionBlend);
            #endif
         #endif



         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
            o.Alpha = 1.0;
            if (config.uv0.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.x;
            else if (config.uv1.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.y;
            else if (config.uv2.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.z;
            else if (config.uv3.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.w;
         #endif



         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
            ApplyDetailDistanceNoise(albedo.rgb, normSAO, surfGrad, config, camDist, i.worldPos, worldNormalVertex);
         #endif

         #if _SPLATFADE
         }
         #endif

         #if _SPLATFADE
            
            float2 sfDX = ddx(config.uv * _UVScale);
            float2 sfDY = ddy(config.uv * _UVScale);

            MSBRANCHOTHER(camDist - _SplatFade.x)
            {
               float falloff = saturate(InverseLerp(_SplatFade.x, _SplatFade.y, camDist));
               half4 sfalb = SAMPLE_TEXTURE2D_ARRAY_GRAD(_Diffuse, sampler_Diffuse, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY);
               COUNTSAMPLE
               albedo.rgb = lerp(albedo.rgb, sfalb.rgb, falloff);

               #if !_NONORMALMAP && !_AUTONORMAL
                  half4 sfnormSAO = SAMPLE_TEXTURE2D_ARRAY_GRAD(_NormalSAO, sampler_NormalSAO, config.uv * _UVScale, _SplatFade.z, sfDX, sfDY).agrb;
                  COUNTSAMPLE
                  sfnormSAO.xy = sfnormSAO.xy * 2 - 1;

                  normSAO = lerp(normSAO, sfnormSAO, falloff);
  
                  #if _SURFACENORMALS
                     surfGrad = lerp(surfGrad, ConvertNormal2ToGradient(sfnormSAO.xy), falloff);
                  #endif
               #endif
              
            }
         #endif

         #if _AUTONORMAL
            float3 autoNormal = HeightToNormal(albedo.a * _AutoNormalHeightScale, i.worldPos);
            normSAO.xy = autoNormal;
            normSAO.z = 0;
            normSAO.w = (autoNormal.z * autoNormal.z);
         #endif
 


         #if _MESHCOMBINED
            SampleMeshCombined(albedo, normSAO, surfGrad, emisMetal, specular, o.Alpha, SSSThickness, SSSTint, config, heightWeights);
         #endif

         #if _ISOBJECTSHADER
            SampleObjectShader(i, albedo, normSAO, surfGrad, emisMetal, specular, config);
         #endif

         #if _GEOMAP
            GeoTexture(albedo.rgb, normSAO, surfGrad, i.worldPos, worldHeight, config, worldNormalVertex, upVector);
         #endif

         
         #if _SCATTER
            ApplyScatter(
               #if _MEGASPLAT
               config, 
               #endif
               i, albedo, normSAO, surfGrad, config.uv, camDist);
         #endif

         #if _DECAL
            DoDecalBlend(decalOutput, albedo, normSAO, surfGrad, emisMetal, i.uv_Control0);
         #endif
         

         #if _GLOBALTINT && !_PERTEXGLOBALTINTSTRENGTH
            GlobalTintTexture(albedo.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _VSGRASSMAP
            VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMALSTRENGTH
            GlobalNormalTexture(normSAO, surfGrad, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALSMOOTHAOMETAL && !_PERTEXGLOBALSAOMSTRENGTH
            GlobalSAOMTexture(normSAO, emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif
         
         #if _GLOBALEMIS && !_PERTEXGLOBALEMISSTRENGTH
            GlobalEmisTexture(emisMetal, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         #if _GLOBALSPECULAR && !_PERTEXGLOBALSPECULARSTRENGTH && _USESPECULARWORKFLOW
            GlobalSpecularTexture(specular.rgb, config, camDist, globalSlopeFilter, noiseUV);
         #endif

         
         
         o.Albedo = albedo.rgb;
         o.Height = albedo.a;

         #if _NONORMALMAP
            o.Normal = half3(0,0,1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #elif _SURFACENORMALS
            o.Normal = ResolveNormalFromSurfaceGradient(surfGrad);
            o.Normal = mul(GetTBN(i), o.Normal);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;
         #else
            o.Normal = half3(normSAO.xy, 1);
            o.Smoothness = normSAO.b;
            o.Occlusion = normSAO.a;       
         #endif


         

         #if _USEEMISSIVEMETAL || _GLOBALSMOOTHAOMETAL || _GLOBALEMIS || _PERTEXRIMLIGHT
           #if _USEEMISSIVEMETAL
	           emisMetal.rgb *= _EmissiveMult;
	        #endif
           
           o.Emission += emisMetal.rgb;
           o.Metallic = emisMetal.a;
	        
         #endif

         #if _USESPECULARWORKFLOW
            o.Specular = specular;
         #endif

         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(i, o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         
         #if _SNOW
         snowCover = DoSnow(i, o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, 
            config, weights, SSSTint, SSSThickness, traxBuffer, traxNormal);
         #endif

         #if _PERTEXSSS || _MESHCOMBINEDUSESSS || (_SNOW && _SNOWSSS)
         {
            half3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

            o.Emission += ComputeSSS(i, worldView, WorldNormalVector(i, o.Normal),
               SSSTint, SSSThickness, _SSSDistance, _SSSScale, _SSSPower);
         }
         #endif
         
         #if _SNOWGLITTER
            DoSnowGlitter(i, config, o, camDist, worldNormalVertex, snowCover);
         #endif

         #if _WINDPARTICULATE || _SNOWPARTICULATE
            DoWindParticulate(i, o, config, weights, camDist, worldNormalVertex, snowCover);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         #if _SPECULARFADE
         {
            float specFade = saturate((i.worldPos.y - _SpecularFades.x) / max(_SpecularFades.y - _SpecularFades.x, 0.0001));
            o.Metallic *= specFade;
            o.Smoothness *= specFade;
         }
         #endif

         #if _VSSHADOWMAP
         VSShadowTexture(o, i, config, camDist);
         #endif
         
         #if _TOONWIREFRAME
         ToonWireframe(config.uv, o.Albedo, camDist);
         #endif

        #if _SEETHROUGHSHADER
            SeethroughShader(o.Albedo, o.Emission, o.Alpha, i.worldPos, o.Normal, i.worldNormal);                   
        #endif


         #if _DEBUG_TRAXBUFFER
            ClearAllButAlbedo(o, half3(traxBuffer, 0, 0) * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMALVERTEX
            ClearAllButAlbedo(o, worldNormalVertex * saturate(o.Albedo.z+1));
         #elif _DEBUG_WORLDNORMAL
            ClearAllButAlbedo(o,  WorldNormalVector(i, o.Normal) * saturate(o.Albedo.z+1));
         #endif

         #if _DEBUG_MEGABARY && _MEGASPLAT
            o.Albedo = i.baryWeights.xyz;
         #endif


         return o;
      }
      
      void SampleSplats(float2 controlUV, inout half4 w0, inout half4 w1, inout half4 w2, inout half4 w3, inout half4 w4, inout half4 w5, inout half4 w6, inout half4 w7)
      {
         #if _CUSTOMSPLATTEXTURES
            #if !_MICROMESH
            controlUV = (controlUV * (_CustomControl0_TexelSize.zw - 1.0f) + 0.5f) * _CustomControl0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _CustomControl0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _CustomControl0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_CustomControl0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_CustomControl1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_CustomControl2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_CustomControl3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_CustomControl4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_CustomControl5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_CustomControl6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_CustomControl7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #else
            #if !_MICROMESH
            controlUV = (controlUV * (_Control0_TexelSize.zw - 1.0f) + 0.5f) * _Control0_TexelSize.xy;
            #endif

            #if  _CONTROLNOISEUV
               controlUV += (SAMPLE_TEXTURE2D(_NoiseUV, sampler_Diffuse, controlUV * _Control0_TexelSize.zw * 0.2 * _NoiseUVParams.x).ga - 0.5) * _Control0_TexelSize.xy * _NoiseUVParams.y;
            #endif

            w0 = SAMPLE_TEXTURE2D(_Control0, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE

            #if !_MAX4TEXTURES
            w1 = SAMPLE_TEXTURE2D(_Control1, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES
            w2 = SAMPLE_TEXTURE2D(_Control2, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            w3 = SAMPLE_TEXTURE2D(_Control3, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w4 = SAMPLE_TEXTURE2D(_Control4, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            w5 = SAMPLE_TEXTURE2D(_Control5, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX28TEXTURES || _MAX32TEXTURES
            w6 = SAMPLE_TEXTURE2D(_Control6, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif

            #if _MAX32TEXTURES
            w7 = SAMPLE_TEXTURE2D(_Control7, shared_linear_clamp_sampler, controlUV);
            COUNTSAMPLE
            #endif
         #endif
      }   


      

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
         #if _MEGANOUV
            i.uv_Control0 = i.worldPos.xz;
         #endif
         
         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);
          
         #if _FORCELOCALSPACE
            worldNormalVertex = mul((float3x3)GetWorldToObjectMatrix(), worldNormalVertex).xyz;
            i.worldPos = i.worldPos -  mul(GetObjectToWorldMatrix(), float4(0,0,0,1)).xyz;
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ORIGINSHIFT
             i.worldPos = i.worldPos + mul(_GlobalOriginMTX, float4(0,0,0,1)).xyz;
             i.worldHeight = i.worldPos.y;
         #endif

         #if _DEBUG_USE_TOPOLOGY
            i.worldPos = SAMPLE_TEXTURE2D(_DebugWorldPos, sampler_Diffuse, i.uv_Control0);
            worldNormalVertex = SAMPLE_TEXTURE2D(_DebugWorldNormal, sampler_Diffuse, i.uv_Control0);
            i.worldHeight = i.worldPos.y;
         #endif

         #if _ALPHABELOWHEIGHT && !_TBDISABLEALPHAHOLES
            ClipWaterLevel(i.worldPos);
         #endif

         #if !_TBDISABLEALPHAHOLES && defined(_ALPHATEST_ON)
            // UNITY 2019.3 holes
            ClipHoles(i.uv_Control0);
         #endif


         float2 origUV = i.uv_Control0;

         #if _MICROMESH && _MESHUV2
         float2 controlUV = i.uv2_Diffuse;
         #else
         float2 controlUV = i.uv_Control0;
         #endif


         #if _MICROMESH
            controlUV = InverseLerp(_UVMeshRange.xy, _UVMeshRange.zw, controlUV);
         #endif

         half4 weights = half4(1,0,0,0);

         Config config = (Config)0;
         UNITY_INITIALIZE_OUTPUT(Config,config);
         config.uv = origUV;

         DecalOutput decalOutput = (DecalOutput)0;
         #if _DECAL
            decalOutput = DoDecals(i.uv_Control0, i.worldPos, camDist, worldNormalVertex);
         #endif

         #if _SURFACENORMALS
         // Initialize the surface gradient basis vectors
         ConstructSurfaceGradientTBN(i);
         #endif
        


         #if _SPLATFADE
         MSBRANCHOTHER(_SplatFade.y - camDist)
         #endif // _SPLATFADE
         {
            #if !_DISABLESPLATMAPS

               // Sample the splat data, from textures or vertices, and setup the config..
               #if _MICRODIGGERMESH
                  DiggerSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLAT
                  MegaSplatVertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MEGASPLATTEXTURE
                   MegaSplatTextureSetup(controlUV, weights, origUV, config, i.worldPos, decalOutput);
               #elif _MICROVERTEXMESH
                  VertexSetup(i, weights, origUV, config, i.worldPos, decalOutput);
               #elif !_PROCEDURALTEXTURE || _PROCEDURALBLENDSPLATS
                  half4 w0 = 0; half4 w1 = 0; half4 w2 = 0; half4 w3 = 0; half4 w4 = 0; half4 w5 = 0; half4 w6 = 0; half4 w7 = 0;
                  SampleSplats(controlUV, w0, w1, w2, w3, w4, w5, w6, w7);
                  Setup(weights, origUV, config, w0, w1, w2, w3, w4, w5, w6, w7, i.worldPos, decalOutput);
               #endif

               #if _PROCEDURALTEXTURE
                  float3 procNormal = worldNormalVertex;
                  float3 worldPos = i.worldPos;
                  ProceduralSetup(i, worldPos, i.worldHeight, procNormal, i.worldUpVector, weights, origUV, config, ddx(origUV), ddy(origUV), ddx(worldPos), ddy(worldPos), decalOutput);
               #endif
            #else // _DISABLESPLATMAPS
                Setup(weights, origUV, config, half4(1,0,0,0), 0, 0, 0, 0, 0, 0, 0, i.worldPos, decalOutput);
            #endif

            #if _SLOPETEXTURE
               SlopeTexture(config, weights, worldNormalVertex);
            #endif
         } // _SPLATFADE else case


         #if _TOONFLATTEXTURE
            float2 quv = floor(origUV * _ToonTerrainSize);
            float2 fuv = frac(origUV * _ToonTerrainSize);
            #if !_TOONFLATTEXTUREQUAD
               quv = Hash2D((fuv.x > fuv.y) ? quv : quv * 0.333);
            #endif
            float2 uvq = quv / _ToonTerrainSize;
            config.uv0.xy = uvq;
            config.uv1.xy = uvq;
            config.uv2.xy = uvq;
            config.uv3.xy = uvq;
         #endif
         
         #if (_TEXTURECLUSTER2 || _TEXTURECLUSTER3) && !_DISABLESPLATMAPS
            PrepClusters(origUV, config, i.worldPos, worldNormalVertex);
         #endif

         #if (_ALPHAHOLE || _ALPHAHOLETEXTURE) && !_DISABLESPLATMAPS && !_TBDISABLEALPHAHOLES
         ClipAlphaHole(config, weights);
         #endif


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex, decalOutput);

         // On windows, sometimes the shared samplers gets stripped, so we have to do this crap.
         // We sample from the lowest mip, so it shouldn't cost much, but still, I hate this, wtf..
         float stripVal = saturate(SAMPLE_TEXTURE2D_LOD(_Diffuse, sampler_Diffuse, config.uv0, 11).r + 2);
         stripVal *= saturate(SAMPLE_TEXTURE2D_LOD(_NormalSAO, sampler_NormalSAO, config.uv0, 11).r + 2);
         l.Albedo *= stripVal;
         l.Normal *= stripVal;

         #if _PROCEDURALTEXTURE
            ProceduralTextureDebugOutput(l, weights, config);
         #endif


         return l;

      }



   





float4 ConstructTerrainTangent(float3 normal, float3 positiveZ)
{
    // Consider a flat terrain. It should have tangent be (1, 0, 0) and bitangent be (0, 0, 1) as the UV of the terrain grid mesh is a scale of the world XZ position.
    // In CreateTangentToWorld function (in SpaceTransform.hlsl), it is cross(normal, tangent) * sgn for the bitangent vector.
    // It is not true in a left-handed coordinate system for the terrain bitangent, if we provide 1 as the tangent.w. It would produce (0, 0, -1) instead of (0, 0, 1).
    // Also terrain's tangent calculation was wrong in a left handed system because cross((0,0,1), terrainNormalOS) points to the wrong direction as negative X.
    // Therefore all the 4 xyzw components of the tangent needs to be flipped to correct the tangent frame.
    // (See TerrainLitData.hlsl - GetSurfaceAndBuiltinData)
    float3 tangent = cross(normal, positiveZ);
    return float4(tangent, -1);
}



void TerrainInstancing(inout float4 vertex, inout float3 normal, inout float2 uv)
{
#if _MICROTERRAIN && defined(UNITY_INSTANCING_ENABLED) && !_TERRAINBLENDABLESHADER
   
    float2 patchVertex = vertex.xy;
    float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

    float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
    uv = sampleCoords * _TerrainHeightmapRecipSize.zw;

    float2 sampleUV = (uv / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;

    float height = UnpackHeightmap(SAMPLE_TEXTURE2D_LOD(_TerrainHeightmapTexture, shared_linear_clamp_sampler, sampleUV, 0));
   
    vertex.xz = sampleCoords * _TerrainHeightmapScale.xz;
    vertex.y = height * _TerrainHeightmapScale.y;

    
    normal = float3(0, 1, 0);

#endif
}


void ApplyMeshModification(inout VertexData input)
{
   #if _MICROTERRAIN && !_TERRAINBLENDABLESHADER
      float2 uv = input.texcoord0.xy;
      TerrainInstancing(input.vertex, input.normal, uv);
      input.texcoord0.xy = uv;
   #endif
   #if _PERPIXNORMAL && !_TERRAINBLENDABLESHADER
      input.normal = float3(0,1,0);
   #endif

}

// called by the template, so we can remove tangent from VertexData
void ApplyTerrainTangent(inout VertexToPixel input)
{
   #if (_MICROTERRAIN || _PERPIXNORMAL) && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif

   // digger meshes ain't got no tangent either..
   #if _MICRODIGGERMESH && !_TERRAINBLENDABLESHADER
      input.worldTangent = ConstructTerrainTangent(input.worldNormal, float3(0, 0, 1));
   #endif
}


void ModifyVertex(inout VertexData v, inout ExtraV2F d)
{
   ApplyMeshModification(v);

   #if _MICROVERTEXMESH || _MICRODIGGERMESH
      EncodeVertexWorkflow(v, d);
   #elif _MEGASPLAT
      EncodeMegaSplatVertex(v, d);
   #endif

}


void ModifyTessellatedVertex(inout VertexData v, inout ExtraV2F d)
{
   #if _TESSDISTANCE
      v.vertex.xyz += OffsetVertex(v, d);
   #endif
}

float3 GetTessFactors ()
{
   #if _TESSDISTANCE
      return float3(_TessData2.x, _TessData2.y, _TessData1.x);
   #endif
   return 0;
}


        


    
    void SurfaceFunction(inout Surface o, inout ShaderData d)
    {
       
        float3 worldNormalVertex = d.worldSpaceNormal;

        #if (defined(UNITY_INSTANCING_ENABLED) && _MICROTERRAIN && !_TERRAINBLENDABLESHADER)
            float2 sampleCoords = (d.texcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            float3 geomBitangent = normalize(cross(geomNormal, geomTangent)) * -1;
            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

         #elif _PERPIXNORMAL &&  (_MICROTERRAIN || _MICROMESHTERRAIN) && !_TERRAINBLENDABLESHADER
            float2 sampleCoords = (d.texcoord0.xy * _PerPixelNormal_TexelSize.zw + 0.5f) * _PerPixelNormal_TexelSize.xy;
            #if _TOONHARDEDGENORMAL
               sampleCoords = ToonEdgeUV(d.texcoord0.xy);
            #endif

            float3 geomNormal = normalize(SAMPLE_TEXTURE2D(_PerPixelNormal, shared_linear_clamp_sampler, sampleCoords).xyz * 2 - 1);
            
            float3 geomTangent = normalize(cross(geomNormal, float3(0, 0, 1)));
            
            float3 geomBitangent = normalize(cross(geomTangent, geomNormal)) * -1;
            #if _MICROMESHTERRAIN
                geomBitangent *= -1;
            #endif

            worldNormalVertex = geomNormal;
            d.worldSpaceNormal = geomNormal;
            d.worldSpaceTangent = geomTangent;
            d.TBNMatrix = float3x3(geomTangent, geomBitangent, geomNormal);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);

        #endif

        #if _TOONPOLYEDGE
           FlatShade(d);
        #endif

         Input i = DescToInput(d);

         
         
         #if _SRPTERRAINBLEND
            MicroSplatLayer l = BlendWithTerrain(d);

               #if _DEBUG_WORLDNORMAL
                  ClearAllButAlbedo(l, normalize(TangentToWorldSpace(d, l.Normal)) * saturate(l.Albedo.z+1));
               #endif
         #else
            MicroSplatLayer l = SurfImpl(i, worldNormalVertex);
         #endif

        DoDebugOutput(l);




        o.Albedo = l.Albedo;
        o.Normal = l.Normal;
        o.Smoothness = l.Smoothness;
        o.Occlusion = l.Occlusion;
        o.Metallic = l.Metallic;
        o.Emission = l.Emission;
        #if _USESPECULARWORKFLOW
        o.Specular = l.Specular;
        #endif
        o.Height = l.Height;
        o.Alpha = l.Alpha;


    }



        



         // SHADERDESC

         ShaderData CreateShaderData(VertexToPixel i)
         {
            ShaderData d = (ShaderData)0;
            d.worldSpacePosition = i.worldPos;

            d.worldSpaceNormal = i.worldNormal;
            d.worldSpaceTangent = i.worldTangent.xyz;
            float3 bitangent = cross(i.worldTangent.xyz, i.worldNormal) * i.worldTangent.w * -1;
            

            d.TBNMatrix = float3x3(d.worldSpaceTangent, bitangent, d.worldSpaceNormal);
            d.worldSpaceViewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            d.tangentSpaceViewDir = mul(d.worldSpaceViewDir, d.TBNMatrix);
             d.texcoord0 = i.texcoord0;
            #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
                d.texcoord1 = i.texcoord1;
               // d.texcoord2 = i.texcoord2;
            #endif
            // d.texcoord3 = i.texcoord3;
            // d.vertexColor = i.vertexColor;

            // these rarely get used, so we back transform them. Usually will be stripped.
            #if _HDRP
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(GetCameraRelativePositionWS(i.worldPos), 1));
            #else
                // d.localSpacePosition = mul(GetWorldToObjectMatrix(), float4(i.worldPos, 1));
            #endif
            // d.localSpaceNormal = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldNormal));
            // d.localSpaceTangent = normalize(mul((float3x3)GetWorldToObjectMatrix(), i.worldTangent.xyz));

            // d.screenPos = i.screenPos;
            // d.screenUV = i.screenPos.xy / i.screenPos.w;

            // d.extraV2F0 = i.extraV2F0;
            // d.extraV2F1 = i.extraV2F1;
            // d.extraV2F2 = i.extraV2F2;
            // d.extraV2F3 = i.extraV2F3;
            // d.extraV2F4 = i.extraV2F4;
            // d.extraV2F5 = i.extraV2F5;
            // d.extraV2F6 = i.extraV2F6;
            // d.extraV2F7 = i.extraV2F7;

            return d;
         }
         // CHAINS

            void ChainModifyVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               ModifyVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }

            void ChainModifyTessellatedVertex(inout VertexData v, inout VertexToPixel v2p)
            {
               ExtraV2F d = (ExtraV2F)0;
               // d.extraV2F0 = v2p.extraV2F0;
               // d.extraV2F1 = v2p.extraV2F1;
               // d.extraV2F2 = v2p.extraV2F2;
               // d.extraV2F3 = v2p.extraV2F3;
               // d.extraV2F4 = v2p.extraV2F4;
               // d.extraV2F5 = v2p.extraV2F5;
               // d.extraV2F6 = v2p.extraV2F6;
               // d.extraV2F7 = v2p.extraV2F7;

               ModifyTessellatedVertex(v, d);

               // v2p.extraV2F0 = d.extraV2F0;
               // v2p.extraV2F1 = d.extraV2F1;
               // v2p.extraV2F2 = d.extraV2F2;
               // v2p.extraV2F3 = d.extraV2F3;
               // v2p.extraV2F4 = d.extraV2F4;
               // v2p.extraV2F5 = d.extraV2F5;
               // v2p.extraV2F6 = d.extraV2F6;
               // v2p.extraV2F7 = d.extraV2F7;
            }


            void ChainFinalColorForward(inout Surface l, inout ShaderData d, inout half4 color)
            {
               
            }

            void ChainFinalGBufferStandard(inout Surface s, inout ShaderData d, inout half4 GBuffer0, inout half4 GBuffer1, inout half4 GBuffer2, inout half4 outEmission, inout half4 outShadowMask)
            {
               
            }


         
         #if _PASSSHADOW
            float3 _LightDirection;
            float3 _LightPosition;
         #endif

         // vertex shader
         VertexToPixel Vert (VertexData v)
         {
           
           VertexToPixel o = (VertexToPixel)0;

           UNITY_SETUP_INSTANCE_ID(v);
           UNITY_TRANSFER_INSTANCE_ID(v, o);
           UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


#if !_TESSELLATION_ON
           ChainModifyVertex(v, o);
#endif

            o.texcoord0 = v.texcoord0;

           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               o.texcoord1 = v.texcoord1;
              // o.texcoord2 = v.texcoord2;
           #endif

           // o.texcoord3 = v.texcoord3;
           // o.vertexColor = v.vertexColor;
           
           VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
           o.worldPos = TransformObjectToWorld(v.vertex.xyz);
           o.worldNormal = TransformObjectToWorldNormal(v.normal);

           
           #if !_MICROTERRAIN || _TERRAINBLENDABLESHADER
               float2 uv1 = v.texcoord1.xy;
               float2 uv2 = v.texcoord2.xy;
               o.worldTangent = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);
           #else
               float2 uv1 = v.texcoord0.xy;
               float2 uv2 = uv1;
           #endif

           // MS Only
           ApplyTerrainTangent(o);
           

          #if _PASSSHADOW
              #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                 float3 lightDirectionWS = normalize(_LightPosition - o.worldPos);
              #else
                 float3 lightDirectionWS = _LightDirection;
              #endif
              // Define shadow pass specific clip position for Universal
              o.pos = TransformWorldToHClip(ApplyShadowBias(o.worldPos, o.worldNormal, lightDirectionWS));
              #if UNITY_REVERSED_Z
                  o.pos.z = min(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #else
                  o.pos.z = max(o.pos.z, o.pos.w * UNITY_NEAR_CLIP_VALUE);
              #endif
          #elif _PASSMETA
              #if _MICROTERRAIN
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), v.texcoord0.xy, v.texcoord0.xy, unity_LightmapST, unity_DynamicLightmapST);
              #else
               o.pos = MetaVertexPosition(float4(v.vertex.xyz, 0), uv1, uv2, unity_LightmapST, unity_DynamicLightmapST);
              #endif
          #else
              o.pos = TransformWorldToHClip(o.worldPos);
          #endif



          // o.screenPos = ComputeScreenPos(o.pos, _ProjectionParams.x);
          

          #if _PASSFORWARD || _PASSGBUFFER
              #if _MICROTERRAIN
                 OUTPUT_LIGHTMAP_UV(v.texcoord0.xy, unity_LightmapST, o.lightmapUV);
              #else
                 OUTPUT_LIGHTMAP_UV(uv1, unity_LightmapST, o.lightmapUV);
              #endif
              OUTPUT_SH(o.worldNormal, o.sh);
              #if defined(DYNAMICLIGHTMAP_ON)
                   o.dynamicLightmapUV.xy = uv2 * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
              #endif
          #endif

          #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
              half fogFactor = 0;
              #if defined(_FOG_FRAGMENT)
                fogFactor = ComputeFogFactor(o.pos.z);
              #endif
              #if _BAKEDLIT
                  half3 vertexLight = VertexLighting(o.worldPos, o.worldNormal);
                  o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
              #else
                  o.fogFactorAndVertexLight = half4(fogFactor, 0,0,0);
              #endif
          #endif

          #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             o.shadowCoord = GetShadowCoord(vertexInput);
          #endif

           return o;
         }


         

         // fragment shader
         half4 Frag (VertexToPixel IN
            #ifdef _DEPTHOFFSET_ON
              , out float outputDepth : SV_Depth
            #endif
         ) : SV_Target
         {
           UNITY_SETUP_INSTANCE_ID(IN);
           UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

           ShaderData d = CreateShaderData(IN);
           Surface l = (Surface)0;

           #ifdef _DEPTHOFFSET_ON
              l.outputDepth = outputDepth;
           #endif

           l.Albedo = half3(0.5, 0.5, 0.5);
           l.Normal = float3(0,0,1);
           l.Occlusion = 1;
           l.Alpha = 1;

           SurfaceFunction(l, d);

           #ifdef _DEPTHOFFSET_ON
              outputDepth = l.outputDepth;
           #endif

           #if defined(_GBUFFER_NORMALS_OCT)
              float3 normalWS = d.worldSpaceNormal;
              float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
              float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
              half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
              return half4(packedNormalWS, 0.0);
           #else
              float3 wsn = l.Normal;
              #if !_WORLDSPACENORMAL
                wsn = TangentToWorldSpace(d, l.Normal);
              #endif
              return half4(NormalizeNormalPerPixel(wsn), 0.0);
          #endif

         }

         ENDHLSL

      }


      
      
              Pass
        {
            Name "SceneSelectionPass"
            Tags { "LightMode" = "SceneSelectionPass" }

            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap

            #define SCENESELECTIONPASS
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Terrain/TerrainLitPasses.hlsl"
            ENDHLSL
        }

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
   }
   Dependency "BaseMapShader" =  "Hidden/MicroSplat_Base-1634248963"
   Fallback "Hidden/MicroSplat_Base-1634248963"
   CustomEditor "MicroSplatShaderGUI"
}
