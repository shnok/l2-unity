using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParticleTimerReset : MonoBehaviour
{
    void Start() {
#if UNITY_EDITOR
        float time = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
#else
        float time = Time.time;
#endif

        GetComponent<Renderer>().material.SetFloat("_StartTime", time);
    }
}
