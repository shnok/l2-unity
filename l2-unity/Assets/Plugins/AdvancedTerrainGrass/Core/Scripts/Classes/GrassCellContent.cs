using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace AdvancedTerrainGrass {

	[Serializable] 
	public class GrassCellContent  {

		public int index;
		public int Layer;

		public int[] SoftlyMergedLayers;

		public int state = 0; // 0 -> not initialized; 1 -> queued; 2 -> readyToInitialize; 3 -> initialized

		public Mesh v_mesh;
		public Material v_mat;
		public Material d_mat;

        public int GrassMatrixBufferPID;

		public ShadowCastingMode ShadowCastingMode = ShadowCastingMode.Off;
		public MotionVectorGenerationMode motionVectorGenerationMode;

		public int Instances;
		public Vector3 Center;	// center in world space
		public Vector3 Pivot; 	// lower left corner in local terrain space
		
		public Matrix4x4[] v_matrices;
		
		public int PatchOffsetX;
		public int PatchOffsetZ;

        [System.NonSerialized]
        public GraphicsBuffer matrixBuffer;
        [System.NonSerialized]
		public RenderParams rp;
		[System.NonSerialized]
		public GraphicsBuffer commandBuf;
		[System.NonSerialized]
		public GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;

		private Bounds bounds = new Bounds();

		#if !UNITY_5_6_0 && !UNITY_5_6_1 && !UNITY_5_6_2
			public MaterialPropertyBlock block;
		#endif

	//	Function used to release a complete cellcontent – as on disable.	
		public void ReleaseCompleteCellContent() {
			state = 0;
			v_matrices = null;
			matrixBuffer?.Release();
			matrixBuffer = null;
			commandBuf?.Release();
			commandBuf = null;
			rp.matProps?.Clear();
		}

	//	Function used to release a cellcontent when it gets out of cache distance – this one only releases the memory heavy matrixbuffer.
		public void ReleaseCellContent() {
			state = 0;
			v_matrices = null;
			matrixBuffer?.Release();
			matrixBuffer = null;
			rp.matProps?.Clear();
		}

		public void InitCellContent_Delegated(bool UseCompute, bool usingSinglePassInstanced) {

		//	Safe guard - needed in some rare cases i could not reproduce... checking state == 3 did not work out?!
			if (v_matrices == null) {
				return;
			}

        //  Update number of Instances as it is used by Compute
            Instances = v_matrices.Length;

            UnityEngine.Profiling.Profiler.BeginSample("Create Matrix Buffer");
                if(Instances == 0) {
                    state = 0;
                    return;
                }
				matrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, Instances, 64);
				matrixBuffer.SetData(v_matrices);
			UnityEngine.Profiling.Profiler.EndSample();
        
        //  No need to keep the matrix array
            v_matrices = null;

            if (!UseCompute) {
                UnityEngine.Profiling.Profiler.BeginSample("Set Graphics Buffers");

                	bounds.center = Center;
                //	Mind order to get proper Extent :) (otherwise point lights might get lost)
                	var Extent = Math.Abs(Center.x - Pivot.x);
                	var Extents = new Vector3(Extent, Extent, Extent);
                	bounds.extents = Extents;

                	rp = new RenderParams(v_mat);
                	rp.worldBounds = bounds; //new Bounds(Vector3.zero, 10000 * Vector3.one);
                	rp.shadowCastingMode = ShadowCastingMode;
                	rp.motionVectorMode = motionVectorGenerationMode;
                	rp.matProps = new MaterialPropertyBlock();
                	rp.matProps.Clear();
                	rp.matProps.SetBuffer(GrassMatrixBufferPID, matrixBuffer);

                	commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
                    commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];
                    commandData[0].indexCountPerInstance = (uint)v_mesh.GetIndexCount(0);
                    commandData[0].instanceCount = (uint)Instances;
                    commandBuf.SetData(commandData);

                UnityEngine.Profiling.Profiler.EndSample();
            }

        //	Now we are ready to go.
        	state = 3;
	    }


		public void DrawCellContent_Delegated(Camera CameraInWichGrassWillBeDrawn, int CameraLayer, Vector3 TerrainShift, LightProbeProxyVolume lppv, bool useLppv, uint renderingLayerMask) {

            var t_bounds = bounds;
            var pos = t_bounds.center;
            pos.x -= TerrainShift.x;
            pos.y -= TerrainShift.y;
            pos.z -= TerrainShift.z;
            t_bounds.center = pos;

//rp.renderingLayerMask = renderingLayerMask;
            
            if(useLppv) {
	   //          Graphics.DrawMeshInstancedIndirect(
				// 	v_mesh,
				// 	0, 
				// 	v_mat,
				// 	t_bounds,
				// 	argsBuffer,
				// 	0,
	   //              block,
	   //              ShadowCastingMode,
				// 	true,
				// 	CameraLayer,
				// 	CameraInWichGrassWillBeDrawn,
				// 	LightProbeUsage.UseProxyVolume,
    //                 lppv
				// );
			}
			
			else {
				Graphics.RenderMeshIndirect(
                	rp,
                	v_mesh,
                	commandBuf
            	);	
			}
		}
	}
}
