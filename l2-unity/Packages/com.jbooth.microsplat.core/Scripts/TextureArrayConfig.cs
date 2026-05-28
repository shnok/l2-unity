//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////


using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroSplat
{
   [CreateAssetMenu(menuName = "MicroSplat/Texture Array Config", order = 1)]
   [ExecuteInEditMode]
   public class TextureArrayConfig : ScriptableObject 
   {
      public enum AllTextureChannel
      {
         R = 0,
         G,
         B,
         A,
         Custom
      }

      public enum TextureChannel
      {
         R = 0,
         G,
         B,
         A
      }

      public enum Compression
      {
         AutomaticCompressed,
         ForceDXT,
         ForceBC7,
         ForcePVR,
         ForceETC2,
         ForceASTC,
         ForceCrunch,
         Uncompressed
      }

      public enum CompressionQuality
      {
         High,
         Medium,
         Low
      }

      public enum TextureSize
      {
         k4096 = 4096,
         k2048 = 2048,
         k1024 = 1024,
         k512 = 512,
         k256 = 256,
         k128 = 128,
         k64 = 64,
         k32 = 32,
      }

#if __MICROSPLAT_SLOPETEXTURE__
      public int maxSyncCount = 32;
#endif

      [System.Serializable]
      public class TextureArraySettings
      {
         public TextureArraySettings(TextureSize s, Compression c, FilterMode f, int a = 1)
         {
            textureSize = s;
            compression = c;
            compressionQuality = CompressionQuality.Medium;
            filterMode = f;
            Aniso = a;
         }

         public TextureSize textureSize;
         public Compression compression;
         public CompressionQuality compressionQuality;
         public FilterMode filterMode;
         [Range(0, 16)]
         public int Aniso = 1;
      }

      public enum PBRWorkflow
      {
         Metallic,
         Specular
      }

      public enum PackingMode
      {
         Fastest,
         Quality,
      }

      public enum SourceTextureSize
      {
         Unchanged,
         k32 = 32,
         k256 = 256,
      }

      // for the interface
      public enum TextureMode
      {
         Basic = 0,
         PBR = 1,
         #if __MICROSPLAT_SCATTER__
         Scatter = 2,
         #endif
         #if __MICROSPLAT_DECAL__
         Decal = 3,
         DecalSplatMap = 4,
         #endif
         #if __MICROSPLAT_STARREACH__
         StarReach,
         #endif
      }

      public enum ClusterMode
      {
         None,
         TwoVariations,
         ThreeVariations
      }

      public bool diffuseIsLinear;

      public bool IsScatter()
      {
      #if __MICROSPLAT_SCATTER__
         return textureMode == TextureMode.Scatter;
      #else
         return false;
      #endif
      }

      public bool IsStarReach()
      {
#if __MICROSPLAT_STARREACH__
         return textureMode == TextureMode.StarReach;
#else
         return false;
#endif
      }

      public bool IsDecal ()
      {
#if __MICROSPLAT_DECAL__
         return textureMode == TextureMode.Decal;
#else
         return false;
#endif
      }

      public bool IsDecalSplat ()
      {
#if __MICROSPLAT_DECAL__
         return textureMode == TextureMode.DecalSplatMap;
#else
         return false;
#endif
      }

      [HideInInspector]
      public bool antiTileArray = false;

      [HideInInspector]
      public bool emisMetalArray = false;

      public bool traxArray = false;

      [HideInInspector]
      public TextureMode textureMode = TextureMode.PBR;   

      [HideInInspector]
      public ClusterMode clusterMode = ClusterMode.None;

      [HideInInspector]
      public PackingMode packingMode = PackingMode.Fastest;

      [HideInInspector]
      public PBRWorkflow pbrWorkflow = PBRWorkflow.Metallic;

      [HideInInspector]
      public int hash;

#if UNITY_EDITOR
      public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
      {
         List<T> assets = new List<T>();
         string[] guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).ToString().Replace("UnityEngine.", "")));
         for( int i = 0; i < guids.Length; i++ )
         {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath( guids[i] );
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>( assetPath );
            if( asset != null )
            {
               assets.Add(asset);
            }
         }
         return assets;
      }

      public static TextureArrayConfig FindConfig(Texture2DArray diffuse)
      {
         var path = UnityEditor.AssetDatabase.GetAssetPath (diffuse);
         path = path.Replace ("_diff_tarray", "");
         TextureArrayConfig cfg = UnityEditor.AssetDatabase.LoadAssetAtPath<TextureArrayConfig> (path);
         return cfg;
      }
