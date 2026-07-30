//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JBooth.MicroSplat;
using UnityEngine.SceneManagement;
using System;

namespace JBooth.MicroSplat
{
   [ExecuteInEditMode]
   [DisallowMultipleComponent]
   public class MicroSplatTerrain : MicroSplatObject
   {
      public delegate void MaterialSyncAll();
      public delegate void MaterialSync(Material m);

      public static event MaterialSyncAll OnMaterialSyncAll;
      public event MaterialSync OnMaterialSync;

      static List<MicroSplatTerrain> sInstances = new List<MicroSplatTerrain>();

      public Terrain terrain;

      // unity asset bundles/addressables do not handle base map shaders correctly
      // and will not load it because there is no hard reference to it. So we do it here.
      public Shader baseMapShader;
      
      public Texture2D customControl0;
      public Texture2D customControl1;
      public Texture2D customControl2;
      public Texture2D customControl3;
      public Texture2D customControl4;
      public Texture2D customControl5;
      public Texture2D customControl6;
      public Texture2D customControl7;

      // used to expand tessellation bounds.
      public Vector3 patchBoundsMultiplier = Vector3.one;

      // LOTS of hacking around Unity bugs. In some versions of Unity, accessing any data on the terrain too early can cause a crash, either
      // in editor, or in playmode, but not in the same way. Really just want to call Sync on Enabled, and Cleanup on Disabled, but triggers
      // too many issues, so we dodge the bullet in various cases to hack around it- so ugly.

      void Awake()
      {
         terrain = GetComponent<Terrain>();
#if UNITY_EDITOR
         Sync();
#endif
      }

      void OnEnable()
      {
         terrain = GetComponent<Terrain>();
         sInstances.Add(this);
#if UNITY_EDITOR
         Sync();
         UnityEditor.SceneManagement.EditorSceneManager.sceneSaved -= OnSceneSave;
         UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += OnSceneSave;
#else
      if (reenabled)
      {
         Sync();
      }
#endif
      }

#if UNITY_EDITOR
      [UnityEditor.Callbacks.DidReloadScripts]
      private static void OnScriptsReloaded()
      {
         MicroSplatObject.SyncAll();
      }
      private void OnSceneSave(Scene scene)
      {
         Sync();
      }
#endif

#if !UNITY_EDITOR
   void Start()
   {
      Sync();
   }
#endif

      [HideInInspector]
      public bool reenabled = false;
      void OnDisable()
      {
         sInstances.Remove(this);
         Cleanup();
         reenabled = true;
#if UNITY_EDITOR
         UnityEditor.SceneManagement.EditorSceneManager.sceneSaved -= OnSceneSave;
#endif
      }

#if UNITY_EDITOR
      bool inTerrainChanged = false;
      void OnTerrainChanged(int f)
      {
         if (inTerrainChanged)
            return;
         TerrainChangedFlags flags = (TerrainChangedFlags)f;
         bool changed = ((flags & TerrainChangedFlags.Heightmap) != 0);
         if ((flags & TerrainChangedFlags.DelayedHeightmapUpdate) != 0)
         {
            changed = true;
         }
         
         if (changed)
         {
            inTerrainChanged = true;
            Sync();
            inTerrainChanged = false;
         }
      }
#endif

      void Cleanup()
      {
         if (matInstance != null && matInstance != templateMaterial)
         {
            DestroyImmediate(matInstance);
            terrain.materialTemplate = null;
#if !UNITY_2019_2_OR_NEWER
         terrain.materialType = Terrain.MaterialType.BuiltInStandard;
#endif
         }
#if UNITY_EDITOR
         terrain.basemapDistance = 512;
#endif
      }

      public override TerrainDescriptor GetTerrainDescriptor()
      {
         TerrainDescriptor td = new TerrainDescriptor();
         td.heightMap = terrain.terrainData.heightmapTexture;
         td.normalMap = terrain.normalmapTexture;
         if (perPixelNormal != null)
         {
            td.normalMap = perPixelNormal;
         }
         td.heightMapScale = terrain.terrainData.heightmapScale;
         return td;
      }

#if UNITY_EDITOR
        static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);

            if (place == -1)
                return source;

            return source.Remove(place, find.Length).Insert(place, replace);
        }
