using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using static KWS.KWS_CoreUtils;
using static KWS.KW_WaterOrthoDepth;

namespace KWS
{
    public class KW_ShorelineWaves : MonoBehaviour
    {
        public WaterSystem WaterInstance;

        private const string shorelineWaveBakinng_shaderName = "Hidden/KriptoFX/KWS/ShorelineWavePosition";


        private const string path_shoreLineFolder = "ShorelineMaps";
        private const string path_shoreLineWavesData = "KW_ShorelineWavesData";
        private const string path_shorelineDepthData = "KW_Shoreline_DepthData";
        private const string path_shorelineDepth = "KW_Shoreline_Depth";
        private const string path_shorelineMapUV1 = "KW_Shoreline1_UV_Angle_Alpha";
        private const string path_shorelineMapData1 = "KW_BakedWaves1_TimeOffset_Scale";
        private const string path_shorelineMapUV2 = "KW_Shoreline2_UV_Angle_Alpha";
        private const string path_shorelineMapData2 = "KW_BakedWaves2_TimeOffset_Scale";

        private const string path_ToShorelineWaveTex = "Shoreline_Pos_14x15";
        private const string path_ToShorelineWaveNormTex = "Shoreline_Norm_14x15";
        private const string path_VAT_PositionTex = "VAT_Position";
        private const string path_VAT_AlphaTex = "VAT_Alpha";
        private const string path_VAT_RangeLookupTex = "VAT_RangeLookup";
        private const string path_VAT_OffsetTex = "BeachWaveParticlesOffset";

        private int ID_KW_ShorelineAreaPos = Shader.PropertyToID("KW_ShorelineAreaPos");

        private int ID_KW_ShorelineDepth = Shader.PropertyToID("KW_ShorelineDepthTex");
        private int ID_KW_ShorelineDepthOrthoSize = Shader.PropertyToID("KW_ShorelineDepthOrthoSize");
        private int ID_KW_ShorelineDepthNearFarDistance = Shader.PropertyToID("KW_ShorelineDepth_Near_Far_Dist");


        private int ID_KW_ShorelineWaveDisplacement = Shader.PropertyToID("KW_ShorelineWaveDisplacement");
        private int ID_KW_ShorelineWaveNormal = Shader.PropertyToID("KW_ShorelineWaveNormal");
        private int ID_KW_ShorelineVAT_Mesh = Shader.PropertyToID("KW_Vat_Mesh");
        private int ID_KW_ShorelineVAT_Position = Shader.PropertyToID("KW_VAT_Position");
        private int ID_KW_ShorelineVAT_Alpha = Shader.PropertyToID("KW_VAT_Alpha");
        private int ID_KW_ShorelineVAT_RangeLookup = Shader.PropertyToID("KW_VAT_RangeLookup");
        private int ID_KW_ShorelineVAT_Offset = Shader.PropertyToID("KW_VAT_Offset");
        private int ID_KW_ShorelineMapSize = Shader.PropertyToID("KW_WavesMapSize");
        // private int ID_KWS_FoamParticle = Shader.PropertyToID("KWS_FoamParticle");

        private Texture2D shorelineWaveDisplacementTex;
        private Texture2D shorelineWaveNormalTex;
        private Mesh VAT_Mesh_Lod0;
        private Mesh VAT_Mesh_Lod1;
        private Mesh VAT_Mesh_Lod2;
        private Mesh VAT_Mesh_Lod3;
        private Mesh VAT_Mesh_Lod4;
        private Mesh VAT_Mesh_Lod5;
        private Mesh VAT_Mesh_Lod6;
        private Mesh VAT_Mesh_Lod7;
        public Texture2D VAT_Position;
        public Texture2D VAT_Alpha;
        private Texture2D VAT_RangeLookup;
        private Texture2D VAT_Offset;
        //private Texture2D _foamParticle;

        private Camera cam;
        private GameObject camGO;

        private Camera depthCam;
        private GameObject depthCamGO;

        Material waveMaterial;
        private Material foamMaterial;
        private Material foamShadowMaterial;
       // private GameObject quad;
        Mesh tempMesh1, tempMesh2;

        public TemporaryRenderTexture depthRT = new TemporaryRenderTexture();
        public TemporaryRenderTexture wavesRT1 = new TemporaryRenderTexture();
        public TemporaryRenderTexture wavesRT2 = new TemporaryRenderTexture();
        public TemporaryRenderTexture wavesDataRT1 = new TemporaryRenderTexture();
        public TemporaryRenderTexture wavesDataRT2 = new TemporaryRenderTexture();

        public Texture2D depth_tex;
        public Texture2D waves_tex1;
        public Texture2D waves_tex2;
        public Texture2D wavesData1_tex;
        public Texture2D wavesData2_tex;

        List<GameObject> wavesObjects = new List<GameObject>();
        private Dictionary<int, CustomLod> foamGameObjects = new Dictionary<int, CustomLod>();
        List<GameObject> foamLodsForLateDeactivation = new List<GameObject>();
        ShorelineData _shorelineData;
        OrthoDepthParams depthParams;


        private bool isInitializedEditorResources;
        private bool isInitializedShorelineResources;

