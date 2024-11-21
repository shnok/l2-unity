//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
#if __MICROSPLAT__ && (__MICROSPLAT_STREAMS__ || __MICROSPLAT_GLOBALTEXTURE__ || __MICROSPLAT_SNOW__ || __MICROSPLAT_SCATTER__ || __MICROSPLAT_PROCTEX__ || __MICROSPLAT_MEGA__)
   public class TerrainPaintJob : ScriptableObject
   {
      public Terrain terrain;
      public Texture2D streamTex;
      public Texture2D tintTex;
      public Texture2D snowTex;
      public Texture2D scatterTex;
      public Texture2D biomeMask;
      public Texture2D biomeMask2;
      public Texture2D megaSplat;
      public Collider collider;

      public byte [] streamBuffer;
      public byte [] tintBuffer;
      public byte [] snowBuffer;
      public byte [] scatterBuffer;

      public byte [] biomeMaskBuffer;
      public byte [] biomeMaskBuffer2;
      public byte [] megaBuffer;

      public void RegisterUndo()
      {
         bool undo = false;
         if (streamTex != null)
         {
            streamBuffer = streamTex.GetRawTextureData();
            undo = true;
         }
         if (tintTex != null)
         {
            tintBuffer = tintTex.GetRawTextureData ();
            undo = true;
         }
         if (snowTex != null)
         {
            snowBuffer = snowTex.GetRawTextureData ();
            undo = true;
         }
         if (scatterTex != null)
         {
            scatterBuffer = scatterTex.GetRawTextureData ();
            undo = true;
         }
         if (biomeMask != null)
         {
            biomeMaskBuffer = biomeMask.GetRawTextureData ();
            undo = true;
         }
         if (biomeMask2 != null)
         {
            biomeMaskBuffer2 = biomeMask2.GetRawTextureData ();
            undo = true;
         }
         if (megaSplat != null)
         {
            megaBuffer = megaSplat.GetRawTextureData();
            undo = true;
         }
         if (undo)
         {
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Terrain Edit");
         }
      }

      public void RestoreUndo()
      {
         if (streamBuffer != null && streamBuffer.Length > 0)
         {
            streamTex.LoadRawTextureData(streamBuffer);
            streamTex.Apply();
         }
         if (tintTex != null && tintBuffer.Length > 0)
         {
            tintTex.LoadRawTextureData (tintBuffer);
            tintTex.Apply ();
         }
         if (snowBuffer != null && snowBuffer.Length > 0)
         {
            snowTex.LoadRawTextureData (streamBuffer);
            snowTex.Apply ();
         }
         if (scatterBuffer != null && scatterBuffer.Length > 0)
         {
            scatterTex.LoadRawTextureData (scatterBuffer);
            scatterTex.Apply ();
         }

         if (biomeMask != null && biomeMaskBuffer.Length > 0)
         {
            biomeMask.LoadRawTextureData (biomeMaskBuffer);
            biomeMask.Apply ();
         }
         if (biomeMask2 != null && biomeMaskBuffer2.Length > 0)
         {
            biomeMask.LoadRawTextureData (biomeMaskBuffer2);
            biomeMask.Apply ();
         }
         if (megaSplat != null && megaBuffer.Length > 0)
         {
            megaSplat.LoadRawTextureData(megaBuffer);
            megaSplat.Apply();
         }
      }
   }
   #endif
}
