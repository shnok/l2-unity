using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleTimerResetGroup : MonoBehaviour
{
        private Renderer[] _particles;

        void Start()
        {
                ResetTimer();
        }

        void OnEnable()
        {
                Debug.Log("PrintOnEnable: script was enabled");
                ResetTimer();
        }

        public void ResetTimer()
        {
#if UNITY_EDITOR
                float time = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
#else
        float time = Time.time;
#endif

                if (_particles == null || _particles.Length == 0)
                {
                        _particles = GetComponentsInChildren<Renderer>();
                }

                for (int i = 0; i < _particles.Length; i++)
                {
                        _particles[i].material.SetFloat("_StartTime", time);
                }
        }
}
