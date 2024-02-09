using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public class KWS_VolumetricLighting_Pass : ScriptableRenderPass
    {
        public bool                     IsInitialized;
        KWS_VolumetricLighting_CommandPass pass = new KWS_VolumetricLighting_CommandPass();

        private WaterSystem _waterInstance;
        string              profilerTag;
        public KWS_VolumetricLighting_Pass(string profilerTag, RenderPassEvent renderPassEvent)
        {
            this.profilerTag     = profilerTag;
            this.renderPassEvent = renderPassEvent;
        }

        public void Initialize(WaterSystem currentWaterInstance)
        {
            _waterInstance = currentWaterInstance;
            IsInitialized = true;
            pass.Initialize(_waterInstance);
#if UNITY_2021_1_OR_NEWER
            Shader.EnableKeyword("KW_POINT_SHADOWS_SUPPORTED");
#endif
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