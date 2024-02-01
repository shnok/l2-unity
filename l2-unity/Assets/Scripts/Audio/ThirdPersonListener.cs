using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonListener : MonoBehaviour
{
    [SerializeField] private GameObject _player, _cam;
    [SerializeField] private int _listener;

    private FMOD.ATTRIBUTES_3D _attributes = new FMOD.ATTRIBUTES_3D();

    private static ThirdPersonListener _instance;
    public static ThirdPersonListener Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        _cam = Camera.main.gameObject;
    }

    void Update() {
        if(_player == null) {
            if(PlayerController.GetInstance() != null) {
                _player = PlayerController.GetInstance().gameObject;
            }
        } else {
            _attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(_player.transform.position);
        }
        
        _attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(_cam.transform.forward);
        _attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(_cam.transform.up);
        FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(_listener, _attributes);
    }
}

