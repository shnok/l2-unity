using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    public class KWS_WaterSystemFeature : ScriptableRendererFeature
    {
        //NOTE
        //by some reasons in URP I can't clear RenderTexture.GetTemporary after OnCameraCleanup,
        //it cause bugs with (material/cmd/shader).SetTexture and unity provide random textures.
        //Also GetNativeTexturePtr is changed when I move my mouse. WTF unity? 

        public WaterSystem WaterInstance;

        KWS_MaskDepthNormal_Pass maskDepthNormal_Pass;
        KWS_VolumetricLighting_Pass volumetricLighting_Pass;
        KWS_SSR_Pass ssr_Pass;
        KWS_Caustic_Pass caustic_Pass;
        KWS_Underwater_Pass underwater_Pass;
        private KWS_OffscreenRendering_Pass offscreenRendering_Pass;
        private KWS_DrawToDepth_Pass drawToDepth_Pass;

        private bool isInitialized;

        public override void Create()
        {


        }

        void InitializePasses()
        {
            maskDepthNormal_Pass = new KWS_MaskDepthNormal_Pass("Water.MaskDepthNormal_Pass", RenderPassEvent.BeforeRenderingSkybox);
            caustic_Pass = new KWS_Caustic_Pass("Water.Caustic_Pass", RenderPassEvent.BeforeRenderingSkybox);
            volumetricLighting_Pass = new KWS_VolumetricLighting_Pass("Water.VolumetricLighting_Pass", RenderPassEvent.BeforeRenderingSkybox);
            ssr_Pass = new KWS_SSR_Pass("Water.SSR_Pass", RenderPassEvent.BeforeRenderingTransparents);

            offscreenRendering_Pass = new KWS_OffscreenRendering_Pass("Water.OffscreenRendering_Pass", RenderPassEvent.BeforeRenderingTransparents);
            underwater_Pass = new KWS_Underwater_Pass("Water.Underwater_Pass", RenderPassEvent.AfterRenderingTransparents);

            drawToDepth_Pass = new KWS_DrawToDepth_Pass("Water.DrawToDepth_Pass", RenderPassEvent.BeforeRenderingPostProcessing);

            isInitialized = true;
        }

        // called every frame once per camera
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var cam = renderingData.cameraData.camera;

            if (WaterInstance == null || cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView) return;
            if (!IsWaterVisible(cam, WaterInstance)) return;

            if (!isInitialized) InitializePasses();

            var cameraSize = KWS_CoreUtils.GetCameraScreenSizeLimited(cam);
            KWS_RTHandles.SetReferenceSize(cameraSize.x, cameraSize.y, KWS.MSAASamples.None);

            if (WaterInstance.UseVolumetricLight || WaterInstance.UseCausticEffect || WaterInstance.UseUnderwaterEffect)
            {
                maskDepthNormal_Pass.Initialize(WaterInstance);
                renderer.EnqueuePass(maskDepthNormal_Pass);
            }
            else if (maskDepthNormal_Pass.IsInitialized) maskDepthNormal_Pass.Release();

            if (WaterInstance.UseCausticEffect)
            {
                caustic_Pass.Initialize(WaterInstance);
                renderer.EnqueuePass(caustic_Pass);
            }
            else if (caustic_Pass.IsInitialized) caustic_Pass.Release();

            if (WaterInstance.UseVolumetricLight)
            {
                volumetricLighting_Pass.Initialize(WaterInstance);
                renderer.EnqueuePass(volumetricLighting_Pass);
            }
            else if (volumetricLighting_Pass.IsInitialized) volumetricLighting_Pass.Release();

            if (WaterInstance.ReflectionMode == WaterSystem.ReflectionModeEnum.ScreenSpaceReflection)
            {
                ssr_Pass.Initialize(WaterInstance);
                renderer.EnqueuePass(ssr_Pass);
            }
            else if (ssr_Pass.IsInitialized) ssr_Pass.Release();

            if (WaterInstance.UseOffscreenRendering)
            {
                offscreenRendering_Pass.Initialize(WaterInstance);
                renderer.EnqueuePass(offscreenRendering_Pass);
            }
            else if (offscreenRendering_Pass.IsInitialized) offscreenRendering_Pass.Release();

            if (WaterInstance.UseUnderwaterEffect)
            {
                underwater_Pass.Initialize(WaterInstance);
                renderer.EnqueuePass(underwater_Pass);
            }
            else if (underwater_Pass.IsInitialized) underwater_Pass.Release();

            if (WaterInstance.DrawToPosteffectsDepth)
            {
                drawToDepth_Pass.Initialize(WaterInstance);
                renderer.EnqueuePass(drawToDepth_Pass);
            }
            else if (drawToDepth_Pass.IsInitialized) drawToDepth_Pass.Release();

        }
        bool IsWaterVisible(Camera cam, WaterSystem water)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            var bounds = water.GetWorldSpaceBounds();
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }


        public void Release()
        {
            if (!isInitialized) return;

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);

            maskDepthNormal_Pass.Release();
            caustic_Pass.Release();
            ssr_Pass.Release();
            volumetricLighting_Pass.Release();
            underwater_Pass.Release();
            offscreenRendering_Pass.Release();
            drawToDepth_Pass.Release();

            isInitialized = false;
        }

        protected override void Dispose(bool disposing)
        {
            Release();
        }
    }
}