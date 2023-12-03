using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData {
    public string objectLayerName;
    public int objectLayer;
    public string objectTag;
    public string objectScene;
    public Transform objectTransform;

    public ObjectData(GameObject gameObject) {
        objectTransform = gameObject.transform;
        objectTag = gameObject.tag;
        objectLayerName = LayerMask.LayerToName(gameObject.layer);
        objectLayer = gameObject.layer;
        objectScene = gameObject.scene.name;
    }
}