#endif

        public void Sync()
      {
         if (templateMaterial == null)
            return;

#if UNITY_EDITOR
         RevisionFromMat();

         // Unity doesn't handle base map shaders when using asset bundles
         // or addressables. Basically it doesn't load them because there is
         // no hard reference to them, and their own shaders work because they
         // are included assets. So when loading from a bundle, the base map
         // uses Shader.FindShader for the base map and fails to find it, because
         // it's not loaded.
         Shader shader = templateMaterial.shader;
         if (shader != null)
         {
            var path = UnityEditor.AssetDatabase.GetAssetPath(shader);
            path = ReplaceLastOccurrence(path, ".shader", "_Base.shader");
            Shader baseShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (baseMapShader != baseShader && baseShader != null)
            {
               baseMapShader = baseShader;
               UnityEditor.EditorUtility.SetDirty(this);
            }
         }
#endif

         ApplySharedData(templateMaterial);

         Material m = null;

         if (terrain.materialTemplate == matInstance && matInstance != null)
         {
            terrain.materialTemplate.CopyPropertiesFromMaterial(templateMaterial);
            m = terrain.materialTemplate;
         }
         else
         {
            if (matInstance != null)
            {
                DestroyImmediate(matInstance);
            }
            m = new Material(templateMaterial);
         }
#if !UNITY_2019_2_OR_NEWER
      terrain.materialType = Terrain.MaterialType.Custom;
#endif


         m.hideFlags = HideFlags.HideAndDontSave;
         terrain.materialTemplate = m;
         matInstance = m;
         
         ApplyMaps(m);

         if (terrain.drawInstanced)
         {
            m.SetTexture("_PerPixelNormal", terrain.normalmapTexture);
         }



         if (keywordSO != null && keywordSO.IsKeywordEnabled("_CUSTOMSPLATTEXTURES"))
         {
            m.SetTexture("_CustomControl0", customControl0 != null ? customControl0 : Texture2D.blackTexture);
            m.SetTexture("_CustomControl1", customControl1 != null ? customControl1 : Texture2D.blackTexture);
            m.SetTexture("_CustomControl2", customControl2 != null ? customControl2 : Texture2D.blackTexture);
            m.SetTexture("_CustomControl3", customControl3 != null ? customControl3 : Texture2D.blackTexture);
            m.SetTexture("_CustomControl4", customControl4 != null ? customControl4 : Texture2D.blackTexture);
            m.SetTexture("_CustomControl5", customControl5 != null ? customControl5 : Texture2D.blackTexture);
            m.SetTexture("_CustomControl6", customControl6 != null ? customControl6 : Texture2D.blackTexture);
            m.SetTexture("_CustomControl7", customControl7 != null ? customControl7 : Texture2D.blackTexture);
         }
         else
         {
            if (terrain == null || terrain.terrainData == null)
            {
               Debug.LogError("Terrain or terrain data is null, cannot sync");
               return;
            }
            var controls = terrain.terrainData.alphamapTextures;
            ApplyControlTextures(controls, m);
         }


         // set base map distance to max of fancy features
         // base map does not use the "base map" texture, so slam to 16
         // only do this stuff editor time to avoid runtime cost
#if UNITY_EDITOR

         float basemapDistance = 0;

         if (m.HasProperty("_TessData2"))
         {
            float d = m.GetVector("_TessData2").y;
            if (d > basemapDistance)
               basemapDistance = d;

            // this makes it less likely for terrain to be clipped from displacement.
            // However, since it's not a patchBoundsAddition, but a multiplier,
            // there is no correct value here- what we really want to do is increase
            // the patch size by the max displacement.
            if (terrain.patchBoundsMultiplier != patchBoundsMultiplier)
               terrain.patchBoundsMultiplier = patchBoundsMultiplier; 
         }
         if (m.HasProperty("_ParallaxParams"))
         {
            Vector4 v = m.GetVector("_ParallaxParams");
            float d = v.y + v.z;
            if (d > basemapDistance)
               basemapDistance = d;
         }
         if (m.HasProperty("_POMParams"))
         {
            Vector4 v = m.GetVector("_POMParams");
            float d = v.y + v.z;
            if (d > basemapDistance)
               basemapDistance = d;
         }
         if (m.HasProperty("_DetailNoiseScaleStrengthFade"))
         {
            float d = m.GetVector("_DetailNoiseScaleStrengthFade").z;
            if (d > basemapDistance)
               basemapDistance = d;
         }

         terrain.basemapDistance = basemapDistance > 0 ? basemapDistance : 99999;
#endif

         ApplyBlendMap();

         if (OnMaterialSync != null)
         {
            OnMaterialSync(m);
         }

#if UNITY_EDITOR
         RestorePrototypes();
#endif

      }

      public override Bounds GetBounds()
      {
         return terrain.terrainData.bounds;
      }

