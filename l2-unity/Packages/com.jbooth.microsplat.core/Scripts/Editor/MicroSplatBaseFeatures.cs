//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
   [InitializeOnLoad]
   public class MicroSplatBaseFeatures : FeatureDescriptor
   {
      public override int DisplaySortOrder()
      {
         return -1000;
      }

      public override string ModuleName()
      {
         return "Core";
      }

      public override string GetHelpPath ()
      {
         return "https://docs.google.com/document/d/1t_ZEKW8bHJqVWulH9Tcu-V5PIMr-sBOV_Ob7k2y1XY8/";         
      }

      public enum DefineFeature
      {
         _MICROSPLAT = 0,
         _MAX3LAYER,
         _MAX2LAYER,
         _MAX4TEXTURES,
         _MAX8TEXTURES,
         _MAX12TEXTURES,
         _MAX16TEXTURES,
         _MAX20TEXTURES,
         _MAX24TEXTURES,
         _MAX28TEXTURES,
         _MAX32TEXTURES,
         _PERTEXTINT,
         _PERTEXBRIGHTNESS,
         _PERTEXCOLORINTENSITY,
         _PERTEXCONTRAST,
         _PERTEXSATURATION,
         _PERTEXAOSTR,
         _PERTEXNORMSTR,
         _PERTEXSMOOTHSTR,
         _PERTEXMETALLIC,
         _PERTEXUVSCALEOFFSET,
         _PERTEXINTERPCONTRAST,
         _PERTEXHEIGHTOFFSET,
         _PERTEXHEIGHTCONTRAST,
         _PERTEXUVROTATION,
         _PERTEXFUZZYSHADE,
         _PERTEXSSS,
         _PERTEXMICROSHADOWS,
         _PERTEXCURVEWEIGHT,
         _PERTEXRIMLIGHT,
         _CONTROLNOISEUV,
         _PERTEXOUTLINECOLOR,
         _NORMALIZEWEIGHTS,
         _BDRF3,
         _BDRFLAMBERT,
         _SPECULARFROMMETALLIC,
         _USELODMIP,
         _USEGRADMIP,
         _DISABLEHEIGHTBLENDING,
         _HYBRIDHEIGHTBLEND,
         _WORLDUV,
         _USEEMISSIVEMETAL,
         _FORCEMODEL46,
         _FORCEMODEL50,
         _PACKINGHQ,
         _USESPECULARWORKFLOW,
         _PERPIXNORMAL,
         _NONORMALMAP,
         _AUTONORMAL,
         _MICROTERRAIN,
         _MICROMESH,
         _MICROMESHTERRAIN,
	      _MICROVERTEXMESH,
         _MICROPOLARISMESH,
         _MEGASPLAT,
         _MEGASPLATTEXTURE,
         _ORIGINSHIFT,
         _SURFACENORMALS,
         _BRANCHSAMPLES,
         _BRANCHSAMPLESAGR,
         _UNLIT,
         _NOMINDIELETRIC,
         _DEBUG_OUTPUT_ALBEDO,
         _DEBUG_OUTPUT_HEIGHT,
         _DEBUG_OUTPUT_NORMAL,
         _DEBUG_OUTPUT_METAL,
         _DEBUG_OUTPUT_SMOOTHNESS,
         _DEBUG_OUTPUT_AO,
         _DEBUG_OUTPUT_EMISSION,
         _DEBUG_OUTPUT_SPECULAR,
         _DEBUG_OUTPUT_MICROSHADOWS,
         _DEBUG_OUTPUT_SPLAT0,
         _DEBUG_OUTPUT_SPLAT1,
         _DEBUG_OUTPUT_SPLAT2,
         _DEBUG_OUTPUT_SPLAT3,
         _DEBUG_OUTPUT_SPLAT4,
         _DEBUG_OUTPUT_SPLAT5,
         _DEBUG_OUTPUT_SPLAT6,
         _DEBUG_OUTPUT_SPLAT7,
         _DEBUG_OUTPUT_SPLAT0A,
         _DEBUG_OUTPUT_SPLAT1A,
         _DEBUG_OUTPUT_SPLAT2A,
         _DEBUG_OUTPUT_SPLAT3A,
         _DEBUG_OUTPUT_SPLAT4A,
         _DEBUG_OUTPUT_SPLAT5A,
         _DEBUG_OUTPUT_SPLAT6A,
         _DEBUG_OUTPUT_SPLAT7A,
         _DEBUG_BRANCHCOUNT_WEIGHT,
         _DEBUG_BRANCHCOUNT_TRIPLANAR,
         _DEBUG_BRANCHCOUNT_CLUSTER,
         _DEBUG_BRANCHCOUNT_OTHER,
         _DEBUG_BRANCHCOUNT_TOTAL,
         _DEBUG_SAMPLECOUNT,
         _DEBUG_TRAXBUFFER,
         _DEBUG_PROCLAYERS,
         _DEBUG_WORLDNORMALVERTEX,
         _DEBUG_WORLDNORMAL,
         _DEBUG_FINALNORMALTANGENT,
         _DEBUG_DECAL_STATIC,
         _DEBUG_MEGABARY,
         _DEBUG_SNOWSPARKLE,
         _CUSTOMSPLATTEXTURES,
         kNumFeatures,
      }
         

      public enum MaxTextureCount
      {
         Four = 4,
         Eight = 8,
         Twelve = 12,
         Sixteen = 16,
         Twenty = 20,
         TwentyFour = 24,
         TwentyEight = 28,
         ThirtyTwo = 32,
      }

      public enum Workflow
      {
         UnityTerrain,
#if __MICROSPLAT_POLARIS__
         PolarisMesh,
#endif
#if __MICROSPLAT_MESHTERRAIN__
         MeshTerrain,
#endif
#if __MICROSPLAT_MESH__
         Mesh,
         VertexMesh,
#endif
#if __MICROSPLAT_MEGA__
         MegaSplatMesh,
         MegaSplatTerrain
#endif
      }

		public enum BranchSamples
      {
         None,
         Regular,
         Aggressive
      }

      public enum PerformanceMode
      {
         BestQuality,
         Balanced,
         Fastest
      }
         
      public enum UVMode
      {
         UV, 
         WorldSpace,
      }

      public enum LightingMode
      {
         Automatic = 0,
         BlinnPhong,
         Lambert,
         StandardShaderNoSheen
      }

      public enum URPLightingMode
      {
         Standard,
         Simple,
         Baked
      }

      public enum DebugOutput
      {
         None = 0,
         Albedo,
         Height,
         Normal,
         Metallic,
         Smoothness,
         AO,
         Emission,
         Specular,
         Unlit,
         MicroShadows,
         WorldNormal,
         WorldNormalVertex,
         FinalNormalTangent,
         BranchWeightCount,
         BranchTriplanarCount,
         BranchClusterCount,
         BranchOtherCount,
         BranchTotal,
         SampleCount,
#if __MICROSPLAT_TRAX__
         TraxBuffer,
#endif
#if __MICROSPLAT_DECAL__
         StaticDecals,
#endif
#if __MICROSPLAT_SNOW__
         SnowSparkle,
#endif
#if __MICROSPLAT_PROCTEX__
         ProceduralLayerCount,
         ProceduralSplatOutput0,
         ProceduralSplatOutput1,
         ProceduralSplatOutput2,
         ProceduralSplatOutput3,
         ProceduralSplatOutput4,
         ProceduralSplatOutput5,
         ProceduralSplatOutput6,
         ProceduralSplatOutput7,
         ProceduralSplatOutput0A,
         ProceduralSplatOutput1A,
         ProceduralSplatOutput2A,
         ProceduralSplatOutput3A,
         ProceduralSplatOutput4A,
         ProceduralSplatOutput5A,
         ProceduralSplatOutput6A,
         ProceduralSplatOutput7A,
#endif
#if __MICROSPLAT_MEGA__
          MegaBaryWeights,
#endif
      }

      public enum ShaderModel
      {
         Automatic,
         Force46,
         Force50
      }

      public enum SamplerMode
      {
         Default,
         LODSampler,
         GradientSampler
      }

      public enum BlendMode
      {
         HeightBlended,
         UnityLinear,
         HybridHeightLinear,
         NormalizedLinear
      }

      public enum NormalMode
      {
         Disabled,
         AutoNormal,
         Regular,
         SurfaceGradient
      }

      public bool disableMinDielectric;

      public BlendMode blendMode = BlendMode.HeightBlended;
      public bool useCustomSplatMaps = false;

      // state for the shader generation
      public PerformanceMode perfMode = PerformanceMode.BestQuality;
      public MaxTextureCount maxTextureCount = MaxTextureCount.Sixteen;
      public TextureArrayConfig.PackingMode packMode = TextureArrayConfig.PackingMode.Fastest;
      public TextureArrayConfig.PBRWorkflow pbrWorkflow = TextureArrayConfig.PBRWorkflow.Metallic;



      public NormalMode normalMode = NormalMode.Regular;
      public bool perTexMicroShadows;
      public bool perTexTint;
      public bool perTexBrightness;
      public bool perTexColorIntensity;
      public bool perTexContrast;
      public bool perTexSaturation;
      public bool perTexAOStr;
      public bool perTexNormStr;
      public bool perTexSmoothStr;
      public bool perTexMetallic;
      public bool perTexUVScale;
      public bool perTexUVRotation;
      public bool perTexInterpContrast;
      public bool perTexSSS;
      public bool perTexHeightOffset;
      public bool perTexHeightContrast;
      public bool perTexFuzzyShading;
      public bool perTexCurveWeight;
      public bool perTexRimLight;
      public bool perTexOutlines;
      public bool originShift;
      public bool controlNoiseUV;
      public bool emissiveArray = false;
      public UVMode uvMode = UVMode.UV;
      public bool perPixelNormal;
      public BranchSamples branchSamples = BranchSamples.Aggressive;


      public LightingMode lightingMode;
      public URPLightingMode lightingModeURP;
      public DebugOutput debugOutput = DebugOutput.None;
      public ShaderModel shaderModel = ShaderModel.Automatic;
      public SamplerMode samplerMode = SamplerMode.Default;

      public Workflow shaderType = Workflow.UnityTerrain;


		// files to include
		static TextAsset properties_splat;
      static TextAsset cbuffer;

      GUIContent CWorkflow = new GUIContent ("Shader Type", "What type of object is this shader going to be used on");
      GUIContent CPerTexMicroShadows = new GUIContent ("Micro Shadows", "Generate small shadow details from Normal/AO maps");
      GUIContent CInterpContrast = new GUIContent("Interpolation Contrast", "Controls how much height map based blending is used");
      GUIContent CHybridBlendDistance = new GUIContent ("Linear Blend Distance", "Distance at which blendin becomes linear instead of height blended");
      GUIContent CPackMode = new GUIContent("Packing Mode", "Mode in which the textures are packed (2 arrays for better speed, 3 arrays for better quality). Note if you use quality mode, you must set the same settings on your texture array and assign the newly generated _normal and _smoothnessao arrays to your material.");
      GUIContent CPBRWorlkflow = new GUIContent ("PBR Workflow", "Metallic or Specular workflow");
      GUIContent CShaderPerfMode = new GUIContent("Blend Quality", "Can be used to reduce the number of textures blended per pixel to increase speed. May create blending artifacts when set too low");
      GUIContent CMaxTexCount = new GUIContent("Max Texture Count", "How many textures your terrain is allowed to use - This allows you to optimize our the work of sampling the extra control textures, and should be set to the lowest value great than the number of textures sets used on your terrain");
      GUIContent CLightingMode = new GUIContent("Lighting Model", "Override Unity's automatic selection of a BDRF function to a fixed one. This will force the shader to render in forward rendering mode when not set to automatic, and drastically alter specular response in NoSheen mode");
      GUIContent CLightingModeURP = new GUIContent("Lighting Model", "Override URP lighting mode");
      GUIContent CHeightBlendMode = new GUIContent("Texture Blend Mode", "How should blending be performed between textures, using a height map, matching Unity, or a normalized linear blend");
      GUIContent CUVMode = new GUIContent("UV Mode", "Mode for Splat UV coordinates");
      GUIContent CForceShaderModel = new GUIContent("Shader Model", "Force a specific shader model to be used. By default, MicroSplat will use the minimum required shader model based on your shader settings. It's extremely rare that setting this to something other than default is necissary");
      GUIContent CSamplerMode = new GUIContent("Sampler Mode", "Force usage of manual mip selection in the shader (fast) or gradient samplers (slow). This will be forced to a non-default value when certain features are active, and usually you want Gradient, not LOD. See documentation for more info");
      GUIContent CEmissiveArray = new GUIContent("Emissive/Metallic Array", "Sample an emissive and metallic texture array");
      GUIContent CUseCustomSplatMaps = new GUIContent("Use Custom Splatmaps", "Use user provided splat maps instead of the ones unity generates");
      GUIContent CPerPixelNormal = new GUIContent("Per-Pixel Normal", "Allows you to generate and use a per-pixel normal. In 2018.3+ when using Draw Instanced, this is not necissary as Unity provdies the normal map automatically");
      GUIContent CNormalMode = new GUIContent("Normal Map Mode", "Allows you to set normals to be disabled, generated from the height map (AutoNormal), sampled and blended in the traditional way, or using the Surface Gradient framework");
      GUIContent CSSSDistance = new GUIContent ("Distance", "Distance of Subsurface Scattering");
      GUIContent CSSSPower = new GUIContent ("Power", "Power of Subsurface Scattering");
      GUIContent CSSSScale = new GUIContent ("Scale", "Scale of Subsurface Scattering");
      GUIContent CBranchSamples = new GUIContent ("Branch Samples", "When Branch Samples is on, dynamic flow control is used to cull un-needed texture samples, which can speed up the shader when memory bound. In basic mode, unused splat weights are culled. In aggressive mode, triplanar, stochastic, and other features are culled as well. There should be no visible difference when setting this setting, and usually you want aggresive");
      GUIContent COriginShift = new GUIContent ("Origin Shift", "Enabled a global origin shift for large worlds. Please read the docs on how this has to be set");
      GUIContent CControlNoiseUV = new GUIContent ("Control UV Noise", "Apply noise to the control UVs, which can break up linear filtering of splat maps");
      GUIContent CDisableDieletric = new GUIContent("Disable Minimum Dielectric", "By deault, Unity's lighting model uses a minimum metallic values of 0.04, which can cause shine at glancing angles");
      // Can we template these somehow?
      static Dictionary<DefineFeature, string> sFeatureNames = new Dictionary<DefineFeature, string>();
      public static string GetFeatureName(DefineFeature feature)
      {
         string ret;
         if (sFeatureNames.TryGetValue(feature, out ret))
         {
            return ret;
         }
         string fn = System.Enum.GetName(typeof(DefineFeature), feature);
         sFeatureNames[feature] = fn;
         return fn;
      }

      public static bool HasFeature(string[] keywords, DefineFeature feature)
      {
         string f = GetFeatureName(feature);
         for (int i = 0; i < keywords.Length; ++i)
         {
            if (keywords[i] == f)
               return true;
         }
         return false;
      }

      public static bool HasFeature (string [] keywords, string f)
      {
         for (int i = 0; i < keywords.Length; ++i)
         {
            if (keywords [i] == f)
               return true;
         }
         return false;
      }

      public override string GetVersion()
      {
         return MicroSplatShaderGUI.MicroSplatVersion;
      }

      public override void WriteFunctions(string [] features, System.Text.StringBuilder sb)
      {

      }


      void DoMaxTextureGUI(MicroSplatKeywords keywords)
      {
         var max = MaxTextureCount.ThirtyTwo;
#if __MICROSPLAT_MESH__ 
         if (shaderType == Workflow.VertexMesh)
         {
            max = MaxTextureCount.TwentyEight;
            if (maxTextureCount == MaxTextureCount.TwentyEight &&
               (keywords.IsKeywordEnabled ("_STREAMS") ||
               keywords.IsKeywordEnabled ("_LAVA") ||
               keywords.IsKeywordEnabled ("_WETNESS") ||
               keywords.IsKeywordEnabled ("_PUDDLES")))
            {
               max = MaxTextureCount.TwentyFour;
            }
         }
#endif
#if __MICROSPLAT_DIGGER__
         if (keywords.IsKeywordEnabled("_OUTPUTDIGGER"))
         {
            max = MaxTextureCount.TwentyEight;
            if (maxTextureCount == MaxTextureCount.TwentyEight &&
               (keywords.IsKeywordEnabled("_STREAMS") ||
               keywords.IsKeywordEnabled("_LAVA") ||
               keywords.IsKeywordEnabled("_WETNESS") ||
               keywords.IsKeywordEnabled("_PUDDLES")))
            {
               max = MaxTextureCount.TwentyFour;
            }
         }
#endif
         bool isMega = false;
#if __MICROSPLAT_MEGA__
         isMega = shaderType == Workflow.MegaSplatTerrain || shaderType == Workflow.MegaSplatMesh;
#endif
         if (!isMega)
         {
            maxTextureCount = (MaxTextureCount)EditorGUILayout.EnumPopup (CMaxTexCount, maxTextureCount);
         }
         if ((int)maxTextureCount > (int)max)
         {
            maxTextureCount = max;
         }

         if (!isMega)
         {
            var mat = MicroSplatShaderGUI.targetMat;
            if (mat != null && mat.HasProperty("_Diffuse"))
            {
               var ta = mat.GetTexture("_Diffuse") as Texture2DArray;
               if (ta != null && ta.depth > (int)maxTextureCount)
               {
                  EditorGUILayout.HelpBox("Max Texture Count is lower than the number of textures in the array. This will prevent you from painting those textures, please increase this value", MessageType.Warning);
               }
               if (ta != null && ta.depth < ((int)maxTextureCount) - 4)
               {
                  EditorGUILayout.HelpBox("Max Texture Count is " + (int)maxTextureCount + " but your array only has " + ta.depth + " textures, this is wasting performance", MessageType.Warning);
               }
            }
         }
      }

      void DoBranchSamplesGUI(MicroSplatKeywords keywords)
      {
         bool agr = keywords.IsKeywordEnabled ("_TRIPLANAR") || keywords.IsKeywordEnabled ("_STOCHASTIC") || keywords.IsKeywordEnabled ("_TEXTURECLUSTER2") || keywords.IsKeywordEnabled ("_TEXTURECLUSTER3") || keywords.IsKeywordEnabled ("_DISTANCERESAMPLE");

         if (branchSamples != BranchSamples.Aggressive && agr)
         {
            using (new GUILayout.VerticalScope (GUI.skin.box))
            {
               EditorGUILayout.HelpBox ("Setting Branch Samples to aggressive is highly recomended, as this will make the shader much faster at no visual quality loss", MessageType.Warning);
               branchSamples = (BranchSamples)EditorGUILayout.EnumPopup (CBranchSamples, branchSamples);
            }
         }
         else if (branchSamples == BranchSamples.None)
         {
            using (new GUILayout.VerticalScope (GUI.skin.box))
            {
               EditorGUILayout.HelpBox ("Turning on Branch Sampling is highly recomended, as it will make the shader faster with no visual quality loss", MessageType.Warning);
               branchSamples = (BranchSamples)EditorGUILayout.EnumPopup (CBranchSamples, branchSamples);
            }
         }
         else
         {
            branchSamples = (BranchSamples)EditorGUILayout.EnumPopup (CBranchSamples, branchSamples);
         }
      }

      bool advancedState = false;
      public override void DrawFeatureGUI(MicroSplatKeywords keywords)
      {
         if (keywords.IsKeywordEnabled ("_DISABLESPLATMAPS"))
            return;
         bool isBIRP = keywords.IsKeywordEnabled("_MSRENDERLOOP_SURFACESHADER");
         shaderType = (Workflow)EditorGUILayout.EnumPopup(CWorkflow, shaderType);
         pbrWorkflow = (TextureArrayConfig.PBRWorkflow)EditorGUILayout.EnumPopup (CPBRWorlkflow, pbrWorkflow);

         DoMaxTextureGUI (keywords);

         DoBranchSamplesGUI (keywords);

         
         uvMode = (UVMode)EditorGUILayout.EnumPopup(CUVMode, uvMode);
         
         emissiveArray = EditorGUILayout.Toggle(CEmissiveArray, emissiveArray);
         if (!keywords.IsKeywordEnabled ("_MICROVERTEXMESH") && !keywords.IsKeywordEnabled("_MICROMESH"))
         {
            perPixelNormal = EditorGUILayout.Toggle (CPerPixelNormal, perPixelNormal);
         }
         else
         {
            perPixelNormal = false;
         }
         
         blendMode = (BlendMode)EditorGUILayout.EnumPopup(CHeightBlendMode, blendMode);
         
         normalMode = (NormalMode)EditorGUILayout.EnumPopup(CNormalMode, normalMode);
         controlNoiseUV = EditorGUILayout.Toggle (CControlNoiseUV, controlNoiseUV);

         // ok, a bit hackish for my taste, but don't want to chnage the API.
         // should pass in the editor instead, or a ref to this, so you can do this cleaner
         // but even then it assumes the change check is one deep, so.. ugh.
         MicroSplatShaderGUI.needsCompile = MicroSplatShaderGUI.needsCompile || EditorGUI.EndChangeCheck ();
         advancedState = (EditorGUILayout.Foldout (advancedState, "Advanced"));
         EditorGUI.BeginChangeCheck ();

         if (advancedState)
         {
            using (new GUILayout.VerticalScope (GUI.skin.box))
            {
               EditorGUILayout.HelpBox ("You should only use things in this section if you really understand them. I often see people flicking these settings to different values, then coming to me asking why something doesn't work right, because they didn't read the docs and randomly set one of these setting without knowing what they do", MessageType.Info);
               EditorGUI.indentLevel++;

               packMode = (TextureArrayConfig.PackingMode)EditorGUILayout.EnumPopup (CPackMode, packMode);
               perfMode = (PerformanceMode)EditorGUILayout.EnumPopup (CShaderPerfMode, perfMode);

               if (isBIRP)
               {
                  lightingMode = (LightingMode)EditorGUILayout.EnumPopup (CLightingMode, lightingMode);
                  if (lightingMode != LightingMode.Automatic && lightingMode != LightingMode.StandardShaderNoSheen)
                  {
                     EditorGUILayout.HelpBox ("Shader is forced to run in forward rendering due to lighting mode", MessageType.Info);
                  }
               }
               else if (keywords.IsKeywordEnabled("_MSRENDERLOOP_UNITYURP") ||
                  keywords.IsKeywordEnabled("_MSRENDERLOOP_UNITYURP2020") ||
                  keywords.IsKeywordEnabled("_MSRENDERLOOP_UNITYURP2021"))
               {
                  lightingModeURP = (URPLightingMode)EditorGUILayout.EnumPopup(CLightingModeURP, lightingModeURP);
               }

               if (isBIRP)
               {
                  disableMinDielectric = EditorGUILayout.Toggle(CDisableDieletric, disableMinDielectric);
               }

               shaderModel = (ShaderModel)EditorGUILayout.EnumPopup (CForceShaderModel, shaderModel);
               samplerMode = (SamplerMode)EditorGUILayout.EnumPopup (CSamplerMode, samplerMode);
               if (shaderType == Workflow.UnityTerrain)
               {
                  useCustomSplatMaps = EditorGUILayout.Toggle (CUseCustomSplatMaps, useCustomSplatMaps);
               }
               originShift = EditorGUILayout.Toggle (COriginShift, originShift);
               debugOutput = (DebugOutput)EditorGUILayout.EnumPopup ("Debug", debugOutput);
               EditorGUI.indentLevel--;
            }
         }

         
      }

      static GUIContent CAlbedoTex = new GUIContent("Albedo/Height Array", "Texture Array which contains albedo and height information");
      static GUIContent CNormalSpec = new GUIContent("Normal/Smooth/AO Array", "Texture Array with normal, smoothness, and ambient occlusion");
      static GUIContent CNormal = new GUIContent("Normal Array", "Texture Array with normals");
      static GUIContent CEmisMetal = new GUIContent("Emissive/Metal Array", "Texture Array with emissive and metalic values");
      static GUIContent CSmoothAO = new GUIContent("Smoothness/AO Array", "Texture Array with Smoothness and AO");
      static GUIContent CSpecular = new GUIContent ("Specular Array", "Specular Color array");
      static GUIContent CEmissiveMultiplier = new GUIContent ("Emissive Multiplier", "Increase/decrease strength of emission");
      static GUIContent CNoiseUV = new GUIContent ("UV Noise Texture", "Texture for noise lookup");

      public override void DrawShaderGUI(MicroSplatShaderGUI shaderGUI, MicroSplatKeywords keywords, Material mat, MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (!keywords.IsKeywordEnabled ("_DISABLESPLATMAPS"))
         {
            if (MicroSplatUtilities.DrawRollup ("Splats"))
            {
               var albedoMap = shaderGUI.FindProp ("_Diffuse", props);
               var normalMap = shaderGUI.FindProp ("_NormalSAO", props);
               materialEditor.TexturePropertySingleLine (CAlbedoTex, albedoMap);
               if (normalMode != NormalMode.Disabled && normalMode != NormalMode.AutoNormal)
               {
                  if (packMode == TextureArrayConfig.PackingMode.Fastest)
                  {
                     materialEditor.TexturePropertySingleLine (CNormalSpec, normalMap);
                  }
                  else
                  {
                     materialEditor.TexturePropertySingleLine (CNormal, normalMap);
                  }
                  
               }

               if (normalMap.textureValue != null)
               {
                  bool sao = (normalMap.textureValue.name.EndsWith ("_normSAO_tarray"));
                  bool normal = (normalMap.textureValue.name.EndsWith ("_normal_tarray"));
                  if (packMode == TextureArrayConfig.PackingMode.Fastest && normal)
                  {
                     EditorGUILayout.HelpBox ("Packing mode is set to Fastest and it looks like you have a texture array without smoothness and ao baked into a normalSAO array. Please assign an NSAO packed array instead", MessageType.Warning);
                  }
                  else if (packMode == TextureArrayConfig.PackingMode.Quality && sao)
                  {
                     EditorGUILayout.HelpBox ("Packing mode is set to quality and normalSAO array is assigned. You want to set your Texture Array Config to Quality and assign the _normal array instead", MessageType.Warning);
                  }
               }
               if (normalMode == NormalMode.AutoNormal)
               {
                  if (mat.HasProperty ("_AutoNormalHeightScale"))
                  {
                     materialEditor.RangeProperty (shaderGUI.FindProp ("_AutoNormalHeightScale", props), "Auto Normal Height");
                  }
               }
               if (pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular && mat.HasProperty ("_Specular"))
               {
                  var specMap = shaderGUI.FindProp ("_Specular", props);
                  materialEditor.TexturePropertySingleLine (CSpecular, specMap);
               }

               if (emissiveArray && mat.HasProperty ("_EmissiveMetal"))
               {
                  var emisMap = shaderGUI.FindProp ("_EmissiveMetal", props);
                  materialEditor.TexturePropertySingleLine (CEmisMetal, emisMap);
                  
               }
               if (mat.HasProperty("_EmissiveMult"))
               {
                  var emisMult = shaderGUI.FindProp("_EmissiveMult", props);
                  emisMult.floatValue = EditorGUILayout.FloatField(CEmissiveMultiplier, emisMult.floatValue);
                  
               }
               if (mat.HasProperty("_EmissiveMult") || mat.HasProperty("_LavaEmissiveMult"))
               {
                  materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true, true);
               }

               if (packMode == TextureArrayConfig.PackingMode.Quality && mat.HasProperty ("_SmoothAO"))
               {
                  var smoothAO = shaderGUI.FindProp ("_SmoothAO", props);
                  materialEditor.TexturePropertySingleLine (CSmoothAO, smoothAO);
               }

               if (blendMode == BlendMode.HeightBlended || blendMode == BlendMode.HybridHeightLinear)
               {
                  var contrastProp = shaderGUI.FindProp ("_Contrast", props);
                  contrastProp.floatValue = EditorGUILayout.Slider (CInterpContrast, contrastProp.floatValue, 1.0f, 0.0001f);
                  if (blendMode == BlendMode.HybridHeightLinear && mat.HasProperty("_HybridHeightBlendDistance"))
                  {
                     var distProp = shaderGUI.FindProp ("_HybridHeightBlendDistance", props);
                     distProp.floatValue = EditorGUILayout.Slider (CHybridBlendDistance, distProp.floatValue, 40, 4000f);
                  }
               }


               if (!keywords.IsKeywordEnabled ("_TRIPLANAR"))
               {
                  EditorGUI.BeginChangeCheck ();
                  Vector4 uvScale = shaderGUI.FindProp ("_UVScale", props).vectorValue;
                  Vector2 scl = new Vector2 (uvScale.x, uvScale.y);
                  Vector2 offset = new Vector2 (uvScale.z, uvScale.w);
                  scl = EditorGUILayout.Vector2Field ("Global UV Scale", scl);
                  offset = EditorGUILayout.Vector2Field ("Global UV Offset", offset);
                  if (EditorGUI.EndChangeCheck ())
                  {
                     uvScale.x = scl.x;
                     uvScale.y = scl.y;
                     uvScale.z = offset.x;
                     uvScale.w = offset.y;
                     shaderGUI.FindProp ("_UVScale", props).vectorValue = uvScale;
                     EditorUtility.SetDirty (mat);
                  }
               }

               if (mat.HasProperty("_SampleCountDiv"))
               {
                  materialEditor.FloatProperty (shaderGUI.FindProp ("_SampleCountDiv", props), "Debug Sample Divisor");
               }

               if (mat.HasProperty ("_NoiseUVParams"))
               {
                  var tex = shaderGUI.FindProp ("_NoiseUV", props);
                  materialEditor.TexturePropertySingleLine (CNoiseUV, tex);
                  MicroSplatUtilities.EnforceDefaultTexture (tex, "microsplat_def_perlin4");
                  Vector4 noise = mat.GetVector ("_NoiseUVParams");

                  EditorGUI.BeginChangeCheck ();
                  noise.x = EditorGUILayout.FloatField ("UV Noise Frequency", noise.x);
                  noise.y = EditorGUILayout.FloatField ("UV Noise Amplitude", noise.y);

                  if (EditorGUI.EndChangeCheck ())
                  {
                     mat.SetVector ("_NoiseUVParams", noise);
                     EditorUtility.SetDirty (mat);
                  }
               }
               materialEditor.TexturePropertySingleLine (new GUIContent("Property LUT"), shaderGUI.FindProp ("_PerTexProps", props));
            }
         }

         materialEditor.RenderQueueField();

         if (mat.HasProperty ("_SSSPower"))
         {
            if (MicroSplatUtilities.DrawRollup ("Subsurface Scattering"))
            {
               var distance = shaderGUI.FindProp ("_SSSDistance", props);
               var power = shaderGUI.FindProp ("_SSSPower", props);
               var scale = shaderGUI.FindProp ("_SSSScale", props);
               distance.floatValue = EditorGUILayout.FloatField (CSSSDistance, distance.floatValue);
               scale.floatValue = EditorGUILayout.FloatField (CSSSScale, scale.floatValue);
               power.floatValue = EditorGUILayout.FloatField (CSSSPower, power.floatValue);
            }
         }
      }

      public override string[] Pack()
      {
         List<string> features = new List<string>();
         features.Add(GetFeatureName(DefineFeature._MICROSPLAT));
         if (shaderType == Workflow.UnityTerrain)
         {
            features.Add (GetFeatureName (DefineFeature._MICROTERRAIN));
         }
#if __MICROSPLAT_MESHTERRAIN__
         if (shaderType == Workflow.MeshTerrain)
         {
            features.Add(GetFeatureName(DefineFeature._MICROMESHTERRAIN));
         }
#endif
#if __MICROSPLAT_MESH__
         if (shaderType == Workflow.Mesh)
         {
            features.Add(GetFeatureName(DefineFeature._MICROMESH));
         }
         if (shaderType == Workflow.VertexMesh)
         {
            features.Add(GetFeatureName(DefineFeature._MICROVERTEXMESH));
            useCustomSplatMaps = false;
         }
#endif
#if __MICROSPLAT_POLARIS__
         if (shaderType == Workflow.PolarisMesh)
         {
            features.Add (GetFeatureName (DefineFeature._MICROPOLARISMESH));
         }
#endif
#if __MICROSPLAT_MEGA__
         if (shaderType == Workflow.MegaSplatMesh)
         {
            features.Add (GetFeatureName (DefineFeature._MEGASPLAT));
         }
         else if (shaderType == Workflow.MegaSplatTerrain)
         {
            features.Add(GetFeatureName(DefineFeature._MEGASPLATTEXTURE));
            features.Add(GetFeatureName(DefineFeature._MICROTERRAIN)); // make it a terrain as well
         }
#endif
         // force gradient sampler sometimes
         if (samplerMode == SamplerMode.Default)
         {
            if (perTexUVScale || branchSamples != BranchSamples.None)
            {
               samplerMode = SamplerMode.GradientSampler;
            }
         }
         if (originShift)
         {
            features.Add (GetFeatureName (DefineFeature._ORIGINSHIFT));
         }
         
         if (perTexMicroShadows)
         {
            features.Add (GetFeatureName (DefineFeature._PERTEXMICROSHADOWS));
         }
         if (perTexRimLight)
         {
            features.Add (GetFeatureName (DefineFeature._PERTEXRIMLIGHT));
         }
         if (blendMode == BlendMode.NormalizedLinear)
         {
            features.Add (GetFeatureName (DefineFeature._DISABLEHEIGHTBLENDING));
            features.Add (GetFeatureName (DefineFeature._NORMALIZEWEIGHTS));
         }
         else if (blendMode == BlendMode.UnityLinear)
         {
            features.Add (GetFeatureName (DefineFeature._DISABLEHEIGHTBLENDING));
         }
         else if (blendMode == BlendMode.HybridHeightLinear)
         {
            features.Add (GetFeatureName (DefineFeature._HYBRIDHEIGHTBLEND));
         }

         if (pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
         {
            features.Add (GetFeatureName (DefineFeature._USESPECULARWORKFLOW));
         }
         if (disableMinDielectric)
         {
            features.Add(GetFeatureName(DefineFeature._NOMINDIELETRIC));
         }

         if (useCustomSplatMaps)
         {
            features.Add(GetFeatureName(DefineFeature._CUSTOMSPLATTEXTURES));
         }
         if (perTexOutlines)
         {
            features.Add (GetFeatureName (DefineFeature._PERTEXOUTLINECOLOR));
         }

         if (normalMode == NormalMode.Disabled)
         {
            features.Add(GetFeatureName(DefineFeature._NONORMALMAP));
         }
         else if (normalMode == NormalMode.SurfaceGradient)
         {
            features.Add (GetFeatureName (DefineFeature._SURFACENORMALS));
         }
         else if (normalMode == NormalMode.AutoNormal)
         {
            features.Add (GetFeatureName (DefineFeature._AUTONORMAL));
         }
         
         if (samplerMode == SamplerMode.LODSampler)
         {
            features.Add(GetFeatureName(DefineFeature._USELODMIP));
         }
         else if (samplerMode == SamplerMode.GradientSampler)
         {
            features.Add(GetFeatureName(DefineFeature._USEGRADMIP));
         }
         if (packMode == TextureArrayConfig.PackingMode.Quality)
         {
            features.Add(GetFeatureName(DefineFeature._PACKINGHQ));
         }
         
         if (emissiveArray)
         {
            features.Add(GetFeatureName(DefineFeature._USEEMISSIVEMETAL));
         }

         if (perfMode == PerformanceMode.Balanced)
         {
            features.Add(GetFeatureName(DefineFeature._MAX3LAYER));
         }
         else if (perfMode == PerformanceMode.Fastest)
         {
            features.Add(GetFeatureName(DefineFeature._MAX2LAYER));
         }
         if (maxTextureCount == MaxTextureCount.Four)
         {
            features.Add(GetFeatureName(DefineFeature._MAX4TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.Eight)
         {
            features.Add(GetFeatureName(DefineFeature._MAX8TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.Twelve)
         {
            features.Add(GetFeatureName(DefineFeature._MAX12TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.Twenty)
         {
            features.Add(GetFeatureName(DefineFeature._MAX20TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.TwentyFour)
         {
            features.Add(GetFeatureName(DefineFeature._MAX24TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.TwentyEight)
         {
            features.Add(GetFeatureName(DefineFeature._MAX28TEXTURES));
         }
         else if (maxTextureCount == MaxTextureCount.ThirtyTwo)
         {
            features.Add(GetFeatureName(DefineFeature._MAX32TEXTURES));
         }

         if (perPixelNormal)
         {
            features.Add(GetFeatureName(DefineFeature._PERPIXNORMAL));
         }
         if (lightingMode == LightingMode.StandardShaderNoSheen)
         {
            features.Add (GetFeatureName (DefineFeature._SPECULARFROMMETALLIC));
         }
         else if (lightingMode == LightingMode.BlinnPhong)
         {
            features.Add(GetFeatureName(DefineFeature._BDRF3));
         }
         else if (lightingMode == LightingMode.Lambert)
         {
            features.Add(GetFeatureName(DefineFeature._BDRFLAMBERT));
         }
         if (lightingModeURP == URPLightingMode.Simple)
         {
            features.Add("_SIMPLELIT");
         }
         else if (lightingModeURP == URPLightingMode.Baked)
         {
            features.Add("_BAKEDLIT");
         }

         if (perTexUVScale)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXUVSCALEOFFSET));
         }
         if (perTexUVRotation)
         {
            features.Add(GetFeatureName (DefineFeature._PERTEXUVROTATION));
         }
         if (perTexFuzzyShading)
         {
            features.Add (GetFeatureName (DefineFeature._PERTEXFUZZYSHADE));
         }
         if (perTexSSS)
         {
            features.Add (GetFeatureName (DefineFeature._PERTEXSSS));
         }
         if (perTexHeightOffset)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXHEIGHTOFFSET));
         }
         if (perTexHeightContrast)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXHEIGHTCONTRAST));
         }

         if (uvMode == UVMode.WorldSpace)
         {
            features.Add(GetFeatureName(DefineFeature._WORLDUV));
         }

         if (perTexSaturation)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXSATURATION));
         }

         if (perTexInterpContrast)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXINTERPCONTRAST));
         }
         if (perTexTint)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXTINT));
         }
         if (perTexBrightness)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXBRIGHTNESS));
         }
         if (perTexColorIntensity)
         {
            features.Add (GetFeatureName (DefineFeature._PERTEXCOLORINTENSITY));
         }
         if (perTexContrast)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXCONTRAST));
         }
         if (perTexAOStr)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXAOSTR));
         }
         if (perTexNormStr)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXNORMSTR));
         }
         if (perTexSmoothStr)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXSMOOTHSTR));
         }
         if (perTexMetallic)
         {
            features.Add(GetFeatureName(DefineFeature._PERTEXMETALLIC));
         }
         if (shaderModel != ShaderModel.Automatic)
         {
            if (shaderModel == ShaderModel.Force46)
            {
               features.Add(GetFeatureName(DefineFeature._FORCEMODEL46));
            }
            else
            {
               features.Add(GetFeatureName(DefineFeature._FORCEMODEL50));
            }
         }
         if (controlNoiseUV)
         {
            features.Add (GetFeatureName (DefineFeature._CONTROLNOISEUV));
         }
         if (perTexCurveWeight)
         {
            features.Add (GetFeatureName (DefineFeature._PERTEXCURVEWEIGHT));
         }

         if (branchSamples == BranchSamples.Regular)
         {
            features.Add (GetFeatureName (DefineFeature._BRANCHSAMPLES));
         }
         else if (branchSamples == BranchSamples.Aggressive)
         {
            features.Add (GetFeatureName (DefineFeature._BRANCHSAMPLES));
            features.Add (GetFeatureName (DefineFeature._BRANCHSAMPLESAGR));
         }

         if (debugOutput != DebugOutput.None)
         {
            if (debugOutput == DebugOutput.Albedo)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_ALBEDO));
            }
            else if (debugOutput == DebugOutput.Height)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_HEIGHT));
            }
            else if (debugOutput == DebugOutput.Normal)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_NORMAL));
            }
            else if (debugOutput == DebugOutput.Metallic)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_METAL));
            }
            else if (debugOutput == DebugOutput.Smoothness)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SMOOTHNESS));
            }
            else if (debugOutput == DebugOutput.AO)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_AO));
            }
            else if (debugOutput == DebugOutput.Emission)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_EMISSION));
            }
            else if (debugOutput == DebugOutput.Specular)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_OUTPUT_SPECULAR));
            }
            else if (debugOutput == DebugOutput.MicroShadows)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_OUTPUT_MICROSHADOWS));
            }
            else if (debugOutput == DebugOutput.BranchWeightCount)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_BRANCHCOUNT_WEIGHT));
            }
            else if (debugOutput == DebugOutput.BranchTriplanarCount)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_BRANCHCOUNT_TRIPLANAR));
            }
            else if (debugOutput == DebugOutput.BranchClusterCount)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_BRANCHCOUNT_CLUSTER));
            }
            else if (debugOutput == DebugOutput.BranchOtherCount)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_BRANCHCOUNT_OTHER));
            }
            else if (debugOutput == DebugOutput.BranchTotal)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_BRANCHCOUNT_TOTAL));
            }
            else if (debugOutput == DebugOutput.SampleCount)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_SAMPLECOUNT));
            }
            else if (debugOutput == DebugOutput.WorldNormal)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_WORLDNORMAL));
            }
            else if (debugOutput == DebugOutput.WorldNormalVertex)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_WORLDNORMALVERTEX));
            }
            else if (debugOutput == DebugOutput.FinalNormalTangent)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_FINALNORMALTANGENT));
            }
            else if (debugOutput == DebugOutput.Unlit)
            {
               features.Add (GetFeatureName (DefineFeature._UNLIT));
            }
