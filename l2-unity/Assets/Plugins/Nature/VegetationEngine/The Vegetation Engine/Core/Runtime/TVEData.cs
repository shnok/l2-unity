// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TheVegetationEngine
{
    public class TVEConstants
    {
        public const string ElementName = "Element";
        public const string ElementTypeTag = "ElementType";
        public const string ElementLayerMask = "_ElementLayerMask";
        public const string ElementPassValue = "_ElementPassValue";
        public const string ElementDirectionMode = "_ElementDirectionMode";
        public const string ElementRaycastMode = "_ElementRaycastMode";
        public const string RaycastLayerMask = "_RaycastLayerMask";
        public const string RaycastDistanceEndValue = "_RaycastDistanceEndValue";
        public const string SpeedTresholdValue = "_SpeedTresholdValue";
    }

    public enum TVEBoolMode
    {
        Off = 0,
        On = 1,
    }

    public enum TVEPropertyType
    {
        Texture = 0,
        Vector = 1,
        Value = 2,
    }

    public enum TVEElementsVisibilityMode
    {
        AlwaysHidden = 0,
        AlwaysVisible = 10,
        HiddenAtRuntime = 20,
    }

    public enum TVEElementVisibilityMode
    {
        UseGlobalVolumeSettings = -1,
        AlwaysHidden = 0,
        AlwaysVisible = 10,
        HiddenAtRuntime = 20,
    }

    public enum TVEElementsSortingMode
    {
        SortInEditMode = 0,
        SortAtRuntime = 10,
    }

    public enum TVEVolumeDataMode
    {
        Off = -1,
        InsideGlobalVolume = 10,
        InsideRenderVolume = 20,
    }

    public enum TVERenderDataMode
    {
        Off = -1,
        InsideGlobalVolume = 10,
        InsideRenderVolume = 20,
        ScreenSpaceProjection = 50,
    }

    public enum TVETerrainDataMode
    {
        None = -1,
        HeightTexture = 10,
        NormalTexture = 20,
        HolesTexture = 30,
    }

    public enum TVETerrainRefreshMode
    {
        Always = 0,
        Selection = 10,
    }

#if UNITY_EDITOR
    public enum TVEPrefabMode
    {
        Undefined = -1,
        Converted = 10,
        Supported = 20,
        Backup = 25,
        Unsupported = 30,
        ConvertedMissingBackup = 40,
    }

    [System.Serializable]
    public class TVEPathData
    {
        public string folder = "";
        public string name = "";
        public string GUID = "";
        public string suffix = "";
        public string type = "";
        public string extention = "";

        public TVEPathData()
        {

        }
    }

    [System.Serializable]
    public class TVEPrefabData
    {
        public string prefabPath = "";
        public GameObject gameObject;
        public TVEPrefabMode status;
        public string attributes = "";
        public bool isShared = false;
        public bool isNested = false;

        public TVEPrefabData()
        {

        }
    }

    [System.Serializable]
    public class TVETransformData
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.one;

        public TVETransformData()
        {

        }
    }

    [System.Serializable]
    public class TVEGameObjectData
    {
        public GameObject parentPrefab;
        public GameObject gameObject;
        public MeshFilter meshFilter;
        public Mesh originalMesh;
        public Mesh instanceMesh;
        public List<MeshCollider> meshColliders = new List<MeshCollider>();
        public List<Mesh> originalColliders = new List<Mesh>();
        public List<Mesh> instanceColliders = new List<Mesh>();
        public MeshRenderer meshRenderer;
        public Material[] originalMaterials;
        public Material[] instanceMaterials;

        public TVEGameObjectData()
        {

        }
    }

    [System.Serializable]
    public class TVECollectionData
    {
        public string status = "";
        public string online = "";
        public string message = "";
        public float decode = 0.0f;

        public TVECollectionData()
        {

        }
    }

    [System.Serializable]
    public class TVEPropertyData
    {
        public enum PropertyType
        {
            Value = 0,
            Range = 1,
            Vector = 2,
            Color = 3,
            Enum = 4,
            Toggle = 5,
            Space = 90,
            Category = 91,
            Message = 92,
        }

        public enum PropertySetter
        {
            Value = 0,
            Vector = 1,
            Display = 90,
        }

        public PropertyType type;
        public PropertySetter setter;
        public string prop;
        public string name;
        public float value;
        public float min;
        public float max;
        public bool snap;
        public Vector4 vector;
        public string file;
        public string options;
        public bool hdr;
        public bool space;
        public int spaceTop;
        public int spaceDown;
        public string category;
        public string message;
        public MessageType messageType = MessageType.Info;

        public int isVisible = 0;
        public int isLocked = 0;

        public TVEPropertyData(string prop)
        {
            type = PropertyType.Space;
            setter = PropertySetter.Display;
            this.prop = prop;
        }

        public TVEPropertyData(string prop, string category)
        {
            type = PropertyType.Category;
            setter = PropertySetter.Display;
            this.prop = prop;
            this.category = category;
        }

        public TVEPropertyData(string prop, string message, int spaceTop, int spaceDown, MessageType messageType)
        {
            type = PropertyType.Message;
            setter = PropertySetter.Display;
            this.prop = prop;
            this.message = message;
            this.spaceTop = spaceTop;
            this.spaceDown = spaceDown;
            this.messageType = messageType;
        }

        public TVEPropertyData(string prop, string name, float value, bool snap, bool space)
        {
            type = PropertyType.Value;
            setter = PropertySetter.Value;
            this.prop = prop;
            this.name = name;
            this.value = value;
            this.snap = snap;
            this.space = space;
        }

        public TVEPropertyData(string prop, string name, float value, int min, int max, bool snap, bool space)
        {
            type = PropertyType.Range;
            setter = PropertySetter.Value;
            this.prop = prop;
            this.name = name;
            this.value = value;
            this.min = min;
            this.max = max;
            this.snap = snap;
            this.space = space;
        }

        public TVEPropertyData(string prop, string name, float value, string options, bool space)
        {
            type = PropertyType.Enum;
            setter = PropertySetter.Value;
            this.prop = prop;
            this.name = name;
            this.value = value;
            this.options = options;
            this.space = space;
        }

        public TVEPropertyData(string prop, string name, float value, string file, string options, bool space)
        {
            type = PropertyType.Enum;
            setter = PropertySetter.Value;
            this.prop = prop;
            this.name = name;
            this.value = value;
            this.file = file;
            this.options = options;
            this.space = space;
        }

        public TVEPropertyData(string prop, string name, float value, bool space)
        {
            type = PropertyType.Toggle;
            setter = PropertySetter.Value;
            this.prop = prop;
            this.name = name;
            this.value = value;
            this.space = space;
        }

        public TVEPropertyData(string prop, string name, Vector4 vector, bool space)
        {
            type = PropertyType.Vector;
            setter = PropertySetter.Vector;
            this.prop = prop;
            this.name = name;
            this.vector = vector;
            this.space = space;
        }

        public TVEPropertyData(string prop, string name, Vector4 vector, bool hdr, bool space)
        {
            type = PropertyType.Color;
            setter = PropertySetter.Vector;
            this.prop = prop;
            this.name = name;
            this.vector = vector;
            this.hdr = hdr;
            this.space = space;
        }
    }

    [System.Serializable]
    public class TVEPackerData
    {
        public Material blitMaterial;
        public Mesh blitMesh;
        public int blitPass = 0;
        public Texture[] maskTextures;
        public int[] maskChannels;
        public int[] maskCoords;
        //int[] maskLayers;
        public int[] maskActions0;
        public int[] maskActions1;
        public int[] maskActions2;
        public bool saveAsSRGB = true;
        public bool saveAsDefault = true;
        public int transformSpace;

        public TVEPackerData()
        {

        }
    }

    public class TVEModelSettings
    {
        public bool requiresProcessing = false;
        public string meshPath;
        public bool isReadable = false;
        public bool keepQuads = false;
        public ModelImporterMeshCompression meshCompression;

        public TVEModelSettings()
        {

        }
    }

    public class TVETextureSettings
    {
        public string texturePath;
        public TextureImporterSettings textureSettings = new TextureImporterSettings();
        public TextureImporterCompression textureCompression;
        public int maxTextureSize;

        public TVETextureSettings()
        {

        }
    }

    public class TVEShaderSettings
    {
        public string shaderPath;
        public string renderEngine = "Unity Default Renderer";
        public string shaderModel = "Shader Model 4.5";

        public TVEShaderSettings()
        {

        }
    }
