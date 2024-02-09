using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public class KWS_MaskDepthNormal_Pass : ScriptableRenderPass
    {
        public bool IsInitialized;
        KWS_MaskDepthNormal_CommandPass pass = new KWS_MaskDepthNormal_CommandPass();

        private WaterSystem _waterInstance;
        string profilerTag;
        public KWS_MaskDepthNormal_Pass(string profilerTag, RenderPassEvent renderPassEvent)
        {
            this.profilerTag = profilerTag;
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
            ConfigureTarget(pass.GetTargetColorBuffer(), pass.GetTargetDepthBuffer());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var           cam = renderingData.cameraData.camera;
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            cmd.Clear();

            CoreUtils.SetRenderTarget(cmd, pass.GetTargetColorBuffer(), pass.GetTargetDepthBuffer());
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