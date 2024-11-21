//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using JBooth.MicroSplat;


public partial class MicroSplatShaderGUI : ShaderGUI
{
   public static readonly string MicroSplatVersion = "3.9";

   MicroSplatCompiler compiler = new MicroSplatCompiler ();

   public MaterialProperty FindProp (string name, MaterialProperty [] props)
   {
      return FindProperty (name, props);
   }

   GUIContent CShaderName = new GUIContent ("Name", "Menu path with name for the shader");
   GUIContent CRenderLoop = new GUIContent ("Render Loop", "You can select which render loop the shader should be compiled for here");

   public static bool needsCompile = false;
   bool bulkEditMode = false;
   int perTexIndex = 0;
   System.Text.StringBuilder builder = new System.Text.StringBuilder (1024);
   GUIContent [] renderLoopNames;



   bool DrawRenderLoopGUI (MicroSplatKeywords keywords, Material targetMat)
   {
      // init render loop name list
      if (renderLoopNames == null || renderLoopNames.Length != availableRenderLoops.Count)
      {
         var rln = new List<GUIContent> ();
         for (int i = 0; i < availableRenderLoops.Count; ++i)
         {
            rln.Add (new GUIContent (availableRenderLoops [i].GetDisplayName ()));
         }
         renderLoopNames = rln.ToArray ();
      }

      int curRenderLoopIndex = 0;
      for (int i = 0; i < keywords.keywords.Count; ++i)
      {
         string s = keywords.keywords [i];
         for (int j = 0; j < availableRenderLoops.Count; ++j)
         {
            if (s == availableRenderLoops [j].GetRenderLoopKeyword ())
            {
               curRenderLoopIndex = j;
               compiler.renderLoop = availableRenderLoops [j];
               break;
            }
         }
      }

      int oldIdx = curRenderLoopIndex;
      curRenderLoopIndex = EditorGUILayout.Popup (CRenderLoop, curRenderLoopIndex, renderLoopNames);
      if (oldIdx != curRenderLoopIndex && curRenderLoopIndex >= 0 && curRenderLoopIndex < availableRenderLoops.Count)
      {
         if (compiler.renderLoop != null)
         {
            keywords.DisableKeyword (compiler.renderLoop.GetRenderLoopKeyword ());
         }
         compiler.renderLoop = availableRenderLoops [curRenderLoopIndex];
         keywords.EnableKeyword (compiler.renderLoop.GetRenderLoopKeyword ());
         return true;
      }

      if (targetMat != null && !targetMat.enableInstancing)
      {
         EditorUtility.SetDirty (targetMat);
         targetMat.enableInstancing = true;
      }
      return false;
   }

   int cachedKeywordCount;
   Material cachedMaterial;
   void Undo_UndoRedoPerformed ()
   {
      if (cachedMaterial != null && cachedKeywordCount > 0)
      {
         var keywordSO = MicroSplatUtilities.FindOrCreateKeywords (cachedMaterial);
         if (cachedKeywordCount != keywordSO.keywords.Count)
         {
            needsCompile = true;
         }
      }
   }

   string cachedTitle;
   GUIStyle moduleLabelStyle;
   Dictionary<string, bool> moduleFoldoutState = new Dictionary<string, bool> ();
   public static Material targetMat { get; private set; }

   static int tabIdx = 0;
   static GUIContent[] tabs = new GUIContent[] { new GUIContent("Features"), new GUIContent("Settings"), new GUIContent("Per-Texture"), new GUIContent("Arrays") };
   public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
   {
      Undo.undoRedoPerformed -= Undo_UndoRedoPerformed;
      Undo.undoRedoPerformed += Undo_UndoRedoPerformed;
      targetMat = materialEditor.target as Material;

      if (cachedTitle == null)
      {
         cachedTitle = "Shader Generator        v:" + MicroSplatVersion;
      }
      if (moduleLabelStyle == null)
      {
         moduleLabelStyle = new GUIStyle (EditorStyles.foldout);
         moduleLabelStyle.fontStyle = FontStyle.Bold;
      }
      if (GUI.enabled == false || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(targetMat)))
      {
         EditorGUILayout.HelpBox("You must edit the template material, not the instance being used. You can find this in the MicroSplatData directory, or assigned on your MicroSplatTerrain component", MessageType.Info);
         return;
      }
      EditorGUI.BeginChangeCheck(); // sync materials

