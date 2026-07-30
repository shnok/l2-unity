using UnityEngine;

namespace ProjectCube.UI
{
    [ExecuteAlways]
    public sealed class WorldSpaceUIScaler : MonoBehaviour
    {
        private static readonly int ReferenceScreenSizeId = Shader.PropertyToID("_ReferenceScreenSize");
        private static readonly int UnityScreenSizeId = Shader.PropertyToID("_UnityScreenSize");

        [SerializeField] private float _referenceHeight = 600.0f;
        [SerializeField] private int _lastWidth;
        [SerializeField] private int _lastHeight;

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            int width = Screen.width;
            int height = Screen.height;

            if (width != _lastWidth || height != _lastHeight)
            {
                Apply();
            }
        }

        private void Apply()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;

            Shader.SetGlobalFloat(ReferenceScreenSizeId, _referenceHeight);
            Shader.SetGlobalFloat(UnityScreenSizeId, Screen.height);
        }
    }
}