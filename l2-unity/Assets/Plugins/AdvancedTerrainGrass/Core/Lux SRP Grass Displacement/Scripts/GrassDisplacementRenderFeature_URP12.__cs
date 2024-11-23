using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Lux_SRP_GrassDisplacement
{
    public class GrassDisplacementRenderFeature : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {
        
        [System.Serializable]
        public enum RTDisplacementSize {
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024
        }

        [System.Serializable]
        public class GrassDisplacementSettings
        {
            public RTDisplacementSize Resolution = RTDisplacementSize._256;
            public float Size = 20.0f;
            public bool ShiftRenderTex = false;
            //public bool SinglePassInstancing = false;
        }

        public GrassDisplacementSettings settings = new GrassDisplacementSettings();
        GrassDisplacementPass m_GrassDisplacementPass;
        
        public override void Create()
        {
            m_GrassDisplacementPass = new GrassDisplacementPass();
            m_GrassDisplacementPass.renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingShadows;

        //  Apply settings
            m_GrassDisplacementPass.m_Resolution = (int)settings.Resolution;
            m_GrassDisplacementPass.m_Size = settings.Size;
            m_GrassDisplacementPass.m_ShiftRenderTex = settings.ShiftRenderTex;
            //m_GrassDisplacementPass.m_SinglePassInstancing = settings.SinglePassInstancing;

        }
        
        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            renderer.EnqueuePass(m_GrassDisplacementPass);
        }
    }




//  ---------------------------------------------------------
//  The Pass

    public class GrassDisplacementPass : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {
        
        private const string ProfilerTag = "Render Lux Grass Displacement FX";
        private static ProfilingSampler m_ProfilingSampler = new ProfilingSampler(ProfilerTag);

        ShaderTagId m_GrassDisplacementFXShaderTag = new ShaderTagId("LuxGrassDisplacementFX");

        private SinglePassStereoMode m_StereoRenderingMode;

    //  There is no 0.5 in 8bit colors...
        Color m_ClearColor = new Color(127.0f/255.0f, 127.0f/255.0f,1,1);

    //  In case of XR enabled this most likely will be an array        
        UnityEngine.Rendering.Universal.RenderTargetHandle m_GrassDisplacementFX = UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget;
        //UnityEngine.Rendering.Universal.RenderTargetHandle m_GrassDisplacementFX = new UnityEngine.Rendering.Universal.RenderTargetHandle { id = -1 };

        private Matrix4x4 projectionMatrix;
        private Matrix4x4 worldToCameraMatrix;

        public float m_Size = 20.0f;
        public int m_Resolution = 256;
        public bool m_ShiftRenderTex = false;
        //public bool m_SinglePassInstancing = false;

        private float stepSize;
        private float oneOverStepSize;

        private Vector4 posSize = Vector4.zero;
        private static int DisplacementTexPosSizePID = Shader.PropertyToID("_Lux_DisplacementPosition");

        private FilteringSettings transparentFilterSettings { get; set; }

        public GrassDisplacementPass()
        {
            m_GrassDisplacementFX.Init("_Lux_DisplacementRT");
            transparentFilterSettings = new FilteringSettings(RenderQueueRange.transparent);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cameraTextureDescriptor.depthBufferBits = 0;
            cameraTextureDescriptor.width = m_Resolution;
            cameraTextureDescriptor.height = m_Resolution;
            cameraTextureDescriptor.colorFormat = RenderTextureFormat.Default;

        //  XR force our RT to be always 2D
            cameraTextureDescriptor.dimension = TextureDimension.Tex2D;

// Does not help for single pass instanced...
// cameraTextureDescriptor.vrUsage = VRTextureUsage.None;
// https://docs.unity3d.com/ScriptReference/Rendering.AttachmentDescriptor.ConfigureTarget.html

            cmd.GetTemporaryRT(m_GrassDisplacementFX.id, cameraTextureDescriptor, FilterMode.Bilinear);
            ConfigureTarget(m_GrassDisplacementFX.Identifier());
            ConfigureClear(ClearFlag.Color, m_ClearColor);

        //  Set up all constants
            stepSize = m_Size / (float)m_Resolution;
            oneOverStepSize = 1.0f / stepSize;
            var halfSize = m_Size  * 0.5f;
            projectionMatrix = Matrix4x4.Ortho(-halfSize, halfSize, -halfSize, halfSize, 0.1f, 80.0f);
            projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, false);
            worldToCameraMatrix.SetRow(0, new Vector4(1,0,0,0) ); //last is x pos
            worldToCameraMatrix.SetRow(1, new Vector4(0,0,1,0) ); //last is z pos
            worldToCameraMatrix.SetRow(2, new Vector4(0,1,0,0) ); //last is y pos
            worldToCameraMatrix.SetRow(3, new Vector4(0,0,0,1) );
        }

        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);

            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var drawSettings = CreateDrawingSettings(m_GrassDisplacementFXShaderTag, ref renderingData, SortingCriteria.CommonTransparent);
                var filteringSettings = transparentFilterSettings;

                var camera = renderingData.cameraData.camera;
                var cameraTransform = camera.transform;
                var cameraPos = cameraTransform.position;