      var keywordSO = MicroSplatUtilities.FindOrCreateKeywords(targetMat);

      cachedKeywordCount = keywordSO.keywords.Count; // for undo
      cachedMaterial = targetMat;

      compiler.Init();
      // must unpack everything before the generator draws- otherwise we get IMGUI errors
      for (int i = 0; i < compiler.extensions.Count; ++i)
      {
         var ext = compiler.extensions[i];
         ext.Unpack(keywordSO.keywords.ToArray());
      }
      
      string shaderName = targetMat.shader.name;
      DrawModules();


      tabIdx = GUILayout.Toolbar(tabIdx, tabs);
      EditorGUI.BeginChangeCheck(); // needs compile
      var propTex = FindOrCreatePropTex(targetMat);

      Undo.RecordObjects(new Object[3] { targetMat, keywordSO, propTex }, "MicroSplat Material Edit");

      if (tabIdx == 0)
      {
         using (new GUILayout.VerticalScope(MicroSplatUtilities.boxStyle))
         {
            if (MicroSplatUtilities.DrawRollup(cachedTitle))
            {
               GUILayout.BeginHorizontal();
               GUILayout.FlexibleSpace();
               if (bulkEditMode)
               {

                  if (GUILayout.Button("Exit Bulk Shader Feature Edit Mode", GUILayout.Width(230)))
                  {
                     bulkEditMode = false;
                     needsCompile = true;
                  }
               }
               else
               {
                  if (GUILayout.Button("Enter Bulk Shader Feature Edit Mode", GUILayout.Width(230)))
                  {
                     bulkEditMode = true;
                  }
               }
               GUILayout.FlexibleSpace();
               GUILayout.EndHorizontal();
               if (bulkEditMode)
               {
                  EditorGUILayout.HelpBox("Shader is in bulk edit mode, allowing you to change many options without recompiling the shader. No material properties will be shown during bulk editing, and the shader will be recompiled and properties shown once you exit this mode", MessageType.Warning);
               }

               shaderName = EditorGUILayout.DelayedTextField(CShaderName, shaderName);

               if (DrawRenderLoopGUI(keywordSO, targetMat))
               {
                  needsCompile = true;
               }

               for (int i = 0; i < compiler.extensions.Count; ++i)
               {
                  var e = compiler.extensions[i];
                  if (e.GetVersion() == MicroSplatVersion)
                  {
                     needsCompile = EditorGUI.EndChangeCheck() || needsCompile;

                     if (!moduleFoldoutState.ContainsKey(e.ModuleName()))
                     {
                        moduleFoldoutState[e.ModuleName()] = true;
                     }

                     if (!e.HideModule())
                     {
                        using (new GUILayout.VerticalScope(MicroSplatUtilities.boxStyle))
                        {
                           EditorGUI.indentLevel++;
                           EditorGUILayout.BeginHorizontal();
                           moduleFoldoutState[e.ModuleName()] = EditorGUILayout.Foldout(moduleFoldoutState[e.ModuleName()], e.ModuleName(), moduleLabelStyle);

                           if (e.GetHelpPath() != null)
                           {
                              if (GUILayout.Button("Documentation", GUILayout.Width(100), GUILayout.Height(18)))
                              {
                                 string p = e.GetHelpPath();

                                 string path = p.Replace(" ", "%20");
                                 Application.OpenURL(path);
                              }
                           }

                           EditorGUILayout.EndHorizontal();
                           EditorGUI.BeginChangeCheck();
                           if (moduleFoldoutState[e.ModuleName()])
                           {
                              //EditorGUI.indentLevel++;
                              e.DrawFeatureGUI(keywordSO);
                              //EditorGUI.indentLevel--;
                           }
                           EditorGUI.indentLevel--;
                        }
                     }
                     else
                     {
                        EditorGUI.BeginChangeCheck();
                     }

                  }
                  else
                  {
                     EditorGUILayout.HelpBox("Extension : " + e.ModuleName() + " is version " + e.GetVersion() + " and MicroSplat is version " + MicroSplatVersion + ", please update", MessageType.Error);
                  }
               }

               for (int i = 0; i < availableRenderLoops.Count; ++i)
               {
                  var rl = availableRenderLoops[i];
                  if (rl.GetVersion() != MicroSplatVersion)
                  {
                     EditorGUILayout.HelpBox("Render Loop : " + rl.GetDisplayName() + " is version " + rl.GetVersion() + " and MicroSplat is version " + MicroSplatVersion + ", please update", MessageType.Error);
                  }
               }
            }
            if (bulkEditMode)
            {
               if (!keywordSO.IsKeywordEnabled("_DISABLESPLATMAPS"))
               {
                  Texture2DArray diff = targetMat.GetTexture("_Diffuse") as Texture2DArray;
                  if (diff != null && MicroSplatUtilities.DrawRollup("Per Texture Properties"))
                  {

                     perTexIndex = MicroSplatUtilities.DrawTextureSelector(perTexIndex, diff);

                     for (int i = 0; i < compiler.extensions.Count; ++i)
                     {
                        var ext = compiler.extensions[i];
                        if (ext.GetVersion() == MicroSplatVersion)
                        {
                           ext.DrawPerTextureGUI(perTexIndex, keywordSO, targetMat, propTex);
                        }
                     }
                  }
               }

               needsCompile = needsCompile || EditorGUI.EndChangeCheck();
               if (needsCompile)
               {
                  keywordSO.keywords.Clear();
                  for (int i = 0; i < compiler.extensions.Count; ++i)
                  {
                     compiler.extensions[i].Pack(keywordSO);
                  }
                  if (compiler.renderLoop != null)
                  {
                     keywordSO.EnableKeyword(compiler.renderLoop.GetRenderLoopKeyword());
                  }
                  needsCompile = false;
               }
               return; // Don't draw rest of GUI
            }
         }
         needsCompile = needsCompile || EditorGUI.EndChangeCheck();
      }
      int featureCount = keywordSO.keywords.Count;
      if (tabIdx == 1)
      {
         // note, ideally we wouldn't draw the GUI for the rest of stuff if we need to compile.
         // But we can't really do that without causing IMGUI to split warnings about
         // mismatched GUILayout blocks
         if (!needsCompile)
         {

            for (int i = 0; i < compiler.extensions.Count; ++i)
            {
               var ext = compiler.extensions[i];
               if (ext.GetVersion() == MicroSplatVersion)
               {
                  ext.DrawShaderGUI(this, keywordSO, targetMat, materialEditor, props);
               }
               else
               {
                  EditorGUILayout.HelpBox("Extension : " + ext.ModuleName() + " is version " + ext.GetVersion() + " and MicroSplat is version " + MicroSplatVersion + ", please update so that all modules are using the same version.", MessageType.Error);
               }

            }
            int arraySampleCount = 0;
            int textureSampleCount = 0;
            int maxSamples = 0;
            int tessSamples = 0;
            int depTexReadLevel = 0;
            builder.Length = 0;
            for (int i = 0; i < compiler.extensions.Count; ++i)
            {
               var ext = compiler.extensions[i];
               if (ext.GetVersion() == MicroSplatVersion)
               {
                  ext.ComputeSampleCounts(keywordSO.keywords.ToArray(), ref arraySampleCount, ref textureSampleCount, ref maxSamples, ref tessSamples, ref depTexReadLevel);
               }
            }
            if (MicroSplatUtilities.DrawRollup("Debug"))
            {
               string shaderModel = compiler.GetShaderModel(keywordSO.keywords.ToArray());
               builder.Append("Shader Model : ");
               builder.AppendLine(shaderModel);
               if (maxSamples != arraySampleCount)
               {
                  builder.Append("Texture Array Samples : ");
                  builder.AppendLine(arraySampleCount.ToString());

                  builder.Append("Regular Samples : ");
                  builder.AppendLine(textureSampleCount.ToString());
               }
               else
               {
                  builder.Append("Texture Array Samples : ");
                  builder.AppendLine(arraySampleCount.ToString());
                  builder.Append("Regular Samples : ");
                  builder.AppendLine(textureSampleCount.ToString());
               }
               if (tessSamples > 0)
               {
                  builder.Append("Tessellation Samples : ");
                  builder.AppendLine(tessSamples.ToString());
               }
               if (depTexReadLevel > 0)
               {
                  builder.Append(depTexReadLevel.ToString());
                  builder.AppendLine(" areas with dependent texture reads");
               }

               EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);
            }
         }
         if (!needsCompile)
         {
            if (featureCount != keywordSO.keywords.Count)
            {
               needsCompile = true;
            }
         }
      }

      if (tabIdx == 2 && !needsCompile)
      {
         if (!keywordSO.IsKeywordEnabled("_DISABLESPLATMAPS"))
         {
            Texture2DArray diff = targetMat.GetTexture("_Diffuse") as Texture2DArray;
            if (diff != null && MicroSplatUtilities.DrawRollup("Per Texture Properties"))
            {

               perTexIndex = MicroSplatUtilities.DrawTextureSelector(perTexIndex, diff);

               for (int i = 0; i < compiler.extensions.Count; ++i)
               {
                  var ext = compiler.extensions[i];
                  if (ext.GetVersion() == MicroSplatVersion)
                  {
                     ext.DrawPerTextureGUI(perTexIndex, keywordSO, targetMat, propTex);
                  }
               }
               if (!needsCompile)
               {
                  if (featureCount != keywordSO.keywords.Count)
                  {
                     needsCompile = true;
                  }
               }
            }

         }
      }
      


      if (EditorGUI.EndChangeCheck () && !needsCompile)
      {
         MicroSplatObject.SyncAll ();
      }

      if (needsCompile)
      {
         needsCompile = false;
         keywordSO.keywords.Clear ();
         for (int i = 0; i < compiler.extensions.Count; ++i)
         {
            compiler.extensions [i].Pack (keywordSO);
         }
         if (compiler.renderLoop != null)
         {
            keywordSO.EnableKeyword (compiler.renderLoop.GetRenderLoopKeyword ());
         }

         // horrible workaround to GUI warning issues
         compileMat = targetMat;
         compileName = shaderName;
         targetCompiler = compiler;
         EditorApplication.delayCall += TriggerCompile;
      }

      if (tabIdx == 3)
      {
         if (!keywordSO.IsKeywordEnabled("_DISABLESPLATMAPS"))
         {
            Texture2DArray diff = targetMat.GetTexture("_Diffuse") as Texture2DArray;
            var path = AssetDatabase.GetAssetPath(diff);
            path = path.Replace("_diff_tarray.asset", ".asset");
            
            TextureArrayConfig cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(path);

            if (cfg != null)
            {
               if (configUI == null)
               {
                  configUI = new TextureArrayConfigUI();
               }
               SerializedObject so = new SerializedObject(cfg);
               configUI.OnInspectorGUI(cfg, so);
            }
         }
      }

   }
   static TextureArrayConfigUI configUI;
   static Material compileMat;
   static string compileName;
   static MicroSplatCompiler targetCompiler;
   protected void TriggerCompile()
   {
      targetCompiler.Compile(compileMat, compileName);
   }


   class Module
   {
      public Module(string url, string img)
      {
         assetStore = url;
         texture = Resources.Load<Texture2D>(img);
      }
      public string assetStore;
      public Texture2D texture;
   }

   void InitModules()
   {
      if (modules.Count == 0)
      {
         //
#if !__MICROSPLAT_GLOBALTEXTURE__
         modules.Add(new Module(MicroSplatDefines.link_globalTexture, "microsplat_module_globaltexture"));
#endif
#if !__MICROSPLAT_SNOW__
         modules.Add(new Module(MicroSplatDefines.link_snow, "microsplat_module_snow"));
#endif
#if !__MICROSPLAT_TESSELLATION__
         modules.Add(new Module(MicroSplatDefines.link_tessellation, "microsplat_module_tessellation"));
#endif
#if !__MICROSPLAT_DETAILRESAMPLE__
         modules.Add(new Module(MicroSplatDefines.link_antitile, "microsplat_module_detailresample"));
#endif
#if !__MICROSPLAT_TERRAINBLEND__
         modules.Add(new Module(MicroSplatDefines.link_terrainblend, "microsplat_module_terrainblending"));
#endif
#if !__MICROSPLAT_STREAMS__
         modules.Add(new Module(MicroSplatDefines.link_streams, "microsplat_module_streams"));
#endif
#if !__MICROSPLAT_ALPHAHOLE__
         modules.Add(new Module(MicroSplatDefines.link_alphahole, "microsplat_module_alphahole"));
#endif
#if !__MICROSPLAT_TRIPLANAR__
         modules.Add(new Module(MicroSplatDefines.link_triplanar, "microsplat_module_triplanaruvs"));
#endif
#if !__MICROSPLAT_TEXTURECLUSTERS__
         modules.Add(new Module(MicroSplatDefines.link_textureclusters, "microsplat_module_textureclusters"));
#endif
#if !__MICROSPLAT_WINDGLITTER__
         modules.Add(new Module(MicroSplatDefines.link_windglitter, "microsplat_module_windglitter"));
#endif
#if !__MICROSPLAT_PROCTEX__
         modules.Add(new Module(MicroSplatDefines.link_proctex, "microsplat_module_proctexture"));
#endif
#if !__MICROSPLAT_LOWPOLY__
         modules.Add(new Module(MicroSplatDefines.link_lowpoly, "microsplat_module_lowpoly"));
#endif
#if !__MICROSPLAT_MESHTERRAIN__
         modules.Add(new Module(MicroSplatDefines.link_meshterrain, "microsplat_module_terrainmesh"));
#endif
#if !__MICROSPLAT_MESH__
         modules.Add(new Module(MicroSplatDefines.link_meshworkflow, "microsplat_module_meshworkflow"));
#endif
#if !__MICROSPLAT_DIGGER__
         modules.Add(new Module(MicroSplatDefines.link_digger, "microsplat_module_digger"));
#endif
#if !__MICROSPLAT_TRAX__
         modules.Add(new Module(MicroSplatDefines.link_trax, "microsplat_module_trax"));
#endif
#if !__MICROSPLAT_POLARIS__
         modules.Add(new Module(MicroSplatDefines.link_polaris, "microsplat_module_polaris"));
#endif

#if !__MICROSPLAT_SCATTER__
         modules.Add (new Module (MicroSplatDefines.link_scatter, "microsplat_module_scatter"));
#endif
         
#if !__MICROSPLAT_DECAL__
         modules.Add (new Module (MicroSplatDefines.link_decal, "microsplat_module_decals"));
#endif            

         int n = modules.Count;
         if (n > 1)
         {
            System.Random rnd = new System.Random((int)(UnityEngine.Random.value * 1000)); 
            while (n > 1)
            {  
               n--;  
               int k = rnd.Next(n + 1);  
               var value = modules[k];  
               modules[k] = modules[n];  
               modules[n] = value;  
            } 
         }
      }
       

   }

   List<Module> modules = new List<Module>();

   void DrawModule(Module m)
   {
      if (GUILayout.Button(m.texture, GUI.skin.box, GUILayout.Width(92), GUILayout.Height(92)))
      {
         Application.OpenURL(m.assetStore);
      }
   }
   Vector2 moduleScroll;
   void DrawModules()
   {
      InitModules();
      if (modules.Count == 0)
      {
         return;
      }

      EditorGUILayout.LabelField("Want more features? Add them here..");

      moduleScroll = EditorGUILayout.BeginScrollView(moduleScroll, GUILayout.Height(100));
      GUILayout.BeginHorizontal();
      for (int i = 0; i < modules.Count; ++i)
      {
         DrawModule(modules[i]);
      }
      GUILayout.EndHorizontal();
      EditorGUILayout.EndScrollView();

   }
}

