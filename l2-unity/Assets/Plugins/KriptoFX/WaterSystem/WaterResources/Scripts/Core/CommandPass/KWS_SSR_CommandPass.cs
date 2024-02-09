using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KWS_SSR_CommandPass
    {
        bool debugNonDx11Features = false;

        private KWS_RTHandle _reflectionRT;
        private KWS_RTHandle _reflectionRT_FilteredMip;
        private KWS_RTHandle _reflectionHash;

        private Material _filteringMaterial;
        private ComputeShader _cs;

        const int SHADER_NUMTHREAD_X = 8;
        const int SHADER_NUMTHREAD_Y = 8;

        RenderTargetIdentifier _depthBuffer;
        RenderTargetIdentifier _colorBuffer;

        int _kernelClear;
        int _kernelRenderHash;
        int _kernelRenderColorFromHash;
        int _kernelRenderSinglePassSsr;
        int _kernelFillHoles;

        private WaterSystem _waterInstance;
        bool _isTexturesInitialized;

        Dictionary<Camera, PrevCameraData> prevCameraDatas = new Dictionary<Camera, PrevCameraData>();
        class PrevCameraData
        {
            public TemporaryRenderTexture PrevReflectionRT = new TemporaryRenderTexture();
            public Matrix4x4 matrix_VP;
            public Matrix4x4 matrix_I_VP;
            public Vector2 Jitter;
            public long FrameIdx;
            public Vector4 RTHandleSize;
        }

        enum SSR_TargetApiEnum
        {
            None,
            dx11,
            AndroidMetal
        }

        SSR_TargetApiEnum _ssrAPI = SSR_TargetApiEnum.None;

        private WaterSystem.ScreenSpaceReflectionResolutionQualityEnum _lastResolutionQuality;
        private bool _lastUseAnisotropicReflections;


        void UpdateRTHandlesSize(Camera cam)
        {
            if (_waterInstance.ScreenSpaceReflectionResolutionQuality != _lastResolutionQuality)
            {
                _lastResolutionQuality = _waterInstance.ScreenSpaceReflectionResolutionQuality;
                var newSize = GetCameraScreenSizeLimited(cam);
                KWS_RTHandles.ResetReferenceSize(newSize.x, newSize.y);
                KW_Extensions.WaterLog(this.ToString(), "Reset RTHandle Reference Size");
            }

            if (_waterInstance.UseAnisotropicReflections != _lastUseAnisotropicReflections)
            {
                _lastUseAnisotropicReflections = _waterInstance.UseAnisotropicReflections;
                ReleaseTextures();
                InitializeTextures();
                KW_Extensions.WaterLog(this.ToString(), "Reset RTAlloc");
            }
        }

        SSR_TargetApiEnum GetSupportedAPI()
        {
            if (debugNonDx11Features) return SSR_TargetApiEnum.AndroidMetal;

            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RInt))
                return SSR_TargetApiEnum.AndroidMetal;

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                return SSR_TargetApiEnum.AndroidMetal;

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12)
                return SSR_TargetApiEnum.dx11;
#if UNITY_ANDROID
            return SSR_TargetApiEnum.AndroidMetal;
