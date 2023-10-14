using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapGenerationData {
    public string mapName;
    public bool enabled;
    public bool generateHeightmaps = true;
    public bool generateUVLayers = true;
    public bool generateDecoLayers = true;
    public bool generateStaticMeshes = true;
}

