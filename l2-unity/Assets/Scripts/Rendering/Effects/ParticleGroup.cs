using UnityEngine;

// [ExecuteInEditMode]
public class ParticleGroup : MonoBehaviour
{
    [SerializeField] private L2Particle _owner;
    [SerializeField] private Renderer[] _particles;

    public Vector3 OwnerPosition { get; set; }
    public Vector3 SurfaceNormal { get; set; }
    public L2Particle Owner { get => _owner; set => _owner = value; }
    public int CountPerSecond { get => _countPerSecond; set => _countPerSecond = value; }
    public int MaxCount { get => _maxCount; set => _maxCount = value; }

    [Header("Spawning")]
    [SerializeField] private int _countPerSecond;
    [SerializeField] private int _maxCount;
    private int _particleIndex = 0;

    [Header("Loop")]
    // [SerializeField] private bool _maintain;
    [SerializeField] private bool _stopped;
    [SerializeField] private float _lastEnable;
    [SerializeField] private Vector2 _defaultDurationRange;

    public Vector2 DefaultDurationRange { get => _defaultDurationRange; set => _defaultDurationRange = value; }


    public void Update()
    {
        if (_countPerSecond > _maxCount || _particles == null || _particles.Length == 0 || _stopped)
        {
            return;
        }

        float now = Now();

        if (now - _lastEnable >= 1f / _countPerSecond)
        {
            _lastEnable = now; // Reset timer

            ActivateParticle(now);
        }
    }

    public void ResetTimer()
    {
        _lastEnable = Now();

        if (_particles == null || _particles.Length == 0)
        {
            _particles = GetComponentsInChildren<Renderer>(); //TODO: Set renderer in prefab to save performances
        }

        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].gameObject.SetActive(false);
        }

        //Initial count
        if (_countPerSecond > _maxCount)
        {
            for (int i = 0; i < _maxCount; i++)
            {
                ActivateParticle(_lastEnable);
            }
        }

        _stopped = false;
        // if (_maintain)
        // {
        //     for (int i = 0; i < _maxCount; i++)
        //     {
        //         _particles[i].material.SetVector("_LifetimeRange", Vector2.one * 120f);
        //     }
        // }
    }

    // public void Stop()
    // {
    //     _stopped = true;
    //     if (_maintain)
    //     {
    //         _particles[_particleIndex].material.SetFloat("_StartTime", Now());
    //         _particles[_particleIndex].material.SetVector("_LifetimeRange", _defaultDurationRange);
    //     }
    // }

    // // Set a fixed duration based on the player castend
    // public void SetDuration(float duration)
    // {
    //     _defaultDurationRange = new Vector2(duration, duration);
    //     _particles[_particleIndex].material.SetVector("_LifetimeRange", _defaultDurationRange);
    // }

    private void ActivateParticle(float now)
    {
        if (_particleIndex >= _maxCount)
        {
            _particleIndex = 0;
        }

        _particles[_particleIndex].gameObject.SetActive(true);
        _particles[_particleIndex].material.SetFloat("_StartTime", now);
        float seed = Random.Range(-100f, 100f);
        _particles[_particleIndex].material.SetFloat("_Seed", seed);
        _particles[_particleIndex].material.SetVector("_SurfaceNormals", SurfaceNormal);

        _particleIndex++;
    }

    private float Now()
    {
#if UNITY_EDITOR
        float now = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
#else
        float now = Time.time;
#endif
        return now;
    }
}
