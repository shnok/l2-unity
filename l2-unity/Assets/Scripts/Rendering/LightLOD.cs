#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;

namespace LlamAcademy.LightLOD
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Light))]
    public class LightLOD : MonoBehaviour
    {
        private Light Light;
        public bool LightShouldBeOn = true;
        [SerializeField]
        [Range(0, 15)]
        private float UpdateDelay = 1f;
        [SerializeField] private float _squareDistanceFromCamera;
        [SerializeField] private float _lastUpdate;
        [SerializeField]
        private List<LODAdjustment> LODLevels = new();

        private void Awake()
        {
            _lastUpdate = 0;
            Light = GetComponent<Light>();
        }


        private void Update()
        {
            if (Light == null)
            {
                Light = GetComponent<Light>();
                if (Light == null)
                {
                    return;
                }
            }

            if (Time.time - _lastUpdate >= UpdateDelay)
            {
                Debug.LogWarning(Time.time - _lastUpdate);
                _lastUpdate = Time.time;
#if (UNITY_EDITOR)
                if (EditorApplication.isPlaying)
                {
                    AdjustLODQuality(Camera.main);
                }
                else
                {
                    if (Camera.current != null)
                    {
                        AdjustLODQuality(Camera.current);
                    }
                }
#else
                AdjustLODQuality(Camera.main);
#endif
            }
        }


        private void AdjustLODQuality(Camera camera)
        {
            if (LightShouldBeOn)
            {
                _squareDistanceFromCamera = Vector3.SqrMagnitude(
                    camera.transform.position - transform.position
                );

                for (int i = 0; i < LODLevels.Count; i++)
                {
                    if (_squareDistanceFromCamera > LODLevels[i].MinSquareDistance
                        && _squareDistanceFromCamera <= LODLevels[i].MaxSquareDistance
                    )
                    {
                        Light.enabled = true;
                        Light.shadows = LODLevels[i].LightShadows;
                        if (QualitySettings.shadowResolution <= LODLevels[i].ShadowResolution)
                        {
                            Light.shadowResolution = (LightShadowResolution)QualitySettings.shadowResolution;
                        }
                        else
                        {
                            Light.shadowResolution = (LightShadowResolution)LODLevels[i].ShadowResolution;
                        }

                        return;
                    }
                }

                Light.enabled = false;
            }
            else
            {
                Light.enabled = false;
            }

        }
    }
}