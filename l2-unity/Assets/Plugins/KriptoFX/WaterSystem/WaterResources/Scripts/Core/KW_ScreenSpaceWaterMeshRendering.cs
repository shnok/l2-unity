using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace KWS
{
    public class KW_ScreenSpaceWaterMeshRendering : MonoBehaviour
    {
        public RenderTexture WaterRT;
        private Material sceneCombineMaterial;

        private string cb_DrawMesh_Name = "Draw Water Mesh to Texture";
        private CommandBuffer cb_DrawMesh;
        private CameraEvent cb_Event = CameraEvent.AfterForwardAlpha;

        private string cb_BlitMesh_Name = "Blit Water Mesh to Target";
        private CommandBuffer cb_blitMesh;
        private CameraEvent cb_blitEvent = CameraEvent.AfterForwardAlpha;


        private const string WaterDepthShaderName = "Hidden/KriptoFX/Water/ScreenSpaceWaterMeshCombine";

        void InitializeTextures(int width, int height, int screenSpaceWaterMeshAA)
        {
            if (WaterRT != null) WaterRT.Release();
            WaterRT = new RenderTexture(width, height, 16, RenderTextureFormat.ARGBHalf);
            WaterRT.useMipMap = false;
            WaterRT.antiAliasing = screenSpaceWaterMeshAA;
        }

        public void RenderWaterMeshToTexture(Camera currentCamera, float resolution, int screenSpaceWaterMeshAA, Mesh mesh, Matrix4x4 matrix, Material material, Dictionary<CommandBuffer, CameraEvent> waterSharedBuffers)
        {
            if (cb_DrawMesh == null) cb_DrawMesh = new CommandBuffer() { name = cb_DrawMesh_Name };
            cb_DrawMesh.Clear();

            var sizeX = (int)(currentCamera.pixelWidth * resolution);
            var sizeY = (int)(currentCamera.pixelHeight * resolution);
            InitializeTextures(sizeX, sizeY, screenSpaceWaterMeshAA);

            cb_DrawMesh.SetRenderTarget(WaterRT);
            cb_DrawMesh.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            cb_DrawMesh.DrawMesh(mesh, matrix, material);

            if (!waterSharedBuffers.ContainsKey(cb_DrawMesh)) waterSharedBuffers.Add(cb_DrawMesh, cb_Event);
        }

        public void BlitToTargetColor(Dictionary<CommandBuffer, CameraEvent> waterSharedBuffers)
        {
            if (cb_blitMesh == null) cb_blitMesh = new CommandBuffer() { name = cb_BlitMesh_Name };
            else cb_blitMesh.Clear();

            if (sceneCombineMaterial == null) sceneCombineMaterial = KWS_CoreUtils.CreateMaterial(WaterDepthShaderName);
            Shader.SetGlobalTexture("KW_ScreenSpaceWater", WaterRT);

            cb_blitMesh.Blit(null, BuiltinRenderTextureType.CurrentActive, sceneCombineMaterial);

            if (!waterSharedBuffers.ContainsKey(cb_blitMesh)) waterSharedBuffers.Add(cb_blitMesh, cb_blitEvent);
        }

        public void Release()
        {
            if (WaterRT != null) WaterRT.Release();
            if (sceneCombineMaterial != null) KW_Extensions.SafeDestroy(sceneCombineMaterial);
        }

        void OnDisable()
        {
            Release();
        }
    }
}