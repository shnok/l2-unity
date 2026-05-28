#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class LightLOD : MonoBehaviour
{
    private Light _light;

    [SerializeField]
    [Range(0, 15)]
    private float _updateDelay = 1f;
    [SerializeField] private float _squareDistanceFromCamera;
    [SerializeField] private float _lastUpdate;
    private Camera _mainCamera;

    [SerializeField]
    private List<LODAdjustment> LODLevels = new();

    private bool _ready = false;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void Start()
    {
        _ready = false;
        _lastUpdate = 0;
        _mainCamera = CameraController.Instance.GetComponent<Camera>();
    }


    private void Update()
    {
        if (_light == null)
        {
            _light = GetComponent<Light>();
            if (_light == null)
            {
                return;
            }
        }

#if (UNITY_EDITOR) 
        if (!EditorApplication.isPlaying)
        {
            _ready = true;
        }
#endif

        if (!_ready)
        {
            if (CameraController.Instance == null || CameraController.Instance.Target == null)
            {
                return;
            }

            if (CameraController.Instance.CurrentDistance > CameraController.Instance.MaxDistance)
            {
                return;
            }

            _ready = true;
        }

        if (Time.time - _lastUpdate >= _updateDelay)
        {
            _lastUpdate = Time.time;
#if (UNITY_EDITOR)
            if (EditorApplication.isPlaying)
            {
                AdjustLODQuality(_mainCamera);
            }
            else
            {
                if (Camera.current != null)
                {
                    AdjustLODQuality(Camera.current);
                }
            }
#else
            AdjustLODQuality(_mainCamera);
#endif
        }
    }


    private void AdjustLODQuality(Camera camera)
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
                _light.enabled = true;
                _light.shadows = LODLevels[i].LightShadows;
                if (QualitySettings.shadowResolution <= LODLevels[i].ShadowResolution)
                {
                    _light.shadowResolution = (LightShadowResolution)QualitySettings.shadowResolution;
                }
                else
                {
                    _light.shadowResolution = (LightShadowResolution)LODLevels[i].ShadowResolution;
                }

                return;
            }
        }

        _light.enabled = false;
    }
}