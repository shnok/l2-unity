#if (UNITY_EDITOR) 
using UnityEngine;

[System.Serializable]
public class L2DecoLayer {
    public bool showOnTerrain { get; set; }
    public string densityMapPath { get; set; }
    public Texture2D densityMap { get; set; }
    public GameObject staticMesh { get; set; }
    public float minHeight { get; set; }
    public float maxHeight { get; set; }
    public float minWidth { get; set; }
    public float maxWidth { get; set; }
}
#endif
