//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// This class can be used as part of your build process to automatically compress splat maps
// and other data before a build. See the DrawGUI function for an example of how you can do this.

namespace JBooth.MicroSplat
{
   public class MicroSplatCompressor
   {

      static GUIContent CCompSplatMaps = new GUIContent ("Compress Splat Maps", "This will compress terrain and mesh splat maps. Note that terrain editing will not work until splat maps are reverted from this mode");
      static GUIContent CCompStreamMap = new GUIContent ("Compress Stream Map", "This will compress the map holding wetness, puddles, streams and lava painting");
      static GUIContent CCompSnowMaps = new GUIContent ("Compress Snow Maps", "This will compress snow masking maps");
      static GUIContent CCompBiomeMaps = new GUIContent ("Compress Biome Maps", "This will compress procedural biome masks");
      static GUIContent CCompTintMaps = new GUIContent ("Compress Tint Maps", "This will compress any tint maps");

      public static void DrawGUI (MicroSplatObject mso, Options o)
      {
         if (MicroSplatUtilities.DrawRollup ("Compressor", false))
         {
            EditorGUILayout.HelpBox ("You can use this section to automatically covert various maps to compressed formats before a build. Please note, that once compressed, external files will be used for things like the terrain splat maps, and changing the terrain through Unity's tools will have no effect", MessageType.Info);
            EditorGUILayout.LabelField ("Compression Options");

            o.splatMaps = EditorGUILayout.Toggle (CCompSplatMaps, o.splatMaps);
            if (o.splatMaps)
            {
               EditorGUILayout.HelpBox ("This will remove the terrain painting from your terrain file to save memory! The Uncompress button will restore this from the output textures, but if you delete or unhook them your painting may be lost.", MessageType.Error);
            }
            o.streamMaps = EditorGUILayout.Toggle (CCompStreamMap, o.streamMaps);
            o.snowMask = EditorGUILayout.Toggle (CCompSnowMaps, o.snowMask);
            o.biomeMask = EditorGUILayout.Toggle (CCompBiomeMaps, o.biomeMask);
            o.tintMap = EditorGUILayout.Toggle (CCompTintMaps, o.tintMap);

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.Space ();
            if (GUILayout.Button ("Compress"))
            {
               var gos = Selection.gameObjects;
               foreach (var go in gos)
               {
                  var m = go.GetComponent<MicroSplatTerrain>();
                  if (m != null)
                  {
                     MicroSplatCompressor comp = new MicroSplatCompressor();
                     comp.Compress(m, o);
                  }
               }
            }
            if (GUILayout.Button ("Uncompress"))
            {
               var gos = Selection.gameObjects;
               foreach (var go in gos)
               {
                  var m = go.GetComponent<MicroSplatTerrain>();
                  if (m != null)
                  {
                     MicroSplatCompressor comp = new MicroSplatCompressor();
                     comp.Revert(m);
                  }
               }
               
            }
            EditorGUILayout.Space ();
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.Space ();
            if (GUILayout.Button ("Compress Scene"))
            {
               MicroSplatCompressor comp = new MicroSplatCompressor ();
               MicroSplatObject [] objs = GameObject.FindObjectsOfType<MicroSplatObject> ();
               foreach (var obj in objs)
               {
                  comp.Compress (obj, o);
               }
            }
            if (GUILayout.Button ("Uncompress Scene"))
            {
               MicroSplatCompressor comp = new MicroSplatCompressor ();
               MicroSplatObject [] objs = GameObject.FindObjectsOfType<MicroSplatObject> ();
               foreach (var obj in objs)
               {
                  comp.Revert (obj);
               }
            }
            EditorGUILayout.Space ();
            EditorGUILayout.EndHorizontal ();
         }
      }



      [System.Serializable]
      public class Options
      {
         public bool splatMaps = false;
         public bool streamMaps = true;
         public bool snowMask = true;
         public bool biomeMask = true;
         public bool tintMap = true;
      }

      Texture2D CompressTexture(string path, bool sRGB)
      {
         AssetImporter ai = AssetImporter.GetAtPath (path);
         if (ai == null)
            return null;
         TextureImporter ti = (TextureImporter)ai;
         if (ti != null)
         {
            ti.compressionQuality = 100;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.textureCompression = TextureImporterCompression.Compressed;
            ti.sRGBTexture = sRGB;
            ti.isReadable = false;
            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            settings.format = TextureImporterFormat.Automatic;
            settings.compressionQuality = 100;
            settings.textureCompression = TextureImporterCompression.Compressed;
            ti.SetPlatformTextureSettings(settings);
            ti.SaveAndReimport();
         }
         return AssetDatabase.LoadAssetAtPath<Texture2D> (path);
      }