#if __MICROSPLAT_SNOW__
            else if (debugOutput == DebugOutput.SnowSparkle)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_SNOWSPARKLE));
            }
#endif
#if __MICROSPLAT_TRAX__
            else if (debugOutput == DebugOutput.TraxBuffer)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_TRAXBUFFER));
            }
#endif
#if __MICROSPLAT_DECAL__
            else if (debugOutput == DebugOutput.StaticDecals)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_DECAL_STATIC));
            }
#endif

#if __MICROSPLAT_PROCTEX__
            else if (debugOutput == DebugOutput.ProceduralSplatOutput0)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT0));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput1)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT1));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput2)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT2));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput3)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT3));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput4)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT4));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput5)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT5));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput6)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT6));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput7)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT7));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput0A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT0A));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput1A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT1A));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput2A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT2A));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput3A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT3A));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput4A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT4A));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput5A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT5A));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput6A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT6A));
            }
            else if (debugOutput == DebugOutput.ProceduralSplatOutput7A)
            {
               features.Add(GetFeatureName(DefineFeature._DEBUG_OUTPUT_SPLAT7A));
            }
            else if (debugOutput == DebugOutput.ProceduralLayerCount)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_PROCLAYERS));
            }
#endif
#if __MICROSPLAT_MEGA__
             else if (debugOutput == DebugOutput.MegaBaryWeights)
            {
               features.Add (GetFeatureName (DefineFeature._DEBUG_MEGABARY));
            }
