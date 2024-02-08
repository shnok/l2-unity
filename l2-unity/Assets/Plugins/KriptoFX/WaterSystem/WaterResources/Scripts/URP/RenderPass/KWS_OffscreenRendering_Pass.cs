using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public class KWS_OffscreenRendering_Pass : ScriptableRenderPass
    {
        public bool IsInitialized;

        KWS_OffscreenRendering_CommandPass pass = new KWS_OffscreenRendering_CommandPass();

        private WaterSystem _waterInstance;
        string              profilerTag;
        public KWS_OffscreenRendering_Pass(string profilerTag, RenderPassEvent renderPassEvent)
        {
            this.profilerTag     = profilerTag;
            this.renderPassEvent = renderPassEvent;
        }

        public void Initialize(WaterSystem currentWaterInstance)
        {
            _waterInstance = currentWaterInstance;
            pass.Initialize(_waterInstance);
            IsInitialized = true;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTargetDescriptor)
        {
            ConfigureTarget(pass.GetTargetColorTexture());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var           cam = renderingData.cameraData.camera;
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            cmd.Clear();

            pass.SetColorBuffer(renderingData.cameraData.renderer.cameraColorTarget);
            CoreUtils.SetRenderTarget(cmd, pass.GetTargetColorTexture());
            pass.Execute(cam, cmd);

            pass.SetColorBuffer(renderingData.cameraData.renderer.cameraColorTarget);
            pass.Execute_DrawToCameraBuffer(cam, cmd);

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