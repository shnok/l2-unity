using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public class KWS_DrawToDepth_Pass : ScriptableRenderPass
    {
        public bool IsInitialized;

        KWS_DrawToDepth_CommandPass pass = new KWS_DrawToDepth_CommandPass();
        private WaterSystem _waterInstance;
        string              profilerTag;

        private readonly RenderTargetIdentifier _cameraDepthTextureRT = new RenderTargetIdentifier("_CameraDepthTexture");

        public KWS_DrawToDepth_Pass(string profilerTag, RenderPassEvent renderPassEvent)
        {
            this.profilerTag     = profilerTag;
            this.renderPassEvent = renderPassEvent;
        }

        public void Initialize(WaterSystem currentWaterInstance)
        {
            _waterInstance = currentWaterInstance;
            pass.Initialize(_waterInstance, _cameraDepthTextureRT);
            IsInitialized = true;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTargetDescriptor)
        {
            ConfigureTarget(pass.GetTargetColorBuffer());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var           cam = renderingData.cameraData.camera;
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            cmd.Clear();

           
            pass.Execute(cam, cmd);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void Release()
        {
            pass.Release();
            IsInitialized = false;
        }
    }
}
