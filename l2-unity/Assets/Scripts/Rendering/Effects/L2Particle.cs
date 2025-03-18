using UnityEngine;

public class L2Particle : MonoBehaviour
{
    [SerializeField] private Transform _owner;
    [SerializeField] private Vector3 _surfaceNormal;
    [SerializeField] private PooledEffect _pooledEffect;
    [SerializeField] private ParticleGroup[] _particleGroups;

    public PooledEffect PooledEffect { get { return _pooledEffect; } }
    public Vector3 SurfaceNormal { get { return _surfaceNormal; } set { _surfaceNormal = value; } }

    private void Awake()
    {
        _pooledEffect.ResetTimerCallback = () =>
        {
            ResetTimer();
        };
    }

    public void ResetTimer()
    {
        if (_particleGroups == null || _particleGroups.Length == 0)
        {
            _particleGroups = GetComponentsInChildren<ParticleGroup>();
        }

        for (int i = 0; i < _particleGroups.Length; i++)
        {
            if (_owner != null)
            {
                _particleGroups[i].OwnerPosition = _owner.position;
            }
            _particleGroups[i].SurfaceNormal = _surfaceNormal;
            _particleGroups[i].ResetTimer(_pooledEffect.HitTime);
        }
    }
}
