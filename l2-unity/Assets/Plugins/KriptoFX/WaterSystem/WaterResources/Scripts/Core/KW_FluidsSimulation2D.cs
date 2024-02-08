
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KW_FluidsSimulation2D : MonoBehaviour
    {
        public WaterSystem WaterInstance;

        FluidsData[] fluidsData = new FluidsData[4];

        public Texture2D depth_tex;
        public Texture2D foamTex;
        public TemporaryRenderTexture prebakedFluidsRT = new TemporaryRenderTexture();
        public Texture2D prebakedFluidsTex2D;

        Material fluidsMaterial;
        private Vector3? lastPosition = null;
        private Vector3? lastPosition_lod1;
        private int frameNumber;
        int lastWidth;
        int lastHeight;
        float currentJitterOffsetTime;
        private bool isDepthTextureInitialized;

        private const string fluidSimulationShaderName = "Hidden/KriptoFX/KWS/FluidSimulation";
        private const string path_fluidsFolder = "FlowMaps";
        private const string path_fluidsPrebakedSim = "PrebakedFluidsSim";

        private const string path_fluidsDepthTexture = "KW_FluidsDepthTexture";
        private const string path_fluidsDepthData = "KW_FluidsDepthData";

        private int ID_KW_FluidsDepth = Shader.PropertyToID("KW_FluidsDepthTex");
        private int ID_KW_FluidsDepthOrthoSize = Shader.PropertyToID("KW_FluidsDepthOrthoSize");
        private int ID_KW_FluidsDepthNearFarDistance = Shader.PropertyToID("KW_FluidsDepth_Near_Far_Dist");
        private int ID_KW_FluidsDepthPos = Shader.PropertyToID("KW_FluidsDepthPos");

        private const int nearPlaneDepth = -2;
        private const int farPlaneDepth = 100;



        class FluidsData
        {
            public TemporaryRenderTexture DataRT = new TemporaryRenderTexture();
            public TemporaryRenderTexture FoamRT = new TemporaryRenderTexture();
            public RenderBuffer[] MRT = new RenderBuffer[2];
        }

        void InitializeTextures(int width, int height)
        {
            for (int i = 0; i < 4; i++)
            {
                if (fluidsData[i] == null) fluidsData[i] = new FluidsData();

                fluidsData[i].DataRT.Alloc("fluidsDataRT", width, height, 0, GraphicsFormat.R32G32B32A32_SFloat);
                fluidsData[i].FoamRT.Alloc("fluidsFoamRT", width, height, 0, GraphicsFormat.R8_UNorm);
                KWS_CoreUtils.ClearRenderTexture(fluidsData[i].DataRT.rt, KWS.ClearFlag.Color, Color.black);
                KWS_CoreUtils.ClearRenderTexture(fluidsData[i].FoamRT.rt, KWS.ClearFlag.Color, Color.black);
                fluidsData[i].MRT[0] = fluidsData[i].DataRT.rt.colorBuffer;
                fluidsData[i].MRT[1] = fluidsData[i].FoamRT.rt.colorBuffer;
            }
            lastWidth = width;
            lastHeight = height;
        }

        void LoadFoamTexture()
        {
            if (foamTex == null)
            {
                foamTex = Resources.Load<Texture2D>("KW_Foam");
                Shader.SetGlobalTexture("KW_FluidsFoamTex", foamTex);
            }
        }

        private async void LoadDepthTexture(string GUID)
        {
            isDepthTextureInitialized = true;
            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            var pathToDepthTex = Path.Combine(pathToBakedDataFolder, path_fluidsFolder, GUID, path_fluidsDepthTexture);
            var pathToDepthData = Path.Combine(pathToBakedDataFolder, path_fluidsFolder, GUID, path_fluidsDepthData);
            var depthParams = await KW_Extensions.DeserializeFromFile<KW_WaterOrthoDepth.OrthoDepthParams>(pathToDepthData);
            if (depthParams != null)
            {
                if (depth_tex == null) depth_tex = await KW_Extensions.ReadTextureFromFileAsync(pathToDepthTex);
                Shader.SetGlobalTexture(ID_KW_FluidsDepth, depth_tex);
                Shader.SetGlobalFloat(ID_KW_FluidsDepthOrthoSize, depthParams.OtrhograpicSize);
                Shader.SetGlobalVector(ID_KW_FluidsDepthNearFarDistance, new Vector3(nearPlaneDepth, farPlaneDepth, farPlaneDepth - nearPlaneDepth));
                Shader.SetGlobalVector(ID_KW_FluidsDepthPos, new Vector3(depthParams.PositionX, depthParams.PositionY, depthParams.PositionZ));
            }
        }

        void OnDisable()
        {
            for (int i = 0; i < 4; i++)
            {
                if (fluidsData[i] != null)
                {
                    fluidsData[i].DataRT.Release(true);
                    fluidsData[i].FoamRT.Release(true);
                }
            }
            prebakedFluidsRT.Release(true);
            KW_Extensions.SafeDestroy(fluidsMaterial, prebakedFluidsTex2D, depth_tex);
            lastPosition = null;
            lastWidth = 0;
            lastHeight = 0;
            frameNumber = 0;
            isDepthTextureInitialized = false;
        }

        public void Release()
        {
            OnDisable();
        }

        public int skipedFrames;

        public void AddMaterialsToWaterRendering(List<Material> waterShaderMaterials)
        {
            if (fluidsMaterial == null) fluidsMaterial = KWS_CoreUtils.CreateMaterial(fluidSimulationShaderName);
            if (!waterShaderMaterials.Contains(fluidsMaterial)) waterShaderMaterials.Add(fluidsMaterial);
        }

        Vector3 ComputeAreaSimulationJitter(float offsetX, float offsetZ)
        {
            var jitterSin = Mathf.Sin(currentJitterOffsetTime);
            var jitterCos = Mathf.Cos(currentJitterOffsetTime);
            var jitter = new Vector3(offsetX * jitterSin, 0, offsetZ * jitterCos);
            currentJitterOffsetTime += 2.0f;
            return jitter;
        }

        Vector3 RayToWaterPos(Camera currentCamera, float height)
        {
            var ray = currentCamera.ViewportPointToRay(new Vector3(0.5f, 0.0f, 0));
            var plane = new Plane(Vector3.down, height);
            float distanceToPlane;
            if (plane.Raycast(ray, out distanceToPlane))
            {
                return ray.GetPoint(distanceToPlane);
            }
            return currentCamera.transform.position;
        }

        public void SaveOrthoDepth(string GUID, Vector3 position, int areaSize, int texSize)
        {
            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            var pathToDepthTex = Path.Combine(pathToBakedDataFolder, path_fluidsFolder, GUID, path_fluidsDepthTexture);
            var pathToDepthData = Path.Combine(pathToBakedDataFolder, path_fluidsFolder, GUID, path_fluidsDepthData);
            KW_WaterOrthoDepth.RenderAndSaveDepth(transform, position, areaSize, texSize, nearPlaneDepth, farPlaneDepth, pathToDepthTex, pathToDepthData);
            isDepthTextureInitialized = false;
        }

        public void PrebakeSimulation(Vector3 waterPosition, int areaSize, int resolution, float flowSpeed, float foamStrength, string guid)
        {

            var areaPosition = waterPosition;

            areaPosition += ComputeAreaSimulationJitter(1f * areaSize / resolution, 1f * areaSize / resolution);
            if (lastPosition == null) lastPosition = areaPosition;
            var offset = areaPosition - lastPosition;


            if (lastWidth != resolution || lastHeight != resolution) InitializeTextures(resolution, resolution);
            LoadFoamTexture();
            if (!isDepthTextureInitialized) LoadDepthTexture(guid);

            Shader.SetGlobalFloat("KW_FluidsVelocityAreaScale", (0.5f * areaSize / 40f) * flowSpeed);
            Shader.SetGlobalVector("KW_FluidsMapWorldPosition_lod0", areaPosition);
            Shader.SetGlobalVector("KW_FluidsMapWorldPosition_lod1", areaPosition * 4);
            Shader.SetGlobalFloat("KW_FluidsMapAreaSize_lod0", areaSize);
            Shader.SetGlobalFloat("KW_FluidsMapAreaSize_lod1", areaSize * 4);
            Shader.EnableKeyword("KW_FLUIDS_PREBAKE_SIM");


            var target_lod0 = RenderFluidLod(fluidsData[2], fluidsData[3], flowSpeed * 1.0f, areaSize, foamStrength * 5f, (Vector3)offset, areaPosition, false);

            Shader.SetGlobalTexture("KW_Fluids_Lod0", target_lod0.DataRT.rt);
            Shader.SetGlobalTexture("KW_FluidsFoam_Lod0", target_lod0.FoamRT.rt);
            prebakedFluidsRT = target_lod0.DataRT;

            lastPosition = areaPosition;
            frameNumber++;

        }

        public void SavePrebakedSimulation(string GUID)
        {
            if (prebakedFluidsRT == null) return;

            var pathToSimDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            var tempRT = new RenderTexture(prebakedFluidsRT.rt.width, prebakedFluidsRT.rt.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            Graphics.Blit(prebakedFluidsRT.rt, tempRT, fluidsMaterial, 1);
            tempRT.SaveRenderTextureToFile(Path.Combine(pathToSimDataFolder, path_fluidsFolder, GUID, path_fluidsPrebakedSim), TextureFormat.RGBA32);

            tempRT.Release();
            tempRT = null;
            frameNumber = 0;
            Shader.DisableKeyword("KW_FLUIDS_PREBAKE_SIM");
        }

        public async Task<bool> ReadPrebakedSimulation(string GUID)
        {
            var pathToSimDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            if (prebakedFluidsTex2D != null) KW_Extensions.SafeDestroy(prebakedFluidsTex2D);

            var pathToFile = Path.Combine(pathToSimDataFolder, path_fluidsFolder, GUID, path_fluidsPrebakedSim);
            if (!File.Exists(pathToFile + ".gz"))
            {
                return false;
            }
            prebakedFluidsTex2D = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToSimDataFolder, path_fluidsFolder, GUID, path_fluidsPrebakedSim), true, FilterMode.Bilinear, TextureWrapMode.Clamp);
            Shader.SetGlobalTexture("KW_FluidsPrebaked", prebakedFluidsTex2D);
            return true;
        }

        public void RenderFluids(Camera currentCamera, Vector3 waterPosition, int areaSize, int resolution, float flowSpeed, float foamStrength, string GUID)
        {
            if (prebakedFluidsTex2D == null) return;
            var centerAreaPosition = RayToWaterPos(currentCamera, waterPosition.y);
            var areaPosition = centerAreaPosition + currentCamera.transform.forward * areaSize * 0.5f;

            areaPosition += ComputeAreaSimulationJitter(1f * areaSize / resolution, 1f * areaSize / resolution);
            if (lastPosition == null) lastPosition = areaPosition;
            var offset = areaPosition - lastPosition;

            var areaPosition_lod1 = centerAreaPosition;
            areaPosition_lod1 += ComputeAreaSimulationJitter(1f * areaSize / resolution, 1f * areaSize / resolution);
            if (lastPosition_lod1 == null) lastPosition_lod1 = areaPosition_lod1;
            var offset_lod1 = areaPosition_lod1 - lastPosition_lod1;

            if (lastWidth != resolution || lastHeight != resolution) InitializeTextures(resolution, resolution);
            LoadFoamTexture();
            if (!isDepthTextureInitialized) LoadDepthTexture(GUID);

            Shader.SetGlobalFloat("KW_FluidsRequiredReadPrebakedSim", frameNumber == 0 ? 1 : 0);
            Shader.SetGlobalFloat("KW_FluidsVelocityAreaScale", (0.5f * areaSize / 40f) * flowSpeed);
            Shader.SetGlobalVector("KW_FluidsMapWorldPosition_lod0", areaPosition);
            Shader.SetGlobalVector("KW_FluidsMapWorldPosition_lod1", areaPosition_lod1);
            Shader.SetGlobalFloat("KW_FluidsMapAreaSize_lod0", areaSize);
            Shader.SetGlobalFloat("KW_FluidsMapAreaSize_lod1", areaSize * 4);

            var target_lod1 = RenderFluidLod(fluidsData[0], fluidsData[1], flowSpeed * 0.5f, areaSize * 4, foamStrength * 2.5f, (Vector3)offset_lod1, areaPosition_lod1, false);
            var target_lod0 = RenderFluidLod(fluidsData[2], fluidsData[3], flowSpeed * 1.0f, areaSize, foamStrength * 5f, (Vector3)offset, areaPosition, true, target_lod1.DataRT.rt);

            Shader.SetGlobalTexture("KW_Fluids_Lod0", target_lod0.DataRT.rt);
            Shader.SetGlobalTexture("KW_FluidsFoam_Lod0", target_lod0.FoamRT.rt);

            Shader.SetGlobalTexture("KW_Fluids_Lod1", target_lod1.DataRT.rt);
            Shader.SetGlobalTexture("KW_FluidsFoam_Lod1", target_lod1.FoamRT.rt);

            lastPosition = areaPosition;
            lastPosition_lod1 = areaPosition_lod1;
            frameNumber++;

        }

        FluidsData RenderFluidLod(FluidsData data1, FluidsData data2, float flowSpeedMultiplier, float areaSize, float foamTexelOffset, Vector3 offset, Vector3 worldPos,
            bool canUseNextLod, RenderTexture nextLod = null)
        {

            if (canUseNextLod)
            {
                fluidsMaterial.EnableKeyword("CAN_USE_NEXT_LOD");
                fluidsMaterial.SetTexture("_FluidsNextLod", nextLod);
            }
            else fluidsMaterial.DisableKeyword("CAN_USE_NEXT_LOD");


            fluidsMaterial.SetFloat("_FlowSpeed", flowSpeedMultiplier);
            fluidsMaterial.SetFloat("_AreaSize", areaSize);
            fluidsMaterial.SetFloat("_FoamTexelOffset", foamTexelOffset);

            var source = (frameNumber % 2 == 0) ? data1 : data2;
            var target = (frameNumber % 2 == 0) ? data2 : data1;
            fluidsMaterial.SetVector("_CurrentPositionOffset", offset / areaSize);
            fluidsMaterial.SetVector("_CurrentFluidMapWorldPos", worldPos);
            Graphics.SetRenderTarget(target.MRT, target.DataRT.rt.depthBuffer);
            Graphics.Blit(source.DataRT.rt, fluidsMaterial, 0); // 设置source.DataRT.rt作为fluidsMaterial的maintex, 调用第0个pass, 并且把渲染结果画到上一行代码指定的RT上去
            return target;
        }
    }
}