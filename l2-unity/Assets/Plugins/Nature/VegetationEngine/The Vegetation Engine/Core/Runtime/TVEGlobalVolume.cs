// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;
using UnityEngine.Rendering;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TheVegetationEngine
{
    [HelpURL("https://docs.google.com/document/d/145JOVlJ1tE-WODW45YoJ6Ixg23mFc56EnB_8Tbwloz8/edit#heading=h.a39m1w5ouu94")]
    [ExecuteInEditMode]
    [AddComponentMenu("BOXOPHOBIC/The Vegetation Engine/TVE Global Volume")]
    public class TVEGlobalVolume : StyledMonoBehaviour
    {
        [StyledBanner(0.890f, 0.745f, 0.309f, "Global Volume")]
        public bool styledBanner;

        [StyledCategory("Camera Settings", 5, 10)]
        public bool cameraCat;

        [StyledMessage("Error", "Main Camera not found! Make sure you have a main camera with Main Camera tag in your scene! Particle elements updating will be skipped without it. Enter play mode to update the status!", 0, 10)]
        public bool styledCameraMessaage = false;

        [Tooltip("Sets the main camera used for scene rendering.")]
        public Camera mainCamera;

        [StyledCategory("Render Settings")]
        public bool textureCat;

        [Tooltip("Use the Seasons slider to control the element properties when the element is set to Seasons mode.")]
        [StyledRangeOptions("Texture Scale", 0, 2, new string[] { "Small", "0.5x", "Default", "1.5x", "Double" })]
        public float renderScale = 1f;

        [StyledCategory("Elements Settings")]
        public bool elementsCat;

#if UNITY_EDITOR
        [StyledMessage("Info", "Realtime Sorting is not supported for elements with GPU Instanceing enabled!", 0, 10)]
        public bool styledSortingMessaage = true;
#endif

        [Tooltip("Controls the elements visibility in scene and game view.")]
        public TVEElementsVisibilityMode elementsVisibility = TVEElementsVisibilityMode.HiddenAtRuntime;
        [HideInInspector]
        public TVEElementsVisibilityMode elementsVisibilityOld = TVEElementsVisibilityMode.HiddenAtRuntime;
        [Tooltip("Controls the elements sorting by element position. Always on in edit mode.")]
        public TVEElementsSortingMode elementsSorting = TVEElementsSortingMode.SortInEditMode;
        [Tooltip("Controls the elements fading at the volume edges if the Enable Volume Edge Fading support is toggled on the element material.")]
        [Range(0.0f, 1.0f)]
        public float elementsEdgeFade = 0.75f;

        [StyledCategory("Advanced Settings")]
        public bool dataCat;

#if UNITY_EDITOR
        [StyledMessage("Info", "The elements are rendered inside the Global Volume by default. Custom Render Volumes can be used to render the elements locally in higher resolution. They can be moved and scaled manually or attached to the camera or to the player!", 0, 10)]
        public bool styledInfoMessage = true;
#endif

        [Tooltip("Render mode used for Colors elements rendering.")]
        public TVEVolumeData renderColors = new TVEVolumeData();

        [Tooltip("Render mode used for Extras elements rendering.")]
        public TVEVolumeData renderExtras = new TVEVolumeData();

        [Tooltip("Render mode used for Motion elements rendering.")]
        public TVEVolumeData renderMotion = new TVEVolumeData();

        [Tooltip("Render mode used for Size elements rendering.")]
        public TVEVolumeData renderVertex = new TVEVolumeData();

#if THE_VEGETATION_ENGINE_DEBUG
        [Space(10)]
#endif

#if !THE_VEGETATION_ENGINE_DEBUG
        [System.NonSerialized]
#endif
        public List<TVERenderData> renderDataSet = new List<TVERenderData>();

#if !THE_VEGETATION_ENGINE_DEBUG
        [System.NonSerialized]
#endif
        public List<TVEElement> elementObjects = new List<TVEElement>();

#if !THE_VEGETATION_ENGINE_DEBUG
        [System.NonSerialized]
#endif
        public List<TVEInstanced> elementInstances = new List<TVEInstanced>();

        [StyledSpace(10)]
        public bool styledSpace0;

        [System.NonSerialized]
        public TVERenderData colorsData = new TVERenderData();
        [System.NonSerialized]
        public TVERenderData extrasData = new TVERenderData();
        [System.NonSerialized]
        public TVERenderData motionData = new TVERenderData();
        [System.NonSerialized]
        public TVERenderData vertexData = new TVERenderData();

        MaterialPropertyBlock propertyBlock;
        int propertyBlockArrCount = 0;

        Matrix4x4 projectionMatrix;
        Matrix4x4 modelViewMatrix = new Matrix4x4
        (
            new Vector4(1f, 0f, 0f, 0f),
            new Vector4(0f, 0f, -1f, 0f),
            new Vector4(0f, -1f, 0f, 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );

        void OnEnable()
        {
            gameObject.name = "Global Volume";
            gameObject.transform.SetSiblingIndex(3);

            //InitVolumeRendering();
            CreateRenderBuffers();

            SortElementObjects();
            SetElementsVisibility();
        }

        void OnDisable()
        {
            DestroyRenderBuffers();
        }

        void OnDestroy()
        {
            DestroyRenderBuffers();
        }

        void LateUpdate()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            if (elementsSorting == TVEElementsSortingMode.SortAtRuntime || sortDirty)
            {
                SortElementObjects();
            }

            if (elementsVisibilityOld != elementsVisibility)
            {
                SetElementsVisibility();

                elementsVisibilityOld = elementsVisibility;
            }

            UpdateRenderBuffers();
            ExecuteRenderBuffers();

            SetGlobalShaderParameters();

#if UNITY_EDITOR

            if (Selection.Contains(gameObject))
            {
                UpdateDebugData();

                if (elementsSorting == TVEElementsSortingMode.SortAtRuntime)
                {
                    styledSortingMessaage = true;
                }
                else
                {
                    styledSortingMessaage = false;
                }

                if (mainCamera == null)
                {
                    styledCameraMessaage = true;
                }
                else
                {
                    styledCameraMessaage = false;
                }
            }
#endif
        }

        public void InitVolumeRendering()
        {
            renderDataSet = new List<TVERenderData>();
            elementObjects = new List<TVEElement>();
            elementInstances = new List<TVEInstanced>();

            InitRenderData(colorsData, "TVE_Colors", "ColorsElement");
            InitRenderData(extrasData, "TVE_Extras", "ExtrasElement");
            InitRenderData(motionData, "TVE_Motion", "MotionElement");
            InitRenderData(vertexData, "TVE_Vertex", "VertexElement");

            UpdateRenderData(colorsData, renderColors);
            UpdateRenderData(extrasData, renderExtras);
            UpdateRenderData(motionData, renderMotion);
            UpdateRenderData(vertexData, renderVertex);

            renderDataSet.Add(colorsData);
            renderDataSet.Add(extrasData);
            renderDataSet.Add(motionData);
            renderDataSet.Add(vertexData);
        }

        public void UpdateVolumeRendering()
        {
            if (renderDataSet == null)
            {
                return;
            }

            if (colorsData == null || extrasData == null || motionData == null || vertexData == null)
            {
                return;
            }

            UpdateRenderData(colorsData, renderColors);
            UpdateRenderData(extrasData, renderExtras);
            UpdateRenderData(motionData, renderMotion);
            UpdateRenderData(vertexData, renderVertex);

            CreateRenderBuffers();
        }

        public void DisableVolumeRendering()
        {
            for (int e = 0; e < elementObjects.Count; e++)
            {
                var element = elementObjects[e];

                if (element != null)
                {
                    element.isActive = false;
                }
            }

            for (int e = 0; e < elementInstances.Count; e++)
            {
                var instances = elementInstances[e];

                for (int p = 0; p < instances.elements.Count; p++)
                {
                    if (instances.elements[p] != null)
                    {
                        instances.elements[p].isActive = false;
                    }
                }

                renderDataSet = new List<TVERenderData>();
                elementObjects = new List<TVEElement>();
                elementInstances = new List<TVEInstanced>();
            }
        }

        void InitRenderData(TVERenderData renderData, string rendererName, string materialTag)
        {
            renderData.renderName = rendererName;
            renderData.texName = rendererName + "Tex";
            renderData.texParams = rendererName + "Params";
            renderData.texLayers = rendererName + "Layers";
            renderData.texUsage = rendererName + "Usage";
            renderData.volCoords = rendererName + "Coords";
            renderData.volPosition = rendererName + "Position";
            renderData.volScale = rendererName + "Scale";

            renderData.materialTag = materialTag;

            renderData.renderDataID = materialTag.GetHashCode();
            renderData.bufferSize = -1;
            renderData.useRenderTextureArray = true;
            renderData.textureFormat = RenderTextureFormat.ARGBHalf;
        }

        void UpdateRenderData(TVERenderData renderData, TVEVolumeData volumeData)
        {
            if (volumeData.renderMode == TVEVolumeDataMode.Off)
            {
                renderData.renderMode = TVERenderDataMode.Off;
            }
            else if (volumeData.renderMode == TVEVolumeDataMode.InsideGlobalVolume)
            {
                renderData.renderMode = TVERenderDataMode.InsideGlobalVolume;
            }
            else if (volumeData.renderMode == TVEVolumeDataMode.InsideRenderVolume)
            {
                renderData.renderMode = TVERenderDataMode.InsideRenderVolume;
            }

            renderData.renderVolume = volumeData.renderVolume;
            renderData.textureWidth = volumeData.textureWidth;
            renderData.textureHeight = volumeData.textureHeight;
        }

        void CreateRenderBuffers()
        {
            for (int i = 0; i < renderDataSet.Count; i++)
            {
                var renderData = renderDataSet[i];

                if (renderData == null)
                {
                    continue;
                }

                CreateRenderBuffer(renderData);
            }
        }

        public void CreateRenderBuffer(TVERenderData renderData)
        {
            if (renderData.textureObject != null)
            {
                renderData.textureObject.Release();
            }

            if (renderData.commandBuffers != null)
            {
                for (int b = 0; b < renderData.commandBuffers.Length; b++)
                {
                    renderData.commandBuffers[b].Clear();
                }
            }

            renderData.bufferUsage = new float[9];

            // Might need initialization to avoid issues in the shaders
            for (int i = 0; i < renderData.bufferUsage.Length; i++)
            {
                renderData.bufferUsage[i] = 0f;
            }

            Shader.SetGlobalFloatArray(renderData.texUsage, renderData.bufferUsage);

            if (renderData.renderMode != TVERenderDataMode.Off && renderData.bufferSize > -1)
            {
                if (renderData.renderTexture != null)
                {
                    renderData.textureObject = renderData.renderTexture;
                }
                else
                {
                    int texWidth = Mathf.Max(Mathf.RoundToInt(renderData.textureWidth * renderData.textureScale * renderScale), 32); ;
                    int texHeight = Mathf.Max(Mathf.RoundToInt(renderData.textureHeight * renderData.textureScale * renderScale), 32); ;

                    renderData.textureObject = new RenderTexture(texWidth, texHeight, 0, renderData.textureFormat, 0);

                    if (renderData.useRenderTextureArray)
                    {
                        renderData.textureObject.dimension = TextureDimension.Tex2DArray;
                    }
                    else
                    {
                        renderData.textureObject.dimension = TextureDimension.Tex2D;
                    }

                    renderData.textureObject.volumeDepth = renderData.bufferSize + 1;
                    renderData.textureObject.name = renderData.texName;
                    renderData.textureObject.wrapMode = TextureWrapMode.Clamp;
                }

                renderData.commandBuffers = new CommandBuffer[renderData.bufferSize + 1];

                for (int b = 0; b < renderData.commandBuffers.Length; b++)
                {
                    renderData.commandBuffers[b] = new CommandBuffer();
                    renderData.commandBuffers[b].name = renderData.texName;
                }

                Shader.SetGlobalTexture(renderData.texName, renderData.textureObject);
                Shader.SetGlobalInt(renderData.texLayers, renderData.bufferSize + 1);
            }
            else
            {
                if (renderData.useRenderTextureArray)
                {
                    Shader.SetGlobalTexture(renderData.texName, Resources.Load<Texture2DArray>("Internal ArrayTex"));
                }
                else
                {
                    Shader.SetGlobalTexture(renderData.texName, Texture2D.whiteTexture);
                }

                Shader.SetGlobalInt(renderData.texLayers, 1);
            }
        }

        void UpdateRenderBuffers()
        {
            for (int d = 0; d < renderDataSet.Count; d++)
            {
                var renderData = renderDataSet[d];

                if (renderData == null || renderData.commandBuffers == null || renderData.renderMode == TVERenderDataMode.Off || !renderData.isRendering)
                {
                    continue;
                }

                var bufferParams = Shader.GetGlobalVector(renderData.texParams);

                for (int b = 0; b < renderData.commandBuffers.Length; b++)
                {
                    renderData.commandBuffers[b].Clear();
                    renderData.commandBuffers[b].ClearRenderTarget(true, true, bufferParams);
                    renderData.bufferUsage[b] = 0f;

                    for (int e = 0; e < elementObjects.Count; e++)
                    {
                        var element = elementObjects[e];

                        if (element.isActive == false || element == null)
                        {
                            elementObjects.RemoveAt(e);

                            continue;
                        }

                        if (renderData.renderDataID == element.renderDataID)
                        {
                            if (element.renderLayers[b] == 1)
                            {
                                // Optimization when particle elements are not used
                                if (element.elementMesh == null)
                                {
                                    Camera.SetupCurrent(mainCamera);
                                }

                                propertyBlock.SetVector("_ElementParams", element.elementParams);
                                element.elementRenderer.SetPropertyBlock(propertyBlock);

                                renderData.commandBuffers[b].DrawRenderer(element.elementRenderer, element.elementMaterial, 0, element.renderPass);
                                renderData.bufferUsage[b] = 1f;
                            }
                        }

                    }

                    for (int e = 0; e < elementInstances.Count; e++)
                    {
                        var instances = elementInstances[e];

                        if (!instances.material.enableInstancing)
                        {
                            continue;
                        }

                        if (instances.elements.Count == 0)
                        {
                            continue;
                        }

                        if (renderData.renderDataID == instances.renderDataID)
                        {
                            if (instances.renderLayers[b] == 1)
                            {
                                for (int p = 0; p < instances.elements.Count; p++)
                                {
                                    if (instances.elements[p].isActive == false || instances.elements[p] == null)
                                    {
                                        instances.elements.RemoveAt(p);
                                        instances.renderers.RemoveAt(p);
                                    }
                                }

                                var elementsCount = instances.elements.Count;

                                if (instances.matrices == null || instances.matrices.Length != elementsCount)
                                {
                                    instances.matrices = new Matrix4x4[elementsCount];
                                    instances.parameters = new Vector4[elementsCount];
                                }

                                for (int p = 0; p < elementsCount; p++)
                                {
                                    instances.matrices[p] = instances.renderers[p].localToWorldMatrix;
                                    instances.parameters[p] = instances.elements[p].elementParams;
                                }

                                if (elementsCount != propertyBlockArrCount)
                                {
                                    instances.propertyBlock = new MaterialPropertyBlock();
                                    propertyBlockArrCount = elementsCount;
                                }

                                if (elementsCount != 0)
                                {
                                    instances.propertyBlock.SetVectorArray("_ElementParams", instances.parameters);
                                }

                                renderData.commandBuffers[b].DrawMeshInstanced(instances.mesh, 0, instances.material, instances.renderPass, instances.matrices, elementsCount, instances.propertyBlock);
                                renderData.bufferUsage[b] = 1f;
                            }
                        }
                    }
                }

                Shader.SetGlobalFloatArray(renderData.texUsage, renderData.bufferUsage);

                //for (int u = 0; u < renderData.bufferUsage.Length; u++)
                //{
                //    Debug.Log(renderData.texUsage + " Index: " + u + " Usage: " + renderData.bufferUsage[u]);
                //}
            }
        }

        void ExecuteRenderBuffers()
        {
            for (int i = 0; i < renderDataSet.Count; i++)
            {
                var renderData = renderDataSet[i];

                if (renderData == null || renderData.commandBuffers == null || renderData.renderMode == TVERenderDataMode.Off || !renderData.isRendering)
                {
                    continue;
                }

                GL.PushMatrix();
                RenderTexture currentRenderTexture = RenderTexture.active;

                var position = gameObject.transform.position;
                var scale = gameObject.transform.lossyScale;

                if (renderData.renderMode == TVERenderDataMode.InsideRenderVolume)
                {
                    if (renderData.renderVolume != null)
                    {
                        position = renderData.renderVolume.transform.position;
                        scale = renderData.renderVolume.transform.lossyScale;
                    }
                }

                var gridX = scale.x / renderData.textureWidth;
                var gridZ = scale.z / renderData.textureHeight;
                var posX = Mathf.Round(position.x / gridX) * gridX;
                var posZ = Mathf.Round(position.z / gridZ) * gridZ;

                position = new Vector3(posX, position.y, posZ);

                var x = 1 / scale.x;
                var y = 1 / scale.z;
                var z = x * position.x - 0.5f;
                var w = y * position.z - 0.5f;
                var coords = new Vector4(x, y, -z, -w);

                Shader.SetGlobalVector(renderData.volCoords, coords);
                Shader.SetGlobalVector(renderData.volPosition, position);
                Shader.SetGlobalVector(renderData.volScale, scale);

                renderData.internalPosition = position;
                renderData.internalScale = scale;

                if (renderData.renderMode == TVERenderDataMode.ScreenSpaceProjection)
                {
                    if (mainCamera != null)
                    {
                        projectionMatrix = mainCamera.projectionMatrix;
                    }
                }
                else
                {
                    GL.modelview = modelViewMatrix;

                    projectionMatrix = Matrix4x4.Ortho(-scale.x / 2 + position.x,
                                                        scale.x / 2 + position.x,
                                                        scale.z / 2 - position.z,
                                                       -scale.z / 2 - position.z,
                                                       -scale.y / 2 + position.y,
                                                        scale.y / 2 + position.y);
                }

                GL.LoadProjectionMatrix(projectionMatrix);

                for (int b = 0; b < renderData.commandBuffers.Length; b++)
                {
                    Graphics.SetRenderTarget(renderData.textureObject, 0, CubemapFace.Unknown, b);
                    Graphics.ExecuteCommandBuffer(renderData.commandBuffers[b]);
                }

                RenderTexture.active = currentRenderTexture;
                GL.PopMatrix();
            }
        }

        void DestroyRenderBuffers()
        {
            for (int i = 0; i < renderDataSet.Count; i++)
            {
                var renderData = renderDataSet[i];

                if (renderData == null)
                {
                    continue;
                }

                if (renderData.textureObject != null)
                {
                    renderData.textureObject.Release();
                }

                if (renderData.commandBuffers != null)
                {
                    for (int b = 0; b < renderData.commandBuffers.Length; b++)
                    {
                        renderData.commandBuffers[b].Clear();
                    }
                }
            }
        }

        void SetGlobalShaderParameters()
        {
            Shader.SetGlobalFloat("TVE_ElementsFadeValue", elementsEdgeFade);
        }

        bool sortDirty = false;
        public void MarkSortDirty() => sortDirty = true;

        public void SortElementObjects()
        {
            elementObjects.Sort((e1, e2) =>
            {
                if (e1 == null && e2 == null) return 0;
                if (e1 == null) return -1;
                if (e2 == null) return 1;
                return e1.transform.position.y.CompareTo(e2.transform.position.y);
            });
            sortDirty = false;
            //for (int i = 0; i < elementObjects.Count - 1; i++) { 
            //  for (int j = 0; j < elementObjects.Count - 1; j++) {
            //    var prevElement = elementObjects[j];
            //    var nextElement = elementObjects[j + 1];

            //    if (prevElement != null && nextElement != null) {
            //      if (prevElement.gameObject.transform.position.y > nextElement.gameObject.transform.position.y) {
            //        var next = elementObjects[j + 1];
            //        elementObjects[j + 1] = elementObjects[j];
            //        elementObjects[j] = next;
            //      }
            //    }
            //  }
            //}
        }

        void SetElementsVisibility()
        {
            if (elementsVisibility == TVEElementsVisibilityMode.AlwaysHidden)
            {
                DisableElementsVisibility();
            }
            else if (elementsVisibility == TVEElementsVisibilityMode.AlwaysVisible)
            {
                EnableElementsVisibility();
            }
            else if (elementsVisibility == TVEElementsVisibilityMode.HiddenAtRuntime)
            {
                if (Application.isPlaying)
                {
                    DisableElementsVisibility();
                }
                else
                {
                    EnableElementsVisibility();
                }
            }
        }

        void EnableElementsVisibility()
        {
            for (int i = 0; i < elementObjects.Count; i++)
            {
                var element = elementObjects[i];

                if (element != null && element.customVisibility == TVEElementVisibilityMode.UseGlobalVolumeSettings)
                {
                    element.elementRenderer.enabled = true;
                }
            }
        }

        void DisableElementsVisibility()
        {
            for (int i = 0; i < elementObjects.Count; i++)
            {
                var element = elementObjects[i];

                if (element != null && element.customVisibility == TVEElementVisibilityMode.UseGlobalVolumeSettings)
                {
                    element.elementRenderer.enabled = false;
                }
            }
        }

#if UNITY_EDITOR
        void UpdateDebugData()
        {
            for (int i = 0; i < renderDataSet.Count; i++)
            {
                var renderData = renderDataSet[i];

                float memory = 0;
                float pixels = 0;

                if (renderData.renderMode != TVERenderDataMode.Off && renderData.bufferSize > -1)
                {
                    memory = renderData.textureObject.height * renderData.textureObject.width * (renderData.bufferSize + 1) * 0.00762939453125f / 1000f;
                    pixels = (renderData.textureObject.width / renderData.internalScale.x + renderData.textureObject.height / renderData.internalScale.z) / 2;
                }

                string debug = "<size=10>Memory: " + memory.ToString("F2") + " mb | Resolution: " + pixels.ToString("F2") + " pix/unit </size>";

                if (renderData.renderName == "TVE_Colors")
                {
                    renderColors.debugData = debug;
                }

                if (renderData.renderName == "TVE_Extras")
                {
                    renderExtras.debugData = debug;
                }

                if (renderData.renderName == "TVE_Motion")
                {
                    renderMotion.debugData = debug;
                }

                if (renderData.renderName == "TVE_Vertex")
                {
                    renderVertex.debugData = debug;
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.0f, 0.0f, 0.0f, 0.1f);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }

        void OnValidate()
        {
            UpdateVolumeRendering();
        }
#endif
    }
}
