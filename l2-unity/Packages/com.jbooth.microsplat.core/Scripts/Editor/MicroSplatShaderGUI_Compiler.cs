//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using JBooth.MicroSplat;
using System.Collections.Generic;
using System.Linq;

public partial class MicroSplatShaderGUI : ShaderGUI
{
   static List<IRenderLoopAdapter> availableRenderLoops = new List<IRenderLoopAdapter> ();


   [MenuItem ("Window/MicroSplat/Utilities/Regenerate all Texture Arrays")]
   static void RegenAllArrays ()
   {
      var configs = AssetDatabase.FindAssets ("t:TextureArrayConfig");
      foreach (var c in configs)
      {
         var path = AssetDatabase.GUIDToAssetPath (c);
         TextureArrayConfig cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig> (path);
         if (cfg != null)
         {
            Debug.Log ("Recompressing " + path);
            TextureArrayConfigEditor.CompileConfig (cfg);
         }
         Resources.UnloadAsset (cfg);
         cfg = null;
      }
   }

   [MenuItem ("Window/MicroSplat/Utilities/Regenerate all Shaders")]
   static void RegenAllShaders ()
   {
      var mats = AssetDatabase.FindAssets ("t:Material");
      foreach (var m in mats)
      {
         Material mat = AssetDatabase.LoadAssetAtPath<Material> (AssetDatabase.GUIDToAssetPath (m));
         if (mat != null && mat.shader != null)
         {
            if (MicroSplatUtilities.CanFindKeywords (mat))
            {
               Debug.Log ("Regenerating shader " + AssetDatabase.GetAssetPath (mat.shader));
               MicroSplatShaderGUI.MicroSplatCompiler compiler = new MicroSplatShaderGUI.MicroSplatCompiler ();
               compiler.Compile (mat);
            }
         }
      }
   }

   [MenuItem ("Assets/Create/Shader/MicroSplat Shader")]
   static void NewShader2 ()
   {
      NewShader ();
   }

   [MenuItem ("Assets/Create/MicroSplat/MicroSplat Shader")]
   public static Shader NewShader ()
   {
      string path = "Assets";
      foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
      {
         path = AssetDatabase.GetAssetPath (obj);
         if (System.IO.File.Exists (path))
         {
            path = System.IO.Path.GetDirectoryName (path);
         }
         break;
      }
      path = path.Replace ("\\", "/");
      
      path = AssetDatabase.GenerateUniqueAssetPath (path + "/MicroSplat.shader");
      string name = path.Substring (path.LastIndexOf ("/"));
      name = name.Substring (0, name.IndexOf ("."));
      name = name.Replace("/", "");

      string [] keywords = new string [0];
      MicroSplatCompiler compiler = new MicroSplatCompiler ();
      compiler.Init ();
      string ret = compiler.Compile (keywords, name, null);
      System.IO.File.WriteAllText (path, ret);
      AssetDatabase.Refresh ();
      return AssetDatabase.LoadAssetAtPath<Shader> (path);
   }

   public static string ReplaceLastOccurrence(string source, string find, string replace)
   {
      int place = source.LastIndexOf(find);

      if (place == -1)
         return source;

      return source.Remove(place, find.Length).Insert(place, replace);
   }

   public static Material NewShaderAndMaterial (string path, string name, string[] keywords = null)
   {
      // if no branch sampling is specified, go straight to aggressive. Usually defaults are not done this way, but this seems
      // to make the most sense..
      if (!keywords.Contains("_BRANCHSAMPLES"))
      {
         System.Array.Resize (ref keywords, keywords.Length + 2);
         keywords [keywords.Length - 2] = "_BRANCHSAMPLES";
         keywords [keywords.Length - 1] = "_BRANCHSAMPLESARG";
      }
      string shaderPath = AssetDatabase.GenerateUniqueAssetPath (path + "/MicroSplat.shader");
      string shaderBasePath = ReplaceLastOccurrence(shaderPath, ".shader", "_Base.shader");
      string matPath = AssetDatabase.GenerateUniqueAssetPath (path + "/MicroSplat.mat");

      shaderPath = shaderPath.Replace ("//", "/");
      shaderBasePath = shaderBasePath.Replace ("//", "/");
      matPath = matPath.Replace ("//", "/");

      MicroSplatCompiler compiler = new MicroSplatCompiler ();
      compiler.Init ();

      if (keywords == null)
      {
         keywords = new string[0];
      }

      string baseName = "Hidden/MicroSplat/" + name + "_Base";

      string baseShader = compiler.Compile (keywords, baseName);
      string regularShader = compiler.Compile (keywords, name, baseName);

      System.IO.File.WriteAllText (shaderPath, regularShader);
      System.IO.File.WriteAllText (shaderBasePath, baseShader);

      compiler.GenerateAuxShaders (name, shaderPath, new List<string>(keywords));
     
      AssetDatabase.Refresh ();
      Shader s = AssetDatabase.LoadAssetAtPath<Shader> (shaderPath);
      if (s == null)
      {
         Debug.LogError ("Shader not found at path " + shaderPath);
      }
      Material m = new Material (s);
      AssetDatabase.CreateAsset (m, matPath);
      AssetDatabase.SaveAssets ();
      AssetDatabase.Refresh ();
      var kwds = MicroSplatUtilities.FindOrCreateKeywords (m);
      kwds.keywords = new List<string> (keywords);
      EditorUtility.SetDirty (kwds);
      var propData = MicroSplatShaderGUI.FindOrCreatePropTex (m);
      if (propData != null)
      {
         EditorUtility.SetDirty (propData);
      }
      AssetDatabase.SaveAssets ();

      return AssetDatabase.LoadAssetAtPath<Material> (matPath);
   }

