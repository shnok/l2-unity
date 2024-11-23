// Based on: https://github.com/gokselgoktas/hi-z-buffer


using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedTerrainGrass {

    [RequireComponent(typeof(Camera))]
    public class AtgHiZBuffer : MonoBehaviour
    {
        private const int MAXIMUM_BUFFER_SIZE = 2048;

        private enum Pass {
            Blit,
            Reduce
        }

        public bool HiZavailable = false;

        private Shader m_Shader;
        public Shader shader {
            get {
                if (m_Shader == null)
                    m_Shader = Shader.Find("Hidden/AdvancedTerrainGrass/Hi-Z Buffer");
                return m_Shader;
            }
        }
        private Material m_Material;
        public Material material {
            get {
                if (m_Material == null) {
                    if (shader == null || shader.isSupported == false)
                        return null;
                    m_Material = new Material(shader);
                }
                return m_Material;
            }
        }

        private Camera m_Camera;
        public new Camera camera {
            get {
                if (m_Camera == null)
                    m_Camera = GetComponent<Camera>();
                return m_Camera;
            }
        }

        private RenderTexture m_HiZ;
        public RenderTexture texture {
            get {
                return m_HiZ;
            }
        }

        private int m_LODCount = 0;
        public int lodCount {
            get {
                if (m_HiZ == null)
                    return 0;

                return 1 + m_LODCount;
            }
        }

        private CommandBuffer m_CommandBuffer;
        private CameraEvent m_CameraEvent; // = CameraEvent.AfterDepthTexture; //BeforeReflections;

        private int[] m_Temporaries;

        private bool VRenabled = false;

        void OnEnable() {
            if (UnityEngine.XR.XRSettings.enabled) {
                VRenabled = true;
                //var renderingMode = UnityEngine.XR.XRSettings.stereoRenderingMode == UnityEngine.XR.XRSettings.StereoRenderingMode.SinglePass ? 0 : 1;
                //var buildSettings = Unity.XR.MockHMD.MockHMDBuildSettings.Instance;
                //if (buildSettings != null) {
                //    renderingMode = buildSettings.renderMode == Unity.XR.MockHMD.MockHMDBuildSettings.RenderMode.MultiPass ? 0 : 1;
                //}
                //if(renderingMode == 1) {
                    //Debug.Log("Single");
                //}
                //else {
                    //Debug.Log("Multi");
                //}
                return;
            }
            else {
                VRenabled = false;
            }

            camera.depthTextureMode |= DepthTextureMode.Depth;
            if(camera.actualRenderingPath == RenderingPath.DeferredShading) {
                m_CameraEvent = CameraEvent.BeforeReflections;   
            }
            else {
                m_CameraEvent = CameraEvent.AfterDepthTexture; 
            }
        }

        void OnDisable() {
            if (camera != null)
            {
                if (m_CommandBuffer != null)
                {
                    camera.RemoveCommandBuffer(m_CameraEvent, m_CommandBuffer);
                    m_CommandBuffer = null;
                }
            }

            if (m_HiZ != null)
            {
                m_HiZ.Release();
                m_HiZ = null;
            }
        }

        void OnPreRender() {

            if(VRenabled) {
                return;
            }

            //int size = (int)Mathf.Max((float)camera.pixelWidth, (float)camera.pixelHeight);
            //size = (int)Mathf.Min((float)Mathf.NextPowerOfTwo(size), (float)MAXIMUM_BUFFER_SIZE);
            //m_LODCount = (int)Mathf.Floor(Mathf.Log(size, 2f));
            //bool isCommandBufferInvalid = false;

            int sizeX = camera.pixelWidth;
            int sizeY = camera.pixelHeight;
            m_LODCount = (int)Mathf.Floor(Mathf.Log(sizeX, 2f));
            bool isCommandBufferInvalid = false;

            if (m_LODCount == 0)
                return;

            if (m_HiZ == null || (m_HiZ.width != sizeX || m_HiZ.height != sizeY)) {
                if (m_HiZ != null)
                    m_HiZ.Release();
// was Float
                m_HiZ = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
                m_HiZ.filterMode = FilterMode.Point;
                m_HiZ.useMipMap = true;
                m_HiZ.autoGenerateMips = false;
                m_HiZ.Create();
                m_HiZ.hideFlags = HideFlags.HideAndDontSave;
                isCommandBufferInvalid = true;

                HiZavailable = true;
            }

            if (m_CommandBuffer == null || isCommandBufferInvalid == true) {
                m_Temporaries = new int[m_LODCount];

                if (m_CommandBuffer != null)
                    camera.RemoveCommandBuffer(m_CameraEvent, m_CommandBuffer);

                m_CommandBuffer = new CommandBuffer();
                m_CommandBuffer.name = "Hi-Z Buffer";
                RenderTargetIdentifier id = new RenderTargetIdentifier(m_HiZ);
                m_CommandBuffer.Blit(null, id, material, (int)Pass.Blit);

                for (int i = 0; i < m_LODCount; ++i) {
                    m_Temporaries[i] = Shader.PropertyToID("_09659d57_Temporaries" + i.ToString());
                    //size >>= 1;
                    //if (size == 0)
                    //    size = 1;
                    sizeX >>= 1;
                    sizeY >>= 1;
                    if(sizeX == 0) {
                        sizeX = 1;
                    }
                    if(sizeY == 0) {
                        sizeY = 1;
                    }
 // was RGFloat
                    m_CommandBuffer.GetTemporaryRT(m_Temporaries[i], sizeX, sizeY, 0, FilterMode.Point, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
                    if (i == 0) 
                        m_CommandBuffer.Blit(id, m_Temporaries[0], material, (int)Pass.Reduce);
                    else
                        m_CommandBuffer.Blit(m_Temporaries[i - 1], m_Temporaries[i], material, (int)Pass.Reduce);
                    m_CommandBuffer.CopyTexture(m_Temporaries[i], 0, 0, id, 0, i + 1);
                    if (i >= 1)
                        m_CommandBuffer.ReleaseTemporaryRT(m_Temporaries[i - 1]);
                }
                m_CommandBuffer.ReleaseTemporaryRT(m_Temporaries[m_LODCount - 1]);
                camera.AddCommandBuffer(m_CameraEvent, m_CommandBuffer);
            }
        }
    }
}
