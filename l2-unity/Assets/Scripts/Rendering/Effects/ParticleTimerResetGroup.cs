using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleTimerResetGroup : MonoBehaviour
{
        [SerializeField] private Transform _owner;
        [SerializeField] private Vector3 _surfaceNormal;
        [SerializeField] private PooledEffect _pooledEffect; //TODO: Set values in prefab to save performances
        [SerializeField] private Renderer[] _particles;

        public PooledEffect PooledEffect { get { return _pooledEffect; } }

        public bool playOnEnable = false;

        void Start()
        {
                // if (playOnEnable)
                // {
                //         ResetTimer();
                // }
        }

        void OnEnable()
        {
                // if (playOnEnable)
                // {
                ResetTimer();
                // }
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
                        _particles = GetComponentsInChildren<Renderer>(); //TODO: Set renderer in prefab to save performances
                }

                for (int i = 0; i < _particles.Length; i++)
                {
                        if (_owner != null)
                        {
                                _particles[i].material.SetVector("_OwnerPosition", _owner.position);
                        }
                        _particles[i].material.SetFloat("_StartTime", time);
                        float seed = Random.Range(-100f, 100f);
                        _particles[i].material.SetFloat("_Seed", seed);
                        _particles[i].material.SetVector("_SurfaceNormals", _surfaceNormal);
                }
        }
}