#endif

            return SSR_TargetApiEnum.AndroidMetal;
        }

        Vector2Int GetReflectionResolution(int width, int height)
        {
            var resolutionDownsample = (int)_waterInstance.ScreenSpaceReflectionResolutionQuality / 100f;
            return new Vector2Int((int)(width * resolutionDownsample), (int)(height * resolutionDownsample));
        }

        Vector2Int ComputeRTHandleSize(Vector2Int screenSize)
        {
            return GetReflectionResolution(screenSize.x, screenSize.y);
        }

        void InitializeTextures()
        {
            if (_isTexturesInitialized) return;

            var colorFormat = GraphicsFormat.R16G16B16A16_SFloat;
            if (_ssrAPI == SSR_TargetApiEnum.None) _ssrAPI = GetSupportedAPI();

#if UNITY_2019_2_OR_NEWER
            if (_waterInstance.UseAnisotropicReflections)
            {
                _reflectionRT = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_reflectionRT", colorFormat: colorFormat, enableRandomWrite: true, useMipMap: false);
                _reflectionRT_FilteredMip = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_reflectionRT_Mip", colorFormat: colorFormat, useMipMap: true, autoGenerateMips: true, mipMapCount: 4);
            }
            else
            {
                _reflectionRT = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_reflectionRT", colorFormat: colorFormat, enableRandomWrite: true, useMipMap: true, autoGenerateMips: false, mipMapCount: 4);
            }
#else
            _reflectionRT = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_reflectionRT", colorFormat: colorFormat, enableRandomWrite: true, useMipMap: false);
            if (_waterInstance.UseAnisotropicReflections) _reflectionRT_FilteredMip = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_reflectionRT_Mip", colorFormat: colorFormat, useMipMap: false);
#endif

            _reflectionHash = KWS_RTHandles.Alloc(ComputeRTHandleSize, name: "_reflectionHash", colorFormat: _ssrAPI == SSR_TargetApiEnum.dx11 ? GraphicsFormat.R32_SInt : GraphicsFormat.R32_SFloat, enableRandomWrite: true, useMipMap: false);

            _lastResolutionQuality = _waterInstance.ScreenSpaceReflectionResolutionQuality;
            _lastUseAnisotropicReflections = _waterInstance.UseAnisotropicReflections;
            _isTexturesInitialized = true;

            KW_Extensions.WaterLog(this, _reflectionRT, _reflectionHash, _reflectionRT_FilteredMip);
        }

        public RenderTargetIdentifier GetTargetColorBuffer()
        {
            return _reflectionRT;
        }

        void InitializeMaterials()
        {
            if (_cs == null)
            {
                _cs = (ComputeShader)Resources.Load($"PlatformSpecific/" + GetShaderNameForPipeline("KWS_SSR"));
                _kernelClear = _cs.FindKernel(KWS_ShaderConstants.SSR_Kernels.Clear_kernel);
                _kernelRenderHash = _cs.FindKernel(KWS_ShaderConstants.SSR_Kernels.RenderHash_kernel);
                _kernelRenderColorFromHash = _cs.FindKernel(KWS_ShaderConstants.SSR_Kernels.RenderColorFromHash_kernel);
                _kernelRenderSinglePassSsr = _cs.FindKernel(KWS_ShaderConstants.SSR_Kernels.RenderSinglePassSSR_kernel);
                _kernelFillHoles = _cs.FindKernel(KWS_ShaderConstants.SSR_Kernels.FillHoles_kernel);
            }

            if (_filteringMaterial == null) _filteringMaterial = KWS_CoreUtils.CreateMaterial(KWS_ShaderConstants.ShaderNames.ReflectionFiltering);
        }

        public void Initialize(WaterSystem currentWater, RenderTargetIdentifier depthBuffer)
        {
            _waterInstance = currentWater;
            _depthBuffer = depthBuffer;
            InitializeTextures();
            InitializeMaterials();
        }

        public void Execute(Camera cam, CommandBuffer cmd)
        {
            UpdateRTHandlesSize(cam);
            var resolution = GetReflectionResolution(cam.scaledPixelWidth, cam.scaledPixelHeight);

            int dispatchThreadGroupXCount = Mathf.CeilToInt((float)resolution.x / SHADER_NUMTHREAD_X);
            int dispatchThreadGroupYCount = Mathf.CeilToInt((float)resolution.y / SHADER_NUMTHREAD_Y);
            int dispatchThreadGroupZCount = 1;

            var rtWidth = _reflectionRT.rt.width * _reflectionRT.rtHandleProperties.rtHandleScale.x;
            var rtHeight = _reflectionRT.rt.height * _reflectionRT.rtHandleProperties.rtHandleScale.y;
            cmd.SetComputeVectorParam(_cs, KWS_ShaderConstants.SSR_ID._RTSize, new Vector4(rtWidth, rtHeight, 1f / rtWidth, 1f / rtHeight));
            cmd.SetComputeFloatParam(_cs, KWS_ShaderConstants.SSR_ID._HorizontalPlaneHeightWS, _waterInstance.transform.position.y);
            cmd.SetComputeFloatParam(_cs, KWS_ShaderConstants.SSR_ID._DepthHolesFillDistance, _waterInstance.ReflectioDepthHolesFillDistance);

            var matrixV = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true);
            var matrixP = cam.worldToCameraMatrix;
            var matrixVP = matrixV * matrixP;

            cmd.SetComputeMatrixParam(_cs, KWS_ShaderConstants.SSR_ID.KW_MATRIX_VP, matrixVP);
            cmd.SetComputeMatrixParam(_cs, KWS_ShaderConstants.SSR_ID.KW_MATRIX_I_VP, matrixVP.inverse);

            if (_ssrAPI == SSR_TargetApiEnum.dx11)
            {
                cmd.SetComputeTextureParam(_cs, _kernelClear, KWS_ShaderConstants.SSR_ID.HashRT, _reflectionHash.rt);
                cmd.SetComputeTextureParam(_cs, _kernelClear, KWS_ShaderConstants.SSR_ID.ColorRT, _reflectionRT.rt);
                cmd.DispatchCompute(_cs, _kernelClear, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

                cmd.SetComputeTextureParam(_cs, _kernelRenderHash, KWS_ShaderConstants.SSR_ID.HashRT, _reflectionHash.rt);
                cmd.SetComputeTextureParam(_cs, _kernelRenderHash, KWS_ShaderConstants.SSR_ID.ColorRT, _reflectionRT.rt);

                cmd.SetComputeTextureParam(_cs, _kernelRenderHash, KWS_ShaderConstants.SSR_ID._CameraDepthTexture, _depthBuffer);
                cmd.DispatchCompute(_cs, _kernelRenderHash, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

                //cmd.SetComputeTextureParam(cs, kernel_RenderColorFromHash_kernel, "_CameraOpaqueTexture", colorTexture);
                cmd.SetComputeTextureParam(_cs, _kernelRenderColorFromHash, KWS_ShaderConstants.SSR_ID.HashRT, _reflectionHash.rt);
                cmd.SetComputeTextureParam(_cs, _kernelRenderColorFromHash, KWS_ShaderConstants.SSR_ID.ColorRT, _reflectionRT.rt);
                cmd.DispatchCompute(_cs, _kernelRenderColorFromHash, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

            }
            else if (_ssrAPI == SSR_TargetApiEnum.AndroidMetal)
            {
                if (!prevCameraDatas.ContainsKey(cam)) prevCameraDatas.Add(cam, new PrevCameraData());
                var prevData = prevCameraDatas[cam];

                cmd.SetComputeMatrixParam(_cs, KWS_ShaderConstants.SSR_ID.KW_PREV_MATRIX_VP, prevData.matrix_VP);
                cmd.SetComputeMatrixParam(_cs, KWS_ShaderConstants.SSR_ID.KW_PREV_MATRIX_I_VP, prevData.matrix_I_VP);
                cmd.SetComputeTextureParam(_cs, _kernelRenderSinglePassSsr, KWS_ShaderConstants.SSR_ID._PrevReflectionRT, prevData.PrevReflectionRT.rt);
                cmd.SetComputeVectorParam(_cs, KWS_ShaderConstants.SSR_ID._PrevReflectionRTHandleSize, prevData.RTHandleSize);

                cmd.SetComputeTextureParam(_cs, _kernelRenderSinglePassSsr, KWS_ShaderConstants.SSR_ID.ColorRT, _reflectionRT.rt);
                cmd.SetComputeTextureParam(_cs, _kernelRenderSinglePassSsr, KWS_ShaderConstants.SSR_ID.PosWSyRT, _reflectionHash.rt);
                cmd.SetComputeTextureParam(_cs, _kernelRenderSinglePassSsr, KWS_ShaderConstants.SSR_ID._CameraDepthTexture, _depthBuffer);
                cmd.DispatchCompute(_cs, _kernelRenderSinglePassSsr, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

                cmd.SetComputeTextureParam(_cs, _kernelFillHoles, KWS_ShaderConstants.SSR_ID.ColorRT, _reflectionRT.rt);
                cmd.SetComputeTextureParam(_cs, _kernelFillHoles, KWS_ShaderConstants.SSR_ID.PackedDataRT, _reflectionHash.rt);
                cmd.DispatchCompute(_cs, _kernelFillHoles, Mathf.CeilToInt(dispatchThreadGroupXCount / 2f), Mathf.CeilToInt(dispatchThreadGroupYCount / 2f), dispatchThreadGroupZCount);

                prevData.matrix_VP = matrixVP;
                prevData.matrix_I_VP = matrixVP.inverse;
                var rt = _reflectionRT.rt;
                prevData.PrevReflectionRT.Alloc("_PrevReflectionRT", rt.width, rt.height, rt.depth, GraphicsFormat.R16G16B16A16_SFloat);
                prevData.RTHandleSize = _reflectionRT.rtHandleProperties.rtHandleScale;
                cmd.Blit(rt, prevData.PrevReflectionRT.rt);
            }
            else
            {
                Debug.LogError("Not initialized Water SSR API");
                return;
            }

            if (_waterInstance.UseAnisotropicReflections)
            {
                _filteringMaterial.SetFloat(KWS_ShaderConstants.ReflectionsID.KWS_AnisoReflectionsScale, _waterInstance.AnisotropicReflectionsScale);
                _filteringMaterial.SetFloat(KWS_ShaderConstants.ReflectionsID.KWS_NormalizedWind, Mathf.Clamp01(_waterInstance.WindSpeed * 0.5f));

                cmd.BlitTriangleRTHandle(_reflectionRT, _reflectionRT.rtHandleProperties.rtHandleScale, _reflectionRT_FilteredMip, _filteringMaterial, ClearFlag.None, Color.clear, _waterInstance.AnisotropicReflectionsHighQuality ? 1 : 0);
            }
            else
            {
                //#if UNITY_2019_2_OR_NEWER //rendering of bright objects (like a sun) will cause artifacts in last mips levels and I must limit "mipmap levels". This feature added only in unity 2020.2 
                //cmd.GenerateMips(reflectionRT.rt);
#if UNITY_2019_2_OR_NEWER
                _reflectionRT.rt.GenerateMips();
#endif
            }

            var targetRT = _waterInstance.UseAnisotropicReflections ? _reflectionRT_FilteredMip : _reflectionRT;
            var rtScale = targetRT.rtHandleProperties.rtHandleScale;

            _waterInstance.SetTextures(cmd, (KWS_ShaderConstants.SSR_ID.KWS_ScreenSpaceReflectionRT, targetRT));
            _waterInstance.SetVectors(cmd, (KWS_ShaderConstants.SSR_ID.KWS_ScreenSpaceReflection_RTHandleScale, rtScale));
        }

        public void ReleaseTextures()
        {
            _reflectionRT?.Release();
            _reflectionRT_FilteredMip?.Release();
            _reflectionHash?.Release();
            _isTexturesInitialized = false;
        }

        public void Release()
        {
            ReleaseTextures();

            Resources.UnloadAsset(_cs);
            _ssrAPI = SSR_TargetApiEnum.None;
            KW_Extensions.SafeDestroy(_filteringMaterial);
            prevCameraDatas.Clear();

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }
    }
}