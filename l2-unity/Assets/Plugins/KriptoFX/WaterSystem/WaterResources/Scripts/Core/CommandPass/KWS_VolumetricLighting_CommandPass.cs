using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;
using static KWS.KWS_ShaderConstants;

namespace KWS
{
    public class KWS_VolumetricLighting_CommandPass
    {
        private Vector4[] _nearPlaneWorldPos = new Vector4[4];

        public KWS_RTHandle volumeLightRT_Blured;
        public KWS_RTHandle volumeLightRT;
        //public RTHandle volumeLightRT_Temp;
        //public RTHandle lowResDepthRT;

        private Material       volumeLightMat;
        private Material       volumeLightBlurMat;
        private KW_PyramidBlur pyramidBlur = new KW_PyramidBlur();

        WaterSystem _waterInstance;
        bool        _isTexturesInitialized;
        private WaterSystem.VolumetricLightResolutionQualityEnum _lastResolutionQuality;
        private WaterSystem.VolumetricLightFilterEnum _lastVolumetricLightFilterMode;

        private int _SourceRTHandleScaleID = Shader.PropertyToID("_SourceRTHandleScale");

        Vector4 ComputeMieVector(float MieG)
        {
            return new Vector4(1 - (MieG * MieG), 1 + (MieG * MieG), 2 * MieG, 1.0f / (4.0f * Mathf.PI));
        }

        void CheckAndResetRTHandleSize(Camera cam)
        {
            if (_waterInstance.VolumetricLightResolutionQuality != _lastResolutionQuality || _waterInstance.VolumetricLightFilter != _lastVolumetricLightFilterMode)
            {
                _lastResolutionQuality = _waterInstance.VolumetricLightResolutionQuality;
                _lastVolumetricLightFilterMode = _waterInstance.VolumetricLightFilter;

                var newSize = GetCameraScreenSizeLimited(cam);
                KWS_RTHandles.ResetReferenceSize(newSize.x, newSize.y);
                KW_Extensions.WaterLog(this, volumeLightRT, volumeLightRT_Blured);
            }
        }

        Vector2Int ComputeRTHandleSize(Vector2Int screenSize)
        {
            var underwaterResolutionDownsample = (int)_waterInstance.VolumetricLightResolutionQuality / 100f;
            return new Vector2Int((int) (screenSize.x * underwaterResolutionDownsample), (int) (screenSize.y * underwaterResolutionDownsample));
        }

        Vector2Int ComputeRTHandleSizeBlured(Vector2Int screenSize)
        {
            if (_waterInstance.VolumetricLightFilter == WaterSystem.VolumetricLightFilterEnum.Bilateral) return screenSize;
            else return ComputeRTHandleSize(screenSize);
        }

        public void InitializeTextures()
        {
            if (_isTexturesInitialized) return;

            volumeLightRT = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_volumeLightRT", colorFormat: GraphicsFormat.R16G16B16A16_SFloat);
            volumeLightRT_Blured = KWS_RTHandles.Alloc(ComputeRTHandleSizeBlured, name: "_volumeLightRT_Blured", colorFormat: GraphicsFormat.R16G16B16A16_SFloat);

            _isTexturesInitialized = true;

            KW_Extensions.WaterLog(this, volumeLightRT, volumeLightRT_Blured);
        }

        public RenderTargetIdentifier GetTargetColorBuffer()
        {
            return volumeLightRT.rt;
        }

        void InitializeMaterials()
        {
            if (volumeLightMat == null)
            {
                volumeLightMat = CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.VolumetricLightingShaderName));
                _waterInstance.AddMaterialToWaterRendering(volumeLightMat);
            }

