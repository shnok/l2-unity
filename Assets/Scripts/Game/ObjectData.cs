using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData {
    public string objectLayer;
    public string objectTag;
    public string objectScene;
    public Transform objectTransform;

    public ObjectData(GameObject gameObject) {
        objectTransform = gameObject.transform;
        objectTag = gameObject.tag;
        objectLayer = LayerMask.LayerToName(gameObject.layer);
        objectScene = gameObject.scene.name;
    }
}