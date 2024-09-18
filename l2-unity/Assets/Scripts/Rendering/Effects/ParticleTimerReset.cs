using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class ParticleTimerReset : MonoBehaviour
{
        void Start()
        {
                ResetTimer();
        }

        void OnEnable()
        {
                Debug.Log("PrintOnEnable: script was enabled");
                ResetTimer();
        }

        private void ResetTimer()
        {
#if UNITY_EDITOR
                float time = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
#else
        float time = Time.time;
#endif

                GetComponent<Renderer>().material.SetFloat("_StartTime", time);
        }
}
