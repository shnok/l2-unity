using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lux_SRP_GrassDisplacement
{
    public class DebugGrassDisplacementTex : MonoBehaviour
    {
        
        [System.Serializable]
        public enum DebugSize {
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024
        }

        public bool m_EnableDebug = true;
        public DebugSize currentDebugSize = DebugSize._256;

        #if UNITY_EDITOR
                void OnDrawGizmos() {
                    if (m_EnableDebug) {
                    	var GrassDisplacementTex = Shader.GetGlobalTexture("_Lux_DisplacementRT");
                        if(GrassDisplacementTex != null) {
                            GL.PushMatrix();
                            var size = (int)currentDebugSize;
                            GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
                            Graphics.DrawTexture(new Rect(0, 0, size, size), Texture2D.normalTexture);
                            Graphics.DrawTexture(new Rect(0, 0, size, size), GrassDisplacementTex);
                            GL.PopMatrix();
                        }
                    }
                }
        #endif
    }
}