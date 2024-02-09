using UnityEngine;
using UnityEngine.Rendering;

namespace KWS
{
    public partial class WaterSystem
    {
        KWS_WaterPassHandler waterPassHandler;
        KWS_CameraReflection reflection;

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

        void InitializeWaterPlatformSpecificResources()
        {
            if (waterPassHandler == null)
            {
                waterPassHandler = _tempGameObject.AddComponent<KWS_WaterPassHandler>();
                waterPassHandler.InitializeWaterRenderFeature(this);
            }



            if (reflection == null)
            {
                reflection = _tempGameObject.AddComponent<KWS_CameraReflection>();
                reflection.WaterInstance = this;
            }
            isWaterPlatformSpecificResourcesInitialized = true;
        }

        void RenderPlatformSpecificFeatures(Camera cam)
        {
            SetAmbientLightToShaders();
        }

        void ReleasePlatformSpecificResources()
        {
            isWaterPlatformSpecificResourcesInitialized = false;
        }

        void SetAmbientLightToShaders()
        {
            // return half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
            SphericalHarmonicsL2 sh;
            LightProbes.GetInterpolatedProbe(WaterWorldPosition, null, out sh);
            var ambient = new Vector3(sh[0, 0] - sh[0, 6], sh[1, 0] - sh[1, 6], sh[2, 0] - sh[2, 6]);
            ambient = Vector3.Max(ambient, Vector3.zero);
            Shader.SetGlobalVector(KWS_ShaderConstants.WaterID.KWS_AmbientColor, ambient);
        }
    }
}
