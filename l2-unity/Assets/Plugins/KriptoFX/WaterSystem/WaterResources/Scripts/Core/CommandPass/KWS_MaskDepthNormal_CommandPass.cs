using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KWS_MaskDepthNormal_CommandPass
    {
        private KW_PyramidBlur _pyramidBlurMask;
        private Material _maskDepthNormalMaterial;

        KWS_RTHandle _waterMaskRT;
        KWS_RTHandle _waterMaskRTBlured;
        KWS_RTHandle _waterDepthRT;

        WaterSystem _waterInstance;
        bool _isTexturesInitialized;

        //private bool _lastUseUnderwaterEffect;
        //private bool _lastUseHightQualityUnderwater;

        //void CheckAndResetRTHandleSize(Camera cam)
        //{
        //    if (_waterInstance.UseUnderwaterEffect != _lastUseUnderwaterEffect || _waterInstance.UseHighQualityUnderwater != _lastUseHightQualityUnderwater)
        //    {
        //        _lastUseUnderwaterEffect       = _waterInstance.UseUnderwaterEffect;
        //        _lastUseHightQualityUnderwater = _waterInstance.UseHighQualityUnderwater;

        //        var newSize = GetCameraScreenSizeLimited(cam);
        //        RTHandles.ResetReferenceSize(newSize.x, newSize.y);
        //    }
        //}

        //Vector2Int ComputeRTHandleSize(Vector2Int screenSize)
        //{
        //    var resolutionDownsample = (_waterInstance.UseUnderwaterEffect && _waterInstance.UseHighQualityUnderwater) ? 2 : 4;
        //    return new Vector2Int(screenSize.x / resolutionDownsample, screenSize.y / resolutionDownsample);
        //}

        void InitializeTextures()
        {
            if (_isTexturesInitialized) return;

            var scale = new Vector2(0.35f, 0.35f);
            _waterMaskRT = KWS_RTHandles.Alloc(scale, name: "_waterMaskRT", colorFormat: GraphicsFormat.R16G16B16A16_SFloat);
            _waterDepthRT = KWS_RTHandles.Alloc(scale, name: "_waterDepthRT", depthBufferBits: DepthBits.Depth24, useDepthOnlyFormat: true);
            _waterMaskRTBlured = KWS_RTHandles.Alloc(scale, name: "_waterMaskRT", colorFormat: GraphicsFormat.R16G16B16A16_SFloat);

            _isTexturesInitialized = true;

            KW_Extensions.WaterLog(this, _waterMaskRT, _waterDepthRT, _waterMaskRTBlured);
        }

        public RenderTargetIdentifier GetTargetColorBuffer()
        {
            return _waterMaskRT;
        }

        public RenderTargetIdentifier GetTargetDepthBuffer()
        {
            return _waterDepthRT;
        }

        void InitializeMaterials()
        {
            if (_maskDepthNormalMaterial == null)
            {
                _maskDepthNormalMaterial = CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.MaskDepthNormalShaderName));
                _waterInstance.AddMaterialToWaterRendering(_maskDepthNormalMaterial);
            }
        }

        public void Initialize(WaterSystem currentWater)
        {
            _waterInstance = currentWater;
            InitializeTextures();
            InitializeMaterials();
        }

        public void Execute(Camera cam, CommandBuffer cmd)
        {
            //CheckAndResetRTHandleSize(cam);
            KWS_SPR_CoreUtils.SetRenderTarget(cmd, _waterMaskRT, _waterDepthRT, ClearFlag.All);
           
            if (_waterInstance.WaterMeshTransform == null || _waterInstance.WaterMesh == null) return;

            var shaderPass = _waterInstance.UseTesselation && SystemInfo.graphicsShaderLevel >= 46 ? 0 : 1;
            cmd.DrawMesh(_waterInstance.WaterMesh, _waterInstance.WaterMeshTransform.localToWorldMatrix, _maskDepthNormalMaterial, 0, shaderPass);

            if (_pyramidBlurMask == null) _pyramidBlurMask = new KW_PyramidBlur();
            _pyramidBlurMask.ComputeBlurPyramid(2.0f, _waterMaskRT, _waterMaskRTBlured, cmd);

            _waterInstance.SetTextures(cmd,
                (KWS_ShaderConstants.MaskPassID.KW_WaterDepth_ID, _waterDepthRT.rt),
                (KWS_ShaderConstants.MaskPassID.KW_WaterMaskScatterNormals_ID, _waterMaskRT.rt),
                (KWS_ShaderConstants.MaskPassID.KW_WaterMaskScatterNormals_Blured_ID, _waterMaskRTBlured.rt));
            _waterInstance.SetVectors(cmd, (KWS_ShaderConstants.MaskPassID.KWS_WaterMask_RTHandleScale, _waterMaskRT.rtHandleProperties.rtHandleScale));
        }

        public void Release()
        {
            _waterMaskRT?.Release();
            _waterMaskRTBlured?.Release();
            _waterDepthRT?.Release();
            _isTexturesInitialized = false;

            KW_Extensions.SafeDestroy(_maskDepthNormalMaterial);
            _pyramidBlurMask?.Release();
            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }
    }
}