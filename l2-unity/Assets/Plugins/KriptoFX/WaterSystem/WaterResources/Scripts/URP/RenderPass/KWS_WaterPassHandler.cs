using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KWS
{
    [ExecuteAlways]
    public class KWS_WaterPassHandler : MonoBehaviour
    {
        public WaterSystem WaterInstance;

        private ScriptableRendererData scriptableRendererData;
        private KWS_WaterSystemFeature _waterFeature;

        public void InitializeWaterRenderFeature(WaterSystem currentWaterInstance)
        {
            WaterInstance = currentWaterInstance;

            var pipeline = ((UniversalRenderPipelineAsset) GraphicsSettings.renderPipelineAsset);
            pipeline.supportsCameraOpaqueTexture = true;
            pipeline.supportsCameraDepthTexture  = true;
            var propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            scriptableRendererData = ((ScriptableRendererData[]) propertyInfo?.GetValue(pipeline))?[0];
            if (scriptableRendererData != null)
            {
                _waterFeature               = ScriptableObject.CreateInstance<KWS_WaterSystemFeature>();
                _waterFeature.name          = "WaterRenderFeature";
                _waterFeature.WaterInstance = WaterInstance;
                var features = scriptableRendererData.rendererFeatures;

                for (var i = 0; i < features.Count; i++)
                {
                    if(features[i] == null) features.RemoveAt(i);
                }

                features.Add(_waterFeature);

                scriptableRendererData.SetDirty();
            }
            else
            {
                Debug.Log("URP rendering requires correct ScriptableRendererData for the KWS water system");
            }
        }

        void OnDisable()
        {
            if (scriptableRendererData != null && _waterFeature != null)
            {
                _waterFeature.Release();
                scriptableRendererData.rendererFeatures.Remove(_waterFeature);
                scriptableRendererData.SetDirty();
                _waterFeature = null;
                scriptableRendererData = null;
            }
        }

        void OnDestroy()
        {
            OnDisable();
        }

//        public bool IsRequiredSetupRenderFeature()
//        {
//#if UNITY_EDITOR
//            var pipeline               = ((UniversalRenderPipelineAsset) GraphicsSettings.renderPipelineAsset);
//            var propertyInfo           = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
//            var scriptableRendererData = ((ScriptableRendererData[]) propertyInfo?.GetValue(pipeline))?[0];
//            if (scriptableRendererData != null)
//            {
//                var waterFeature = scriptableRendererData.rendererFeatures.OfType<KWS_WaterSystemRendererFeature>().FirstOrDefault();
//                if (waterFeature == null) return true;
//                else if (waterFeature.WaterGameObject == null) return true;
//                else return false;
//            }
//            return true;
//#else
//            Debug.LogError("Can't initialize render features at runtime");
//#endif
//        }

//        public void InitializeRenderFeature(WaterSystem currentWater)
//        {
//#if UNITY_EDITOR
//            if (Application.isPlaying) return;

//            WaterInstance = currentWater;
//            var pipeline     = ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset);
//            var propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
//            var scriptableRendererData = ((ScriptableRendererData[])propertyInfo?.GetValue(pipeline))?[0];
//            //if (scriptableRendererData != null)
//            //{
//            //    _waterFeature = scriptableRendererData.rendererFeatures.OfType<KWS_WaterSystemRendererFeature>().FirstOrDefault();
//            //    if (_waterFeature == null)
//            //    {
//            //        _waterFeature = new KWS_WaterSystemRendererFeature();
//            //        _waterFeature.name = "WaterRenderFeature";
//            //        _waterFeature.WaterInstance = WaterInstance;
//            //        scriptableRendererData.rendererFeatures.Add(_waterFeature);
//            //    }
//            //    scriptableRendererData.SetDirty();
//            //}

//#else
//            Debug.LogError("Can't initialize render features at runtime");
//#endif
//        }
    }

}
