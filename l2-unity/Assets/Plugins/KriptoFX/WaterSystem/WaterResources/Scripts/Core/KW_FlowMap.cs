using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KW_FlowMap : MonoBehaviour
    {
        public WaterSystem WaterInstance;

        private Material    _flowMaterial;
        private FlowMapData _currentFlowmapData;

        public  TemporaryRenderTexture _flowmapRT = new TemporaryRenderTexture();
        public  Texture2D     _flowMapTex2D;
        private Texture2D     _grayTex;
        private FlowMapData   _loadedFlowmapData;

        private Material FlowMaterial
        {
            get
            {
                if (_flowMaterial == null) _flowMaterial = CreateMaterial(KWS_ShaderConstants.ShaderNames.FlowMapShaderName);
                return _flowMaterial;
            }
        }

        public void Release()
        {
            _flowmapRT.Release();
            KW_Extensions.SafeDestroy(_flowMaterial, _flowMapTex2D, _grayTex);
        }

        private void OnDisable()
        {
            //print("FlowMap.Disabled");
            Release();
        }

        public void ClearFlowMap(string GUID)
        {
            var pathToDepthDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            var pathToTexture         = Path.Combine(pathToDepthDataFolder, KWS_Settings.DataPaths.FlowmapFolder, GUID, KWS_Settings.DataPaths.FlowmapTexture + ".gz");
            var pathToData            = Path.Combine(pathToDepthDataFolder, KWS_Settings.DataPaths.FlowmapFolder, GUID, KWS_Settings.DataPaths.FlowmapData    + ".gz");
            if (File.Exists(pathToTexture)) File.Delete(pathToTexture);
            if (File.Exists(pathToData)) File.Delete(pathToData);

            ClearRenderTexture(_flowmapRT.rt, ClearFlag.Color, new Color(0.5f, 0.5f, 0.5f, 0.5f));

            WaterInstance.SetTextures((KWS_ShaderConstants.FlowmapID.KW_FlowMapTex, _flowmapRT.rt));
        }

        public void InitializeFlowMapEditorResources(int size, int areaSize)
        {
            if (_flowmapRT.isInitialized && _flowmapRT.rt.width != size)
            {
                var tempRT = new TemporaryRenderTexture();
                tempRT.Alloc("_flowmapRT", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat);
                Graphics.Blit(_flowmapRT.rt, tempRT.rt);
                _flowmapRT.Release(true);
                _flowmapRT = tempRT;
            }
            else _flowmapRT.Alloc("_flowmapRT", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat, ClearFlag.Color, new Color(0.5f, 0.5f, 0.5f, 0.5f));
           

            if (_flowMapTex2D != null)
            {
                var activeRT = RenderTexture.active;
                Graphics.Blit(_flowMapTex2D, _flowmapRT.rt);
                RenderTexture.active = activeRT;
                KW_Extensions.SafeDestroy(_flowMapTex2D);
            }

            if (_currentFlowmapData == null) _currentFlowmapData = new FlowMapData();
            _currentFlowmapData.AreaSize    = areaSize;
            _currentFlowmapData.TextureSize = size;

            WaterInstance.SetTextures((KWS_ShaderConstants.FlowmapID.KW_FlowMapTex, _flowmapRT.rt));
        }


        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius, float brushStrength, bool eraseMode = false)
        {
            var brushSize = _currentFlowmapData.AreaSize / circleRadius;
            var uv        = new Vector2(brushPosition.x / _currentFlowmapData.AreaSize + 0.5f, brushPosition.z / _currentFlowmapData.AreaSize + 0.5f);
            if (brushMoveDirection.magnitude < 0.001f) brushMoveDirection = Vector3.zero;

            FlowMaterial.SetVector("_MousePos",  uv);
            FlowMaterial.SetVector("_Direction", new Vector2(brushMoveDirection.x, brushMoveDirection.z));
            FlowMaterial.SetFloat("_Size",          brushSize     * 0.75f);
            FlowMaterial.SetFloat("_BrushStrength", brushStrength / (circleRadius * 3));
            FlowMaterial.SetFloat("isErase",        eraseMode ? 1 : 0);

            DrawFlowmapPass(0);
        }

        public void SaveFlowMap(int areaSize, string GUID)
        {
            
            if (_currentFlowmapData == null) _currentFlowmapData = new FlowMapData {AreaSize = areaSize};

            var pathToDepthDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            _flowmapRT.rt.SaveRenderTextureToFile(Path.Combine(pathToDepthDataFolder, KWS_Settings.DataPaths.FlowmapFolder, GUID, KWS_Settings.DataPaths.FlowmapTexture), TextureFormat.RGBAHalf);
            KW_Extensions.SerializeToFile(Path.Combine(pathToDepthDataFolder, KWS_Settings.DataPaths.FlowmapFolder, GUID, KWS_Settings.DataPaths.FlowmapData), _currentFlowmapData);
        }

        public void RedrawFlowMap(int newAreaSize)
        {
            var uvScale = (float) newAreaSize / _currentFlowmapData.AreaSize;
            _currentFlowmapData.AreaSize = newAreaSize;
            FlowMaterial.SetFloat("_UvScale", uvScale);

            DrawFlowmapPass(1);
        }

        private void DrawFlowmapPass(int pass)
        {
            var tempRT = new TemporaryRenderTexture("_TempRT", _flowmapRT);
            var activeRT = RenderTexture.active;

            Graphics.Blit(_flowmapRT.rt, tempRT.rt, FlowMaterial, pass);
            Graphics.Blit(tempRT.rt,    _flowmapRT.rt);

            RenderTexture.active = activeRT;
            tempRT.Release();
        }

        public async Task<bool> ReadFlowMap(string GUID)
        {
            var pathToDepthDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();

            _loadedFlowmapData = await KW_Extensions.DeserializeFromFile<FlowMapData>(Path.Combine(pathToDepthDataFolder, KWS_Settings.DataPaths.FlowmapFolder, GUID, KWS_Settings.DataPaths.FlowmapData));

            if (_loadedFlowmapData == null || _loadedFlowmapData.AreaSize == 0)
            {
                WaterInstance.SetTextures((KWS_ShaderConstants.FlowmapID.KW_FlowMapTex, GrayScaleTexture()));
                Debug.LogError("Flow map reading data error. Probably not found flow map file or file version doesn't match with the water version. " +
                               "You need to draw new flow map in 'FlowMap Painter' and save it again.");
                return false;
            }

            _currentFlowmapData = _loadedFlowmapData;

            if (_flowMapTex2D != null) KW_Extensions.SafeDestroy(_flowMapTex2D);
            _flowMapTex2D = await KW_Extensions.ReadTextureFromFileAsync(Path.Combine(pathToDepthDataFolder, KWS_Settings.DataPaths.FlowmapFolder, GUID, KWS_Settings.DataPaths.FlowmapTexture), true, FilterMode.Bilinear, TextureWrapMode.Clamp);
            if (_flowMapTex2D == null) return false;

            WaterInstance.SetTextures((KWS_ShaderConstants.FlowmapID.KW_FlowMapTex, _flowMapTex2D));
            return true;
        }

        private Texture2D GrayScaleTexture()
        {
            if (_grayTex == null)
            {
                _grayTex = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
                var arr         = new Color[4];
                arr[0] = arr[1] = arr[2] = arr[3] = Color.gray;
                _grayTex.SetPixels(arr);
                _grayTex.Apply();
            }

            return _grayTex;
        }

        public FlowMapData GetFlowMapDataFromFile()
        {
            return _loadedFlowmapData;
        }

        [Serializable]
        public class FlowMapData
        {
            [SerializeField] public int AreaSize;
            [SerializeField] public int TextureSize;
        }
    }
}