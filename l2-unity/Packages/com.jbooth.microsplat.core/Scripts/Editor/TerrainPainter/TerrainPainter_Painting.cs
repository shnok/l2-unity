//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace JBooth.MicroSplat
{
#if __MICROSPLAT__ && (__MICROSPLAT_STREAMS__ || __MICROSPLAT_GLOBALTEXTURE__ || __MICROSPLAT_SNOW__ || __MICROSPLAT_SCATTER__ || __MICROSPLAT_PROCTEX__ || __MICROSPLAT_MEGA__)
   public partial class TerrainPainterWindow : EditorWindow 
   {
      double deltaTime = 0;
      double lastTime = 0;
      bool painting = false;

      public Vector3         oldpos = Vector3.zero;
      public float           brushSize = 1;
      public float           brushFlow = 8;
      public float           brushFalloff = 1; // linear
      public Color           paintColor = Color.grey;
      public float           paintValue = 1;
      public int biomeChannel = 0;
      [System.Serializable]
      public enum MegaLayerMode
      {
         Bottom,
         Top,
         Auto
      } 
      [System.Serializable]
      public enum MegaBrushMode
      {
         Single,
         Cluster,
         Blend
      }

      [System.Serializable]
      public class ClusterBrush
      {
         public List<int> textures = new List<int>();
         public float frequency = 1;
         public float offset;
      }

      public int megaIndex = 0;
      public MegaLayerMode megaLayerMode = MegaLayerMode.Auto;
      public MegaBrushMode megaBrushMode = MegaBrushMode.Single;
      public ClusterBrush clusterBrush = new ClusterBrush();

      public System.Action<TerrainPaintJob []> OnBeginStroke;
      public System.Action<TerrainPaintJob, bool> OnStokeModified;  // bool is true when doing a fill or other non-bounded opperation
      public System.Action OnEndStroke;
     

      public enum BrushVisualization
      {
         Sphere,
         Disk
      }
      public BrushVisualization brushVisualization = BrushVisualization.Sphere;

      public Vector2 lastMousePosition;
      void OnSceneGUI(SceneView sceneView)
      {
         deltaTime = EditorApplication.timeSinceStartup - lastTime;
         lastTime = EditorApplication.timeSinceStartup;

         if (terrains == null || terrains.Length == 0 && Selection.activeGameObject != null)
         {
            InitTerrains();
         }

         if (!enabled || terrains.Length == 0 || Selection.activeGameObject == null)
         {
            return;
         }

         RaycastHit hit;
         float distance = float.MaxValue;
         Vector3 mousePosition = Event.current.mousePosition;

         // So, in 5.4, Unity added this value, which is basically a scale to mouse coordinates for retna monitors.
         // Not all monitors, just some of them.
         // What I don't get is why the fuck they don't just pass me the correct fucking value instead. I spent hours
         // finding this, and even the paid Unity support my company pays many thousands of dollars for had no idea
         // after several weeks of back and forth. If your going to fake the coordinates for some reason, please do
         // it everywhere to not just randomly break things everywhere you don't multiply some new value in. 
         float mult = EditorGUIUtility.pixelsPerPoint;

         mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y * mult;
         mousePosition.x *= mult;
         Vector3 fakeMP = mousePosition;
         fakeMP.z = 20;
         Vector3 point = sceneView.camera.ScreenToWorldPoint(fakeMP);
         Vector3 normal = Vector3.forward;
         Ray ray = sceneView.camera.ScreenPointToRay(mousePosition);

         bool registerUndo = (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.alt == false);

         for (int i = 0; i < terrains.Length; ++i)
         {
            if (terrains[i] == null)
               continue;
            // Early out if we're not in the area..
            var cld = terrains[i].collider;
            Bounds b = cld.bounds;
            b.Expand(brushSize*2);
            if (!b.IntersectRay(ray))
            {
               continue;
            }

            if (registerUndo)
            {
               painting = true;
               for (int x = 0; x < jobEdits.Length; ++x)
               {
                  jobEdits[x] = false;
               }
               if (i == 0 && OnBeginStroke != null)
               {
                  OnBeginStroke(terrains);
               }
            }

            if (cld.Raycast(ray, out hit, float.MaxValue))
            {
               if (Event.current.shift == false) 
               {
                  if (hit.distance < distance) 
                  {
                     distance = hit.distance;
                     point = hit.point;
                     normal = hit.normal;
                  }
               } 
               else 
               {
                  point = oldpos;
               }
            } 
            else 
            {
               if (Event.current.shift == true) 
               {
                  point = oldpos;
               }
            }  
         }

         if (Event.current.type == EventType.MouseMove && Event.current.shift) 
         {
            brushSize += Event.current.delta.x * (float)deltaTime * 6.0f;
            brushFalloff -= Event.current.delta.y * (float)deltaTime * 48.0f;
         }

         if (Event.current.rawType == EventType.MouseUp)
         {
            EndStroke();
         }
         if (Event.current.type == EventType.MouseMove && Event.current.alt)
         {
            brushSize += Event.current.delta.y * (float)deltaTime;
         }
            

         if (brushVisualization == BrushVisualization.Sphere)
         {
            Handles.SphereHandleCap(0, point, Quaternion.identity, brushSize * 2, EventType.Repaint);
         }
         else
         {
            Handles.color = new Color(0.8f, 0, 0, 1.0f);
            float r = Mathf.Pow(0.5f, brushFalloff);
            Handles.DrawWireDisc(point, normal, brushSize * r);
            Handles.color = new Color(0.9f, 0, 0, 0.8f);
            Handles.DrawWireDisc(point, normal, brushSize);
         }
         // eat current event if mouse event and we're painting
         if (Event.current.isMouse && painting)
         {
            Event.current.Use();
         } 

         if (Event.current.type == EventType.Layout)
         {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
         }

         // only paint once per frame
         if (Event.current.type != EventType.Repaint)
         {
            return;
         }


         if (terrains.Length > 0 && painting)
         {
            for (int i = 0; i < terrains.Length; ++i)
            {
               Bounds b = terrains[i].collider.bounds;
               b.Expand(brushSize * 2);
               if (!b.IntersectRay(ray))
               {
                  continue;
               }
               if (jobEdits[i] == false)
               {
                  jobEdits[i] = true;
                  terrains[i].RegisterUndo();
               }
               PaintTerrain(terrains[i], point);
               if (OnStokeModified != null)
               {
                  OnStokeModified(terrains[i], false);
               }
            }
         }

         lastMousePosition = Event.current.mousePosition;
         // update views
         sceneView.Repaint();
         HandleUtility.Repaint();
      }


      void EndStroke()
      {
         painting = false;
         if (OnEndStroke != null)
         {
            OnEndStroke();
         }
      }

      public static Vector3 WorldToTerrain(Terrain ter, Vector3 point, Texture2D splatControl)
      {
         float x = (point.x / ter.terrainData.size.x) * splatControl.width;
         float z = (point.z / ter.terrainData.size.z) * splatControl.height;
         float y = ter.terrainData.GetHeight((int)x, (int)z);
         return new Vector3(x, y, z);
      }

      public static Vector3 TerrainToWorld(Terrain ter, int x, int y, Texture2D splatControl)
      {
         Vector3 wp = new Vector3(x, 0, y);
         wp.x *= ter.terrainData.size.x / (float)splatControl.width;
         wp.y = ter.terrainData.GetHeight(x, y);
         wp.z *= ter.terrainData.size.z / (float)splatControl.height;
         wp += ter.transform.position;
         return wp;

      }

      void MegaBrush(Texture2D tex, int x, int y, float finalStr)
      {
         Color data = tex.GetPixel(x, y);
         int index0 = Mathf.RoundToInt(data.r * 256);
         int index1 = Mathf.RoundToInt(data.g * 256);
         float weight = data.b;
         if (megaBrushMode != MegaBrushMode.Blend)
         {
            if (megaBrushMode == MegaBrushMode.Cluster)
            {
               float px = (float)x / (tex.width) * 128;
               float py = (float)y / (tex.height) * 128;

               float v = Mathf.PerlinNoise(px * clusterBrush.frequency + clusterBrush.offset, py * clusterBrush.frequency - clusterBrush.offset);
               v = Mathf.Clamp01(v);
               int idx = (int)(v * clusterBrush.textures.Count);
               if (idx >= clusterBrush.textures.Count)
               {
                  idx = clusterBrush.textures.Count - 1;
               }
               megaIndex = clusterBrush.textures[idx];
            }
            if (megaLayerMode == MegaLayerMode.Auto)
            {
               // replace lowest weight if we're new.
               if (megaIndex != index0 && megaIndex != index1)
               {
                  if (weight >= 0.5f)
                  {
                     index0 = megaIndex;
                  }
                  else
                  {
                     index1 = megaIndex;
                  }
               }

               if (megaIndex == index0)
               {
                  weight -= finalStr;
               }
               else
               {
                  weight += finalStr;
               }
            }
            else if (megaLayerMode == MegaLayerMode.Bottom)
            {
               index0 = megaIndex;
               weight -= finalStr;
            }
            else
            {
               index1 = megaIndex;
               weight += finalStr;
            }
         }
         else if (megaBrushMode == MegaBrushMode.Blend)
         {
            weight = Mathf.Lerp(weight, paintValue, finalStr);
         }

         weight = Mathf.Clamp01(weight);
         data.r = (float)index0 / 255.0f;
         data.g = (float)index1 / 255.0f;
         data.b = weight;
         tex.SetPixel(x, y, data);
      }

      void PaintTerrain(TerrainPaintJob tj, Vector3 worldPoint)
      {
         if (tj == null)
            return;

         // convert point into local space, so we don't have to convert every point
         var mtx = Matrix4x4.TRS(tj.terrain.transform.position, tj.terrain.transform.rotation, Vector3.one).inverse;
         Vector3 localPoint = mtx.MultiplyPoint3x4(worldPoint);

         float bz = brushSize;

         float pressure = Event.current.pressure > 0 ? Event.current.pressure : 1.0f;

         Texture2D tex = null;
         int channel = -1;
         GetTexAndChannel (tj, out tex, out channel);

         if (tex == null)
         {
            return;
         }

         Vector3 terPoint = WorldToTerrain(tj.terrain, localPoint, tex);

         if (terPoint.x >= 0 && terPoint.z >= 0 && terPoint.x < tex.width || terPoint.z < tex.height)
         {
            // scale brush into texture space
            Vector3 offsetPnt = localPoint - new Vector3(bz, 0, bz);
            Vector3 beginTerPnt = WorldToTerrain(tj.terrain, offsetPnt, tex);
            beginTerPnt.x = Mathf.Clamp(beginTerPnt.x, 0, tex.width);
            beginTerPnt.z = Mathf.Clamp(beginTerPnt.z, 0, tex.height);

            Vector3 offset = terPoint - beginTerPnt;
            int pbx = (int)beginTerPnt.x;
            int pby = (int)beginTerPnt.z;
            int pex = (int)(terPoint.x + offset.x * 2.0f);
            int pey = (int)(terPoint.z + offset.z * 2.0f);

            pex = Mathf.Clamp(pex, 0, tex.width);
            pey = Mathf.Clamp(pey, 0, tex.height);

            for (int x = pbx; x < pex; ++x)
            {
               for (int y = pby; y < pey; ++y)
               {
                  float h = tj.terrain.terrainData.GetHeight(x, y);
                  float d = Vector3.Distance(terPoint, new Vector3(x, h, y));
                  float str = 1.0f - d / bz;
                  str = Mathf.Pow(str, brushFalloff);
                  float finalStr = str * (float)deltaTime * brushFlow * pressure;
                  if (finalStr > 0)
                  {
                     Vector3 normal = tj.terrain.terrainData.GetInterpolatedNormal((float)x / tex.width, (float)y / tex.height);
                     float dt = Vector3.Dot(normal, Vector3.up);
                     dt = 1 - Mathf.Clamp01(dt);
                     bool filtered = dt < slopeRange.x || dt > slopeRange.y;
                     if (tab == Tab.Scatter)
                     {
                        filtered = false;
                     }

                     if (!filtered)
                     {
                        if (tab == Tab.TintMap)
                        {
                           Color c = tex.GetPixel (x, y);
                           c.r = Mathf.Lerp (c.r, paintColor.r, finalStr);
                           c.g = Mathf.Lerp (c.g, paintColor.g, finalStr);
                           c.b = Mathf.Lerp (c.b, paintColor.b, finalStr);
                           tex.SetPixel (x, y, c);
                        }
                        else if (tab == Tab.SnowMin)
                        {
                           Color c = tex.GetPixel (x, y);
                           c.g = Mathf.Lerp (c.g, paintValue, finalStr);
                           tex.SetPixel (x, y, c);
                        }
                        else if (tab == Tab.SnowMax)
                        {
                           Color c = tex.GetPixel (x, y);
                           c.r = Mathf.Lerp (c.r, paintValue, finalStr);
                           tex.SetPixel (x, y, c);
                        }

                        else if (tab == Tab.Wetness)
                        {
                           Color c = tex.GetPixel(x, y);
                           c.r = Mathf.Lerp(c.r, paintValue, finalStr);
                           tex.SetPixel(x, y, c);
                        }
                        else if (tab == Tab.Puddles)
                        {
                           Color c = tex.GetPixel(x, y);
                           c.g = Mathf.Lerp(c.g, paintValue, finalStr);
                           tex.SetPixel(x, y, c);
                        }
                        else if (tab == Tab.Streams)
                        {
                           Color c = tex.GetPixel(x, y);
                           c.b = Mathf.Lerp(c.b, paintValue, finalStr);
                           tex.SetPixel(x, y, c);
                        }
                        else if (tab == Tab.Lava)
                        {
                           Color c = tex.GetPixel(x, y);
                           c.a = Mathf.Lerp(c.a, paintValue, finalStr);
                           tex.SetPixel(x, y, c);
                        }
                        else if (tab == Tab.Scatter)
                        {
                           Color c = tex.GetPixel (x, y);
#if __MICROSPLAT_SCATTER__
                           if (scatterLayer == ScatterLayer.First)
                           {
                              c.r = (float)(scatterIndex + 1) / 64.0f;
                              c.g = Mathf.Lerp (c.g, paintValue, finalStr);
                              if (c.g <= 0)
                              {
                                 c.r = 0;
                              }
                           }
                           else
                           {
                              c.a = (float)(scatterIndex + 1) / 64.0f;
                              c.b = Mathf.Lerp (c.b, paintValue, finalStr);
                              if (c.b <= 0)
                              {
                                 c.a = 0;
                              }
                           }
#endif
                           tex.SetPixel (x, y, c);
                        }
                        else if (tab == Tab.Biome)
                        {
                           Color c = tex.GetPixel (x, y);
                           int ch = biomeChannel;
                           if (ch > 3)
                              ch -= 4;
                           c[ch] = Mathf.Lerp(c[ch], paintValue, finalStr);
                           tex.SetPixel (x, y, c);
                        }
                        else if (tab == Tab.Mega)
                        {
                           MegaBrush(tex, x, y, finalStr);
                        }
                     }
                  }
               }
            }
            tex.Apply();
         }
      }

   }
   #endif
}