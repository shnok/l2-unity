using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KW_Extensions;
using static KWS.KWS_CoreUtils;
using static KWS.KWS_ShaderConstants;

namespace KWS
{
    public class KWS_Caustic_CommandPass
    {
        WaterSystem _waterInstance;

        TemporaryRenderTexture _causticLod0 = new TemporaryRenderTexture();
        TemporaryRenderTexture _causticLod1 = new TemporaryRenderTexture();
        TemporaryRenderTexture _causticLod2 = new TemporaryRenderTexture();
        TemporaryRenderTexture _causticLod3 = new TemporaryRenderTexture();
        //TemporaryRenderTexture _causticFinal = new TemporaryRenderTexture();

        Texture2D _depthTex;

        private Material  _causticComputeMaterial;
        private Material  _causticDecalMaterial;
        //private Material _causticFastMaterial;
        private Mesh      _causticMesh;
        private Mesh      _decalMesh;
        private Mesh _fastCausticMesh;

        RenderTargetIdentifier _colorBuffer;
        KW_WaterOrthoDepth.OrthoDepthParams _depthParams;
        private int _currentMeshResolution;
        private AsyncInitializingStatusEnum _depthTextureInitStatus;
        
        static Vector4 LodSettings = new Vector4(10, 20, 40, 80);

        public void InitializeTextures()
        {
            var size = _waterInstance.CausticTextureSize;

            _causticLod0.Alloc("causticLod0", size, size, 0, GraphicsFormat.R8_UNorm, useMipMap: true, autoGenerateMips: true, mipMapCount: 3);
            _causticLod1.Alloc("causticLod1", size, size, 0, GraphicsFormat.R8_UNorm, useMipMap: true, autoGenerateMips: true, mipMapCount: 3);
            _causticLod2.Alloc("causticLod2", size, size, 0, GraphicsFormat.R8_UNorm);
            _causticLod3.Alloc("causticLod3", size, size, 0, GraphicsFormat.R8_UNorm);

            //var finalWidth = Mathf.Clamp(size, 256, 1024);
            //var finalHeight = Mathf.Clamp(size, 256, 512);
            //_causticFinal.Alloc("_causticFinal", finalHeight, finalWidth, 0, GraphicsFormat.R8_UNorm);
        }

        public RenderTargetIdentifier GetTargetColorBuffer()
        {
            return _causticLod0.rt;
        }

        public void InitializeMaterials()
        {
            if (_causticComputeMaterial == null)
            {
                _causticComputeMaterial = KWS_CoreUtils.CreateMaterial(ShaderNames.CausticComputeShaderName);
                _waterInstance.AddMaterialToWaterRendering(_causticComputeMaterial);
            }

            if (_causticDecalMaterial == null)
            {
                _causticDecalMaterial = KWS_CoreUtils.CreateMaterial(GetShaderNameForPipeline(ShaderNames.CausticDecalShaderName));
                _waterInstance.AddMaterialToWaterRendering(_causticDecalMaterial);
            }

            //if(_causticFastMaterial == null)
            //{
            //    _causticFastMaterial = KWS_CoreUtils.CreateMaterial("Hidden/KriptoFX/KWS/FastCaustic");
            //    _waterInstance.AddMaterialToWaterRendering(_causticFastMaterial);
            //}
        }

        public void Initialize(WaterSystem currentWater, RenderTargetIdentifier colorBuffer)
        {
            _waterInstance = currentWater;
            _colorBuffer = colorBuffer;
            if (!_causticLod0.isInitialized || _waterInstance.CausticTextureSize != _causticLod0.rt.width) InitializeTextures();
            InitializeMaterials();
        }

        public void Execute(Camera cam, CommandBuffer cmd)
        {
            var camT = cam.transform;

            ComputeCaustic(camT, cmd);
            DrawCausticDecal(cmd, camT);
            //DrawFastCaustic(cmd, cam);
        }

        public void Release()
        {
            _causticLod0.Release();
            _causticLod1.Release();
            _causticLod2.Release();
            _causticLod3.Release();
            //_causticFinal.Release();

            //KW_Extensions.SafeDestroy(_depthTex, _causticComputeMaterial, _causticDecalMaterial, _causticFastMaterial, _causticMesh, _fastCausticMesh, _decalMesh);
            KW_Extensions.SafeDestroy(_depthTex, _causticComputeMaterial, _causticDecalMaterial, _causticMesh, _fastCausticMesh, _decalMesh);

            _currentMeshResolution     = 0;
            _depthTextureInitStatus = AsyncInitializingStatusEnum.NonInitialized;

            KW_Extensions.WaterLog(this, "Release", KW_Extensions.WaterLogMessageType.Release);
        }

