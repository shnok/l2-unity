using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SurfaceDetector : MonoBehaviour 
{
    public ObjectData surfaceObject;

    public string GetSurfaceTag() {
        if(Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out var hit, 2f, World.GetInstance().groundMask)) {
            surfaceObject = new ObjectData(hit.collider.gameObject);
        } else {
            return null;
        }

        return surfaceObject.objectTag;
    }
}
