using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L2DecoLayer {

    public bool showOnTerrain;
    public Texture2D densityMap;
    public GameObject staticMesh;
    public float minHeight;
    public float maxHeight;
    public float minWidth;
    public float maxWidth;

    public L2DecoLayer() { }
}
