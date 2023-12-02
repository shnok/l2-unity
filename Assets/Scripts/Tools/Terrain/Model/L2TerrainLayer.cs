#if (UNITY_EDITOR) 
using UnityEngine;

[System.Serializable]
public class L2TerrainLayer {
    public Texture2D texture { get; set; }
    public Texture2D alphaMap { get; set; }
    public float uScale { get; set; }
    public float vScale { get; set; }
}
#endif
