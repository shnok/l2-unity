// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;
using UnityEngine.Serialization;

namespace AtmosphericHeightFog
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class HeightFogGlobal : StyledMonoBehaviour
    {
        private static HeightFogGlobal _instance;
        public static HeightFogGlobal Instance { get { return _instance; } }

        [StyledBanner(0.55f, 0.75f, 1f, "Height Fog Global", "", "https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.kfvqsi6kusw4")]
        public bool styledBanner;

        [StyledCategory("Scene Settings", 5, 10)]
        public bool categoryScene;

        public Camera mainCamera;
        public Light mainDirectional;

        [StyledCategory("Preset Settings")]
        public bool categoryMode;

        public FogMode fogMode = FogMode.UseScriptSettings;

        [StyledMessage("Info", "The Preset feature requires a material using the BOXOPHOBIC > Atmospherics > Fog Preset shader.", 10, 0)]
        public bool messagePreset = false;

        [StyledMessage("Info", "The Time Of Day feature works by interpolating two Fog Preset materials using the BOXOPHOBIC > Atmospherics > Fog Preset shader. Please note that not all material properties can be interpolated properly!", 10, 0)]
        public bool messageTimeOfDay = false;

        [Space(10)]
        public Material presetMaterial;

        [Space(10)]
        public Material presetDay;
        public Material presetNight;

        [Space(10)]
        [Range(0, 1)]
        public float timeOfDay = 0;

        [StyledCategory("Fog Settings")]
        public bool categoryFog;

        [Range(0, 1)]
        public float fogIntensity = 1;

        [Space(10)]
        public FogAxisMode fogAxisMode = FogAxisMode.YAxis;
        public FogLayersMode fogLayersMode = FogLayersMode.MultiplyDistanceAndHeight;
        public FogCameraMode fogCameraMode = FogCameraMode.Perspective;

        [Space(10)]
        [FormerlySerializedAs("fogColor")]
        [ColorUsage(false, true)]
        public Color fogColorStart = new Color(0.5f, 0.75f, 1.0f, 1.0f);
        [ColorUsage(false, true)]
        public Color fogColorEnd = new Color(0.75f, 1f, 1.25f, 1.0f);
        [Range(0f, 1f)]
        public float fogColorDuo = 0;

        [Space(10)]
        public float fogDistanceStart = 0;
        public float fogDistanceEnd = 100;
        [Range(1, 8)]
        public float fogDistanceFalloff = 1;

        [Space(10)]
        public float fogHeightStart = 0;
        public float fogHeightEnd = 100;
        [Range(1f, 8f)]
        public float fogHeightFalloff = 1;

        [Space(10)]
        public float farDistanceHeight = 0;
        public float farDistanceOffset = 0;

        [StyledCategory("Skybox Settings")]
        public bool categorySkybox;

        [Range(0, 1)]
        public float skyboxFogIntensity = 1;
        [Range(0, 8)]
        public float skyboxFogHeight = 1;
        [Range(1, 8)]
        public float skyboxFogFalloff = 1;
        [Range(-1, 1)]
        public float skyboxFogOffset = 0;
        [Range(0, 1)]
        public float skyboxFogBottom = 0;
        [Range(0, 1)]
        public float skyboxFogFill = 0;

        [StyledCategory("Directional Settings")]
        public bool categoryDirectional;

        [Range(0, 1)]
        public float directionalIntensity = 1;
        [Range(1, 8)]
        public float directionalFalloff = 1;
        [ColorUsage(false, true)]
        public Color directionalColor = new Color(1f, 0.75f, 0.5f, 1f);

        [StyledCategory("Noise Settings")]
        public bool categoryNoise;

        [Range(0, 1)]
        public float noiseIntensity = 1;
        [Range(0, 1)]
        public float noiseMin = 0;
        [Range(0, 1)]
        public float noiseMax = 1;
        public float noiseScale = 30;
        public Vector3 noiseSpeed = new Vector3(0.5f, 0f, 0.5f);

        [Space(10)]
        public float noiseDistanceEnd = 200;

        [StyledCategory("Advanced Settings")]
        public bool categoryAdvanced;

        public float jitterIntensity = 0;
        public int renderPriority = 1;

        [Space(10)]
        public bool manualPositionAndScale = false;

        [StyledSpace(5)]
        public bool styledSpace0;

        Material localMaterial;
        Material blendMaterial;
        Material globalMaterial;
        Material missingMaterial;
        Material currentMaterial;

        [HideInInspector]
        public Material overrideMaterial;
        [HideInInspector]
        public float overrideCamToVolumeDistance = 1f;
        [HideInInspector]
        public float overrideVolumeDistanceFade = 0f;

        [HideInInspector]
        public int version = 0;

        void OnEnable()
        {
            _instance = this;

            gameObject.name = "Height Fog Global";

            if (!manualPositionAndScale)
            {
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.rotation = Quaternion.identity;
            }

            GetCamera();
            GetDirectional();

            if (mainCamera != null)
            {
                if (mainCamera.depthTextureMode != DepthTextureMode.Depth || mainCamera.depthTextureMode != DepthTextureMode.DepthNormals)
                {
                    mainCamera.depthTextureMode = DepthTextureMode.Depth;
                }
            }
            else
            {
                Debug.Log("[Atmospheric Height Fog] Camera not found! Make sure you have a camera in the scene or your camera has the MainCamera tag!");
            }

            var sphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

            gameObject.GetComponent<MeshFilter>().sharedMesh = sphereMesh;

            localMaterial = new Material(Shader.Find("BOXOPHOBIC/Atmospherics/Height Fog Preset"));
            localMaterial.name = "Local";

            overrideMaterial = new Material(localMaterial);
            overrideMaterial.name = "Override";

            blendMaterial = new Material(localMaterial);
            blendMaterial.name = "Blend";

            globalMaterial = new Material(Shader.Find("Hidden/BOXOPHOBIC/Atmospherics/Height Fog Global"));
            globalMaterial.name = "Height Fog Global";

            missingMaterial = Resources.Load<Material>("Height Fog Preset");

            gameObject.GetComponent<MeshRenderer>().sharedMaterial = globalMaterial;

            gameObject.GetComponent<MeshRenderer>().enabled = true;
            Shader.SetGlobalFloat("AHF_Enabled", 1);
        }

        void OnDisable()
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            Shader.SetGlobalFloat("AHF_Enabled", 0);
        }

        void Update()
        {
            if (mainCamera == null)
            {
                Debug.Log("[Atmospheric Height Fog] " + "Make sure you set scene camera tag to Main Camera for the fog to work!");
                return;
            }

            if (!manualPositionAndScale)
            {
                SetFogSphereSize();
                SetFogSpherePosition();
            }

            currentMaterial = localMaterial;

            if (fogMode == FogMode.UseScriptSettings)
            {
                SetLocalMaterial();

                messageTimeOfDay = false;
                messagePreset = false;
            }
            else if (fogMode == FogMode.UsePresetSettings)
            {
                if (presetMaterial != null && presetMaterial.HasProperty("_IsHeightFogPreset"))
                {
                    currentMaterial = presetMaterial;
                    messagePreset = false;
                }
                else
                {
                    currentMaterial = missingMaterial;
                    messagePreset = true;
                }

                messageTimeOfDay = false;
            }
            else if (fogMode == FogMode.UseTimeOfDay)
            {
                if (presetDay != null && presetDay.HasProperty("_IsHeightFogPreset") && presetNight != null && presetNight.HasProperty("_IsHeightFogPreset"))
                {
                    currentMaterial.Lerp(presetDay, presetNight, timeOfDay);
                    messageTimeOfDay = false;
                }
                else
                {
                    currentMaterial = missingMaterial;
                    messageTimeOfDay = true;
                }

                messagePreset = false;
            }

            if (mainDirectional != null)
            {
                currentMaterial.SetVector("_DirectionalDir", -mainDirectional.transform.forward);
            }
            else
            {
                currentMaterial.SetVector("_DirectionalDir", Vector4.zero);
            }

            if (overrideCamToVolumeDistance > overrideVolumeDistanceFade)
            {
                blendMaterial.CopyPropertiesFromMaterial(currentMaterial);
            }
            else if (overrideCamToVolumeDistance < overrideVolumeDistanceFade)
            {
                var lerp = 1 - (overrideCamToVolumeDistance / overrideVolumeDistanceFade);
                blendMaterial.Lerp(currentMaterial, overrideMaterial, lerp);
            }

            SetGlobalMaterials();
            SetRenderQueue();
        }

        void GetCamera()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        void GetDirectional()
        {
            if (mainDirectional == null)
            {
                var allLights = FindObjectsOfType<Light>();
                var intensity = 0.0f;

                for (int i = 0; i < allLights.Length; i++)
                {
                    if (allLights[i].type == LightType.Directional)
                    {
                        if (allLights[i].intensity > intensity)
                        {
                            mainDirectional = allLights[i];
                        }
                    }
                }
            }
        }

        void SetLocalMaterial()
        {
            localMaterial.SetFloat("_FogIntensity", fogIntensity);

            localMaterial.SetColor("_FogColorStart", fogColorStart);
            localMaterial.SetColor("_FogColorEnd", fogColorEnd);
            localMaterial.SetFloat("_FogColorDuo", fogColorDuo);

            localMaterial.SetFloat("_FogDistanceStart", fogDistanceStart);
            localMaterial.SetFloat("_FogDistanceEnd", fogDistanceEnd);
            localMaterial.SetFloat("_FogDistanceFalloff", fogDistanceFalloff);

            localMaterial.SetFloat("_FogHeightStart", fogHeightStart);
            localMaterial.SetFloat("_FogHeightEnd", fogHeightEnd);
            localMaterial.SetFloat("_FogHeightFalloff", fogHeightFalloff);

            localMaterial.SetFloat("_FarDistanceHeight", farDistanceHeight);
            localMaterial.SetFloat("_FarDistanceOffset", farDistanceOffset);

            localMaterial.SetFloat("_SkyboxFogIntensity", skyboxFogIntensity);
            localMaterial.SetFloat("_SkyboxFogHeight", skyboxFogHeight);
            localMaterial.SetFloat("_SkyboxFogFalloff", skyboxFogFalloff);
            localMaterial.SetFloat("_SkyboxFogOffset", skyboxFogOffset);
            localMaterial.SetFloat("_SkyboxFogBottom", skyboxFogBottom);
            localMaterial.SetFloat("_SkyboxFogFill", skyboxFogFill);

            localMaterial.SetFloat("_DirectionalIntensity", directionalIntensity);
            localMaterial.SetFloat("_DirectionalFalloff", directionalFalloff);
            localMaterial.SetColor("_DirectionalColor", directionalColor);

            localMaterial.SetFloat("_NoiseIntensity", noiseIntensity);
            localMaterial.SetFloat("_NoiseMin", noiseMin);
            localMaterial.SetFloat("_NoiseMax", noiseMax);
            localMaterial.SetFloat("_NoiseScale", noiseScale);
            localMaterial.SetVector("_NoiseSpeed", noiseSpeed);
            localMaterial.SetFloat("_NoiseDistanceEnd", noiseDistanceEnd);

            localMaterial.SetFloat("_JitterIntensity", jitterIntensity);

            if (fogAxisMode == FogAxisMode.XAxis)
            {
                localMaterial.SetVector("_FogAxisOption", new Vector4(1, 0, 0, 0));
            }
            else if (fogAxisMode == FogAxisMode.YAxis)
            {
                localMaterial.SetVector("_FogAxisOption", new Vector4(0, 1, 0, 0));
            }
            else if (fogAxisMode == FogAxisMode.ZAxis)
            {
                localMaterial.SetVector("_FogAxisOption", new Vector4(0, 0, 1, 0));
            }

            if (fogLayersMode == FogLayersMode.MultiplyDistanceAndHeight)
            {
                localMaterial.SetFloat("_FogLayersMode", 0.0f);
            }
            else
            {
                localMaterial.SetFloat("_FogLayersMode", 1.0f);
            }

            if (fogCameraMode == FogCameraMode.Perspective)
            {
                localMaterial.SetFloat("_FogCameraMode", 0.0f);
            }
            else if (fogCameraMode == FogCameraMode.Orthographic)
            {
                localMaterial.SetFloat("_FogCameraMode", 1.0f);
            }
            else if (fogCameraMode == FogCameraMode.Both)
            {
                localMaterial.SetFloat("_FogCameraMode", 2.0f);
            }
        }

        void SetGlobalMaterials()
        {
            if (blendMaterial.HasProperty("_IsHeightFogPreset") == false)
            {
                return;
            }

            Shader.SetGlobalFloat("AHF_FogIntensity", blendMaterial.GetFloat("_FogIntensity"));

            Shader.SetGlobalVector("AHF_FogAxisOption", blendMaterial.GetVector("_FogAxisOption"));
            Shader.SetGlobalFloat("AHF_FogLayersMode", blendMaterial.GetFloat("_FogLayersMode"));

            Shader.SetGlobalColor("AHF_FogColorStart", blendMaterial.GetColor("_FogColorStart"));
            Shader.SetGlobalColor("AHF_FogColorEnd", blendMaterial.GetColor("_FogColorEnd"));
            Shader.SetGlobalFloat("AHF_FogColorDuo", blendMaterial.GetFloat("_FogColorDuo"));

            Shader.SetGlobalFloat("AHF_FogDistanceStart", blendMaterial.GetFloat("_FogDistanceStart"));
            Shader.SetGlobalFloat("AHF_FogDistanceEnd", blendMaterial.GetFloat("_FogDistanceEnd"));
            Shader.SetGlobalFloat("AHF_FogDistanceFalloff", blendMaterial.GetFloat("_FogDistanceFalloff"));

            Shader.SetGlobalFloat("AHF_FogHeightStart", blendMaterial.GetFloat("_FogHeightStart"));
            Shader.SetGlobalFloat("AHF_FogHeightEnd", blendMaterial.GetFloat("_FogHeightEnd"));
            Shader.SetGlobalFloat("AHF_FogHeightFalloff", blendMaterial.GetFloat("_FogHeightFalloff"));

            Shader.SetGlobalFloat("AHF_FarDistanceHeight", blendMaterial.GetFloat("_FarDistanceHeight"));
            Shader.SetGlobalFloat("AHF_FarDistanceOffset", blendMaterial.GetFloat("_FarDistanceOffset"));

            Shader.SetGlobalFloat("AHF_SkyboxFogIntensity", blendMaterial.GetFloat("_SkyboxFogIntensity"));
            Shader.SetGlobalFloat("AHF_SkyboxFogHeight", blendMaterial.GetFloat("_SkyboxFogHeight"));
            Shader.SetGlobalFloat("AHF_SkyboxFogFalloff", blendMaterial.GetFloat("_SkyboxFogFalloff"));
            Shader.SetGlobalFloat("AHF_SkyboxFogOffset", blendMaterial.GetFloat("_SkyboxFogOffset"));
            Shader.SetGlobalFloat("AHF_SkyboxFogBottom", blendMaterial.GetFloat("_SkyboxFogBottom"));
            Shader.SetGlobalFloat("AHF_SkyboxFogFill", blendMaterial.GetFloat("_SkyboxFogFill"));

            Shader.SetGlobalVector("AHF_DirectionalDir", blendMaterial.GetVector("_DirectionalDir"));
            Shader.SetGlobalFloat("AHF_DirectionalIntensity", blendMaterial.GetFloat("_DirectionalIntensity"));
            Shader.SetGlobalFloat("AHF_DirectionalFalloff", blendMaterial.GetFloat("_DirectionalFalloff"));
            Shader.SetGlobalColor("AHF_DirectionalColor", blendMaterial.GetColor("_DirectionalColor"));

            Shader.SetGlobalFloat("AHF_NoiseIntensity", blendMaterial.GetFloat("_NoiseIntensity"));
            Shader.SetGlobalFloat("AHF_NoiseMin", blendMaterial.GetFloat("_NoiseMin"));
            Shader.SetGlobalFloat("AHF_NoiseMax", blendMaterial.GetFloat("_NoiseMax"));
            Shader.SetGlobalFloat("AHF_NoiseScale", blendMaterial.GetFloat("_NoiseScale"));
            Shader.SetGlobalVector("AHF_NoiseSpeed", blendMaterial.GetVector("_NoiseSpeed"));
            Shader.SetGlobalFloat("AHF_NoiseDistanceEnd", blendMaterial.GetFloat("_NoiseDistanceEnd"));

            Shader.SetGlobalFloat("AHF_JitterIntensity", blendMaterial.GetFloat("_JitterIntensity"));

            var cameraMode = blendMaterial.GetInt("_FogCameraMode");

            if (cameraMode == 0)
            {
                Shader.EnableKeyword("AHF_CAMERAMODE_PERSPECTIVE");
                Shader.DisableKeyword("AHF_CAMERAMODE_ORTHOGRAPHIC");
                Shader.DisableKeyword("AHF_CAMERAMODE_BOTH");
            }
            else if (cameraMode == 1)
            {
                Shader.DisableKeyword("AHF_CAMERAMODE_PERSPECTIVE");
                Shader.EnableKeyword("AHF_CAMERAMODE_ORTHOGRAPHIC");
                Shader.DisableKeyword("AHF_CAMERAMODE_BOTH");
            }
            else if (cameraMode == 2)
            {
                Shader.DisableKeyword("AHF_CAMERAMODE_ORTHOGRAPHIC");
                Shader.DisableKeyword("AHF_CAMERAMODE_PERSPECTIVE");
                Shader.EnableKeyword("AHF_CAMERAMODE_BOTH");
            }
        }

        void SetFogSphereSize()
        {
            var cameraFar = mainCamera.farClipPlane - 1;
            gameObject.transform.localScale = new Vector3(cameraFar, cameraFar, cameraFar);
        }

        void SetFogSpherePosition()
        {
            transform.position = mainCamera.transform.position;
        }

        void SetRenderQueue()
        {
            globalMaterial.renderQueue = 3000 + renderPriority;
        }
    }
}

