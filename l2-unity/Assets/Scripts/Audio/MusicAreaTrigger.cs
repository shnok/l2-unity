using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicAreaTrigger : MonoBehaviour
{
    [SerializeField] private int _areaPrority;
    [SerializeField] private EventReference _enterEvent;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Enter area:" + transform.name);
        MusicManager.Instance.PlayMusic(_enterEvent, _areaPrority);
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("Exit area:" + transform.name);
        MusicManager.Instance.ResetPriority();
        MusicManager.Instance.StopMusic(_enterEvent);
    }

    private void OnTriggerStay(Collider other) {
        if(MusicManager.Instance.CurrentEventPriority > _areaPrority) {
            MusicManager.Instance.StopMusic(_enterEvent);
        } else {
            MusicManager.Instance.PlayMusic(_enterEvent, _areaPrority);
        }
    }
}
