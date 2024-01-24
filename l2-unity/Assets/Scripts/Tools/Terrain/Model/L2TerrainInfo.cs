#if (UNITY_EDITOR) 
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L2TerrainInfo {
    public string mapName { get; set; }
    public int generatedSectorCounter { get; set; }
    public string terrainMapPath { get; set; }
    public Vector3 terrainScale { get; set; }
    public Vector3 location { get; set; }
    public List<L2TerrainLayer> uvLayers { get; set; }
    public List<L2DecoLayer> decoLayers { get; set; }
}
#endif