        private const float HeightWave1 = 7000;
        private const float HeightWave2 = 7010;
        private const float GlobalTimeOffsetMultiplier = 34;
        private const float GlobalTimeSpeedMultiplier = 1.0f;
        private const int nearPlaneDepth = -2;
        private const int farPlaneDepth = 100;
        private int _lastShorelineAreaSize;
        private Vector3 _lastShorelinePos;

        private const int VATMeshMaxParticlesCount = 385871;

        [Serializable]
        public class ShorelineData
        {
            [SerializeField] public List<ShorelineWaveInfo> ShorelineWaves = new List<ShorelineWaveInfo>();
        }

        [Serializable]
        public class ShorelineWaveInfo
        {
            [SerializeField] public int ID;
            [SerializeField] public float PositionX;
            [SerializeField] public float PositionZ;
            [SerializeField] public float EulerRotation;
            [SerializeField] public float ScaleX = 14;
            [SerializeField] public float ScaleY = 4.5f;
            [SerializeField] public float ScaleZ = 16;
            [SerializeField] public float TimeOffset = 0;
            [SerializeField] public float DefaultScaleX = 14;
            [SerializeField] public float DefaultScaleY = 4.5f;
            [SerializeField] public float DefaultScaleZ = 16;
        }


        public float[] LodFoamSize = new float[] { 0, 0.02f, 0.045f, 0.07f, 0.11f, 0.15f, 0.22f, 0.3f };
        public float[] LodDistances_High = new float[] { 40, 43, 46, 50, 54, 60, 70, 90 };
        public float[] LodDistances_Medium = new float[] { 20, 25, 30, 35, 40, 45, 50, 70 };
        //public float[] LodFoamSize_Medium = new float[] { 0, 0.02f, 0.045f, 0.07f, 0.11f, 0.15f, 0.22f, 0.3f };
        public float[] LodDistances_Low = new float[] { 10, 13, 16, 20, 24, 30, 40, 60 };
        //public float[] LodFoamSize_Low = new float[] { 0, 0.02f, 0.045f, 0.07f, 0.11f, 0.15f, 0.22f, 0.3f };

        class CustomLod
        {
            public GameObject Parent;
            public GameObject[] LodObjects;
            public GameObject[] ShadowObjects;

            public Vector3 Position;
            public int ActiveLod = -1;
        }

