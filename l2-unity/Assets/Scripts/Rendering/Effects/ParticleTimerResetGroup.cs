using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleTimerResetGroup : MonoBehaviour
{
        public Transform owner;
        public Vector3 surfaceNormal;

        [SerializeField] private Renderer[] _particles;

        void Start()
        {
                ResetTimer();
        }

        void OnEnable()
        {
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
                        if (owner != null)
                        {
                                _particles[i].material.SetVector("_OwnerPosition", owner.position);
                        }
                        _particles[i].material.SetFloat("_StartTime", time);
                        float seed = Random.Range(-100f, 100f);
                        _particles[i].material.SetFloat("_Seed", seed);
                        _particles[i].material.SetVector("_SurfaceNormals", surfaceNormal);
                }
        }
}
