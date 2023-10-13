using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L2TerrainInfo
{
    public string mapName;
    public int generatedSectorCounter = 0;
    public string terrainMapPath = string.Empty;
    public Vector3 terrainScale;
    public Vector3 location;
    public List<L2TerrainLayer> layers;

    public L2TerrainInfo() { }
}