      Texture2D UncompressTexture (string path)
      {
         AssetImporter ai = AssetImporter.GetAtPath (path);
         if (ai == null)
            return null;
         TextureImporter ti = (TextureImporter)ai;
         if (ti != null)
         {
            ti.compressionQuality = 100;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.isReadable = true;
            ti.sRGBTexture = false;
            TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
            settings.format = TextureImporterFormat.RGBA32;
            settings.textureCompression = TextureImporterCompression.Uncompressed;
            ti.SetPlatformTextureSettings(settings);
            ti.SaveAndReimport();
         }
         return AssetDatabase.LoadAssetAtPath<Texture2D> (path);
      }

      void CompressTerrainSplats (MicroSplatTerrain t)
      {
         int splatCount = t.terrain.terrainData.alphamapTextureCount;
         // write out
         for (int i = 0; i < splatCount; ++i)
         {
            var tex = t.terrain.terrainData.GetAlphamapTexture (i);
            var path = MicroSplatUtilities.RelativePathFromAsset (t);
            path += "/" + t.name + "_splat" + i + ".tga";
            System.IO.File.WriteAllBytes (path, tex.EncodeToTGA ());
         }

         AssetDatabase.Refresh ();
         // load and adjust importer
         for (int i = 0; i < splatCount; ++i)
         {
            var path = MicroSplatUtilities.RelativePathFromAsset (t);
            path += "/" + t.name + "_splat" + i + ".tga";

            var tex = CompressTexture (path, false);

            if (i == 0)
            {
               t.customControl0 = tex;
            }
            else if (i == 1)
            {
               t.customControl1 = tex;
            }
            else if (i == 2)
            {
               t.customControl2 = tex;
            }
            else if (i == 3)
            {
               t.customControl3 = tex;
            }
            else if (i == 4)
            {
               t.customControl4 = tex;
            }
            else if (i == 5)
            {
               t.customControl5 = tex;
            }
            else if (i == 6)
            {
               t.customControl6 = tex;
            }
            else if (i == 7)
            {
               t.customControl7 = tex;
            }
         }
         EditorUtility.SetDirty (t);

         MicroSplatKeywords keywords = MicroSplatUtilities.FindOrCreateKeywords (t.templateMaterial);
         if (!keywords.IsKeywordEnabled("_CUSTOMSPLATTEXTURES"))
         {
            keywords.EnableKeyword ("_CUSTOMSPLATTEXTURES");
            MicroSplatShaderGUI.MicroSplatCompiler compiler = new MicroSplatShaderGUI.MicroSplatCompiler ();
            compiler.Compile (t.templateMaterial);
            MicroSplatTerrain.SyncAll ();
         }

         // destructive operation
         t.terrain.terrainData.alphamapResolution = 16;
      }

      void ExtractSplats(ref float[,,] maps, Texture2D tex, int index, int layers)
      {
         if (tex == null)
            return;
         int size = tex.width;

         if (tex != null)
         {
            for (int x = 0; x < size; ++x)
            {
               for (int y = 0; y < size; ++y)
               {
                  Color c = tex.GetPixel (x, y);
                  if (index < layers)
                  {
                     maps [y, x, index + 0] = c.r;
                  }
                  if (index + 1 < layers)
                  {
                     maps [y, x, index + 1] = c.g;
                  }
                  if (index + 2 < layers)
                  {
                     maps [y, x, index + 2] = c.b;
                  }
                  if (index + 3 < layers)
                  {
                     maps [y, x, index + 3] = c.a;
                  }
               }
            }
         }
      }

      void RevertTerrainSplats(MicroSplatTerrain t)
      {
         MicroSplatKeywords keywords = MicroSplatUtilities.FindOrCreateKeywords (t.templateMaterial);
         if (keywords.IsKeywordEnabled ("_CUSTOMSPLATTEXTURES"))
         {
            if (t.customControl0 == null)
            {
               Debug.LogError ("Could not revert terrain because textures are missing!");
               return;
            }

            UncompressTexture (t.customControl0);
            UncompressTexture (t.customControl1);
            UncompressTexture (t.customControl2);
            UncompressTexture (t.customControl3);
            UncompressTexture (t.customControl4);
            UncompressTexture (t.customControl5);
            UncompressTexture (t.customControl6);
            UncompressTexture (t.customControl7);

            int size = t.customControl0.width;
            int layers = t.terrain.terrainData.alphamapLayers;
            t.terrain.terrainData.alphamapResolution = size;
            var maps = t.terrain.terrainData.GetAlphamaps (0, 0, size, size);

            ExtractSplats (ref maps, t.customControl0, 0, layers);
            ExtractSplats (ref maps, t.customControl1, 4, layers);
            ExtractSplats (ref maps, t.customControl2, 8, layers);
            ExtractSplats (ref maps, t.customControl3, 12, layers);
            ExtractSplats (ref maps, t.customControl4, 16, layers);
            ExtractSplats (ref maps, t.customControl5, 20, layers);
            ExtractSplats (ref maps, t.customControl6, 24, layers);
            ExtractSplats (ref maps, t.customControl7, 28, layers);

            t.terrain.terrainData.SetAlphamaps (0, 0, maps);
            EditorUtility.SetDirty (t.terrain.terrainData);


            keywords.DisableKeyword ("_CUSTOMSPLATTEXTURES");
            MicroSplatShaderGUI.MicroSplatCompiler compiler = new MicroSplatShaderGUI.MicroSplatCompiler ();
            compiler.Compile (t.templateMaterial);
            t.customControl0 = null;
            t.customControl1 = null;
            t.customControl2 = null;
            t.customControl3 = null;
            t.customControl4 = null;
            t.customControl5 = null;
            t.customControl6 = null;
            t.customControl7 = null;
            EditorUtility.SetDirty (t);
            MicroSplatTerrain.SyncAll ();
         }
      }

