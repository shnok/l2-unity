using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonListener : MonoBehaviour
{
    [SerializeField]
    private GameObject player, cam;

    [SerializeField]
    private int listener;

    FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();

    private static ThirdPersonListener instance;
    public static ThirdPersonListener GetInstance() {
        return instance;
    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        }

        cam = Camera.main.gameObject;
    }

    void Update() {
        if(player == null) {
            if(PlayerController.GetInstance() != null) {
                player = PlayerController.GetInstance().gameObject;
            }
        } else {
            attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(player.transform.position);
        }
        
        attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(cam.transform.forward);
        attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(cam.transform.up);
        FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(listener, attributes);
    }
}

