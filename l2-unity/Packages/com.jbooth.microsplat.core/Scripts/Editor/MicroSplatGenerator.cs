//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JBooth.MicroSplat
{
   public static class StringExtensions
   {
      public static string RemoveBetween(this string s, char begin, char end)
      {
         Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
         return regex.Replace(s, string.Empty);
      }

      public static string StripComments(this string str)
      {
         var blockComments = @"/\*(.*?)\*/";
         var lineComments = @"//(.*?)\r?\n";
         var strings = @"""((\\[^\n]|[^""\n])*)""";
         var verbatimStrings = @"@(""[^""]*"")+";

         string noComments = Regex.Replace(str, blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings, me =>
         {
            if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
               return me.Value.StartsWith("//") ? System.Environment.NewLine : "";
            return me.Value;
         },
             RegexOptions.Singleline);

         return noComments;
      }

      public static string[] ToLines(this string str)
      {
         return str.Split("\n\r".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
      }
   }

   public class Blocks
   {
      public string cbuffer;
      public string defines;
      public string code;
      public string properties;
      public string options;
   }

   [InitializeOnLoad]
   public class MicroSplatGenerator
   {
      

      void WriteOptions(string[] features, StringBuilder sb, MicroSplatShaderGUI.MicroSplatCompiler compiler, MicroSplatShaderGUI.MicroSplatCompiler.AuxShader auxShader, string baseName)
      {

         if (features.Contains("_TESSEDGE"))
         {
            sb.AppendLine("Tessellation \"Edge\"");
         }
         else if (features.Contains("_TESSDISTANCE"))
         {
            sb.AppendLine("Tessellation \"Distance\"");
         }
         

         sb.Append("      Tags {\"RenderType\" = \"Opaque\" \"Queue\" = \"Geometry+100\" \"IgnoreProjector\" = \"False\" ");

         if (features.Contains("_MICROTERRAIN") || features.Contains("_MEGASPLATTEXTURE"))
         {
            sb.Append(" \"TerrainCompatible\" = \"true\" ");
         }
         if (features.Contains("_MAX4TEXTURES"))
         {
            sb.Append("\"SplatCount\" = \"4\"");
         }
         else if (features.Contains("_MAX8TEXTURES"))
         {
            sb.Append("\"SplatCount\" = \"8\"");
         }
         else if (features.Contains("_MAX12TEXTURES"))
         {
            sb.Append("\"SplatCount\" = \"12\"");
         }
         else if (features.Contains("_MAX20TEXTURES"))
         {
            sb.Append("\"SplatCount\" = \"20\"");
         }
         else if (features.Contains("_MAX24TEXTURES"))
         {
            sb.Append("\"SplatCount\" = \"24\"");
         }
         else if (features.Contains("_MAX28TEXTURES"))
         {
            sb.Append("\"SplatCount\" = \"28\"");
         }
         else if (features.Contains("_MAX32TEXTURES"))
         {
            sb.Append("\"SplatCount\" = \"32\"");
         }
         else
         {
            sb.Append("\"SplatCount\" = \"16\"");
         }
         sb.AppendLine("}");

         if (features.Contains("_MESHOVERLAYSPLATS"))
         {
            sb.AppendLine("   Alpha \"Blend\"");
         }
         if (auxShader != null && !string.IsNullOrEmpty(auxShader.customEditor))
         {
            sb.AppendLine("      CustomEditor \"" + auxShader.customEditor + "\"");
         }
         if (auxShader != null && !string.IsNullOrEmpty(auxShader.options))
         {
            sb.AppendLine(auxShader.options);
         }
         else if (auxShader != null)
         {
            sb.AppendLine("   CustomEditor \"MicroSplatShaderGUI\"");
         }
         else if (baseName == null)
         {
            sb.AppendLine("   CustomEditor \"MicroSplatShaderGUI\"");
         }
         else if (baseName != null)
         {
            if (features.Contains ("_MICROTERRAIN") && !features.Contains ("_TERRAINBLENDABLESHADER"))
            {
               sb.AppendLine("   Dependency {\"BaseMapShader\" = \"" + baseName + "\"}");
            }
            sb.AppendLine("   CustomEditor \"MicroSplatShaderGUI\"");
         }
         sb.AppendLine("   Fallback \"Nature/Terrain/Diffuse\"");

      }

      static TextAsset adapter;
      static TextAsset sharedInc;
      static TextAsset terrainBody;
      static TextAsset terrainBlendBody;
      static TextAsset terrainBlendCBuffer;
      static TextAsset sharedHD;
      static TextAsset vertex;
      static TextAsset vertexData;
      static TextAsset megasplatData;
      static TextAsset mainFunc;


      public void Init(string[] paths)
      {
         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths[i];
            if (p.EndsWith("microsplat_template_adapter.txt"))
            {
               adapter = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_terrainblend_body.txt"))
            {
               terrainBlendBody = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_terrainblend_cbuffer.txt"))
            {
               terrainBlendCBuffer = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }

            if (p.EndsWith("microsplat_terrain_body.txt"))
            {
               terrainBody = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_shared.txt"))
            {
               sharedInc = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_terrain_core_shared.txt"))
            {
               sharedHD = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_terrain_core_vertex.txt"))
            {
               vertex = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_terrain_core_vertexdata.txt"))
            {
               vertexData = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_terrain_core_megasplat.txt"))
            {
               megasplatData = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            if (p.EndsWith("microsplat_terrain_core_mainfunc.txt"))
            {
               mainFunc = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
         }
      }


      string StripMegaSplat(string[] features)
      {
         if (features.Contains("_MEGASPLAT") && megasplatData != null)
         {
            var lines = megasplatData.text.ToLines();
            var stripList = new List<string>();
            stripList.Add("%SCATTER%");
            stripList.Add("%SCATTER2%");
            stripList.Add("%FX%");
            stripList.Add("%FX2%");

            if (features.Contains("_SCATTER"))
            {
               stripList.Remove("%SCATTER%");
            }
            if (features.Contains("_SCATTERSECONDLAYER"))
            {
               stripList.Remove("%SCATTER2%");
            }
            if (features.Contains("_WETNESS") || features.Contains("_PUDDLES") || features.Contains("_STREAMS") || features.Contains("_LAVA"))
            {
               stripList.Remove("%FX%");
            }
            if (features.Contains("_SNOW") && features.Contains("_SNOWMASK"))
            {
               stripList.Remove("%FX2%");
            }
            // strip any lines we need to remove
            for (int i = 0; i < lines.Length; ++i)
            {
               string l = lines[i];
               foreach (var s in stripList)
               {
                  if (l.Contains(s))
                  {
                     lines[i] = "";
                  }
               }
            }
            // strip lines remaining.
            string result = string.Join("\n", lines);
            result = result.Replace("%SCATTER%", "");
            result = result.Replace("%SCATTER2%", "");
            result = result.Replace("%FX%", "");
            result = result.Replace("%FX2%", "");
            return result;
         }
         return "";
      }

      string StripVertexWorkflow(string[] features)
      {
         if (vertexData != null &&
            (features.Contains("_MICROVERTEXMESH") || features.Contains("_MICRODIGGERMESH"))
         )
         {
            var lines = vertexData.text.ToLines();

            var stripList = new List<string>();
            stripList.Add("%MAX8%");
            stripList.Add("%MAX12%");
            stripList.Add("%MAX16%");
            stripList.Add("%MAX20%");
            stripList.Add("%MAX24%");
            stripList.Add("%MAX28%");
            stripList.Add("%FX%");
            stripList.Add("%FX2%");

            if (features.Contains("_MAX8TEXTURES"))
            {
               stripList.Remove("%MAX8%");
            }
            else if (features.Contains("_MAX12TEXTURES"))
            {
               stripList.Remove("%MAX8%");
               stripList.Remove("%MAX12%");
            }
            else if (features.Contains("_MAX20TEXTURES"))
            {
               stripList.Remove("%MAX8%");
               stripList.Remove("%MAX12%");
               stripList.Remove("%MAX16%");
               stripList.Remove("%MAX20%");
            }
            else if (features.Contains("_MAX24TEXTURES"))
            {
               stripList.Remove("%MAX8%");
               stripList.Remove("%MAX12%");
               stripList.Remove("%MAX16%");
               stripList.Remove("%MAX20%");
               stripList.Remove("%MAX24%");
            }
            else if (features.Contains("_MAX28TEXTURES"))
            {
               stripList.Remove("%MAX8%");
               stripList.Remove("%MAX12%");
               stripList.Remove("%MAX16%");
               stripList.Remove("%MAX20%");
               stripList.Remove("%MAX24%");
               stripList.Remove("%MAX28%");
            }
            else if (features.Contains("_MAX32TEXTURES"))
            {
               stripList.Remove("%MAX8%");
               stripList.Remove("%MAX12%");
               stripList.Remove("%MAX16%");
               stripList.Remove("%MAX20%");
               stripList.Remove("%MAX24%");
               stripList.Remove("%MAX28%");
            }
            else
            {
               stripList.Remove("%MAX8%");
               stripList.Remove("%MAX12%");
               stripList.Remove("%MAX16%");
            }

            if (features.Contains("_WETNESS") || features.Contains("_PUDDLES") || features.Contains("_STREAMS") || features.Contains("_LAVA"))
            {
               stripList.Remove("%FX%");
            }
            if (features.Contains("_SNOW") && features.Contains("_SNOWMASK"))
            {
               stripList.Remove("%FX2%");
            }
            // strip any lines we need to remove
            for (int i = 0; i < lines.Length; ++i)
            {
               string l = lines[i];
               foreach (var s in stripList)
               {
                  if (l.Contains(s))
                  {
                     lines[i] = "";
                  }
               }
            }
            // strip lines remaining.
            string result = string.Join("\n", lines);
            result = result.Replace("%MAX8%", "");
            result = result.Replace("%MAX12%", "");
            result = result.Replace("%MAX16%", "");
            result = result.Replace("%MAX20%", "");
            result = result.Replace("%MAX24%", "");
            result = result.Replace("%MAX28%", "");
            result = result.Replace("%FX%", "");
            result = result.Replace("%FX2%", "");
            return result;
         }
         else
         {
            return "";
         }
      }

      public Blocks GetShaderBlocks(string[] features,
            MicroSplatShaderGUI.MicroSplatCompiler compiler,
            MicroSplatShaderGUI.MicroSplatCompiler.AuxShader auxShader)
      {


         StringBuilder defines = new StringBuilder();
         compiler.WriteDefines(features, defines);
         if (auxShader != null && auxShader.trigger == "_TERRAINBLENDING")
         {
            defines.AppendLine("      #define _SRPTERRAINBLEND 1");
         }

         if (features.Contains("_USESPECULARWORKFLOW"))
         {
            defines.AppendLine("      #define _SPECULAR_SETUP");
         }
         if (features.Contains("_MICROTERRAIN") && !features.Contains("_TERRAINBLENDABLESHADER")) // digger? mesh terrain?
         {
            defines.AppendLine("#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd");
         }
         StringBuilder cbuffer = new StringBuilder();
         compiler.WritePerMaterialCBuffer(features, cbuffer);
         if (auxShader != null && auxShader.trigger == "_TERRAINBLENDING")
         {
            cbuffer.AppendLine(terrainBlendCBuffer.text);
         }

         StringBuilder options = new StringBuilder();
         WriteOptions(features, options, compiler, auxShader, null);
         StringBuilder properties = new StringBuilder();
         compiler.WriteProperties(features, properties, auxShader);

         StringBuilder ext = new StringBuilder();
         compiler.WriteExtensions(features, ext);

         StringBuilder afterVertex = new StringBuilder();
         foreach (var e in compiler.extensions)
         {
            e.WriteAfterVetrexFunctions(afterVertex);
         }

         StringBuilder code = new StringBuilder(100000);
         code.AppendLine(adapter.text);
         code.AppendLine(sharedInc.text);
         code.AppendLine(StripVertexWorkflow(features));
         code.AppendLine((StripMegaSplat(features)));
         code.AppendLine(sharedHD.text);
         code.AppendLine(ext.ToString());
         code.AppendLine(terrainBody.text);
         code.AppendLine(vertex.text);
         if (auxShader != null && auxShader.trigger == "_TERRAINBLENDING")
         {
            code.AppendLine(terrainBlendBody.text);
         }
         code.AppendLine(mainFunc.text);
         code.AppendLine(afterVertex.ToString());

         Blocks b = new Blocks();
         b.code = code.ToString();
         b.cbuffer = cbuffer.ToString();
         b.properties = properties.ToString();
         b.defines = defines.ToString();
         b.options = options.ToString();
         
         return b;
      }
   }
}
