using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public partial class KWS_CameraReflection
    {
        private ScriptableRenderContext lastContext;
        void SubscribeBeforeCameraRendering()
        {
            RenderPipelineManager.beginCameraRendering += RenderPipelineManagerOnbeginCameraRendering;
        }

        void UnsubscribeBeforeCameraRendering()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManagerOnbeginCameraRendering;
        }

        private void RenderPipelineManagerOnbeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            lastContext = ctx;
            OnBeforeCameraRendering(cam);
        }

        void SubscribeAfterCameraRendering()
        {
            RenderPipelineManager.endCameraRendering += RenderPipelineManagerOnendCameraRendering;
        }

        void UnsubscribeAfterCameraRendering()
        {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManagerOnendCameraRendering;
        }

        private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            OnAfterCameraRendering(cam);
        }

        void CameraRender(Camera cam)
        {
            UniversalRenderPipeline.RenderSingleCamera(lastContext, cam);
        }

        void InitializeCameraParamsSRP()
        {

        }

        void CopyCameraParamsSRP(Camera currentCamera, int cullingMask, WaterSystem.ReflectionClearFlagEnum clearFlag, Color clearColor)
        {

        }
    }
}