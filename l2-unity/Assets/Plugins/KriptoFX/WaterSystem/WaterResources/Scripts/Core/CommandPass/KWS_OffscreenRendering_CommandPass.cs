using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KWS_OffscreenRendering_CommandPass
    {
        private Material sceneCombineMaterial;

        KWS_RTHandle _waterRT;
        private const string waterDepthShaderName = "Hidden/KriptoFX/KWS/OffscreenRendering";
        RenderTargetIdentifier _colorBuffer;

        WaterSystem _waterInstance;
        bool _isTexturesInitialized;
        private KWS_RTHandleSystem handleSystem;

        private WaterSystem.OffscreenResolutionQualityEnum _lastResolutionQuality;
        private WaterSystem.AntialiasingEnum _lastOffscreenRenderingAntialiasing;


        void UpdateRTHandlesSize(Camera cam)
        {
            if (_waterInstance.OffscreenResolutionQuality != _lastResolutionQuality)
            {
                _lastResolutionQuality = _waterInstance.OffscreenResolutionQuality;
                var newSize = GetCameraScreenSizeLimited(cam);
                handleSystem.ResetReferenceSize(newSize.x, newSize.y);
                KW_Extensions.WaterLog(this.ToString(), "Reset RTHandle Reference Size");
            }

            if (_waterInstance.OffscreenRenderingAntialiasing != _lastOffscreenRenderingAntialiasing)
            {
                _lastOffscreenRenderingAntialiasing = _waterInstance.OffscreenRenderingAntialiasing;
                ReleaseTextures();
                InitializeTextures();
                KW_Extensions.WaterLog(this.ToString(), "Reset RTAlloc");
            }
        }

        Vector2Int ComputeRTHandleSize(Vector2Int screenSize)
        {
            var resDownsample = (int)_waterInstance.OffscreenResolutionQuality / 100f;
            return new Vector2Int((int)(screenSize.x * resDownsample), (int)(screenSize.y * resDownsample));
        }

        void InitializeTextures()
        {
            if (_isTexturesInitialized) return;

            if (handleSystem == null)
            {
                handleSystem = new KWS_RTHandleSystem();
                handleSystem.Initialize(Screen.width, Screen.height);
            }

            _waterRT = new KWS_RTHandle(handleSystem);
            _waterRT = handleSystem.Alloc(ComputeRTHandleSize, name: "_WaterOffscreenRT", colorFormat: GraphicsFormat.R16G16B16A16_SFloat, msaaSamples: (MSAASamples)_waterInstance.OffscreenRenderingAntialiasing);

            _lastOffscreenRenderingAntialiasing = _waterInstance.OffscreenRenderingAntialiasing;
            _lastResolutionQuality = _waterInstance.OffscreenResolutionQuality;

            _isTexturesInitialized = true;

            KW_Extensions.WaterLog(this, _waterRT);
        }

        public RenderTargetIdentifier GetTargetColorTexture()
        {
            return _waterRT;
        }

        public void InitializeMaterials()
        {
            if (sceneCombineMaterial == null)
            {
                sceneCombineMaterial = KWS_CoreUtils.CreateMaterial(GetShaderNameForPipeline(waterDepthShaderName));
            }
        }

        public void Initialize(WaterSystem currentWater)
        {
            _waterInstance = currentWater;
            InitializeMaterials();
            InitializeTextures();
        }

        public void SetColorBuffer(RenderTargetIdentifier buffer)
        {
            _colorBuffer = buffer;
        }

        public void Execute(Camera cam, CommandBuffer cmd)
        {
            UpdateRTHandlesSize(cam);
            handleSystem.SetReferenceSize(cam.scaledPixelWidth, cam.scaledPixelHeight);
            cmd.SetViewportAndClear(_waterRT, ClearFlag.Color, Color.clear);
            cmd.DrawMesh(_waterInstance.WaterMesh, _waterInstance.WaterMeshTransform.localToWorldMatrix, _waterInstance.WaterMaterial);
        }

        public void Execute_DrawToCameraBuffer(Camera cam, CommandBuffer cmd)
        {
            cmd.BlitTriangle(_waterRT, _waterRT.rtHandleProperties.rtHandleScale, _colorBuffer, sceneCombineMaterial);
        }

        void ReleaseTextures()
        {
            _waterRT?.Release();
            _isTexturesInitialized = false;
        }

        public void Release()
        {
            ReleaseTextures();
            KW_Extensions.SafeDestroy(sceneCombineMaterial);

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }
    }
}