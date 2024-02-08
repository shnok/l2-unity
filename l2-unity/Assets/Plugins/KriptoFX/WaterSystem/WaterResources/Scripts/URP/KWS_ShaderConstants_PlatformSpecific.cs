using UnityEngine;

namespace KWS
{
    public static class KWS_ShaderConstants_PlatformSpecific
    {

        public static class CopyColorID
        {
            public static readonly int _CameraOpaqueTexture = Shader.PropertyToID("_CameraOpaqueTexture");
            public static readonly int _CameraOpaqueTexture_RTHandleScale = Shader.PropertyToID("_CameraOpaqueTexture_RTHandleScale");
        }
    }
}

