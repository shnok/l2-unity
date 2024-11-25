#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using JBooth.MicroSplat;
using UnityEditor;
using UnityEngine;
using static JBooth.MicroSplat.MicroSplatPropData;
using static L2TerrainGeneratorTextureMatcher;

public class L2TerrainGeneratorTool : MonoBehaviour
{
    public static int UV_TEXTURE_SIZE = 256;
    public static int UV_LAYER_ALPHAMAP_SIZE = 1024;
    public static int DECO_LAYER_ALPHAMAP_SIZE = 512;
    public static float UV_TILE_SIZE = 5f;
    public static float MAP_SCALE = 1f;

    public int uvTextureSize = 256;
    public int uvLayerAlphaMapSize = 1024;
    public int decoLayerAlphaMapSize = 512;
    public float uvTileSize = 5f;
    public float mapScale = 1f;

    public bool generateMap = true;
    public static List<L2TerrainInfo> terrainInfos;
    public List<L2StaticMeshActor> meshActors;
    public static List<Terrain> terrains;
    public static Dictionary<string, Terrain> terrainsDict;
    public List<MapGenerationData> maps;

    [MenuItem("Shnok/04. [Terrain] Generate terrain")]
    static void GenerateTerrain()
    {
        string title = "Select terrain t3d";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess))
        {
            Debug.Log("Selected file: " + fileToProcess);
            string mapName = Path.GetFileNameWithoutExtension(fileToProcess);

            List<MapGenerationData> mapsToGenerate = new List<MapGenerationData>();
            MapGenerationData data = new MapGenerationData();

            data.mapName = mapName;
            data.generateDecoLayers = true;
            data.generateUVLayers = true;
            data.generateHeightmaps = true;
            data.generateStaticMeshes = false;
            data.convertToMicrosplat = true;
            data.generationMode = GenerationMode.Generate;
            mapsToGenerate.Add(data);
            GenerateMap(mapsToGenerate);
        }
    }


    [MenuItem("Shnok/05. [Terrain] Convert terrain to microsplat")]
    static void ConvertTerrain()
    {
        string title = "Select terrain t3d";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess))
        {
            Debug.Log("Selected file: " + fileToProcess);
            string mapName = Path.GetFileNameWithoutExtension(fileToProcess);

            MapGenerationData data = new MapGenerationData();
            data.mapName = mapName;
            data.convertToMicrosplat = true;
            data.generationMode = GenerationMode.Generate;
            ConvertTerrainToMicroplat(data);
        }
    }


    [MenuItem("Shnok/06. [Terrain] Update microsplat params")]
    static void Update()
    {
        string title = "Select terrain t3d";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess))
        {
            Debug.Log("Selected file: " + fileToProcess);
            string mapName = Path.GetFileNameWithoutExtension(fileToProcess);

            MapGenerationData data = new MapGenerationData();
            data.mapName = mapName;
            data.convertToMicrosplat = true;
            data.generationMode = GenerationMode.Generate;
            UpdateMicrosplatParams(data);
        }
    }

    [MenuItem("Shnok/03. [StaticMeshes] Generate staticmeshes")]
    static void GenerateStaticMeshes()
    {
        List<MapGenerationData> mapsToGenerate = new List<MapGenerationData>();
        MapGenerationData data = new MapGenerationData();

        data.mapName = "l2_lobby";
        data.generateDecoLayers = false;
        data.generateUVLayers = false;
        data.generateHeightmaps = false;
        data.generateStaticMeshes = true;
        data.generationMode = GenerationMode.Generate;
        mapsToGenerate.Add(data);

        GenerateMap(mapsToGenerate);

    }


    [MenuItem("Shnok/11. [Terrain] Stitch terrain seams")]
    static void StitchTerrainSeams()
    {
        Dictionary<string, Terrain> mapTerrains = new Dictionary<string, Terrain>();
        string mapId = "17_25";
        mapTerrains.Add(mapId, GameObject.Find(mapId).GetComponent<Terrain>());
        mapId = "16_25";
        mapTerrains.Add(mapId, GameObject.Find(mapId).GetComponent<Terrain>());
        mapId = "16_24";
        mapTerrains.Add(mapId, GameObject.Find(mapId).GetComponent<Terrain>());
        mapId = "17_24";
        mapTerrains.Add(mapId, GameObject.Find(mapId).GetComponent<Terrain>());

        L2TerrainGenerator generator = new L2TerrainGenerator();
        generator.StitchTerrainSeams(mapTerrains);
    }

    private static void GenerateMap(List<MapGenerationData> mapsToGenerate)
    {
        L2TerrainGenerator generator = new L2TerrainGenerator();
        terrainInfos = new List<L2TerrainInfo>();
        terrains = new List<Terrain>();
        terrainsDict = new Dictionary<string, Terrain>();

        for (int i = 0; i < mapsToGenerate.Count; i++)
        {

            if (!mapsToGenerate[i].enabled)
            {
                continue;
            }

            L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapsToGenerate[i].mapName);
            terrainInfos.Add(terrainInfo);

            Terrain terrain = generator.InstantiateTerrain(mapsToGenerate[i], terrainInfo);

            terrains.Add(terrain);

            terrainsDict.Add(mapsToGenerate[i].mapName, terrain);
        }

        // generator.StitchTerrainSeams(terrainsDict);
        //   generator.StitchTerrainSeams(terrainsDict);
    }

    private static void ConvertTerrainToMicroplat(MapGenerationData mapToGenerate)
    {
        GameObject terrainGo = GameObject.Find(mapToGenerate.mapName);
        Terrain terrain;
        if (terrainGo != null)
        {
            terrain = terrainGo.GetComponent<Terrain>();
        }
        else
        {
            terrainGo = Resources.Load<GameObject>(Path.Combine("Data", "Maps", mapToGenerate.mapName, "TerrainData", mapToGenerate.mapName));
            GameObject newTerrain = GameObject.Instantiate(terrainGo);
            terrain = newTerrain.GetComponent<Terrain>();
        }

        MicroSplatTerrain mst = terrainGo.AddComponent<MicroSplatTerrain>();
        MicroSplatTerrainEditor.ConvertTerrains(new Terrain[] { terrain }, terrain.terrainData.terrainLayers);
        mst.Sync();
    }

    private static void UpdateMicrosplatParams(MapGenerationData mapToGenerate)
    {
        GameObject terrainGo = GameObject.Find(mapToGenerate.mapName);
        Terrain terrain;
        if (terrainGo != null)
        {
            terrain = terrainGo.GetComponent<Terrain>();
        }
        else
        {
            terrainGo = Resources.Load<GameObject>(Path.Combine("Data", "Maps", mapToGenerate.mapName, "TerrainData", mapToGenerate.mapName));
            GameObject newTerrain = GameObject.Instantiate(terrainGo);
            terrain = newTerrain.GetComponent<Terrain>();
        }

        L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapToGenerate.mapName);

        MicroSplatTerrain mst = terrain.GetComponent<MicroSplatTerrain>();

        for (int layer = 0; layer < terrainInfo.uvLayers.Count; layer++)
        {
            string texName = terrainInfo.uvLayers[layer].texture.name;
            if (L2TerrainGeneratorTextureMatcher.Instance.scaleMatches.TryGetValue(texName, out float scale))
            {
                mst.propData.SetValue(layer, PerTexVector2.SplatUVScale, new Vector2(scale, scale));
            }
            else
            {
                Debug.LogError("Can't find matching value in scaleMatches for textue " + texName);
            }
            if (L2TerrainGeneratorTextureMatcher.Instance.pertexFloatMatches.TryGetValue(texName, out List<PerTexFloatVal> ptv))
            {
                if (ptv != null)
                {
                    ptv.ForEach((ptvv) =>
                    {
                        Debug.Log($"Setting propdata float per tex: {ptvv.ptf}:{ptvv.value}");
                        mst.propData.SetValue(layer, ptvv.ptf, ptvv.value);
                    });
                }
            }
            else
            {
                Debug.LogError("Can't find matching value in pertexFloatMatches for textue " + texName);
            }
            if (L2TerrainGeneratorTextureMatcher.Instance.pertextColorMatches.TryGetValue(texName, out List<PerTexColorVal> ptc))
            {
                if (ptc != null)
                {
                    ptc.ForEach((ptcv) =>
                    {
                        Debug.Log($"Setting propdata color per tex: {ptcv.ptf}:{ptcv.value}");
                        mst.propData.SetValue(layer, ptcv.ptf, ptcv.value);
                    });
                }
            }
            else
            {
                Debug.LogError("Can't find matching value in pertextColorMatches for textue " + texName);
            }
        }

        /*      
        public void SetValue(int textureIndex, PerTexFloat channel, float value)
        public void SetValue(int textureIndex, PerTexColor channel, Color value)
        public void SetValue(int textureIndex, PerTexVector2 channel, Vector2 value)*/

        // TextureArrayConfig.
        TextureArrayConfig cfg = Resources.Load<TextureArrayConfig>(Path.Combine("Data", "Maps", mapToGenerate.mapName, "TerrainData", "MicroSplatData", "MicroSplatConfig"));
        // if (TextureArrayConfigEditor.GetFromTerrain(cfg, terrain))
        if (cfg != null)
        {
            cfg.allTextureChannelHeight = TextureArrayConfig.AllTextureChannel.A;
            cfg.allTextureChannelSmoothness = TextureArrayConfig.AllTextureChannel.A;
            cfg.allTextureChannelAO = TextureArrayConfig.AllTextureChannel.A;
            cfg.pbrWorkflow = TextureArrayConfig.PBRWorkflow.Specular;
            cfg.sourceTextures.ForEach((sourceTexture) =>
            {
                //TODO update source textures
                // string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(info);

                // // get the updated texture based on matching table
                if (L2TerrainGeneratorTextureMatcher.Instance.textureMatches.TryGetValue(sourceTexture.diffuse.name, out string splatTexture))
                {
                    string path = TextureUtils.GetSplatTexturePath(splatTexture);
                    string diffuse = path + "_BaseColor.jpg";
                    string ao = path + "_AO.jpg";
                    string bump = path + "_Bump.jpg";
                    string normal = path + "_Normal.jpg";
                    string roughness = path + "_Roughness.jpg";
                    string specular = path + "_Specular.jpg";

                    Texture2D diffuseTex = AssetDatabase.LoadAssetAtPath<Texture2D>(diffuse);
                    Texture2D aoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ao);
                    Texture2D bumpTex = AssetDatabase.LoadAssetAtPath<Texture2D>(bump);
                    Texture2D normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normal);
                    Texture2D roughnessTex = AssetDatabase.LoadAssetAtPath<Texture2D>(roughness);
                    Texture2D specularTex = AssetDatabase.LoadAssetAtPath<Texture2D>(specular);

                    if (diffuseTex != null)
                        sourceTexture.diffuse = diffuseTex;
                    if (aoTex != null)
                        sourceTexture.ao = aoTex;
                    if (bumpTex != null)
                        sourceTexture.height = bumpTex;
                    if (normalTex != null)
                        sourceTexture.normal = normalTex;
                    if (roughnessTex != null)
                        sourceTexture.smoothness = roughnessTex;
                    if (specularTex != null)
                        sourceTexture.specular = specularTex;
                }
            });

            TextureArrayConfigEditor.CompileConfig(cfg);
        }
        else
        {
            Debug.LogError("Cant open TextureArrayConfig for map " + mapToGenerate.mapName);
        }
        mst.Sync();
    }
}
#endif
