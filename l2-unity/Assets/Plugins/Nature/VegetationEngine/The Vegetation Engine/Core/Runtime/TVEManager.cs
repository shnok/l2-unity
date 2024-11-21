// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;

namespace TheVegetationEngine
{
    [HelpURL("https://docs.google.com/document/d/145JOVlJ1tE-WODW45YoJ6Ixg23mFc56EnB_8Tbwloz8/edit#heading=h.hbq3w8ae720x")]
    [ExecuteInEditMode]
    [AddComponentMenu("BOXOPHOBIC/The Vegetation Engine/TVE Manager")]
    public class TVEManager : StyledMonoBehaviour
    {
        public static TVEManager Instance;

        [StyledBanner(0.890f, 0.745f, 0.309f, "The Vegetation Engine")]
        public bool styledBanner;

        [HideInInspector]
        public TVEGlobalMotion globalMotion;
        [HideInInspector]
        public TVEGlobalDetails globalDetails;
        [HideInInspector]
        public TVEGlobalControl globalControl;
        [HideInInspector]
        public TVEGlobalVolume globalVolume;

        void Awake()
        {
            CreateComponents();
        }

        void OnEnable()
        {
            EnableManager();
        }

        void OnDisable()
        {
            DisableManager();
        }

        void OnDestroy()
        {
            DisableManager();
        }

        void Update()
        {
            Shader.SetGlobalFloat("TVE_IsEnabled", 1);
            Shader.SetGlobalFloat("TVE_IsManager", 1);

            if (Application.isPlaying)
            {
                Shader.SetGlobalFloat("TVE_IsPlaying", 1);
            }
            else
            {
                Shader.SetGlobalFloat("TVE_IsPlaying", 0);
            }
        }

        void CreateComponents()
        {
            if (globalMotion == null)
            {
                GameObject go = new GameObject();

                go.AddComponent<MeshFilter>();
                go.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("Internal Mesh Arrow");

                go.AddComponent<MeshRenderer>();
                go.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Internal Arrow");

                go.AddComponent<TVEGlobalMotion>();

                SetParent(go);

                go.transform.localPosition = new Vector3(0, 2f, 0);

                globalMotion = go.GetComponent<TVEGlobalMotion>();
                globalMotion.name = "Global Motion";
            }

            if (globalDetails == null)
            {
                GameObject go = new GameObject();
                go.AddComponent<TVEGlobalDetails>();
                SetParent(go);

                globalDetails = go.GetComponent<TVEGlobalDetails>();
                globalDetails.name = "Global Details";
            }

            if (globalControl == null)
            {
                GameObject go = new GameObject();
                go.AddComponent<TVEGlobalControl>();
                SetParent(go);

                globalControl = go.GetComponent<TVEGlobalControl>();
                globalControl.name = "Global Control";
            }

            if (globalVolume == null)
            {
                GameObject go = new GameObject();
                go.AddComponent<TVEGlobalVolume>();
                SetParent(go);

                go.transform.localScale = new Vector3(400, 200, 400);

                globalVolume = go.GetComponent<TVEGlobalVolume>();
                globalVolume.name = "Global Volume";
            }
        }

        void SetParent(GameObject go)
        {
            go.transform.parent = gameObject.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.eulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
        }

        void EnableManager()
        {
            if (TVEManager.Instance != null && TVEManager.Instance != this)
            {
                TVEManager.Instance.gameObject.SetActive(false);
            }

            Instance = this;

            globalVolume.InitVolumeRendering();

            Instance.name = "The Vegetation Engine";
        }

        void DisableManager()
        {
            Instance = null;

            globalVolume.DisableVolumeRendering();

            Shader.SetGlobalFloat("TVE_IsEnabled", 0);
            Shader.SetGlobalFloat("TVE_IsManager", 0);
        }
    }
}