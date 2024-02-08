using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;


namespace KWS
{
    [ExecuteAlways]
    public partial class KWS_CameraReflection : MonoBehaviour
    {
        public WaterSystem WaterInstance;

        bool _isCanUpdate;
        private GameObject _reflCameraGo;
        private Camera _reflectionCamera;
        RenderTexture _planarRT;
        RenderTexture _planarRT_MipOrFiltered;

        RenderTexture _cubemapRT_Side;
        Dictionary<Camera, RenderTexture> _cubemapRTBuffers = new Dictionary<Camera, RenderTexture>();
        private Material _filteringMaterial;
        private CommandBuffer cmd;

        const float m_ClipPlaneOffset = -0.025f;
        const float m_planeOffset = -0.025f;

        private float currentInterval = 0;
        private int sideIdx = 0;
        private int[] cubeMapSides = new int[]
        {
        0, 1, 2, 4, 5
        };

        bool requiredUpdateAllFaces = true;
        int waterCullingMask = ~(1 << 4);

        private bool _isPlanarTextureInitialized;
        private bool _isCubemapTextureInitialized;


        private void OnEnable()
        {
            _isCanUpdate = true;

            SubscribeBeforeCameraRendering();
            SubscribeAfterCameraRendering();
        }


        void OnDisable()
        {
            _isCanUpdate = false;

            UnsubscribeBeforeCameraRendering();
            UnsubscribeAfterCameraRendering();

            Release();
        }


        void OnBeforeCameraRendering(Camera cam)
        {
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView) return;

            if (!_isCanUpdate) return;

            if (WaterInstance.ReflectionMode == WaterSystem.ReflectionModeEnum.PlanarReflection)
                RenderPlanar(cam, transform.position, WaterInstance.ReflectionClearFlag, WaterInstance.ReflectionClearColor);

            if (WaterInstance.ReflectionMode == WaterSystem.ReflectionModeEnum.CubemapReflection || WaterInstance.ReflectionMode == WaterSystem.ReflectionModeEnum.ScreenSpaceReflection)
                RenderCubemap(cam, transform.position, WaterInstance.CubemapUpdateInterval, WaterInstance.CubemapCullingMask, WaterInstance.ReflectionClearFlag, WaterInstance.ReflectionClearColor);
        }

        void OnAfterCameraRendering(Camera cam)
        {

        }


        void ReleaseTextures()
        {
            _planarRT?.Release();
            _planarRT_MipOrFiltered?.Release();
            _cubemapRT_Side?.Release();
            KW_Extensions.SafeDestroy(_planarRT, _planarRT_MipOrFiltered, _cubemapRT_Side);
            _planarRT = _planarRT_MipOrFiltered = _cubemapRT_Side = null;

            foreach (var cubemapRTBuffer in _cubemapRTBuffers.Values)
            {
                cubemapRTBuffer?.Release();
                KW_Extensions.SafeDestroy(cubemapRTBuffer);
            }
            _cubemapRTBuffers.Clear();

            _isPlanarTextureInitialized = false;
            _isCubemapTextureInitialized = false;

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }

        void Release()
        {
            if (_reflectionCamera != null) _reflectionCamera.targetTexture = null;
            KW_Extensions.SafeDestroy(_reflCameraGo, _filteringMaterial);
            ReleaseTextures();
            currentInterval = 0;
            requiredUpdateAllFaces = true;

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }

        private WaterSystem.PlanarReflectionResolutionQualityEnum _lastPlanarQuality;
        private WaterSystem.CubemapReflectionResolutionQualityEnum _lastCubemapQuality;
        private bool _lastUseAnisotropicReflections;
        private int _lastCubemapLayers;

        void UpdateWaterLocalVariables()
        {
            _lastPlanarQuality = WaterInstance.PlanarReflectionResolutionQuality;
            _lastCubemapQuality = WaterInstance.CubemapReflectionResolutionQuality;
            _lastUseAnisotropicReflections = WaterInstance.UseAnisotropicReflections;
            _lastCubemapLayers = WaterInstance.CubemapCullingMask;
        }

