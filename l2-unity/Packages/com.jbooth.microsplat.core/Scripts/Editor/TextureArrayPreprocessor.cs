//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;

namespace JBooth.MicroSplat
{
    [InitializeOnLoad]
    public class TextureArrayActiveBuildTargetListener : IActiveBuildTargetChanged
    {
        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            var guids = AssetDatabase.FindAssets("t: TextureArrayConfig");
            foreach (var guid in guids)
            {
                AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(guid));
            }
        }
    }



   class TextureArrayPreProcessor : AssetPostprocessor
   {
      // this is a shitty hash, but good enough for unity versions..
      static int HashString(string str)
      {
         unchecked
         {
            int h = 0;
            int[] hashPrimes = { 3, 5, 7, 11, 13, 17, 23, 27 };
            int pidx = 0;
            foreach (char c in str)
            {
               h += (int)c * hashPrimes[pidx % hashPrimes.Length];
               pidx++;
            }
            return h;
         }
         
      }

      static int GetNewHash(TextureArrayConfig cfg)
      {
         unchecked
         {
            var settings = TextureArrayConfigEditor.GetSettingsGroup(cfg, UnityEditor.EditorUserBuildSettings.activeBuildTarget);
            int h = 17;

            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.diffuseSettings.compression, settings.diffuseSettings.compressionQuality) * 7;
            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.normalSettings.compression, settings.normalSettings.compressionQuality) * 13;
            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.emissiveSettings.compression, settings.emissiveSettings.compressionQuality) * 17;
            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.antiTileSettings.compression, settings.antiTileSettings.compressionQuality) * 23;
            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.smoothSettings.compression, settings.smoothSettings.compressionQuality) * 31;
            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.traxDiffuseSettings.compression, settings.traxDiffuseSettings.compressionQuality) * 37;
            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.traxNormalSettings.compression, settings.traxNormalSettings.compressionQuality) * 41;
            h = h * (int)TextureArrayConfigEditor.GetTextureFormat(cfg, settings.decalSplatSettings.compression, settings.decalSplatSettings.compressionQuality) * 43;
            h = h * HashString(Application.unityVersion) * 51;
            //h = h * EditorUserBuildSettings.activeBuildTarget.GetHashCode () * 47;
            return h;
         }
      }

      public static bool sIsPostProcessing = false;

      static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
      {
         var updates = new HashSet<TextureArrayConfig>();
         AddChangedConfigsToHashSet(updates, importedAssets);
         AddChangedConfigsToHashSet(updates, movedAssets);
         AddChangedConfigsToHashSet(updates, movedFromAssetPaths);

         // this block allows users to not include the texture arrays in
         // source control, and MS will regenerate them if they are missing.
         bool needsSync = false;
         var guids = AssetDatabase.FindAssets("t: TextureArrayConfig");
         foreach (var guid in guids)
         {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var diffuseArrayPath = path.Replace(".asset", "_diff_tarray.asset");
            if (!System.IO.File.Exists(diffuseArrayPath))
            {
               var cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(path);

               sIsPostProcessing = true;
               TextureArrayConfigEditor.CompileConfig(cfg);
               needsSync = true;
               sIsPostProcessing = false;
            }
         }
         if (needsSync)
         {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            MicroSplatTerrain.SyncAll();
         }

         foreach (var updatedConfig in updates)
         {
            CheckConfigForUpdates(updatedConfig);
         }
      }

      private static void AddChangedConfigsToHashSet(HashSet<TextureArrayConfig> hashSet, string[] paths)
      {
         for (int i = 0; i < paths.Length; i++)
         {
            var type = AssetDatabase.GetMainAssetTypeAtPath(paths[i]);
            if (type != typeof(TextureArrayConfig))
            {
                continue;
            }

            var cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(paths[i]);
            if (cfg != null)
            {
               hashSet.Add(cfg);
            }
            else
            {
               Debug.LogWarning($"Unexpectedly failed to load ${nameof(TextureArrayConfig)} at path ${paths[i]}");
            }
         }
      }

      private static void CheckConfigForUpdates(TextureArrayConfig cfg)
      {
         int hash = GetNewHash(cfg);
         if (hash != cfg.hash)
         {
            cfg.hash = hash;
            EditorUtility.SetDirty(cfg);
            try 
            { 
               sIsPostProcessing = true;
               TextureArrayConfigEditor.CompileConfig(cfg);
            }
            finally
            {
               sIsPostProcessing = false;
               AssetDatabase.Refresh();
               AssetDatabase.SaveAssets();
               MicroSplatTerrain.SyncAll();
            }
         }
      }
   }
}
