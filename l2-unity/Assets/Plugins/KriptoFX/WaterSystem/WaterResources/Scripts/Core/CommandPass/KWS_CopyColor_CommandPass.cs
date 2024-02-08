using UnityEngine;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KWS_CopyColor_CommandPass
    {
        KWS_RTHandle _colorRT;
        RenderTargetIdentifier _colorBuffer;

        bool _isTexturesInitialized;
        WaterSystem _waterInstance;
        private Material copyColorMaterial;

        public void InitializeMaterials()
        {
            if (copyColorMaterial == null) copyColorMaterial = KWS_CoreUtils.CreateMaterial(GetShaderNameForPipeline(KWS_ShaderConstants.ShaderNames.CopyColorShaderName));
        }

        void InitializeTextures()
        {
            if (_isTexturesInitialized) return;
            _colorRT = KWS_RTHandles.Alloc(Vector2.one, name: "_CameraOpaqueTexture", colorFormat: GetGraphicsFormatHDR());
            _isTexturesInitialized = true;

            KW_Extensions.WaterLog(this, _colorRT);
        }

        public void Initialize(WaterSystem currentWater, RenderTargetIdentifier colorBuffer)
        {
            _waterInstance = currentWater;
            _colorBuffer = colorBuffer;
            InitializeTextures();
            InitializeMaterials();
        }

        public RenderTargetIdentifier GetTargetColorBuffer()
        {
            return _colorRT;
        }

        public void Execute(Camera cam, CommandBuffer cmd)
        {
            cmd.BlitTriangleRTHandle(_colorBuffer, Vector4.one, _colorRT, copyColorMaterial, ClearFlag.None, Color.clear, 0);
            cmd.SetGlobalTexture(KWS_ShaderConstants_PlatformSpecific.CopyColorID._CameraOpaqueTexture, _colorRT); //we need to provide color source to compute shaders, so I can't update materials only, I need to update compute shader kernels too
            cmd.SetGlobalVector(KWS_ShaderConstants_PlatformSpecific.CopyColorID._CameraOpaqueTexture_RTHandleScale, _colorRT.rtHandleProperties.rtHandleScale); 
            //var waterMaterials = currentWater.GetWaterRenderingMaterials();
            //foreach (var waterMaterial in waterMaterials)
            //{
            //    waterMaterial.SetTexture(KWS_ShaderConstants.CopyColorID._CameraOpaqueTexture, colorRT.rt);
            //}
        }

        public void Release()
        {
            _colorRT?.Release();
            KW_Extensions.SafeDestroy(copyColorMaterial);
            _isTexturesInitialized = false;

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }

    }
}