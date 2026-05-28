// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;
using UnityEngine.Serialization;

namespace AtmosphericHeightFog
{
    [ExecuteInEditMode]
    [HelpURL("https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.hd5jt8lucuqq")]
    public class HeightFogOverride : StyledMonoBehaviour
    {
        [StyledBanner(0.55f, 0.75f, 1f, "Height Fog Override", "", "https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.hd5jt8lucuqq")]
        public bool styledBanner;

        [StyledMessage("Info", "The Height Fog Global object is missing from your scene! Please add it before using the Height Fog Override component!", 5, 0)]
        public bool messageNoHeightFogGlobal = false;

        [StyledCategory("Volume Settings", 5, 10)]
        public bool categoryVolume;

        public float volumeDistanceFade = 3;
        public Color volumeGizmoColor = Color.white;

        [StyledCategory("Scene Settings")]
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
        public Color fogColorStart = new Color(0.5f, 0.75f, 0.0f, 1.0f);
        [ColorUsage(false, true)]
        public Color fogColorEnd = new Color(0.75f, 1f, 0.0f, 1.0f);
        [Range(0, 1)]
        public float fogColorDuo = 0;

        [Space(10)]
        public float fogDistanceStart = -100;
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
        [Range(0, 1)]
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

        [StyledSpace(5)]
        public bool styledSpace0;

        Material localMaterial;
        Material missingMaterial;
        Material currentMaterial;
        Collider volumeCollider;
        HeightFogGlobal globalFog = null;
        bool distanceSent = false;

        [HideInInspector]
        public int version = 0;

        void Start()
        {
            volumeCollider = GetComponent<Collider>();

            if (volumeCollider == null)
            {
                Debug.Log("[Atmospheric Height Fog] Please create override volumes from the GameObject menu > BOXOPHOBIC > Atmospheric Height Fog > Override!");
                DestroyImmediate(this);
            }

            if (GameObject.Find("Height Fog Global") != null)
            {
                GameObject globalFogGO = GameObject.Find("Height Fog Global");
                globalFog = globalFogGO.GetComponent<HeightFogGlobal>();

                messageNoHeightFogGlobal = false;
            }
            else
            {
                messageNoHeightFogGlobal = true;
            }

            GetDirectional();

            localMaterial = new Material(Shader.Find("BOXOPHOBIC/Atmospherics/Height Fog Preset"));
            localMaterial.name = "Local";

            missingMaterial = Resources.Load<Material>("Height Fog Preset");

            SetLocalMaterial();
        }

        void OnDisable()
        {
            if (globalFog != null)
            {
                globalFog.overrideCamToVolumeDistance = 1;
                globalFog.overrideVolumeDistanceFade = 0;
            }
        }

        void OnDestroy()
        {
            if (globalFog != null)
            {
                globalFog.overrideCamToVolumeDistance = 1;
                globalFog.overrideVolumeDistanceFade = 0;
            }
        }

        void Update()
        {
            GetCamera();

            if (mainCamera == null || globalFog == null)
            {
                return;
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

            Vector3 camPos = mainCamera.transform.position;
            Vector3 closestPos = volumeCollider.ClosestPoint(camPos);

            float dist = Vector3.Distance(camPos, closestPos);

            if (dist > volumeDistanceFade && distanceSent == false)
            {
                globalFog.overrideCamToVolumeDistance = Mathf.Infinity;
                distanceSent = true;
            }
            else if (dist < volumeDistanceFade)
            {
                globalFog.overrideMaterial = currentMaterial;
                globalFog.overrideCamToVolumeDistance = dist;
                globalFog.overrideVolumeDistanceFade = volumeDistanceFade;
                distanceSent = false;
            }
        }

        void OnDrawGizmos()
        {
            if (volumeCollider == null)
            {
                return;
            }

            var color = volumeGizmoColor;
            var mul = 1.0f;

            if (volumeCollider.GetType() == typeof(BoxCollider))
            {
                var col = GetComponent<BoxCollider>();

                Gizmos.color = new Color(color.r * mul, color.g * mul, color.b * mul, color.a);
                Gizmos.DrawWireCube(transform.position, new Vector3(transform.lossyScale.x * col.size.x, transform.lossyScale.y * col.size.y, transform.lossyScale.z * col.size.z));

                Gizmos.color = new Color(color.r * mul, color.g * mul, color.b * mul, color.a * 0.5f);
                Gizmos.DrawWireCube(transform.position, new Vector3(transform.lossyScale.x * col.size.x + (volumeDistanceFade * 2), transform.lossyScale.y * col.size.y + (volumeDistanceFade * 2), transform.lossyScale.z * col.size.z + (volumeDistanceFade * 2)));
            }
            else
            {
                var col = GetComponent<SphereCollider>();
                var scale = Mathf.Max(Mathf.Max(gameObject.transform.localScale.x, gameObject.transform.localScale.y), gameObject.transform.localScale.z);

                Gizmos.color = new Color(color.r * mul, color.g * mul, color.b * mul, color.a);
                Gizmos.DrawWireSphere(transform.position, col.radius * scale);

                Gizmos.color = new Color(color.r * mul, color.g * mul, color.b * mul, color.a * 0.5f);
                Gizmos.DrawWireSphere(transform.position, col.radius * scale + volumeDistanceFade);
            }
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
            localMaterial.SetFloat("_SkyboxFogBottom", skyboxFogFill);
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
    }
}

