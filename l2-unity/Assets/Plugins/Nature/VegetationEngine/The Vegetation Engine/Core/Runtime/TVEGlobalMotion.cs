// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;

namespace TheVegetationEngine
{
    [HelpURL("https://docs.google.com/document/d/145JOVlJ1tE-WODW45YoJ6Ixg23mFc56EnB_8Tbwloz8/edit#heading=h.czf8ud5bmaq2")]
    [ExecuteInEditMode]
    [AddComponentMenu("BOXOPHOBIC/The Vegetation Engine/TVE Global Motion")]
    public class TVEGlobalMotion : StyledMonoBehaviour
    {
        [StyledBanner(0.890f, 0.745f, 0.309f, "Global Motion")]
        public bool styledBanner;

        [StyledCategory("Direction Settings", 5, 10)]
        public bool goCat;

        [Tooltip("Sets the main direction from a gameobject.")]
        public GameObject mainDirection;

        [StyledCategory("Wind Settings")]
        public bool windCat;

        [Tooltip("Controls the global wind power.")]
        [StyledRangeOptions("Wind Power", 0, 1, new string[] { "None", "Windy", "Strong" })]
        public float windPower = 0.5f;

        [StyledCategory("Motion Settings")]
        public bool motionCat;

        [Tooltip("Sets the texture used for wind gust and motion highlight.")]
        public Texture2D noiseTexture;
        [Tooltip("Controls the global Noise Texture scale.")]
        public float noiseTilling = 1.0f;

        [Space(10)]
        [Tooltip("Controls the global Motion Bending amplitude.")]
        [Range(0, 2)]
        public float motionBending = 1.0f;
        [Tooltip("Controls the global Motion Branch amplitude.")]
        [Range(0, 2)]
        public float motionBranch = 1.0f;
        [Tooltip("Controls the global Motion Flutter amplitude.")]
        [Range(0, 2)]
        public float motionFlutter = 1.0f;

        [StyledCategory("Fade Settings")]
        public bool fadeCat;

        [Tooltip("Controls the Flutter Motion fade out distance in world units.")]
        public float motionFadeDistance = 100.0f;

        //[StyledSpace(5)]
        //public bool styledSpace0;

        void Start()
        {

#if UNITY_EDITOR
            gameObject.GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
            gameObject.GetComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
#endif

            // Disable Arrow in play mode
            if (Application.isPlaying == true)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }

            gameObject.name = "Global Motion";
            gameObject.transform.SetSiblingIndex(0);

            if (noiseTexture == null)
            {
                noiseTexture = Resources.Load<Texture2D>("Internal NoiseTex");
            }

            SetGlobalShaderProperties();
        }

        void Update()
        {
            if (mainDirection == null)
            {
                mainDirection = gameObject;
            }

            gameObject.transform.eulerAngles = new Vector3(0, mainDirection.transform.eulerAngles.y, 0);

            SetGlobalShaderProperties();
        }

        void SetGlobalShaderProperties()
        {
            var windDirection = transform.forward;
            Shader.SetGlobalVector("TVE_MotionParams", new Vector4(windDirection.x * 0.5f + 0.5f, windDirection.z * 0.5f + 0.5f, windPower, 0.0f));

            Shader.SetGlobalTexture("TVE_NoiseTex", noiseTexture);
            Shader.SetGlobalFloat("TVE_NoiseTexTilling", noiseTilling);

            Shader.SetGlobalFloat("TVE_MotionValue_10", motionBending);
            Shader.SetGlobalFloat("TVE_MotionValue_20", motionBranch);
            Shader.SetGlobalFloat("TVE_MotionValue_30", motionFlutter);

            Shader.SetGlobalFloat("TVE_MotionFadeStart", (motionFadeDistance + 0.01f) * 0.5f);
            Shader.SetGlobalFloat("TVE_MotionFadeEnd", motionFadeDistance + 0.01f);

            // Legacy
            Shader.SetGlobalVector("TVE_NoiseParams", Vector4.one);
            Shader.SetGlobalVector("TVE_FlutterParams", Vector4.one);
            Shader.SetGlobalFloat("TVE_MotionSpeed", 1);
        }
    }
}