      void CompressTexture (Texture2D tex, bool sRGB)
      {
         if (tex != null)
         {
            CompressTexture (AssetDatabase.GetAssetPath (tex), sRGB);
         }
      }

      void UncompressTexture (Texture2D tex)
      {
         if (tex != null)
         {
            UncompressTexture (AssetDatabase.GetAssetPath (tex));
         }
      }

      void CompressTexture(Material mat, string name, bool sRGB)
      {
         if (mat.HasProperty(name))
         {
            CompressTexture (mat.GetTexture (name) as Texture2D, sRGB);
         }
      }

      void UncompressTexture (Material mat, string name)
      {
         if (mat.HasProperty (name))
         {
            UncompressTexture (mat.GetTexture (name) as Texture2D);
         }
      }

      public void Compress (MicroSplatObject mso, Options opt)
      {
         MicroSplatTerrain t = mso as MicroSplatTerrain;
         if (t != null)
         {
            if (t.templateMaterial == null)
            {
               Debug.LogError ("MicroSplatTerrain " + mso.gameObject.name + " does not have template material");
            }
            else
            {
               if (opt.splatMaps)
               {
                  CompressTerrainSplats (t);
               }
            }
         }

#if __MICROSPLAT_MESH__
         MicroSplatMesh msm = mso as MicroSplatMesh;
         if (msm != null)
         {
            foreach (var subMesh in msm.subMeshEntries)
            {
               foreach (var tex in subMesh.subMeshOverride.controlTextures)
               {
                  CompressTexture (tex, false);
               }
            }
         }
#endif

#if __MICROSPLAT_PROCTEX__
         if (opt.biomeMask)
         {
            CompressTexture (mso.procBiomeMask, false);
            CompressTexture (mso.procBiomeMask2, false);
            CompressTexture (mso.templateMaterial, "_ProcTexBiomeMask", false);
            CompressTexture (mso.templateMaterial, "_ProcTexBiomeMask2", false);
         }
#endif
#if __MICROSPLAT_SNOW__
         if (opt.snowMask)
         {
            CompressTexture (mso.snowMaskOverride, false);
            CompressTexture (mso.templateMaterial, "_SnowMask", false);
         }
#endif
#if __MICROSPLAT_STREAMS__
         if (opt.streamMaps)
         {
            CompressTexture (mso.streamTexture, false);
            CompressTexture (mso.templateMaterial, "_StreamControl", false);
         }
#endif
#if __MICROSPLAT_GLOBALTEXTURE__
         if (opt.tintMap)
         {
            CompressTexture (mso.streamTexture, true);
            CompressTexture (mso.templateMaterial, "_GlobalTintTex", true);
         }
#endif

      }

      public void Revert(MicroSplatObject mso)
      {
         MicroSplatTerrain t = mso as MicroSplatTerrain;
         if (t != null)
         {
            if (t.templateMaterial == null)
            {
               Debug.LogError ("MicroSplatTerrain " + mso.gameObject.name + " does not have template material");
            }
            else
            {
               RevertTerrainSplats (t);
            }
         }

#if __MICROSPLAT_MESH__
         MicroSplatMesh msm = mso as MicroSplatMesh;
         if (msm != null)
         {
            foreach (var subMesh in msm.subMeshEntries)
            {
               foreach (var tex in subMesh.subMeshOverride.controlTextures)
               {
                  UncompressTexture (tex);
               }
            }
         }
#endif

#if __MICROSPLAT_PROCTEX__
         UncompressTexture (mso.procBiomeMask);
         UncompressTexture (mso.procBiomeMask2);
         UncompressTexture (mso.templateMaterial, "_ProcTexBiomeMask");
         UncompressTexture (mso.templateMaterial, "_ProcTexBiomeMask2");
#endif
#if __MICROSPLAT_SNOW__
         UncompressTexture (mso.snowMaskOverride);
         UncompressTexture (mso.templateMaterial, "_SnowMask");
#endif
#if __MICROSPLAT_STREAMS__
         UncompressTexture (mso.streamTexture);
         UncompressTexture (mso.templateMaterial, "_StreamControl");
#endif
#if __MICROSPLAT_GLOBALTEXTURE__
         UncompressTexture (mso.streamTexture);
         UncompressTexture (mso.templateMaterial, "_GlobalTintTex");
#endif
      }
   }
}
