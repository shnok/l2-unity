//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// layout
// 0 perTex UV Scale and Offset
// 1 PerTex Tint and interpolation contrast (a)
// 2 Normal Strength, Smoothness, AO, Metallic
// 3 Brightness, Contrast, porosity, foam
// 4 DetailNoiseStrength, distance Noise Strength, Distance Resample, DisplaceMip
// 5 geoTex, tint strength, normal strength, SmoothAOMetal strength
// 6 displace, bias, offset, global emis strength
// 7 Noise 0, Noise 1, Noise 2, Wind Particulate Strength
// 8 Snow (R), Glitter (G), GeoHeightFilter(B), (A) GeoHeightFilterStrength 
// 9 Triplanar, trplanar contrast, stochastic enabled, (A) Saturation
// 10 Texture Cluster Contrast, boost, Height Offset, Height Contrast
// 11 Advanced Detail UV Scale/Offset
// 12 Advanced Detail (G)Normal Blend, (B)Tex Overlay (A) MeshOverlayNormalBlend
// 13 Advanced Detail (R)Contrast, (G) AngleContrast, (B)HeightConttast, (A) Distance Resample UV scale
// 14 AntiTileArray (R)Normal Str, (G) Detail Strength, (B) Distance Strength (A) DisplaceShaping
// 15 Reserved for initialization marking
// 16 UV Rotation, Triplanar rot, triplanar rot, (A) GlobalSpecularStrength
// 17 FuzzyShading Core, Edge, Power, (A) MicroShadow
// 18 SSS Tint, (A) SSS thickness
// 19 (R) Curve Interpolator
// 20 Trax Dig Depth, opacity, normal strength (A) Open
// 21 Trax Tint (A) open
// 22 Noise Height Frequency, Amplitude, NoiseUV Frequency, Amplitude
// 23 ColorIntensity(R)
// 24 Scatter UV Scale (RG), blendMode (B), Alpha Mult (A)
// 25 Scatter Height Filter (RG), Scatter SlopeFilter (BA)
// 26 Scatter Distance Fade (R), RimLight Power (G)
// 27 Rim Light Color, intensity (A)
// 28 Outline Color and intensity
// 29 Slope Texture Angle (R), Contrast (G)
namespace JBooth.MicroSplat
{
   // because unity's HDR import pipeline is broke (assumes gamma, so breaks data in textures)
   public class MicroSplatPropData : ScriptableObject
   {

      public enum PerTexVector2
      {
         SplatUVScale = 0,
         SplatUVOffset = 2,
      }

      public enum PerTexColor
      {
         Tint = 1 * 4,
         SSSRTint = 18 * 4,
         TraxTint = 21 * 4,
         RimLightColor = 27*4,
         OutlineColor = 28*4,
      }

      public enum PerTexFloat
      {
         InterpolationContrast = 1 * 4 + 1,
         NormalStrength = 2 * 4,
         Smoothness = 2 * 4 + 1,
         AO = 2 * 4 + 2,
         Metallic = 2 * 4 + 3,

         Brightness = 3 * 4,
         Contrast = 3 * 4 + 1,
         Porosity = 3 * 4 + 2,
         Foam = 3 * 4 + 3,

         DetailNoiseStrength = 4 * 4,
         DistanceNoiseStrength = 4 * 4 + 1,
         DistanceResample = 4 * 4 + 2,
         DisplacementMip = 4 * 4 + 3,

         GeoTexStrength = 5 * 4,
         GeoTintStrength = 5 * 4 + 1,
         GeoNormalStrength = 5 * 4 + 2,
         GlobalSmoothMetalAOStength = 5 * 4 + 3,

         DisplacementStength = 6 * 4,
         DisplacementBias = 6 * 4 + 1,
         DisplacementOffset = 6 * 4 + 2,
         GlobalEmisStength = 6 * 4 + 3,