#endif

      [HideInInspector]
      public Texture2DArray splatArray;

      [HideInInspector]
      public Texture2DArray diffuseArray;
      [HideInInspector]
      public Texture2DArray normalSAOArray;
      [HideInInspector]
      public Texture2DArray smoothAOArray;
      [HideInInspector]
      public Texture2DArray specularArray;

      [HideInInspector]
      public Texture2DArray diffuseArray2;
      [HideInInspector]
      public Texture2DArray normalSAOArray2;
      [HideInInspector]
      public Texture2DArray smoothAOArray2;
      [HideInInspector]
      public Texture2DArray specularArray2;

      [HideInInspector]
      public Texture2DArray diffuseArray3;
      [HideInInspector]
      public Texture2DArray normalSAOArray3;
      [HideInInspector]
      public Texture2DArray smoothAOArray3;
      [HideInInspector]
      public Texture2DArray specularArray3;

      [HideInInspector]
      public Texture2DArray emisArray;
      [HideInInspector]
      public Texture2DArray emisArray2;
      [HideInInspector]
      public Texture2DArray emisArray3;

      [System.Serializable]
      public class TextureArrayGroup
      {
         public TextureArraySettings diffuseSettings = new TextureArraySettings(TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
         public TextureArraySettings normalSettings = new TextureArraySettings(TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Trilinear, 1);
         public TextureArraySettings smoothSettings = new TextureArraySettings(TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
         public TextureArraySettings antiTileSettings = new TextureArraySettings(TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
         public TextureArraySettings emissiveSettings = new TextureArraySettings(TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
         public TextureArraySettings specularSettings = new TextureArraySettings (TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
         public TextureArraySettings traxDiffuseSettings = new TextureArraySettings (TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
         public TextureArraySettings traxNormalSettings = new TextureArraySettings (TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
         public TextureArraySettings decalSplatSettings = new TextureArraySettings (TextureSize.k1024, Compression.AutomaticCompressed, FilterMode.Bilinear, 1);
      }


      [System.Serializable]
      public class PlatformTextureOverride
      {
#if UNITY_EDITOR
         public UnityEditor.BuildTarget platform = UnityEditor.BuildTarget.StandaloneWindows;
#endif
         public TextureArrayGroup settings = new TextureArrayGroup(); 
      }


      // default settings, and overrides
      public TextureArrayGroup defaultTextureSettings = new TextureArrayGroup();
      public List<PlatformTextureOverride> platformOverrides = new List<PlatformTextureOverride>();

      public SourceTextureSize sourceTextureSize = SourceTextureSize.Unchanged;

      [HideInInspector]
      public AllTextureChannel allTextureChannelHeight = AllTextureChannel.G;
      [HideInInspector]
      public AllTextureChannel allTextureChannelSmoothness = AllTextureChannel.G;
      [HideInInspector]
      public AllTextureChannel allTextureChannelAO = AllTextureChannel.G;

      [System.Serializable]
      public class TextureEntry
      {
         public TerrainLayer terrainLayer;
         public Texture2D diffuse;
         public Texture2D height;
         public TextureChannel heightChannel = TextureChannel.G;
         public Texture2D normal;
         public Texture2D smoothness;
         public TextureChannel smoothnessChannel = TextureChannel.G;
         public bool isRoughness;
         public Texture2D ao;
         public TextureChannel aoChannel = TextureChannel.G;       

         public Texture2D emis;
         public Texture2D metal;
         public TextureChannel metalChannel = TextureChannel.G;

         public Texture2D specular;

         // anti tile
         public Texture2D noiseNormal;
         public Texture2D detailNoise;
         public TextureChannel detailChannel = TextureChannel.G;      
         public Texture2D distanceNoise;
         public TextureChannel distanceChannel = TextureChannel.G;

         public Texture2D traxDiffuse;
         public Texture2D traxHeight;
         public TextureChannel traxHeightChannel = TextureChannel.G;
         public Texture2D traxNormal;
         public Texture2D traxSmoothness;
         public TextureChannel traxSmoothnessChannel = TextureChannel.G;
         public bool traxIsRoughness;
         public Texture2D traxAO;
         public TextureChannel traxAOChannel = TextureChannel.G;

         public Texture2D splat;

         public void Reset()
         {
            diffuse = null;
            height = null;
            normal = null;
            smoothness = null;
            specular = null;
            ao = null;
            isRoughness = false;
            detailNoise = null;
            distanceNoise = null;
            metal = null;
            emis = null;
            heightChannel = TextureChannel.G;
            smoothnessChannel = TextureChannel.G;
            aoChannel = TextureChannel.G;
            distanceChannel = TextureChannel.G;
            detailChannel = TextureChannel.G;

            traxDiffuse = null;
            traxNormal = null;
            traxHeight = null;
            traxSmoothness = null;
            traxAO = null;
            traxHeightChannel = TextureChannel.G;
            traxSmoothnessChannel = TextureChannel.G;
            traxAOChannel = TextureChannel.G;

            splat = null;
         }

         public bool HasTextures(PBRWorkflow wf)
         {
            if (wf == PBRWorkflow.Specular)
            {
               return (
#if __MICROSPLAT_DECAL__
               splat != null ||
#endif
               diffuse != null ||
               height != null ||
               normal != null ||
               smoothness != null ||
               specular != null ||
               ao != null);
            }
            else
            {
               return (
#if __MICROSPLAT_DECAL__
               splat != null ||
#endif
               diffuse != null ||
               height != null ||
               normal != null ||
               smoothness != null ||
               metal != null ||
               ao != null);
            }

         }
      }

      [HideInInspector]
      public List<TextureEntry> sourceTextures = new List<TextureEntry>();
      [HideInInspector]
      public List<TextureEntry> sourceTextures2 = new List<TextureEntry>();
      [HideInInspector]
      public List<TextureEntry> sourceTextures3 = new List<TextureEntry>();

      public bool HasTerrainLayer(TerrainLayer l)
      {
         foreach (var s in sourceTextures)
         {
            if (s.diffuse == l.diffuseTexture && s.normal == l.normalMapTexture && s.diffuse != null)
               return true;
         }
         return false;
      }
      public void AddTerrainLayer(TerrainLayer l)
      {
            TextureEntry e = new TextureEntry();
            e.terrainLayer = l;
            e.diffuse = l.diffuseTexture;
            e.normal = l.normalMapTexture;
            e.ao = l.maskMapTexture;
            e.smoothness = l.maskMapTexture;
            e.height = l.maskMapTexture;
            e.aoChannel = TextureChannel.G;
            e.smoothnessChannel = TextureChannel.A;
            e.heightChannel = TextureChannel.B;
            sourceTextures.Add(e);
            if (clusterMode == ClusterMode.TwoVariations)
            {
                sourceTextures2.Add(e);
            }
            if (clusterMode == ClusterMode.ThreeVariations)
            {
                sourceTextures3.Add(e);
            }
        }
   }
}