#endif
         }
         return features.ToArray();
      }

      public override void Unpack(string[] keywords)
      {
         if (HasFeature(keywords, DefineFeature._MAX2LAYER))
         {
            perfMode = PerformanceMode.Fastest;
         }
         else if (HasFeature(keywords, DefineFeature._MAX3LAYER))
         {
            perfMode = PerformanceMode.Balanced;
         }
         else
         {
            perfMode = PerformanceMode.BestQuality;
         }

         useCustomSplatMaps = (HasFeature(keywords, DefineFeature._CUSTOMSPLATTEXTURES));

         shaderType = Workflow.UnityTerrain;

#if __MICROSPLAT_MESH__
         if (HasFeature(keywords, DefineFeature._MICROMESH))
         {
            shaderType = Workflow.Mesh;
            useCustomSplatMaps = false;
         }
         if (HasFeature(keywords, DefineFeature._MICROVERTEXMESH))
         {
            shaderType = Workflow.VertexMesh;
            useCustomSplatMaps = false;
         }
#endif
#if __MICROSPLAT_MESHTERRAIN__
         if (HasFeature(keywords, DefineFeature._MICROMESHTERRAIN))
         {
            shaderType = Workflow.MeshTerrain;
            useCustomSplatMaps = false;
         }

#endif

#if __MICROSPLAT_POLARIS__
         if (HasFeature(keywords, DefineFeature._MICROPOLARISMESH))
         {
            shaderType = Workflow.PolarisMesh;
            useCustomSplatMaps = false;
         }
#endif
#if __MICROSPLAT_MEGA__
         if (HasFeature (keywords, DefineFeature._MEGASPLAT))
         {
            shaderType = Workflow.MegaSplatMesh;
            useCustomSplatMaps = false;
         }
         else if (HasFeature(keywords, DefineFeature._MEGASPLATTEXTURE))
         {
            shaderType = Workflow.MegaSplatTerrain;
            useCustomSplatMaps = false;
         }
#endif
         normalMode = NormalMode.Regular;

         if (HasFeature(keywords, DefineFeature._NONORMALMAP) || HasFeature(keywords, "NONOMALMAP"))
         {
            normalMode = NormalMode.Disabled;
         }
         else if (HasFeature(keywords, DefineFeature._AUTONORMAL))
         {
            normalMode = NormalMode.AutoNormal;
         }
         else if (HasFeature(keywords, DefineFeature._SURFACENORMALS))
         {
            normalMode = NormalMode.SurfaceGradient;
         }

         packMode = HasFeature(keywords, DefineFeature._PACKINGHQ) ? TextureArrayConfig.PackingMode.Quality : TextureArrayConfig.PackingMode.Fastest;
         if (HasFeature(keywords, DefineFeature._USESPECULARWORKFLOW))
         {
            pbrWorkflow = TextureArrayConfig.PBRWorkflow.Specular;
         }
         else
         {
            pbrWorkflow = TextureArrayConfig.PBRWorkflow.Metallic;
         }
         disableMinDielectric = false;
         if (HasFeature(keywords, DefineFeature._NOMINDIELETRIC))
         {
            disableMinDielectric = true;
         }

         originShift = HasFeature (keywords, DefineFeature._ORIGINSHIFT);
         emissiveArray = HasFeature(keywords, DefineFeature._USEEMISSIVEMETAL);
         samplerMode = SamplerMode.Default;
         if (HasFeature(keywords, DefineFeature._USELODMIP))
         {
            samplerMode = SamplerMode.LODSampler;
         }
         else if (HasFeature(keywords, DefineFeature._USEGRADMIP))
         {
            samplerMode = SamplerMode.GradientSampler;
         }
         // force gradient sampling for stochastic mode
         if (samplerMode == SamplerMode.Default && System.Array.Exists(keywords, e => e == "_STOCHASTIC"))
         {
            samplerMode = SamplerMode.GradientSampler;
         }

         perPixelNormal = HasFeature(keywords, DefineFeature._PERPIXNORMAL);
         uvMode = HasFeature(keywords, DefineFeature._WORLDUV) ? UVMode.WorldSpace : UVMode.UV;
         
         perTexHeightOffset = HasFeature(keywords, DefineFeature._PERTEXHEIGHTOFFSET);
         perTexHeightContrast = HasFeature(keywords, DefineFeature._PERTEXHEIGHTCONTRAST);

         if (HasFeature(keywords, DefineFeature._MAX4TEXTURES))
         {
            maxTextureCount = MaxTextureCount.Four;
         }
         else if (HasFeature(keywords, DefineFeature._MAX8TEXTURES))
         {
            maxTextureCount = MaxTextureCount.Eight;
         }
         else if (HasFeature(keywords, DefineFeature._MAX12TEXTURES))
         {
            maxTextureCount = MaxTextureCount.Twelve;
         }
         else if (HasFeature(keywords, DefineFeature._MAX20TEXTURES))
         {
            maxTextureCount = MaxTextureCount.Twenty;
         }
         else if (HasFeature(keywords, DefineFeature._MAX24TEXTURES))
         {
            maxTextureCount = MaxTextureCount.TwentyFour;
         }
         else if (HasFeature(keywords, DefineFeature._MAX28TEXTURES))
         {
            maxTextureCount = MaxTextureCount.TwentyEight;
         }
         else if (HasFeature(keywords, DefineFeature._MAX32TEXTURES))
         {
            maxTextureCount = MaxTextureCount.ThirtyTwo;
         }
         else
         {
            maxTextureCount = MaxTextureCount.Sixteen;
         }

         controlNoiseUV = HasFeature (keywords, DefineFeature._CONTROLNOISEUV);

         blendMode = BlendMode.HeightBlended;
         
         if (HasFeature(keywords, DefineFeature._DISABLEHEIGHTBLENDING))
         {
            blendMode = BlendMode.UnityLinear;
            if (HasFeature(keywords, DefineFeature._NORMALIZEWEIGHTS))
            {
               blendMode = BlendMode.NormalizedLinear;
            }
         }
         if (HasFeature(keywords, DefineFeature._HYBRIDHEIGHTBLEND))
         {
            blendMode = BlendMode.HybridHeightLinear;
         }

         lightingMode = LightingMode.Automatic;
         if (HasFeature (keywords, DefineFeature._SPECULARFROMMETALLIC))
         {
            lightingMode = LightingMode.StandardShaderNoSheen;
         }
         else if (HasFeature(keywords, DefineFeature._BDRF3))
         {
            lightingMode = LightingMode.BlinnPhong;
         }
         else if (HasFeature(keywords, DefineFeature._BDRFLAMBERT))
         {
            lightingMode = LightingMode.Lambert;
         }

         branchSamples = BranchSamples.None;
         if (HasFeature (keywords, DefineFeature._BRANCHSAMPLES))
         {
            branchSamples = BranchSamples.Regular;
         }
         if (HasFeature(keywords, DefineFeature._BRANCHSAMPLESAGR))
         {
            branchSamples = BranchSamples.Aggressive;
         }

         lightingModeURP = URPLightingMode.Standard;
         if (HasFeature(keywords, "_SIMPLELIT"))
         {
            lightingModeURP = URPLightingMode.Simple;
         }
         else if (HasFeature(keywords, "_BAKEDLIT"))
         {
            lightingModeURP = URPLightingMode.Baked;
         }

         perTexUVScale = (HasFeature(keywords, DefineFeature._PERTEXUVSCALEOFFSET));
         perTexUVRotation = (HasFeature(keywords, DefineFeature._PERTEXUVROTATION));
         perTexInterpContrast = HasFeature(keywords, DefineFeature._PERTEXINTERPCONTRAST);
         perTexBrightness = HasFeature(keywords, DefineFeature._PERTEXBRIGHTNESS);
         perTexColorIntensity = HasFeature (keywords, DefineFeature._PERTEXCOLORINTENSITY);
         perTexContrast = HasFeature(keywords, DefineFeature._PERTEXCONTRAST);
         perTexSaturation = HasFeature(keywords, DefineFeature._PERTEXSATURATION);
         perTexAOStr = (HasFeature(keywords, DefineFeature._PERTEXAOSTR));
         perTexMetallic = (HasFeature(keywords, DefineFeature._PERTEXMETALLIC));
         perTexNormStr = (HasFeature(keywords, DefineFeature._PERTEXNORMSTR));
         perTexSmoothStr = (HasFeature(keywords, DefineFeature._PERTEXSMOOTHSTR));
         perTexTint = (HasFeature(keywords, DefineFeature._PERTEXTINT));
         perTexFuzzyShading = (HasFeature (keywords, DefineFeature._PERTEXFUZZYSHADE));
         perTexSSS = (HasFeature (keywords, DefineFeature._PERTEXSSS));
         perTexCurveWeight = HasFeature (keywords, DefineFeature._PERTEXCURVEWEIGHT);
         perTexRimLight = HasFeature (keywords, DefineFeature._PERTEXRIMLIGHT);
         perTexOutlines = HasFeature (keywords, DefineFeature._PERTEXOUTLINECOLOR);
         shaderModel = ShaderModel.Automatic;
         if (HasFeature(keywords, DefineFeature._FORCEMODEL46))
         {
            shaderModel = ShaderModel.Force46;
         }
         if (HasFeature(keywords, DefineFeature._FORCEMODEL50))
         {
            shaderModel = ShaderModel.Force50;
         }

         debugOutput = DebugOutput.None;
         if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_ALBEDO))
         {
            debugOutput = DebugOutput.Albedo;
         }
         else if (HasFeature(keywords, DefineFeature._UNLIT))
         {
            debugOutput = DebugOutput.Unlit;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_HEIGHT))
         {
            debugOutput = DebugOutput.Height;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_NORMAL))
         {
            debugOutput = DebugOutput.Normal;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SMOOTHNESS))
         {
            debugOutput = DebugOutput.Smoothness;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_METAL))
         {
            debugOutput = DebugOutput.Metallic;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_AO))
         {
            debugOutput = DebugOutput.AO;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_EMISSION))
         {
            debugOutput = DebugOutput.Emission;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPECULAR))
         {
            debugOutput = DebugOutput.Specular;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_MICROSHADOWS))
         {
            debugOutput = DebugOutput.MicroShadows;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_BRANCHCOUNT_WEIGHT))
         {
            debugOutput = DebugOutput.BranchWeightCount;
         }
         else if (HasFeature (keywords, DefineFeature._DEBUG_BRANCHCOUNT_TRIPLANAR))
         {
            debugOutput = DebugOutput.BranchTriplanarCount;
         }
         else if (HasFeature (keywords, DefineFeature._DEBUG_BRANCHCOUNT_CLUSTER))
         {
            debugOutput = DebugOutput.BranchClusterCount;
         }
         else if (HasFeature (keywords, DefineFeature._DEBUG_BRANCHCOUNT_OTHER))
         {
            debugOutput = DebugOutput.BranchOtherCount;
         }
         else if (HasFeature (keywords, DefineFeature._DEBUG_BRANCHCOUNT_TOTAL))
         {
            debugOutput = DebugOutput.BranchTotal;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_SAMPLECOUNT))
         {
            debugOutput = DebugOutput.SampleCount;
         }
         else if (HasFeature (keywords, DefineFeature._DEBUG_WORLDNORMALVERTEX))
         {
            debugOutput = DebugOutput.WorldNormalVertex;
         }
         else if (HasFeature (keywords, DefineFeature._DEBUG_WORLDNORMAL))
         {
            debugOutput = DebugOutput.WorldNormal;
         }
         else if (HasFeature (keywords, DefineFeature._DEBUG_FINALNORMALTANGENT))
         {
            debugOutput = DebugOutput.FinalNormalTangent;
         }
