using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using static KWS.KWS_CoreUtils;
namespace KWS
{
    public class KW_PyramidBlur
    {
        private string BlurShaderName = "Hidden/KriptoFX/KWS/BlurGaussian";
        const int kMaxIterations = 8;
        public Material blurMaterial;


        TemporaryRenderTexture[] tempBuffers1 = new TemporaryRenderTexture[kMaxIterations];
        TemporaryRenderTexture[] tempBuffers2 = new TemporaryRenderTexture[kMaxIterations];



        public class BufferSize
        {
            public int Width;
            public int Height;
        }

        public void Release()
        {
            KW_Extensions.SafeDestroy(blurMaterial);
        }


        public void ComputeBlurPyramid(float blurRadius, TemporaryRenderTexture source, TemporaryRenderTexture target, CommandBuffer cmd)
        {
            if (blurMaterial == null) blurMaterial = CreateMaterial(BlurShaderName);
            GraphicsFormat targetFormat;
#if UNITY_2019_2_OR_NEWER
        targetFormat = source.descriptor.graphicsFormat;
#else
            targetFormat = GraphicsFormatUtility.GetGraphicsFormat(source.descriptor.colorFormat, false);
#endif

            TemporaryRenderTexture last = null;

            var logh = Mathf.Log(Mathf.Max(target.descriptor.width, target.descriptor.height), 2) + blurRadius - 8;
            var logh_i = (int)logh;
            var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

            blurMaterial.SetFloat("_SampleScale", 0.5f + logh - logh_i);
            cmd.SetGlobalVector("_BlurRTHandleScale", Vector4.one);
            var lastWidth = target.descriptor.width;
            var lastHeight = target.descriptor.height;

            for (var level = 0; level < iterations; level++)
            {
                if (tempBuffers1[level] == null) tempBuffers1[level] = new TemporaryRenderTexture();
                tempBuffers1[level].Alloc("_blurPassDown", lastWidth, lastHeight, 0, targetFormat);
                var downPassTarget = iterations == 1 ? target : tempBuffers1[level];
                cmd.Blit(level == 0 ? source.rt : last.rt, downPassTarget.rt, blurMaterial, 0);
                lastWidth = lastWidth / 2;
                lastHeight = lastHeight / 2;
                last = tempBuffers1[level];
            }

            for (var level = iterations - 2; level >= 0; level--)
            {
                if (tempBuffers2[level] == null) tempBuffers2[level] = new TemporaryRenderTexture();
                tempBuffers2[level].Alloc("_blurPassUp", tempBuffers1[level].rt.width, tempBuffers1[level].rt.height, 0, targetFormat);
                cmd.Blit(last.rt, level == 0 ? target.rt : tempBuffers2[level].rt, blurMaterial, 1);
                last = tempBuffers2[level];
            }
            foreach (var temp in tempBuffers1) if (temp != null) temp.Release();
            foreach (var temp in tempBuffers2) if (temp != null) temp.Release();
        }

        readonly int _BlurRTHandleScale = Shader.PropertyToID("_BlurRTHandleScale");
        private readonly int _SourceRT = Shader.PropertyToID("_SourceRT");
        private readonly int _SampleScale = Shader.PropertyToID("_SampleScale");

        public void ComputeBlurPyramid(float blurRadius, KWS_RTHandle source, KWS_RTHandle target, CommandBuffer cmd)
        {
            var tempBuffersDown = new RenderTexture[kMaxIterations];
            var tempBuffersUp = new RenderTexture[kMaxIterations];

            if (blurMaterial == null) blurMaterial = CreateMaterial(BlurShaderName);

            var targetRT = target.rt;
            RenderTexture last = null;

            int cw = source.rt.width;
            int ch = source.rt.height;
            int tw = target.rt.width;
            int th = target.rt.height;

            Debug.Assert(cw == tw && ch == th);

            //if (target.useScaling)
            //{

            //    cmd.SetViewport(new Rect(0.0f, 0.0f, scaledViewportSize.x, scaledViewportSize.y));
            //}
            var scaledViewportSize = source.GetScaledSize(source.rtHandleProperties.currentViewportSize);
            var logh = Mathf.Log(Mathf.Max(scaledViewportSize.x, scaledViewportSize.y), 2) + blurRadius - 8;
            var logh_i = (int)logh;
            var iterations = Mathf.Clamp(logh_i, 2, kMaxIterations);

            cmd.SetGlobalFloat(_SampleScale, 0.5f + logh - logh_i);

            var lastWidth = scaledViewportSize.x;
            var lastHeight = scaledViewportSize.y;

            for (var level = 0; level < iterations; level++)
            {
                tempBuffersDown[level] = RenderTexture.GetTemporary(lastWidth, lastHeight, targetRT.depth, targetRT.format);
                var downPassTarget = iterations == 1 ? target : tempBuffersDown[level];

                var sourceRTHandleScale = level == 0 ? source.rtHandleProperties.rtHandleScale : Vector4.one;
                cmd.BlitTriangle(level == 0 ? source.rt : last, sourceRTHandleScale, downPassTarget, blurMaterial);

                lastWidth = lastWidth / 2;
                lastHeight = lastHeight / 2;
                last = tempBuffersDown[level];
            }

            for (var level = iterations - 2; level >= 0; level--)
            {
                tempBuffersUp[level] = RenderTexture.GetTemporary(tempBuffersDown[level].width, tempBuffersDown[level].height, 0, targetRT.format);

                if (level != 0)
                {
                    cmd.BlitTriangle(last, tempBuffersUp[level], blurMaterial, 1);
                    last = tempBuffersUp[level];
                }
                else
                {
                    cmd.BlitTriangleRTHandle(last, Vector4.one, target, blurMaterial, KWS.ClearFlag.None, Color.clear, 1);
                }
            }
            foreach (var temp in tempBuffersDown) if (temp != null) RenderTexture.ReleaseTemporary(temp);
            foreach (var temp in tempBuffersUp) if (temp != null) RenderTexture.ReleaseTemporary(temp);
        }
    }
}