#if UNITY_EDITOR

      bool inRestorePrototypes = false;
      void RestorePrototypes()
      {
         if (Application.isPlaying || inRestorePrototypes)
             return;

         inRestorePrototypes = true;
         if (templateMaterial != null)
         {
            Texture2DArray diffuseArray = templateMaterial.GetTexture("_Diffuse") as Texture2DArray;
            if (diffuseArray != null)
            {
               var cfg = JBooth.MicroSplat.TextureArrayConfig.FindConfig(diffuseArray);

               if (cfg != null && propData != null && keywordSO != null)
               {
                  int count = cfg.sourceTextures.Count;
                  if (count > 32)
                     count = 32;

#if __MICROSPLAT_SLOPETEXTURE__
                  if (count > cfg.maxSyncCount)
                     count = cfg.maxSyncCount;
#endif

                  var protos = terrain.terrainData.terrainLayers;
                  
                  bool needsRefresh = false;
                  if (protos.Length != count)
                  {
                     needsRefresh = true;
                  }
                  if (!needsRefresh)
                  {
                     for (int i = 0; i < protos.Length; ++i)
                     {
                        if (protos[i] == null)
                        {
                           needsRefresh = true;
                           break;
                        }
                        if (protos[i] != null && cfg.sourceTextures[i] != null && protos[i].diffuseTexture != cfg.sourceTextures[i].diffuse)
                        {
                           needsRefresh = true;
                           break;
                        }
                     }
                  }

                  if (needsRefresh)
                  {
                     Vector4 v4 = templateMaterial.GetVector("_UVScale");

                     Vector2 uvScales = new Vector2(v4.x, v4.y);
                     uvScales = MicroSplatRuntimeUtil.UVScaleToUnityUVScale(uvScales, terrain);

                     protos = new TerrainLayer[count];
                     for (int i = 0; i < count; ++i)
                     {
                        string path = UnityEditor.AssetDatabase.GetAssetPath(cfg);
                        path = path.Replace("\\", "/");
                        path = path.Substring(0, path.LastIndexOf("/"));

                        if (cfg.sourceTextures[i].terrainLayer == null || cfg.sourceTextures[i].terrainLayer.diffuseTexture != cfg.sourceTextures[i].diffuse)
                        {
                           path += "/microsplat_layer_";
                           path = path.Replace("//", "/");

                           if (cfg.sourceTextures[i].diffuse != null)
                           {
                              path += cfg.sourceTextures[i].diffuse.name;
                           }
                           path += "_" + i;
                           path += ".terrainlayer";

                           TerrainLayer sp = new TerrainLayer();

                           sp.diffuseTexture = cfg.sourceTextures[i].diffuse;
                           sp.tileSize = uvScales;
                           if (keywordSO.IsKeywordEnabled("_PERTEXUVSCALEOFFSET"))
                           {
                              Color c = propData.GetValue(i, 0);
                              Vector2 ptScale = new Vector2(c.r, c.b);
                              sp.tileSize = MicroSplatRuntimeUtil.UVScaleToUnityUVScale(uvScales * ptScale, terrain);
                           }

                           cfg.sourceTextures[i].terrainLayer = sp;
                           protos[i] = sp;
                           UnityEditor.EditorApplication.delayCall += () =>
                           {
                              UnityEditor.AssetDatabase.CreateAsset(sp, path);
                           };

                        }
                        else
                        {
                           protos[i] = cfg.sourceTextures[i].terrainLayer;
                        }
                        
                     }

                     terrain.terrainData.terrainLayers = protos;
                     UnityEditor.EditorUtility.SetDirty(terrain);
                     UnityEditor.EditorUtility.SetDirty(terrain.terrainData);

                  }

               }
            }
         }
         inRestorePrototypes = false;
      }
#endif

      public static new void SyncAll()
      {
         for (int i = 0; i < sInstances.Count; ++i)
         {
            sInstances[i].Sync();
         }
         if (OnMaterialSyncAll != null)
         {
            OnMaterialSyncAll();
         }
      }

#if UNITY_EDITOR
      public List<Texture2D> importSplatMaps = new List<Texture2D>();
#endif
   }
}
