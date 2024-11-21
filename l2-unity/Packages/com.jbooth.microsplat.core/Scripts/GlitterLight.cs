//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JBooth.MicroSplat
{
   [ExecuteInEditMode]
   [RequireComponent(typeof(Light))]
   public class GlitterLight : MonoBehaviour
   {
      Light lght = null;

#if UNITY_EDITOR
      void OnEnable()
      {
         UnityEditor.EditorApplication.update += Update;
         lght = GetComponent<Light> ();
      }

      void OnDisable()
      {
         UnityEditor.EditorApplication.update -= Update;
         lght = GetComponent<Light> ();
      }

#else
      void OnEnable()
      {
         lght = GetComponent<Light> ();
      }

      void OnDisable()
      {
         lght = GetComponent<Light> ();
      }
#endif

      void Update ()
      {
         Shader.SetGlobalVector("_gGlitterLightDir", -this.transform.forward);
         Shader.SetGlobalVector("_gGlitterLightWorldPos", this.transform.position);
         if (lght != null)
         {
            Shader.SetGlobalColor ("_gGlitterLightColor", lght.color);
         }
      }
   }

}
