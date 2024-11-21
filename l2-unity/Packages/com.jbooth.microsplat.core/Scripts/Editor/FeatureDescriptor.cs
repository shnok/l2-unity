//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace JBooth.MicroSplat
{
   public abstract class FeatureDescriptor
   {
      /// <summary>
      /// All versions must match for module to be active
      /// </summary>
      /// <returns>The version.</returns>
      public abstract string GetVersion();


      public virtual bool HideModule()
      {
         return false;
      }
      // used when you have compiler ordering issues
      public virtual int CompileSortOrder() 
      {
         return 0;
      }

      public virtual int DisplaySortOrder()
      {
         return 0;
      }

      public virtual string GetHelpPath() { return null; }

      public abstract string ModuleName();

      /// <summary>
      /// Called after the shader is generated, for search/replace operations
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="features"></param>
      /// <param name="name"></param>
      /// <param name="baseName"></param>
      /// <param name="auxShader"></param>
      public virtual void OnPostGeneration(ref StringBuilder sb, string[] features, string name, string baseName = null, MicroSplatShaderGUI.MicroSplatCompiler.AuxShader auxShader = null) { }

      /// <summary>
      /// If you wish to generate an addtional shader with special functionality, return one of these
      /// </summary>
      /// <returns></returns>
      public virtual MicroSplatShaderGUI.MicroSplatCompiler.AuxShader GetAuxShader () { return null; }
      /// <summary>
      /// if you return above, this will be called when your additional shader is about to be compiled, allowing you to modify the keywords list for you extra shader
      /// </summary>
      /// <param name="keywords"></param>
      public virtual void ModifyKeywordsForAuxShader(List<string> keywords) { }
      


      /// <summary>
      /// Requireses the shader model46.
      /// </summary>
      /// <returns><c>true</c>, if shader model46 was requiresed, <c>false</c> otherwise.</returns>
      public virtual bool RequiresShaderModel46() { return false; }

      /// <summary>
      /// DrawGUI for shader compiler feature options
      /// </summary>
      /// <param name="mat">Mat.</param>
      public abstract void DrawFeatureGUI(MicroSplatKeywords keywords);

      /// <summary>
      /// Draw the editor for the shaders options
      /// </summary>
      /// <param name="shaderGUI">Shader GU.</param>
      /// <param name="mat">Mat.</param>
      /// <param name="materialEditor">Material editor.</param>
      /// <param name="props">Properties.</param>
      public abstract void DrawShaderGUI(MicroSplatShaderGUI shaderGUI, MicroSplatKeywords keywords, Material mat, MaterialEditor materialEditor, MaterialProperty[] props);


      /// <summary>
      /// Got per texture properties? Draw the GUI for them here..
      /// </summary>
      /// <param name="index">Index.</param>
      /// <param name="shaderGUI">Shader GU.</param>
      /// <param name="mat">Mat.</param>
      /// <param name="materialEditor">Material editor.</param>
      /// <param name="props">Properties.</param>
      public virtual void DrawPerTextureGUI(int index, MicroSplatKeywords keywords, Material mat, MicroSplatPropData propData)
      {
      }

      /// <summary>
      /// Unpack your keywords from the material
      /// </summary>
      /// <param name="keywords">Keywords.</param>
      public abstract void Unpack(string[] keywords);

      /// <summary>
      /// pack keywords to a string[]
      /// </summary>
      public abstract string[] Pack();

      /// <summary>
      /// Init yourself
      /// </summary>
      /// <param name="paths">Paths.</param>
      public abstract void InitCompiler(string[] paths);

      /// <summary>
      /// write property definitions to the shader
      /// </summary>
      /// <param name="features">Features.</param>
      /// <param name="sb">Sb.</param>
      public abstract void WriteProperties(string[] features, StringBuilder sb);


      /// <summary>
      /// HDRP/LWRP benifit from declaring variables in CBuffers for instancing
      /// </summary>
      /// <param name="features">Features.</param>
      /// <param name="sb">Sb.</param>
      public virtual void WritePerMaterialCBuffer(string[] features, StringBuilder sb) { }

      /// <summary>
      /// Write any functions which might be used by other modules, basically, a prepass so that something can rely on
      /// code included by another module
      /// </summary>
      /// <param name="sb">Sb.</param>
      public virtual void WriteSharedFunctions(string[] features, StringBuilder sb) { }

      /// <summary>
      /// Some things, like tessellation in LWRP, require being able to call the vertex function
      /// </summary>
      /// <param name="sb">Sb.</param>
      public virtual void WriteAfterVetrexFunctions(StringBuilder sb) { }

      /// <summary>
      /// Write the core functions you use to the shader
      /// </summary>
      /// <param name="sb">Sb.</param>
      public abstract void WriteFunctions(string[] features, StringBuilder sb);

      /// <summary>
      /// Compute rough cost parameters for your section of the shader
      /// </summary>
      /// <param name="features">List of material features.<param> 
      /// <param name="arraySampleCount">Array sample count.</param>
      /// <param name="textureSampleCount">Texture sample count.</param>
      /// <param name="maxSamples">Max samples.</param>
      /// <param name="tessellationSamples">Tessellation samples.</param>
      /// <param name="depTexReadLevel">Dep tex read level.</param>
      public abstract void ComputeSampleCounts(string[] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, 
                                               ref int tessellationSamples, ref int depTexReadLevel);


      public void Pack(MicroSplatKeywords keywords)
      {
         var pck = Pack();
         for (int i = 0; i < pck.Length; ++i)
         {
            keywords.EnableKeyword(pck[i]);
         }
      }


      public enum Channel
      {
         R = 0,
         G,
         B,
         A
      }

      static bool drawPertexToggle = true;
      static protected int noPerTexToggleWidth = 20;

      static bool PerTexToggle(MicroSplatKeywords keywords, string keyword)
      {
         if (drawPertexToggle)
         {
            bool enabled = keywords.IsKeywordEnabled(keyword);
            bool newEnabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(20));
            if (enabled != newEnabled)
            {
               if (newEnabled)
                  keywords.EnableKeyword(keyword);
               else
                  keywords.DisableKeyword(keyword);
            }
            return newEnabled;
         }
         else
         {
            EditorGUILayout.LabelField("", GUILayout.Width(noPerTexToggleWidth));
            drawPertexToggle = true;
            return keywords.IsKeywordEnabled(keyword);
         }
      }

      static protected void InitPropData(int pixel, MicroSplatPropData propData, Color defaultValues)
      {
         if (propData == null)
         {
            return;
         }
         // we reserve the last row of potential values as an initialization bit. 
         if (propData.GetValue(pixel, 15) == new Color(0,0,0,0))
         {
            for (int i = 0; i < propData.maxTextures; ++i)
            {
               propData.SetValue(i, pixel, defaultValues);
            }
            propData.SetValue(pixel, 15, Color.white);
         }
      }
         
      static protected bool DrawPerTexFloatSlider(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, Channel channel, 
         GUIContent label, float min = 0, float max = 0, bool showHeader = true)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = keywords.IsKeywordEnabled (keyword);
         if (showHeader)
         {
            enabled = PerTexToggle (keywords, keyword);
            GUI.enabled = enabled;
         }
         else
         {
            EditorGUILayout.LabelField ("", GUILayout.Width (20));
            GUI.enabled = enabled;
         }

         Color c = propData.GetValue(curIdx, pixel);
         float v = c[(int)channel];
         float nv = v;
         if (min != max)
         {
            nv = EditorGUILayout.Slider(label, v, min, max);
         }
         else
         {
            nv = EditorGUILayout.FloatField(label, v);
         }
         if (nv != v)
         {
            c[(int)channel] = nv;
            propData.SetValue(curIdx, pixel, c);

         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < propData.maxTextures; ++i)
            {
               propData.SetValue(i, pixel, (int)channel, nv);
            }
         }

         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }



      public enum V2Cannel
      {
         RG = 0,
         BA
      }

      static protected bool DrawPerTexVector2(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, V2Cannel channel, 
         GUIContent label)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(keywords, keyword);
         GUI.enabled = enabled;

         Color c = propData.GetValue(curIdx, pixel);
         Vector2 v2 = new Vector2(c.r, c.g);
         if (channel == V2Cannel.BA)
         {
            v2.x = c.b;
            v2.y = c.a;
         }
         Vector2 nv = v2;

         nv = EditorGUILayout.Vector2Field(label, v2);

         if (nv != v2)
         {
            if (channel == V2Cannel.RG)
            {
               c.r = nv.x;
               c.g = nv.y;
            }
            else
            {
               c.b = nv.x;
               c.a = nv.y;
            }
            propData.SetValue(curIdx, pixel, c);
         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < propData.maxTextures; ++i)
            {
               // don't erase other pixels..
               var fv = propData.GetValue(i, pixel);
               if (channel == V2Cannel.RG)
               {
                  c.r = nv.x;
                  c.g = nv.y;
               }
               else
               {
                  c.b = nv.x;
                  c.a = nv.y;
               }
               propData.SetValue(i, pixel, fv);
            }
         }
         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }


      static protected bool DrawPerTexVector3 (int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData,
         GUIContent label)
      {
         EditorGUILayout.BeginHorizontal ();
         bool enabled = PerTexToggle (keywords, keyword);
         GUI.enabled = enabled;

         Color c = propData.GetValue (curIdx, pixel);
         Vector3 v = new Vector3 (c.r, c.g, c.b);
         
         Vector3 nv = EditorGUILayout.Vector2Field (label, v);

         if (nv != v)
         {
            c.r = nv.x;
            c.g = nv.y;
            c.b = nv.z;
            propData.SetValue (curIdx, pixel, c);
         }

         if (GUILayout.Button ("All", GUILayout.Width (40)))
         {
            for (int i = 0; i < propData.maxTextures; ++i) {
               // don't erase other pixels..
               var fv = propData.GetValue (i, pixel);
               c.r = nv.x;
               c.g = nv.y;
               c.b = nv.z;
               propData.SetValue (i, pixel, fv);
            }
         }
         GUI.enabled = true;
         EditorGUILayout.EndHorizontal ();

         return enabled;
      }


      static protected bool DrawPerTexVector2Vector2(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData,
         GUIContent label, GUIContent label2)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(keywords, keyword);
         GUI.enabled = enabled;

         Color c = propData.GetValue(curIdx, pixel);
         Vector2 v1 = new Vector2(c.r, c.g);
         Vector2 v2 = new Vector2(c.b, c.a);
         Vector2 nv1 = v1;
         Vector2 nv2 = v2;
         EditorGUILayout.BeginVertical();
         nv1 = EditorGUILayout.Vector2Field(label, v1);
         nv2 = EditorGUILayout.Vector2Field(label2, v2);
         EditorGUILayout.EndVertical();

         if (nv1 != v1 || nv2 != v2)
         {
            c.r = nv1.x;
            c.g = nv1.y;
            c.b = nv2.x;
            c.a = nv2.y;
            propData.SetValue(curIdx, pixel, c);
         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            c.r = nv1.x;
            c.g = nv1.y;
            c.b = nv2.x;
            c.a = nv2.y;
            for (int i = 0; i < propData.maxTextures; ++i)
            {
               propData.SetValue(i, pixel, c);
            }
         }
         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }

      protected bool DrawPerTexColor(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, 
         GUIContent label, bool hasAlpha)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(keywords, keyword);
         GUI.enabled = enabled;
         Color c = propData.GetValue(curIdx, pixel);
         Color nv = EditorGUILayout.ColorField(label, c);
         if (nv != c)
         {
            if (!hasAlpha)
            {
               nv.a = c.a;
            }
            propData.SetValue(curIdx, pixel, nv);

         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < propData.maxTextures; ++i)
            {
               if (!hasAlpha)
               {
                  nv.a = propData.GetValue(i, pixel).a;
               }
               propData.SetValue(i, pixel, nv);
            }
         }

         GUI.enabled = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }

      static protected bool DrawPerTexPopUp(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, Channel channel,
         GUIContent label, GUIContent[] options)
      {
         EditorGUILayout.BeginHorizontal();
         bool enabled = PerTexToggle(keywords, keyword);
         GUI.enabled = enabled;
         Color c = propData.GetValue(curIdx, pixel);
         float v = c[(int)channel];

         EditorGUI.BeginChangeCheck ();
         int selected = EditorGUILayout.Popup(label, (int)v, options);
         if (EditorGUI.EndChangeCheck())
         {
            c [(int)channel] = selected;
            propData.SetValue (curIdx, pixel, c);
         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < propData.maxTextures; ++i)
            {
               Color nv = propData.GetValue(i, pixel);
               nv[(int)channel] = selected;
               propData.SetValue(i, pixel, nv);
            }
         }

         GUI.enabled = true;
         drawPertexToggle = true;
         EditorGUILayout.EndHorizontal();

         return enabled;
      }

      static protected void DrawPerTexPopUpNoToggle (int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, Channel channel,
         GUIContent label, GUIContent [] options)
      {
         drawPertexToggle = false;
         DrawPerTexPopUp (curIdx, pixel, keyword, keywords, propData, channel, label, options);
      }



      static protected void DrawPerTexVector2NoToggle(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, V2Cannel channel,
        GUIContent label)
      {
         drawPertexToggle = false;
         DrawPerTexVector2(curIdx, pixel, keyword, keywords, propData, channel, label);
      }

      static protected void DrawPerTexVector2Vector2NoToggle(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData,
         GUIContent label, GUIContent label2)
      {
         drawPertexToggle = false;
         DrawPerTexVector2Vector2(curIdx, pixel, keyword, keywords, propData, label, label2);
      }

      static protected void DrawPerTexFloatSliderNoToggle(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, Channel channel,
         GUIContent label, float min = 0, float max = 0)
      {
         drawPertexToggle = false;
         DrawPerTexFloatSlider(curIdx, pixel, keyword, keywords, propData, channel, label, min, max);
      }

      static protected void DrawPerTexColorNoToggle(int curIdx, int pixel, MicroSplatPropData propData, GUIContent label)
      {
         EditorGUILayout.BeginHorizontal();
         EditorGUILayout.LabelField("", GUILayout.Width(20));
         Color c = propData.GetValue(curIdx, pixel);
         Color nv = EditorGUILayout.ColorField(label, c);
         if (nv != c)
         {
            propData.SetValue(curIdx, pixel, nv);
         }

         if (GUILayout.Button("All", GUILayout.Width(40)))
         {
            for (int i = 0; i < propData.maxTextures; ++i)
            {
               propData.SetValue(i, pixel, nv);
            }
         }

         EditorGUILayout.EndHorizontal();
         drawPertexToggle = true;
      }

      static protected void DrawPerTexPopUpNoToggle(int curIdx, int pixel, string keyword, MicroSplatKeywords keywords, MicroSplatPropData propData, Channel channel,
         GUIContent label, GUIContent[] options, float[] values)
      {
         drawPertexToggle = false;
         DrawPerTexPopUp(curIdx, pixel, keyword, keywords, propData, channel, label, options);
      }


      GUIStyle globalButtonPressedStyle = null;
      static GUIContent globalButton = new GUIContent("G", "Make property driven by a global variable. Used to integrate with external weathering systems");
      protected bool DrawGlobalToggle(string keyword, MicroSplatKeywords keywords)
      {
         bool b = keywords.IsKeywordEnabled(keyword);
         
         if (globalButtonPressedStyle == null)
         {
            globalButtonPressedStyle = new GUIStyle(GUI.skin.label);
            globalButtonPressedStyle.normal.background = new Texture2D(1, 1);
            globalButtonPressedStyle.normal.background.SetPixel(0, 0, Color.yellow);
            globalButtonPressedStyle.normal.background.Apply();
            globalButtonPressedStyle.normal.textColor = Color.black;
         }

         bool pressed = (GUILayout.Button(globalButton, b ? globalButtonPressedStyle : GUI.skin.label, GUILayout.Width(14)));

         if (keywords.IsKeywordEnabled ("_ISOBJECTSHADER"))
            return b;

         if (pressed)
         {
            if (b)
            {
               keywords.DisableKeyword(keyword); 
            }
            else
            {
               keywords.EnableKeyword(keyword);
            }
            b = !b;
            EditorUtility.SetDirty(keywords);
         }
         return b;
      }
   }
}