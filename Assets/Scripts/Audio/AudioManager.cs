using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private EventReference UI_default_btn_down;
    [SerializeField] private EventReference UI_default_btn_up;

    public static AudioManager instance;

    public static AudioManager GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }


    void Start()
    {
        SetReferences();
    }

    void SetReferences() {
        UI_default_btn_down = RuntimeManager.PathToEventReference("event:/SFX/UI/UI_default_btn_down");
        UI_default_btn_up = RuntimeManager.PathToEventReference("event:/SFX/UI/UI_default_btn_up");
    }


    void Update()
    {
        
    }

    public void PlaySound3D(EventReference sound, Vector3 postition) {
        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlaySound2D(EventReference sound) {
        RuntimeManager.PlayOneShot(sound);
    }
}