            if (volumeLightBlurMat == null) volumeLightBlurMat = CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.BlurBilateralName));
        }

        public void Initialize(WaterSystem currentWater)
        {
            _waterInstance = currentWater;

            InitializeTextures();
            InitializeMaterials();
        }


        public void Execute(Camera cam, CommandBuffer cmd)
        {
            CheckAndResetRTHandleSize(cam);
            UpdateMaterialParams(cam);
            cmd.BlitTriangleRTHandle(volumeLightRT, volumeLightMat, ClearFlag.Color, Color.clear, 0);

            if (_waterInstance.VolumetricLightFilter == WaterSystem.VolumetricLightFilterEnum.Bilateral)
            {
                var rtHandleScale = volumeLightRT.rtHandleProperties.rtHandleScale;
                var scaledViewportSize = volumeLightRT.GetScaledSize(volumeLightRT.rtHandleProperties.currentViewportSize);
                var temp = RenderTexture.GetTemporary(scaledViewportSize.x, scaledViewportSize.y, 0, volumeLightRT.rt.format);
                var lowResDepthTemp = RenderTexture.GetTemporary(scaledViewportSize.x, scaledViewportSize.y, 0, GraphicsFormatUtility.GetRenderTextureFormat(GraphicsFormat.R32_SFloat));

                volumeLightBlurMat.SetTexture(VolumetricLightConstantsID.KWS_CameraDepthTextureLowRes, lowResDepthTemp);
                cmd.BlitTriangle(lowResDepthTemp,      volumeLightBlurMat, pass: KWS_Settings.VolumetricLighting.UseFastBilateralMode ? 1 : 0); 
                
                cmd.SetGlobalVector(_SourceRTHandleScaleID, rtHandleScale);
                cmd.BlitTriangle(volumeLightRT, temp,   volumeLightBlurMat, pass: KWS_Settings.VolumetricLighting.UseFastBilateralMode ? 3 : 2);

                cmd.BlitTriangleRTHandle(temp, Vector4.one, volumeLightRT, volumeLightBlurMat, ClearFlag.None, Color.clear, pass: KWS_Settings.VolumetricLighting.UseFastBilateralMode ? 5 : 4);
                cmd.BlitTriangleRTHandle(volumeLightRT, rtHandleScale, volumeLightRT_Blured, volumeLightBlurMat, ClearFlag.None, Color.clear, pass: 6);

                RenderTexture.ReleaseTemporary(temp);
                RenderTexture.ReleaseTemporary(lowResDepthTemp);
            }
            else
            {
                pyramidBlur.ComputeBlurPyramid(_waterInstance.VolumetricLightBlurRadius, volumeLightRT, volumeLightRT_Blured, cmd);
            }

            _waterInstance.SetTextures(cmd, (VolumetricLightConstantsID.KWS_VolumetricLightRT, volumeLightRT_Blured.rt));
            _waterInstance.SetVectors(cmd, (VolumetricLightConstantsID.KWS_VolumetricLight_RTHandleScale, volumeLightRT_Blured.rtHandleProperties.rtHandleScale));
        }

        private void UpdateMaterialParams(Camera cam)
        {
            var anisoMie = ComputeMieVector(0.05f);

            var volumeLightMaxDist = Mathf.Max(0.3f, _waterInstance.Transparent * 3);
            volumeLightMaxDist = Mathf.Min(40, volumeLightMaxDist);
            float volumeLightFade = Mathf.Clamp01(Mathf.Exp(-1 * (_waterInstance.transform.position.y - cam.transform.position.y) / _waterInstance.Transparent));

            var rtHandleScale = volumeLightRT.rtHandleProperties.rtHandleScale;
            var actualRTSize = new Vector2(volumeLightRT.rt.width * rtHandleScale.x, volumeLightRT.rt.height * rtHandleScale.y);

            //frustum[0] = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.farClipPlane));
            //frustum[1] = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.farClipPlane));
            //frustum[2] = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.farClipPlane));
            //frustum[3] = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.farClipPlane));

            // TODO
            // I use full screen triangle with 3 points : [0, 0], [2, 0], [0, 2] and uv clamped from 0-center.
            // But by default, Y position is reverted and I need to check
            // UV_START_AT_TOP for _nearPlaneWorldPos[2] = cam.ViewportToWorldPoint(new Vector3(0, 2,  0.3f)); instead of '-2'

            _nearPlaneWorldPos[0] = cam.ViewportToWorldPoint(new Vector3(0, 0, 0.3f));
            _nearPlaneWorldPos[1] = cam.ViewportToWorldPoint(new Vector3(2, 0, 0.3f));
            _nearPlaneWorldPos[2] = cam.ViewportToWorldPoint(new Vector3(0, -2,  0.3f)); 
           
            //_nearPlaneWorldPos[3] = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            volumeLightMat.SetFloat(VolumetricLightConstantsID.KWS_Transparent, _waterInstance.Transparent);
            volumeLightMat.SetVector(VolumetricLightConstantsID.KWS_LightAnisotropy, anisoMie);
            volumeLightMat.SetFloat(VolumetricLightConstantsID.KWS_VolumeDepthFade, volumeLightFade);
            volumeLightMat.SetFloat(VolumetricLightConstantsID.KWS_VolumeLightMaxDistance, volumeLightMaxDist);
            volumeLightMat.SetFloat(VolumetricLightConstantsID.KWS_RayMarchSteps, _waterInstance.VolumetricLightIteration);
            volumeLightMat.SetFloat(VolumetricLightConstantsID.KWS_VolumeLightBlurRadius, _waterInstance.VolumetricLightBlurRadius);
            volumeLightMat.SetVector(VolumetricLightConstantsID.KWS_VolumeTexSceenSize, actualRTSize);
            volumeLightMat.SetVectorArray(VolumetricLightConstantsID.KWS_NearPlaneWorldPos, _nearPlaneWorldPos);
        }

        public void Release()
        {
            volumeLightRT_Blured?.Release();
            volumeLightRT?.Release();
           
            _isTexturesInitialized = false;

            KW_Extensions.SafeDestroy(volumeLightMat, volumeLightBlurMat);
            pyramidBlur?.Release();

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }
    }
}