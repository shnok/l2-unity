using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static KWS.KW_Extensions;

namespace KWS
{
    public partial class WaterSystem
    {
        public int BakedFluidsSimPercentPassed { get; private set; }

        bool IsCanRendererForCurrentCamera()
        {
            if (Application.isPlaying)
            {
                return _currentCamera.cameraType == CameraType.Game;
            }
            else
            {
                return _currentCamera.cameraType == CameraType.SceneView;
            }
        }

        public void BakeFluidSimulation()
        {
            fluidsSimInitializingStatus = AsyncInitializingStatusEnum.BakingStarted;
            currentBakeFluidsFrames = 0;
        }

        void BakeFluidSimulationFrame()
        {
            if (currentBakeFluidsFrames < BakeFluidsLimitFrames)
            {
                currentBakeFluidsFrames++;
                BakedFluidsSimPercentPassed = (int)((100f / BakeFluidsLimitFrames) * currentBakeFluidsFrames);
                for (int j = 0; j < 10; j++)
                    fluidsSimulation.PrebakeSimulation(transform.position, FlowMapAreaSize, FluidsTextureSize * 2, FluidsSpeed, 0.1f, WaterGUID);
            }
            else
            {
                fluidsSimulation.SavePrebakedSimulation(WaterGUID);
                BakedFluidsSimPercentPassed = 0;
                ReadPrebakedFluidsSimulation();
                fluidsSimInitializingStatus = AsyncInitializingStatusEnum.Initialized;
                Debug.Log("Fluids obstacles saved!");
            }
        }

        public async Task ReadPrebakedFluidsSimulation()
        {
            await fluidsSimulation.ReadPrebakedSimulation(WaterGUID);
            fluidsSimInitializingStatus = AsyncInitializingStatusEnum.Initialized;
        }


        #region  Shoreline Methods

        public bool IsEditorAllowed()
        {
            if (_tempGameObject == null) return false;
            else return true;
        }

        public void Editor_RenderShorelineWavesWithFoam()
        {
            RenderShorelineWavesWithFoam();
        }

        void UpdateShorelineLods()
        {
            if (!IsCanRendererForCurrentCamera()) return;

            shorelineWaves.UpdateLodLevels(_currentCamera.transform.position, (int)FoamLodQuality, FoamCastShadows, FoamReceiveShadows);
        }

        public async Task BakeWavesToTexture()
        {
            var curvedSurfaceQualityScale = ShorelineCurvedSurfacesQuality == QualityEnum.High ? 20f : (ShorelineCurvedSurfacesQuality == QualityEnum.Medium ? 2.0f : 0.25f);
            await shorelineWaves.BakeWavesToTexture(ShorelineAreaSize, ShorelineAreaPosition, curvedSurfaceQualityScale, transform.position, WaterGUID);
        }


        public async Task InitialiseShorelineEditorResources()
        {
            await shorelineWaves.InitializeShorelineResources(WaterGUID);
        }

        public void SaveShorelineWavesParamsToDataFolder()
        {
            shorelineWaves.SavesWavesParamsToDataFolder(WaterGUID);
        }

        public void SaveShorelineToDataFolder()
        {
            shorelineWaves.SaveWavesToDataFolder(WaterGUID);
        }

        public void SaveShorelineDepth()
        {
            shorelineWaves.SaveOrthoDepth(WaterGUID);
        }

        public void ClearShorelineFoam()
        {
            shorelineWaves.ClearFoam();
        }
        public void ClearShorelineWavesWithFoam()
        {
            shorelineWaves.ClearShorelineWavesWithFoam(WaterGUID);
        }

        public async Task<List<KW_ShorelineWaves.ShorelineWaveInfo>> GetShorelineWavesData()
        {
            var result = await shorelineWaves.GetShorelineData(WaterGUID);
            return result.ShorelineWaves;
        }

        #endregion

        #region FlowMap Methods

        void InitializeFlowMapEditorResources()
        {
            flowMap.InitializeFlowMapEditorResources((int)FlowMapTextureResolution, FlowMapAreaSize);
            shoreLineInitializingStatus = AsyncInitializingStatusEnum.Initialized;
        }

        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius, float brushStrength, bool eraseMode = false)
        {
            InitializeFlowMapEditorResources();
            flowMap.DrawOnFlowMap(brushPosition, brushMoveDirection, circleRadius, brushStrength, eraseMode);
            flowmapInitializingStatus = AsyncInitializingStatusEnum.BakingStarted;
        }

        public void RedrawFlowMap(int newAreaSize)
        {
            InitializeFlowMapEditorResources();
            flowMap.RedrawFlowMap(newAreaSize);
        }


        public void SaveFlowMap()
        {
            InitializeFlowMapEditorResources();
            flowMap.SaveFlowMap(FlowMapAreaSize, WaterGUID);
        }

        public async Task ReadFlowMap()
        {
            var isInitialized = await flowMap.ReadFlowMap(WaterGUID);
            var flowData = flowMap.GetFlowMapDataFromFile();
            flowmapInitializingStatus = (isInitialized && flowData != null) ? AsyncInitializingStatusEnum.Initialized : AsyncInitializingStatusEnum.Failed;
            if (flowData == null) return;
            FlowMapTextureResolution = (FlowmapTextureResolutionEnum)flowData.TextureSize;
            FlowMapAreaSize = flowData.AreaSize;
        }

        public void ClearFlowMap()
        {
            flowMap.ClearFlowMap(WaterGUID);
        }

        #endregion


        #region Caustic Methods

        public void Editor_SaveCausticDepth()
        {
            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            var pathToDepthTex = Path.Combine(pathToBakedDataFolder, KWS_Settings.DataPaths.CausticFolder, WaterGUID, KWS_Settings.DataPaths.CausticDepthTexture);
            var pathToDepthData = Path.Combine(pathToBakedDataFolder, KWS_Settings.DataPaths.CausticFolder, WaterGUID, KWS_Settings.DataPaths.CausticDepthData);
            KW_WaterOrthoDepth.RenderAndSaveDepth(transform, CausticOrthoDepthPosition, CausticOrthoDepthAreaSize, CausticOrthoDepthTextureResolution, KWS_Settings.Caustic.CausticCameraDepth_Near, KWS_Settings.Caustic.CausticCameraDepth_Far, pathToDepthTex, pathToDepthData);
            Debug.Log("Caustic depth texture saved");
        }

        public void Editor_SaveFluidsDepth()
        {
            fluidsSimulation.SaveOrthoDepth(WaterGUID, transform.position, FlowMapAreaSize, (int)FlowMapTextureResolution);
        }



        #endregion
    }

}