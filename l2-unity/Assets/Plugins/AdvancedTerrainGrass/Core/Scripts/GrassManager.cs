/* last changes:
- cell definition/initialization
*/

/*
useful links:
https://forum.unity3d.com/threads/how-to-assign-matrix4x4-to-transform.121966/
http://answers.unity3d.com/questions/402280/how-to-decompose-a-trs-matrix.html
https://github.com/MattRix/UnityDecompiled/blob/master/UnityEngine/UnityEngine/Matrix4x4.cs
http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/index.htm
https://forum.unity3d.com/threads/problem-copying-real-time-lighting-data.488474/
copiedRenderer.realtimeLightmapIndex = sourceRenderer.realtimeLightmapIndex;
copiedRenderer.realtimeLightmapScaleOffset = sourceRenderer.realtimeLightmapScaleOffset;
*/

using System;
using System.Threading;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
    using UnityEditor;
#endif



namespace AdvancedTerrainGrass {

    [System.Serializable]
    public enum GrassCameras {
        AllCameras = 0,
        MainCameraOnly = 1
    }

    [System.Serializable]
    public enum RotationMode {
        AlignedRandomYAxis = 0,
        AlignedRandomYXAxes = 1,
        NotAlignedRandomYAxis = 2
    }

    [System.Serializable]
    public enum BucketsPerCell : int {
        _4x4 = 4,
        _8x8 = 8,
        _16x16 = 16,
        _32x32 = 32,
        _64x64 = 64
    }

    [RequireComponent(typeof(Terrain))]
    [ExecuteAlways]
    public class GrassManager : MonoBehaviour {

        private struct LayerData {
            [System.NonSerialized]
            public RenderParams rp;
            [System.NonSerialized]
            public GraphicsBuffer commandBuf;
            [System.NonSerialized]
            public GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;  
        }


//  ////////////////////////////////////////////////////

        public bool DrawInEditMode = false;
        public bool FollowCam = false;
        public Vector3 MainCamPos = Vector3.zero;
        public Vector3 MainCamRot = Vector3.zero;

        public GrassCameras rtCameraSelection;
        public bool rtIngnoreOcclusion = true;
        public float rtBurstRadius = 50.0f;

//  ////////////////////////////////////////////////////

        public Camera Cam;
        public bool IngnoreOcclusion = true;
        private Transform CamTransform;


        public bool showGrid = false;
        private GameObject go;
        public Material ProjMat;

        // public /*static*/ System.Random random = new System.Random(1234); // Must not be static in case we have multiple instances / We use a custom seed to make sure that grass will look the same each time we enter playmode
        public ulong ATGSeed; // changed to ulong for IL2CPP and Android
        public static float OneOverInt32MaxVal = 1.0f / (float)uint.MaxValue;

        public Terrain ter;
        public TerrainData terData;

        public GrassTerrainDefinitions SavedTerrainData;

        public bool useBurst = false;
        public float BurstRadius = 50.0f;

        [Range(1,8)]
        public int CellsPerFrame = 1;

        public float DetailDensity = 1;
            private float CurrentDetailDensity;

//  URP
        public bool usingSRP = false;
//  VR
        private bool usingSinglePassInstanced = false;
        private bool useHizCulling = true;

        private ComputeBuffer t_argsbuffer;
        uint[] t_args = new uint[5] { 0, 0, 0, 0, 0 };

//  Terrain
        private Vector3 TerrainPosition;
        private Vector3 TerrainSize;
        private Vector3 OneOverTerrainSize;

        private Vector2 TerrainDetailSize;
        private float SqrTerrainCullingDist;

// Threading
        private bool ThreadIsRunning = false;
        private Thread WorkerThread = null;
        private EventWaitHandle WorkerThreadWait = new EventWaitHandle(true, EventResetMode.ManualReset);
        private EventWaitHandle MainThreadWait = new EventWaitHandle(true, EventResetMode.ManualReset);
        private List<int> wt_cellindexList = new List<int>();
        private int wt_cellindexListCount;
        private bool stopThread = false;
        private object threadEndSync = new object();

// Height
        private int TerrainHeightmapWidth;
        private int TerrainHeightmapHeight;
        private float[] TerrainHeights;
        private float OneOverHeightmapWidth;
        private float OneOverHeightmapHeight;
        private float TerrainSizeOverHeightmap;
        private float OneOverHeightmapWidthRight;
        private float OneOverHeightmapHeightUp;

// Culling
        public GrassCameras CameraSelection;
        private Camera CameraInWichGrassWillBeDrawn; // default null -> all cameras
        public int CameraLayer = 0;

        public uint renderingLayerMask = 1; // default

        private CullingGroup cullingGroup;
        private BoundingSphere[] boundingSpheres;
            private BoundingSphere[] origBoundingSpheres;
        private int numResults = 0;
        private int[] resultIndices;
        private bool[] isVisibleBoundingSpheres;
        private int numOfVisibleCells = 0;
        public float CullDistance = 80.0f;
            private float CurrentCullDistance;
        public float FadeLength = 20.0f;
            private float CurrentFadeLength;
        public float CacheDistance = 120.0f;
            private float CurrentCacheDistance;

            public float VertexFadeStart = 10.0f;
            private float CurrentVertexFadeStart;
        public float VertexFadeLength = 10.0f;
            private float CurrentVertexFadeLength;
        
        public float DetailFadeStart = 20.0f;
            private float CurrentDetailFadeStart;
        public float DetailFadeLength = 30.0f;
            private float CurrentDetailFadeLength;
        
        public float ShadowStart = 10.0f;
            private float CurrentShadowStart;
        public float ShadowFadeLength = 20.0f;
            private float CurrentShadowFadeLength;
        
        public float ShadowStartFoliage = 30.0f;
            private float CurrentShadowStartFoliage;
        public float ShadowFadeLengthFoliage = 20.0f;
            private float CurrentShadowFadeLengthFoliage;

        private Vector4 GrassFadeProperties;

        public float CullingGroupDistanceFactor = 1.0f;

// Lighting
        public LightProbeProxyVolume lppv;
        private bool useLppv = false;
        private LightProbeUsage lpUsage = LightProbeUsage.Off;

// Layers
        [Space(12)]
        private int NumberOfLayers;
        private int OrigNumberOfLayers;
        public Mesh[] v_mesh;
        public Material[] v_mat;
        public float[] v_clip;
        // public Vector4[] v_bounds;
        
        private Vector3[] v_boundsCenter;
        private Vector3[] v_boundsExtents;
        
        public RotationMode[] InstanceRotation;
        public bool[] WriteNormalBuffer;
        public ShadowCastingMode[] ShadowCastingMode;
        public MotionVectorGenerationMode[] motionVectorGenerationMode;
        public float[] MinSize;
        public float[] MaxSize;
        public float[] Noise;
        public int[] LayerToMergeWith;
        public bool[] DoSoftMerge;
        public int[][] SoftlyMergedLayers;
        
//  Compute
        public bool rtUseCompute = false; // The variable exposed in the inspector. Real UseCompute depends on support and if we are in edit mode.
        public bool UseCompute = false;
        public float GrassInstanceSize = 2.0f;

        private int NumberOfFinalLayers;
        private int[] FinalLayersIndices; // As we might merge layers we have to store pointers
        //private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        private GraphicsBuffer[] MergedCellcontentsBuffer;
        //private ComputeBuffer[] argsBuffer;
        private MaterialPropertyBlock[] MergedMatrixBlock;
        private int[] LayersMaxDensity;

        private ComputeShader ComputeCellcontentsBufferShader;
        private int ComputeCellcontentsKernel;
        private int SourceCellcontentsBufferID;
        private int MergedCellcontentsBufferID;

//  RenderMesh Stuff
        LayerData[] layerData;

        private int AtgVPPID;
        private int AtgVPID;
        private int AtgCameraPositionPID;
            private int AtgSurfaceCameraPositionPID;
        private int AtgCameraForwardPID;
        private int AtgInstanceCountPID;
        private int AtgCullDistSqrPID;
        private int AtgGrassFadePropsPID;
        private int AtgTerrainShiftComputePID;
        private int AtgClipValPID;

        private int AtgBoundsCenterPID;
        private int AtgBoundsExtentsPID;

        //private int AtgBoundingSpherePID;

        private float[] VP_MatrixAsFloats = new float[16];
        private Matrix4x4 VP_Matrix;
        //private float[] V_MatrixAsFloats = new float[16];
        //private Matrix4x4 V_Matrix;

        public AtgHiZBuffer HiZBuffer;  // changed to public to support CutScenes
        private int AtgHiZBufferPID;
        private int AtgHiZBufferSizePID;

        private ComputeShader DoubleCopyCountShader;
        private int DoubleCopyCountKernel;
        private int ArgsBufferPID;

//  VR
        public bool vrForceSinglePassInstanced = false;

//  Terrain Shift
        public bool EnableTerrainShift;
        private Vector3 TerrainShift;
        private int AtgTerrainShiftSurfacePID;

//  Basics
        private byte[][] mapByte;            // Density Maps
        private static readonly int GrassMatrixBufferPID = Shader.PropertyToID("GrassMatrixBuffer");

        private int TotalCellCount;         // Number of Cells
        private int NumberOfCells;          // Number of Cells per Axis
        public  BucketsPerCell NumberOfBucketsPerCellEnum = BucketsPerCell._16x16; // Number of Patches per Cell per Axis (Detail Resolution)
        private int NumberOfBucketsPerCell;

        private float CellSize = 0.0f;
        private float BucketSize;
        //private float OneOverBucketSize;

//  Safe Guards - regarding culling groups
        private bool firstFrameSkipped = false;

//  The shared tempArrays as used by the workerthread
        private int maxBucketDensity = 0;
        private Matrix4x4[] tempMatrixArray;
//

        private Vector2 samplePosition;
        private Vector2 tempSamplePosition;

        private GrassCell[] Cells;
        private GrassCellContent[] CellContent;
        private List<int> CellsOrCellContentsToInit = new List<int>();

        // private Shader sh;

        private static readonly int GrassFadePropsPID = Shader.PropertyToID("_AtgGrassFadeProps");
        private Vector4 GrassFadeProps;
        private static readonly int GrassShadowFadePropsPID = Shader.PropertyToID("_AtgGrassShadowFadeProps");
        private Vector2 GrassShadowFadeProps;
        private static readonly int VertexFadePropsPID = Shader.PropertyToID("_AtgVertexFadeProps");

        //private Vector3 ZeroVec = Vector3.zero;
        //private Matrix4x4 ZeroMatrix = Matrix4x4.identity;
        //private Vector4 Zero4Vec = Vector4.zero;

        private Matrix4x4 tempMatrix = Matrix4x4.identity;
        private Vector3 tempPosition;
        private Quaternion tempRotation;
        private Vector3 tempScale;

        //private Vector3 UpVec = Vector3.up;
        private Quaternion ZeroQuat = Quaternion.identity;

//  Editor variables
        public bool FreezeSizeAndColor = false;
        
        public bool DebugStats = false;
        public bool DebugCells = false;

        public bool FirstTimeSynced = false;
        public int LayerEditMode = 0;
        public int LayerSelection = 0;
        public bool Foldout_Rendersettings = true;
        public bool Foldout_Advancedsettings = true;
        public bool Foldout_Prototypes = true;



//  --------------------------------------------------------------------

        void Cleanup() {
            if(cullingGroup != null) {
                cullingGroup.Dispose();
                cullingGroup = null;
            }
        //  Release ComputeBuffers
            if (CellContent != null) {
                for (int i = 0; i < CellContent.Length; i ++) {
                    CellContent[i].ReleaseCompleteCellContent();
                }
            }
            if (MergedCellcontentsBuffer != null) {
                for (int i = 0; i < MergedCellcontentsBuffer.Length; i++) {
                    MergedCellcontentsBuffer[i]?.Release();
                    MergedCellcontentsBuffer[i] = null;
                }
            }
        //  Release Layer Data
            if (layerData != null)
            {
                for (int i = 0; i < layerData.Length; i++) {
                    layerData[i].commandBuf?.Release();
                    layerData[i].commandBuf = null;
                }    
            }
        //  Shut down the worker thread
            lock (threadEndSync) {
                stopThread = true;
            }
            if (WorkerThread != null) {
                WorkerThreadWait.Set();
                WorkerThread.Join();
                WorkerThread = null;
            }
        //  Release Cells and CellContents
            Cells = null;
            CellContent = null;
            mapByte = null;
            TerrainHeights = null;
            tempMatrixArray = null;
            SoftlyMergedLayers = null;
        }

        public void UpdateGrass() {
            #if UNITY_EDITOR
                Cleanup();
                Init();
                DrawGrass();
            #endif
        }

//  --------------------------------------------------------------------