        private async void LoadDepthTexture(string GUID, CommandBuffer cmd, WaterSystem _waterInstance)
        {
            var pathToBakedDataFolder = KW_Extensions.GetPathToStreamingAssetsFolder();
            var pathToDepthTex        = Path.Combine(pathToBakedDataFolder, KWS_Settings.DataPaths.CausticFolder, GUID, KWS_Settings.DataPaths.CausticDepthTexture);
            var pathToDepthData       = Path.Combine(pathToBakedDataFolder, KWS_Settings.DataPaths.CausticFolder, GUID, KWS_Settings.DataPaths.CausticDepthData);
            _depthParams           = await KW_Extensions.DeserializeFromFile<KW_WaterOrthoDepth.OrthoDepthParams>(pathToDepthData);
            if (_depthParams != null)
            {
                if (_depthTex == null) _depthTex = await KW_Extensions.ReadTextureFromFileAsync(pathToDepthTex);
                UpdateDepthTextureParams(cmd, _waterInstance);
                _depthTextureInitStatus = AsyncInitializingStatusEnum.Initialized;
            }
            else _depthTextureInitStatus = AsyncInitializingStatusEnum.Failed;
        }

        void ComputeCaustic(Transform camT, CommandBuffer cmd)
        {
            if (_currentMeshResolution != _waterInstance.CausticMeshResolution) _causticMesh = GeneratePlane(_causticMesh, _waterInstance.CausticMeshResolution, 1.2f);

            if (_waterInstance.UseDepthCausticScale && _depthTextureInitStatus == AsyncInitializingStatusEnum.NonInitialized)
            {
                LoadDepthTexture(_waterInstance.WaterGUID, cmd, _waterInstance);
            }

            cmd.SetKeyword(CausticKeywords.USE_DEPTH_SCALE, _waterInstance.UseDepthCausticScale && _depthTextureInitStatus == AsyncInitializingStatusEnum.Initialized);

            var camPos = camT.position;
            var camDir = camT.forward;

            _waterInstance.SetKeywords(cmd, (CausticKeywords.USE_CAUSTIC_FILTERING, _waterInstance.UseCausticBicubicInterpolation));
            _waterInstance.SetFloats(cmd,
                (CausticID.KW_CaustisStrength, _waterInstance.CausticStrength),
                (CausticID.KW_CausticDepthScale, _waterInstance.CausticDepthScale));
            
            RenderLod(cmd, camPos, camDir, LodSettings.x, _causticLod0.rt);
            if (_waterInstance.CausticActiveLods > 1) RenderLod(cmd, camPos, camDir, LodSettings.y, _causticLod1.rt);
            if (_waterInstance.CausticActiveLods > 2) RenderLod(cmd, camPos, camDir, LodSettings.z, _causticLod2.rt);
            if (_waterInstance.CausticActiveLods > 3) RenderLod(cmd, camPos, camDir, LodSettings.w, _causticLod3.rt);
        }

        void RenderLod(CommandBuffer cmd, Vector3 camPos, Vector3 camDir, float lodSize, RenderTexture target)
        {
            var bakeCamPos = camPos + camDir * lodSize * 0.5f;

            cmd.SetGlobalFloat(CausticID.KW_MeshScale, lodSize);
            cmd.SetGlobalVector(CausticID.KW_CausticCameraOffset, bakeCamPos);

            KWS_SPR_CoreUtils.SetRenderTarget(cmd, target, ClearFlag.Color, Color.clear);
            cmd.DrawMesh(_causticMesh, Matrix4x4.identity, _causticComputeMaterial, 0);
        }

        void DrawCausticDecal(CommandBuffer cmd, Transform camT)
        {
            var camPos = camT.position;
            var camDir = camT.forward;

            var decalScale = LodSettings[_waterInstance.CausticActiveLods - 1] * 2;
            var decalPos = camPos;
            decalPos.y = _waterInstance.WaterMeshWorldPosition.y - 23;

            var lodDir = camDir * 0.5f;
            var useDepthScale = _waterInstance.UseDepthCausticScale && _depthTextureInitStatus == AsyncInitializingStatusEnum.Initialized;

            _waterInstance.SetTextures(cmd,
               (CausticID.KW_CausticLod0, _causticLod0.rt),
               (CausticID.KW_CausticLod1, _causticLod1.rt),
               (CausticID.KW_CausticLod2, _causticLod2.rt),
               (CausticID.KW_CausticLod3, _causticLod3.rt));
            _waterInstance.SetVectors(cmd,
                (CausticID.KW_CausticLodSettings, LodSettings),
                (CausticID.KW_CausticLodOffset, lodDir),
                (CausticID.KW_CausticLodPosition, camPos));
            _waterInstance.SetFloats(cmd, (CausticID.KW_DecalScale, decalScale));
            _waterInstance.SetKeywords(cmd,                
              (CausticKeywords.USE_LOD1, _waterInstance.CausticActiveLods == 2),
              (CausticKeywords.USE_LOD2, _waterInstance.CausticActiveLods == 3),
              (CausticKeywords.USE_LOD3, _waterInstance.CausticActiveLods == 4),
              (CausticKeywords.USE_DEPTH_SCALE, useDepthScale));

            if (useDepthScale) UpdateDepthTextureParams(cmd, _waterInstance);

            _causticDecalMaterial.SetFloat(CausticID.KW_CaustisStrength, _waterInstance.CausticStrength);
            if (_waterInstance.UseCausticDispersion)
            {
                var dispersionStrength = 1 - (Mathf.RoundToInt(Mathf.Log((int)_waterInstance.FFT_SimulationSize, 2)) - 5) / 4.0f; // 0 - 4 => 1-0
                if (dispersionStrength > 0.1f)
                {
                    dispersionStrength = Mathf.Lerp(dispersionStrength * 0.25f, dispersionStrength, _waterInstance.CausticTextureSize / 1024f);
                    _causticDecalMaterial.SetFloat(CausticID.KW_CausticDispersionStrength, dispersionStrength);
                }
            }
            _causticDecalMaterial.SetKeyword(CausticKeywords.USE_DISPERSION, _waterInstance.UseCausticDispersion);

            var decalTRS = Matrix4x4.TRS(decalPos, Quaternion.identity, new Vector3(decalScale, 50, decalScale));
            if (_decalMesh == null) GenerateDecalMesh();
            KWS_SPR_CoreUtils.SetRenderTarget(cmd, _colorBuffer);
            cmd.DrawMesh(_decalMesh, decalTRS, _causticDecalMaterial);

            //foreach (var waterMaterial in waterMaterials)
            //{
            //    waterMaterial.SetTexture("_causticRT", _causticFinal.rt);
            //}
        }

