#if (UNITY_EDITOR) 
public enum GenerationMode {
    Load,
    Generate
}

[System.Serializable]
public class MapGenerationData {
    public string mapName;
    public bool enabled = true;
    public GenerationMode generationMode = GenerationMode.Generate;
    public bool generateHeightmaps = true;
    public bool generateUVLayers = true;
    public bool generateDecoLayers = true;
    public bool generateStaticMeshes = true;
    public bool generateBrushes = true;
}
#endif
