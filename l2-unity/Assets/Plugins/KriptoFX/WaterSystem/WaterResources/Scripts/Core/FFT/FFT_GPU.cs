using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KWS
{
    public class FFT_GPU : MonoBehaviour
    {
        public enum SizeSetting
        {
            Size_32 = 32,
            Size_64 = 64,
            Size_128 = 128,
            Size_256 = 256,
            Size_512 = 512,
        }


        public RenderTexture DisplaceTexture;
        public RenderTexture NormalTexture;
        //public RenderTexture LeanTexture;

        RenderTexture spectrumInitRenderTexture;
        RenderTexture spectrumHeight;
        RenderTexture spectrumDisplaceX;
        RenderTexture spectrumDisplaceZ;
        RenderTexture fftTemp1;
        RenderTexture fftTemp2;
        RenderTexture fftTemp3;

        Material normalComputeMaterial;


        ComputeShader spectrumShader;
        ComputeShader shaderFFT;


        Texture2D texButterfly;

        float prevWindTurbulence;
        float prevWindSpeed;
        private float prevWindRotation;
        float currentHeightDataDomainScale;

        int kernelSpectrumInit;
        int kernelSpectrumUpdate;
        Color[] butterflyColors;




        private bool isInitialized = false;

        public void Release()
        {
            KW_Extensions.SafeDestroy(spectrumShader, shaderFFT, texButterfly, normalComputeMaterial);
            KWS_CoreUtils.ReleaseRenderTextures(spectrumInitRenderTexture, spectrumHeight, spectrumDisplaceX, spectrumDisplaceZ, fftTemp1, fftTemp2, fftTemp3, DisplaceTexture, NormalTexture);

            prevWindTurbulence = -1;
            prevWindSpeed = -1;
            prevWindRotation = -1;
            isInitialized = false;
        }

        void InitializeResources(int size, int anisoLevel)
        {
            Release();

            spectrumShader = Object.Instantiate(Resources.Load<ComputeShader>("Common/FFT/Spectrum_GPU"));
            kernelSpectrumInit = spectrumShader.FindKernel("SpectrumInitalize");
            kernelSpectrumUpdate = spectrumShader.FindKernel("SpectrumUpdate");
            shaderFFT = Object.Instantiate(Resources.Load<ComputeShader>("Common/FFT/ComputeFFT_GPU"));
            normalComputeMaterial = new Material(Shader.Find("KriptoFX/Water/ComputeNormal"));


            texButterfly = new Texture2D(size, Mathf.RoundToInt(Mathf.Log(size, 2)), TextureFormat.RGBAFloat, false, true);
            spectrumInitRenderTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            spectrumInitRenderTexture.enableRandomWrite = true;
            spectrumInitRenderTexture.Create();

            spectrumHeight = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
            spectrumHeight.enableRandomWrite = true;
            spectrumHeight.Create();

            spectrumDisplaceX = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
            spectrumDisplaceX.enableRandomWrite = true;
            spectrumDisplaceX.Create();

            spectrumDisplaceZ = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
            spectrumDisplaceZ.enableRandomWrite = true;
            spectrumDisplaceZ.Create();

            fftTemp1 = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat);
            fftTemp1.enableRandomWrite = true;
            fftTemp1.Create();

            fftTemp2 = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat);
            fftTemp2.enableRandomWrite = true;
            fftTemp2.Create();

            fftTemp3 = new RenderTexture(size, size, 0, RenderTextureFormat.RGFloat);
            fftTemp3.enableRandomWrite = true;
            fftTemp3.Create();

            DisplaceTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            DisplaceTexture.enableRandomWrite = true;
            DisplaceTexture.Create();

            NormalTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            NormalTexture.filterMode = FilterMode.Trilinear;
            NormalTexture.wrapMode = TextureWrapMode.Repeat;
            NormalTexture.useMipMap = true;
            NormalTexture.anisoLevel = anisoLevel;

            InitializeButterfly(size);
            isInitialized = true;
        }

        void OnDisable()
        {
            Release();
            //print("FFT.Disable");
        }

        public void ComputeFFT(LodPrefix lodPrefix, int size, int anisoLevel,
            float domainSize, float windTurbulence, float windSpeed, float windRotation, float timeOffset, List<Material> waterSharedMaterials)
        {
            if (!isInitialized || DisplaceTexture.width != size || NormalTexture.anisoLevel != anisoLevel) InitializeResources(size, anisoLevel);

            if (Mathf.Abs(prevWindTurbulence - windTurbulence) > 0.001f || Mathf.Abs(prevWindSpeed - windSpeed) > 0.01f || Mathf.Abs(prevWindRotation - windRotation) > 0.01f)
            {
                prevWindTurbulence = windTurbulence;
                prevWindSpeed = windSpeed;
                prevWindRotation = windRotation;
                InitializeSpectrum(size, domainSize, windSpeed, windTurbulence, windRotation);
            }

            UpdateSpectrum(size, timeOffset, windRotation);
            DispatchFFT(size, domainSize, windSpeed);

            var normalLodSize = Mathf.RoundToInt(Mathf.Log(size, 2)) - 4;
            var mipCount = Mathf.RoundToInt(Mathf.Log(size, 2));

            foreach (var mat in waterSharedMaterials)
            {
                if (mat == null) continue;
                UpdateMaterialParameters(lodPrefix, normalLodSize, domainSize, mipCount, mat);
            }
        }

        private int ID_DispTex = Shader.PropertyToID("KW_DispTex");
        private int ID_DispTex1 = Shader.PropertyToID("KW_DispTex_LOD1");
        private int ID_DispTex2 = Shader.PropertyToID("KW_DispTex_LOD2");


        private int ID_NormTex = Shader.PropertyToID("KW_NormTex");
        private int ID_LeanTex = Shader.PropertyToID("KW_LeanTex");
        private int ID_NormTex1 = Shader.PropertyToID("KW_NormTex_LOD1");
        private int ID_NormTex2 = Shader.PropertyToID("KW_NormTex_LOD2");

        private int ID_MipCount = Shader.PropertyToID("KW_NormMipCount");
        private int ID_MipCount1 = Shader.PropertyToID("KW_NormMipCount_LOD1");
        private int ID_MipCount2 = Shader.PropertyToID("KW_NormMipCount_LOD2");

        private int ID_FFTDomainSize = Shader.PropertyToID("KW_FFTDomainSize");
        private int ID_FFTDomainSize1 = Shader.PropertyToID("KW_FFTDomainSize_LOD1");
        private int ID_FFTDomainSize2 = Shader.PropertyToID("KW_FFTDomainSize_LOD2");

        // private int ID_WindSpeed = Shader.PropertyToID("KW_WindSpeed");
        private int ID_NormalLod = Shader.PropertyToID("KW_NormalLod");


        public enum LodPrefix
        {
            LOD0,
            LOD1,
            LOD2,
        }

        void UpdateMaterialParameters(LodPrefix lodPrefix, float normalLodSize, float domainSize, float mipCount, Material material)
        {
            switch (lodPrefix)
            {
                case LodPrefix.LOD0:
                    material.SetTexture(ID_DispTex, DisplaceTexture);
                    material.SetTexture(ID_NormTex, NormalTexture);
                    material.SetFloat(ID_FFTDomainSize, domainSize);
                    material.SetFloat(ID_NormalLod, normalLodSize);
                    material.SetFloat(ID_MipCount, mipCount);
                    break;
                case LodPrefix.LOD1:
                    material.SetTexture(ID_DispTex1, DisplaceTexture);
                    material.SetTexture(ID_NormTex1, NormalTexture);
                    material.SetFloat(ID_FFTDomainSize1, domainSize);
                    material.SetFloat(ID_MipCount1, mipCount);
                    break;
                case LodPrefix.LOD2:
                    material.SetTexture(ID_DispTex2, DisplaceTexture);
                    material.SetTexture(ID_NormTex2, NormalTexture);
                    material.SetFloat(ID_FFTDomainSize2, domainSize);
                    material.SetFloat(ID_MipCount2, mipCount);
                    break;

            }
        }

        void UpdateComputeParameters(LodPrefix lodPrefix, float normalLodSize, float domainSize, float mipCount, ComputeShader computeShader, List<int> kernels)
        {
            foreach (int kernel in kernels)
            {
                switch (lodPrefix)
                {

                    case LodPrefix.LOD0:
                        computeShader.SetTexture(kernel, ID_DispTex, DisplaceTexture);
                        computeShader.SetTexture(kernel, ID_NormTex, NormalTexture);
                        computeShader.SetFloat(ID_FFTDomainSize, domainSize);
                        computeShader.SetFloat(ID_NormalLod, normalLodSize);
                        computeShader.SetFloat(ID_MipCount, mipCount);
                        break;
                    case LodPrefix.LOD1:
                        computeShader.SetTexture(kernel, ID_DispTex1, DisplaceTexture);
                        computeShader.SetTexture(kernel, ID_NormTex1, NormalTexture);
                        computeShader.SetFloat(ID_FFTDomainSize1, domainSize);
                        computeShader.SetFloat(ID_MipCount1, mipCount);
                        break;
                    case LodPrefix.LOD2:
                        computeShader.SetTexture(kernel, ID_DispTex2, DisplaceTexture);
                        computeShader.SetTexture(kernel, ID_NormTex2, NormalTexture);
                        computeShader.SetFloat(ID_FFTDomainSize2, domainSize);
                        computeShader.SetFloat(ID_MipCount2, mipCount);
                        break;

                }
            }
        }

        void InitializeButterfly(int size)
        {
            var log2Size = Mathf.RoundToInt(Mathf.Log(size, 2));
            butterflyColors = new Color[size * log2Size];

            int offset = 1, numIterations = size >> 1;
            for (int rowIndex = 0; rowIndex < log2Size; rowIndex++)
            {
                int rowOffset = rowIndex * size;
                {
                    int start = 0, end = 2 * offset;
                    for (int iteration = 0; iteration < numIterations; iteration++)
                    {
                        var bigK = 0.0f;
                        for (int K = start; K < end; K += 2)
                        {
                            var phase = 2.0f * Mathf.PI * bigK * numIterations / size;
                            var cos = Mathf.Cos(phase);
                            var sin = Mathf.Sin(phase);
                            butterflyColors[rowOffset + K / 2] = new Color(cos, -sin, 0, 1);
                            butterflyColors[rowOffset + K / 2 + offset] = new Color(-cos, sin, 0, 1);

                            bigK += 1.0f;
                        }
                        start += 4 * offset;
                        end = start + 2 * offset;
                    }
                }
                numIterations >>= 1;
                offset <<= 1;
            }

            texButterfly.SetPixels(butterflyColors);
            texButterfly.Apply();
        }

        void InitializeSpectrum(int size, float domainSize, float windSpeed, float windTurbulence, float windRotation)
        {
            spectrumShader.SetInt("size", size);

            spectrumShader.SetFloat("domainSize", domainSize);
            spectrumShader.SetFloats("turbulence", windTurbulence);
            spectrumShader.SetFloat("windSpeed", windSpeed);
            spectrumShader.SetFloat("windRotation", windRotation);
            spectrumShader.SetTexture(kernelSpectrumInit, "resultInit", spectrumInitRenderTexture);
            spectrumShader.Dispatch(kernelSpectrumInit, size / 8, size / 8, 1);
        }

        void UpdateSpectrum(int size, float timeOffset, float windRotation)
        {
            spectrumShader.SetFloat("time", timeOffset);
            spectrumShader.SetTexture(kernelSpectrumUpdate, "init0", spectrumInitRenderTexture);
            spectrumShader.SetTexture(kernelSpectrumUpdate, "resultHeight", spectrumHeight);
            spectrumShader.SetTexture(kernelSpectrumUpdate, "resultDisplaceX", spectrumDisplaceX);
            spectrumShader.SetTexture(kernelSpectrumUpdate, "resultDisplaceZ", spectrumDisplaceZ);

            spectrumShader.Dispatch(kernelSpectrumUpdate, size / 8, size / 8, 1);
        }

        void DispatchFFT(int size, float domainSize, float windSpeed)
        {
            var temp = RenderTexture.active;

            var kernelOffset = 0;
            switch (size)
            {
                case (int)SizeSetting.Size_32:
                    kernelOffset = 0;
                    break;
                case (int)SizeSetting.Size_64:
                    kernelOffset = 2;
                    break;
                case (int)SizeSetting.Size_128:
                    kernelOffset = 4;
                    break;
                case (int)SizeSetting.Size_256:
                    kernelOffset = 6;
                    break;
                case (int)SizeSetting.Size_512:
                    kernelOffset = 8;
                    break;
            }

            shaderFFT.SetTexture(kernelOffset, "inputH", spectrumHeight);
            shaderFFT.SetTexture(kernelOffset, "inputX", spectrumDisplaceX);
            shaderFFT.SetTexture(kernelOffset, "inputZ", spectrumDisplaceZ);
            shaderFFT.SetTexture(kernelOffset, "inputButterfly", texButterfly);
            shaderFFT.SetTexture(kernelOffset, "output1", fftTemp1);
            shaderFFT.SetTexture(kernelOffset, "output2", fftTemp2);
            shaderFFT.SetTexture(kernelOffset, "output3", fftTemp3);
            shaderFFT.Dispatch(kernelOffset, 1, size, 1);

            shaderFFT.SetTexture(kernelOffset + 1, "inputH", fftTemp1);
            shaderFFT.SetTexture(kernelOffset + 1, "inputX", fftTemp2);
            shaderFFT.SetTexture(kernelOffset + 1, "inputZ", fftTemp3);
            shaderFFT.SetTexture(kernelOffset + 1, "inputButterfly", texButterfly);
            shaderFFT.SetTexture(kernelOffset + 1, "output", DisplaceTexture);
            shaderFFT.Dispatch(kernelOffset + 1, size, 1, 1);


            normalComputeMaterial.SetFloat("KW_FFTDomainSize", domainSize);
            normalComputeMaterial.SetTexture("_DispTex", DisplaceTexture);
            var sizeLog = Mathf.RoundToInt(Mathf.Log(size, 2)) - 4;
            normalComputeMaterial.SetFloat("_SizeLog", sizeLog);
            normalComputeMaterial.SetFloat("_WindSpeed", windSpeed);


            //if (!Application.isPlaying ) RenderTexture.active = null;
            Graphics.SetRenderTarget(NormalTexture);
            Graphics.Blit(null, NormalTexture, normalComputeMaterial, 0);
            //Graphics.SetRenderTarget(LeanTexture);
            //normalComputeMaterial.SetTexture("_NormalTex", NormalTexture);
            //Graphics.Blit(NormalTexture, LeanTexture, normalComputeMaterial, 1);
            RenderTexture.active = temp;
            //if (!Application.isPlaying)

        }

    }
}