         NoiseNormal0Strength = 7 * 4,
         NoiseNormal1Strength = 7 * 4 + 1,
         NoiseNormal2Strength = 7 * 4 + 2,
         WindParticulateStrength = 7 * 4 + 3,

         SnowAmount = 8 * 4,
         GlitterAmount = 8 * 4 + 1,
         GeoHeightFilter = 8 * 4 + 2,
         GeoHeightFilterStrength = 8 * 4 + 3,

         TriplanarMode = 9 * 4,
         TriplanarContrast = 9 * 4 + 1,
         StochatsicEnabled = 9 * 4 + 2,
         Saturation = 9 * 4 + 3,

         TextureClusterContrast = 10 * 4,
         TextureClusterBoost = 10 * 4 + 1,
         HeightOffset = 10 * 4 + 2,
         HeightContrast = 10 * 4 + 3,

         AntiTileArrayNormalStrength = 14 * 4,
         AntiTileArrayDetailStrength = 14 * 4 + 1,
         AntiTileArrayDistanceStrength = 14 * 4 + 2,
         DisplaceShaping = 14 * 4 + 3,

         UVRotation = 16 * 4,
         TriplanarRotationX = 16 * 4 + 1,
         TriplanarRotationY = 16 * 4 + 2,

         FuzzyShadingCore = 17 * 4,
         FuzzyShadingEdge = 17 * 4 + 1,
         FuzzyShadingPower = 17 * 4 + 2,

         SSSThickness = 18 * 4 + 3,
         CurveInterpolator = 19 * 4,
         TraxDigDepth = 20 * 4,
         TraxOpacity = 20 * 4 + 1,
         TraxNormalStrength = 20 * 4 + 2,

         NoiseHeightFrequency = 22 * 4,
         NoiseHeightAmplitude = 22 * 4 + 1,
         NoiseUVFrequency = 22 * 4 + 2,
         NoiseUVAmplitude = 22 * 4 + 3,

         ColorIntensity = 23 * 4,
         ScatterBlendMode = 24 * 4 + 2,
         ScatterAlphaMult = 24 * 4 + 3,
         ScatterDistanceFade = 26 * 4,
         RimPower = 26 * 4 + 1,
         RimIntensity = 27 * 4 + 3,
         OutlineIntensity = 28 * 4 + 3,
         SlopeTextureAngle = 29 * 4,
         SlopeTextureContrast = 29 * 4 + 1,
        }


      public const int sMaxAttributes = 32;

      public Color[] values = new Color[32 * sMaxAttributes];


      public Texture2D propTex;

      [HideInInspector]
      public AnimationCurve geoCurve = AnimationCurve.Linear(0, 0.0f, 0, 0.0f);
      [HideInInspector]
      public Texture2D geoTex;

      [HideInInspector]
      public AnimationCurve geoSlopeFilter = AnimationCurve.Linear(0, 0.2f, 0.4f, 1.0f);
      [HideInInspector]
      public Texture2D geoSlopeTex;

      [HideInInspector]
      public AnimationCurve globalSlopeFilter = AnimationCurve.Linear(0, 0.2f, 0.4f, 1.0f);
      [HideInInspector]
      public Texture2D globalSlopeTex;

      void ClearPropTex()
      {

#if UNITY_EDITOR

         Object[] objs = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(UnityEditor.AssetDatabase.GetAssetPath(this));
         for (int i = 0; i < objs.Length; ++i)
         {
            if (objs[i] != null && objs[i].name == "PropTex")
            {
               DestroyImmediate(objs[i], true);
            }
         }
         propTex = null;
#else
         if (propTex != null)
         {
            DestroyImmediate(propTex);
         }
#endif

      }

      [HideInInspector]
      public int maxTextures = 32;  // can be set to 256 or 32

