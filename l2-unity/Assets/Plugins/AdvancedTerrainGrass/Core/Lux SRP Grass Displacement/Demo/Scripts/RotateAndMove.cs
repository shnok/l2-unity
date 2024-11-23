using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lux_SRP_GrassDisplacement
{
    public class RotateAndMove : MonoBehaviour {
        
    	public bool Rotate = true;
        public bool MoveUpDown = false;
        
        float posy;
        Transform trans;

        void OnEnable() {
           trans = this.GetComponent<Transform>();
           var pos = trans.position;
           posy = pos.y;
        }

        void Update() {
            if (Rotate) {
                trans.Rotate(0, 10.0f * Time.deltaTime, 0, Space.World);
            }
            if (MoveUpDown) {
                var pos = trans.position;
                pos.y = posy + 1.0f + Mathf.Sin(Time.time);
                trans.position = pos;
            }
        }
    }
}