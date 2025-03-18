using UnityEngine;

[ExecuteInEditMode]
public class L2Particle : MonoBehaviour
{
    [SerializeField] private Transform _owner;
    [SerializeField] private Vector3 _surfaceNormal;
    [SerializeField] private PooledEffect _pooledEffect; //TODO: Set values in prefab to save performances
    [SerializeField] private ParticleGroup[] _particleGroups;

    public PooledEffect PooledEffect { get { return _pooledEffect; } }
    public Vector3 SurfaceNormal { get { return _surfaceNormal; } set { _surfaceNormal = value; } }

    void Start()
    {
    }

    void OnEnable()
    {
        ResetTimer();
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
            _particleGroups[i].ResetTimer();
        }
    }
}