      public void RevisionData()
      {
         Debug.Assert(maxTextures == 32 || maxTextures == 256);
         // revision from 16 to 32 max textures
         if (values.Length == (16 * 16))
         {
            ClearPropTex();
            Color[] c = new Color[maxTextures * sMaxAttributes];
            for (int x = 0; x < 16; ++x)
            {
               for (int y = 0; y < 16; ++y)
               {
                  c[y * maxTextures + x] = values[y * sMaxAttributes + x];
               }
            }
            values = c;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
         }
         else if (values.Length == (32 * 16))
         {
            ClearPropTex();
            Color[] c = new Color[maxTextures * sMaxAttributes];
            for (int x = 0; x < 32; ++x)
            {
               for (int y = 0; y < 16; ++y)
               {
                  c[y * maxTextures + x] = values[y * sMaxAttributes + x];
               }
            }
            values = c;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
         }
         // handle switch from mega -> regular
         else if (values.Length == 32 * 256 && maxTextures == 32)
         {
            ClearPropTex();
            Color[] c = new Color[maxTextures * sMaxAttributes];
            for (int x = 0; x < 32; ++x)
            {
               for (int y = 0; y < 32; ++y)
               {
                  c[y * maxTextures + x] = values[y * sMaxAttributes + x];
               }
            }
            values = c;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
         }
         // switch from regular -> mega
         else if (values.Length == 32 * 32 && maxTextures == 256)
         {
            ClearPropTex();
            Color [] c = new Color [maxTextures * sMaxAttributes];
            for (int x = 0; x < 32; ++x)
            {
               for (int y = 0; y < 32; ++y)
               {
                  c [y * maxTextures + x] = values [y * sMaxAttributes + x];
               }
            }
            // copy first pixel into remaining.
            for (int x = 32; x < 256; ++x)
            {
               for (int y = 0; y < 32; ++y)
               {
                  c[y * maxTextures + x] = values[y * sMaxAttributes];
               }
            }
            values = c;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty (this);
#endif
         }
      }



      public Color GetValue(int x, int y)
      {
         RevisionData();
         return values[y * maxTextures + x];
      }

      public void SetValue(int x, int y, Color c)
      {
         RevisionData();
#if UNITY_EDITOR
         UnityEditor.Undo.RecordObject(this, "Changed Value");
#endif

         values[y * maxTextures + x] = c;

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(this);
#endif
      }

      public void SetValue(int x, int y, int channel, float value)
      {
         RevisionData();
#if UNITY_EDITOR
         UnityEditor.Undo.RecordObject(this, "Changed Value");
#endif
         int index = y * maxTextures + x;
         Color c = values[index];
         c[channel] = value;
         values[index] = c;

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(this);
#endif
      }

      public void SetValue(int x, int y, int channel, Vector2 value)
      {
         RevisionData();
#if UNITY_EDITOR
         UnityEditor.Undo.RecordObject(this, "Changed Value");
#endif
         int index = y * maxTextures + x;
         Color c = values[index];
         if (channel == 0)
         {
            c.r = value.x;
            c.g = value.y;
         }
         else
         {
            c.b = value.x;
            c.a = value.y;
         }

         values[index] = c;

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(this);
#endif
      }

      public void SetValue(int textureIndex, PerTexFloat channel, float value)
      {
         float e = ((float)channel) / 4.0f;
         int x = (int)e;
         int y = Mathf.RoundToInt((e - x) * 4);

         SetValue(textureIndex, x, y, value);
      }

      public void SetValue(int textureIndex, PerTexColor channel, Color value)
      {
         float e = ((float)channel) / 4.0f;
         int x = (int)e;

         SetValue(textureIndex, x, value);
      }

      public void SetValue(int textureIndex, PerTexVector2 channel, Vector2 value)
      {
         float e = ((float)channel) / 4.0f;
         int x = (int)e;
         int y = Mathf.RoundToInt((e - x) * 4);

         SetValue(textureIndex, x, y, value);
      }