#endif

    [System.Serializable]
    public class TVEElementMaterialData
    {
        public Shader shader;
        public string shaderName = "";
        public List<TVEElementPropertyData> props;

        public TVEElementMaterialData()
        {

        }
    }

    [System.Serializable]
    public class TVEElementPropertyData
    {
        public TVEPropertyType type;
        public string prop;
        public Texture texture;
        public Vector4 vector;
        public float value;

        public TVEElementPropertyData()
        {

        }
    }

    //[System.Serializable]
    //public class TVEElementData
    //{
    //    public TVEElement element;
    //    public int elementDataID = 0;
    //    public int instancedDataID = 0;
    //    public int renderDataID = 0;
    //    public List<int> renderLayers;
    //    public int renderPass = 0;
    //    public bool useProceduralMesh = false;
    //    public bool useGlobalVolumeVisibility = true;

    //    public TVEElementData()
    //    {

    //    }
    //}

    [System.Serializable]
    public class TVEInstanced
    {
        public int instancedDataID = 0;
        public int renderDataID = 0;
        public List<int> renderLayers;
        public int renderPass = 0;
        public Material material;
        public Mesh mesh;
        public List<TVEElement> elements = new List<TVEElement>();
        public List<Renderer> renderers = new List<Renderer>();
        public Matrix4x4[] matrices;
        public Vector4[] parameters;
        public MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        public TVEInstanced()
        {

        }
    }

    [System.Serializable]
    public class TVEVolumeData
    {
        public TVEVolumeDataMode renderMode = TVEVolumeDataMode.InsideGlobalVolume;

        [Tooltip("Custom Volume used for elements rendering.")]
        public TVEVolume renderVolume;

        //[Range(0, 2)]
        //public float textureScale = 1;
        [Tooltip("Sets the render texture width.")]
        public int textureWidth = 1024;
        [Tooltip("Sets the render texture height.")]
        public int textureHeight = 1024;

#if UNITY_EDITOR
        [Boxophobic.StyledGUI.StyledText(TextAnchor.MiddleRight, 10, 0)]
        public string debugData = "";
#endif

        public TVEVolumeData()
        {

        }
    }

    [System.Serializable]
    public class TVERenderData
    {
        public TVERenderDataMode renderMode = TVERenderDataMode.InsideGlobalVolume;

        [Tooltip("The name used for the global shader parameters.")]
        public string renderName = "CustomRenderer";

        [Tooltip("Custom Volume used for elements rendering.")]
        public TVEVolume renderVolume;
        [Tooltip("Custom Render Texture used for elements rendering.")]
        public RenderTexture renderTexture;

        [Space(10)]
        [Tooltip("The material ElementType tag used for elements filtering.")]
        public string materialTag = "CustomElement";

        [Space(10)]
        [Tooltip("Sets the render texture format.")]
        public RenderTextureFormat textureFormat = RenderTextureFormat.ARGBHalf;
        [Tooltip("Sets the render texture width.")]
        public int textureWidth = 1024;
        [Tooltip("Sets the render texture height.")]
        public int textureHeight = 1024;
        [Tooltip("Sets the render texture resolution scale.")]
        public float textureScale = 1;
        [ColorUsage(true, true)]
        [Tooltip("Sets render texture background color.")]
        public Color textureColor = Color.black;

        [Space(10)]
        [Tooltip("When enabled, the elements are rendered in realtime.")]
        public bool isRendering = true;

        [Space(10)]
        [Tooltip("When enabled, the active color space will be applied to the background color.")]
        public bool useActiveColorSpace = true;
        [Tooltip("When enabled, the render texture will render elements based on their layer.")]
        public bool useRenderTextureArray = true;

        [System.NonSerialized]
        public int renderDataID = 0;
        [System.NonSerialized]
        public int bufferSize = -1;
        [System.NonSerialized]
        public float[] bufferUsage;
        [System.NonSerialized]
        public RenderTexture textureObject;
        [System.NonSerialized]
        public CommandBuffer[] commandBuffers;
        [System.NonSerialized]
        public Vector3 internalPosition;
        [System.NonSerialized]
        public Vector3 internalScale;

        [HideInInspector]
        public string texName;
        [HideInInspector]
        public string volCoords;
        [HideInInspector]
        public string texParams;
        [HideInInspector]
        public string texLayers;
        [HideInInspector]
        public string texUsage;
        [HideInInspector]
        public string volPosition;
        [HideInInspector]
        public string volScale;

        public TVERenderData()
        {

        }
    }

    [System.Serializable]
    public class TVEMeshData
    {
        public Mesh mesh;
        public List<Vector3> vertices;
        public List<Color> colors;
        public List<Vector3> normals;
        public List<Vector4> tangents;
        public List<Vector4> UV0;
        public List<Vector4> UV2;
        public List<Vector4> UV4;

        public TVEMeshData()
        {

        }
    }

    [System.Serializable]
    public class TVEModelData
    {
        public Mesh mesh;
        public float height = 0;
        public float radius = 0;
        public List<float> variationMask;
        public List<float> occlusionMask;
        public List<float> detailMask;
        public List<float> heightMask;
        public List<Vector2> detailCoord;
        public List<float> motion2Mask;
        public List<float> motion3Mask;
        public List<Vector3> pivotPositions;

        public TVEModelData()
        {

        }
    }

    [System.Serializable]
    public class TVETerrainSettings
    {
        //[Space(10)]
        //public bool overrideAllLayers = true;
        //public bool overrideAllTextures = true;
        //public bool overrideAllSettings = true;

        [Space(10)]
        public bool useCustomTextures = false;

        [Space(10)]
        public Texture terrainControl01;
        public Texture terrainControl02;
        public Texture terrainControl03;
        public Texture terrainControl04;
        public Texture terrainHolesMask;

        [Space(10)]
        public bool useLayersOrderAsID = false;

        [Space(10)]
        public List<TVETerrainLayerSettings> terrainLayers = new List<TVETerrainLayerSettings>();
    }

    [System.Serializable]
    public class TVETerrainLayerSettings
    {
        [HideInInspector]
        public string name = "";
        [HideInInspector]
        public bool isInitialized = false;

        [Space(10)]
        [Range(1, 16)]
        public int layerID = 1;

        [Space(10)]
        public bool useCustomLayer = false;

        [Space(10)]
        public TerrainLayer terrainLayer;

        [Space(10)]
        public bool useCustomTextures = false;

        [Space(10)]
        public Texture layerAlbedo;
        public Texture layerNormal;
        public Texture layerMask;

        [Space(10)]
        public bool useCustomSettings = false;

        [Space(10)]
        public Color layerSpecular = Color.black;
        public Vector4 layerRemapMin = Vector4.zero;
        public Vector4 layerRemapMax = Vector4.one;
        [Range(0, 1)]
        public float layerSmoothness = 1;
        [Range(-8, 8)]
        public float layerNormalScale = 1;
        public Vector4 layerScaleAndOffset = new Vector4(1, 1, 0, 0);

        public TVETerrainLayerSettings()
        {

        }
    }
}