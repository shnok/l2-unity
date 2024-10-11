using UnityEngine;

[System.Serializable]
public class PooledEffect
{
    [SerializeField] private float _startTime;
    [SerializeField] private float _effectDurationSec;
    [SerializeField] private float _maximumInactiveTimeSec;
    [SerializeField] private GameObject _gameObject;
    public float StartTime { get { return _startTime; } set { _startTime = value; } }
    public float EffectDurationSec { get { return _effectDurationSec; } set { _effectDurationSec = value; } }
    public float MaximumInactiveTimeSec { get { return _maximumInactiveTimeSec; } set { _maximumInactiveTimeSec = value; } }
    public GameObject GameObject { get { return _gameObject; } set { _gameObject = value; } }
}