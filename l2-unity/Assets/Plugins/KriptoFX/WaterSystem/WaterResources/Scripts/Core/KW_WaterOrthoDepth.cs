using UnityEngine;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public static class KW_WaterOrthoDepth
    {
        private const int WaterLayer = (1 << 4);

        [System.Serializable]
        public class OrthoDepthParams
        {
            [SerializeField] public int OtrhograpicSize;
            [SerializeField] public float PositionX;
            [SerializeField] public float PositionY;
            [SerializeField] public float PositionZ;
            public void SetData(int orthoSize, Vector3 pos)
            {
                OtrhograpicSize = orthoSize;
                PositionX = pos.x;
                PositionY = pos.y;
                PositionZ = pos.z;
            }
        }

        //public static void ReinitializeDepthTexture(TemporaryRenderTexture depth_rt, int size)
        //{
        //    depth_rt.RTAllocDepth("depthRT", size, size);
        //}

        public static Camera InitializeDepthCamera(float nearPlane, float farPlane, Transform parent)
        {

            var cameraGO = new GameObject("TopViewDepthCamera");
            cameraGO.transform.parent = parent;
            var depthCam = cameraGO.AddComponent<Camera>();

            depthCam.renderingPath = RenderingPath.Forward;
            depthCam.orthographic = true;
            depthCam.allowMSAA = false;
            depthCam.allowHDR = false;
            depthCam.nearClipPlane = nearPlane;
            depthCam.farClipPlane = farPlane;
            depthCam.transform.rotation = Quaternion.Euler(90, 0, 0);
            depthCam.enabled = false;

            return depthCam;
        }

        public static void RenderDepth(Camera cam, TemporaryRenderTexture depthRT, Vector3 position, int depthAreaSize, int depthTextureSize)
        {
            depthRT.AllocDepth("depthRT", depthTextureSize, depthTextureSize);

            cam.orthographicSize = depthAreaSize * 0.5f;
            cam.transform.position = position;
            cam.cullingMask = ~WaterLayer;
            cam.targetTexture = depthRT.rt;
            cam.Render();
        }

        public static void SaveDepthTextureToFile(TemporaryRenderTexture depthRT, string path)
        {
            if (depthRT.rt == null)
            {
                Debug.LogError("Can't save ortho depth");
                return;
            }
            var tempRT = new RenderTexture(depthRT.rt.width, depthRT.rt.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            Graphics.Blit(depthRT.rt, tempRT);
            tempRT.SaveRenderTextureToFile(path, TextureFormat.RFloat);
            tempRT.Release();
        }

        public static void SaveDepthDataToFile(OrthoDepthParams depthParams, string path)
        {
            KW_Extensions.SerializeToFile(path, depthParams);
        }

        public static void RenderAndSaveDepth(Transform parent, Vector3 position, int areaSize, int textureSize, float nearPlane, float farPlane, string pathToTexture, string pathToData)
        {
            var cam = InitializeDepthCamera(nearPlane, farPlane, parent);
            var depthRT = new TemporaryRenderTexture();
            depthRT.AllocDepth("depthRT", textureSize, textureSize);
            RenderDepth(cam, depthRT, position, areaSize, textureSize);

            var depthParams = new OrthoDepthParams();
            depthParams.SetData(areaSize, position);
            SaveDepthTextureToFile(depthRT, pathToTexture);
            SaveDepthDataToFile(depthParams, pathToData);
            KW_Extensions.SafeDestroy(cam.gameObject);
            depthRT.Release(true);
        }

    }
}