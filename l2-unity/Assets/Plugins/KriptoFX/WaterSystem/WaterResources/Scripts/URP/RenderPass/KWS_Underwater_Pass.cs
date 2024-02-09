using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public class KWS_Underwater_Pass : ScriptableRenderPass
    {
        public bool IsInitialized;

        KWS_Underwater_CommandPass pass = new KWS_Underwater_CommandPass();
        private WaterSystem _waterInstance;
        string              profilerTag;

        public KWS_Underwater_Pass(string profilerTag, RenderPassEvent renderPassEvent)
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
            ConfigureTarget(pass.GetTargetColorBuffer());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var           cam = renderingData.cameraData.camera;
            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);
            cmd.Clear();

            if (!IsUnderwaterVisible(cam, _waterInstance.GetWorldSpaceBounds())) return;

            pass.SetColorBuffer(renderingData.cameraData.renderer.cameraColorTarget);
            CoreUtils.SetRenderTarget(cmd, renderingData.cameraData.renderer.cameraColorTarget);
            pass.Execute(cam, cmd, _waterInstance);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public void Release()
        {
            pass.Release();
        }

        private Vector4[] nearPlane = new Vector4[4];

        bool IsUnderwaterVisible(Camera cam, Bounds waterBounds)
        {
            nearPlane[0] = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            nearPlane[1] = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            nearPlane[2] = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
            nearPlane[3] = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

            if (IsPointInsideAABB(nearPlane[0], waterBounds)
                || IsPointInsideAABB(nearPlane[1], waterBounds)
                || IsPointInsideAABB(nearPlane[2], waterBounds)
                || IsPointInsideAABB(nearPlane[3], waterBounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IsPointInsideAABB(Vector3 point, Bounds box)
        {
            return (point.x >= box.min.x && point.x <= box.max.x) &&
                   (point.y >= box.min.y && point.y <= box.max.y) &&
                   (point.z >= box.min.z && point.z <= box.max.x);
        }
    }
}