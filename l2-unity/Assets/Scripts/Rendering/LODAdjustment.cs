using System;
using UnityEngine;

namespace LlamAcademy.LightLOD
{
    [Serializable]
    public class LODAdjustment
    {
        public float MinSquareDistance;
        public float MaxSquareDistance;
        public ShadowResolution ShadowResolution;
        public LightShadows LightShadows;
        public Color DebugColor;
    }
}