        void OnEnable() {
            #if UNITY_EDITOR
                Invoke("Init", 0.0f);
            #else
                Init();
            #endif
        }

    //  Make sure we clean up everything
        void OnDisable() {
            Cleanup();
        }

//  --------------------------------------------------------------------
//  Update function

        void LateUpdate () {    
            DrawGrass();
        }

//  --------------------------------------------------------------------
//  Custom Random function, changed to ulong for IL2CPP and Android

        public /*static*/ float GetATGRandomNext() {
            ATGSeed = (((ulong)ATGSeed * (ulong)279470273) % (ulong)4294967291);
            return (float)ATGSeed * OneOverInt32MaxVal;
        }

//  --------------------------------------------------------------------
//  Function to change rendering settings at run time
        
        public void RefreshGrassRenderingSettings(
            float t_DetailDensity,
            float t_CullDistance,
            float t_FadeLength,
            float t_CacheDistance,
            float t_DetailFadeStart,
            float t_DetailFadeLength,
            float t_ShadowStart,
            float t_ShadowFadeLength,
            float t_ShadowStartFoliage,
            float t_ShadowFadeLengthFoliage
            )
        {
        
        //  Remove already queued cells
            CellsOrCellContentsToInit.Clear();

        //  Set new culling values
            CurrentDetailDensity = t_DetailDensity;
            CurrentCullDistance = t_CullDistance;
            CurrentFadeLength = t_FadeLength;
            CurrentCacheDistance = t_CacheDistance;
            CurrentDetailFadeStart = t_DetailFadeStart;
            CurrentDetailFadeLength = t_FadeLength;
            CurrentShadowStart = t_ShadowStart;
            CurrentShadowFadeLength = t_ShadowFadeLength;
            CurrentShadowStartFoliage = t_ShadowStartFoliage;
            CurrentShadowFadeLengthFoliage = t_ShadowFadeLengthFoliage;

            SqrTerrainCullingDist = Mathf.Sqrt(TerrainSize.x * TerrainSize.x + TerrainSize.z * TerrainSize.z) + CurrentCullDistance;
            SqrTerrainCullingDist *= SqrTerrainCullingDist;

        //  Update Culling Group
            float[] distances = new float[] {CurrentCullDistance * CullingGroupDistanceFactor, CurrentCacheDistance * CullingGroupDistanceFactor };
            cullingGroup.SetBoundingDistances(distances);

        //  Update fade distances in shaders
            GrassFadeProperties = new Vector4(
                CurrentCullDistance * CurrentCullDistance,
                1.0f / (CurrentFadeLength * CurrentFadeLength * ((CurrentCullDistance / CurrentFadeLength) * 2.0f)),
                CurrentDetailFadeStart * CurrentDetailFadeStart,
                1.0f / (CurrentDetailFadeLength * CurrentDetailFadeLength)
            );
            Shader.SetGlobalVector(GrassFadePropsPID, GrassFadeProperties);
            Shader.SetGlobalVector(GrassShadowFadePropsPID, new Vector4(
                CurrentShadowStart * CurrentShadowStart,
                1.0f / (CurrentShadowFadeLength * CurrentShadowFadeLength),
                CurrentShadowStartFoliage * CurrentShadowStartFoliage,
                1.0f / (CurrentShadowFadeLengthFoliage * CurrentShadowFadeLengthFoliage)
            ));

            //  Clear all currently set CellContents
            //  We simply rely on the Update function here, which will generate new CellContents based on the new settings.
            for (int i = 0; i < Cells.Length; i ++) {
                var CurrentCell = Cells[i];
                if (CurrentCell.state == 3) {
                    int NumberOfLayersInCell = CurrentCell.CellContentCount;
                    for (int j = 0; j < NumberOfLayersInCell; j++) {
                        CellContent[ CurrentCell.CellContentIndexes[j] ].ReleaseCellContent();                         
                        #if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                            Destroy(CellContent[ CurrentCell.CellContentIndexes[j] ].v_mat);
                        #endif
                    }
                }
                CurrentCell.state = 0; 
            }

        //  Set up tempMatrixArray
            var maxInstances = Mathf.CeilToInt(NumberOfBucketsPerCell * NumberOfBucketsPerCell * maxBucketDensity * CurrentDetailDensity);
            tempMatrixArray = new Matrix4x4[maxInstances];

        //  Update Compute Buffer
            if (UseCompute) {
                var maxVisibleInstancesRatio = (CurrentCullDistance / CellSize);
                maxVisibleInstancesRatio = maxVisibleInstancesRatio * maxVisibleInstancesRatio;
                
                var finalLayerPointer = 0;
                var bufferMemoryUsage = 0.0f;
                for (int i = 0; i < OrigNumberOfLayers; i++) {
                    if (FinalLayersIndices[i] != -1) {
                        MergedCellcontentsBuffer[finalLayerPointer].Release();
                    //  Scale down buffer if the layer supports clip (currently grass shader only)
                        var clip = v_clip[finalLayerPointer];
                        var clippedInstancesRatio = (DetailFadeStart + DetailFadeLength) / CellSize;
                    //  This is: outer circle - inner circle = outer ring with lowered density due to 2 step culling
                        clippedInstancesRatio = maxVisibleInstancesRatio - clippedInstancesRatio * clippedInstancesRatio;
                        var size = Mathf.CeilToInt(maxVisibleInstancesRatio * LayersMaxDensity[finalLayerPointer] * CurrentDetailDensity)
                                 - Mathf.CeilToInt(clippedInstancesRatio * LayersMaxDensity[finalLayerPointer] * CurrentDetailDensity * clip);
                    //  Make sure size does not get zero.
                        if (size == 0) {
                            size = 1;
                        }
                        MergedCellcontentsBuffer[finalLayerPointer] = new GraphicsBuffer(GraphicsBuffer.Target.Append, size, 64);
                        finalLayerPointer++;
                        bufferMemoryUsage += size * 64.0f;
                    }
                }
                #if UNITY_EDITOR
                    Debug.Log("Buffer Memory Usage: " + Mathf.Round(bufferMemoryUsage / 100000) + "MB / VRAM Size: " + SystemInfo.graphicsMemorySize + "MB / " + (100.0f / SystemInfo.graphicsMemorySize * Mathf.Round(bufferMemoryUsage / 100000)).ToString("0.00") + "%" );
                #endif
            }

            if (useBurst) {
                BurstInit();
            }
        }

//  --------------------------------------------------------------------
//  Init Cells, CellContens and Density Maps from Scriptable Object

        public void InitCellsFast() {
            mapByte = new byte[SavedTerrainData.DensityMaps.Count][];
            for (int i = 0; i < SavedTerrainData.DensityMaps.Count; i ++) {
               mapByte[i] =  SavedTerrainData.DensityMaps[i].mapByte;
            }
            Cells = new GrassCell[ SavedTerrainData.Cells.Length ];
            CellContent = new GrassCellContent[ SavedTerrainData.CellContent.Length ];
            maxBucketDensity = SavedTerrainData.maxBucketDensity;

            int CellContentIndex = 0;

        //  Get TerrainShift as the terrain might not be positioned at the same spot were the data was baked.
            Vector3 TerrainShift = SavedTerrainData.TerrainPosition - TerrainPosition;

        //  We really have to copy the arrays as otherwise material, mesh or argsbuffer error?!
            int cLength = SavedTerrainData.Cells.Length;
            for(int CurrentCellIndex = 0; CurrentCellIndex < cLength; CurrentCellIndex++) {

                Cells[CurrentCellIndex] = new GrassCell();

                Cells[CurrentCellIndex].state = 0;
                Cells[CurrentCellIndex].index = CurrentCellIndex;

            //  Handle shifted terrains
                Cells[CurrentCellIndex].Center = SavedTerrainData.Cells[CurrentCellIndex].Center;
                Cells[CurrentCellIndex].Center.x -= TerrainShift.x;
                Cells[CurrentCellIndex].Center.y -= TerrainShift.y;
                Cells[CurrentCellIndex].Center.z -= TerrainShift.z;

                //Cells[CurrentCellIndex].CellContentIndexes = new List<int>();
                Cells[CurrentCellIndex].CellContentIndexes = SavedTerrainData.Cells[CurrentCellIndex].CellContentIndexes;
                Cells[CurrentCellIndex].CellContentCount = SavedTerrainData.Cells[CurrentCellIndex].CellContentCount;

                int CellContentsinCell = Cells[CurrentCellIndex].CellContentCount;
                for (int layer = 0; layer < CellContentsinCell; layer++) {

                    CellContent[CellContentIndex] = new GrassCellContent();

                    var CurrentCellContent = CellContent[CellContentIndex];
                    var CurrentSavedCellContent = SavedTerrainData.CellContent[CellContentIndex];

                    CurrentCellContent.index = CellContentIndex;
                    CurrentCellContent.Layer = CurrentSavedCellContent.Layer;
                    if(CurrentSavedCellContent.SoftlyMergedLayers.Length > 0) {
                        CurrentCellContent.SoftlyMergedLayers = CurrentSavedCellContent.SoftlyMergedLayers;   
                    }
                    else {
                        CurrentCellContent.SoftlyMergedLayers = null;  
                    }
                    CurrentCellContent.GrassMatrixBufferPID = GrassMatrixBufferPID;
                
                //  Handle shifted terrains
                    var Center = CurrentSavedCellContent.Center;
                    Center.x -= TerrainShift.x;
                    Center.y -= TerrainShift.y; 
                    Center.z -= TerrainShift.z;
                    CurrentCellContent.Center = Center;
                    CurrentCellContent.Pivot = CurrentSavedCellContent.Pivot; // Pivot is in local terrain space

                    CurrentCellContent.PatchOffsetX = CurrentSavedCellContent.PatchOffsetX;
                    CurrentCellContent.PatchOffsetZ = CurrentSavedCellContent.PatchOffsetZ;
                    CurrentCellContent.Instances = CurrentSavedCellContent.Instances;

                    CurrentCellContent.ShadowCastingMode = ShadowCastingMode[CurrentCellContent.Layer];
                    CurrentCellContent.motionVectorGenerationMode = motionVectorGenerationMode[CurrentCellContent.Layer];

                    CellContentIndex += 1;
                }
            }
            LayersMaxDensity = SavedTerrainData.LayersMaxDensity;
        }

//  --------------------------------------------------------------------
//  Init Cells, CellContens and Density Maps directly at start

        public void InitCells() {
        //  Merge Layers
            NumberOfLayers = terData.detailPrototypes.Length;
            OrigNumberOfLayers = NumberOfLayers;
            int[][] MergeArray = new int[OrigNumberOfLayers][]; // Actually too big...
            int[][] SoftMergeArray = new int[OrigNumberOfLayers][]; // Actually too big...

        //  Check if we have to merge detail layers
            for (int i = 0; i < OrigNumberOfLayers; i ++) {

                int t_LayerToMergeWith = LayerToMergeWith[i];
                int index_LayerToMergeWith = t_LayerToMergeWith - 1;

                if( (t_LayerToMergeWith != 0) && (t_LayerToMergeWith != (i+1))  ) {
                //  Check if the Layer we want to merge with does not get merged itself..
                    if ( LayerToMergeWith[ index_LayerToMergeWith  ] == 0 ) {
                        if ( MergeArray[ index_LayerToMergeWith ] == null ) {
                            MergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                        }

                        if ( DoSoftMerge[i] ) {
                            if( SoftMergeArray[ index_LayerToMergeWith ] == null ) {
                                SoftMergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                            }
                        }
                    //  Find a the first free entry Merge
                        for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                            if ( MergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                MergeArray[index_LayerToMergeWith][j] = i + 1; // as index starts at 1 (0 means: do not merge)
                                break;
                            } 
                        }

                    //  Find a the first free entry Soft Merge
                        if ( DoSoftMerge[i] ) {
                            for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                                if ( SoftMergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                    SoftMergeArray[index_LayerToMergeWith][j] = i + 1; // as index starts at 1 (0 means: do not merge)
                                    break;
                                } 
                            }
                        }
                    }
                }
            }


            Cells = new GrassCell[NumberOfCells * NumberOfCells];
        //  As we do not know the final count we create cellcontents in a temporary list
            List<GrassCellContent> tempCellContent = new List<GrassCellContent>();
            tempCellContent.Capacity = NumberOfCells * NumberOfCells * NumberOfLayers;
            mapByte = new byte[NumberOfLayers][];