#if __MICROSPLAT_SNOW__
         else if (HasFeature(keywords, DefineFeature._DEBUG_SNOWSPARKLE))
         {
            debugOutput = DebugOutput.SnowSparkle;
         }
#endif
#if __MICROSPLAT_TRAX__
         else if (HasFeature(keywords, DefineFeature._DEBUG_TRAXBUFFER))
         {
            debugOutput = DebugOutput.TraxBuffer;
         }
#endif
#if __MICROSPLAT_DECAL__
         else if (HasFeature(keywords, DefineFeature._DEBUG_DECAL_STATIC))
         {
            debugOutput = DebugOutput.StaticDecals;
         }
#endif
#if __MICROSPLAT_PROCTEX__
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT0))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput0;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT1))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput1;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT2))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput2;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT3))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput3;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT4))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput4;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT5))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput5;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT6))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput6;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT7))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput7;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT0A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput0A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT1A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput1A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT2A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput2A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT3A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput3A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT4A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput4A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT5A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput5A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT6A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput6A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_OUTPUT_SPLAT7A))
         {
            debugOutput = DebugOutput.ProceduralSplatOutput7A;
         }
         else if (HasFeature(keywords, DefineFeature._DEBUG_PROCLAYERS))
         {
            debugOutput = DebugOutput.ProceduralLayerCount;
         }
