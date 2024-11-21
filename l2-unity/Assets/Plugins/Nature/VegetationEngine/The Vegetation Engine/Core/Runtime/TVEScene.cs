// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;
using UnityEngine.Rendering;

namespace TheVegetationEngine
{
    [ExecuteInEditMode]
    [AddComponentMenu("BOXOPHOBIC/The Vegetation Engine/TVE Scene")]
    public class TVEScene : StyledMonoBehaviour
    {
        [StyledBanner(0.890f, 0.745f, 0.309f, "Scene")]
        public bool styledBanner;

        public GameObject setupStandard;
        public GameObject setupUniversal;
        public GameObject setupHD;

        [Space(10)]
        public Material skyboxMaterial;
        [Range(0, 8)]
        public float skyboxAmbient = 1;
        [Range(0, 1)]
        public float skyboxReflection = 1;

        [HideInInspector]
        public GameObject setupScene;

        [StyledSpace(5)]
        public bool styledSpace;

        void OnEnable()
        {
            if (setupScene != null)
            {
                DestroyImmediate(setupScene);
            }

            if (GraphicsSettings.defaultRenderPipeline != null)
            {
                if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("Universal"))
                {
                    setupScene = Instantiate(setupUniversal);
                }

                if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HD"))
                {
                    setupScene = Instantiate(setupHD);
                }
            }
            else
            {
                setupScene = Instantiate(setupStandard);
            }

            setupScene.name = setupScene.name.Replace("(Clone)", "");
            setupScene.transform.SetSiblingIndex(2);

            RenderSettings.skybox = skyboxMaterial;
            RenderSettings.ambientIntensity = skyboxAmbient;
            RenderSettings.reflectionIntensity = skyboxReflection;
        }
    }
}



