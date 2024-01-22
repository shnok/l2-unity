using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicAreaTrigger : MonoBehaviour
{
    public int areaPrority;
    public EventReference enterEvent;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Enter area:" + transform.name);
        MusicManager.GetInstance().PlayMusic(enterEvent, areaPrority);
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("Exit area:" + transform.name);
        MusicManager.GetInstance().ResetPriority();
        MusicManager.GetInstance().StopMusic(enterEvent);
    }

    private void OnTriggerStay(Collider other) {
        if(MusicManager.GetInstance().currentEventPriority > areaPrority) {
            MusicManager.GetInstance().StopMusic(enterEvent);
        } else {
            MusicManager.GetInstance().PlayMusic(enterEvent, areaPrority);
        }
    }
}