        public void InitializeMaterials()
        {
            if (foamMaterial == null)
            {
                foamMaterial = CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.ShorelineFoamParticles_shaderName));
                WaterInstance.AddMaterialToWaterRendering(foamMaterial);
            }
            if (foamShadowMaterial == null)
            {
                foamShadowMaterial = CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.ShorelineFoamParticlesShadow_shaderName));
                WaterInstance.AddMaterialToWaterRendering(foamShadowMaterial);
            }
        }

        public async Task<ShorelineData> GetShorelineData(string GUID)
        {
            if (_shorelineData == null || _shorelineData.ShorelineWaves == null || _shorelineData.ShorelineWaves.Count == 0)
            {
                var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();

                var pathToShorelineDirectory = Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID);
                if (!Directory.Exists(pathToShorelineDirectory))
                {
                    _shorelineData = new ShorelineData();
                    return _shorelineData;
                }

                _shorelineData = await KW_Extensions.DeserializeFromFile<ShorelineData>(Path.Combine(pathToShorelineDirectory, path_shoreLineWavesData));
                if (_shorelineData == null) _shorelineData = new ShorelineData();
            }

            return _shorelineData;
        }

        class DataQuadsMesh
        {
            Mesh _mesh;

            List<Vector3> _vertices = new List<Vector3>();
            List<int> _triangles = new List<int>();
            List<Vector2> _uv = new List<Vector2>();
            List<Vector2> _uv2 = new List<Vector2>();
            List<Vector2> _uv3 = new List<Vector2>();
            List<Vector2> _uv4 = new List<Vector2>();
            int vertOffset;

            public DataQuadsMesh(Mesh mesh)
            {
                vertOffset = 0;

                _mesh = mesh;
                _mesh.MarkDynamic();
            }

            public void WriteDataQuad(Vector3 position, Quaternion rotation, Vector3 scale, Vector2 uv2, Vector2 uv3, Vector2 uv4)
            {
                for (int j = 0; j < 4; j++)
                {
                    _uv2.Add(uv2);
                    _uv3.Add(uv3);
                    _uv4.Add(uv4);
                }

                var trsMatrix = Matrix4x4.TRS(position, rotation, scale);

                _vertices.Add(trsMatrix.MultiplyPoint3x4(new Vector3(-0.5f, -0.5f, 0)));
                _vertices.Add(trsMatrix.MultiplyPoint3x4(new Vector3(0.5f, -0.5f, 0)));
                _vertices.Add(trsMatrix.MultiplyPoint3x4(new Vector3(-0.5f, 0.5f, 0)));
                _vertices.Add(trsMatrix.MultiplyPoint3x4(new Vector3(0.5f, 0.5f, 0)));

                _uv.Add(new Vector2(0, 0));
                _uv.Add(new Vector2(1, 0));
                _uv.Add(new Vector2(0, 1));
                _uv.Add(new Vector2(1, 1));

                _triangles.Add(0 + vertOffset);
                _triangles.Add(1 + vertOffset);
                _triangles.Add(2 + vertOffset);
                _triangles.Add(2 + vertOffset);
                _triangles.Add(1 + vertOffset);
                _triangles.Add(3 + vertOffset);

                vertOffset += 4;
            }

            public void Apply()
            {
                _mesh.vertices = _vertices.ToArray();
                _mesh.triangles = _triangles.ToArray();
                _mesh.SetUVs(0, _uv);
                _mesh.SetUVs(1, _uv2);
                _mesh.SetUVs(2, _uv3);
                _mesh.SetUVs(3, _uv4);
                var bounds = _mesh.bounds;
                bounds.size = Vector3.one * 100000;
                _mesh.bounds = bounds;
            }
        }

        public async Task BakeWavesToTexture(int areaSize, Vector3 shorelineAreaPos, float curvedSurfacesQualityScale, Vector3 waterPos, string GUID)
        {
            var shorelineData = await GetShorelineData(GUID);
            var shorelineWaves = shorelineData.ShorelineWaves;
            if (shorelineWaves.Count == 0) return;

            if (!isInitializedEditorResources) InitializeEditorResources();

            if (tempMesh1 == null) tempMesh1 = new Mesh();
            if (tempMesh2 == null) tempMesh2 = new Mesh();
            var shorelineDataMesh1 = new DataQuadsMesh(tempMesh1);
            var shorelineDataMesh2 = new DataQuadsMesh(tempMesh2);

            int texSize = areaSize * 2;
            texSize = Math.Min(texSize, 4096);

            for (var i = 0; i < shorelineWaves.Count; i++)
            {
                var metersInPixels = (1f * areaSize / texSize);

                var roundedPos = new Vector2(Mathf.Round(shorelineWaves[i].PositionX / metersInPixels) * metersInPixels, Mathf.Round(shorelineWaves[i].PositionZ / metersInPixels) * metersInPixels);
                var roundedScale = new Vector2(Mathf.Round(shorelineWaves[i].ScaleX / metersInPixels) * metersInPixels, Mathf.Round(shorelineWaves[i].ScaleZ / metersInPixels) * metersInPixels);

                var position = new Vector3(roundedPos.x, 0, roundedPos.y);
                var rotation = Quaternion.Euler(270, 0, shorelineWaves[i].EulerRotation);
                var scale = roundedScale;

                var scaleTexels = new Vector2(metersInPixels * scale.x, metersInPixels * scale.y);
                var uvOffset = new Vector2(1 - (scaleTexels.x - metersInPixels) / scaleTexels.x, 1 - (scaleTexels.y - metersInPixels) / scaleTexels.y);

                var scaleMultiplier = shorelineWaves[i].ScaleY / shorelineWaves[i].DefaultScaleY;
                if (scaleMultiplier < 1.0f) scaleMultiplier *= Mathf.Lerp(0.25f, 1f, scaleMultiplier);

                var targetMesh = shorelineWaves[i].ID % 2 == 0 ? shorelineDataMesh1 : shorelineDataMesh2;
                targetMesh.WriteDataQuad(position, rotation, scale, uvOffset, new Vector2(scaleMultiplier, shorelineWaves[i].TimeOffset), new Vector2((rotation.eulerAngles.y % 360f) / 360f, 0));
            }
            shorelineDataMesh1.Apply();
            shorelineDataMesh2.Apply();

            InitializeEditorTextures(texSize);

            var activeRT = Graphics.activeColorBuffer;
            var activeDepthRT = Graphics.activeDepthBuffer;
            waveMaterial.SetVector("KW_ShorelineAreaPos", shorelineAreaPos);
            waveMaterial.SetFloat("KW_ShorelineAreaSize", areaSize);
            waveMaterial.SetPass(0);
            Graphics.SetRenderTarget(new[] { wavesRT1.rt.colorBuffer, wavesDataRT1.rt.colorBuffer }, wavesRT1.rt.depthBuffer);
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            Graphics.DrawMeshNow(tempMesh1, Matrix4x4.identity);

            Graphics.SetRenderTarget(new[] { wavesRT2.rt.colorBuffer, wavesDataRT2.rt.colorBuffer }, wavesRT2.rt.depthBuffer);
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            Graphics.DrawMeshNow(tempMesh2, Matrix4x4.identity);

            Graphics.SetRenderTarget(activeRT, activeDepthRT);

            var depthSize = Mathf.Min(4096, (int)(texSize * curvedSurfacesQualityScale));
            shorelineAreaPos.y = transform.position.y;
            RenderDepth(depthCam, depthRT, shorelineAreaPos, areaSize, depthSize);
            if (depthParams == null) depthParams = new OrthoDepthParams();
            depthParams.SetData(areaSize, waterPos);
            UpdateShaderParameters();
        }

        public void SavesWavesParamsToDataFolder(string GUID, string pathToBakedDataFolder = "")
        {
            if (pathToBakedDataFolder == String.Empty) pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();

            KW_Extensions.SerializeToFile(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shoreLineWavesData), _shorelineData);
        }

        public void SaveWavesToDataFolder(string GUID)
        {
            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();

            SavesWavesParamsToDataFolder(GUID, pathToBakedDataFolder);

            wavesRT1.rt.SaveRenderTextureToFile(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineMapUV1), TextureFormat.RGBAHalf);
            wavesRT2.rt.SaveRenderTextureToFile(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineMapUV2), TextureFormat.RGBAHalf);
            wavesDataRT1.rt.SaveRenderTextureToFile(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineMapData1), TextureFormat.RGHalf);
            wavesDataRT2.rt.SaveRenderTextureToFile(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineMapData2), TextureFormat.RGHalf);
        }

        public void SaveOrthoDepth(string GUID)
        {
            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();

            SaveDepthTextureToFile(depthRT, Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineDepth));
            SaveDepthDataToFile(depthParams, Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineDepthData));
        }

        public async Task<bool> RenderShorelineWavesWithFoam(int shorelineAreaSize, Vector3 shorelineAreaPos, string GUID)
        {
            var shorelineData = await GetShorelineData(GUID);
            var shorelineWaves = shorelineData.ShorelineWaves;
            if (shorelineWaves.Count == 0) return false;

            InitializeMaterials();
            if (!isInitializedShorelineResources) await InitializeShorelineResources(GUID);
            _lastShorelineAreaSize = shorelineAreaSize;
            _lastShorelinePos = shorelineAreaPos;
            UpdateShaderParameters();
            RenderFoam(shorelineWaves);
            return true;
        }

        void RenderFoam(List<ShorelineWaveInfo> shorelineWavesData)
        {
            //Debug.Log("RenderFoam");

            if (VAT_Mesh_Lod0 == null || foamMaterial == null || foamShadowMaterial == null) return;

            var lodArray = new[] { VAT_Mesh_Lod0, VAT_Mesh_Lod1, VAT_Mesh_Lod2, VAT_Mesh_Lod3, VAT_Mesh_Lod4, VAT_Mesh_Lod5, VAT_Mesh_Lod6, VAT_Mesh_Lod7 };
            var props = new MaterialPropertyBlock();
            var activeID = new List<int>();
            foreach (var wave in shorelineWavesData)
            {
                if (!foamGameObjects.ContainsKey(wave.ID))
                {
                    var vatGO = new GameObject("FoamParticles");
                    vatGO.transform.parent = transform;
                    var customLod = new CustomLod
                    {
                        Parent = vatGO,
                        LodObjects = new GameObject[lodArray.Length],
                        ShadowObjects = new GameObject[lodArray.Length]
                    };

                    for (var i = 0; i < lodArray.Length; i++)
                    {
                        var lodGO = CreateLodWave("FoamParticles_Lod" + i, vatGO.transform, lodArray[i], props, wave.TimeOffset, LodFoamSize[i], false);
                        var shadowMesh = lodArray.Length > i + KWS_Settings.Shoreline.ShadowParticlesDivider ? lodArray[i + KWS_Settings.Shoreline.ShadowParticlesDivider] : lodArray.Last();
                        var shadowLodGO = CreateLodWave("FoamParticlesShadow_Lod" + i, lodGO.transform, shadowMesh, props, wave.TimeOffset, LodFoamSize[i] + 0.25f, true);

                        customLod.LodObjects[i] = lodGO;
                        customLod.ShadowObjects[i] = shadowLodGO;
                    }

                    foamGameObjects.Add(wave.ID, customLod);
                }

                activeID.Add(wave.ID);
                var vatParticlesGO = foamGameObjects[wave.ID];
                var lessScaleFix = Mathf.Lerp(0.15f, 0, Mathf.Clamp01((wave.ScaleY / wave.DefaultScaleY)));
                vatParticlesGO.Parent.transform.position = new Vector3(wave.PositionX, transform.position.y + lessScaleFix, wave.PositionZ);
                vatParticlesGO.Parent.transform.rotation = Quaternion.Euler(0, wave.EulerRotation, 0);
                vatParticlesGO.Parent.transform.localScale = new Vector3(wave.ScaleX / wave.DefaultScaleX, wave.ScaleY / wave.DefaultScaleY, wave.ScaleZ / wave.DefaultScaleZ);
                vatParticlesGO.Position = vatParticlesGO.Parent.transform.position;
            }

            RemoveNotActualFoam(activeID);
        }

        private void RemoveNotActualFoam(List<int> activeID)
        {
            foreach (var foamGO in foamGameObjects.ToList())
            {
                if (!activeID.Contains(foamGO.Key))
                {
                    KW_Extensions.SafeDestroy(foamGameObjects[foamGO.Key].Parent);
                    foamGameObjects.Remove(foamGO.Key);
                }
            }
        }


        GameObject CreateLodWave(string lodName, Transform parent, Mesh mesh, MaterialPropertyBlock props, float timeOffset, float lodParticleSize, bool useShadows)
        {
            var lod = new GameObject(lodName);
            lod.SetActive(false);
            lod.transform.parent = parent;
            lod.layer = 4 << 0;
            lod.AddComponent<MeshFilter>().sharedMesh = mesh;

            var vatRend = lod.AddComponent<MeshRenderer>();
            vatRend.sharedMaterial = useShadows ? foamShadowMaterial : foamMaterial;
            vatRend.shadowCastingMode = useShadows ? ShadowCastingMode.TwoSided : ShadowCastingMode.Off;
            vatRend.GetPropertyBlock(props);
            props.SetFloat("KW_WaveTimeOffset", timeOffset);
            props.SetFloat("KW_SizeAdditiveScale", lodParticleSize);
            vatRend.SetPropertyBlock(props);
            return lod;
        }

        public void ClearFoam()
        {
            if (wavesObjects != null)
            {
                foreach (var waveObj in wavesObjects) KW_Extensions.SafeDestroy(waveObj);
                wavesObjects.Clear();
            }

            if (foamGameObjects != null)
            {
                foreach (var vatParticles in foamGameObjects)
                {
                    KW_Extensions.SafeDestroy(vatParticles.Value.Parent);
                }
                foamGameObjects.Clear();
            }
            foamLodsForLateDeactivation.Clear();
        }

        public void ClearShorelineWavesWithFoam(string GUID)
        {
            ClearFoam();
            if (_shorelineData != null && _shorelineData.ShorelineWaves != null) _shorelineData.ShorelineWaves.Clear();

            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            var directory = Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID);
            if (Directory.Exists(directory)) Directory.Delete(directory, true);

            ReleaseEditorResources();
            isInitializedEditorResources = false;
        }

        public void Release()
        {
            OnDisable();
        }

        private float UpdateEachSeconds = 0.5f;
        private float lastLodUpdateLeftTime = 0;
        private bool updateLodTest = false;

        bool lastFoamShadowsStatus;

        public void UpdateLodLevels(Vector3 camPos, int qualityLevel, bool foamCastShadows, bool foamReceiveShadows)
        {
            if (!isInitializedShorelineResources || foamGameObjects.Count == 0) return;

            foreach (var foamRend in foamLodsForLateDeactivation)
            {
                if (foamRend != null) foamRend.SetActive(false);
            }

            if (lastLodUpdateLeftTime < 0) lastLodUpdateLeftTime = 0;
            lastLodUpdateLeftTime += KW_Extensions.DeltaTime();
            if (lastLodUpdateLeftTime < UpdateEachSeconds) return;
            lastLodUpdateLeftTime = 0;
            foamLodsForLateDeactivation.Clear();

            foreach (var vatLodGO in foamGameObjects)
            {
                if (vatLodGO.Value != null)
                {
                    var customLod = vatLodGO.Value;
                    var lods = customLod.LodObjects;
                    var distance = Vector3.Distance(camPos, customLod.Position);
                    int n = 0;
                    for (var i = 0; i < lods.Length; i++)
                    {
                        float lodDist = 10f;
                        if (qualityLevel == 0) lodDist = LodDistances_High[i];
                        else if (qualityLevel == 1) lodDist = LodDistances_Medium[i];
                        else if (qualityLevel == 2) lodDist = LodDistances_Low[i];

                        if (distance > lodDist) n = i;
                    }
                    if (n != customLod.ActiveLod)
                    {
                        if (customLod.ActiveLod != -1) foamLodsForLateDeactivation.Add(lods[customLod.ActiveLod]);
                        customLod.ActiveLod = n;
                        lods[n].SetActive(true);
                    }
                }
            }

            if (lastFoamShadowsStatus != foamCastShadows)
            {

                foreach (var vatLodGO in foamGameObjects)
                {
                    if (vatLodGO.Value != null)
                    {
                        var shadowsObjects = vatLodGO.Value.ShadowObjects;
                        foreach (var shadowGO in shadowsObjects)
                        {
                            shadowGO.SetActive(foamCastShadows);
                        }
                    }
                }
                lastFoamShadowsStatus = foamCastShadows;
            }
            foamMaterial.SetKeyword(KWS_ShaderConstants.ShorelineKeywords.FOAM_RECEIVE_SHADOWS, foamReceiveShadows);
            UpdateFoamParameters();
        }

        void OnDisable()
        {
            KW_Extensions.SafeDestroy(camGO, waveMaterial, foamMaterial, foamShadowMaterial);

            ClearFoam();

            if (_shorelineData != null && _shorelineData.ShorelineWaves != null) _shorelineData.ShorelineWaves.Clear();

            ReleaseEditorResources();
            KW_Extensions.SafeDestroy(foamMaterial, foamShadowMaterial, shorelineWaveDisplacementTex, shorelineWaveNormalTex,
                VAT_Mesh_Lod0, VAT_Mesh_Lod1, VAT_Mesh_Lod2, VAT_Mesh_Lod3, VAT_Mesh_Lod4, VAT_Mesh_Lod5, VAT_Mesh_Lod6, VAT_Mesh_Lod7,
                VAT_Position, VAT_Alpha, VAT_RangeLookup, VAT_Offset);
            // Resources.UnloadAsset(_foamParticle);

            isInitializedShorelineResources = false;
            isInitializedEditorResources = false;

            lastLodUpdateLeftTime = 0;
            lastFoamShadowsStatus = false;
        }

        public void ReleaseEditorResources()
        {
            KW_Extensions.SafeDestroy(waves_tex1, waves_tex2, wavesData1_tex, wavesData2_tex, depth_tex, camGO, depthCamGO, tempMesh1, tempMesh2);
            KWS_CoreUtils.ReleaseTemporaryRenderTextures(true, wavesRT1, wavesRT2, wavesDataRT1, wavesDataRT2);
            depthRT.Release(true);
        }

        //GameObject CreateTempGO()
        //{
        //    var go = Instantiate(quad);
        //    go.transform.parent = transform;
        //    var rend = go.GetComponent<MeshRenderer>();
        //    rend.sharedMaterial = waveMaterial;
        //    go.SetActive(false);
        //    return go;
        //}

        public async Task InitialiseShorelineEditorResources(string GUID)
        {
            await InitializeShorelineResources(GUID);
        }

        public async Task InitializeShorelineResources(string GUID)
        {
            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();

            if (shorelineWaveDisplacementTex == null) shorelineWaveDisplacementTex = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, path_ToShorelineWaveTex));
            if (shorelineWaveNormalTex == null) shorelineWaveNormalTex = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, path_ToShorelineWaveNormTex), true);

            if (VAT_Position == null) VAT_Position = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, path_VAT_PositionTex), true, FilterMode.Point, TextureWrapMode.Repeat);
            if (VAT_Alpha == null) VAT_Alpha = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, path_VAT_AlphaTex), true, FilterMode.Point, TextureWrapMode.Repeat);
            if (VAT_RangeLookup == null) VAT_RangeLookup = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, path_VAT_RangeLookupTex), true, FilterMode.Point, TextureWrapMode.Repeat);
            if (VAT_Offset == null) VAT_Offset = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, path_VAT_OffsetTex));
            if (VAT_Mesh_Lod0 == null)
            {
                var boundSize = new Vector3(14, 4.5f, 16);
                VAT_Mesh_Lod0 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 0);
                VAT_Mesh_Lod1 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 1);
                VAT_Mesh_Lod2 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 2);
                VAT_Mesh_Lod3 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 3);
                VAT_Mesh_Lod4 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 4);
                VAT_Mesh_Lod5 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 5);
                VAT_Mesh_Lod6 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 6);
                VAT_Mesh_Lod7 = CreateVATMesh(VATMeshMaxParticlesCount, boundSize, 7);
            }

            //if(_foamParticle == null) _foamParticle = Resources.Load<Texture2D>("KW_FoamParticle");

            if (foamMaterial != null)
            {
                foamMaterial.SetTexture(ID_KW_ShorelineVAT_Position, VAT_Position);
                foamMaterial.SetTexture(ID_KW_ShorelineVAT_Alpha, VAT_Alpha);
                foamMaterial.SetTexture(ID_KW_ShorelineVAT_RangeLookup, VAT_RangeLookup);
                foamMaterial.SetTexture(ID_KW_ShorelineVAT_Offset, VAT_Offset);
                //vatMaterial.SetTexture(ID_KWS_FoamParticle, _foamParticle);
            }
            if (foamShadowMaterial != null)
            {
                foamShadowMaterial.SetTexture(ID_KW_ShorelineVAT_Position, VAT_Position);
                foamShadowMaterial.SetTexture(ID_KW_ShorelineVAT_Alpha, VAT_Alpha);
                foamShadowMaterial.SetTexture(ID_KW_ShorelineVAT_RangeLookup, VAT_RangeLookup);
                foamShadowMaterial.SetTexture(ID_KW_ShorelineVAT_Offset, VAT_Offset);
            }

            if (foamMaterial != null && VAT_Mesh_Lod0 && VAT_Position != null && foamShadowMaterial != null) isInitializedShorelineResources = true;

            var pathToShorelineDirectory = Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID);
            if (File.Exists(Path.Combine(pathToShorelineDirectory, path_shorelineMapUV1 + ".gz")))
            {
                if (waves_tex1 == null) waves_tex1 = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToShorelineDirectory, path_shorelineMapUV1), true, FilterMode.Bilinear, TextureWrapMode.Clamp);
                if (waves_tex2 == null) waves_tex2 = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToShorelineDirectory, path_shorelineMapUV2), true, FilterMode.Bilinear, TextureWrapMode.Clamp);
                if (wavesData1_tex == null) wavesData1_tex = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToShorelineDirectory, path_shorelineMapData1), true, FilterMode.Point, TextureWrapMode.Clamp);
                if (wavesData2_tex == null) wavesData2_tex = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToShorelineDirectory, path_shorelineMapData2), true, FilterMode.Point, TextureWrapMode.Clamp);

                depthParams = await KW_Extensions.DeserializeFromFile<OrthoDepthParams>(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineDepthData));
                if (depth_tex == null) depth_tex = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToBakedDataFolder, path_shoreLineFolder, GUID, path_shorelineDepth));

                if (foamMaterial != null)
                {
                    foamMaterial.SetTexture(ID_KW_ShorelineDepth, depthRT.rt != null ? depthRT.rt as Texture : depth_tex);
                    foamMaterial.SetFloat(ID_KW_ShorelineDepthOrthoSize, depthParams.OtrhograpicSize);
                    foamMaterial.SetVector(ID_KW_ShorelineDepthNearFarDistance, new Vector3(nearPlaneDepth, farPlaneDepth, farPlaneDepth - nearPlaneDepth));
                }
                if (foamShadowMaterial != null)
                {
                    foamShadowMaterial.SetTexture(ID_KW_ShorelineDepth, depthRT.rt != null ? depthRT.rt as Texture : depth_tex);
                    foamShadowMaterial.SetFloat(ID_KW_ShorelineDepthOrthoSize, depthParams.OtrhograpicSize);
                    foamShadowMaterial.SetVector(ID_KW_ShorelineDepthNearFarDistance, new Vector3(nearPlaneDepth, farPlaneDepth, farPlaneDepth - nearPlaneDepth));
                }
            }


            // Debug.Log("Initialized Shoreline Resources " + VAT_Position.width + "  " + VAT_Mesh_Lod0.triangles.Length);
        }

        void InitializeEditorTextures(int size)
        {
            wavesRT1.Alloc("waves1_rt", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat);
            wavesRT2.Alloc("waves2_rt", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat);
            wavesDataRT1.Alloc("wavesData1_rt", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat);
            wavesDataRT2.Alloc("wavesData2_rt", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat);
        }

        void InitializeEditorResources()
        {
            waveMaterial = KWS_CoreUtils.CreateMaterial(shorelineWaveBakinng_shaderName);

            depthCam = InitializeDepthCamera(nearPlaneDepth, farPlaneDepth, transform);
            depthCamGO = depthCam.gameObject;

            isInitializedEditorResources = true;
        }

        //public Mesh CreateVATMesh(int maxParticlesCount, Vector3 size, int lodLevel)
        //{
        //    var lodOffset = Mathf.Pow(2, lodLevel);
        //    var currentParticles = (int)(1.0f * maxParticlesCount / lodOffset);

        //    var vertices = new Vector3[4 * currentParticles];
        //    var tris = new int[6 * currentParticles];
        //    var uv = new List<Vector3>(4 * currentParticles);

        //    int vertIdx = 0;
        //    int trisIdx = 0;
        //    var pos1 = new Vector3(0, 0, 0);
        //    var pos2 = new Vector3(1, 0, 0);
        //    var pos3 = new Vector3(0, 1, 0);
        //    var pos4 = new Vector3(1, 1, 0);

        //    for (int i = 0; i < currentParticles; i++)
        //    {
        //        vertices[vertIdx + 0] = pos1;
        //        vertices[vertIdx + 1] = pos2;
        //        vertices[vertIdx + 2] = pos3;
        //        vertices[vertIdx + 3] = pos4;

        //        tris[trisIdx + 0] = vertIdx + 0;
        //        tris[trisIdx + 1] = vertIdx + 2;
        //        tris[trisIdx + 2] = vertIdx + 1;

        //        tris[trisIdx + 3] = vertIdx + 2;
        //        tris[trisIdx + 4] = vertIdx + 3;
        //        tris[trisIdx + 5] = vertIdx + 1;

        //        uv.Add(new Vector3(0, 0, 1f * i * lodOffset));
        //        uv.Add(new Vector3(1, 0, 1f * i * lodOffset));
        //        uv.Add(new Vector3(0, 1, 1f * i * lodOffset));
        //        uv.Add(new Vector3(1, 1, 1f * i * lodOffset));

        //        vertIdx += 4;
        //        trisIdx += 6;
        //    }

        //    var mesh = new Mesh();
        //    mesh.indexFormat = IndexFormat.UInt32;
        //    mesh.vertices = vertices;
        //    mesh.triangles = tris;
        //    mesh.SetUVs(0, uv);
        //    mesh.bounds = new Bounds(Vector3.zero, size);
        //    return mesh;
        //}


        public Mesh CreateVATMesh(int maxParticlesCount, Vector3 size, int lodLevel)
        {
            var lodOffset = Mathf.Pow(2, lodLevel);
            var currentParticles = (int)(1.0f * maxParticlesCount / lodOffset);

            var vertices = new Vector3[3 * currentParticles];
            var tris = new int[3 * currentParticles];
            var uv = new List<Vector3>(3 * currentParticles);

            int vertIdx = 0;
            int trisIdx = 0;
            var pos1 = new Vector3(0, 0, 0);
            var pos2 = new Vector3(1, 0, 0);
            var pos3 = new Vector3(1, 1, 0);


            for (int i = 0; i < currentParticles; i++)
            {
                vertices[vertIdx + 0] = pos1;
                vertices[vertIdx + 1] = pos2;
                vertices[vertIdx + 2] = pos3;


                tris[trisIdx + 0] = vertIdx + 0;
                tris[trisIdx + 1] = vertIdx + 1;
                tris[trisIdx + 2] = vertIdx + 2;

                uv.Add(new Vector3(0, 0, 1f * i * lodOffset));
                uv.Add(new Vector3(1, 0, 1f * i * lodOffset));
                uv.Add(new Vector3(1, 1, 1f * i * lodOffset));

                vertIdx += 3;
                trisIdx += 3;
            }

            var mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.SetUVs(0, uv);
            mesh.bounds = new Bounds(Vector3.zero, size);
            return mesh;
        }

        void UpdateShaderParameters()
        {
            var target1 = wavesRT1.rt != null ? wavesRT1.rt as Texture : waves_tex1;
            var target2 = wavesRT2.rt != null ? wavesRT2.rt as Texture : waves_tex2;
            var target3 = wavesDataRT1.rt != null ? wavesDataRT1.rt as Texture : wavesData1_tex;
            var target4 = wavesDataRT2.rt != null ? wavesDataRT2.rt as Texture : wavesData2_tex;

            WaterInstance.SetTextures(
                (KWS_ShaderConstants.ShorelineID.KW_BakedWaves1_UV_Angle_Alpha, target1),
                (KWS_ShaderConstants.ShorelineID.KW_BakedWaves2_UV_Angle_Alpha, target2),
                (KWS_ShaderConstants.ShorelineID.KW_BakedWaves1_TimeOffset_Scale, target3),
                (KWS_ShaderConstants.ShorelineID.KW_BakedWaves2_TimeOffset_Scale, target4),
                (ID_KW_ShorelineWaveDisplacement, shorelineWaveDisplacementTex),
                (ID_KW_ShorelineWaveNormal, shorelineWaveNormalTex));
            WaterInstance.SetFloats(
                (KWS_ShaderConstants.ShorelineID.KW_GlobalTimeOffsetMultiplier, GlobalTimeOffsetMultiplier),
                (KWS_ShaderConstants.ShorelineID.KW_GlobalTimeSpeedMultiplier, GlobalTimeSpeedMultiplier),
                (ID_KW_ShorelineMapSize, _lastShorelineAreaSize));
            WaterInstance.SetVectors((ID_KW_ShorelineAreaPos, _lastShorelinePos));

            if (depthParams != null)
            {
                WaterInstance.SetTextures((ID_KW_ShorelineDepth, depthRT.rt != null ? depthRT.rt as Texture : depth_tex));
                WaterInstance.SetFloats((ID_KW_ShorelineDepthOrthoSize, depthParams.OtrhograpicSize));
                WaterInstance.SetVectors((ID_KW_ShorelineDepthNearFarDistance, new Vector3(nearPlaneDepth, farPlaneDepth, farPlaneDepth - nearPlaneDepth)));
            }

        }

        void UpdateFoamParameters()
        {
            if (foamMaterial == null || foamShadowMaterial == null) return;

            foamMaterial.SetFloat(KWS_ShaderConstants.ShorelineID.KW_GlobalTimeOffsetMultiplier, GlobalTimeOffsetMultiplier);
            foamMaterial.SetFloat(KWS_ShaderConstants.ShorelineID.KW_GlobalTimeSpeedMultiplier, GlobalTimeSpeedMultiplier);

            if (depthParams != null)
            {
                foamMaterial.SetTexture(ID_KW_ShorelineDepth, depthRT.rt != null ? depthRT.rt as Texture : depth_tex);
                foamMaterial.SetFloat(ID_KW_ShorelineDepthOrthoSize, depthParams.OtrhograpicSize);
                foamMaterial.SetVector(ID_KW_ShorelineDepthNearFarDistance, new Vector3(nearPlaneDepth, farPlaneDepth, farPlaneDepth - nearPlaneDepth));
            }

            foamMaterial.SetTexture(ID_KW_ShorelineWaveDisplacement, shorelineWaveDisplacementTex);
            foamMaterial.SetTexture(ID_KW_ShorelineWaveNormal, shorelineWaveNormalTex);
            foamMaterial.SetFloat(ID_KW_ShorelineMapSize, _lastShorelineAreaSize);
            foamMaterial.SetVector(ID_KW_ShorelineAreaPos, _lastShorelinePos);
            //vatMaterial.SetTexture(ID_KWS_FoamParticle, _foamParticle);

            foamShadowMaterial.SetFloat(KWS_ShaderConstants.ShorelineID.KW_GlobalTimeOffsetMultiplier, GlobalTimeOffsetMultiplier);
            foamShadowMaterial.SetFloat(KWS_ShaderConstants.ShorelineID.KW_GlobalTimeSpeedMultiplier, GlobalTimeSpeedMultiplier);

            if (depthParams != null)
            {
                foamShadowMaterial.SetTexture(ID_KW_ShorelineDepth, depthRT.rt != null ? depthRT.rt as Texture : depth_tex);
                foamShadowMaterial.SetFloat(ID_KW_ShorelineDepthOrthoSize, depthParams.OtrhograpicSize);
                foamShadowMaterial.SetVector(ID_KW_ShorelineDepthNearFarDistance, new Vector3(nearPlaneDepth, farPlaneDepth, farPlaneDepth - nearPlaneDepth));
            }

            foamShadowMaterial.SetTexture(ID_KW_ShorelineWaveDisplacement, shorelineWaveDisplacementTex);
            foamShadowMaterial.SetTexture(ID_KW_ShorelineWaveNormal, shorelineWaveNormalTex);
            foamShadowMaterial.SetFloat(ID_KW_ShorelineMapSize, _lastShorelineAreaSize);
            foamShadowMaterial.SetVector(ID_KW_ShorelineAreaPos, _lastShorelinePos);

            var useWaterOffset = WaterInstance.WindSpeed > 3.01f;
            foamMaterial.SetKeyword(KWS_ShaderConstants.ShorelineKeywords.FOAM_COMPUTE_WATER_OFFSET, useWaterOffset);
            foamShadowMaterial.SetKeyword(KWS_ShaderConstants.ShorelineKeywords.FOAM_COMPUTE_WATER_OFFSET, useWaterOffset);
        }
    }
}