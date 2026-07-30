//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;

namespace JBooth.MicroSplat
{
   public class SurfaceShaderRenderLoopAdapter : IRenderLoopAdapter 
   {
      
      public string GetDisplayName() 
      { 
         return "Standard"; 
      }

      public string GetShaderExtension()
      {
         return "shader";
      }

      public string GetRenderLoopKeyword() 
      {
         return "_MSRENDERLOOP_SURFACESHADER";
      }

      public string GetVersion()
      {
         return MicroSplatShaderGUI.MicroSplatVersion;
      }

      MicroSplatGenerator gen;

      TextAsset templateStandard;
      TextAsset templatePassForward;
      TextAsset templatePassForwardAdd;
      TextAsset templatePassGBuffer;
      TextAsset templatePassShadow;
      TextAsset templatePassMeta;
      TextAsset templateShared;
      TextAsset templateShaderDesc;
      TextAsset templateCommonHLSL;
      TextAsset templateChain;
      TextAsset templateTess;
      

      public void Init(string[] paths)
      {
         if (gen == null)
         {
            gen = new MicroSplatGenerator();
         }
         gen.Init(paths);
         foreach (var p in paths)
         {
            if (p.EndsWith("microsplat_template_standard.txt"))
            {
               templateStandard = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_func_tess2.txt"))
            {
               templateTess = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_chain.txt"))
            {
               templateChain = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_standard_passforward.txt"))
            {
               templatePassForward = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_standard_passforwardadd.txt"))
            {
               templatePassForwardAdd = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_standard_passgbuffer.txt"))
            {
               templatePassGBuffer = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_standard_passmeta.txt"))
            {
               templatePassMeta = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_standard_passshadow.txt"))
            {
               templatePassShadow = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_standard_commonHLSL.txt"))
            {
               templateCommonHLSL = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_shaderdesc.txt"))
            {
               templateShaderDesc = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
            else if (p.EndsWith("microsplat_template_shared.txt"))
            {
               templateShared = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
         }

         if (templateStandard == null)
         {
            Debug.LogError ("Cannot find microsplat_template_standard.txt, will not be able to compile valid shaders");
         }
         if (templateChain == null)
         {
            Debug.LogError ("Cannot find microsplat_template_chain.txt, will not be able to compile valid shaders");
         }
         if (templatePassForward == null)
         {
            Debug.LogError ("Cannot find template for pass forward, will not be able to compile valid shaders");
         }
         if (templatePassForwardAdd == null)
         {
            Debug.LogError ("Cannot find template for pass forward add, will not be able to compile valid shaders");
         }
         if (templatePassGBuffer == null)
         {
            Debug.LogError ("Cannot find template for pass gbuffer, will not be able to compile valid shaders");
         }
         if (templatePassShadow == null)
         {
            Debug.LogError ("Cannot find template for pass shadow, will not be able to compile valid shaders");
         }
         if (templatePassMeta == null)
         {
            Debug.LogError ("Cannot find template for pass meta, will not be able to compile valid shaders");
         }
         if (templateShaderDesc == null)
         {
            Debug.LogError ("Cannot find template for shader desc, will not be able to compile valid shaders");
         }
         if (templateShared == null)
         {
            Debug.LogError ("Cannot find template for template shared, will not be able to compile valid shaders");
         }
         if (templateCommonHLSL == null)
         {
            Debug.LogError ("Cannot find template for template common HLSL, will not be able to compile valid shaders");
         }
      }

      public static StringBuilder Strip(string code, StringBuilder template, string param, string[] keys)
      {
         bool contains = false;
         foreach (var k in keys)
         {
            if (code.Contains(k))
            {
               contains = true;
               break;
            }
         }
         if (contains)
         {
            return template.Replace(param, "");
         }
         else
         {
            return template.Replace(param, "//");
         }
      }

      public static StringBuilder Strip(string code, StringBuilder template, string param, string key)
      {
         if (code.Contains(key))
         {
            return template.Replace(param, "");
         }
         else
         {
            return template.Replace(param, "//");
         }
      }

      public static StringBuilder Strip(string code, StringBuilder template)
      {

         template = Strip(code, template, "%SCREENPOS%", new string[] { ".screenPos", ".screenUV" });
         template = Strip(code, template, "%UV0%", ".texcoord0");
         template = Strip(code, template, "%UV1%", ".texcoord1");
         template = Strip(code, template, "%UV2%", ".texcoord2");
         template = Strip(code, template, "%UV3%", ".texcoord3");
         template = Strip(code, template, "%V2FUV0%", ".texcoord0");
         template = Strip(code, template, "%V2FUV1%", ".texcoord1");
         template = Strip(code, template, "%V2FUV2%", ".texcoord2");
         template = Strip(code, template, "%V2FUV3%", ".texcoord3");
         template = Strip(code, template, "%LOCALSPACEPOSITION%", ".localSpacePosition");
         template = Strip(code, template, "%LOCALSPACENORMAL%", ".localSpaceNormal");
         template = Strip(code, template, "%LOCALSPACETANGENT%", ".localSpaceTangent");
         template = Strip(code, template, "%VERTEXCOLOR%", ".vertexColor");
         template = Strip(code, template, "%V2FVERTEXCOLOR%", ".vertexColor");

         template = Strip(code, template, "%EXTRAV2F0%", ".extraV2F0");
         template = Strip(code, template, "%EXTRAV2F1%", ".extraV2F1");
         template = Strip(code, template, "%EXTRAV2F2%", ".extraV2F2");
         template = Strip(code, template, "%EXTRAV2F3%", ".extraV2F3");
         template = Strip(code, template, "%EXTRAV2F4%", ".extraV2F4");
         template = Strip(code, template, "%EXTRAV2F5%", ".extraV2F5");
         template = Strip(code, template, "%EXTRAV2F6%", ".extraV2F6");
         template = Strip(code, template, "%EXTRAV2F7%", ".extraV2F7");
         template = Strip(code, template, "%VERTEXID%", ".vertexID");

         template = template.Replace("ChainSurfaceFunction", "SurfaceFunction");
         

         return template;
      }

      public static StringBuilder ReplaceOrRemove(StringBuilder template, string key, string tag, string option)
      {
         if (!string.IsNullOrEmpty(option))
         {
            return template.Replace(key, tag + " \"" + option + "\"");
         }
         else
         {
            return template.Replace(key, "");
         }
      }



      public static string GetTags(string options)
      {
         string[] lines = options.ToLines();
         foreach (var l in lines)
         {
            string s = l.Trim();
            if (s.Contains("Tags {"))
            {
               return l;
            }
         }
         return "";
      }

      public static string GetOption(string options, string name)
      {
         string[] lines = options.ToLines();
         foreach (var l in lines)
         {
            string s = l.Trim();
            if (s.Contains(name))
            {
               return s.Replace(name, "").Replace("\"", "").Trim();
            }
         }
         return "";
      }

      StringBuilder BuildTemplate(Blocks blocks, string fallbackOverride = null)
      {
         
         var template = new StringBuilder(100000);
         template.Append(templateStandard.text);
         var passforward = new StringBuilder(templatePassForward.text);
         var passforwardAdd = new StringBuilder(templatePassForwardAdd.text);
         var passGBuffer = new StringBuilder(templatePassGBuffer.text);
         var passShadow = new StringBuilder(templatePassShadow.text);
         var passMeta = new StringBuilder(templatePassMeta.text);

         if (blocks.defines.Contains("_BDRF3") || blocks.defines.Contains("_BDRFLAMBERT"))
         {
            passGBuffer.Length = 0;
         }

         if (blocks.options.Contains("Unlit"))
         {
            passGBuffer.Length = 0;
            passforwardAdd.Length = 0;
         }

         if (blocks.options.Contains("Alpha"))
         {
            passGBuffer.Length = 0;
            passShadow.Length = 0;
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "Blend SrcAlpha OneMinusSrcAlpha");
            passforwardAdd = passforwardAdd.Replace("%FORWARDADDBLEND%", "Blend SrcAlpha One");
            blocks.defines += "\n   #define _ALPHABLEND_ON 1";

            passforward = passforward.Insert(0, "\nZWrite Off ColorMask RGB\n\n");
         }
         else
         {
            passforward = passforward.Replace("%FORWARDBASEBLEND%", "");
            passforwardAdd = passforwardAdd.Replace("%FORWARDADDBLEND%", "");
         }

         template = template.Replace("%PASSGBUFFER%", passGBuffer.ToString());
         template = template.Replace("%PASSMETA%", passMeta.ToString());
         template = template.Replace("%PASSFORWARD%", passforward.ToString());
         template = template.Replace("%PASSFORWARDADD%", passforwardAdd.ToString());
         template = template.Replace("%PASSSHADOW%", passShadow.ToString());
         
         
         StringBuilder header = new StringBuilder();
         header.AppendLine("////////////////////////////////////////");
         header.AppendLine("// MicroSplat");
         header.AppendLine("// Copyright (c) Jason Booth");
         header.AppendLine("//");
         header.AppendLine("// Auto-generated shader code, don't hand edit!");
         header.AppendLine("//");
         header.AppendLine("//   Unity Version: " + Application.unityVersion);
         header.AppendLine("//   MicroSplat Version: " + MicroSplatShaderGUI.MicroSplatVersion);
         header.AppendLine("//   Render Pipeline: Standard");
         header.AppendLine("//   Platform: " + Application.platform);
         header.AppendLine("////////////////////////////////////////\n\n");

         template = template.Insert(0, header);

         template = template.Replace("%TAGS%", GetTags(blocks.options));



         template = template.Replace("%TEMPLATE_SHARED%", templateShared.text);
         template = ReplaceOrRemove(template, "%CUSTOMEDITOR%", "CustomEditor", GetOption(blocks.options, "CustomEditor"));
         if (fallbackOverride != null)
         {
            template = template.Replace("%FALLBACK%", "Fallback \"" + fallbackOverride + "\"");
            template = ReplaceOrRemove(template, "%DEPENDENCY%", "Dependency \"BaseMapShader\" = ", fallbackOverride);
         }
         else
         {
            template = ReplaceOrRemove(template, "%FALLBACK%", "Fallback", "");
            template = ReplaceOrRemove(template, "%DEPENDENCY%", "Dependency", "");
         }
         
         return template;
      }

      public StringBuilder WriteShader(string[] features,
            MicroSplatShaderGUI.MicroSplatCompiler compiler,
            MicroSplatShaderGUI.MicroSplatCompiler.AuxShader auxShader,
            string name,
            string baseName)
      {
         StringBuilder defines = new StringBuilder();
         var blocks = gen.GetShaderBlocks(features, compiler, auxShader);

         var shader = BuildTemplate(blocks, baseName);
         defines.AppendLine(blocks.defines);
         defines.AppendLine("\n   #define _STANDARD 1");


         string shaderTarget = "3.0";
         if (features.Contains("_TESSDISTANCE") || features.Contains("_FORCEMODEL46"))
         {
            shaderTarget = "4.6";
         }
         else if (features.Contains("_FORCEMODEL50"))
         {
            shaderTarget = "5.0";
         }

         if (features.Contains("_TESSDISTANCE") || features.Contains("_TESSEDGE"))
         {
            defines.AppendLine("\n      #define _TESSELLATION_ON 1");
            shader = shader.Replace("%TESSELLATION%", templateTess.text);
            shader = shader.Replace("%PRAGMAS%", "   #pragma hull Hull\n   #pragma domain Domain\n   #pragma vertex TessVert\n   #pragma fragment Frag\n   #pragma require tesshw\n");
         }
         else
         {
            shader = shader.Replace("%PRAGMAS%", "   #pragma vertex Vert\n   #pragma fragment Frag");
            shader = shader.Replace("%TESSELLATION%", "");
         }






         shader = shader.Replace("%SHADERTARGET%", shaderTarget);
         if (features.Contains("_USESPECULARWORKFLOW"))
         {
            defines.AppendLine("\n#define _USESPECULAR 1");
            defines.AppendLine("#define _MATERIAL_FEATURE_SPECULAR_COLOR 1");
         }

         defines.AppendLine();
         defines.AppendLine(templateCommonHLSL.text);

         string shaderDesk = templateShaderDesc.text;
         shaderDesk += templateChain.text;
         shader = shader.Replace("%SHADERDESC%", shaderDesk);
         shader = shader.Replace("%SHADERNAME%", name);
         shader = shader.Replace("%PROPERTIES%", blocks.properties);
         shader = shader.Replace("%CODE%", blocks.code);
         shader = shader.Replace("%DEFINES%", defines.ToString());
         shader = shader.Replace("%CBUFFER%", blocks.cbuffer);
         string codeNoComments = blocks.code.StripComments();

         shader = Strip(codeNoComments, shader);

         return shader;
      }
   }
}