        void UpdateRTHandlesSize()
        {
            if (WaterInstance.PlanarReflectionResolutionQuality != _lastPlanarQuality
                || WaterInstance.UseAnisotropicReflections != _lastUseAnisotropicReflections)
            {
                UpdateWaterLocalVariables();

                ReleaseTextures();
                KW_Extensions.WaterLog(this.ToString(), "Reset RTAlloc");
            }
        }


        void InitializePlanarTextures()
        {
            if (_isPlanarTextureInitialized) return;

            var format = GraphicsFormat.R16G16B16A16_SFloat;
            var height = (int)WaterInstance.PlanarReflectionResolutionQuality;
            var width = height * 2; // typical resolution ratio is 16x9 (or 2x1), for better pixel filling we use [2 * width] x [height], instead of square [width] * [height]. Also Camera.Render doesn't have SetViewportSize and we can't use RTHandle.DynamicSize
#if UNITY_2019_2_OR_NEWER

        if (WaterInstance.UseAnisotropicReflections)
        {
            _planarRT = new RenderTexture(width, height, 24, format) { name = "_planarRT", useMipMap = false, hideFlags = HideFlags.HideAndDontSave };
            _planarRT_MipOrFiltered = new RenderTexture(width, height, 0, format, 4) { name = "_planarRT_MipOrFiltered", useMipMap = true, autoGenerateMips = true, hideFlags = HideFlags.HideAndDontSave };
        }
        else
        {
            _planarRT = new RenderTexture(width, height, 24, format, 4) { name = "_planarRT", useMipMap = true, autoGenerateMips = true, hideFlags = HideFlags.HideAndDontSave };
        }
#else
            _planarRT = new RenderTexture(width, height, 24, format) { name = "_planarRT", useMipMap = false, hideFlags = HideFlags.HideAndDontSave };
            if (WaterInstance.UseAnisotropicReflections) _planarRT_MipOrFiltered = new RenderTexture(width, height, 0, format) { name = "_planarRT_MipOrFiltered", useMipMap = false, hideFlags = HideFlags.HideAndDontSave };

#endif

            UpdateWaterLocalVariables();
            _isPlanarTextureInitialized = true;

            KW_Extensions.WaterLog(this, _planarRT, _planarRT_MipOrFiltered);
        }

        void CreateCamera()
        {
            _reflCameraGo = new GameObject("WaterReflectionCamera");
#if KWS_DEBUG
            _reflCameraGo.hideFlags = HideFlags.DontSave;
#else
        _reflCameraGo.hideFlags = HideFlags.HideAndDontSave;
#endif

            _reflCameraGo.transform.parent = transform;
            _reflectionCamera = _reflCameraGo.AddComponent<Camera>();
            _reflectionCamera.enabled = false;

            InitializeCameraParamsSRP();
        }

        void CopyCameraParams(Camera currentCamera, int cullingMask, WaterSystem.ReflectionClearFlagEnum clearFlag, Color clearColor)
        {
            _reflectionCamera.CopyFrom(currentCamera);

            _reflectionCamera.cullingMask = cullingMask;
            _reflectionCamera.clearFlags = currentCamera.clearFlags;
            _reflectionCamera.backgroundColor = currentCamera.backgroundColor;
            _reflectionCamera.depth = currentCamera.depth - 100;
            _reflectionCamera.cameraType = CameraType.Reflection;
            _reflectionCamera.allowMSAA = false;

            if (clearFlag == WaterSystem.ReflectionClearFlagEnum.Color)
            {
                _reflectionCamera.clearFlags = CameraClearFlags.SolidColor;
                clearColor.a = 0;
                _reflectionCamera.backgroundColor = clearColor;
            }
            else _reflectionCamera.clearFlags = CameraClearFlags.Skybox;

            CopyCameraParamsSRP(currentCamera, cullingMask, clearFlag, clearColor);
        }