        //  Read and convert [,] into byte[]
            // Here we are working on "pixels" or buckets and lay out the array like x0.y0, x0.y2, x0.y3, ....
            for(int layer = 0; layer < NumberOfLayers; layer++) {
        //  TODO: remapping of layers did not work out. So we do it brute force...
 
            //  Skip mapByte if layer uses hard merge
                if (LayerToMergeWith[layer] == 0 || DoSoftMerge[layer] ) {
                    mapByte[layer] = new byte[ (int)(TerrainDetailSize.x * TerrainDetailSize.y) ];
                //  Check merge. Only merge densities if SoftMerge is disabled.
                    bool doMerge = false;
                    if ( MergeArray[layer] != null && !DoSoftMerge[layer] ) {
                        doMerge = true;
                    }
                    for (int x = 0; x < (int)TerrainDetailSize.x; x++ ){
                        for (int y = 0; y < (int)TerrainDetailSize.y; y ++) {
                            // flipped!!!!!????????? no,not in this case.
                            int[,] temp = terData.GetDetailLayer(x, y, 1, 1, layer );


// blur
/*
int outerSamples;
int[,] temp1 = terData.GetDetailLayer(x + 1, y + 1, 1, 1, layer );
outerSamples = (int)temp1[0,0];
temp1 = terData.GetDetailLayer(x + 1, y - 1, 1, 1, layer );
outerSamples += (int)temp1[0,0];
temp1 = terData.GetDetailLayer(x - 1, y + 1, 1, 1, layer );
outerSamples += (int)temp1[0,0];
temp1 = terData.GetDetailLayer(x - 1, y - 1, 1, 1, layer );
outerSamples += (int)temp1[0,0];

temp1 = terData.GetDetailLayer(x + 1, y , 1, 1, layer );
outerSamples += (int)temp1[0,0];
temp1 = terData.GetDetailLayer(x - 1, y , 1, 1, layer );
outerSamples += (int)temp1[0,0];
temp1 = terData.GetDetailLayer(x , y + 1, 1, 1, layer );
outerSamples += (int)temp1[0,0];
temp1 = terData.GetDetailLayer(x , y - 1, 1, 1, layer );
outerSamples += (int)temp1[0,0];

outerSamples = Mathf.CeilToInt( (float)(outerSamples) * 0.125f );

temp[0,0] = Mathf.FloorToInt( (float)( (int)temp[0,0] * 3 + outerSamples) * 0.25f);
*/
                            mapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( (int)temp[0,0] );
                        //  Merge
                            if(doMerge) {
                                for (int m = 0; m < OrigNumberOfLayers - 1; m++ ) {
                                //  Check DoSoftMerge as otherwise even soft merged layers would be added
                                    if (MergeArray[layer][m] != 0 && !DoSoftMerge[MergeArray[layer][m] - 1]) {
                                        temp = terData.GetDetailLayer(x, y, 1, 1, MergeArray[layer][m] - 1 ); // as index starts as 1!
                                        mapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( mapByte[layer][ x * (int)TerrainDetailSize.y + y ] + (int)temp[0,0] );  
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int density = 0;
            int CellContentOffset = 0;
            Vector3 Center;
            int ArrayOffset = 0;

            int TerrainDetailSizeX = (int)TerrainDetailSize.x;
            int TerrainDetailSizeY = (int)TerrainDetailSize.y;

            for(int x = 0; x < NumberOfCells; x++) {
                for(int z = 0; z < NumberOfCells; z++) {
                    var CurrentCellIndex = x * NumberOfCells + z;
                //  Find Height or Position.y of Cell
                    Vector2 normalizedPos;
                    
                    //  center
                    normalizedPos.x = (x * CellSize + 0.5f * CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize + 0.5f * CellSize) * OneOverTerrainSize.z;
                    float sampledHeight = terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  lower left
                    normalizedPos.x = (x * CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  upper left
                    normalizedPos.x = (x * CellSize + CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  upper right
                    normalizedPos.x = (x * CellSize + CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize + CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  lower right
                    normalizedPos.x = (x * CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize + CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  average
                    sampledHeight *= 0.2f;

                    Cells[CurrentCellIndex] = new GrassCell();
                    var CurrentCell = Cells[CurrentCellIndex];
                    CurrentCell.state = 0;
                    CurrentCell.index = CurrentCellIndex;
                //  Center in World Space – used by cullingspheres
                    Center.x = x * CellSize + 0.5f * CellSize  +  TerrainPosition.x;
                    Center.y = sampledHeight + TerrainPosition.y;
                    Center.z = z * CellSize + 0.5f * CellSize  +  TerrainPosition.z;
                    CurrentCell.Center = Center;
                    //CurrentCell.CellContentIndexes = new List<int>();
                    CurrentCell.CellContentIndexes = new int[NumberOfLayers];
                    CurrentCell.CellContentCount = 0;

                    int tempBucketDensity;
                    int layerPointer = 0;

                //  Create and setup all CellContens (layers) for the given Cell
                    for(int layer = 0; layer < NumberOfLayers; layer++) {
                    
                    //  Only create cellcontent entry if the layer does not get merged.
                        if (LayerToMergeWith[layer] == 0 ) {
                        //  Sum up density for all buckets 
                            for(int xp = 0; xp < NumberOfBucketsPerCell; xp++) {
                                for(int yp = 0; yp < NumberOfBucketsPerCell; yp++) {
                                    //  here we are working on cells and buckets!
                                    tempBucketDensity = (int)(mapByte[layer][
                                       (x * NumberOfBucketsPerCell) * TerrainDetailSizeY + xp * TerrainDetailSizeY 
                                       + z * NumberOfBucketsPerCell + yp 
                                    ]);
                                    if (tempBucketDensity > maxBucketDensity) {
                                        maxBucketDensity = tempBucketDensity;
                                    }
                                    density += tempBucketDensity;
                                }
                            }

                        //  Check for softly merged layers
                            int NumberOfSoftlyMergedLayers = 0;

                            if(SoftMergeArray[layer] != null) {
                                for (int l = 0; l < OrigNumberOfLayers - 1; l++) {
                                    if ( SoftMergeArray[layer][l] == 0) {
                                        break;
                                    }
                                    int softMergedLayer = SoftMergeArray[layer][l] - 1; // as index starts at 1
                                    int softDensity = 0;

                                    //  Sum up density for all buckets 
                                    for(int xp = 0; xp < NumberOfBucketsPerCell; xp++) {
                                        for(int yp = 0; yp < NumberOfBucketsPerCell; yp++) {
                                            //  here we are working on cells and buckets!
                                            tempBucketDensity = (int)(mapByte[softMergedLayer][
                                               (x * NumberOfBucketsPerCell) * TerrainDetailSizeY + xp * TerrainDetailSizeY
                                               + z * NumberOfBucketsPerCell + yp 
                                            ]);
                                            softDensity += tempBucketDensity;
                                        }
                                    }
                                    if (softDensity > 0) {
                                        NumberOfSoftlyMergedLayers += 1;
                                        density += softDensity;
                                    }
                                }
                                if (NumberOfSoftlyMergedLayers * 16 > maxBucketDensity) {
                                // TODO: double check if i can skip safeguard                       
                                    maxBucketDensity = NumberOfSoftlyMergedLayers * 16 + 16; // + 16 * 2; // * 2 is safe guard!
                                }
                            }

                        //  Compute: Store max density per layer which will determine the size of the append buffers
                            if (UseCompute) {
                                if(density > LayersMaxDensity[layerPointer]) {
                                    LayersMaxDensity[layerPointer] = density;
                                }
                            }
                            layerPointer++;

                        //  Skip Content if density = 0
                            if (density > 0) {
                            //  Register CellContent to Cell
                                //CurrentCell.CellContentIndexes.Add(CellContentOffset);
                                CurrentCell.CellContentIndexes[ CurrentCell.CellContentCount ] = CellContentOffset;
                                CurrentCell.CellContentCount += 1;
                            //  Add new CellContent
                                var tempContent = new GrassCellContent();
                                tempContent.index = CellContentOffset;
                                tempContent.Layer = layer;
                                tempContent.GrassMatrixBufferPID = GrassMatrixBufferPID;
                                //tempContent.GrassNormalBufferPID = GrassNormalBufferPID;
                            //  Center in World Space – used by drawmeshindirect
                                tempContent.Center = Center;
                            //  Pivot of cell in local terrain space
                                tempContent.Pivot = new Vector3( x * CellSize, sampledHeight, z * CellSize );
                                tempContent.PatchOffsetX = x * NumberOfBucketsPerCell * TerrainDetailSizeY;
                                tempContent.PatchOffsetZ = z * NumberOfBucketsPerCell;
                                tempContent.Instances = density;

                                tempContent.ShadowCastingMode = ShadowCastingMode[layer];
                                tempContent.motionVectorGenerationMode = motionVectorGenerationMode[layer];

                            //  Softly merged Layers
                                if( NumberOfSoftlyMergedLayers > 0 ) {
                                    List<int> tempSoffMergedLayers = new List<int>();
                                    for (int l = 0; l < OrigNumberOfLayers - 1; l++) {
                                        if(SoftMergeArray[layer][l] != 0) {
                                            tempSoffMergedLayers.Add( SoftMergeArray[layer][l] - 1  );
                                        }
                                    }
                                    tempContent.SoftlyMergedLayers = tempSoffMergedLayers.ToArray();
                                }

                            //  Add content to temp content list
                                tempCellContent.Add(tempContent);
                                CellContentOffset += 1;
                            }
                            density = 0;
                        }
                    }
                }
                ArrayOffset += TerrainDetailSizeX;                
            }
            CellContent = tempCellContent.ToArray();
            tempCellContent.Clear();
        }


//  --------------------------------------------------------------------
//  Init Grid, Culling groups and all the rest

        public void Init() {

        //  URP specific settings - as used by the compute shader

        //  VR specific settings
#if UNITY_2018_3_OR_NEWER
            //MockHMD might not be loaded...
            //var MockHMDSinglePassInstanced = false;
            //var buildSettings = Unity.XR.MockHMD.MockHMDBuildSettings.Instance;
            //if (buildSettings != null) {
            //    MockHMDSinglePassInstanced = buildSettings.renderMode != Unity.XR.MockHMD.MockHMDBuildSettings.RenderMode.MultiPass;
            //}
            if (UnityEngine.XR.XRSettings.stereoRenderingMode == UnityEngine.XR.XRSettings.StereoRenderingMode.SinglePassInstanced ||
                vrForceSinglePassInstanced
            ) {
                usingSinglePassInstanced = true;
            //  No compute? - no, now we can use compute :)
                //rtUseCompute = false;
            }
            if (UnityEngine.XR.XRSettings.enabled || usingSRP) {
            //  But we disable HiZ
                useHizCulling = false;  
            }
#endif

        //  Just for the case that there is no wind script.
            Shader.SetGlobalFloat("_AtgWindGust", 0);    
            Shader.SetGlobalVector("_AtgWindDirSize", new Vector4(1,0,0,0));
            Shader.SetGlobalVector("_AtgWindStrengthMultipliers", new Vector4(0,0,0,0) );

        //  Copy bool from variable exposed in inspector
            UseCompute = rtUseCompute;
        //  Disable Compute in editor
            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    UseCompute = false;
                }
            #endif

        //  Compute: Check if compute actually is supported.
            if(UseCompute) {
               UseCompute = SystemInfo.supportsComputeShaders;
            }

            ter = GetComponent<Terrain>();
            terData = ter.terrainData;

        //  Hide grass from the terrain
            ter.detailObjectDistance = 0;

        //  Copy inputs to actually used variables 
            CurrentDetailDensity = DetailDensity;
            CurrentCullDistance = CullDistance;
            CurrentFadeLength = FadeLength;
            CurrentCacheDistance = CacheDistance;
            CurrentDetailFadeStart = DetailFadeStart;
            CurrentDetailFadeLength = DetailFadeLength;
            CurrentShadowStart = ShadowStart;
            CurrentShadowFadeLength = ShadowFadeLength;
            CurrentShadowStartFoliage = ShadowStartFoliage;
            CurrentShadowFadeLengthFoliage = ShadowFadeLengthFoliage;

            CurrentVertexFadeStart = VertexFadeStart;
            CurrentVertexFadeLength = VertexFadeLength;

            TerrainPosition = ter.GetPosition();
            TerrainSize = terData.size;
            OneOverTerrainSize.x = 1.0f/TerrainSize.x;
            OneOverTerrainSize.y = 1.0f/TerrainSize.y;
            OneOverTerrainSize.z = 1.0f/TerrainSize.z;
            TerrainDetailSize.x = terData.detailWidth;
            TerrainDetailSize.y = terData.detailHeight;

        //  We assume squared Terrains here...
            BucketSize = TerrainSize.x / TerrainDetailSize.x;
            // OneOverBucketSize = 1.0f / BucketSize;
            // Number of buckets or detail map pixels per cell per axis. Must be 2^x! as it has to fit the detail resolution.
            NumberOfBucketsPerCell = (int)NumberOfBucketsPerCellEnum;
            CellSize = NumberOfBucketsPerCell * BucketSize;
            NumberOfCells = (int) (TerrainSize.x / CellSize);
            TotalCellCount = NumberOfCells * NumberOfCells;

            //sh = Shader.Find("AdvancedTerrainGrass/Grass Base Shader");
            
        //  Update fade distances in shaders
            GrassFadeProperties = new Vector4(
                CurrentCullDistance * CurrentCullDistance,
                1.0f / (CurrentFadeLength * CurrentFadeLength * ((CurrentCullDistance / CurrentFadeLength) * 2.0f)),
                CurrentDetailFadeStart * CurrentDetailFadeStart,
                1.0f / (CurrentDetailFadeLength * CurrentDetailFadeLength)
            );
            Shader.SetGlobalVector(GrassFadePropsPID, GrassFadeProperties);
            Shader.SetGlobalVector(GrassShadowFadePropsPID, new Vector4(
                CurrentShadowStart * CurrentShadowStart,
                1.0f / (CurrentShadowFadeLength * CurrentShadowFadeLength),
                CurrentShadowStartFoliage * CurrentShadowStartFoliage,
                1.0f / (CurrentShadowFadeLengthFoliage * CurrentShadowFadeLengthFoliage)
            ));

            var VertexFadeProperties = new Vector2(
                CurrentVertexFadeStart * CurrentVertexFadeStart,
                1.0f / (CurrentVertexFadeLength * CurrentVertexFadeLength)
            );
            Shader.SetGlobalVector(VertexFadePropsPID, VertexFadeProperties);

        //  Go through all layers and determine the number of actually drawn layers - needed by Compute
            OrigNumberOfLayers = terData.detailPrototypes.Length;
            NumberOfFinalLayers = 0;
            FinalLayersIndices = new int[OrigNumberOfLayers];
            
        //  Due to ExecuteInEditMode LayerToMergeWith might not be setup by the editor. So we will simply create and fill it
            bool updateLayerToMergeWith = false;
            if(LayerToMergeWith == null) {
                updateLayerToMergeWith = true;
            }
            else if (LayerToMergeWith.Length != OrigNumberOfLayers) {
                updateLayerToMergeWith = true;
            }
            if(updateLayerToMergeWith) {
                LayerToMergeWith = new int[OrigNumberOfLayers];
                for (int i = 0; i < OrigNumberOfLayers; i++) {
                    LayerToMergeWith[i] = 0;
                }
            }

            for (int i = 0; i < OrigNumberOfLayers; i++) {
                if (LayerToMergeWith[i] == 0) { // && LayerToMergeWith[LayerToMergeWith[i]] == 0)
                    FinalLayersIndices[i] = NumberOfFinalLayers;
                    NumberOfFinalLayers++;
                }
                else {
                    FinalLayersIndices[i] = -1;
                }
            }
            LayersMaxDensity = new int[NumberOfFinalLayers];
            for (int i = 0; i < NumberOfFinalLayers; i++) {
                LayersMaxDensity[i] = 0;
            }

        //  We have to fill some per layer attributes as the editor scrit might not have run
            if (ShadowCastingMode == null)
            {
                ShadowCastingMode = new ShadowCastingMode[OrigNumberOfLayers];
                for (int i = 0; i < OrigNumberOfLayers; i++) {
                    ShadowCastingMode[i] = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
            if (motionVectorGenerationMode == null)
            {
                motionVectorGenerationMode = new MotionVectorGenerationMode[OrigNumberOfLayers];
                for (int i = 0; i < OrigNumberOfLayers; i++) {
                    motionVectorGenerationMode[i] = MotionVectorGenerationMode.Camera;
                }
            }

        //  Init cells
            if(SavedTerrainData != null) {
                InitCellsFast(); 
            //  Just to make sure
                TotalCellCount = Cells.Length; 
            }
            else {
                InitCells();  
            }

        //  Get Heights
            TerrainHeightmapWidth = terData.heightmapResolution;
            TerrainHeightmapHeight = terData.heightmapResolution;
            TerrainHeights = new float[TerrainHeightmapWidth * TerrainHeightmapHeight];
            for(int x = 0; x < TerrainHeightmapWidth; x++) {
                for(int z = 0; z < TerrainHeightmapHeight; z++) {
                    TerrainHeights[ x * TerrainHeightmapWidth + z ] = terData.GetHeight(x, z);
                }
            }
            OneOverHeightmapWidth = 1.0f / TerrainHeightmapWidth;
            OneOverHeightmapHeight = 1.0f / TerrainHeightmapHeight;
            TerrainSizeOverHeightmap = TerrainSize.x / TerrainHeightmapWidth;
        //  Needed guards for normal sampling
            OneOverHeightmapWidthRight = TerrainSize.x - 2 * (TerrainSize.x / (TerrainHeightmapWidth - 1))     - 1;    
            OneOverHeightmapHeightUp   = TerrainSize.z - 2 * (TerrainSize.z / (TerrainHeightmapHeight - 1))    - 1;

        //  Set up CullingGroup
            cullingGroup = new CullingGroup();
            boundingSpheres = new BoundingSphere[TotalCellCount];
            resultIndices = new int[TotalCellCount];
            isVisibleBoundingSpheres = new bool[TotalCellCount];

            // var side = (CellSize * 0.5f) * (CellSize * 0.5f);
            // The above code should be correct – but cells get rejected too early if the camera looks from steep angles?!
            var side = (CellSize * 0.75f) * (CellSize * 0.75f) ;
            var CullingRadius = Mathf.Sqrt( side * 2.0f );

            for (int i = 0; i < TotalCellCount; i++) {
                boundingSpheres[i] = new BoundingSphere( Cells[i].Center, CullingRadius);
                isVisibleBoundingSpheres[i] = false;
            }
            cullingGroup.SetBoundingSpheres(boundingSpheres);
            cullingGroup.SetBoundingSphereCount(TotalCellCount);
            float[] distances = new float[] {CurrentCullDistance * CullingGroupDistanceFactor, CurrentCacheDistance * CullingGroupDistanceFactor };
        
        //  Floating terrains: Store orig boundingspheres
            if (EnableTerrainShift) {
                origBoundingSpheres = new BoundingSphere[boundingSpheres.Length];
                System.Array.Copy(boundingSpheres, origBoundingSpheres, boundingSpheres.Length);
            }
            
            cullingGroup.SetBoundingDistances(distances);
            cullingGroup.onStateChanged = StateChangedMethod;

            SqrTerrainCullingDist = Mathf.Sqrt(TerrainSize.x * TerrainSize.x + TerrainSize.z * TerrainSize.z) + CurrentCullDistance;
            SqrTerrainCullingDist *= SqrTerrainCullingDist;

            #if UNITY_EDITOR
                Debug.Log("Max Bucket Density: " + maxBucketDensity);
            #endif
        
            //  Set up tempMatrixArray
            var maxInstances = Mathf.CeilToInt(NumberOfBucketsPerCell * NumberOfBucketsPerCell * maxBucketDensity * CurrentDetailDensity);
            tempMatrixArray = new Matrix4x4[maxInstances];

            #if UNITY_EDITOR
                Debug.Log("Max Instances per Layer per Cell: " + maxInstances);
            #endif

        //  Set up Compute
            if (UseCompute) {
                v_clip = new float[OrigNumberOfLayers];
                v_boundsCenter = new Vector3[OrigNumberOfLayers];
                v_boundsExtents = new Vector3[OrigNumberOfLayers];
                //v_bounds = new Vector4[OrigNumberOfLayers];

                for ( int i = 0; i < OrigNumberOfLayers; i++) {
                    if (v_mat[i].HasProperty("_Clip")) {
                        v_clip[i] = v_mat[i].GetFloat("_Clip");
                    }
                    else {
                        v_clip[i] = 0.0f;
                    }

                //  Set bounding sphere
                /*  var meshBounds = v_mesh[i].bounds;
                    var t_bounds = Zero4Vec;
                    t_bounds.x = meshBounds.center.x;
                    t_bounds.y = meshBounds.center.y;
                    t_bounds.z = meshBounds.center.z;
                    var t_extents = meshBounds.extents;
                    var radius = Mathf.Max(t_extents.x,  t_extents.z);
                    radius = radius * radius + t_extents.y * t_extents.y;
                    radius = Mathf.Sqrt(radius);
                    t_bounds.w = radius;
                    v_bounds[i] = t_bounds; */
                
                //  Set bounding boxes
                    v_boundsCenter[i] = v_mesh[i].bounds.center;
                    v_boundsExtents[i] = v_mesh[i].bounds.extents;
                }

                ComputeCellcontentsBufferShader = (ComputeShader)Resources.Load("Compute/ComputeGrassCells");
                
                if(!useHizCulling) {
                    ComputeCellcontentsKernel = ComputeCellcontentsBufferShader.FindKernel("ComputeGrassCellsURP");
                }
                else {
                    ComputeCellcontentsKernel = ComputeCellcontentsBufferShader.FindKernel("ComputeGrassCells");    
                }
                
                MergedCellcontentsBufferID = Shader.PropertyToID("MergedCellcontentsBuffer");
                SourceCellcontentsBufferID = Shader.PropertyToID("CellcontentsSourceBuffer");

                AtgVPPID = Shader.PropertyToID("_AtgVP");
                //AtgVPID = Shader.PropertyToID("_AtgV");
                AtgCameraPositionPID = Shader.PropertyToID("_AtgCameraPosition");
                AtgCameraForwardPID = Shader.PropertyToID("_AtgCameraForward");
                AtgInstanceCountPID = Shader.PropertyToID("_AtgInstanceCount");
                AtgCullDistSqrPID = Shader.PropertyToID("_AtgCullDistSqr");

                AtgHiZBufferPID = Shader.PropertyToID("AtgHiZBuffer");
                AtgHiZBufferSizePID = Shader.PropertyToID("AtgHiZBufferSize");

                AtgTerrainShiftComputePID = Shader.PropertyToID("AtgTerrainShiftCompute");
                AtgGrassFadePropsPID = Shader.PropertyToID("AtgGrassFadeProps");
                AtgClipValPID = Shader.PropertyToID("AtgClipVal");
                //AtgBoundingSpherePID = Shader.PropertyToID("AtgBoundingSphere");

                AtgBoundsCenterPID = Shader.PropertyToID("AtgBoundsCenter");
                AtgBoundsExtentsPID = Shader.PropertyToID("AtgBoundsExtents");

                MergedCellcontentsBuffer = new GraphicsBuffer[NumberOfFinalLayers];

                //argsBuffer = new ComputeBuffer[NumberOfFinalLayers];
                MergedMatrixBlock = new MaterialPropertyBlock[NumberOfFinalLayers];
                var maxVisibleInstancesRatio = (CurrentCullDistance / (CellSize));
                maxVisibleInstancesRatio = maxVisibleInstancesRatio * maxVisibleInstancesRatio;

            //  RenderMesh Stuff
                layerData = new LayerData[NumberOfFinalLayers];

                var finalLayerPointer = 0;
                var bufferMemoryUsage = 0.0f;
                for (int i = 0; i < OrigNumberOfLayers; i++) {
                    if (FinalLayersIndices[i] != -1) {

                        //var currentLayerData = layerData[finalLayerPointer]; // I can't use this here?!
                        layerData[finalLayerPointer].rp = new RenderParams(v_mat[finalLayerPointer]);

                        //will be set on draw!
                        //layerData[finalLayerPointer].rp.camera = CameraInWichGrassWillBeDrawn;
                        //layerData[finalLayerPointer].rp.layer = CameraLayer;

                        layerData[finalLayerPointer].rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one);
                        layerData[finalLayerPointer].rp.shadowCastingMode = ShadowCastingMode[finalLayerPointer];
                        layerData[finalLayerPointer].rp.motionVectorMode = motionVectorGenerationMode[finalLayerPointer];
                        layerData[finalLayerPointer].rp.matProps = new MaterialPropertyBlock();

                        layerData[finalLayerPointer].commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
                        layerData[finalLayerPointer].commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
                        layerData[finalLayerPointer].commandData[0].indexCountPerInstance = v_mesh[finalLayerPointer].GetIndexCount(0);

                    //  Scale down buffer if the layer supports clip (currently grass shader only)
                        var clip = v_clip[finalLayerPointer];
                        var clippedInstancesRatio = (DetailFadeStart + DetailFadeLength) / CellSize;
                    //  This is: outer circle - inner circle = outer ring with lowered density due to 2 step culling
                        clippedInstancesRatio = maxVisibleInstancesRatio - clippedInstancesRatio * clippedInstancesRatio;
                    //  Calculate size.
                        var size = Mathf.CeilToInt(maxVisibleInstancesRatio * LayersMaxDensity[finalLayerPointer] * CurrentDetailDensity )
                                 - Mathf.CeilToInt(clippedInstancesRatio * LayersMaxDensity[finalLayerPointer] * CurrentDetailDensity * clip);
                    //  Make sure size does not get zero.
                        if (size == 0) {
                            size = 1;
                        }
                        MergedCellcontentsBuffer[finalLayerPointer] = new GraphicsBuffer(GraphicsBuffer.Target.Append, size, 64);
                        MergedMatrixBlock[finalLayerPointer] = new MaterialPropertyBlock();

                        bufferMemoryUsage += size * 64.0f;

                        finalLayerPointer++;
                    }
                }
                #if UNITY_EDITOR
                    Debug.Log("Buffer Memory Usage: " + Mathf.Round(bufferMemoryUsage / 100000) + "MB / VRAM Size: " + SystemInfo.graphicsMemorySize + "MB / " + (100.0f / SystemInfo.graphicsMemorySize * Mathf.Round(bufferMemoryUsage / 100000)).ToString("0.00") + "%" );
                #endif

                DoubleCopyCountShader = (ComputeShader)Resources.Load("Compute/DoubleCopyCount");
                DoubleCopyCountKernel = DoubleCopyCountShader.FindKernel("DoubleCopyCount");
                ArgsBufferPID = Shader.PropertyToID("ArgsBuffer");

            }

            AtgSurfaceCameraPositionPID = Shader.PropertyToID("_AtgSurfaceCameraPosition");
            AtgTerrainShiftSurfacePID = Shader.PropertyToID("_AtgTerrainShiftSurface");

        //  Burst init – if checked
            if (useBurst){
                BurstInit();
            }

        //  Lighting init LPPV
            if(lppv != null && !usingSRP ) {
                useLppv = true;
                lpUsage = LightProbeUsage.UseProxyVolume;
            }

        //  Start Worker Thread
            WorkerThread = new Thread(InitCellContentOnThread);
            WorkerThread.Start();
        }


//  --------------------------------------------------------------------
//  Function to immediately init all cells within the user defined BurstRadius

        public void BurstInit() {
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if(DrawInEditMode)
                {    
                    Cam = SceneView.lastActiveSceneView.camera;
                    if (Cam == null) {
                        return;
                    }
                }
                else  
                {
                    Cam = Camera.main;
                    if (Cam == null) {
                        return;
                    }    
                }
            }
            else  
            {
            //  Get the current active camera
                if (Cam == null) {
                    Cam = Camera.main;
                    if (Cam == null) {
                        return;
                    }
                }    
            }
#else  
        //  Get the current active camera
            if (Cam == null) {
                Cam = Camera.main;
                if (Cam == null) {
                    return;
                }
            }
#endif

            CamTransform = Cam.transform;
            var camPosition = CamTransform.position;

            if ( (TerrainPosition - camPosition).sqrMagnitude > SqrTerrainCullingDist ) {
                return;
            }

        //  Loop over all Cells
            int t_NumberOfCells = Cells.Length;
            for (int i = 0; i < t_NumberOfCells; i ++) {
                var CurrentCell = Cells[i];
                float distance = Vector3.Distance(camPosition, CurrentCell.Center);
                int Layer;

            //  Find Cells in BurstRadius – Please note: Culling groups are not available here (first frame!). So we do it manually.
                if (distance < BurstRadius) {

                    var NumberOfLayersInCell = CurrentCell.CellContentCount;
                
                    for (int j = 0; j < NumberOfLayersInCell; j ++) {
                        var CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                        Layer = CurrentCellContent.Layer;
                        #if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                            CurrentCellContent.v_mat = new Material(v_mat[Layer]);
                        #else
                            CurrentCellContent.v_mat = v_mat[Layer];
                        #endif
                        CurrentCellContent.v_mesh = v_mesh[Layer];
                        CurrentCellContent.ShadowCastingMode = ShadowCastingMode[Layer];
                        CurrentCellContent.motionVectorGenerationMode = motionVectorGenerationMode[Layer];
                    }
                    InitCellContent(CurrentCell.index);
                    ThreadIsRunning = false;
                    for (int j = 0; j < NumberOfLayersInCell; j ++) {
                        var CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                    //  Added support for CutScenes: CellContent might already be initialized.
                        if (CurrentCellContent.state == 0)
                            CurrentCellContent.InitCellContent_Delegated(UseCompute, usingSinglePassInstanced);
                    }   
                    
                    CurrentCell.state = 3;
                }
            }
        }

//  --------------------------------------------------------------------
//  Culling group callback methods
    
        private void StateChangedMethod(CullingGroupEvent evt) {
            if (evt.isVisible) { //} && evt.currentDistance == 0) {
                isVisibleBoundingSpheres[evt.index] = true;
            }
            else {
                isVisibleBoundingSpheres[evt.index] = false;
            }
            UnityEngine.Profiling.Profiler.BeginSample("Release Cell");
        //  Method to release cells
            if(evt.currentDistance == 2) { // && evt.previousDistance == 1) { // this && made some cells never to be released
                var CurrentCell = Cells[evt.index];
            //  IMPORTANT: Do not remove cells the worker thread is processing! They will be wiped as soon as they are finished.             
                if(CurrentCell.state != 2) { 
                    int NumberOfLayersInCell = CurrentCell.CellContentCount;
                    for (int j = 0; j < NumberOfLayersInCell; j++) {
                        CellContent[ CurrentCell.CellContentIndexes[j] ].ReleaseCellContent(); // ReleaseCompleteCellContent();
                        #if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                            Destroy(CellContent[ CurrentCell.CellContentIndexes[j] ].v_mat);
                        #endif
                    }
                //  What happens to already queued cells here? Explicitely remove queued cells.
                    UnityEngine.Profiling.Profiler.BeginSample("Release queued Cell");
                        if(CurrentCell.state == 1) {
                            CellsOrCellContentsToInit.Remove( CurrentCell.index );
                        }
                    UnityEngine.Profiling.Profiler.EndSample();
                    CurrentCell.state = 0;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

//  --------------------------------------------------------------------
//  The real update function which culls and queues grass cells and draws the finished ones

        void DrawGrass() {

            if (vrForceSinglePassInstanced) {
                usingSinglePassInstanced = true;
            }

#if UNITY_EDITOR
    if (!Application.isPlaying) 
    {
        if (!DrawInEditMode) 
        {
            ter.detailObjectDistance = 100.0f;
            return;
        }
        else  
        {
            ter.detailObjectDistance = 0.0f;
        }
    }
#endif

            if (Cam == null)
            {
                Cam = Camera.main;
                if (Cam == null)
                {
                    return;
                }
            } 

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (DrawInEditMode)
                {    
                    Cam = SceneView.lastActiveSceneView.camera;
                }
            }
#endif

            CamTransform = Cam.transform;
            var CamPosition = CamTransform.position;

        //  Update Terrain Position
            var CurrentTerrainPos = ter.GetPosition();
          
        //  Check if the terrain is within the culling distance. Otherwise return.
            if ( (CurrentTerrainPos - CamPosition).sqrMagnitude > SqrTerrainCullingDist )
            {
                return;
            }

        //  Handle Terrain Shift
            TerrainShift = TerrainPosition - CurrentTerrainPos;
            Shader.SetGlobalVector(AtgTerrainShiftSurfacePID, new Vector4(TerrainShift.x, TerrainShift.y, TerrainShift.z, (UseCompute) ? 0.0f : 1.0f) );

            // This does not fix culling groups:
            // cullingGroup.SetDistanceReferencePoint(CamTransform.position + TerrainShift);
            // Use a helper camera instead? No, unfortunately culling groups do not support disabled cameras :(
            // So we actually have to move the Boundingspheres.
            if (EnableTerrainShift)
            {
                UnityEngine.Profiling.Profiler.BeginSample("Move CullingSpheres");
                for (int i = 0; i < boundingSpheres.Length; i++) {
                    boundingSpheres[i].position.x = origBoundingSpheres[i].position.x - TerrainShift.x;
                    boundingSpheres[i].position.y = origBoundingSpheres[i].position.y - TerrainShift.y;
                    boundingSpheres[i].position.z = origBoundingSpheres[i].position.z - TerrainShift.z;
                }
                UnityEngine.Profiling.Profiler.EndSample();
            }

        //  Set target cameras
            if ((int)CameraSelection == 0)
            {
                CameraInWichGrassWillBeDrawn = null;
            }
            else
            {
                CameraInWichGrassWillBeDrawn = Cam;
            }

        //  In order to sync compute and regular (surface) shaders we have to pass a "custom" camera position to the surface shaders - which might be a frame delayed.
        //  TODO: How does this work with multiple terrains?
            Shader.SetGlobalVector(AtgSurfaceCameraPositionPID, CamPosition);

        //  HiZ Buffer - currently not supported in URP and VR
            if (UseCompute && !usingSRP && useHizCulling) {
                if(HiZBuffer == null) {
                    var hiz = Cam.GetComponent<AtgHiZBuffer>();
                    if(hiz == null) {
                        HiZBuffer = Cam.gameObject.AddComponent<AtgHiZBuffer>();
                    }
                    else {
                        HiZBuffer = hiz;
                    }
                }
            }

        //  Get all visible Cells
            cullingGroup.targetCamera = Cam;
            cullingGroup.SetDistanceReferencePoint(CamPosition);
            if(IngnoreOcclusion) {
                numResults = cullingGroup.QueryIndices(0, resultIndices, 0);   
            }
            else {
                numResults = cullingGroup.QueryIndices(true, 0, resultIndices, 0);
            }

        //  CullingGroup most likely did not return a valid result... (which happens in the first frame)
        //  Added safe guard "firstFrameSkipped": Now small terrains should be rendered properly even if all cells are visible.
            if(numResults == TotalCellCount && !firstFrameSkipped) {
                firstFrameSkipped = true;
                return;
            }

            GrassCell CurrentCell;
            int CellState;
            GrassCellContent CurrentCellContent;
            int NumberOfLayersInCell;

            numOfVisibleCells = 0;

        //  Compute: As the HiZBuffer may not be available.
            if (UseCompute && useHizCulling) {
                if (HiZBuffer.HiZavailable == false)
                    return;
            }

        //  Compute
            if (UseCompute) {

            //  Reset final buffers
                for (int i = 0; i < NumberOfFinalLayers; i++) {
                    MergedCellcontentsBuffer[i].SetCounterValue(0);
                }
            //  Set ViewProjection matrix float array
                VP_Matrix = Cam.projectionMatrix * Cam.worldToCameraMatrix;
                VP_MatrixAsFloats[0] = VP_Matrix[0, 0];
                VP_MatrixAsFloats[1] = VP_Matrix[1, 0];
                VP_MatrixAsFloats[2] = VP_Matrix[2, 0];
                VP_MatrixAsFloats[3] = VP_Matrix[3, 0];
                VP_MatrixAsFloats[4] = VP_Matrix[0, 1];
                VP_MatrixAsFloats[5] = VP_Matrix[1, 1];
                VP_MatrixAsFloats[6] = VP_Matrix[2, 1];
                VP_MatrixAsFloats[7] = VP_Matrix[3, 1];
                VP_MatrixAsFloats[8] = VP_Matrix[0, 2];
                VP_MatrixAsFloats[9] = VP_Matrix[1, 2];
                VP_MatrixAsFloats[10] = VP_Matrix[2, 2];
                VP_MatrixAsFloats[11] = VP_Matrix[3, 2];
                VP_MatrixAsFloats[12] = VP_Matrix[0, 3];
                VP_MatrixAsFloats[13] = VP_Matrix[1, 3];
                VP_MatrixAsFloats[14] = VP_Matrix[2, 3];
                VP_MatrixAsFloats[15] = VP_Matrix[3, 3];

            //  Set V matrix float array
                /*V_Matrix = Cam.worldToCameraMatrix;
                V_MatrixAsFloats[0] = V_Matrix[0, 0];
                V_MatrixAsFloats[1] = V_Matrix[1, 0];
                V_MatrixAsFloats[2] = V_Matrix[2, 0];
                V_MatrixAsFloats[3] = V_Matrix[3, 0];
                V_MatrixAsFloats[4] = V_Matrix[0, 1];
                V_MatrixAsFloats[5] = V_Matrix[1, 1];
                V_MatrixAsFloats[6] = V_Matrix[2, 1];
                V_MatrixAsFloats[7] = V_Matrix[3, 1];
                V_MatrixAsFloats[8] = V_Matrix[0, 2];
                V_MatrixAsFloats[9] = V_Matrix[1, 2];
                V_MatrixAsFloats[10] = V_Matrix[2, 2];
                V_MatrixAsFloats[11] = V_Matrix[3, 2];
                V_MatrixAsFloats[12] = V_Matrix[0, 3];
                V_MatrixAsFloats[13] = V_Matrix[1, 3];
                V_MatrixAsFloats[14] = V_Matrix[2, 3];
                V_MatrixAsFloats[15] = V_Matrix[3, 3];*/

                ComputeCellcontentsBufferShader.SetFloats(AtgVPPID, VP_MatrixAsFloats);
                //ComputeCellcontentsBufferShader.SetFloats(AtgVPID, V_MatrixAsFloats);
                ComputeCellcontentsBufferShader.SetVector(AtgCameraPositionPID, CamPosition);
                ComputeCellcontentsBufferShader.SetVector(AtgCameraForwardPID, CamTransform.forward * GrassInstanceSize);
                ComputeCellcontentsBufferShader.SetFloat(AtgCullDistSqrPID, CurrentCullDistance * CurrentCullDistance);
                ComputeCellcontentsBufferShader.SetVector(AtgGrassFadePropsPID, GrassFadeProperties);
            //  As when switching between cameras HiZBuffer.texture might not be available.
                if(useHizCulling) {
                    if (HiZBuffer.texture != null) {
                        ComputeCellcontentsBufferShader.SetTexture(0, AtgHiZBufferPID, HiZBuffer.texture);
                        ComputeCellcontentsBufferShader.SetVector(AtgHiZBufferSizePID, new Vector2(HiZBuffer.texture.width, HiZBuffer.texture.height));
                    }
                }
                ComputeCellcontentsBufferShader.SetVector(AtgTerrainShiftComputePID, TerrainShift);
            }

        //  Loop over visible Cells.
            for (int i = 0; i < numResults; i ++) {
                if(IngnoreOcclusion) {
                    if (isVisibleBoundingSpheres[ resultIndices[i] ] == false)
                        continue;
                }
                numOfVisibleCells++;

                CurrentCell = Cells[ resultIndices[i] ];
                CellState = CurrentCell.state;
                NumberOfLayersInCell = CurrentCell.CellContentCount;

                switch(CellState) {
                //  Draw
                    case 3 :
                        UnityEngine.Profiling.Profiler.BeginSample("Draw Grass");
                        for (int j = 0; j < NumberOfLayersInCell; j++) {
                            CurrentCellContent = CellContent[CurrentCell.CellContentIndexes[j]];
                            if (!UseCompute) {
                                if (CurrentCellContent.state == 3) {
                                    if(useLppv) {
                                        CellContent[ CurrentCell.CellContentIndexes[j] ].DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer, TerrainShift, lppv, true, renderingLayerMask);
                                    }
                                    else {                
                                        CellContent[ CurrentCell.CellContentIndexes[j] ].DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer, TerrainShift, null, false, renderingLayerMask);  
                                    }
                                }
                            }
                            else {
                                if (CurrentCellContent.state == 3)
                                {
                                    var baseLayer = CurrentCellContent.Layer;
                                    var layer = FinalLayersIndices[baseLayer];
                                //  Set source Buffer
                                    ComputeCellcontentsBufferShader.SetBuffer(ComputeCellcontentsKernel, SourceCellcontentsBufferID, CurrentCellContent.matrixBuffer);
                                //  Set target Buffer
                                    ComputeCellcontentsBufferShader.SetBuffer(ComputeCellcontentsKernel, MergedCellcontentsBufferID, MergedCellcontentsBuffer[layer]);
                                    var threadGroups = Mathf.CeilToInt((float)CurrentCellContent.Instances / 64.0f); // 1024.0f);
                                //  Set all other per cell content variables
                                    ComputeCellcontentsBufferShader.SetInt(AtgInstanceCountPID, CurrentCellContent.Instances);
                                    ComputeCellcontentsBufferShader.SetFloat(AtgClipValPID, v_clip[baseLayer]);
                                    //ComputeCellcontentsBufferShader.SetVector(AtgBoundingSpherePID, v_bounds[baseLayer]);
                                    ComputeCellcontentsBufferShader.SetVector(AtgBoundsCenterPID, v_boundsCenter[baseLayer]);
                                    ComputeCellcontentsBufferShader.SetVector(AtgBoundsExtentsPID, v_boundsExtents[baseLayer]);
                                    ComputeCellcontentsBufferShader.Dispatch(ComputeCellcontentsKernel, threadGroups, 1, 1);
                                }
                            }
                        }
                        UnityEngine.Profiling.Profiler.EndSample();
                        break;
                //  Init and Draw
                    case 2 :
                        UnityEngine.Profiling.Profiler.BeginSample("InitCellContent Delegated");
                    //  Finalize the initialisation of last updated cell – which has to be done on the main thread.
                        for (int j = 0; j < NumberOfLayersInCell; j++) {
                            CellContent[ CurrentCell.CellContentIndexes[j] ].InitCellContent_Delegated(UseCompute, usingSinglePassInstanced);
                        }
                        CurrentCell.state = 3;
                        UnityEngine.Profiling.Profiler.EndSample();
                    //  Draw
                        if (!UseCompute) {
                            UnityEngine.Profiling.Profiler.BeginSample("Draw Grass");
                            for (int j = 0; j < NumberOfLayersInCell; j++) {
                                if(CellContent[CurrentCell.CellContentIndexes[j]].state == 3) {
                                    if(useLppv) {
                                        CellContent[ CurrentCell.CellContentIndexes[j] ].DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer, TerrainShift, lppv, true, renderingLayerMask);
                                    }
                                    else {
                                        CellContent[ CurrentCell.CellContentIndexes[j] ].DrawCellContent_Delegated( CameraInWichGrassWillBeDrawn, CameraLayer, TerrainShift, null, false, renderingLayerMask);  
                                    }
                                }
                            }
                            UnityEngine.Profiling.Profiler.EndSample();
                        }
                        break;
                //  Queue Cell
                    case 0 :
                        if (CurrentCell.CellContentCount > 0) {
                            CurrentCell.state = 1;
                            CellsOrCellContentsToInit.Add( CurrentCell.index );
                        }
                        break;
                //  Cell is queued but not finished yet – nothing to do.
                    default:
                        break;
                }
            }

        //  Initialise the next queued cell
            var CellsOrCellContentsToInitCount = CellsOrCellContentsToInit.Count;

            if (!ThreadIsRunning) {  
                wt_cellindexList.Clear();
                wt_cellindexListCount = 0;
                for (int c = 0; c < CellsPerFrame; c++) {
                    if (CellsOrCellContentsToInitCount > 0) {
                        UnityEngine.Profiling.Profiler.BeginSample("Queue Cell");
                    //  Check if the cell has already been released 
                        if (Cells[ CellsOrCellContentsToInit[0] ].state != 1) {
                            CellsOrCellContentsToInit.RemoveAt(0);
                            if(CellsOrCellContentsToInitCount == 1) {
                                return;
                            }
                        }
                        CurrentCell = Cells[CellsOrCellContentsToInit[0]];
                        NumberOfLayersInCell = CurrentCell.CellContentCount;
                        int Layer = 0;
                        for (int j = 0; j < NumberOfLayersInCell; j++) {
                            CurrentCellContent = CellContent[ CurrentCell.CellContentIndexes[j] ];
                            Layer = CurrentCellContent.Layer;
                            // As Unity < 5.6.3 does not support MaterialPropertyBlocks properly
                            #if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
                                CurrentCellContent.v_mat = new Material(v_mat[Layer]);
                            #else
                                CurrentCellContent.v_mat = v_mat[Layer];
                            #endif
                            CurrentCellContent.v_mesh = v_mesh[Layer];
                            CurrentCellContent.ShadowCastingMode = ShadowCastingMode[Layer];
                            CurrentCellContent.motionVectorGenerationMode = motionVectorGenerationMode[Layer];
                        }

#if UNITY_EDITOR
                        if(!Application.isPlaying && DrawInEditMode)
                        {
                        //  Create Matrices on the main thread
                            InitCellContent(CellsOrCellContentsToInit[0]); // Layer, 0, CellsOrCellContentsToInit[0], true );                       
                        }
                        else  
                        {
                        //  Create Matrices asynchronously
                            wt_cellindexList.Add(CellsOrCellContentsToInit[0]);
                        }
#else
                    //  Create Matrices asynchronously
                        wt_cellindexList.Add(CellsOrCellContentsToInit[0]);
#endif
                        //ThreadIsRunning = false;

                        CellsOrCellContentsToInit.RemoveAt(0);
                        CellsOrCellContentsToInitCount -= 1;
                        wt_cellindexListCount += 1;
                        UnityEngine.Profiling.Profiler.EndSample();
                        
                    }
                    else {
                        break;
                    }
                }
            //  Virtually "start" worker thread
                if (wt_cellindexListCount > 0) {
                    WorkerThreadWait.Set();
                }
            } // End Init cell

        //  Compute: Draw grass
            if (UseCompute) {

                var tempBounds = new Bounds(CamPosition, new Vector3(1000, 1000, 1000));
                for (int i = 0; i < OrigNumberOfLayers; i ++) {
                    int layer = FinalLayersIndices[i];
                //  Skip merged layers
                    if (layer == -1) {
                        continue;
                    }
                    var currentLayer = layerData[layer];
                    currentLayer.commandBuf.SetData(currentLayer.commandData);
                    GraphicsBuffer.CopyCount(MergedCellcontentsBuffer[layer], currentLayer.commandBuf, /*dstOffsetBytes*/ sizeof(uint));

                //  When using single pass instanced we have to double the amount of drawn instances.
                    //works but costs 10ms!
                    /*if(usingSinglePassInstanced) {
                    argsBuffer[layer].GetData(t_args);
                    t_args[1] += t_args[1];
                    argsBuffer[layer].SetData(t_args);
                    }*/
                    // So we are using another compute shader.
                    if(usingSinglePassInstanced) {
//                        DoubleCopyCountShader.SetBuffer(DoubleCopyCountKernel, ArgsBufferPID, argsBuffer[layer]);
//                        DoubleCopyCountShader.Dispatch(DoubleCopyCountKernel, 1, 1, 1);  
                    }

//MergedMatrixBlock[layer].Clear();
//MergedMatrixBlock[layer].SetBuffer(GrassMatrixBufferPID, MergedCellcontentsBuffer[layer] );

                //  Set up Layer
                    currentLayer.rp.worldBounds = tempBounds;
                    currentLayer.rp.camera = CameraInWichGrassWillBeDrawn;
                    currentLayer.rp.layer = CameraLayer;
                    currentLayer.rp.renderingLayerMask = renderingLayerMask;

                    //currentLayer.rp.renderingLayerMask = 0u;
                    //currentLayer.rp.lightProbeUsage = LightProbeUsage.Off;
                    currentLayer.rp.matProps.Clear();
                    currentLayer.rp.matProps.SetBuffer(GrassMatrixBufferPID, MergedCellcontentsBuffer[layer] );
                        
                    if(useLppv) {
                        // Graphics.DrawMeshInstancedIndirect(
                        //     v_mesh[i],
                        //     0,
                        //     v_mat[i],
                        //     tempBounds,
                        //     argsBuffer[layer],
                        //     0,
                        //     MergedMatrixBlock[layer],
                        //     ShadowCastingMode[i],
                        //     true,
                        //     CameraLayer,
                        //     CameraInWichGrassWillBeDrawn,
                        //     LightProbeUsage.UseProxyVolume,
                        //     lppv
                        // );
                    }
                    else {
                        Graphics.RenderMeshIndirect(
                            currentLayer.rp,
                            v_mesh[i],
                            currentLayer.commandBuf,
                            1  // commandCount[layer]
                        );   
                    }
                }
            }

        } // End Update


//  --------------------------------------------------------------------
//  From here on everything has to be thread safe


        public float GetHeight(int x, int y) {
        //  Adjust x positions to match our array
            //x = (int)Mathf.Clamp(x, 0, TerrainHeightmapWidth - 1);
            //y = (int)Mathf.Clamp(y, 0, TerrainHeightmapWidth - 1);
            return TerrainHeights [ x * TerrainHeightmapWidth + y ];
        }


//  --------------------------------------------------------------------
//  Bilinear sampling from the height array

        public float GetfilteredHeight(float normalizedHeightPos_x, float normalizedHeightPos_y) {
        //  NOTE: Using Floor and Ceil each take 0.8ms - so we go with (int) instead
            int normalizedHeightPosLower_x = (int)(normalizedHeightPos_x);
            int normalizedHeightPosLower_y = (int)(normalizedHeightPos_y);
            int normalizedHeightPosUpper_x = (int)(normalizedHeightPos_x + 1.0f);
            int normalizedHeightPosUpper_y = (int)(normalizedHeightPos_y + 1.0f); 

        //  Get weights
            float Lowerx = (normalizedHeightPos_x - normalizedHeightPosLower_x);
            float Upperx = (normalizedHeightPosUpper_x - normalizedHeightPos_x);
            float Lowery = (normalizedHeightPos_y - normalizedHeightPosLower_y);
            float Uppery = (normalizedHeightPosUpper_y - normalizedHeightPos_y);

        //  Adjust x positions to match our array
            normalizedHeightPosLower_x *= TerrainHeightmapHeight; // "* height" as we sample height in the inner loop
            normalizedHeightPosUpper_x *= TerrainHeightmapHeight;

            // NOTE: We simply use "swapped weights" in order to not have to calculate (1 - factor)
            float HeightSampleLowerRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosLower_y ] * Upperx;
                  HeightSampleLowerRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosLower_y ] * Lowerx;
            float HeightSampleUpperRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosUpper_y ] * Upperx;
                  HeightSampleUpperRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosUpper_y ] * Lowerx;
            return HeightSampleLowerRow * Uppery + HeightSampleUpperRow * Lowery;
        }

//  --------------------------------------------------------------------
//  The worker thread.
        void InitCellContentOnThread() {
            WorkerThreadWait.Reset();
            // Blocks the thread until the current WaitHandle receives a signal.
            WorkerThreadWait.WaitOne();
            while(true) {
                WorkerThreadWait.Reset();
                bool isStopped = false;
                lock (threadEndSync) {
                    isStopped = stopThread;
                }
                if (isStopped) {
                    return;
                }
                ThreadIsRunning = true;
                
                try {
                    for(int c = 0; c < wt_cellindexListCount; c++) {
                        InitCellContent(wt_cellindexList[c]);
                    }
                }
                catch (Exception e) {
                    Debug.LogWarning("GrassManager thread crashed: " + e);
                    ThreadIsRunning = false;
                    return;
                }
                ThreadIsRunning = false;
                WaitHandle.SignalAndWait(MainThreadWait, WorkerThreadWait);
            }
        }

//  --------------------------------------------------------------------
//  The function which runs on the worker thread.
        public float InitCellContent(int cellIndex) {
        
            GrassCell CurrentCell = Cells[ cellIndex ];
            GrassCellContent CurrentCellContent;

            int index = 0;
            int layer = 0;
            int loopCounter = CurrentCell.CellContentCount;

            // Loop over all Cell Contents
            for (int c = 0; c < loopCounter; c ++) {

                //  Here we have to overwrite the initial value
                    index = CurrentCell.CellContentIndexes[c];
                    CurrentCellContent = CellContent[index];
                    layer = CurrentCellContent.Layer;
                                        
                    int tempMatrixArrayPointer = 0;

                    samplePosition.x = CurrentCellContent.Pivot.x;
                    samplePosition.y = CurrentCellContent.Pivot.z;
                    tempSamplePosition = samplePosition;

                    var Rotation = (int)InstanceRotation[layer];
                    var DoWriteNormalBuffer = WriteNormalBuffer[layer];

                    var noise = Noise[layer];

                    float tempMinSize = MinSize[layer];
                    float tempMaxSize = MaxSize[layer] - tempMinSize;

                //  Outer loop for softly merged layers
                    int outerLoop = 1;
                    if (CurrentCellContent.SoftlyMergedLayers != null) {
                        outerLoop += CurrentCellContent.SoftlyMergedLayers.Length;
                    }

                    for (int layers = 0; layers < outerLoop; layers ++) {

                    //  This still creates a lot of garbage..
                        //random = new System.Random(cellIndex + layer + layers * 9949  ); // + layers as otherwise simple quads might fall upon each other

                    //  Reset
                        tempSamplePosition = samplePosition;
                        
                    //  Tweak layer to point at softly merged Layer
                        if( layers > 0) {
                            layer = CurrentCellContent.SoftlyMergedLayers[ layers - 1 ];
                            noise = Noise[layer];
                            tempMinSize = MinSize[layer];
                            tempMaxSize = MaxSize[layer] - tempMinSize;
                        }

                        for(int x = 0; x < NumberOfBucketsPerCell; x++) {
                            for(int z = 0; z < NumberOfBucketsPerCell; z++) { 

                            //  Creating a random for each bucket fixes the problem of jumping grass when some of it is removed...
                                ATGSeed = (ulong)(cellIndex + layer * 347 + layers * 53 - (x * NumberOfBucketsPerCell + z) * 7 );
                                ATGSeed ^= ATGSeed << 13;
                                ATGSeed ^= ATGSeed >> 17;
                                ATGSeed ^= ATGSeed << 5;

                            //  worldPos --> localPos as all terrain data is relative
                                Vector2 terrainLocalPos;
                                // per component: 21ms vs Vector3: 23ms (10000 times)
                                terrainLocalPos.x = tempSamplePosition.x;
                                terrainLocalPos.y = tempSamplePosition.y;

                                int density = mapByte[layer][ 
                                    CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
                                    + x * (int)TerrainDetailSize.y
                                    + z
                                ];

// Get averaged density / Needs safe guards!
/* 
float t_density = density + density
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x+1) * (int)TerrainDetailSize.y
        + z
    ]
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x-1) * (int)TerrainDetailSize.y
        + z
    ]
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x) * (int)TerrainDetailSize.y
        + z + 1
    ]
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x) * (int)TerrainDetailSize.y
        + z - 1
    ]
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x+1) * (int)TerrainDetailSize.y
        + z+1
    ]
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x-1) * (int)TerrainDetailSize.y
        + z+1
    ]
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x-1) * (int)TerrainDetailSize.y
        + z + 1
    ]
+    mapByte[layer][ 
        CurrentCellContent.PatchOffsetX + CurrentCellContent.PatchOffsetZ
        + (x-1) * (int)TerrainDetailSize.y
        + z - 1
    ];
t_density /=10.0f;
// okish...
density = (int)(density * Mathf.Min(1.0f, t_density/15));
*/

                              
                            //  We have to use Ceil here as otherwise we may create empty buffers.
                                density = (int)/*Math.Ceiling*/(density                                         * CurrentDetailDensity);

                            //  When sampling the height map we may not go (x || y < 0). So we use clamped factors:
                                float OneOverHeightmapWidthClamped = (terrainLocalPos.x < TerrainSizeOverHeightmap) ? 0.0f : OneOverHeightmapWidth;
                                float OneOverHeightmapWidthClampedRight = (terrainLocalPos.x >= OneOverHeightmapWidthRight) ? 0.0f : OneOverHeightmapWidth;    
                                float OneOverHeightmapHeightClamped = (terrainLocalPos.y < TerrainSizeOverHeightmap) ? 0.0f : OneOverHeightmapHeight;
                                float OneOverHeightmapHeightClampedUp = (terrainLocalPos.y >= OneOverHeightmapHeightUp) ? 0.0f : OneOverHeightmapHeight;

                            //  Now that we have the density for the Bucket we spawn x instances based on density within the given Bucket.

                                for(int i = 0; i < density; i++) {

                                //  Random Offsets
                                    float rand = GetATGRandomNext();

                                    float XOffset = rand * BucketSize;
                                    rand = GetATGRandomNext();

                                    float ZOffset = rand * BucketSize; 

                                //  localPos --> normalizedPos in 0-1 range to sample height and normal
                                //  0.4ms using the built in functions – most expensive part here
                                //  UnityEngine.Profiling.Profiler.BeginSample("read from terrain");
                                    Vector2 normalizedPos;
                                    normalizedPos.x = (terrainLocalPos.x + XOffset) * OneOverTerrainSize.x;
                                    normalizedPos.y = (terrainLocalPos.y + ZOffset) * OneOverTerrainSize.z;

                                //  UnityEngine.Profiling.Profiler.BeginSample("manually sample height"); // manually: 0.18ms / built in: 0.3ms!!!!!
                                //  tempPosition.y = terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);   // 0.08ms

                                //  Get bilinear filtered height value, see: https://en.wikipedia.org/wiki/Bilinear_interpolation
                                    float normalizedHeightPos_x = normalizedPos.x * (TerrainHeightmapWidth  - 1); // "- 1" because we are working with indexes here
                                    float normalizedHeightPos_y = normalizedPos.y * (TerrainHeightmapHeight - 1);
                                
                                //  NOTE: Using Floor and Ceil each take 0.8ms - so we go with (int) instead
                                    int normalizedHeightPosLower_x = (int)(normalizedHeightPos_x);
                                    int normalizedHeightPosLower_y = (int)(normalizedHeightPos_y);
                                    int normalizedHeightPosUpper_x = normalizedHeightPosLower_x + 1;
                                    int normalizedHeightPosUpper_y = normalizedHeightPosLower_y + 1; 

                                //  Get weights
                                    float Lower_x = (normalizedHeightPos_x - normalizedHeightPosLower_x);
                                    //float Upperx = (normalizedHeightPosUpper_x - normalizedHeightPos_x);
                                    float Lower_y = (normalizedHeightPos_y - normalizedHeightPosLower_y);
                                    //float Uppery = (normalizedHeightPosUpper_y - normalizedHeightPos_y);

                                //  Adjust x positions to match our array
                                    normalizedHeightPosLower_x *= TerrainHeightmapHeight;
                                    normalizedHeightPosUpper_x *= TerrainHeightmapHeight;

                                    // NOTE: We simply use "swapped weights" in order to not have to calculate (1 - factor)
                                //  float HeightSampleLowerRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosLower_y ] * Upperx;
                                //        HeightSampleLowerRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosLower_y ] * Lowerx;
                                //  float HeightSampleUpperRow =  TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosUpper_y ] * Upperx;
                                //        HeightSampleUpperRow += TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosUpper_y ] * Lowerx;
                                //  Final bilinear filtered height value
                                //  tempPosition.y = HeightSampleLowerRow * Uppery + HeightSampleUpperRow * Lowery;
                                
                                //  To better match the geoemtry (triangles...) we only use 3 sample points and no bilinear sampling.
                                    
                                    float HeightLowerLeft  = TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosLower_y ];
                                    float HeightUpperRight = TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosUpper_y ];
                                    if( Lower_x > Lower_y) {
                                        // we use the lower right sample
                                        //      11
                                        //      |
                                        // 00---10
                                        float HeightLowerRight  = TerrainHeights [ normalizedHeightPosUpper_x + normalizedHeightPosLower_y ];
                                        tempPosition.y = HeightLowerLeft + (HeightLowerRight - HeightLowerLeft) * Lower_x + (HeightUpperRight - HeightLowerRight) * Lower_y;
                                    }
                                    else {
                                        // we use the upper left sample
                                        // 01---11
                                        // |
                                        // 00
                                        float HeightUpperLeft  = TerrainHeights [ normalizedHeightPosLower_x + normalizedHeightPosUpper_y ];
                                        tempPosition.y = HeightLowerLeft + (HeightUpperRight - HeightUpperLeft) * Lower_x + (HeightUpperLeft - HeightLowerLeft) * Lower_y;
                                    }

                                //  UnityEngine.Profiling.Profiler.EndSample();
                                //  UnityEngine.Profiling.Profiler.BeginSample("manually sample normal");

                                //  normal  = terData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);  // 0.5ms!!!!!! super expensive, so we better cache it in an array?
                                //  Manually sample and interpolate the normal
                                    Vector3 normal; // = Vector3.Cross(va, vb);
                                //  NOTE: When looking up the height map we need guards!!!!!!                       
                                    float Left = GetfilteredHeight( 
                                                    (normalizedPos.x - OneOverHeightmapWidthClamped) * (TerrainHeightmapWidth - 1),
                                                    normalizedPos.y * (TerrainHeightmapHeight - 1)
                                                );
                                    float Right = GetfilteredHeight( 
                                                    (normalizedPos.x + OneOverHeightmapWidthClampedRight) * (TerrainHeightmapWidth - 1),
                                                    normalizedPos.y * (TerrainHeightmapHeight - 1)
                                                );
                                    float Up = GetfilteredHeight( 
                                                    normalizedPos.x * (TerrainHeightmapWidth - 1),
                                                    (normalizedPos.y + OneOverHeightmapHeightClampedUp) * (TerrainHeightmapHeight - 1)
                                                );
                                    float Down = GetfilteredHeight( 
                                                    normalizedPos.x * (TerrainHeightmapWidth - 1),
                                                    (normalizedPos.y - OneOverHeightmapHeightClamped) * (TerrainHeightmapHeight - 1)
                                                );

                                    // https://stackoverflow.com/questions/33736199/calculating-normals-for-a-height-map
                                    // Vector3 vertical = new Vector3(0.0f, Up - Down, 2.0f );
                                    // Vector3 horizontal = new Vector3(2.0f, Right - Left, 0.0f );
                                    // normal = Vector3.Cross(vertical, horizontal);
                                    // Manually calculated cross product:
                                    normal.x = -2.0f * (Right - Left);
                                //  if aligned to terrain
                                    if(Rotation != 2  && Rotation != 4 ) {
                                        normal.y = 2.0f * 2.0f    *   1.570796f    *   TerrainSizeOverHeightmap; // Magic number? At least it is half PI.
                                    }
                                //  upright oriented
                                    else {
                                        normal.y = 2.0f * 2.0f * TerrainSizeOverHeightmap; 
                                    }
                                    normal.z = (Up - Down) * -2.0f;
                                //  normal.Normalize(); // Takes about 0.77ms for 593 calls! – doing it manually only takes 0.14ms
                                    float lengthSqr = normal.x * normal.x + normal.y * normal.y + normal.z * normal.z;
                                    float length = (float)Math.Sqrt((double)lengthSqr);
                                    float inverseLength = 1.0f / length;
                                    normal.x *= inverseLength;
                                    normal.y *= inverseLength;
                                    normal.z *= inverseLength;

                                //  UnityEngine.Profiling.Profiler.EndSample();
                                //  UnityEngine.Profiling.Profiler.EndSample();

                                //  Set Position in Worldspace
                                    tempPosition.x = tempSamplePosition.x + XOffset   +   TerrainPosition.x;
                                    tempPosition.z = tempSamplePosition.y + ZOffset   +   TerrainPosition.z;

                                //  Set scale
                                    float scale = tempMinSize + Mathf.PerlinNoise(tempPosition.x * noise, tempPosition.z * noise) * tempMaxSize;

// Scale down instances based on averaged density
// Too noisy. Some big instances stick out.
/*
t_density = Mathf.Min(1.0f, t_density/16.0f);
scale = Mathf.Lerp(tempMinSize * 0.75f, tempMinSize, t_density) + Mathf.PerlinNoise(tempPosition.x * noise, tempPosition.z * noise) * tempMaxSize * t_density;
*/


                                //  IMPORTANT: We have to use uniform scaling as otherwise we would need a proper WorldToObject matrix or hack UnityShaderUtilities.cginc
                                    tempScale.x = scale;
                                    tempScale.y = scale;
                                    tempScale.z = scale;

                                //  Align grass to terrain normal / fromto rotation
                                //  q = Quaternion.FromToRotation(UpVec, normal);
                                //  https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another
                                    Quaternion q = ZeroQuat;
                                    if(Rotation != 2) {
                                    /*  Vector3 a = Vector3.Cross(UpVec, normal);
                                        q.x = a.x;
                                        q.y = a.y;
                                        q.z = a.z; */
                                    //  As we use the up vector we can simplify it like this:
                                        q.x = normal.z;
                                        q.y = 0;
                                        q.z =  -normal.x;
                                        //q.w = Mathf.Sqrt(( v1.Magnitude ^ 2) * ( v2.Length ^ 2)) + dotproduct(v1, v2);
                                    //  Another simplification here:
                                        // q.w = Mathf.Sqrt( 1.0f + Vector3.Dot(Vector3.up, normal) );
                                        q.w = (float)Math.Sqrt( 1.0f + normal.y);
                                    //  Normalize!
                                        var OneOverqMagnitude = (float)(1.0 / Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z));
                                        q.w *= OneOverqMagnitude;
                                        q.x *= OneOverqMagnitude;
                                        q.y *= OneOverqMagnitude;
                                        q.z *= OneOverqMagnitude;
                                    }
                                    
                                //  Now add random rotation around y axis
                                    float HalfAngle = GetATGRandomNext() * 270.0f; // 180.0f;

                                    float Cos = (float)Math.Cos(HalfAngle);
                                    float Sin = (float)Math.Sin(HalfAngle);
                                /*  q *= rotation;
                                    which is: return new Quaternion(
                                        lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                                        lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                                        lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                                        lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z); */
                                //  As we only rotate around the y axis, we can do better:
                                    var lhs = q;
                                    q.x = lhs.x * Cos - lhs.z * Sin;
                                    q.y = lhs.w * Sin + lhs.y * Cos;
                                    q.z = lhs.z * Cos + lhs.x * Sin;
                                    q.w = lhs.w * Cos - lhs.y * Sin;
                                //  Rotation around x axis (for e.g. small stones)
                                    if(Rotation == 1) {
                                        HalfAngle = GetATGRandomNext() * 180.0f;
                                        float SinX = (float)Math.Sin(HalfAngle); // --> rhs.x
                                        float CosX = (float)Math.Cos(HalfAngle); // --> rhs.w

                                        lhs = q;
                                        q.x = lhs.w * SinX + lhs.x * CosX;
                                        q.y = lhs.y * CosX + lhs.z * SinX;
                                        q.z = lhs.z * CosX - lhs.y * SinX;
                                        q.w = lhs.w * CosX - lhs.x * SinX;
                                    }
                                //  q.x = lhs.w * SinX + lhs.x * Cos /*+ lhs.y * rhs.z*/ - lhs.z * Sin;
                                //  q.y = lhs.w * Sin + lhs.y * CosX + lhs.z * SinX /*- lhs.x * rhs.z*/;
                                //  q.z = /*lhs.w * rhs.z +*/ lhs.z * Cos + lhs.x * Sin - lhs.y * SinX;
                                //  q.w = lhs.w * Cos - lhs.x * SinX - lhs.y * Sin /*- lhs.z * rhs.z*/;
                                    
                                //  Set Matrix TRS   
                                    //  UnityEngine.Profiling.Profiler.BeginSample("set");
                                    //  tempMatrix.SetTRS( tempPosition, q, tempScale );
                                    //  this is almost 1/3rd of the time of the whole loop! 0.8ms out of 2.3ms
                                    //  Doing it manually takes 2.5 – 3.6ms (10.000 calls) (0.6 - 0.8 ms in built) ---> 2.6 times faster! (1.5 times faster in built)
                                    //  Matrix needs to be normalized!?

                                //  Set position
                                    tempMatrix.m03 = tempPosition.x;
                                    tempMatrix.m13 = tempPosition.y + TerrainPosition.y; // Add terrain y pos
                                    tempMatrix.m23 = tempPosition.z;

                                //  Set rotation
                                    var DoubleSqrqx = 2.0f * q.x*q.x;
                                    var DoubleSqrqy = 2.0f * q.y*q.y;
                                    var DoubleSqrqz = 2.0f * q.z*q.z;

                                    tempMatrix.m00 = 1.0f - DoubleSqrqy/*2.0f * q.y*q.y*/ - DoubleSqrqz /* 2.0f * q.z*q.z*/;
                                    tempMatrix.m01 = 2.0f * q.x * q.y - 2.0f * q.z * q.w;
                                    tempMatrix.m02 = 2.0f * q.x * q.z + 2.0f * q.y * q.w;
                                    
                                    tempMatrix.m10 = 2.0f * q.x * q.y + 2.0f * q.z * q.w;
                                    tempMatrix.m11 = 1.0f - DoubleSqrqx /* 2.0f * q.x*q.x*/ - DoubleSqrqz /* 2.0f * q.z*q.z*/;
                                    tempMatrix.m12 = 2.0f * q.y * q.z - 2.0f * q.x * q.w;

                                    tempMatrix.m20 = 2.0f * q.x * q.z - 2.0f * q.y * q.w;
                                    tempMatrix.m21 = 2.0f * q.y * q.z + 2.0f * q.x * q.w;
                                    tempMatrix.m22 = 1.0f - DoubleSqrqx/* 2.0f * q.x*q.x*/ - DoubleSqrqy/* 2.0f * q.y*q.y*/;

                                //  Set scale
                                    tempMatrix.m00 *= tempScale.x;
                                    tempMatrix.m01 *= tempScale.y;
                                    tempMatrix.m02 *= tempScale.z;
                                    tempMatrix.m10 *= tempScale.x;
                                    tempMatrix.m11 *= tempScale.y;
                                    tempMatrix.m12 *= tempScale.z;
                                    tempMatrix.m20 *= tempScale.x;
                                    tempMatrix.m21 *= tempScale.y;
                                    tempMatrix.m22 *= tempScale.z;

                                //  UnityEngine.Profiling.Profiler.EndSample();
                                    
                                    if(Rotation == 2 && DoWriteNormalBuffer) {
                                    //  As the terrain normal will be rotated around the y axis (HalfAngle) by the unity_ObjectToWorld matrix we have to rotate it the other way around
                                        var rotatedTerrainNormal = normal;
                                        float MinusHalfAngle = -HalfAngle;
                                        Cos = (float)Math.Cos(MinusHalfAngle);
                                        Sin = (float)Math.Sin(MinusHalfAngle);
                                    //  Build and apply a simplified quaternion
                                        float num2 = Sin * 2.0f;
                                        float num5 = Sin * num2;
                                        float num11 = Cos * num2;
                                        rotatedTerrainNormal.x = (1.0f - (num5)) * normal.x + (num11) * normal.z;
                                        rotatedTerrainNormal.z = (- num11) * normal.x + (1.0f - (num5)) * normal.z;
                                    //  Add terrain normal to list
                                        tempMatrix.m30 = rotatedTerrainNormal.x;
                                        tempMatrix.m31 = rotatedTerrainNormal.y;
                                        tempMatrix.m32 = rotatedTerrainNormal.z;
                                    }
                                    else {
                                    //  These are always zero in case we do not need the terrain normal
                                        tempMatrix.m30 = 0.0f; //*= tempScale.x;
                                        tempMatrix.m31 = 0.0f; //*= tempScale.y;
                                        tempMatrix.m32 = 0.0f; //*= tempScale.z;
                                    }
                                //  tempMatrix.m33 is always 1! So layers gets written to the integer part / scale to the fractional
                                    tempMatrix.m33 = layers + tempScale.x * 0.01f;
                                //  Add final matrix
                                    tempMatrixArray[tempMatrixArrayPointer] = tempMatrix;
                                    tempMatrixArrayPointer++;
                                }
                                tempSamplePosition.y += BucketSize;
                            }
                            tempSamplePosition.y = samplePosition.y;
                            tempSamplePosition.x += BucketSize;
                        }

                    } // end outerLoop for softly merged layers

                    CurrentCellContent.v_matrices = new Matrix4x4[tempMatrixArrayPointer];
                    System.Array.Copy(tempMatrixArray, CurrentCellContent.v_matrices, tempMatrixArrayPointer);
                } // end cell content    
                CurrentCell.state = 2;
        //  no callback
            return 1;
        }

// ---------------

#if UNITY_EDITOR
            void OnGUI() {
                if(DebugStats) {
                    var Alignement = GUI.skin.box.alignment;
                    GUI.skin.box.alignment = TextAnchor.MiddleLeft;
                    GUI.Box(new Rect(10, 10, 340, 40), "Queried Cells: " + numResults.ToString() + " / Visible Cells: " + numOfVisibleCells.ToString() +  " / Cells to init: " + CellsOrCellContentsToInit.Count.ToString() );
                    GUI.skin.box.alignment = Alignement;
                }
            }
#endif

#if UNITY_EDITOR
            void OnDrawGizmos () {
                if(DebugCells) {
                    Gizmos.color = new Color(0, 1, 0, 0.2f);
                    GrassCell CurrentCell;
                    int CellContentState;
                    //for (int i = 0; i < numResults; i ++) {
                
                //  cached cells

                    float radius = (CellSize * 0.5f);
                    radius *= radius;
                    radius *=2;
                    radius = Mathf.Sqrt(radius);

                    for (int i = 0; i < NumberOfCells*NumberOfCells; i ++) {
                        CurrentCell = Cells[ i ];
                        CellContentState = CurrentCell.state;
                        var pos = CurrentCell.Center - TerrainShift;
                        if (CellContentState == 3) {
                            Gizmos.DrawCube(pos, new Vector3(CellSize, 1, CellSize));
                        }
                        //Gizmos.DrawWireSphere(pos, radius);
                    }

                // visible cells
                    Gizmos.color = new Color(1, 0, 0, 1.0f);
                    for (int i = 0; i < numResults; i ++) {
                        if(IngnoreOcclusion) {
                            if (isVisibleBoundingSpheres[ resultIndices[i] ] == false) {
                                continue;
                            }
                        }
                        CurrentCell = Cells[ resultIndices[i] ];
                        CellContentState = CurrentCell.state;
                        if (CellContentState == 3) {
                            Gizmos.color = new Color(0, 1, 0, 1.0f);
                            var pos = boundingSpheres[CurrentCell.index].position; //  CurrentCell.Center - TerrainShift;
                            Gizmos.DrawWireCube(pos, new Vector3(CellSize, 1, CellSize));
                            radius = boundingSpheres[CurrentCell.index].radius;
                //            Gizmos.DrawWireSphere(pos, radius);
                        }
                        else {
                            if (CurrentCell.CellContentCount > 0) {
                                Gizmos.color = new Color(1, 0, 0, 1.0f);
                                var pos = CurrentCell.Center - TerrainShift;
                                Gizmos.DrawWireCube(pos, new Vector3(CellSize, 1, CellSize));
                            }
                        }
                    }

                }
            }
#endif

// ---------------

    }
}
