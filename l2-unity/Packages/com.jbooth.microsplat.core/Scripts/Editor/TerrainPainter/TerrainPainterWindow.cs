//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace JBooth.MicroSplat
{
#if __MICROSPLAT__ && (__MICROSPLAT_STREAMS__ || __MICROSPLAT_GLOBALTEXTURE__ || __MICROSPLAT_SNOW__ || __MICROSPLAT_SCATTER__ || __MICROSPLAT_PROCTEX__ || __MICROSPLAT_MEGA__)
   public partial class TerrainPainterWindow : EditorWindow 
   {
      [MenuItem("Window/MicroSplat/Terrain FX Painter")]
      public static void ShowWindow()
      {
         var window = GetWindow<JBooth.MicroSplat.TerrainPainterWindow>();
         window.InitTerrains();
         window.Show();
      }

      bool enabled = true;


      TerrainPaintJob[] terrains;
      bool[] jobEdits;

      TerrainPaintJob FindJob (Terrain t)
      {
         if (terrains == null || t == null)
            return null;

         for (int i = 0; i < terrains.Length; ++i)
         {
            if (terrains[i] != null && terrains[i].terrain == t)
               return terrains[i];
         }
         return null;
      }

      List<Terrain> rawTerrains = new List<Terrain>();

      void InitTerrains()
      {
         Object[] objs = Selection.GetFiltered(typeof(Terrain), SelectionMode.Editable | SelectionMode.Deep);
         List<TerrainPaintJob> ts = new List<TerrainPaintJob> ();
         rawTerrains.Clear();
         for (int i = 0; i < objs.Length; ++i)
         {
            Terrain t = objs[i] as Terrain;
            MicroSplatTerrain mst = t.GetComponent<MicroSplatTerrain>();
            if (mst == null)
               continue;
            rawTerrains.Add(t);
            if (t.materialTemplate != null)
            {
               bool hasStream = t.materialTemplate.HasProperty ("_StreamControl");
               bool hasSnow = t.materialTemplate.HasProperty ("_SnowMask");
               bool hasTint = t.materialTemplate.HasProperty ("_GlobalTintTex");
               bool hasScatter = t.materialTemplate.HasProperty ("_ScatterControl");
               bool hasBiome = t.materialTemplate.HasProperty ("_ProcTexBiomeMask");
               bool hasBiome2 = t.materialTemplate.HasProperty ("_ProcTexBiomeMask2");
               bool hasMega = t.materialTemplate.HasProperty("_MegaSplatTexture");

               if (!hasSnow && !hasStream && !hasTint && !hasScatter && !hasBiome && !hasBiome2 && !hasMega)
                  continue;
#if __MICROSPLAT_STREAMS__
               if (hasStream && mst.streamTexture == null)
               {
                  mst.streamTexture = CreateTexture(t, mst.streamTexture, "_stream_data", new Color(0,0,0,0), true);
               }
#endif
#if __MICROSPLAT_SNOW__
               if (hasSnow && mst.snowMaskOverride == null)
               {
                  mst.snowMaskOverride = CreateTexture (t, mst.snowMaskOverride, "_snowmask", new Color(1,0,0,1), true);
               }
#endif
#if __MICROSPLAT_SCATTER__
               if (hasScatter && mst.scatterMapOverride == null)
               {
                  mst.scatterMapOverride = CreateTexture (t, mst.scatterMapOverride, "_scatter", new Color (0, 0, 0, 1), true);
               }
#endif
#if __MICROSPLAT_MEGA__
               if (hasMega && mst.megaSplatMap == null)
               {
                  mst.megaSplatMap = CreateTexture(t, mst.megaSplatMap, "_megasplat", new Color(0, 0, 0, 0), true, false);
               }
#endif



               var tj = FindJob(t);
               if (tj != null)
               {
                  tj.collider = t.GetComponent<Collider>();
#if __MICROSPLAT_STREAMS__
                  tj.streamTex = mst.streamTexture;
#endif
#if __MICROSPLAT_GLOBALTEXTURE__
                  tj.tintTex = mst.tintMapOverride;
#endif
#if __MICROSPLAT_SNOW__
                  tj.snowTex = mst.snowMaskOverride;
#endif
#if __MICROSPLAT_SCATTER__
                  tj.scatterTex = mst.scatterMapOverride;
#endif
#if __MICROSPLAT_MEGA__
                  tj.megaSplat = mst.megaSplatMap;
#endif

#if __MICROSPLAT_PROCTEX__
                  tj.biomeMask = mst.procBiomeMask;
                  tj.biomeMask2 = mst.procBiomeMask2;
#endif

                  ts.Add(tj);
               }
               else
               {
                  tj = TerrainPaintJob.CreateInstance<TerrainPaintJob> ();
                  tj.terrain = t;
                  tj.collider = t.GetComponent<Collider>();
#if __MICROSPLAT_STREAMS__
                  tj.streamTex = mst.streamTexture;
#endif
#if __MICROSPLAT_GLOBALTEXTURE__
                  tj.tintTex = mst.tintMapOverride;
#endif
#if __MICROSPLAT_SNOW__
                  tj.snowTex = mst.snowMaskOverride;
#endif
#if __MICROSPLAT_SCATTER__
                  tj.scatterTex = mst.scatterMapOverride;
#endif
#if __MICROSPLAT_MEGA__
                  tj.megaSplat = mst.megaSplatMap;
#endif

#if __MICROSPLAT_PROCTEX__
                  tj.biomeMask = mst.procBiomeMask;
                  tj.biomeMask2 = mst.procBiomeMask2;
#endif
                  ts.Add (tj);
               }
            }
         }
         if (terrains != null)
         {
            // clear out old terrains
            for (int i = 0; i < terrains.Length; ++i)
            {
               if (!ts.Contains(terrains[i]))
               {
                  DestroyImmediate(terrains[i]);
               }
            }
         }

         terrains = ts.ToArray();
         jobEdits = new bool[ts.Count];
      }

      void OnSelectionChange()
      {
         InitTerrains();
         this.Repaint();
      }

      void OnFocus() 
      {
#if UNITY_2019_1_OR_NEWER
         SceneView.duringSceneGui -= this.OnSceneGUI;
         SceneView.duringSceneGui += this.OnSceneGUI;
#else
         SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
         SceneView.onSceneGUIDelegate += this.OnSceneGUI;
#endif
         
         Undo.undoRedoPerformed -= this.OnUndo;
         Undo.undoRedoPerformed += this.OnUndo;

         UnityEditor.SceneManagement.EditorSceneManager.sceneSaving -= OnSceneSaving;
         UnityEditor.SceneManagement.EditorSceneManager.sceneSaving += OnSceneSaving;


         this.titleContent = new GUIContent("MicroSplat Terrain FX Painter");
         InitTerrains();
         Repaint();
      }

      public static void SaveTexture (Texture2D tex)
      {
         if (tex != null)
         {
            string path = AssetDatabase.GetAssetPath (tex);
            if (path.EndsWith(".tga"))
            {
               
               AssetImporter ai = AssetImporter.GetAtPath(path);
               TextureImporter ti = ai as TextureImporter;
               if (ti.textureCompression == TextureImporterCompression.Uncompressed &&
                  ti.isReadable)
               {
                  var bytes = tex.EncodeToTGA();
                  System.IO.File.WriteAllBytes(path, bytes);
               }
            }
            else
            {
               Debug.LogError(path + " is not a TGA file");
            }
         }
      }


      void SaveAll ()
      {
         if (terrains == null)
            return;
         for (int i = 0; i < terrains.Length; ++i)
         {
            SaveTexture (terrains [i].streamTex);
            SaveTexture (terrains [i].tintTex);
            SaveTexture (terrains [i].snowTex);
            SaveTexture (terrains [i].scatterTex);
            SaveTexture (terrains [i].biomeMask);
            SaveTexture (terrains [i].biomeMask2);
            SaveTexture (terrains [i].megaSplat);
         }
         AssetDatabase.Refresh ();
      }

      void OnSceneSaving (UnityEngine.SceneManagement.Scene scene, string path)
      {
         SaveAll ();
      }

      void OnUndo()
      {
         if (terrains == null)
            return;
         for (int i = 0; i < terrains.Length; ++i)
         {
            if (terrains[i] != null)
            {
               terrains[i].RestoreUndo();
            }
         }
         Repaint();
      }

      void OnInspectorUpdate()
      {
         // unfortunate...
         Repaint ();
      }

      void OnDestroy() 
      {
#if UNITY_2019_1_OR_NEWER
         SceneView.duringSceneGui -= this.OnSceneGUI;
#else
         SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
#endif
         terrains = null;
      }


   }
#endif
}

