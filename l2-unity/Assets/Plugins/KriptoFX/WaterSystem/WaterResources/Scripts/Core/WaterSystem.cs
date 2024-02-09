using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace KWS
{
    [ExecuteAlways]
    [Serializable]
    public partial class WaterSystem : MonoBehaviour
    {
        //todo add KWS_STANDARD/KWS_HDRP/KWS_URP

        #region public variables

        //Color settings
        public bool ShowColorSettings = true;
        public float Transparent = 10;
        public Color WaterColor = new Color(175 / 255.0f, 225 / 255.0f, 240 / 255.0f);
        public Color TurbidityColor = new Color(10 / 255.0f, 110 / 255.0f, 100 / 255.0f);
        public float Turbidity = 0.08f;


        //Waves settings
        public bool ShowWaves;
        public FFT_GPU.SizeSetting FFT_SimulationSize = FFT_GPU.SizeSetting.Size_256;
        public bool UseMultipleSimulations = false;
        public float WindSpeed = 1.5f;
        public float WindRotation = 0;
        public float WindTurbulence = 0.75f;
        public float TimeScale = 1;


        //Reflection settings
        public bool ShowReflectionSettings;
        public ReflectionModeEnum ReflectionMode = ReflectionModeEnum.ScreenSpaceReflection;
        public float CubemapUpdateInterval = 6;
        public int CubemapCullingMask = 0;
        public CubemapReflectionResolutionQualityEnum CubemapReflectionResolutionQuality = CubemapReflectionResolutionQualityEnum.High;
        public PlanarReflectionResolutionQualityEnum PlanarReflectionResolutionQuality = PlanarReflectionResolutionQualityEnum.Medium;
        public ScreenSpaceReflectionResolutionQualityEnum ScreenSpaceReflectionResolutionQuality = ScreenSpaceReflectionResolutionQualityEnum.High;
        public ReflectionClearFlagEnum ReflectionClearFlag = ReflectionClearFlagEnum.Skybox;
        public Color ReflectionClearColor = Color.black;
        public float ReflectionClipPlaneOffset = 0.002f;
        public int ReflectioDepthHolesFillDistance = 5;

        public bool UseAnisotropicReflections = false;
        public bool AnisotropicReflectionsHighQuality = false;
        public float AnisotropicReflectionsScale = 1.0f;

        public bool ReflectSun = true;
        public float ReflectedSunCloudinessStrength = 0.04f;
        public float ReflectedSunStrength = 1.0f;

        //Refraction settings
        public bool ShowRefractionSettings;
        public RefractionModeEnum RefractionMode = RefractionModeEnum.PhysicalAproximationIOR;
        public float RefractionAproximatedDepth = 2f;
        public float RefractionSimpleStrength = 0.25f;
        public bool UseRefractionDispersion = true;
        public float RefractionDispersionStrength = 0.35f;

        //Volumetric settings
        public bool UseVolumetricLight = false;
        public bool ShowVolumetricLightSettings;
        public VolumetricLightResolutionQualityEnum VolumetricLightResolutionQuality = VolumetricLightResolutionQualityEnum.High;
        public int VolumetricLightIteration = 4;
        public float VolumetricLightBlurRadius = 2.0f;
        public VolumetricLightFilterEnum VolumetricLightFilter = VolumetricLightFilterEnum.Bilateral;


        //FlowMap settings
        public bool UseFlowMap = false;
        public bool ShowFlowMap;
        public bool FlowMapInEditMode = false;
        public Vector3 FlowMapAreaPosition = new Vector3(0, 0, 0);
        public int FlowMapAreaSize = 200;
        public FlowmapTextureResolutionEnum FlowMapTextureResolution = FlowmapTextureResolutionEnum._2048;
        public float FlowMapBrushStrength = 0.75f;
        public float FlowMapSpeed = 1;
        public bool UseFluidsSimulation = false;
        public int FluidsAreaSize = 40;


        //public int FluidsObstacleAreaSize = 400;
        public int FluidsSimulationIterrations = 2;
        public int FluidsTextureSize = 1024;
        public int FluidsSimulationFPS = 60;
        public float FluidsSpeed = 1;
        public float FluidsFoamStrength = 0.5f;


        //Dynamic waves settings
        public bool UseDynamicWaves = false;
        public bool ShowDynamicWaves;
        public int DynamicWavesAreaSize = 25;
        public int DynamicWavesSimulationFPS = 60;
        public int DynamicWavesResolutionPerMeter = 40;
        public float DynamicWavesPropagationSpeed = 0.5f;
        public bool UseDynamicWavesRainEffect;
        public float DynamicWavesRainStrength = 0.2f;


        //Shoreline settings
        public bool UseShorelineRendering = false;
        public bool ShowShorelineMap;
        public QualityEnum FoamLodQuality = QualityEnum.Medium;
        public bool FoamCastShadows = true;
        public bool FoamReceiveShadows = false;
        public bool ShorelineInEditMode = false;
        public Vector3 ShorelineAreaPosition;
        public int ShorelineAreaSize = 512;
        public QualityEnum ShorelineCurvedSurfacesQuality = QualityEnum.Medium;


        //Caustic settings
        public bool UseCausticEffect = false;
        public bool ShowCausticEffectSettings;
        public bool UseCausticBicubicInterpolation = true;
        public bool UseCausticDispersion = true;
        public int CausticTextureSize = 768;
        public int CausticMeshResolution = 320;
        public int CausticActiveLods = 4;
        public float CausticStrength = 1;
        public bool UseDepthCausticScale;
        public bool CausticDepthScaleInEditMode = false;
        public float CausticDepthScale = 1;
        public Vector3 CausticOrthoDepthPosition = Vector3.positiveInfinity;
        public int CausticOrthoDepthAreaSize = 512;
        public int CausticOrthoDepthTextureResolution = 2048;


        //Underwater settings
        public bool UseUnderwaterEffect = true;
        public bool ShowUnderwaterEffectSettings;
        //public bool                     UseHighQualityUnderwater = false;
        public bool UseUnderwaterBlur = true;
        public UnderwaterResolutionQualityEnum UnderwaterResolutionQuality = UnderwaterResolutionQualityEnum.High;
        public float UnderwaterBlurRadius = 1.0f;


        //Rendering settings
        public bool ShowRendering;
        public WaterMeshTypeEnum WaterMeshType;
        public Mesh CustomMesh;
        public bool UseFiltering = true;
        public bool UseAnisotropicFiltering = false;
        public bool UseOffscreenRendering;
        public AntialiasingEnum OffscreenRenderingAntialiasing = AntialiasingEnum.None;
        public OffscreenResolutionQualityEnum OffscreenResolutionQuality = OffscreenResolutionQualityEnum.High;
        public bool DrawToPosteffectsDepth;
        public int MeshQuality = 10;
        public Vector3 MeshSize = new Vector3(10, 10, 10);
        public bool UseTesselation = true;
        public float TesselationFactor = 0.6f;
        public float TesselationMaxDistance = 1000f;

        #endregion

        #region public enums

        public enum QualityEnum
        {
            High = 0,
            Medium = 1,
            Low = 2,
        }

        public enum ReflectionModeEnum
        {
            CubemapReflection,
            PlanarReflection,
            ScreenSpaceReflection,
        }

        public enum PlanarReflectionResolutionQualityEnum
        {
            Ultra = 768,
            High = 512,
            Medium = 368,
            Low = 256,
            VeryLow = 128,
        }

        /// <summary>
        /// Resolution quality in percent relative to current screen size. For example Medium quality = 35, it's mean ScreenSize * (35 / 100)
        /// </summary>
        public enum ScreenSpaceReflectionResolutionQualityEnum
        {
            Ultra = 75,
            High = 50,
            Medium = 35,
            Low = 25,
            VeryLow = 15,
        }

        public enum CubemapReflectionResolutionQualityEnum
        {
            Ultra = 512,
            High = 256,
            Medium = 128,
            Low = 64,
        }

        public enum ReflectionClearFlagEnum
        {
            Skybox,
            Color
        }


        public enum RefractionModeEnum
        {
            Simple,
            PhysicalAproximationIOR
        }

        public enum FoamShadowMode
        {
            None,
            CastOnly,
            CastAndReceive
        }

        public enum WaterMeshTypeEnum
        {
            Infinite,
            Finite,
            CustomMesh
        }

        public enum AntialiasingEnum
        {
            None = 1,
            MSAA2x = 2,
            MSAA4x = 4,
            MSAA8x = 8
        }

        public enum VolumetricLightFilterEnum
        {
            Bilateral,
            Gaussian
        }

        public enum VolumetricLightResolutionQualityEnum
        {
            Ultra = 75,
            High = 50,
            Medium = 35,
            Low = 25,
            VeryLow = 15,
        }

        public enum FlowmapTextureResolutionEnum
        {
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096,
            _8192 = 8192
        }

        public enum UnderwaterResolutionQualityEnum
        {
            High = 50,
            Medium = 35,
            Low = 25,
            VeryLow = 15,
        }

        public enum OffscreenResolutionQualityEnum
        {
            Ultra = 85,
            High = 75,
            Medium = 65,
            Low = 50,
            VeryLow = 40,
        }

        #endregion

        #region public API methods

        public event Action OnWaterRelease;

        /// <summary>
        /// World space bounds of the rendered mesh
        /// </summary>
        /// <returns></returns>
        public Bounds GetWorldSpaceBounds()
        {
            return (_waterMeshRenderer != null) ? _waterMeshRenderer.bounds : new Bounds(_waterTransform.position, Vector3.one);
        }

        /// <summary>
        /// Get world space water position/normal at point. Used for water physics. 
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public KW_WaterSurfaceData GetWaterSurfaceData(Vector3 worldPosition)
        {
            if (!isWaterCommonResourcesInitialized || !isBuoyancyDataReadCompleted) return new KW_WaterSurfaceData { IsActualDataReady = false, Position = worldPosition, Normal = Vector3.up };
            return fft_HeightData.GetWaterSurfaceData(worldPosition);
        }

        #endregion

        #region public internal variables

        public string WaterGUID
        {
            get
            {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(_waterGUID)) _waterGUID = UnityEditor.GUID.Generate().ToString();
                return _waterGUID;
#else
            if (string.IsNullOrEmpty(_waterGUID)) Debug.LogError("Water GUID is empty ");
            Debug.Log("Water GUID is empty " + _waterGUID);
            return _waterGUID;
#endif
            }
        }

        public void AddMaterialToWaterRendering(Material additionalMaterial)
        {
            if (additionalMaterial == null || waterSharedMaterials.Contains(additionalMaterial)) return;
            waterSharedMaterials.Add(additionalMaterial);
        }

        public void RemoveMaterialFromWaterRendering(Material additionalMaterial)
        {
            if (additionalMaterial == null || !waterSharedMaterials.Contains(additionalMaterial)) return;
            waterSharedMaterials.Remove(additionalMaterial);
        }

        public List<Material> GetWaterRenderingMaterials()
        {
            return waterSharedMaterials;
        }

        public Vector3 WaterMeshWorldPosition => WaterMeshTransform != null ? WaterMeshTransform.position : transform.position;
        public Vector3 WaterWorldPosition => _waterTransform.position;
        public Material WaterMaterial { get; private set; }
        public Mesh WaterMesh { get; private set; }
        public GameObject WaterMeshGameObject { get; private set; }
        public Transform WaterMeshTransform { get; private set; }

        public static List<WaterSystem> ActiveWaterInstances { get; private set; } = new List<WaterSystem>();
        public static bool IsRTHandleInitialized;

        public bool IsWaterVisible { get; private set; }

        #endregion

        #region private variables

        [SerializeField] string _waterGUID;

#if KWS_DEBUG
        public Vector4 Test4 = Vector4.zero;
#endif

        private Camera _currentCamera;
        private GameObject _tempGameObject;
        private Transform _waterTransform;
        private MeshRenderer _waterMeshRenderer;
        private MeshFilter _waterMeshFilter;

        private bool isWaterCommonResourcesInitialized;
        private bool isWaterPlatformSpecificResourcesInitialized;
        private bool isBuoyancyDataReadCompleted;
        private bool isMultipleFFTSimInitialized;

        List<Material> waterSharedMaterials = new List<Material>();

        #endregion

        #region properties

        private FFT_GPU _fft_lod0;

        private FFT_GPU fft_lod0
        {
            get
            {
                if (_fft_lod0 == null) _fft_lod0 = _tempGameObject.AddComponent<FFT_GPU>();
                return _fft_lod0;
            }
        }

        private FFT_GPU _fft_lod1;

        private FFT_GPU fft_lod1
        {
            get
            {
                if (_fft_lod1 == null) _fft_lod1 = _tempGameObject.AddComponent<FFT_GPU>();
                return _fft_lod1;
            }
        }

        private FFT_GPU _fft_lod2;

        private FFT_GPU fft_lod2
        {
            get
            {
                if (_fft_lod2 == null) _fft_lod2 = _tempGameObject.AddComponent<FFT_GPU>();
                return _fft_lod2;
            }
        }

        private KWS_FFT_ToHeightMap _fft_HeightData;

        private KWS_FFT_ToHeightMap fft_HeightData
        {
            get
            {
                if (_fft_HeightData == null)
                {
                    _fft_HeightData = _tempGameObject.AddComponent<KWS_FFT_ToHeightMap>();
                    _fft_HeightData.WaterInstance = this;
                    _fft_HeightData.IsDataReadCompleted += () => isBuoyancyDataReadCompleted = true;
                }

                return _fft_HeightData;
            }
        }

        private KW_ShorelineWaves _shorelineWaves;

        private KW_ShorelineWaves shorelineWaves
        {
            get
            {
                if (_shorelineWaves == null)
                {
                    _shorelineWaves = _tempGameObject.AddComponent<KW_ShorelineWaves>();
                    _shorelineWaves.WaterInstance = this;
                }

                return _shorelineWaves;
            }
        }

        private KW_FlowMap _flowMap;

        KW_FlowMap flowMap
        {
            get
            {
                if (_flowMap == null)
                {
                    _flowMap = _tempGameObject.AddComponent<KW_FlowMap>();
                    _flowMap.WaterInstance = this;
                }

                return _flowMap;
            }
        }

        private KW_FluidsSimulation2D _fluidsSimulation;

        KW_FluidsSimulation2D fluidsSimulation
        {
            get
            {
                if (_fluidsSimulation == null)
                {
                    _fluidsSimulation = _tempGameObject.AddComponent<KW_FluidsSimulation2D>();
                    _fluidsSimulation.WaterInstance = this;
                }

                return _fluidsSimulation;
            }
        }

        private KW_DynamicWaves _dynamicWaves;

        KW_DynamicWaves dynamicWaves
        {
            get
            {
                if (_dynamicWaves == null)
                {
                    _dynamicWaves = _tempGameObject.AddComponent<KW_DynamicWaves>();
                    _dynamicWaves.WaterInstance = this;
                }

                return _dynamicWaves;
            }
        }

        #endregion


        KW_Extensions.AsyncInitializingStatusEnum shoreLineInitializingStatus;
        KW_Extensions.AsyncInitializingStatusEnum flowmapInitializingStatus;
        KW_Extensions.AsyncInitializingStatusEnum fluidsSimInitializingStatus;

        private const int waterLayer = 4; //water layer

        private const float DomainSize = 10f;
        private const float DomainSize_LOD1 = 40f;
        private const float DomainSize_LOD2 = 160f;


        private const float MaxTesselationFactor = 10;
        const int BakeFluidsLimitFrames = 200;
        int currentBakeFluidsFrames = 0;
        private const int DepthMaskTextureHeightLimit = 540; //fullHD * 0.5 enough even for 4k

        //bool isDynamicWavesInvoreRepeatingStarted;
        //bool isFluidsInvoreRepeatingStarted;
        KW_CustomFixedUpdate fixedUpdateFluids;
        KW_CustomFixedUpdate fixedUpdateBakingFluids;
        KW_CustomFixedUpdate fixedUpdateDynamicWaves;
        private bool _lastWaterVisible;


#if UNITY_EDITOR
        [MenuItem("GameObject/Effects/Water System")]
        static void CreateWaterSystemEditor(MenuCommand menuCommand)
        {
            var go = new GameObject("Water System");
            go.transform.position = SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 3f);
            go.AddComponent<WaterSystem>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif

        private void Awake()
        {
            _waterTransform = transform;

            var screenSize = KWS_CoreUtils.GetScreenSizeLimited();
            if (!WaterSystem.IsRTHandleInitialized) KWS_RTHandles.Initialize(screenSize.x, screenSize.y, false, KWS.MSAASamples.None);
        }

        private void OnEnable()
        {
            SubscribeBeforeCameraRendering();
            SubscribeAfterCameraRendering();
        }

        void OnDisable()
        {
            if (ActiveWaterInstances.Contains(this)) ActiveWaterInstances.Remove(this);
            _lastWaterVisible = false;

            UnsubscribeBeforeCameraRendering();
            UnsubscribeAfterCameraRendering();

            ReleasePlatformSpecificResources();
            ReleaseCommonResources();

            OnWaterRelease?.Invoke();
        }


        void OnBeforeCameraRendering(Camera cam)
        {
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView) return;

            if (isWaterCommonResourcesInitialized)
            {
                IsWaterVisible = IsWaterVisibleForCamera(cam);
                UpdateActiveWaterInstancesInfo();
                if (!IsWaterVisible) return;
            }

#if UNITY_EDITOR
            KW_Extensions.UpdateEditorDeltaTime();
#endif
            _currentCamera = cam;

            Profiler.BeginSample("Water.Rendering");
            RenderWater();
            Profiler.EndSample();
        }

        void OnAfterCameraRendering(Camera cam)
        {
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView) return;

#if UNITY_EDITOR
            KW_Extensions.SetEditorDeltaTime();
#endif
        }


        public void EnableBuoyancyRendering()
        {
            isBuoyancyDataReadCompleted = false;
        }

        public void DisableBuoyancyRendering()
        {
            isBuoyancyDataReadCompleted = false;
        }
    }
}