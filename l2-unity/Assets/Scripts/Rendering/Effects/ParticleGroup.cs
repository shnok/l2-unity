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
    [SerializeField] private bool _hasCastDuration; // does it it need a lifetime equal to the cast time
    [SerializeField] private float _duration = 5f; // should be as long as cast duration
    [SerializeField] private bool _instantKillAtCastEnd;
    private bool _stopped;
    private float _lastEnable;
    private float _lastLoop;

    public void Update()
    {
        if (_stopped)
        {
            return;
        }

        float now = Now();
        if (now - _lastEnable > _duration && _hasCastDuration)
        {
            _stopped = true;

            if (_instantKillAtCastEnd)
            {
                for (int i = 0; i < _particles.Length; i++)
                {
                    _particles[i].gameObject.SetActive(false);
                }
            }

            return;
        }

        if (_countPerSecond > _maxCount || _particles == null || _particles.Length == 0)
        {
            return;
        }

        if (now - _lastLoop >= 1f / _countPerSecond)
        {
            _lastLoop = now; // Reset timer

            ActivateParticle(now);
        }
    }

    public void ResetTimer(float duration)
    {
        _lastEnable = Now();
        _duration = duration;

        if (_particles == null || _particles.Length == 0)
        {
            _particles = GetComponentsInChildren<Renderer>();
        }

        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].gameObject.SetActive(false);
        }

        //Some effects have their particles fully spawned at startup
        if (_countPerSecond > _maxCount)
        {
            for (int i = 0; i < _maxCount; i++)
            {
                if (_hasCastDuration)
                {
                    foreach (Material m in _particles[i].materials)
                    {
                        float initialDelay = m.GetVector("_InitialDelayRange").y;
                        m.SetVector("_LifetimeRange", Vector2.one * _duration + Vector2.one * initialDelay);
                    }
                }

                ActivateParticle(_lastEnable);
            }
        }

        _stopped = false;
    }

    private void ActivateParticle(float now)
    {
        if (_particleIndex >= _maxCount)
        {
            _particleIndex = 0;
        }

        _particles[_particleIndex].gameObject.SetActive(true);

        float seed = Random.Range(-100f, 100f);
        foreach (Material m in _particles[_particleIndex].materials)
        {
            m.SetFloat("_StartTime", now);
            m.SetFloat("_Seed", seed);
            m.SetVector("_SurfaceNormals", SurfaceNormal);
        }

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