#endif
#if __MICROSPLAT_MEGA__
          else if (HasFeature(keywords, DefineFeature._DEBUG_MEGABARY))
         {
            debugOutput = DebugOutput.MegaBaryWeights;
         }
#endif
      }

      public override void InitCompiler(string[] paths)
      {
         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths[i];
            if (p.EndsWith("microsplat_properties_splat.txt"))
            {
               properties_splat = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith ("microsplat_core_cbuffer.txt"))
            {
               cbuffer = AssetDatabase.LoadAssetAtPath<TextAsset> (p);
            }
         }

      }

      public override void WritePerMaterialCBuffer (string[] features, System.Text.StringBuilder sb)
      {
         sb.AppendLine (cbuffer.text);
         if (perTexSSS || HasFeature(features, "_MESHCOMBINEDUSESSS") || (HasFeature(features, "_SNOWSSS") && (HasFeature(features, "_SNOW"))))
         {
            sb.AppendLine ("      half _SSSScale;");
            sb.AppendLine ("      half _SSSPower;");
            sb.AppendLine ("      half _SSSDistance;");
         }

         if (debugOutput == DebugOutput.SampleCount)
         {
            sb.AppendLine ("      float _SampleCountDiv;");
         }
      }

      public override void WriteProperties(string[] features, System.Text.StringBuilder sb)
      {
         sb.AppendLine(properties_splat.text);
         if (emissiveArray)
         {
            sb.AppendLine("      [NoScaleOffset]_EmissiveMetal (\"Emissive Array\", 2DArray) = \"black\" {}");
            sb.AppendLine ("      _EmissiveMult(\"Emissive Multiplier\", Float) = 1");
         }
         if (packMode != TextureArrayConfig.PackingMode.Fastest)
         {
            sb.AppendLine("      [NoScaleOffset]_SmoothAO (\"Smooth AO Array\", 2DArray) = \"black\" {}");
         }
         if (pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
         {
            sb.AppendLine ("      [NoScaleOffset]_Specular (\"Specular Array\", 2DArray) = \"black\" {}");
         }
         if (perTexSSS || HasFeature (features, "_MESHCOMBINEDUSESSS") || (HasFeature (features, "_SNOWSSS") && (HasFeature (features, "_SNOW"))))
         {
            sb.AppendLine ("      _SSSDistance(\"SSS Distance\", Float) = 1");
            sb.AppendLine ("      _SSSScale(\"SSS Scale\", Float) = 4");
            sb.AppendLine ("      _SSSPower(\"SSS Power\", Float) = 4");

         }
         if (blendMode == BlendMode.HybridHeightLinear)
         {
            sb.AppendLine ("      _HybridHeightBlendDistance(\"Hybrid Blend Distance\", Float) = 300");
         }

         if (controlNoiseUV || System.Array.Exists<string>(features, e => e == "_GLOBALNOISEUV"))
         {
            sb.AppendLine ("      [NoScaleOffset]_NoiseUV (\"Noise UV texture\", 2D) = \"grey\" {}");
            sb.AppendLine ("      _NoiseUVParams(\"Noise UV Params\", Vector) = (1, 1, 0, 0)");

         }

         if (normalMode == NormalMode.AutoNormal)
         {
            sb.AppendLine ("      _AutoNormalHeightScale(\"Auto Normal Height Scale\", Range(0, 4)) = 1");
         }

         if (debugOutput == DebugOutput.SampleCount)
         {
            sb.AppendLine ("      _SampleCountDiv(\"SampleCount\", Float) = 90");
         }
      }

      public override void ComputeSampleCounts(string[] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, ref int tessellationSamples, ref int depTexReadLevel)
      {
#if __MICROSPLAT_MESH__
         if (shaderType != Workflow.VertexMesh)
         {
            textureSampleCount += ((int)maxTextureCount) / 4; // control textures
         }
#endif

         if (perfMode == PerformanceMode.BestQuality)
         {
            arraySampleCount += (normalMode == NormalMode.Disabled || normalMode == NormalMode.AutoNormal) ? 4 : 8;
            if (emissiveArray)
            {
               arraySampleCount += 4;
            }
            if (packMode != TextureArrayConfig.PackingMode.Fastest)
            {
               arraySampleCount += 4;
            }
            if (pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
            {
               arraySampleCount += 4;
            }
         }
         else if (perfMode == PerformanceMode.Balanced)
         {
            arraySampleCount += (normalMode == NormalMode.Disabled || normalMode == NormalMode.AutoNormal) ? 3 : 6;
            if (emissiveArray)
            {
               arraySampleCount += 3;
            }
            if (packMode != TextureArrayConfig.PackingMode.Fastest)
            {
               arraySampleCount += 3;
            }
            if (pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
            {
               arraySampleCount += 3;
            }
         }
         else if (perfMode == PerformanceMode.Fastest)
         {
            arraySampleCount += (normalMode == NormalMode.Disabled || normalMode == NormalMode.AutoNormal) ? 2 : 4;
            if (emissiveArray)
            {
               arraySampleCount += 2;
            }
            if (packMode != TextureArrayConfig.PackingMode.Fastest)
            {
               arraySampleCount += 2;
            }
            if (pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
            {
               arraySampleCount += 2;
            }
         }
         if (perPixelNormal)
         {
            textureSampleCount++;
         }
      }

      static GUIContent CPerTexUV = new GUIContent("UV Scale", "UV Scale for the texture. You may need to change your sampler settings if this is enabled.");
      static GUIContent CPerTexUVOffset = new GUIContent("UV Offset", "UV Offset for each texture");
      static GUIContent CPerTexUVRotation = new GUIContent ("UV Rotation", "UV Rotation for each texture");
      static GUIContent CPerTexInterp = new GUIContent("Interpolation Contrast", "Control blend of sharpness vs other textures");
      static GUIContent CPerTexTint = new GUIContent("Tint", "Tint color for albedo");
      static GUIContent CPerTexNormStr = new GUIContent("Normal Strength", "Increase or decrease strength of normal mapping");
      static GUIContent CPerTexAOStr = new GUIContent("AO Strength", "Increase or decrease strength of the AO map");
      static GUIContent CPerTexSmoothStr = new GUIContent("Smoothness Strength", "Increase or decrease strength of the smoothness");
      static GUIContent CPerTexMetallic = new GUIContent("Metallic", "Set the metallic value of the texture");
      static GUIContent CPerTexBrightness = new GUIContent("Brightness", "Brightness of texture");
      static GUIContent CPerTexColorIntensity = new GUIContent ("Color Intensity", "Non saturating brightness of texture");
      static GUIContent CPerTexContrast = new GUIContent("Contrast", "Contrast of texture");
      static GUIContent CPerTexSaturation = new GUIContent("Saturation", "Saturation of the texture");
      static GUIContent CPerTexHeightOffset = new GUIContent("Height Offset", "Allows you to adjust the heightfield of each texture up or down");
      static GUIContent CPerTexHeightContrast = new GUIContent("Height Contrast", "Allows you to adjust the contrast of the height map");
      static GUIContent CPerTexInnerDarken = new GUIContent ("Fuzzy Direct Darken", "Darken or lighten areas facing camera");
      static GUIContent CPerTexEdgeLighten = new GUIContent ("Fuzzy Edge Lighten", "Darken or lighten areas edge on to the camera");
      static GUIContent CPerTexFuzzyPower = new GUIContent ("Fuzzy Power", "Controls fresnel width of fuzzy shading");
      static GUIContent CPerTexSSSTint = new GUIContent ("SSS Tint", "Subsurface Scattering Tint");
      static GUIContent CPerTexSSSThickness = new GUIContent ("SSS Thickness", "Subsurface Scattering Thickness");
      static GUIContent CPerTexFuzzyShadingLabel = new GUIContent ("Fuzzy Shading", "Adjusts color based on lighting response angle");
      static GUIContent CPerTexCurveWeights = new GUIContent ("Curve Weights", "Changes interpolated weights to produce curve like structures, like fields on a golf course");
      static GUIContent CPerTexRimLightIntensity = new GUIContent ("Rim Light Intensity", "How strong the rim light is");
      static GUIContent CPerTexRimLightColor = new GUIContent ("Rim Light Color", "Color for rim lighting");
      static GUIContent CPerTexRimLightPower = new GUIContent ("Rim Light Power", "Controls the rate of falloff for rim lighting");
      static GUIContent CPerTexOutlines = new GUIContent ("Outlines", "Allows you to tint the texture around the texture blending area. Alpha controls the strength of the effect");

      public override void DrawPerTextureGUI(int index, MicroSplatKeywords keywords, Material mat, MicroSplatPropData propData)
      {

         InitPropData (0, propData, new Color (1.0f, 1.0f, 0.0f, 0.0f)); // uvscale2, uvOffset
         InitPropData (1, propData, new Color (1.0f, 1.0f, 1.0f, 0.0f)); // tint, interp contrast
         InitPropData (2, propData, new Color (1.0f, 0.0f, 1.0f, 0.0f)); // norm str, smooth str, ao str, metal values
         InitPropData (3, propData, new Color (0.0f, 1.0f, 0.4f, 1.0f)); // brightness, contrast, porosity, foam
         InitPropData (9, propData, new Color (1, 1, 1, 1));
         InitPropData (10, propData, new Color (1, 1, 1, 1));
         InitPropData (16, propData, new Color (0, 0, 0, 0));
         InitPropData (17, propData, new Color (0, 0, 1, 0));
         InitPropData (18, propData, new Color (1, 1, 1, 1));
         InitPropData (19, propData, new Color (0, 1, 1, 1));
         InitPropData (23, propData, new Color (0, 0, 0, 1));
         InitPropData (28, propData, new Color (0.5f, 0.5f, 0.5f, 1));

         perTexUVScale = DrawPerTexVector2Vector2 (index, 0, GetFeatureName (DefineFeature._PERTEXUVSCALEOFFSET), 
            keywords, propData, CPerTexUV, CPerTexUVOffset);

         if (keywords.IsKeywordEnabled("_TRIPLANAR"))
         {
            perTexUVRotation = DrawPerTexFloatSlider (index, 16, GetFeatureName (DefineFeature._PERTEXUVROTATION),
            keywords, propData, Channel.R, CPerTexUVRotation, -3.15f, 3.15f);
            if (perTexUVRotation)
            {
               DrawPerTexFloatSlider (index, 16, GetFeatureName (DefineFeature._PERTEXUVROTATION),
               keywords, propData, Channel.G, CPerTexUVRotation, -3.15f, 3.15f, false);
               DrawPerTexFloatSlider (index, 16, GetFeatureName (DefineFeature._PERTEXUVROTATION),
               keywords, propData, Channel.B, CPerTexUVRotation, -3.15f, 3.15f, false);
            }
         }
         else
         {
            perTexUVRotation = DrawPerTexFloatSlider (index, 16, GetFeatureName (DefineFeature._PERTEXUVROTATION),
            keywords, propData, Channel.R, CPerTexUVRotation, -3.15f, 3.15f);
         }
         



         if (blendMode == BlendMode.HeightBlended)
         {
            perTexInterpContrast = DrawPerTexFloatSlider (index, 1, GetFeatureName (DefineFeature._PERTEXINTERPCONTRAST),
               keywords, propData, Channel.A, CPerTexInterp, -1.0f, 1.0f);
         }

         perTexCurveWeight = DrawPerTexFloatSlider (index, 19, GetFeatureName (DefineFeature._PERTEXCURVEWEIGHT),
            keywords, propData, Channel.R, CPerTexCurveWeights, 0.001f, 0.5f);

         perTexTint = DrawPerTexColor (index, 1, GetFeatureName (DefineFeature._PERTEXTINT),
            keywords, propData, CPerTexTint, false);

         perTexBrightness = DrawPerTexFloatSlider (index, 3, GetFeatureName (DefineFeature._PERTEXBRIGHTNESS),
            keywords, propData, Channel.R, CPerTexBrightness, -1.0f, 1.0f);

         perTexColorIntensity = DrawPerTexFloatSlider (index, 23, GetFeatureName (DefineFeature._PERTEXCOLORINTENSITY),
            keywords, propData, Channel.R, CPerTexColorIntensity, -1.0f, 1.0f);

         perTexContrast = DrawPerTexFloatSlider (index, 3, GetFeatureName (DefineFeature._PERTEXCONTRAST),
            keywords, propData, Channel.G, CPerTexContrast, 0.1f, 3.0f);

         perTexSaturation = DrawPerTexFloatSlider (index, 9, GetFeatureName (DefineFeature._PERTEXSATURATION),
            keywords, propData, Channel.A, CPerTexSaturation, 0.0f, 2.0f);

         perTexNormStr = DrawPerTexFloatSlider (index, 2, GetFeatureName (DefineFeature._PERTEXNORMSTR),
            keywords, propData, Channel.R, CPerTexNormStr, 0.0f, 3.0f);
      
         perTexSmoothStr = DrawPerTexFloatSlider (index, 2, GetFeatureName (DefineFeature._PERTEXSMOOTHSTR),
            keywords, propData, Channel.G, CPerTexSmoothStr, -1.0f, 1.0f);

         perTexAOStr = DrawPerTexFloatSlider (index, 2, GetFeatureName (DefineFeature._PERTEXAOSTR),
            keywords, propData, Channel.B, CPerTexAOStr, 0.5f, 3.0f);

         perTexMetallic = DrawPerTexFloatSlider (index, 2, GetFeatureName (DefineFeature._PERTEXMETALLIC),
            keywords, propData, Channel.A, CPerTexMetallic, 0, 1);

         perTexHeightOffset = DrawPerTexFloatSlider (index, 10, GetFeatureName (DefineFeature._PERTEXHEIGHTOFFSET),
            keywords, propData, Channel.B, CPerTexHeightOffset, 0, 2);

         perTexHeightContrast = DrawPerTexFloatSlider (index, 10, GetFeatureName (DefineFeature._PERTEXHEIGHTCONTRAST),
            keywords, propData, Channel.A, CPerTexHeightContrast, 0.2f, 4);

         perTexSSS = DrawPerTexFloatSlider(index, 18, GetFeatureName(DefineFeature._PERTEXSSS),
            keywords, propData, Channel.A, CPerTexSSSThickness, 0.0f, 1.0f);
         if (perTexSSS)
         {
            bool old = GUI.enabled;
            GUI.enabled = perTexSSS;
            DrawPerTexColorNoToggle (index, 18, propData, CPerTexSSSTint);
            GUI.enabled = old;
         }

         perTexMicroShadows = DrawPerTexFloatSlider (index, 17, GetFeatureName (DefineFeature._PERTEXMICROSHADOWS),
            keywords, propData, Channel.A, CPerTexMicroShadows, 0.0f, 1.0f);

         perTexFuzzyShading = DrawPerTexFloatSlider (index, 17, GetFeatureName (DefineFeature._PERTEXFUZZYSHADE),
            keywords, propData, Channel.R, perTexFuzzyShading ?  CPerTexInnerDarken : CPerTexFuzzyShadingLabel, 0.0f, 1.0f);
         if (perTexFuzzyShading)
         {
            DrawPerTexFloatSliderNoToggle (index, 17, GetFeatureName (DefineFeature._PERTEXFUZZYSHADE),
               keywords, propData, Channel.G, CPerTexEdgeLighten, 0.0f, 1.0f);
            DrawPerTexFloatSliderNoToggle (index, 17, GetFeatureName (DefineFeature._PERTEXFUZZYSHADE),
               keywords, propData, Channel.B, CPerTexFuzzyPower, 0.2f, 16.0f);
         }
         perTexRimLight = DrawPerTexColor (index, 27, GetFeatureName (DefineFeature._PERTEXRIMLIGHT),
            keywords, propData, CPerTexRimLightColor, false);

         if (perTexRimLight)
         {
            DrawPerTexFloatSliderNoToggle (index, 27, GetFeatureName (DefineFeature._PERTEXRIMLIGHT),
               keywords, propData, Channel.A, CPerTexRimLightIntensity, 0.0f, 2.0f);
            DrawPerTexFloatSliderNoToggle (index, 26, GetFeatureName (DefineFeature._PERTEXRIMLIGHT),
               keywords, propData, Channel.G, CPerTexRimLightPower, 0.2f, 16.0f);

         }

         perTexOutlines = DrawPerTexColor (index, 28, GetFeatureName (DefineFeature._PERTEXOUTLINECOLOR), keywords,
               propData, CPerTexOutlines, true);

         
      }
   }   
}

