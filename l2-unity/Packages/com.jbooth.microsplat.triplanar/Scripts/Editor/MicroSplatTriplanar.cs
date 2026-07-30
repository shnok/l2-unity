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
   #if __MICROSPLAT__
   [InitializeOnLoad]
   public class MicroSplatTriplanar : FeatureDescriptor
   {
      const string sDefine = "__MICROSPLAT_TRIPLANAR__";
      static MicroSplatTriplanar()
      {
         MicroSplatDefines.InitDefine(sDefine);
      }
      [PostProcessSceneAttribute (0)]
      public static void OnPostprocessScene()
      { 
         MicroSplatDefines.InitDefine(sDefine);
      }

      public override string ModuleName()
      {
         return "Triplanar";
      }

      public override string GetHelpPath ()
      {
         return "https://docs.google.com/document/d/1eRYatxNlDNHb72Z8mRzBPBqgYD7pYc_CWtwxzKLCjXk/edit?usp=sharing";
      }

      public enum DefineFeature
      {
         _TRIPLANAR,
         _PERTEXTRIPLANAR,
         _TRIPLANARHEIGHTBLEND,
         _PERTEXTRIPLANARCONTRAST,
         _TRIPLANARLOCALSPACE,
         _TRIPLANARUSEFACENORMALS,
         kNumFeatures,
      }

      public enum TriplanarSpace
      {
         World = 0,
         Local
      }

      public enum TriplanarMode
      {
         None, 
         Triplanar,
         HeightBlended
      }

      public override int CompileSortOrder()
      {
         return -1000;  // has to be first, since things need uvs..
      }
         
      static TextAsset funcs;

      public TriplanarMode triplanarMode = TriplanarMode.None;
      public bool perTexTriplanar;
      public bool perTexTriplanarContrast;
      public TriplanarSpace space = TriplanarSpace.World;
      public bool useFaceNormals;

      GUIContent CTriplanar = new GUIContent("Triplanar Mode", "Use triplanar UV projection, optionally with height based blending");
      GUIContent CSpace = new GUIContent("Space", "Coordinate space to use for triplanar");

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

      static GUIContent CTriplanarContrast = new GUIContent("Triplanar Contrast", "Tightness of blend between three triplanar angles");

      public override string GetVersion()
      {
         return "3.9";
      }

      public override void DrawFeatureGUI(MicroSplatKeywords keywords)
      {
         triplanarMode = (TriplanarMode)EditorGUILayout.EnumPopup(CTriplanar, triplanarMode);
         if (triplanarMode != TriplanarMode.None)
         {
            EditorGUI.indentLevel++;
            space = (TriplanarSpace)EditorGUILayout.EnumPopup(CSpace, space);
            useFaceNormals = EditorGUILayout.Toggle ("Blend Face Normals", useFaceNormals);
            EditorGUI.indentLevel--;
         }
      }

      public override void DrawShaderGUI(MicroSplatShaderGUI shaderGUI, MicroSplatKeywords keywords, Material mat, MaterialEditor materialEditor, MaterialProperty[] props)
      {
         if (triplanarMode != TriplanarMode.None)
         {
            if (MicroSplatUtilities.DrawRollup("Triplanar") && mat.HasProperty("_TriplanarContrast"))
            {
               materialEditor.ShaderProperty(shaderGUI.FindProp("_TriplanarContrast", props), CTriplanarContrast);
               if (mat.HasProperty("_TriplanarUVScale"))
               {
                  EditorGUI.BeginChangeCheck();
                  Vector4 uvScale = shaderGUI.FindProp("_TriplanarUVScale", props).vectorValue;
                  Vector2 scl = new Vector2(uvScale.x, uvScale.y);
                  Vector2 offset = new Vector2(uvScale.z, uvScale.w);
                  scl = EditorGUILayout.Vector2Field("Triplanar UV Scale", scl);
                  offset = EditorGUILayout.Vector2Field("Triplanar UV Offset", offset);
                  if (EditorGUI.EndChangeCheck ())
                  {
                     uvScale.x = scl.x;
                     uvScale.y = scl.y;
                     uvScale.z = offset.x;
                     uvScale.w = offset.y;
                     shaderGUI.FindProp ("_TriplanarUVScale", props).vectorValue = uvScale;
                     EditorUtility.SetDirty (mat);
                  }
               }
               if (mat.HasProperty("_TriplanarFaceBlend"))
               {
                  materialEditor.ShaderProperty (shaderGUI.FindProp ("_TriplanarFaceBlend", props), "Vertex -> Face Normal");
               }
            }
         }

      }

      public override string[] Pack()
      {
         List<string> features = new List<string>();
         if (triplanarMode != TriplanarMode.None)
         {
            features.Add(GetFeatureName(DefineFeature._TRIPLANAR));
            if (space == TriplanarSpace.Local)
            {
               features.Add(GetFeatureName(DefineFeature._TRIPLANARLOCALSPACE));
            }
            if (perTexTriplanar)
            {
               features.Add(GetFeatureName(DefineFeature._PERTEXTRIPLANAR));
            }
            if (triplanarMode == TriplanarMode.HeightBlended)
            {
               features.Add(GetFeatureName(DefineFeature._TRIPLANARHEIGHTBLEND));
               if (perTexTriplanarContrast)
               {
                  features.Add(GetFeatureName(DefineFeature._PERTEXTRIPLANARCONTRAST));
               }
            }
            if (useFaceNormals)
            {
               features.Add (GetFeatureName (DefineFeature._TRIPLANARUSEFACENORMALS));
            }
         }

         return features.ToArray();
      }

      public override void Unpack(string[] keywords)
      {
         triplanarMode = TriplanarMode.None;
         if (HasFeature(keywords, DefineFeature._TRIPLANAR))
            triplanarMode = TriplanarMode.Triplanar;
         if (HasFeature(keywords, DefineFeature._TRIPLANARHEIGHTBLEND))
            triplanarMode = TriplanarMode.HeightBlended;
         useFaceNormals = HasFeature (keywords, DefineFeature._TRIPLANARUSEFACENORMALS);
         space = HasFeature(keywords, DefineFeature._TRIPLANARLOCALSPACE) ? TriplanarSpace.Local : TriplanarSpace.World;

         perTexTriplanar = HasFeature(keywords, DefineFeature._PERTEXTRIPLANAR);
         perTexTriplanarContrast = HasFeature(keywords, DefineFeature._PERTEXTRIPLANARCONTRAST);
      }

      public override void InitCompiler(string[] paths)
      {
         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths[i];
            if (p.EndsWith("microsplat_func_triplanar.txt"))
            {
               funcs = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
         }
      }

      public override void WriteProperties(string[] features, System.Text.StringBuilder sb)
      {
         if (triplanarMode != TriplanarMode.None || System.Array.Exists(features, element => element == "_TERRAINBLENDMATCHWORLDUV"))
         {
            sb.AppendLine("      _TriplanarContrast(\"Triplanar Contrast\", Range(1.0, 8)) = 4");
            if (useFaceNormals)
            {
               sb.AppendLine ("     _TriplanarFaceBlend(\"Triplanar Face Blend\", Range(0,1)) = 0");
            }
            
         }
         // always write this property so conversion can set it..
         sb.AppendLine ("      _TriplanarUVScale(\"Triplanar UV Scale\", Vector) = (1, 1, 0, 0)");
      }

      public override void WritePerMaterialCBuffer (string[] features, System.Text.StringBuilder sb)
      {
         if (triplanarMode != TriplanarMode.None || System.Array.Exists(features, element => element == "_TERRAINBLENDMATCHWORLDUV"))
         {
            sb.AppendLine ("      float _TriplanarContrast;");
            sb.AppendLine ("      float4 _TriplanarUVScale;");
            if (useFaceNormals)
            {
               sb.AppendLine ("      half _TriplanarFaceBlend;");
            }
         }
      }

      public override void WriteFunctions(string [] features, System.Text.StringBuilder sb)
      {
         // needs to happen first..
         if (triplanarMode != TriplanarMode.None)
         {
            sb.AppendLine(funcs.text);
         }
      }

      static GUIContent CPerTexTriplanar = new GUIContent("Triplanar Projection", "Allows you to choose between Triplanar, side projection, and top down projection for this texture");
      static GUIContent CPerTexTriplanarContrast = new GUIContent("Triplanar Height Contrast", "Allows you to control the blend width of height based triplanar transitions per texture");

      public override void DrawPerTextureGUI(int index, MicroSplatKeywords keywords, Material mat, MicroSplatPropData propData)
      {
         if (triplanarMode != TriplanarMode.None)
         {
            InitPropData(9, propData, new Color(0.0f, 0.5f, 0.0f, 0.0f));

            perTexTriplanar = DrawPerTexFloatSlider(index, 9, GetFeatureName(DefineFeature._PERTEXTRIPLANAR), 
               keywords, propData, Channel.R, CPerTexTriplanar, 0, 1);

            if (triplanarMode == TriplanarMode.HeightBlended)
            {
               perTexTriplanarContrast = DrawPerTexFloatSlider(index, 9, GetFeatureName(DefineFeature._PERTEXTRIPLANARCONTRAST), 
                  keywords, propData, Channel.G, CPerTexTriplanarContrast, 0.01f, 1);
            }
         }
      }

      public override void ComputeSampleCounts(string[] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, ref int tessellationSamples, ref int depTexReadLevel)
      {
         if (triplanarMode != TriplanarMode.None)
         {
            // need to have this evaluated last
            arraySampleCount *= 3;
         }
      }
         
   }   

   #endif
}