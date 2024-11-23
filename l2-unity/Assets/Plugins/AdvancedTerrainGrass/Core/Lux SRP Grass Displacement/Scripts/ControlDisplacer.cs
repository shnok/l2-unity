using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lux_SRP_GrassDisplacement {

    [ExecuteInEditMode]
    public class ControlDisplacer : MonoBehaviour {
        
    	public float maxDistance = 1.0f;
        public float fallOff = 2.0f;
        
        [Layer]
        public int layerMask = 0;

        [Space(5)]
        public bool DebugRay = true;

    	private Transform trans;
    	private Renderer rend;
    	private MaterialPropertyBlock mpb;

        private RaycastHit hit;
        private float alpha;

        void OnEnable() {
            trans = this.GetComponent<Transform>();
            rend = this.GetComponent<Renderer>();
            mpb = new MaterialPropertyBlock();
            mpb.Clear();
            rend.SetPropertyBlock(mpb);
        }

        void OnDisable() {
            mpb.Clear();
            rend.SetPropertyBlock(null);
        }

        void Update() {
            var mask = 1 << layerMask;
            if (Physics.Raycast(trans.position, Vector3.down, out hit, maxDistance, mask )) {
                #if UNITY_EDITOR
                    if(DebugRay) {
                        Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.green);
                    }
                #endif
                alpha = (float)(1.0 - Math.Pow(hit.distance / maxDistance, fallOff));
                mpb.SetFloat("_Alpha", alpha);
                rend.SetPropertyBlock(mpb);
            }
            else {
                #if UNITY_EDITOR
                    if(DebugRay) {
                        Debug.DrawRay(transform.position, Vector3.down * maxDistance, Color.red);
                    }
                #endif
                if (alpha != 0.0f) {
                    alpha = 0.0f;
                    mpb.SetFloat("_Alpha", 0.0f);
                    rend.SetPropertyBlock(mpb);
                }
            }
        }
    }

}