#if ENABLE_VR && ENABLE_XR_MODULE
                var isStereoEnabled = renderingData.cameraData.xr.enabled; //isStereoEnabled; //
                if (isStereoEnabled) {
                    m_StereoRenderingMode = XRSettings.stereoRenderingMode;
                    cmd.SetSinglePassStereo(SinglePassStereoMode.None);
                }
#endif

            //  Push cameraPos forward – if enabled    
                var camForward = cameraTransform.forward;
                // unstable
                // cameraPos.x += camForward.x * m_Size * 0.5f;
                // cameraPos.z += camForward.z * m_Size * 0.5f;
                if (m_ShiftRenderTex) {
                    var t_camForward = new Vector2(camForward.x, camForward.z);
                    t_camForward.Normalize();
                //  still rather unstable...
                    cameraPos.x += t_camForward.x * m_Size * 0.33f;
                    cameraPos.z += t_camForward.y * m_Size * 0.33f;
                }
            
            //  Store original Camera matrices
                var worldToCameraMatrixOrig = camera.worldToCameraMatrix;
                var projectionMatrixOrig = camera.projectionMatrix;

            //  Quantize movement to fit texel size of RT – this stabilzes the final visual result
                Vector2 positionRT = Vector2.zero; // bad
                positionRT.x = Mathf.Floor(cameraPos.x * oneOverStepSize) * stepSize;
                positionRT.y = Mathf.Floor(cameraPos.z * oneOverStepSize) * stepSize;

            //  Update the custom worldToCameraMatrix – we only have to update the translation/position
                worldToCameraMatrix.SetColumn(3, new Vector4(-positionRT.x, -positionRT.y, -cameraPos.y - 40.0f, 1) );
                cmd.SetViewProjectionMatrices(worldToCameraMatrix, projectionMatrix);
            
            //  ---------------------------------------------------------
            //  Calc and set grass shader params
                posSize.x = positionRT.x - m_Size * 0.5f;
                posSize.y = positionRT.y - m_Size * 0.5f;
                posSize.z = 1.0f / m_Size ;
                cmd.SetGlobalVector(DisplacementTexPosSizePID, posSize );

            //  ---------------------------------------------------------
            //  Call execute
                context.ExecuteCommandBuffer(cmd);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);

            //  ---------------------------------------------------------
            //  Restore Camera matrices
                cmd.Clear();
                cmd.SetViewProjectionMatrices(worldToCameraMatrixOrig, projectionMatrixOrig);
#if ENABLE_VR && ENABLE_XR_MODULE
                if (isStereoEnabled) {
                    cmd.SetSinglePassStereo(m_StereoRenderingMode);
                }
#endif
            }
            
        //  ---------------------------------------------------------
        //  Call execute a 2nd time
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(m_GrassDisplacementFX.id);
        }
    }
}