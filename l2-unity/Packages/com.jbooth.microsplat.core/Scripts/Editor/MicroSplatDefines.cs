//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;

namespace JBooth.MicroSplat
{
   [InitializeOnLoad]
   public class MicroSplatDefines
   {
      const string sMicroSplatDefine = "__MICROSPLAT__";
      static MicroSplatDefines ()
      {
         InitDefine (sMicroSplatDefine);
      }

      public static bool HasDefine (string def)
      {
         var target = EditorUserBuildSettings.selectedBuildTargetGroup;
         string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup (target);
         return defines.Contains (def);
      }

      public static void InitDefine (string def)
      {
         var target = EditorUserBuildSettings.selectedBuildTargetGroup;
         string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup (target);
         if (!defines.Contains (def))
         {
            if (string.IsNullOrEmpty (defines))
            {
               PlayerSettings.SetScriptingDefineSymbolsForGroup (target, def);
            }
            else
            {
               if (!defines [defines.Length - 1].Equals (';'))
               {
                  defines += ';';
               }
               defines += def;
               PlayerSettings.SetScriptingDefineSymbolsForGroup (target, defines);
            }
         }
      }

      [PostProcessSceneAttribute (0)]
      public static void OnPostprocessScene ()
      {
         InitDefine (sMicroSplatDefine);
      }

      public static string link_globalTexture = "https://assetstore.unity.com/packages/tools/terrain/microsplat-global-texturing-96482?aid=25047";
      public static string link_snow = "https://assetstore.unity.com/packages/tools/terrain/microsplat-dynamic-snow-96486?aid=25047";
      public static string link_tessellation = "https://assetstore.unity.com/packages/tools/terrain/microsplat-tessellation-and-parallax-96484?aid=25047";
      public static string link_antitile = "https://assetstore.unity.com/packages/tools/terrain/microsplat-anti-tiling-module-96480?aid=25047";
      public static string link_terrainblend = "https://assetstore.unity.com/packages/tools/terrain/microsplat-terrain-blending-97364?aid=25047";
      public static string link_streams = "https://assetstore.unity.com/packages/tools/terrain/microsplat-puddles-streams-lava-wetness-97993?aid=25047";
      public static string link_alphahole = "https://assetstore.unity.com/packages/tools/terrain/microsplat-terrain-holes-97495?aid=25047";
      public static string link_triplanar = "https://assetstore.unity.com/packages/tools/terrain/microsplat-triplanar-uvs-96777?aid=25047";
      public static string link_textureclusters = "https://assetstore.unity.com/packages/tools/terrain/microsplat-texture-clusters-104223?aid=25047";
      public static string link_windglitter = "https://assetstore.unity.com/packages/tools/terrain/microsplat-wind-and-glitter-105627?aid=25047";
      public static string link_proctex = "https://assetstore.unity.com/packages/tools/terrain/microsplat-runtime-procedural-texturing-143039?aid=25047";
      public static string link_lowpoly = "https://assetstore.unity.com/packages/tools/terrain/microsplat-low-poly-look-146119?aid=1011l37NJ&aid=25047";
      public static string link_meshterrain = "https://assetstore.unity.com/packages/tools/terrain/microsplat-mesh-terrains-157356?aid=1011l37NJ&aid=25047";
      public static string link_meshworkflow = "https://assetstore.unity.com/packages/tools/painting/microsplat-mesh-workflow-beta-120008?aid=1011l37NJ&aid=25047";
      public static string link_digger = "https://assetstore.unity.com/packages/tools/terrain/microsplat-digger-integration-162840?pubref=25047";
      public static string link_trax = "https://assetstore.unity.com/packages/tools/terrain/microsplat-trax-166218?pubref=25047";
      public static string link_polaris = "https://assetstore.unity.com/packages/tools/terrain/microsplat-polaris-integration-166851?pubref=25047";
      public static string link_scatter = "https://assetstore.unity.com/packages/tools/terrain/microsplat-scatter-170299?pubref=25047";
      public static string link_decal = "https://assetstore.unity.com/packages/tools/terrain/microsplat-decals-178765?aid=25047";

   }
}