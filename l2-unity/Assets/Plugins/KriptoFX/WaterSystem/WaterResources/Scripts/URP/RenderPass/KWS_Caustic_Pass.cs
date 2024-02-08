using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public class KWS_Caustic_Pass : ScriptableRenderPass
    {
        public bool IsInitialized;
        KWS_Caustic_CommandPass pass = new KWS_Caustic_CommandPass();

        private WaterSystem WaterInstance;
        string              profilerTag;
        public KWS_Caustic_Pass(string profilerTag, RenderPassEvent renderPassEvent)
        {
            this.profilerTag = profilerTag;
            this.renderPassEvent = renderPassEvent;
        }

        public void Initialize(WaterSystem currentWaterInstance)
        {
            WaterInstance = currentWaterInstance;
           
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

            pass.Initialize(WaterInstance, renderingData.cameraData.renderer.cameraColorTarget);
            IsInitialized = true;
            CoreUtils.SetRenderTarget(cmd, pass.GetTargetColorBuffer());
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