   public static Material NewShaderAndMaterial (Terrain t, string [] keywords = null)
   {
      string path = MicroSplatUtilities.RelativePathFromAsset (t.terrainData);
      return NewShaderAndMaterial (path, t.name, keywords);
   }

   public class MicroSplatCompiler
   {

      public class AuxShader
      {
         public AuxShader (string trig, string ext) { trigger = trig; extension = ext; }
         public string trigger;
         public string extension;
         public string customEditor;
         public string options;
      }

      public void GenerateAuxShaders(string name, string path, List<string> keywords)
      {
         var exts = new List<FeatureDescriptor> (extensions); // prevent recursive access due to init
         foreach (var e in exts)
         {
            var aux = e.GetAuxShader ();
            if (aux != null)
            {
               if (keywords.Contains (aux.trigger))
               {
                  var okeys = new List<string> (keywords);

                  // remove all other trigger keywords
                  foreach (var oe in exts)
                  {
                     if (oe != e)
                     {
                        var oaux = oe.GetAuxShader ();
                        if (oaux != null)
                        {
                           okeys.Remove (oaux.trigger);
                        }
                     }
                  }

                  string opath = path.Replace (".shader", aux.extension + ".shader");
                  if (keywords.Contains ("_MSRENDERLOOP_BETTERSHADERS"))
                  {
                     opath = opath.Replace (".shader", ".surfshader");
                  }
                  e.ModifyKeywordsForAuxShader (okeys);
                  var shader = this.Compile (okeys.ToArray (), name + aux.extension, null, aux);
                  MicroSplatUtilities.Checkout (opath);
                  System.IO.File.WriteAllText (opath, shader);
               }
            }
         }
      }

      public List<FeatureDescriptor> extensions = new List<FeatureDescriptor> ();

      public string GetShaderModel (string[] features)
      {
         string minModel = "3.5";
         for (int i = 0; i < extensions.Count; ++i)
         {
            if (extensions [i].RequiresShaderModel46 ())
            {
               minModel = "4.6";
            }
         }
         if (features.Contains ("_FORCEMODEL46"))
         {
            minModel = "4.6";
         }
         if (features.Contains ("_FORCEMODEL50"))
         {
            minModel = "5.0";
         }

         return minModel;
      }

      public void Init ()
      {
         if (extensions.Count == 0)
         {
            string[] paths = AssetDatabase.FindAssets("microsplat_ t:TextAsset");
            for (int i = 0; i < paths.Length; ++i)
            {
               paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            }


            // init extensions
            List<System.Type> extensionTypes = new List<System.Type>();
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
               var asTypes = (from System.Type type in MicroSplatUtilities.GetLoadableTypes(a)
                              where type.IsSubclassOf(typeof(FeatureDescriptor))
                              select type).ToArray();
               extensionTypes.AddRange(asTypes);

            }
            for (int i = 0; i < extensionTypes.Count; ++i)
            {
               var typ = extensionTypes[i];
               FeatureDescriptor ext = System.Activator.CreateInstance(typ) as FeatureDescriptor;
               ext.InitCompiler(paths);
               extensions.Add(ext);
            }
            extensions.Sort(delegate (FeatureDescriptor p1, FeatureDescriptor p2)
            {
               if (p1.DisplaySortOrder() != 0 || p2.DisplaySortOrder() != 0)
               {
                  return p1.DisplaySortOrder().CompareTo(p2.DisplaySortOrder());
               }
               return p1.GetType().Name.CompareTo(p2.GetType().Name);
            });
         }
         if (availableRenderLoops == null || availableRenderLoops.Count == 0)
         {
            string[] paths = AssetDatabase.FindAssets("microsplat_ t:TextAsset");
            for (int i = 0; i < paths.Length; ++i)
            {
               paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
            }

            List<System.Type> adapterTypes = new List<System.Type>();
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
               var adaptTypes = (from System.Type type in MicroSplatUtilities.GetLoadableTypes(a)
                                 where (type.GetInterfaces().Contains(typeof(IRenderLoopAdapter)))
                               select type).ToArray();

               adapterTypes.AddRange(adaptTypes);
            }

