using UnityEngine;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KWS_Underwater_CommandPass
    {
        KW_PyramidBlur _pyramidBlur;
        private Material _underwaterMaterial;

        KWS_RTHandle _underwaterRT;
        KWS_RTHandle _underwaterRTBlured;

        RenderTargetIdentifier _colorBuffer;

        bool _isTexturesInitialized;
        WaterSystem _waterInstance;
        private WaterSystem.UnderwaterResolutionQualityEnum lastResolutionQuality;

        void CheckAndResetRTHandleSize(Camera cam)
        {
            if (_waterInstance.UnderwaterResolutionQuality != lastResolutionQuality)
            {
                lastResolutionQuality = _waterInstance.UnderwaterResolutionQuality;
                var newSize = GetCameraScreenSizeLimited(cam);
                KWS_RTHandles.ResetReferenceSize(newSize.x, newSize.y);
            }

        }

        Vector2Int ComputeRTHandleSize(Vector2Int screenSize)
        {
            var resDownsample = (int)_waterInstance.UnderwaterResolutionQuality / 100f;
            return new Vector2Int((int)(screenSize.x * resDownsample), (int)(screenSize.y * resDownsample));
        }

        void InitializeTextures()
        {
            if (_isTexturesInitialized) return;

            var hdrFormat = KWS_CoreUtils.GetGraphicsFormatHDR();
            _underwaterRT = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_underwaterRT", colorFormat: hdrFormat);
            _underwaterRTBlured = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_underwaterRT_Blured", colorFormat: hdrFormat);
            _isTexturesInitialized = true;

            KW_Extensions.WaterLog(this, _underwaterRT, _underwaterRTBlured);
        }


        void InitializeMaterials()
        {
            if (_underwaterMaterial == null)
            {
                _underwaterMaterial = KWS_CoreUtils.CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.UnderwaterShaderName));
                _waterInstance.AddMaterialToWaterRendering(_underwaterMaterial);
            }
        }

        public void Initialize(WaterSystem currentWater)
        {
            _waterInstance = currentWater;

            if (_waterInstance.UseUnderwaterBlur) InitializeTextures();
            InitializeMaterials();
        }

        public RenderTargetIdentifier SetColorBuffer(RenderTargetIdentifier colorBuffer)
        {
            return _colorBuffer = colorBuffer;
        }

        public RenderTargetIdentifier GetTargetColorBuffer()
        {
            return _underwaterRT;
        }

        public void Execute(Camera cam, CommandBuffer cmd, WaterSystem currentWater)
        {
            if (currentWater.UseUnderwaterBlur)
            {
                CheckAndResetRTHandleSize(cam);

                cmd.BlitTriangleRTHandle(_underwaterRT, _underwaterMaterial, ClearFlag.None, Color.clear);

                if (_pyramidBlur == null) _pyramidBlur = new KW_PyramidBlur();
                _pyramidBlur.ComputeBlurPyramid(currentWater.UnderwaterBlurRadius, _underwaterRT, _underwaterRTBlured, cmd);

                cmd.SetGlobalVector(KWS_ShaderConstants.UnderwaterID.KWS_Underwater_RTHandleScale, _underwaterRTBlured.rtHandleProperties.rtHandleScale);
                cmd.BlitTriangle(_underwaterRTBlured, _colorBuffer, _underwaterMaterial, 1);
            }
            else
            {
                cmd.BlitTriangle(_colorBuffer, _underwaterMaterial, 0);
            }
        }

        public void Release()
        {
            _underwaterRT?.Release();
            _underwaterRTBlured?.Release();
            _pyramidBlur?.Release();
            KW_Extensions.SafeDestroy(_underwaterMaterial);
            _isTexturesInitialized = false;

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }
    }
}