using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KWS_DrawToDepth_CommandPass
    {
        KWS_RTHandle _sceneDepthRT;
        private       Material _drawToDepthMaterial;
        private       Material _copyDepthMaterial;


        RenderTargetIdentifier _depthBuffer;

        WaterSystem _waterInstance;
        bool _isTexturesInitialized;

        void InitializeTextures()
        {
            if (_isTexturesInitialized) return;

            _sceneDepthRT = KWS_RTHandles.Alloc(Vector2.one, name: "_depthRT", colorFormat: GraphicsFormat.R32_SFloat);
            _isTexturesInitialized = true;

            KW_Extensions.WaterLog(this, _sceneDepthRT);
        }

        public RenderTargetIdentifier GetTargetColorBuffer()
        {
            return _sceneDepthRT;
        }

        void InitializeMaterials()
        {
            if (_drawToDepthMaterial == null)
            {
                _drawToDepthMaterial = KWS_CoreUtils.CreateMaterial(KWS_ShaderConstants.ShaderNames.DrawToDepthShaderName);
                _waterInstance.AddMaterialToWaterRendering(_drawToDepthMaterial);
            }

            if (_copyDepthMaterial == null)
            {
                _copyDepthMaterial     = KWS_CoreUtils.CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.CopyDepthShaderName));
            }
        }

        public void Initialize(WaterSystem currentWater, RenderTargetIdentifier depthBuffer)
        {
            _waterInstance = currentWater;
            _depthBuffer = depthBuffer;
            InitializeMaterials();
            InitializeTextures();
        }

        public void Execute(Camera cam, CommandBuffer cmd)
        {
            cmd.BlitTriangleRTHandle(_sceneDepthRT, _copyDepthMaterial, ClearFlag.None, Color.clear);
            cmd.BlitTriangle(_sceneDepthRT, _sceneDepthRT.rtHandleProperties.rtHandleScale, _depthBuffer, _drawToDepthMaterial);
        }

        public void Release()
        {
            _sceneDepthRT?.Release();
            _isTexturesInitialized = false;
            KW_Extensions.SafeDestroy(_drawToDepthMaterial, _copyDepthMaterial);

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }
    }
}