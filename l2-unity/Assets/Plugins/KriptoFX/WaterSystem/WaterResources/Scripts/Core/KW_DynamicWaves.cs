using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;

namespace KWS
{
    public class KW_DynamicWaves : MonoBehaviour
    {
        public WaterSystem WaterInstance;

        const string dynamicWavesShaderName = "Hidden/KriptoFX/KWS/DynamicWaves";
        string keyword_UseRainEffect = "KW_USE_RAIN_EFFECT";

        WavesData[] wavesData = new WavesData[3];
        private CommandBuffer cmd;

        private int frameNumber;
        //List<DrawPointInfo> drawPoints = new List<DrawPointInfo>(100);
        private int currentDrawIdx;
        private float[] drawPointsX = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];
        private float[] drawPointsY = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];
        private float[] drawPointsSize = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];
        private float[] drawPointsForce = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];

        public Mesh GridVBO;
        private Material dynamicWavesMaterial;
        public TemporaryRenderTexture dynamicObjectsRT = new TemporaryRenderTexture();

        private Vector3[] vertices;
        //  private Color[]       colors;
        private List<Vector2> forces;
        private List<Vector3> uv;
        private int[] triangles;

        private int lastWidth;
        private int lastHeight;

        float rainLeftFramesBeforeSpawn;

        public RenderTexture rt1;
        public RenderTexture rt2;


        class WavesData
        {
            public TemporaryRenderTexture DataRT = new TemporaryRenderTexture();
            public TemporaryRenderTexture NormalRT = new TemporaryRenderTexture();
            public RenderBuffer[] MRT = new RenderBuffer[2];
            public Vector3 WorldOffset;
            public int ID;
        }

        void InitializeTextures(int width, int height)
        {
            for (int i = 0; i < 3; i++)
            {
                if (wavesData[i] == null) wavesData[i] = new WavesData();
                wavesData[i].DataRT.Alloc("dynamicWavesDataRT", width, height, 0, GraphicsFormat.R32_SFloat);
                wavesData[i].NormalRT.Alloc("dynamicWavesNormalRT", width, height, 0, GraphicsFormat.R16G16_SFloat);
                wavesData[i].MRT[0] = wavesData[i].DataRT.rt.colorBuffer;
                wavesData[i].MRT[1] = wavesData[i].NormalRT.rt.colorBuffer;
                wavesData[i].ID = i + 1;
            }
            dynamicObjectsRT.Alloc("dynamicObjectsRT", width, height, 0, GraphicsFormat.R16_SFloat);
            rt1 = dynamicObjectsRT.rt;
            lastWidth = width;
            lastHeight = height;
        }

        void InitializeMaterials()
        { }


        void OnDisable()
        {
            for (int i = 0; i < 3; i++)
            {
                if (wavesData[i] != null)
                {
                    wavesData[i].DataRT.Release();
                    wavesData[i].NormalRT.Release();
                    wavesData[i].WorldOffset = Vector3.zero;
                }
            }
            KW_Extensions.SafeDestroy(dynamicWavesMaterial, GridVBO);
            dynamicObjectsRT.Release();
            //lastPosition = null;
            lastWidth = 0;
            lastHeight = 0;
            currentDrawIdx = 0;
            drawPointsX = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];
            drawPointsY = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];
            drawPointsSize = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];
            drawPointsForce = new float[KW_WaterDynamicScripts.DefaultInteractWavesCapacity];
        }

        public void Release()
        {
            OnDisable();
        }


        void IncreaseDrawArray()
        {
            var newSize = (int)(drawPointsX.Length * 1.5f); //increase capacity on 50%
            Array.Resize(ref drawPointsX, newSize);
            Array.Resize(ref drawPointsY, newSize);
            Array.Resize(ref drawPointsSize, newSize);
            Array.Resize(ref drawPointsForce, newSize);

            KW_Extensions.SafeDestroy(GridVBO);
            CreateGridVBO(newSize);
        }

        public void AddPositionToDrawArray(Vector3 areaPos, Vector3 position, float size, float force, float areaSize)
        {
            if (currentDrawIdx >= drawPointsX.Length) IncreaseDrawArray();

            areaSize *= 0.5f;
            position -= areaPos;
            drawPointsX[currentDrawIdx] = (position.x / areaSize) * 0.5f + 0.5f;
            drawPointsY[currentDrawIdx] = (position.z / areaSize) * 0.5f + 0.5f;
            drawPointsSize[currentDrawIdx] = size;
            drawPointsForce[currentDrawIdx] = force;
            ++currentDrawIdx;
        }


        private float interactiveWaterCurrentTime;
        private Vector3 lastInteractPos;
        private Vector3 lastInteractOffset;
        public Vector3 InteractPos; //relative to camera frustrum area position center
        private Vector2 shorelineWavesLastDepthPos = new Vector3(float.MaxValue, 0);
        private float lastWaterLevel = float.MaxValue;

        Vector3 ComputeAreaSimulationJitter(float offset)
        {
            var randTime = Time.time * UnityEngine.Random.Range(20, 60);
            var jitterSin = Mathf.Sin(randTime);
            var jitterCos = Mathf.Cos(randTime);
            var jitter = new Vector3(offset * jitterSin, 0, offset * jitterCos);

            return jitter;
        }
        //  public int DynamicWavesResolutionPerMeter = 40;
        //  public float DynamicWavesPropagationSpeed = 1;
        public void RenderWaves(Camera currentCamera, int fps, int areaSize, int pixelsPerMeter, float propagationSpeed, float rainStrength)
        {
            var bufferSize = pixelsPerMeter * areaSize;

            bufferSize = Mathf.Min(bufferSize, 2048);
            if (lastWidth != bufferSize || lastHeight != bufferSize) InitializeTextures(bufferSize, bufferSize);
            if (GridVBO == null) CreateGridVBO(KW_WaterDynamicScripts.DefaultInteractWavesCapacity);

            if (dynamicWavesMaterial == null) dynamicWavesMaterial = KWS_CoreUtils.CreateMaterial(dynamicWavesShaderName);

            int endIndex;
            var interactScripts = KW_WaterDynamicScripts.GetInteractScriptsInArea(InteractPos, areaSize, out endIndex);

            InteractPos = KW_Extensions.GetRelativeToCameraAreaPos(currentCamera, areaSize, transform.position.y);
            InteractPos += ComputeAreaSimulationJitter(5f / bufferSize);

            for (var i = 0; i < endIndex; i++)
            {
                var script = interactScripts[i];
                var force = script.GetForce(InteractPos.y);
                var intersectedSize = script.GetIntersectionSize();
                AddPositionToDrawArray(InteractPos, script.t.position + script.Offset, intersectedSize, force, areaSize);
            }
            UpdateVBO(areaSize);

            WaterInstance.SetFloats((KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesAreaSize, areaSize));
            WaterInstance.SetVectors((KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesWorldPos, InteractPos));
            WaterInstance.SetTextures((KWS_ShaderConstants.DynamicWaves.KW_DynamicObjectsMap, dynamicObjectsRT.rt));

            dynamicWavesMaterial.SetVector(KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesWorldPos, InteractPos);
            dynamicWavesMaterial.SetFloat(KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesAreaSize, areaSize);
            dynamicWavesMaterial.SetTexture(KWS_ShaderConstants.DynamicWaves.KW_DynamicObjectsMap, dynamicObjectsRT.rt);

            var offset = (InteractPos - lastInteractPos) / (areaSize);

            if (rainStrength > 0.001)
            {
                var rainThreshold = Mathf.Lerp(0.9999999f, 0.9992f, rainStrength);
                dynamicWavesMaterial.SetFloat("KW_DynamicWavesRainThreshold", rainThreshold);

                var scaledRainStrength = Mathf.Lerp(0.05f, 0.25f, Mathf.Pow(rainStrength, 5));
                dynamicWavesMaterial.SetFloat("KW_DynamicWavesRainStrength", scaledRainStrength);
                dynamicWavesMaterial.EnableKeyword(keyword_UseRainEffect);
            }
            else dynamicWavesMaterial.DisableKeyword(keyword_UseRainEffect);

            DrawInstancedArrayToTexture(dynamicObjectsRT.rt, dynamicWavesMaterial);
            UpdateDynamicWavesLod(wavesData[0], wavesData[1], wavesData[2], areaSize, propagationSpeed, offset);

            //if (quality == WaterSystem.QualityEnum.High)
            //{
            //    UpdateDynamicWavesLod(wavesData[0], wavesData[1], wavesData[2], areaSize, propagationSpeed, Vector3.zero);
            //}
            lastInteractPos = InteractPos;
            lastInteractOffset = offset;
        }


        void UpdateDynamicWavesLod(WavesData data1, WavesData data2, WavesData data3, int areaSize, float pixelSpeed, Vector3 offset)
        {

            WavesData lastSource, source, target;
            if (frameNumber == 0)
            {
                lastSource = data1;
                source = data2;
                target = data3;
            }
            else if (frameNumber == 1)
            {
                lastSource = data2;
                source = data3;
                target = data1;
            }
            else
            {
                lastSource = data3;
                source = data1;
                target = data2;
            }
            frameNumber++;
            if (frameNumber > 2) frameNumber = 0;
            target.WorldOffset = offset;


            dynamicWavesMaterial.SetFloat(KWS_ShaderConstants.DynamicWaves.KW_InteractiveWavesPixelSpeed, pixelSpeed);
            dynamicWavesMaterial.SetTexture(KWS_ShaderConstants.DynamicWaves._PrevTex, lastSource.DataRT.rt);
            dynamicWavesMaterial.SetVector(KWS_ShaderConstants.DynamicWaves.KW_AreaOffset, offset);
            dynamicWavesMaterial.SetVector(KWS_ShaderConstants.DynamicWaves.KW_LastAreaOffset, source.WorldOffset + offset);

            var activeRT = RenderTexture.active;
            Graphics.SetRenderTarget(target.MRT, target.DataRT.rt.depthBuffer);
            Graphics.Blit(source.DataRT.rt, dynamicWavesMaterial, 1);
            RenderTexture.active = activeRT;

            WaterInstance.SetTextures(
                (KWS_ShaderConstants.DynamicWaves.KW_DynamicWaves, target.DataRT.rt),
                (KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesPrev, source.DataRT.rt),
                (KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesNormal, target.NormalRT.rt),
                (KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesNormalPrev, source.NormalRT.rt));
            WaterInstance.SetFloats((KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesAreaSize, areaSize));

            dynamicWavesMaterial.SetFloat(KWS_ShaderConstants.DynamicWaves.KW_InteractiveWavesAreaSize, areaSize);
            dynamicWavesMaterial.SetTexture(KWS_ShaderConstants.DynamicWaves.KW_DynamicWaves, target.DataRT.rt);
            dynamicWavesMaterial.SetTexture(KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesPrev, source.DataRT.rt);
            dynamicWavesMaterial.SetTexture(KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesNormal, target.NormalRT.rt);
            dynamicWavesMaterial.SetTexture(KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesNormalPrev, source.NormalRT.rt);
            dynamicWavesMaterial.SetFloat(KWS_ShaderConstants.DynamicWaves.KW_DynamicWavesAreaSize, areaSize);

            rt2 = target.DataRT.rt;
        }


        void DrawInstancedArrayToTexture(RenderTexture rt, Material mat)
        {
            if (cmd == null) cmd = new CommandBuffer() { name = "Water.DynamicWavesObjects" };

            cmd.Clear();
            cmd.SetRenderTarget(rt);
            cmd.ClearRenderTarget(false, true, Color.clear);
            cmd.DrawMesh(GridVBO, Matrix4x4.identity, mat, 0);

            Graphics.ExecuteCommandBuffer(cmd);
        }

        void UpdateVBO(int areaSize)
        {
            int currentVertex = 0;
            for (int i = 0; i < currentDrawIdx; i++)
            {
                var size = (2 * drawPointsSize[i]) / areaSize;
                var halfSize = size / 2;

                vertices[currentVertex].x = drawPointsX[i] - halfSize;
                vertices[currentVertex].y = drawPointsY[i] - size * 0.2886751345948129f;

                vertices[currentVertex + 1].x = drawPointsX[i];
                vertices[currentVertex + 1].y = drawPointsY[i] + size * 0.5773502691896258f;

                vertices[currentVertex + 2].x = drawPointsX[i] + halfSize;
                vertices[currentVertex + 2].y = drawPointsY[i] - size * 0.2886751345948129f;

                //colors[currentVertex].r = colors[currentVertex + 1].r = colors[currentVertex + 2].r = drawPointsForce[i] * 0.5f + 0.5f;
                forces[currentVertex] = forces[currentVertex + 1] = forces[currentVertex + 2] = drawPointsForce[i] * Vector2.one;

                currentVertex += 3;
            }

            var count = vertices.Length;
            var zero = Vector2.zero;
            for (int i = currentVertex; i < count; i++)
            {
                vertices[i] = zero;
            }

            GridVBO.vertices = vertices;
            //GridVBO.colors = colors;
            GridVBO.SetUVs(1, forces);
            currentDrawIdx = 0;
        }

        void CreateGridVBO(int trisCount)
        {
            GridVBO = new Mesh();
            vertices = new Vector3[trisCount * 3];
            forces = new List<Vector2>();
            //colors = new Color[vertices.Length];
            uv = new List<Vector3>();
            triangles = new int[vertices.Length];

            for (int i = 0; i < vertices.Length; i += 3)
            {
                var offset = (float)i / vertices.Length;

                vertices[i] = new Vector3(offset, offset);
                vertices[i + 1] = new Vector3(1.0f / trisCount + offset, offset);
                vertices[i + 2] = new Vector3(offset, 1.0f / trisCount + offset);

                uv.Add(new Vector3(1, 0, 0));
                uv.Add(new Vector3(0, 1, 0));
                uv.Add(new Vector3(0, 0, 1));

                forces.Add(Vector2.zero);
                forces.Add(Vector2.zero);
                forces.Add(Vector2.zero);

                triangles[i] = i;
                triangles[i + 1] = i + 1;
                triangles[i + 2] = i + 2;
            }

            GridVBO.vertices = vertices;
            //GridVBO.colors = colors;
            GridVBO.triangles = triangles;
            GridVBO.SetUVs(0, uv);

            GridVBO.MarkDynamic();
        }

    }
}