            availableRenderLoops.Clear ();
            for (int i = 0; i < adapterTypes.Count; ++i)
            {
               var typ = adapterTypes [i];
               IRenderLoopAdapter adapter = System.Activator.CreateInstance (typ) as IRenderLoopAdapter;
               adapter.Init (paths);
               availableRenderLoops.Add (adapter);
            }

         }
      }

      public static void AddPipelineKeywords(ref string[] keywords)
      {
          var pipeline = MicroSplatUtilities.DetectPipeline();

          if (pipeline == MicroSplatUtilities.PipelineType.HDPipeline)
         {
            System.Array.Resize(ref keywords, keywords.Length + 4);
            keywords[keywords.Length - 4] = "_MSRENDERLOOP_UNITYHD";
            keywords[keywords.Length - 3] = "_MSRENDERLOOP_UNITYHDRP2020";
            keywords[keywords.Length - 2] = "_MSRENDERLOOP_UNITYHDRP2021";
            keywords[keywords.Length - 1] = "_MSRENDERLOOP_UNITYHDRP2022";
         }
         else if (pipeline == MicroSplatUtilities.PipelineType.UniversalPipeline)
         {
            System.Array.Resize(ref keywords, keywords.Length + 4);
            keywords[keywords.Length - 4] = "_MSRENDERLOOP_UNITYLD";
            keywords[keywords.Length - 3] = "_MSRENDERLOOP_UNITYURP2020";
            keywords[keywords.Length - 2] = "_MSRENDERLOOP_UNITYURP2021";
            keywords[keywords.Length - 1] = "_MSRENDERLOOP_UNITYURP2022";
         }
      }

      public void WriteDefines (string[] features, StringBuilder sb)
      {
         sb.AppendLine ();
         for (int i = 0; i < features.Length; ++i)
         {
            sb.AppendLine ("      #define " + features [i] + " 1");
         }

         sb.AppendLine ();
      }

      public void WritePerMaterialCBuffer(string[] features, StringBuilder sb)
      {
         for (int i = 0; i < extensions.Count; ++i)
         {
            var ext = extensions [i];
            if (ext.GetVersion () == MicroSplatVersion)
            {
               extensions [i].WritePerMaterialCBuffer (features, sb);
            }
         }
      }

      public void WriteExtensions (string[] features, StringBuilder sb)
      {
         // sort for compile order
         extensions.Sort (delegate (FeatureDescriptor p1, FeatureDescriptor p2)
         {
            if (p1.CompileSortOrder () != p2.CompileSortOrder ())
               return (p1.CompileSortOrder () < p2.CompileSortOrder ()) ? -1 : 1;
            return p1.GetType ().Name.CompareTo (p2.GetType ().Name);
         });

         // shared functions first, so modules can use them in other modules
         
         for (int i = 0; i < extensions.Count; ++i)
         {
            var ext = extensions [i];
            if (ext.GetVersion () == MicroSplatVersion)
            {
               extensions [i].WriteSharedFunctions (features, sb);
            }
         }
         // now actual function.
         for (int i = 0; i < extensions.Count; ++i)
         {
            var ext = extensions [i];
            if (ext.GetVersion () == MicroSplatVersion)
            {
               extensions [i].WriteFunctions (features, sb);
            }
         }

         // sort by name, then display order..
         extensions.Sort (delegate (FeatureDescriptor p1, FeatureDescriptor p2)
         {
            if (p1.DisplaySortOrder () != 0 || p2.DisplaySortOrder () != 0)
            {
               return p1.DisplaySortOrder ().CompareTo (p2.DisplaySortOrder ());
            }
            return p1.GetType ().Name.CompareTo (p2.GetType ().Name);
         });

      }


      public void WriteProperties (string[] features, StringBuilder sb, AuxShader auxShader)
      {
         bool max4 = features.Contains ("_MAX4TEXTURES");
         bool max8 = features.Contains ("_MAX8TEXTURES");
         bool max12 = features.Contains ("_MAX12TEXTURES");
         bool max20 = features.Contains ("_MAX20TEXTURES");
         bool max24 = features.Contains ("_MAX24TEXTURES");
         bool max28 = features.Contains ("_MAX28TEXTURES");
         bool max32 = features.Contains ("_MAX32TEXTURES");

         // always have this for UVs
         sb.AppendLine ("      [HideInInspector] _Control0 (\"Control0\", 2D) = \"red\" {}");


         bool custom = features.Contains<string> ("_CUSTOMSPLATTEXTURES");
         string controlName = "_Control";
         if (custom)
         {
            controlName = "_CustomControl";
         }


         if (custom)
         {
            sb.AppendLine ("      [HideInInspector] _CustomControl0 (\"Control0\", 2D) = \"red\" {}");
         }
         if (!features.Contains ("_MICROVERTEXMESH") && !features.Contains("_MEGASPLAT") && !features.Contains("_MEGASPLATTEXTURE"))
         {
            if (!max4)
            {
               sb.AppendLine ("      [HideInInspector] " + controlName + "1 (\"Control1\", 2D) = \"black\" {}");
            }
            if (!max4 && !max8)
            {
               sb.AppendLine ("      [HideInInspector] " + controlName + "2 (\"Control2\", 2D) = \"black\" {}");
            }
            if (!max4 && !max8 && !max12)
            {
               sb.AppendLine ("      [HideInInspector] " + controlName + "3 (\"Control3\", 2D) = \"black\" {}");
            }
            if (max20 || max24 || max28 || max32)
            {
               sb.AppendLine ("      [HideInInspector] " + controlName + "4 (\"Control4\", 2D) = \"black\" {}");
            }
            if (max24 || max28 || max32)
            {
               sb.AppendLine ("      [HideInInspector] " + controlName + "5 (\"Control5\", 2D) = \"black\" {}");
            }
            if (max28 || max32)
            {
               sb.AppendLine ("      [HideInInspector] " + controlName + "6 (\"Control6\", 2D) = \"black\" {}");
            }
            if (max32)
            {
               sb.AppendLine ("      [HideInInspector] " + controlName + "7 (\"Control7\", 2D) = \"black\" {}");
            }
         }
         for (int i = 0; i < extensions.Count; ++i)
         {
            var ext = extensions [i];
            if (ext.GetVersion () == MicroSplatVersion)
            {
               ext.WriteProperties (features, sb);
            }
            sb.AppendLine ("");
         }
      }

      public static bool HasDebugFeature (string[] features)
      {
         for (int i = 0; i < features.Length; ++i)
         {
            if (features [i].StartsWith ("_DEBUG_", System.StringComparison.CurrentCulture))
               return true;
         }
         return false;
      }
      
      public IRenderLoopAdapter renderLoop = null;

      void SetPreferedRenderLoopByName(string[] features, string keyword)
      {
         if (!features.Contains (keyword))
            return;

         for (int i = 0; i < availableRenderLoops.Count; ++i)
         {
            if (availableRenderLoops [i].GetRenderLoopKeyword() == keyword)
            {

               renderLoop = availableRenderLoops [i];
            }
         }
      }

      public string Compile (string[] features, string name, string baseName = null, AuxShader auxShader = null)
      {
         try
         {
            EditorUtility.DisplayProgressBar ("Compiling Shaders", "...", 0.5f);

            Init ();

            // get default render loop if it doesn't exist
            if (renderLoop == null)
            {
               for (int i = 0; i < availableRenderLoops.Count; ++i)
               {
                  if (availableRenderLoops [i].GetType () == typeof (SurfaceShaderRenderLoopAdapter))
                  {
                     renderLoop = availableRenderLoops [i];
                  }
               }
            }

            AddPipelineKeywords(ref features);
                // TODO: this would be better if we asked the render loop if it is in the feature list, but
                // would require a change to interface, so wait until we have a version bump.
#if UNITY_2022_2_OR_NEWER
            SetPreferedRenderLoopByName(features, "_MSRENDERLOOP_UNITYHDRP2022");
            SetPreferedRenderLoopByName(features, "_MSRENDERLOOP_UNITYURP2022");
#elif UNITY_2021_2_OR_NEWER
            SetPreferedRenderLoopByName(features, "_MSRENDERLOOP_UNITYHDRP2021");
            SetPreferedRenderLoopByName(features, "_MSRENDERLOOP_UNITYURP2021");
#elif UNITY_2020_2_OR_NEWER
            SetPreferedRenderLoopByName (features, "_MSRENDERLOOP_UNITYURP2020");
            SetPreferedRenderLoopByName (features, "_MSRENDERLOOP_UNITYHDRP2020");
#else
            SetPreferedRenderLoopByName (features, "_MSRENDERLOOP_UNITYLD");
            SetPreferedRenderLoopByName (features, "_MSRENDERLOOP_UNITYHD");
#endif

                for (int i = 0; i < extensions.Count; ++i)
            {
               var ext = extensions [i];
               ext.Unpack (features);
            }

            StringBuilder sb = renderLoop.WriteShader(features, this, auxShader, name, baseName);
            for (int i = 0; i < extensions.Count; ++i)
            {
               var ext = extensions [i];
               ext.OnPostGeneration (ref sb, features, name, baseName, auxShader);
            }

            // in URP/HDRP light layers require this for terrain
            if (features.Contains("_MICROTERRAIN"))
            {
               sb = sb.Replace("#pragma instancing_options renderinglayer", "#pragma instancing_options norenderinglayer assumeuniformscaling nomatrices nolightprobe nolightmap");
            }

            string output = sb.ToString ();

            // fix newline mixing warnings..
            output = System.Text.RegularExpressions.Regex.Replace (output, "\r\n?|\n", System.Environment.NewLine);
            EditorUtility.ClearProgressBar ();
            return output;
         }
         catch
         {
            EditorUtility.ClearProgressBar ();
            return "";
         }
         
      }



      public void Compile (Material m, string shaderName = null)
      {
         int hash = 0;

         MicroSplatKeywords keywords = MicroSplatUtilities.FindOrCreateKeywords (m);

         for (int i = 0; i < keywords.keywords.Count; ++i)
         {
            hash += 31 + keywords.keywords [i].GetHashCode ();
         }
         var path = AssetDatabase.GetAssetPath (m.shader);
         string nm = m.shader.name;
         if (!string.IsNullOrEmpty (shaderName))
         {
            nm = shaderName;
         }
         string baseName = "Hidden/" + nm + "_Base" + hash.ToString ();

         string terrainShader = Compile (keywords.keywords.ToArray (), nm, baseName);
         if (renderLoop != null)
         {
            keywords.EnableKeyword (renderLoop.GetRenderLoopKeyword ());
         }

         GenerateAuxShaders (nm, path, keywords.keywords);

         if (keywords.keywords.Contains ("_MSRENDERLOOP_BETTERSHADERS"))
         {
            path = path.Replace (".shader", ".surfshader");
         }
         else
         {
            path = path.Replace(".surfshader", ".shader");
         }
         MicroSplatUtilities.Checkout (path);
         System.IO.File.WriteAllText (path, terrainShader);

         if (keywords.IsKeywordEnabled ("_MICROTERRAIN"))
         {
            // generate fallback
            string[] oldKeywords = new string[keywords.keywords.Count];
            System.Array.Copy (keywords.keywords.ToArray (), oldKeywords, keywords.keywords.Count);
            keywords.DisableKeyword ("_TESSDISTANCE");
            keywords.DisableKeyword("_TESSEDGE");
            keywords.DisableKeyword ("_POM");
            keywords.DisableKeyword ("_PARALLAX");
            keywords.DisableKeyword ("_DETAILNOISE");
            keywords.EnableKeyword ("_MICROSPLATBASEMAP");

            string fallback = Compile (keywords.keywords.ToArray (), baseName);
            keywords.keywords = new List<string> (oldKeywords);
            string fallbackPath = ReplaceLastOccurrence(path, ".shader", "_Base.shader");
            fallbackPath = ReplaceLastOccurrence(fallbackPath, ".surfshader", "_Base.surfshader");
            MicroSplatUtilities.Checkout (fallbackPath);
            System.IO.File.WriteAllText (fallbackPath, fallback);
         }


         EditorUtility.SetDirty (m);
         AssetDatabase.Refresh ();
#if __MICROSPLAT_MESH__
         MicroSplatMesh.ClearMaterialCache ();
#endif
         MicroSplatObject.SyncAll ();
      }
   }
}