        //void DrawFastCaustic(CommandBuffer cmd, Camera cam)
        //{
        //    _fastCausticMesh = GeneratePlane(_fastCausticMesh, 256, 1.0f);
        //    KWS_SPR_CoreUtils.SetRenderTarget(cmd, _colorBuffer);

        //    _causticFastMaterial.SetMatrix("_CameraMatrix", cam.worldToCameraMatrix);
        //    cmd.DrawMesh(_fastCausticMesh, Matrix4x4.identity, _causticFastMaterial);
        //}

        void UpdateDepthTextureParams(CommandBuffer cmd, WaterSystem _waterInstance)
        {
            _waterInstance.SetTextures(cmd, (CausticID.KW_CausticDepthTex, _depthTex));
            _waterInstance.SetFloats(cmd, (CausticID.KW_CausticDepthOrthoSize, _depthParams.OtrhograpicSize));

            var nearFarDist = new Vector3(KWS_Settings.Caustic.CausticCameraDepth_Near, KWS_Settings.Caustic.CausticCameraDepth_Far, KWS_Settings.Caustic.CausticCameraDepth_Far - KWS_Settings.Caustic.CausticCameraDepth_Near);
            var causticDepthPos = new Vector3(_depthParams.PositionX, _depthParams.PositionY, _depthParams.PositionZ);
            _waterInstance.SetVectors(cmd, (CausticID.KW_CausticDepth_Near_Far_Dist, nearFarDist),
                                                 (CausticID.KW_CausticDepthPos, causticDepthPos));
        }

        Mesh GeneratePlane(Mesh mesh, int meshResolution, float scale)
        {
            _currentMeshResolution = meshResolution;
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.hideFlags   = HideFlags.DontSave;
                mesh.indexFormat = IndexFormat.UInt32;
            }

            var vertices  = new Vector3[(meshResolution + 1) * (meshResolution + 1)];
            var uv        = new Vector2[vertices.Length];
            var triangles = new int[meshResolution * meshResolution * 6];

            for (int i = 0, y = 0; y <= meshResolution; y++)
            for (var x = 0; x <= meshResolution; x++, i++)
            {
                vertices[i] = new Vector3(x * scale / meshResolution - 0.5f * scale, y * scale / meshResolution - 0.5f * scale, 0);
                uv[i]       = new Vector2(x * scale / meshResolution, y * scale / meshResolution);
            }

            for (int ti = 0, vi = 0, y = 0; y < meshResolution; y++, vi++)
            for (var x = 0; x < meshResolution; x++, ti += 6, vi++)
            {
                triangles[ti]     = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + meshResolution + 1;
                triangles[ti + 5] = vi + meshResolution + 2;
            }

            mesh.Clear();
            mesh.vertices  = vertices;
            mesh.uv        = uv;
            mesh.triangles = triangles;
            return mesh;
        }

        void GenerateDecalMesh()
        {
            Vector3[] vertices =
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
            };

            int[] triangles =
            {
                0, 2, 1, //face front
                0, 3, 2,
                2, 3, 4, //face top
                2, 4, 5,
                1, 2, 5, //face right
                1, 5, 6,
                0, 7, 4, //face left
                0, 4, 3,
                5, 4, 7, //face back
                5, 7, 6,
                0, 6, 7, //face bottom
                0, 1, 6
            };

            if (_decalMesh == null)
            {
                _decalMesh           = new Mesh();
                _decalMesh.hideFlags = HideFlags.DontSave;
            }

            _decalMesh.Clear();
            _decalMesh.vertices  = vertices;
            _decalMesh.triangles = triangles;
            _decalMesh.RecalculateNormals();
        }
    }
}