        void RenderCamera(RenderTexture target, Vector3 waterPosition, Matrix4x4 cameraMatrix, Matrix4x4 projectionMatrix, Vector3 currentCameraPos)
        {
            var pos = waterPosition + Vector3.up * m_planeOffset;
            var normal = Vector3.up;

            var d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
            var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            var reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);

            var newPos = reflection.MultiplyPoint(currentCameraPos);
            var xPos = Mathf.Clamp(newPos.x, -float.MaxValue + 100, float.MaxValue - 100) + float.Epsilon; //avoiding error "Screen position out of view frustum"
            var yPos = Mathf.Clamp(newPos.y, -float.MaxValue + 100, float.MaxValue - 100) + float.Epsilon;
            var zPos = Mathf.Clamp(newPos.z, -float.MaxValue + 100, float.MaxValue - 100) + float.Epsilon;
            _reflectionCamera.transform.position = new Vector3(xPos, yPos, zPos);
            _reflectionCamera.worldToCameraMatrix = cameraMatrix * reflection;
            var clipPlane = CameraSpacePlane(_reflectionCamera, pos + normal * 0.05f, normal, 1.0f);

            CalculateObliqueMatrix(ref projectionMatrix, clipPlane);
            _reflectionCamera.projectionMatrix = projectionMatrix;
            var data = new PlanarReflectionSettingData();
            try
            {
                data.Set();
                _reflectionCamera.targetTexture = target;
                CameraRender(_reflectionCamera);
            }
            finally
            {
                data.Restore();
            }
        }

        void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
        {
            var q = projection.inverse * new Vector4(
                Sgn(clipPlane.x),
                Sgn(clipPlane.y),
                1.0f,
                1.0f
            );
            var c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));

            projection[2] = c.x - projection[3];
            projection[6] = c.y - projection[7];
            projection[10] = c.z - projection[11];
            projection[14] = c.w - projection[15];
        }


        public void RenderPlanar(Camera currentCamera, Vector3 waterPosition, WaterSystem.ReflectionClearFlagEnum clearFlag, Color clearColor)
        {

            if (_reflCameraGo == null)
            {
                CreateCamera();
            }

            CopyCameraParams(currentCamera, waterCullingMask, clearFlag, clearColor);

            UpdateRTHandlesSize();
            InitializePlanarTextures();

            RenderCamera(_planarRT, waterPosition, currentCamera.worldToCameraMatrix, currentCamera.projectionMatrix, currentCamera.transform.position);

            if (WaterInstance.UseAnisotropicReflections)
            {
                if (_filteringMaterial == null) _filteringMaterial = KWS_CoreUtils.CreateMaterial(KWS_ShaderConstants.ShaderNames.ReflectionFiltering);

                _filteringMaterial.SetFloat(KWS_ShaderConstants.ReflectionsID.KWS_AnisoReflectionsScale, WaterInstance.AnisotropicReflectionsScale);
                _filteringMaterial.SetFloat(KWS_ShaderConstants.ReflectionsID.KWS_NormalizedWind, Mathf.Clamp01(WaterInstance.WindSpeed * 0.5f));

                if (cmd == null) cmd = new CommandBuffer() { name = "KWS.CameraReflection.AnisotropicFiltering_Pass" };
                cmd.Clear();
                cmd.BlitTriangle(_planarRT, Vector4.one, _planarRT_MipOrFiltered, _filteringMaterial, WaterInstance.AnisotropicReflectionsHighQuality ? 1 : 0);
                Graphics.ExecuteCommandBuffer(cmd);
                //  Graphics.Blit(_planarRT, _planarRT_MipOrFiltered, _filteringMaterial, WaterInstance.AnisotropicReflectionsHighQuality ? 1 : 0);
            }

            var targetRT = WaterInstance.UseAnisotropicReflections ? _planarRT_MipOrFiltered : _planarRT;
            WaterInstance.SetTextures((KWS_ShaderConstants.ReflectionsID.KWS_PlanarReflectionRT, targetRT));
        }

        bool IsRequireUpdateAllSides(Camera cam)
        {
            return (_cubemapRT_Side == null || !_cubemapRTBuffers.ContainsKey(cam) ||
                    WaterInstance.CubemapReflectionResolutionQuality != _lastCubemapQuality || WaterInstance.UseAnisotropicReflections != _lastUseAnisotropicReflections
                 || WaterInstance.CubemapCullingMask != _lastCubemapLayers);
        }

        RenderTexture GetCameraRelativeCubemapTexture(Camera cam)
        {
            if (!_cubemapRTBuffers.ContainsKey(cam))
            {
                var cubemapRT = InitializeCubemapTexture();
                _cubemapRTBuffers.Add(cam, cubemapRT);
                KW_Extensions.WaterLog(this, _cubemapRTBuffers[cam]);
            } 
            else if(_cubemapRTBuffers[cam].width != (int)WaterInstance.CubemapReflectionResolutionQuality)
            {
                var rt = _cubemapRTBuffers[cam];
                rt.Release();
                KW_Extensions.SafeDestroy(rt);
                _cubemapRTBuffers[cam] = InitializeCubemapTexture();
            }

            if (_cubemapRT_Side == null || _cubemapRT_Side.width != (int)WaterInstance.CubemapReflectionResolutionQuality)
            {
                InitializeCubemapTextureSide();
                KW_Extensions.WaterLog(this, _cubemapRT_Side);
            }

            return _cubemapRTBuffers[cam];
        }

        RenderTexture InitializeCubemapTexture()
        {
            var size = (int)WaterInstance.CubemapReflectionResolutionQuality;
            var cubemapRT = new RenderTexture(size, size, 0, GraphicsFormat.R16G16B16A16_SFloat)
            {
                dimension = TextureDimension.Cube,
                hideFlags = HideFlags.HideAndDontSave
            };
            return cubemapRT;
        }

        void InitializeCubemapTextureSide()
        {
            var size = (int)WaterInstance.CubemapReflectionResolutionQuality;
            if(_cubemapRT_Side != null)
            {
                _cubemapRT_Side.Release();
                KW_Extensions.SafeDestroy(_cubemapRT_Side);
            }
            _cubemapRT_Side = new RenderTexture(size, size, 24, GraphicsFormat.R16G16B16A16_SFloat) { name = "_cubemapRT_Side", useMipMap = false, hideFlags = HideFlags.HideAndDontSave };
        }

        public void RenderCubemap(Camera currentCamera, Vector3 waterPosition, float interval, int cullingMask, WaterSystem.ReflectionClearFlagEnum clearFlag, Color clearColor)
        {
            currentInterval += KW_Extensions.DeltaTime();

            if (IsRequireUpdateAllSides(currentCamera)) requiredUpdateAllFaces = true;

            var targetCubemap = GetCameraRelativeCubemapTexture(currentCamera);

            if (requiredUpdateAllFaces || !(currentInterval < interval / 5.0f))
            {
                currentInterval = 0;

                if (_reflCameraGo == null)
                {
                    CreateCamera();
                }

                CopyCameraParams(currentCamera, cullingMask, clearFlag, clearColor); //currentCamera.copyFrom doesn't work correctly 

                _reflectionCamera.fieldOfView = 90;
                _reflectionCamera.aspect = 1;
                var projectionMatrix = Matrix4x4.Perspective(90, 1, currentCamera.nearClipPlane, currentCamera.farClipPlane);

                if (requiredUpdateAllFaces)
                {
                    RenderToCubemapFace(targetCubemap, currentCamera, waterPosition, CubemapFace.NegativeX, projectionMatrix);
                    //RenderToCubemapFace(currentCamera, waterPosition, CubemapFace.NegativeY, projectionMatrix); //optimisation, underwater cubemap face is always invisible. 
                    RenderToCubemapFace(targetCubemap, currentCamera, waterPosition, CubemapFace.NegativeZ, projectionMatrix);
                    RenderToCubemapFace(targetCubemap, currentCamera, waterPosition, CubemapFace.PositiveX, projectionMatrix);
                    RenderToCubemapFace(targetCubemap, currentCamera, waterPosition, CubemapFace.PositiveY, projectionMatrix);
                    RenderToCubemapFace(targetCubemap, currentCamera, waterPosition, CubemapFace.PositiveZ, projectionMatrix);
                }
                else
                {
                    var currentSide = cubeMapSides[sideIdx];
                    sideIdx = (sideIdx < 4) ? sideIdx + 1 : 0;
                    RenderToCubemapFace(targetCubemap, currentCamera, waterPosition, (CubemapFace)currentSide, projectionMatrix);
                }

                requiredUpdateAllFaces = false;
                UpdateWaterLocalVariables();
            }

            WaterInstance.SetTextures((KWS_ShaderConstants.ReflectionsID.KWS_CubemapReflectionRT, targetCubemap));
        }

        void RenderToCubemapFace(RenderTexture targetCubemap, Camera currentCamera, Vector3 waterPosition, CubemapFace face, Matrix4x4 projectionMatrix)
        {
            var camPos = currentCamera.transform.position;
            var viewMatrix = Matrix4x4.Inverse(Matrix4x4.TRS(camPos, GetRotationByCubeFace(face), new Vector3(1, 1, -1)));

            RenderCamera(_cubemapRT_Side, waterPosition, viewMatrix, projectionMatrix, camPos);
            Graphics.CopyTexture(_cubemapRT_Side, 0, targetCubemap, (int)face);

        }

        Quaternion GetRotationByCubeFace(CubemapFace face)
        {
            switch (face)
            {
                case CubemapFace.NegativeX: return Quaternion.Euler(0, -90, 0);
                case CubemapFace.PositiveX: return Quaternion.Euler(0, 90, 0);
                case CubemapFace.PositiveY: return Quaternion.Euler(90, 0, 0);
                case CubemapFace.NegativeY: return Quaternion.Euler(-90, 0, 0);
                case CubemapFace.PositiveZ: return Quaternion.Euler(0, 0, 0);
                case CubemapFace.NegativeZ: return Quaternion.Euler(0, -180, 0);
            }
            return Quaternion.identity;
        }

        private static float Sgn(float a)
        {
            if (a > 0.0f) return 1.0f;
            if (a < 0.0f) return -1.0f;
            return 0.0f;
        }

        private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            var offsetPos = pos + normal * m_ClipPlaneOffset;
            var m = cam.worldToCameraMatrix;
            var cameraPosition = m.MultiplyPoint(offsetPos);
            var cameraNormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cameraNormal.x, cameraNormal.y, cameraNormal.z, -Vector3.Dot(cameraPosition, cameraNormal));
        }

        private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }

        class PlanarReflectionSettingData
        {
            private readonly bool _fog;
            private readonly int _maxLod;
            private readonly float _lodBias;

            public PlanarReflectionSettingData()
            {
                _fog = RenderSettings.fog;
                _maxLod = QualitySettings.maximumLODLevel;
                _lodBias = QualitySettings.lodBias;
            }

            public void Set()
            {
                GL.invertCulling = true;
                RenderSettings.fog = false;
                QualitySettings.maximumLODLevel += 1;
                QualitySettings.lodBias = _lodBias * 0.5f;
            }

            public void Restore()
            {
                GL.invertCulling = false;
                RenderSettings.fog = _fog;
                QualitySettings.maximumLODLevel = _maxLod;
                QualitySettings.lodBias = _lodBias;
            }
        }
    }
}