      public Color[] GetAllValues(int textureIndex)
      {
         RevisionData();
         Color[] c = new Color[sMaxAttributes];
         for (int i = 0; i < sMaxAttributes; ++i)
         {
            c[i] = GetValue(textureIndex, i);
         }
         return c;
      }

      public void SetAllValues( int textureIndex, Color[] c)
      {
         RevisionData();
#if UNITY_EDITOR
         UnityEditor.Undo.RecordObject(this, "Changed Value");
#endif
         for (int i = 0; i < c.Length; ++i)
         {
            SetValue(textureIndex, i, c[i]);
         }

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(this);
#endif
      }

      public Texture2D GetTexture()
      {
         RevisionData();

         if (propTex == null)
         {
            if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
            {
               propTex = new Texture2D(maxTextures, sMaxAttributes, TextureFormat.RGBAFloat, false, true);
            }
            else if (SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf))
            {
               propTex = new Texture2D(maxTextures, sMaxAttributes, TextureFormat.RGBAHalf, false, true);
            }
            else
            {
               Debug.LogError("Could not create RGBAFloat or RGBAHalf format textures, per texture properties will be clamped to 0-1 range, which will break things");
               propTex = new Texture2D(maxTextures, sMaxAttributes, TextureFormat.RGBA32, false, true);
            }
            //tex.hideFlags = HideFlags.HideAndDontSave;
            propTex.wrapMode = TextureWrapMode.Clamp;
            propTex.filterMode = FilterMode.Point;
            propTex.hideFlags = HideFlags.None;
#if UNITY_EDITOR
            propTex.name = "PropTex";
            UnityEditor.AssetDatabase.AddObjectToAsset(propTex, this);
            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(propTex));
#endif
         }

         propTex.SetPixels(values);
         propTex.Apply();

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(propTex);
#endif

         return propTex;
      }

      public Texture2D GetGeoCurve()
      {
         if (geoTex == null)
         {
            geoTex = new Texture2D(256, 1, TextureFormat.RHalf, false, true);
#if UNITY_EDITOR
            geoTex.name = "geoTex";
            UnityEditor.AssetDatabase.AddObjectToAsset(geoTex, this);
            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(geoTex));
#endif

         }
         for (int i = 0; i < 256; ++i)
         {
            float v = geoCurve.Evaluate((float)i / 255.0f);
            geoTex.SetPixel(i, 0, new Color(v, v, v, v));
         }
         geoTex.Apply();

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(geoTex);
#endif

         return geoTex;
      }

      public Texture2D GetGeoSlopeFilter()
      {
         if (geoSlopeTex == null)
         {
            geoSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);

#if UNITY_EDITOR
            geoSlopeTex.name = "geoSlopeTex";
            UnityEditor.AssetDatabase.AddObjectToAsset(geoSlopeTex, this);
            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(geoSlopeTex));
#endif
         }
         for (int i = 0; i < 256; ++i)
         {
            float v = geoSlopeFilter.Evaluate((float)i / 255.0f);
            geoSlopeTex.SetPixel(i, 0, new Color(v, v, v, v));
         }
         geoSlopeTex.Apply();

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(geoSlopeTex);
#endif
         return geoSlopeTex;
      }

      public Texture2D GetGlobalSlopeFilter()
      {
         if (globalSlopeTex == null)
         {
            globalSlopeTex = new Texture2D(256, 1, TextureFormat.Alpha8, false, true);
#if UNITY_EDITOR
            globalSlopeTex.name = "globalSlopeTex";
            UnityEditor.AssetDatabase.AddObjectToAsset(globalSlopeTex, this);
            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(globalSlopeTex));
#endif
         }
         for (int i = 0; i < 256; ++i)
         {
            float v = globalSlopeFilter.Evaluate((float)i / 255.0f);
            globalSlopeTex.SetPixel(i, 0, new Color(v, v, v, v));
         }
         globalSlopeTex.Apply();

#if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(globalSlopeTex);
#endif

         return globalSlopeTex;
      }
   }
}
